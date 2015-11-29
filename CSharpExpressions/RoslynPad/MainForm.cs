// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Tests.Microsoft.CodeAnalysis.CSharp;

namespace RoslynPad
{
    public partial class MainForm : Form
    {
        private IDictionary<string, string> _programs = new Dictionary<string, string>
        {
            {
                "Constant",
                "(Expression<Func<int>>)(() => 42)"
            },
            {
                "Block",
                @"(Expression<Action>)(() =>
{
  // Add statements here
})"
            },
            {
                "Primes",
                @"(Expression<Func<int, List<int>>>)(max =>
{
  var res = new List<int>();

  for (var i = 2; i <= max; i++)
  {
    var hasDiv = false;

    for (var d = 2; d <= Math.Sqrt(i); d++)
    {
      if (i % d == 0)
      {
        hasDiv = true;
        break;
      }
    }

    if (!hasDiv)
    {
      res.Add(i);
    }
  }

  return res;
})"
            }
        };

        public MainForm()
        {
            InitializeComponent();

            cmbProgs.Items.AddRange(_programs.Keys.ToArray());
        }

        private void btnEval_Click(object sender, EventArgs e)
        {
            try
            {
                txtResult.ForeColor = Color.Black;
                txtResult.Text = TestUtilities.GetDebugView(txtCode.Text);
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
            }
        }
    }
}
