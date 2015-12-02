// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Tests.Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

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
                "(Expression<Func<string, int, int, string>>)((s, i, j) => s.Substring(length: j, startIndex: i))"
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
            trvExpr.Nodes.Clear();
            rtf.Clear();
            _diags = _diags.Clear();
            prgNode.SelectedObject = null;
            txtNode.Text = "";

            var sem = default(SemanticModel);
            try
            {
                txtResult.ForeColor = Color.Black;
                _eval = (LambdaExpression)TestUtilities.Eval(txtCode.Text, out sem, includingExpressions: chkModern.Checked, trimCR: true);
                UpdateExpression(_eval);
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

            if (sem != null)
            {
                Highlight(sem);
            }
        }

        private void Highlight(SemanticModel sem)
        {
            var ws = new AdhocWorkspace();

            var txt = sem.SyntaxTree.GetText();
            var src = txt.ToString();

            rtf.AppendText(src);

            var start = 0;
            var length = src.Length;

            var res = Classifier.GetClassifiedSpans(sem, TextSpan.FromBounds(start, start + length), ws).ToArray();

            foreach (var span in res)
            {
                rtf.Select(span.TextSpan.Start - start, span.TextSpan.Length);
                if (span.ClassificationType == "keyword")
                {
                    rtf.SelectionColor = Color.Blue;
                }
                else if (span.ClassificationType.EndsWith("name"))
                {
                    rtf.SelectionColor = Color.DarkCyan;
                }
                else if (span.ClassificationType.StartsWith("string"))
                {
                    rtf.SelectionColor = Color.DarkRed;
                }
                else if (span.ClassificationType == "comment")
                {
                    rtf.SelectionColor = Color.DarkGreen;
                }
            }

            _diags = sem.GetDiagnostics();
            foreach (var diag in _diags)
            {
                if (diag.Severity == DiagnosticSeverity.Error)
                {
                    var span = diag.Location.SourceSpan;
                    rtf.Select(span.Start - start, span.Length);
                    rtf.SelectionColor = Color.Red;
                }
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

            var treeWidth = pnlTree.Width;
            pnlTree.SplitterDistance = treeWidth / 2;

            var detailHeight = pnlDetail.Height;
            pnlDetail.SplitterDistance = detailHeight / 2;
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
                var reduced = new Reducer().Visit(_eval);
                UpdateExpression(reduced);
            }
            catch (Exception ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.ToString();
            }
        }

        private void UpdateExpression(Expression expr)
        {
            trvExpr.Nodes.Clear();
            prgNode.SelectedObject = null;
            _ids.Clear();
            txtNode.Text = "";
            txtResult.Text = expr.DebugView().ToString();

            var root = new TreeNode();
            Expand(root, expr);
            trvExpr.Nodes.Add(root);

            trvExpr.ExpandAll();
            trvExpr.SelectedNode = root;
        }

        private int GetId(object o)
        {
            var res = default(int);
            if (!_ids.TryGetValue(o, out res))
            {
                res = _ids.Count;
                _ids.Add(o, res);
            }

            return res;
        }

        private static Dictionary<ExpressionType, Color> s_exprColors = new Dictionary<ExpressionType, Color>
        {
            { ExpressionType.Lambda, Color.DarkRed },
            { ExpressionType.Constant, Color.DarkBlue },
            { ExpressionType.Parameter, Color.DarkGreen },
            { ExpressionType.Block, Color.DarkMagenta },
        };

        private static Dictionary<CSharpExpressionType, Color> s_csharpColors = new Dictionary<CSharpExpressionType, Color>
        {
            { CSharpExpressionType.AsyncLambda, Color.DarkRed },
            { CSharpExpressionType.Block, Color.DarkMagenta },
            { CSharpExpressionType.Await, Color.DarkOrange },
        };

        private void Expand(TreeNode node, object o)
        {
            node.Tag = o;

            if (o == null)
            {
                return;
            }

            var expr = o as Expression;
            if (expr != null)
            {
                var label = "";

                var csexpr = expr as CSharpExpression;
                if (csexpr != null)
                {
                    label = csexpr.CSharpNodeType.ToString();

                    var color = default(Color);
                    if (s_csharpColors.TryGetValue(csexpr.CSharpNodeType, out color))
                    {
                        node.ForeColor = color;
                    }
                }
                else
                {
                    label = expr.NodeType.ToString();

                    var color = default(Color);
                    if (s_exprColors.TryGetValue(expr.NodeType, out color))
                    {
                        node.ForeColor = color;
                    }
                }

                node.Text = string.IsNullOrEmpty(node.Text) ? label : node.Text + " - " + label;
            }
            else
            {
                node.Text = o.GetType().Name;
            }

            if (o != null)
            {
                ConcatIf<ParameterExpression>(o, node, p => p.Name);
                ConcatIf<LabelTarget>(o, node, l => l.Name);
                ConcatIf<NewExpression>(o, node, n => (n.Constructor?.ToString() ?? ".ctor()").Replace(".ctor", n.Type.ToString()));
                ConcatIf<NewCSharpExpression>(o, node, n => (n.Constructor?.ToString() ?? ".ctor()").Replace(".ctor", n.Type.ToString()));
                ConcatIf<MethodCallExpression>(o, node, c => c.Method.ToString());
                ConcatIf<MethodCallCSharpExpression>(o, node, c => c.Method.ToString());
                ConcatIf<MemberExpression>(o, node, m => m.Member.ToString());
                ConcatIf<LambdaExpression>(o, node, l => l.Type.ToString());
                ConcatIf<AsyncLambdaCSharpExpression>(o, node, l => l.Type.ToString());
                ConcatIf<BlockExpression>(o, node, b => b.Type.ToString());
                ConcatIf<TypeBinaryExpression>(o, node, t => t.TypeOperand.ToString());
                ConcatIf<UnaryExpression>(o, node, u => u.Type.ToString(), u => u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.ConvertChecked);
                ConcatIf<BinaryDynamicCSharpExpression>(o, node, d => d.OperationNodeType.ToString());
                ConcatIf<UnaryDynamicCSharpExpression>(o, node, d => d.OperationNodeType.ToString());
                ConcatIf<GetMemberDynamicCSharpExpression>(o, node, d => d.Name);
                ConcatIf<InvokeMemberDynamicCSharpExpression>(o, node, d => d.Name);
                ConcatIf<InvokeConstructorDynamicCSharpExpression>(o, node, d => d.Type.Name);
                ConcatIf<ConstantExpression>(o, node, d => d.Value?.ToString() ?? "null");
                ConcatIf<GotoExpression>(o, node, g => g.Kind.ToString());
                ConcatIf<GotoCSharpStatement>(o, node, g => g.Kind.ToString());

                if (o is ParameterExpression || o is LabelTarget || o is ConditionalReceiver)
                {
                    node.Text += " [$" + GetId(o) + "]";
                }

                var type = o.GetType();

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => Rank(p.Name)))
                {
                    if (o is BlockExpression && prop.Name == "Result")
                    {
                        continue;
                    }

                    var propType = prop.PropertyType;

                    if (propType.IsClass && (propType.Namespace.StartsWith("System.Linq.Expressions") || propType.Namespace.StartsWith("Microsoft.CSharp.Expressions")))
                    {
                        var propVal = prop.GetValue(o);

                        var child = new TreeNode(prop.Name);
                        child.Tag = propVal;

                        if (propVal != null)
                        {
                            Expand(child, propVal);
                        }
                        else
                        {
                            child.ForeColor = Color.DarkGray;
                        }

                        node.Nodes.Add(child);
                    }
                    else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
                    {
                        var elemType = prop.PropertyType.GetGenericArguments()[0];
                        if (elemType.Namespace.StartsWith("System.Linq.Expressions") || elemType.Namespace.StartsWith("Microsoft.CSharp.Expressions"))
                        {
                            var propVal = prop.GetValue(o);

                            var child = new TreeNode(prop.Name);
                            child.Tag = propVal;

                            if (propVal != null)
                            {
                                //child.ForeColor = Color.DarkBlue;

                                var lst = (IEnumerable)propVal;

                                foreach (var element in lst)
                                {
                                    var value = new TreeNode();
                                    Expand(value, element);
                                    child.Nodes.Add(value);
                                }
                            }
                            else
                            {
                                child.ForeColor = Color.DarkGray;
                            }

                            node.Nodes.Add(child);
                        }
                    }
                }
            }
        }

        private void ConcatIf<T>(object o, TreeNode node, Func<T, string> f, Func<T, bool> p = null)
        {
            if (o is T)
            {
                var t = (T)o;
                if (p?.Invoke(t) ?? true)
                {
                    node.Text += " (" + f(t) + ")";
                }
            }
        }

        private Dictionary<object, int> _ids = new Dictionary<object, int>();

        private static readonly Dictionary<string, int> s_ranks = new Dictionary<string, int>
        {
            // TODO: Add more

            // Lambda
            { "Parameters", 0 },
            { "Body", 4 }, // also used by CatchBlock, Loop, Using, Lock, For

            // Binary
            { "Left", 0 },
            { "Right", 1 },
            { "Conversion", 2 }, // also used by ForEach
            { "LeftConversion", 3 },
            { "FinalConversion", 4 },

            // Call, Invoke, Member, Index
            { "Expression", 0 },
            { "Object", 0 },
            { "Arguments", 1 },

            // Try
            { "Handlers", 5 }, // > Body
            { "Finally", 6 },
            { "Fault", 7 },

            // Block
            { "ReturnLabel", -1 },
            { "Variables", 0 },
            { "Expressions", 1 },
            { "Statements", 1 },

            // CatchBlock
            { "Filter", 1 },

            // Conditional
            { "Test", 2 }, // also used by For
            { "IfTrue", 3 },
            { "IfFalse", 4 },

            // Label
            { "Target", 0 },
            { "DefaultValue", 1 },

            // ListInit, MemberInit
            { "NewExpression", 0 },
            { "Bindings", 1 },
            { "Initializers", 1 }, // also used by For

            // Loop
            { "BreakLabel", -1 },
            { "ContinueLabel", 0 },

            // Switch
            { "SwitchValue", 1 }, // > BreakLabel, > Variables
            { "Cases", 2 },
            { "DefaultBody", 3 },

            // Using
            { "Variable", 0 },
            { "Resource", 1 },

            // ForEach
            { "Collection", 1 }, // > Variable

            // For
            { "Iterators", 3  }, // > Test, < Body

            // ConditionalAccess
            { "Receiver", 0 },
            { "NonNullReceiver", 1 },
            { "WhenNotNull", 2 },
        };

        private ImmutableArray<Diagnostic> _diags;

        private static int Rank(string name)
        {
            var res = default(int);
            if (s_ranks.TryGetValue(name, out res))
            {
                return res;
            }

            return 99;
        }

        class Reducer : ExpressionVisitor
        {
        }

        private void trvExpr_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = e.Node.Tag;

            prgNode.SelectedObject = tag;
            txtNode.Text = "";

            // TODO: Support tear-off nodes a la ElementInit.
            var expr = tag as Expression;
            if (expr != null)
            {
                txtNode.Text = expr.DebugView().ToString();
            }

            if (tag is ParameterExpression || tag is LabelTarget || tag is ConditionalReceiver)
            {
                ForEachNode(n => // NB: Doesn't do scope tracking; just a visual hint to aid in analysis
                {
                    if (n.Tag == tag)
                    {
                        n.BackColor = Color.Yellow;
                    }
                    else
                    {
                        n.BackColor = trvExpr.BackColor;
                    }
                });
            }
            else if (tag is BlockExpression && ((BlockExpression)tag).Type != typeof(void))
            {
                var result = default(TreeNode);

                ForEachNode(n =>
                {
                    if (n == e.Node)
                    {
                        var exprs = n.Nodes[n.Nodes.Count - 1];
                        if (exprs.Nodes.Count > 0)
                        {
                            result = exprs.Nodes[exprs.Nodes.Count - 1];
                        }
                    }

                    if (n == result)
                    {
                        n.BackColor = Color.SpringGreen;
                    }
                    else
                    {
                        n.BackColor = trvExpr.BackColor;
                    }
                });
            }
            else if (tag is BlockCSharpExpression)
            {
                var ret = ((BlockCSharpExpression)tag).ReturnLabel;

                ForEachNode(n =>
                {
                    var lbl = n.Tag as GotoExpression;
                    if (lbl?.Target == ret)
                    {
                        n.BackColor = Color.SpringGreen;
                    }
                    else
                    {
                        n.BackColor = trvExpr.BackColor;
                    }
                });
            }
            else
            {
                ForEachNode(n =>
                {
                    n.BackColor = trvExpr.BackColor;
                });
            }
        }

        private void ForEachNode(Action<TreeNode> visit)
        {
            var nodes = new Queue<TreeNode>();

            foreach (TreeNode node in trvExpr.Nodes)
            {
                nodes.Enqueue(node);
            }

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                visit(node);
                
                foreach (TreeNode child in node.Nodes)
                {
                    nodes.Enqueue(child);
                }
            }
        }

        private void rtf_MouseMove(object sender, MouseEventArgs e)
        {
            var i = rtf.GetCharIndexFromPosition(e.Location);
            if (i >= 0)
            {
                if (!_diags.IsDefaultOrEmpty)
                {
                    var diags = _diags.Where(d => d.Location.SourceSpan.Contains(i)).ToArray();
                    if (diags.Length > 0)
                    {
                        toolTip.Show(string.Join("\r\n", diags.Select(d => d.ToString())), rtf);
                    }
                }
            }
        }
    }
}
