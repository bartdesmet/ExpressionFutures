// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace RoslynPad
{
    public partial class EvalForm : Form
    {
        private readonly LambdaExpression _function;
        private Argument[] _args;

        public EvalForm()
        {
            InitializeComponent();
        }

        public EvalForm(LambdaExpression function)
            : this()
        {
            _function = function;
        }

        private void EvalForm_Load(object sender, EventArgs e)
        {
            _args = _function.Parameters.Select(param => new Argument(param)).ToArray();
            dgvParams.DataSource = _args;
        }

        class Argument
        {
            public Argument(ParameterExpression parameter)
            {
                Parameter = parameter.Name;
                Type = parameter.Type;
                Value = $"default({ToCSharp(parameter.Type)})";
            }

            public string Parameter { get; }
            public string Value { get; set; }
            internal Type Type { get; }
        }

        private async void btnEval_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";

            var options =
                ScriptOptions.Default
                    .WithImports(
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Threading.Tasks"
                    );

            var n = _args.Length;
            var args = new object[n];
            for (var i = 0; i < n; i++)
            {
                var arg = _args[i];
                var type = arg.Type;
                var value = $"({ToCSharp(type)})({arg.Value})";

                try
                {
                    args[i] = await CSharpScript.EvaluateAsync(value, options);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when compiling value for parameter '{arg.Parameter}':\r\n\r\n{ex.Message}", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var f = default(Delegate);
            try
            {
                f = _function.Compile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when compiling expression:\r\n\r\n{ex.Message}", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var res = default(object);
            var cout = new StringWriter();
            var dbg = new DebugWriter(cout, AppendLive);
            try
            {
                Console.SetOut(dbg);
                res = f.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.InnerException.Message;

                _done = true;
                tabControl1.SelectedTab = tabResult;
                txtResult.SelectionStart = txtResult.SelectionStart = 0;
                txtResult.ScrollToCaret();

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when evaluating expression:\r\n\r\n{ex.Message}", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (res is Task task)
            {
                txtResult.Text = "Awaiting task...";

                try
                {
                    await task;

                    var txt = ObjectDumper.Write(res);
                    txtResult.Text = "Awaiting task... Done!\r\n" + txt + "\r\n\r\nConsole output:\r\n" + cout.ToString();

                    _done = true;
                    tabControl1.SelectedTab = tabResult;
                    txtResult.SelectionStart = txtResult.SelectionStart = 0;
                    txtResult.ScrollToCaret();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when evaluating expression:\r\n\r\n{ex.Message}", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                var txt = ObjectDumper.Write(res);

                txtResult.ForeColor = Color.Black;
                txtResult.Text = txt + "\r\n\r\nConsole output:\r\n" + cout.ToString();

                _done = true;
                tabControl1.SelectedTab = tabResult;
                txtResult.SelectionStart = txtResult.SelectionStart = 0;
                txtResult.ScrollToCaret();
            }
        }

        private bool _done;

        private void AppendLive(string value)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (!_done && tabControl1.SelectedTab != tabConsole)
                {
                    tabControl1.SelectedTab = tabConsole;
                }

                txtLive.AppendText(value);
            }));
        }

        private static string ToCSharp(Type type)
        {
            if (type.IsArray)
            {
                var rank = new string(',', type.GetArrayRank() - 1);
                return ToCSharp(type.GetElementType()) + "[" + rank + "]";
            }
            else if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                var defName = def.FullName.Substring(0, def.FullName.LastIndexOf('`'));
                var args = string.Join(", ", type.GetGenericArguments().Select(ToCSharp));
                return defName + "<" + args + ">";
            }
            else
            {
                return type.FullName;
            }
        }
    }

    class DebugWriter : TextWriter
    {
        private readonly StringWriter _sw;
        private readonly Action<string> _append;

        public DebugWriter(StringWriter sw, Action<string> append)
        {
            _sw = sw;
            _append = append;
        }

        public override Encoding Encoding => Console.Out.Encoding;

        public override void Write(char value)
        {
            _sw.Write(value);
            Append(value.ToString());
        }

        public override void Write(string value)
        {
            _sw.Write(value);
            Append(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _sw.Write(buffer, index, count);
            Append(new string(buffer, index, count));
        }

        private void Append(string value)
        {
            _append(value);
        }
    }
}
