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
                    MessageBox.Show($"Error when compiling value for parameter '{arg.Parameter}':\r\n\r\n{ex.Message}");
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
                MessageBox.Show($"Error when compiling expression:\r\n\r\n{ex.Message}");
                return;
            }

            var res = default(object);
            var cout = new StringWriter(); // TODO: Maybe have a TextWriter that dumps to the UI in real time.
            try
            {
                Console.SetOut(cout);
                res = f.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.InnerException.Message;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when evaluating expression:\r\n\r\n{ex.Message}");
                return;
            }

            var task = res as Task;
            if (task != null)
            {
                txtResult.Text = "Awaiting task...";

                try
                {
                    await task;

                    var txt = ObjectDumper.Write(res);
                    txtResult.Text = "Awaiting task... Done!\r\n" + txt + "\r\n\r\nConsole output:\r\n" + cout.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when evaluating expression:\r\n\r\n{ex.Message}");
                    return;
                }
            }
            else
            {
                var txt = ObjectDumper.Write(res);

                txtResult.ForeColor = Color.Black;
                txtResult.Text = txt + "\r\n\r\nConsole output:\r\n" + cout.ToString();
            }
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
}
