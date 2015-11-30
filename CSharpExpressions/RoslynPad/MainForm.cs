// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Tests.Microsoft.CodeAnalysis.CSharp;

namespace RoslynPad
{
    public partial class MainForm : Form
    {
        // TODO: Add features to automatically generate tests and repro cases from the tool's input
        //       and the outcome of the evaluation (maybe just generate a file with code fragments).

        private IDictionary<string, string> _programs = new Dictionary<string, string>
        {
            {
                "Constant",
                "(Expression<Func<int>>)(() => 42)"
            },
            {
                "Arithmetic",
                "(Expression<Func<int, int>>)(x => x * 2 + 1)"
            },
            {
                "Anonymous object",
                "(Expression<Func<object>>)(() => new { a = 1, b = 2 })"
            },
            {
                "Conditional access",
                "(Expression<Func<DateTimeOffset?, int?>>)(dt => dt?.Offset.Hours)"
            },
            {
                "Named parameters",
                "(Expression<Func<string, int, int, int>>)((s, i, j) => s.Substring(length: j, startIndex: i))"
            },
            {
                "Block",
                @"(Expression<Action>)(() =>
{
  // Add statements here
})"
            },
            {
                "Async",
                @"(Expression<Func<int, Task<int>>>)(async x =>
{
  await Task.Delay(1000);
  return 2 * await Task.FromResult(x);
})"
            },
            {
                "Primes",
                @"(Expression<Func<int, List<int>>>)(max =>
{
  var res = new List<int>();

  for (var i = 2; i <= max; i++)
  {
    Console.Write(i);

    var hasDiv = false;

    for (var d = 2; d <= Math.Sqrt(i); d++)
    {
      if (i % d == 0)
      {
        Console.WriteLine($"" has divisor {d}"");
        hasDiv = true;
        break;
      }
    }

    if (!hasDiv)
    {
      Console.WriteLine("" is prime"");
      res.Add(i);
    }
  }

  return res;
})"
            },
            {
                "Primes async",
                @"(Expression<Func<int, Task<List<int>>>>)(async max =>
{
  var res = new List<int>();

  for (var i = 2; i <= max; i++)
  {
    await Task.Delay(10);
    Console.Write(i);

    var hasDiv = false;

    for (var d = 2; d <= Math.Sqrt(i); d++)
    {
      if (i % d == 0)
      {
        Console.WriteLine($"" has divisor {d}"");
        hasDiv = true;
        break;
      }
    }

    if (!hasDiv)
    {
      Console.WriteLine("" is prime"");
      res.Add(i);
    }
  }

  return res;
})"
            },
            {
                "Dynamic",
                @"(Expression<Func<dynamic, dynamic>>)(d => d.Substring(1).Length * 2)"
            }
        };

        private LambdaExpression _eval;

        public MainForm()
        {
            InitializeComponent();

            cmbProgs.Items.AddRange(_programs.Keys.ToArray());
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            btnEval.Enabled = btnReduce.Enabled = false;
            txtResult.Text = "";

            try
            {
                txtResult.ForeColor = Color.Black;
                _eval = (LambdaExpression)TestUtilities.Eval(txtCode.Text);
                txtResult.Text = _eval.DebugView().ToString();
                btnEval.Enabled = btnReduce.Enabled = true;
            }
            catch (InvalidProgramException ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.Message;
            }
            catch (TargetInvocationException ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = "LIBRARY ERROR:\r\n\r\n" + ex.InnerException.Message;
            }
            catch (Exception ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = "COMPILER ERROR:\r\n\r\n" + ex.ToString();
            }
        }

        private void cmbProgs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var txt = (string)cmbProgs.SelectedItem;
            var expr = default(string);
            if (_programs.TryGetValue(txt, out expr))
            {
                txtCode.Text = expr;
                btnEval.Enabled = btnReduce.Enabled = false;
            }
        }

        private void btnEval_Click(object sender, EventArgs e)
        {
            new EvalForm(_eval).ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbProgs.SelectedIndex = 0;
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                txtCode.SelectAll();

                e.Handled = true;
                e.SuppressKeyPress = true; // disables the annoying sound effect
            }
        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                var reduced = new Reducer().Visit(_eval).DebugView();
                txtResult.Text = reduced.ToString();
            }
            catch (Exception ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.ToString();
            }
        }

        class Reducer : ExpressionVisitor
        {
        }
    }
}
