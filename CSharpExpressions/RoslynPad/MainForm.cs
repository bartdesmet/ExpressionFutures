// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using Tests.Microsoft.CodeAnalysis.CSharp;

namespace RoslynPad
{
    public partial class MainForm : Form
    {
        // TODO: Add features to automatically generate tests and repro cases from the tool's input
        //       and the outcome of the evaluation (maybe just generate a file with code fragments).

        private IDictionary<string, string> _programs = new Dictionary<string, string>();
        private LambdaExpression _eval;
        private string _currentCatalog;

        public MainForm()
        {
            InitializeComponent();

            RefreshCatalog();
        }

        private bool _userEditMode = true;

        private void RefreshCatalog()
        {
            _userEditMode = false;

            _current = null;
            _isNew = _isEditing = false;

            txtCode.Clear();
            ClearAll();

            cmbProgs.Items.Clear();
            cmbProgs.Items.Add(NewItem);
            cmbProgs.Items.AddRange(_programs.Keys.ToArray());

            if (cmbProgs.Items.Count > 0)
            {
                cmbProgs.SelectedIndex = 0;
            }

            _userEditMode = true;
        }

        private void ClearAll()
        {
            btnReduce.Enabled = mnuReduce.Enabled = false;
            btnOptimize.Enabled = mnuOptimize.Enabled = false;
            btnEval.Enabled = mnuEvaluate.Enabled = false;

            txtResult.Text = "";
            txtCSharp.Text = "";
            rtfIL.Clear();
            trvExpr.Nodes.Clear();
            rtf.Clear();
            prgNode.SelectedObject = null;
            txtNode.Text = "";

            _sem = default(SemanticModel);
            _diags = _diags.Clear();
            _lastTooltipPosition = -1;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void Compile()
        {
            ClearAll();

            try
            {
                txtResult.ForeColor = Color.Black;
                _eval = (LambdaExpression)TestUtilities.Eval(txtCode.Text, out _sem, includingExpressions: chkModern.Checked, trimCR: true);
                UpdateExpression(_eval);
                btnReduce.Enabled = mnuReduce.Enabled = true;
                btnOptimize.Enabled = mnuOptimize.Enabled = true;
                btnEval.Enabled = mnuEvaluate.Enabled = true;
            }
            catch (InvalidProgramException ex)
            {
                tabMain.SelectedTab = tabDebug;
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.Message;
                txtResult.SelectionStart = txtResult.SelectionLength = 0;
                txtResult.ScrollToCaret();
            }
            catch (TargetInvocationException ex)
            {
                tabMain.SelectedTab = tabDebug;
                txtResult.ForeColor = Color.Red;
                txtResult.Text = "LIBRARY ERROR:\r\n\r\n" + ex.InnerException.Message;
                txtResult.SelectionStart = txtResult.SelectionLength = 0;
                txtResult.ScrollToCaret();
            }
            catch (Exception ex)
            {
                tabMain.SelectedTab = tabDebug;
                txtResult.ForeColor = Color.Red;
                txtResult.Text = "COMPILER ERROR:\r\n\r\n" + ex.ToString();
                txtResult.SelectionStart = txtResult.SelectionLength = 0;
                txtResult.ScrollToCaret();
            }

            HighlightCSharp();
        }

        private void HighlightCSharp()
        {
            if (_sem == null)
            {
                return;
            }

            var ws = new AdhocWorkspace();

            var txt = _sem.SyntaxTree.GetText();
            var src = txt.ToString();

            rtf.Clear();
            rtf.AppendText(src);

            var start = 0;
            var length = src.Length;

            var res = Classifier.GetClassifiedSpans(_sem, TextSpan.FromBounds(start, start + length), ws).ToArray();

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

            _diags = _sem.GetDiagnostics();
            foreach (var diag in _diags)
            {
                if (diag.Severity == DiagnosticSeverity.Error)
                {
                    var span = diag.Location.SourceSpan;
                    rtf.Select(span.Start - start, span.Length);
                    rtf.SelectionColor = Color.Red;
                }
            }

            _lastTooltipPosition = -1;
        }

        private void HighlightIL(string il)
        {
            il = il.Replace("\r\n", "\n");

            rtfIL.Clear();
            rtfIL.AppendText(il);

            var start = 0;

            var res = ILClassifier.Classify(il);

            foreach (var span in res)
            {
                rtfIL.Select(span.TextSpan.Start - start, span.TextSpan.Length);
                if (span.ClassificationType == "keyword")
                {
                    rtfIL.SelectionColor = Color.Blue;
                }
                else if (span.ClassificationType == "instruction")
                {
                    rtfIL.SelectionColor = Color.DarkBlue;
                }
                else if (span.ClassificationType == "labelRef")
                {
                    rtfIL.SelectionColor = Color.Red;
                }
                else if (span.ClassificationType == "labelDef")
                {
                    rtfIL.SelectionColor = Color.Red;
                }
                else if (span.ClassificationType == "labelDefUnused")
                {
                    rtfIL.SelectionColor = Color.DarkGray;
                }
                else if (span.ClassificationType == "string")
                {
                    rtfIL.SelectionColor = Color.DarkRed;
                }
                else if (span.ClassificationType == "type")
                {
                    rtfIL.SelectionColor = Color.DarkCyan;
                }
                else if (span.ClassificationType == "assembly")
                {
                    rtfIL.SelectionColor = Color.DarkSlateBlue;
                }
                //else if (span.ClassificationType == "namespace")
                //{
                //    rtfIL.SelectionColor = Color.DarkGray;
                //}
                else if (span.ClassificationType == "comment")
                {
                    rtfIL.SelectionColor = Color.DarkGreen;
                }
            }
        }

        private void UpdateIL(LambdaExpression expr)
        {
            if (expr != null)
            {
                try
                {
                    HighlightIL(expr.Compile().GetMethodIL());
                }
                catch (Exception ex)
                {
                    rtf.AppendText(ex.ToString());
                }
            }
        }

        private bool _ignoreIndexChange;

        private void cmbProgs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreIndexChange)
            {
                return;
            }

            _userEditMode = false;

            var txt = (string)cmbProgs.SelectedItem;
            var expr = default(string);
            if (_programs.TryGetValue(txt, out expr))
            {
                if (_isEditing && _current != null)
                {
                    var res = MessageBox.Show("Save edits to the current code fragment?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel)
                    {
                        _ignoreIndexChange = true;
                        cmbProgs.SelectedItem = _current;
                        _ignoreIndexChange = false;
                        return;
                    }

                    if (res == DialogResult.Yes)
                    {
                        _programs[_current] = txtCode.Text;
                        _dirty = true;
                    }
                }

                if (_isNew && txtCode.Text != "")
                {
                    var res = MessageBox.Show("Save the current code fragment?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel)
                    {
                        _ignoreIndexChange = true;
                        cmbProgs.SelectedItem = _current;
                        _ignoreIndexChange = false;
                        return;
                    }

                    if (res == DialogResult.Yes)
                    {
                        AddFragment(dontSelect: true);
                    }
                }

                txtCode.Text = expr;
                btnReduce.Enabled = mnuReduce.Enabled = false;
                btnOptimize.Enabled = mnuOptimize.Enabled = false;
                btnEval.Enabled = mnuEvaluate.Enabled = false;

                _isNew = false;
                _isEditing = false;
            }
            else if (txt == NewItem)
            {
                txtCode.Text = "";
                btnReduce.Enabled = mnuReduce.Enabled = false;
                btnOptimize.Enabled = mnuOptimize.Enabled = false;
                btnEval.Enabled = mnuEvaluate.Enabled = false;

                _isEditing = false;
                _isNew = true;
            }

            _userEditMode = true;
            _current = txt;
        }

        private string _current;

        private void btnEval_Click(object sender, EventArgs e)
        {
            Evaluate();
        }

        private void Evaluate()
        {
            new EvalForm(_eval).ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("DefaultCatalog.xml"))
            {
                LoadCatalog("DefaultCatalog.xml");
            }

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
            Reduce();
        }

        private void Reduce()
        {
            try
            {
                // NB: Substituting in order to make subsequent Optimize operate on the reduced
                //     expression (until Reduce implies optimization in future iterations).
                _eval = new Reducer().VisitAndConvert(_eval, nameof(Reduce));
                UpdateExpression(_eval);
            }
            catch (Exception ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.ToString();
                txtCSharp.Text = "";
                rtfIL.Clear();
            }
        }

        private void UpdateExpression(LambdaExpression expr)
        {
            trvExpr.Nodes.Clear();
            prgNode.SelectedObject = null;
            _ids.Clear();
            txtNode.Text = "";
            txtResult.Text = expr.DebugView().ToString();

            try
            {
                txtCSharp.ForeColor = Color.Black;
                txtCSharp.Text = expr.ToCSharp(); // TODO: specify namespaces
            }
            catch (Exception ex)
            {
                txtCSharp.ForeColor = Color.Red;
                txtCSharp.Text = ex.ToString();
            }

            var root = new TreeNode();
            Expand(root, expr);
            trvExpr.Nodes.Add(root);

            trvExpr.ExpandAll();
            trvExpr.SelectedNode = root;

            UpdateIL(expr);
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
        private SemanticModel _sem;

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

        private int _lastTooltipPosition;

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
                        if (_lastTooltipPosition != i)
                        {
                            _lastTooltipPosition = i;
                            toolTip.Active = true;
                            toolTip.Show(string.Join("\r\n", diags.Select(d => d.ToString())), rtf);
                        }
                    }
                    else
                    {
                        _lastTooltipPosition = -1;
                        toolTip.Active = false;
                        toolTip.Hide(this);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new OptionsForm
            {
                EditorFont = txtCode.Font,
                SyntaxFont = rtf.Font,
                DebugViewFont = txtResult.Font,
                TreeFont = trvExpr.Font,
                DetailsFont = prgNode.Font,
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                txtCode.Font = frm.EditorFont;
                rtf.Font = frm.SyntaxFont;
                txtCSharp.Font = frm.SyntaxFont;
                rtfIL.Font = frm.SyntaxFont;
                txtResult.Font = frm.DebugViewFont;
                txtNode.Font = frm.DebugViewFont;
                trvExpr.Font = frm.TreeFont;
                prgNode.Font = frm.DetailsFont;
                HighlightCSharp();
                UpdateIL(_eval);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckSave())
            {
                return;
            }

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var file = openFile.FileName;
                if (File.Exists(file))
                {
                    LoadCatalog(file, reportError: true);
                }
            }
        }

        private bool CheckSave()
        {
            if (_dirty)
            {
                var res = MessageBox.Show("Save changes to current catalog?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    return Save();
                }
                else if (res == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private bool Save()
        {
            if (_currentCatalog != null)
            {
                return SaveToFile(_currentCatalog);
            }
            else
            {
                return SaveAs();
            }
        }

        private bool SaveAs()
        {
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                var res = SaveToFile(saveFile.FileName);

                if (res)
                {
                    _currentCatalog = saveFile.FileName;
                }

                return res;
            }

            return false;
        }

        private bool SaveToFile(string file)
        {
            try
            {
                var exprs = _programs.Select(kv => new XElement("Expression", new XAttribute("Name", kv.Key), new XElement("Code", new XCData(kv.Value))));
                var root = new XElement("Expressions", exprs);
                var doc = new XDocument(root);
                doc.Save(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void LoadCatalog(string file, bool reportError = false)
        {
            try
            {
                var fragments = new Dictionary<string, string>();

                var doc = XDocument.Load(file);
                var root = doc.Element("Expressions");
                if (root != null)
                {
                    foreach (var expression in root.Elements("Expression"))
                    {
                        var name = expression.Attribute("Name")?.Value;
                        var code = expression.Element("Code");

                        if (name != null && code != null)
                        {
                            var csharp = (code.Value ?? "").Replace("\n", "\r\n");
                            fragments.Add(name, csharp);
                        }
                    }
                }

                _programs.Clear();
                _programs = fragments;

                RefreshCatalog();

                _currentCatalog = file;
            }
            catch (Exception ex)
            {
                if (reportError)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mnuEvaluate_Click(object sender, EventArgs e)
        {
            Evaluate();
        }

        private void mnuReduce_Click(object sender, EventArgs e)
        {
            Reduce();
        }

        private void mnuCompile_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void mnuRun_Click(object sender, EventArgs e)
        {
            Compile();

            if (btnEval.Enabled)
            {
                Evaluate();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddFragment();
        }

        private void AddFragment(bool dontSelect = false)
        {
            var dlg = new AddSnippetDialog(_programs);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!_programs.ContainsKey(dlg.SnippetName))
                {
                    _programs.Add(dlg.SnippetName, txtCode.Text);

                    _current = dlg.SnippetName;
                    _isEditing = false;
                    _isNew = false;

                    var i = cmbProgs.Items.Add(dlg.SnippetName);

                    if (!dontSelect)
                    {
                        _ignoreIndexChange = true;
                        cmbProgs.SelectedIndex = i;
                        _ignoreIndexChange = false;
                    }
                }
                else
                {
                    _programs[dlg.SnippetName] = txtCode.Text;
                }

                _dirty = true;
            }
        }

        private bool _dirty;

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            if (_userEditMode)
            {
                if ((string)cmbProgs.SelectedItem != NewItem)
                {
                    _isEditing = true;
                }
            }
        }

        private bool _isEditing;
        private bool _isNew;

        private const string NewItem = "(New)";

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckSave())
            {
                return;
            }

            _programs.Clear();
            RefreshCatalog();
            cmbProgs.SelectedIndex = 0;

            _dirty = false;
            _currentCatalog = null;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckSave())
            {
                e.Cancel = true;
            }
        }

        private void btnOptimize_Click(object sender, EventArgs e)
        {
            Optimize();
        }

        private void mnuOptimize_Click(object sender, EventArgs e)
        {
            Optimize();
        }

        private void Optimize()
        {
            try
            {
                // NB: Substituting in order to make subsequent Reduce operate on the optimized
                //     expression (until Reduce implies optimization in future iterations).
                _eval = (LambdaExpression)_eval.Optimize();
                UpdateExpression(_eval);
            }
            catch (Exception ex)
            {
                txtResult.ForeColor = Color.Red;
                txtResult.Text = ex.ToString();
                txtCSharp.Text = "";
                rtfIL.Clear();
            }
        }
    }
}
