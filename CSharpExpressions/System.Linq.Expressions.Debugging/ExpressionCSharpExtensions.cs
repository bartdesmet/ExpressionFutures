// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Linq.Expressions
{
    // NB: ToCSharp *tries* to provide a C# fragment that's semantically equivalent to the expression
    //     tree but should be used for debugging purposes *only*. While it tries to do a good job, it's
    //     fundamentally impossible to represent all expression trees as valid C# because they have a
    //     richer set of operations, so of which cannot be represented in C# without a lowering step
    //     which would take away the original tree shape. For debugging purposes, this is good enough
    //     though, much like Reflector and ILSpy provide a reasonable means to reverse engineer what's
    //     going on. Although these fundamental restrictions exist, we try to do our best to emit good
    //     and concise C# code, but there's a long tail of room for improvement should this become an
    //     often-used mechanism.

    /// <summary>
    /// Provides a set of extension methods to provide a C#-based debug view of an expression tree.
    /// </summary>
    public static class ExpressionCSharpExtensions
    {
        public static string ToCSharp(this Expression expression)
        {
            return ToCSharp(expression, "System", "System.Collections.Generic", "System.Linq", "System.Threading.Tasks");
        }

        public static string ToCSharp(this Expression expression, params string[] namespaces)
        {
            var sw = new StringWriter();
            var printer = new CSharpPrinter(sw, new HashSet<string>(namespaces));
            printer.Visit(expression);
            return sw.ToString();
        }

        class CSharpPrinter : ExpressionVisitor, ICSharpPrintingVisitor
        {
            private readonly TextWriter _writer;
            private readonly HashSet<string> _namespaces;
            private readonly Dictionary<object, int> _constants = new Dictionary<object, int>();
            private readonly Dictionary<object, string> _variables = new Dictionary<object, string>();
            private readonly Dictionary<object, string> _labels = new Dictionary<object, string>();
            private readonly Stack<Expression> _stack = new Stack<Expression>();
            private readonly Stack<LabelTarget> _breaks = new Stack<LabelTarget>();
            private readonly Stack<LabelTarget> _continues = new Stack<LabelTarget>();
            private string _indent = "";

            public CSharpPrinter(TextWriter writer, HashSet<string> namespaces)
            {
                _writer = writer;
                _namespaces = namespaces;
            }

            // TODO: improve analysis of `checked` so we can emit `unchecked` as well
            public bool InCheckedContext { get; set; }

            public void PushBreak(LabelTarget target)
            {
                _breaks.Push(target);
            }

            public void PushContinue(LabelTarget target)
            {
                _continues.Push(target);
            }

            public void PopBreak()
            {
                _breaks.Pop();
            }

            public void PopContinue()
            {
                _continues.Pop();
            }

            public override Expression Visit(Expression node)
            {
                var res = node;

                if (node != null)
                {
                    var stmtChild = IsStatement(node);
                    var stmtParent = false;

                    if (_stack.Count > 0)
                    {
                        stmtParent = IsStatement(_stack.Peek());
                    }

                    _stack.Push(node);

                    res = base.Visit(node);

                    _stack.Pop();

                    if (stmtParent && !stmtChild)
                    {
                        Out(";");
                    }
                }

                return res;
            }

            public Expression VisitExpression(Expression node)
            {
                _stack.Push(node);

                var res = base.Visit(node);

                _stack.Pop();

                return res;
            }

            protected virtual bool IsStatement(Expression node)
            {
                // TODO: refine
                switch (node.NodeType)
                {
                    case ExpressionType.Block:
                    case ExpressionType.Loop:
                    case ExpressionType.Switch:
                    case ExpressionType.Try:
                    case ExpressionType.Goto:
                    case ExpressionType.Label:
                        return true;
                    case ExpressionType.Conditional:
                        return ((ConditionalExpression)node).Type == typeof(void);
                    case ExpressionType.Default:
                        return ((DefaultExpression)node).Type == typeof(void);
                    case ExpressionType.Extension:
                        var csharp = node as ICSharpPrintableExpression;
                        if (csharp != null)
                        {
                            return csharp.IsStatement;
                        }
                        break;
                }

                return false;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                var value = node.Value;
                Literal(value, node.Type, allowCast: true);
                return node;
            }

            public void Literal(object value, Type type)
            {
                Literal(value, type, false);
            }

            private void Literal(object value, Type type, bool allowCast)
            {
                var needCast = false;
                var res = default(string);

                if (value == null)
                {
                    res = "null";
                    needCast = true;
                }
                else if (type == typeof(int))
                {
                    res = ((int)value).ToString(CultureInfo.InvariantCulture);
                }
                else if (type == typeof(uint))
                {
                    res = ((uint)value).ToString(CultureInfo.InvariantCulture) + "U";
                }
                else if (type == typeof(long))
                {
                    res = ((long)value).ToString(CultureInfo.InvariantCulture) + "L";
                }
                else if (type == typeof(ulong))
                {
                    res = ((ulong)value).ToString(CultureInfo.InvariantCulture) + "UL";
                }
                else if (type == typeof(byte))
                {
                    res = ((byte)value).ToString(CultureInfo.InvariantCulture);
                    needCast = true;
                }
                else if (type == typeof(sbyte))
                {
                    res = ((sbyte)value).ToString(CultureInfo.InvariantCulture);
                    needCast = true;
                }
                else if (type == typeof(short))
                {
                    res = ((short)value).ToString(CultureInfo.InvariantCulture);
                    needCast = true;
                }
                else if (type == typeof(ushort))
                {
                    res = ((ushort)value).ToString(CultureInfo.InvariantCulture);
                    needCast = true;
                }
                else if (type == typeof(float))
                {
                    res = ((float)value).ToString(CultureInfo.InvariantCulture) + "F";
                }
                else if (type == typeof(double))
                {
                    res = ((double)value).ToString(CultureInfo.InvariantCulture) + "D";
                }
                else if (type == typeof(decimal))
                {
                    res = ((decimal)value).ToString(CultureInfo.InvariantCulture) + "M";
                }
                else if (type == typeof(char))
                {
                    var ch = Escape((char)value);
                    res = $"'{ch}'";
                }
                else if (type == typeof(string))
                {
                    var str = Escape((string)value);
                    res = $"\"{str}\"";
                }
                else if (type == typeof(bool))
                {
                    res = (bool)value ? "true" : "false";
                }
                else if (type.IsEnum && type.IsEnumDefined(value))
                {
                    // TODO: could do better for flags enums
                    var name = Enum.GetName(type, value);
                    res = ToCSharp(type) + "." + name;
                }
                else
                {
                    var id = default(int);
                    if (!_constants.TryGetValue(value, out id))
                    {
                        id = _constants.Count;
                        _constants.Add(value, id);
                    }

                    res = "__c" + id;
                    needCast = type != value.GetType();
                }

                if (allowCast && needCast)
                {
                    Out($"({ToCSharp(type)}){res}");
                }
                else
                {
                    Out(res);
                }
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                if (node.Type == typeof(void))
                {
                    Out(";");
                }
                else
                {
                    var type = ToCSharp(node.Type);
                    Out($"default({type})");
                }

                return node;
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                // TODO: Analyze Conversion

                if (node.NodeType == ExpressionType.ArrayIndex)
                {
                    ParenthesizedVisit(node, node.Left);
                    Out("[");
                    Visit(node.Right);
                    Out("]");
                }
                else
                {
                    var op = CSharpLanguageHelpers.GetOperatorSyntax(node.NodeType);
                    var mtd = CSharpLanguageHelpers.GetClsMethodName(node.NodeType);
                    var isChecked = false;
                    var asMethod = false;

                    switch (node.NodeType)
                    {
                        case ExpressionType.AddChecked:
                        case ExpressionType.MultiplyChecked:
                        case ExpressionType.SubtractChecked:
                        case ExpressionType.AddAssignChecked:
                        case ExpressionType.MultiplyAssignChecked:
                        case ExpressionType.SubtractAssignChecked:
                            isChecked = true;
                            break;
                        case ExpressionType.Power:
                        case ExpressionType.PowerAssign:
                            asMethod = true;
                            break;
                    }

                    if (!asMethod && mtd != null && node.Method != null)
                    {
                        // TODO: can suppress method rendering when it involves String.Concat, Delegate.Combine, etc.
                        //       - for String.Concat we can take away spurious argument conversions as well
                        //       - for Delegate.* we can't easily get rid of the parent node's conversion to the delegate type unless we peek from VisitUnary

                        if (node.Method.Name != mtd) // TODO: check declaring type
                        {
                            asMethod = true;
                        }
                    }

                    if (asMethod)
                    {
                        // NB: Not preserving evaluation semantics for indexers etc.
                        if (op.EndsWith("="))
                        {
                            Visit(node.Left);
                            Out(" = ");
                        }

                        StaticMethodCall(node.Method, node.Left, node.Right);
                        return node;
                    }

                    var hasEnteredChecked = false;

                    if (!InCheckedContext && isChecked)
                    {
                        var scan = new CheckedOpScanner();

                        scan.Visit(node.Left);

                        if (!scan.HasUncheckedOperation)
                        {
                            scan.Visit(node.Right);
                        }

                        if (!scan.HasUncheckedOperation)
                        {
                            InCheckedContext = true;
                            hasEnteredChecked = true;

                            Out("checked(");
                        }
                        else
                        {
                            // NB: Produces invalid C#; we'd have to spill expressions into locals if we want
                            //     to emit valid C# in this case, at the expense of losing the tree shape.
                            op = $"/*checked(*/{op}/*)*/";
                            isChecked = false;
                        }

                        // NB: Could produce invalid C# if assign node is used as a statement; need to keep
                        //     track of the parent operation to determine this.
                    }

                    ParenthesizedVisit(node, node.Left);

                    Out($" {op} ");

                    ParenthesizedVisit(node, node.Right);

                    if (hasEnteredChecked)
                    {
                        InCheckedContext = false;
                        Out(")");
                    }
                }

                return node;
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                var nodeType = node.NodeType;

                if (node.NodeType == ExpressionType.Not && node.Type != typeof(bool) && node.Type != typeof(bool?))
                {
                    nodeType = ExpressionType.OnesComplement;
                }

                var op = CSharpLanguageHelpers.GetOperatorSyntax(nodeType);
                var mtd = CSharpLanguageHelpers.GetClsMethodName(nodeType);
                var isChecked = false;
                var isSuffix = false;
                var asMethod = false;

                switch (nodeType)
                {
                    case ExpressionType.ArrayLength:
                        ParenthesizedVisit(node, node.Operand);
                        Out(".Length");
                        return node;
                    case ExpressionType.Throw:
                        if (node.Operand == null)
                        {
                            Out("throw");
                        }
                        else
                        {
                            Out("throw ");
                            Visit(node.Operand);
                        }
                        return node;
                    case ExpressionType.Convert:
                        op = $"({ToCSharp(node.Type)})";
                        break;
                    case ExpressionType.ConvertChecked:
                        op = $"({ToCSharp(node.Type)})";
                        isChecked = true;
                        break;
                    case ExpressionType.Unbox:
                        op = "/* unbox */";
                        break;
                    case ExpressionType.TypeAs:
                        op = $" as {ToCSharp(node.Type)}";
                        isSuffix = true;
                        break;
                    case ExpressionType.IsTrue:
                        op = " == true"; // TODO: review
                        isSuffix = true;
                        break;
                    case ExpressionType.IsFalse:
                        op = " == false"; // TODO: review
                        isSuffix = true;
                        break;
                    case ExpressionType.Decrement:
                        op = " - 1"; // TODO: review; add type suffix
                        isSuffix = true;
                        break;
                    case ExpressionType.Increment:
                        op = " + 1"; // TODO: review; add type suffix
                        isSuffix = true;
                        break;
                    case ExpressionType.NegateChecked:
                        isChecked = true;
                        break;
                    case ExpressionType.PostDecrementAssign:
                    case ExpressionType.PostIncrementAssign:
                        isSuffix = true;
                        break;
                }

                if (node.Method != null)
                {
                    var name = node.Method.Name;

                    if (node.NodeType == ExpressionType.Convert || node.NodeType == ExpressionType.ConvertChecked)
                    {
                        if (name != "op_Explicit" && name != "op_Implicit")
                        {
                            asMethod = true;
                        }
                    }
                    else if (mtd != null)
                    {
                        if (name != mtd) // TODO: check declaring type
                        {
                            asMethod = true;
                        }
                    }
                }

                if (asMethod)
                {
                    // NB: Not preserving evaluation semantics for indexers etc.
                    if (op.Contains("++") || op.Contains("--"))
                    {
                        // TODO: doesn't deal with post/pre logic yet
                        Visit(node.Operand);
                        Out(" = ");
                    }

                    StaticMethodCall(node.Method, node.Operand);
                    return node;
                }

                var hasEnteredChecked = false;

                if (!InCheckedContext && isChecked)
                {
                    var scan = new CheckedOpScanner();

                    scan.Visit(node.Operand);

                    if (!scan.HasUncheckedOperation)
                    {
                        InCheckedContext = true;
                        hasEnteredChecked = true;

                        Out("checked(");
                    }
                    else
                    {
                        // NB: Produces invalid C#; we'd have to spill expressions into locals if we want
                        //     to emit valid C# in this case, at the expense of losing the tree shape.
                        op = $"/*checked(*/{op}/*)*/";
                        isChecked = false;
                    }

                    // NB: Could produce invalid C# if assign node is used as a statement; need to keep
                    //     track of the parent operation to determine this.
                }

                if (!isSuffix)
                {
                    Out(op);
                }

                ParenthesizedVisit(node, node.Operand);

                if (isSuffix)
                {
                    Out(op);
                }

                if (hasEnteredChecked)
                {
                    InCheckedContext = false;
                    Out(")");
                }

                return node;
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                if (node.Type == typeof(void))
                {
                    Out("if (");
                    VisitExpression(node.Test);
                    Out(")");

                    var hasElse = node.IfFalse.NodeType != ExpressionType.Default;

                    var ifTrueNeedsCurly = false;

                    //
                    // We require curly braces around the IfTrue branch if we could fall vicitim to
                    // the dangling else problem:
                    //
                    //   if (a)
                    //     if (b)
                    //       ...
                    //   else
                    //     ...
                    //
                    if (hasElse && node.IfTrue.NodeType == ExpressionType.Conditional)
                    {
                        var nested = (ConditionalExpression)node.IfTrue;
                        if (nested.IfFalse.NodeType == ExpressionType.Default)
                        {
                            ifTrueNeedsCurly = true;
                        }
                    }

                    VisitBlockLike(node.IfTrue, ifTrueNeedsCurly);

                    if (hasElse)
                    {
                        Out("else");

                        VisitBlockLike(node.IfFalse);
                    }
                }
                else
                {
                    ParenthesizedVisit(node, node.Test);
                    Out(" ? ");
                    ParenthesizedVisit(node, node.IfTrue);
                    Out(" : ");
                    ParenthesizedVisit(node, node.IfFalse);
                }

                return node;
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                ParenthesizedVisit(node, node.Object);

                Out("[");
                if (node.Indexer == null)
                {
                    Visit(node.Arguments);
                }
                else
                {
                    ArgsVisit(node.Arguments, node.Indexer.GetGetMethod(true)); // TODO: Indexer could have non-default name
                }
                Out("]");

                return node;
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                ParenthesizedVisit(node, node.Expression);

                Out("(");
                ArgsVisit(node.Arguments, node.Expression.Type.GetMethod("Invoke"));
                Out(")");

                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // TODO: Member could be on declaring type different from object type.

                if (node.Expression == null)
                {
                    Out(ToCSharp(node.Member.DeclaringType));
                    Out(".");
                    Out(node.Member.Name);
                }
                else
                {
                    ParenthesizedVisit(node, node.Expression);
                    Out(".");
                    Out(node.Member.Name);
                }

                return node;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // TODO: Method could be on declaring type different from object type.

                var startIndex = 0;

                if (node.Object != null)
                {
                    ParenthesizedVisit(node, node.Object);
                }
                else if (node.Method.IsDefined(typeof(ExtensionAttribute)) && _namespaces.Contains(node.Method.DeclaringType.Namespace) && node.Arguments.Count >= 1)
                {
                    ParenthesizedVisit(node, node.Arguments[0]);
                    startIndex = 1;
                }
                else
                {
                    Out(ToCSharp(node.Method.DeclaringType));
                }

                Out(".");
                Out(node.Method.Name);

                if (node.Method.IsGenericMethod && !CSharpLanguageHelpers.CanInferGenericArguments(node.Method))
                {
                    var genArgs = string.Join(", ", node.Method.GetGenericArguments().Select(ToCSharp));
                    Out("<");
                    Out(genArgs);
                    Out(">");
                }

                Out("(");
                ArgsVisit(node.Arguments, node.Method, startIndex);
                Out(")");

                return node;
            }

            protected override Expression VisitNew(NewExpression node)
            {
                Out("new ");

                if (node.Members != null || (node.Type.IsDefined(typeof(CompilerGeneratedAttribute)) && node.Type.Name.StartsWith("<>f__AnonymousType")))
                {
                    Out("{");

                    var n = node.Members?.Count ?? 0;
                    for (var i = 0; i < n; i++)
                    {
                        Out(" ");

                        var member = node.Members[i];
                        var arg = node.Arguments[i];

                        Out(member.Name);
                        Out(" = ");
                        Visit(arg);

                        if (i != n - 1)
                        {
                            Out(",");
                        }
                    }

                    Out(" }");
                }
                else
                {
                    Out(ToCSharp(node.Type));
                    Out("(");

                    if (node.Constructor != null)
                    {
                        ArgsVisit(node.Arguments, node.Constructor);
                    }

                    Out(")");
                }

                return node;
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                Out("new ");
                Out(ToCSharp(node.Type.GetElementType()));

                if (node.NodeType == ExpressionType.NewArrayBounds)
                {
                    Out("[");
                    Visit(node.Expressions);
                    Out("]");
                }
                else
                {
                    Out("[");
                    Out(new string(',', node.Type.GetArrayRank() - 1));
                    Out("]");

                    Out(" { ");
                    Visit(node.Expressions);
                    Out(" }");
                }

                return node;
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                Visit(node.NewExpression);

                Out(" {");

                var n = node.Initializers.Count;
                for (var i = 0; i < n; i++)
                {
                    Out(" ");

                    var init = node.Initializers[i];

                    VisitElementInit(init);

                    if (i != n - 1)
                    {
                        Out(",");
                    }
                }

                Out(" }");

                return node;
            }

            protected override ElementInit VisitElementInit(ElementInit node)
            {
                if (node.Arguments.Count > 1)
                {
                    Out("{ ");
                    Visit(node.Arguments);
                    Out(" }");
                }
                else
                {
                    Visit(node.Arguments[0]);
                }

                return node;
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                Visit(node.NewExpression);

                Out(" {");

                var n = node.Bindings.Count;
                for (var i = 0; i < n; i++)
                {
                    Out(" ");

                    var binding = node.Bindings[i];

                    VisitMemberBinding(binding);

                    if (i != n - 1)
                    {
                        Out(",");
                    }
                }

                Out(" }");

                return node;
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                Out(node.Member.Name);
                Out(" = ");
                Visit(node.Expression);

                return node;
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                Out(node.Member.Name);
                Out(" = {");

                var n = node.Initializers.Count;
                for (var i = 0; i < n; i++)
                {
                    Out(" ");

                    var init = node.Initializers[i];

                    VisitElementInit(init);

                    if (i != n - 1)
                    {
                        Out(",");
                    }
                }

                Out(" }");

                return node;
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                Out(node.Member.Name);
                Out(" = {");

                var n = node.Bindings.Count;
                for (var i = 0; i < n; i++)
                {
                    Out(" ");

                    var binding = node.Bindings[i];

                    VisitMemberBinding(binding);

                    if (i != n - 1)
                    {
                        Out(",");
                    }
                }

                Out(" }");

                return node;
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                if (node.NodeType == ExpressionType.TypeIs)
                {
                    ParenthesizedVisit(node, node.Expression);
                    Out(" is ");
                    Out(ToCSharp(node.TypeOperand));
                }
                else // TODO: review
                {
                    ParenthesizedVisit(node, node.Expression);
                    Out("?.GetType() == typeof(");
                    Out(ToCSharp(node.TypeOperand));
                    Out(")");
                }

                return node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // TODO: No guarantee delegate type can be inferred; add `new` every time or track info?

                Out("(");

                var n = node.Parameters.Count;
                for (var i = 0; i < n; i++)
                {
                    var parameter = node.Parameters[i];

                    if (parameter.IsByRef)
                    {
                        Out("ref ");
                    }

                    Out(ToCSharp(parameter.Type));
                    Out(" ");

                    var name = GetVariableName(parameter, true);
                    Out(name);

                    if (i != n - 1)
                    {
                        Out(", ");
                    }
                }

                Out(") =>");

                var isBlock = false;

                if (node.Body.NodeType == ExpressionType.Block)
                {
                    isBlock = true;
                }
                else
                {
                    var csharp = node.Body as ICSharpPrintableExpression;
                    if (csharp != null && csharp.IsBlock)
                    {
                        isBlock = true;
                    }
                }

                if (isBlock)
                {
                    NewLine();
                }
                else
                {
                    Out(" ");
                }

                Visit(node.Body);

                if (isBlock)
                {
                    NewLine();
                }

                return node;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                var name = GetVariableName(node);

                Out(name);

                return node;
            }

            protected override Expression VisitTry(TryExpression node)
            {
                Out("try");

                VisitBlockLike(node.Body, needsCurlies: true);

                foreach (var h in node.Handlers)
                {
                    VisitCatchBlock(h);
                }

                if (node.Finally != null)
                {
                    Out("finally");
                    VisitBlockLike(node.Finally, needsCurlies: true);
                }

                if (node.Fault != null) // NB: This is not valid C#.
                {
                    Out("fault");
                    VisitBlockLike(node.Fault, needsCurlies: true);
                }

                return node;
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                Out("catch");

                if (node.Variable != null)
                {
                    Out($" ({ToCSharp(node.Variable.Type)} {GetVariableName(node.Variable, true)})");
                }
                else if (node.Test != typeof(object))
                {
                    Out($" ({ToCSharp(node.Test)})");
                }

                if (node.Filter != null)
                {
                    Out(" when (");
                    VisitExpression(node.Filter);
                    Out(")");
                }

                VisitBlockLike(node.Body, needsCurlies: true);

                return node;
            }

            protected override Expression VisitBlock(BlockExpression node)
            {
                Out("{");
                Indent();
                NewLine();

                var vars = node.Variables.ToLookup(v => v.Type, v => v);
                foreach (var kv in vars)
                {
                    Out(ToCSharp(kv.Key));
                    Out(" ");
                    Out(string.Join(", ", kv.Select(v => GetVariableName(v, true))));
                    Out(";");
                    NewLine();
                }

                var n = node.Expressions.Count;
                for (var i = 0; i < n; i++)
                {
                    var expr = node.Expressions[i];

                    if (i == n - 1 && node.Type != typeof(void))
                    {
                        var hasExplicitReturn = false;

                        if (expr.NodeType == ExpressionType.Goto)
                        {
                            var gotoExpr = (GotoExpression)expr;
                            if (gotoExpr.Kind == GotoExpressionKind.Return)
                            {
                                hasExplicitReturn = true;
                            }
                        }

                        var hasImplicitReturn = false;

                        if (!hasExplicitReturn)
                        {
                            if (_stack.Count > 1)
                            {
                                var parent = _stack.ElementAt(1);
                                if (parent.NodeType == ExpressionType.Lambda)
                                {
                                    hasImplicitReturn = true;
                                }
                                else
                                {
                                    var csharp = parent as ICSharpPrintableExpression;
                                    if (csharp != null && csharp.IsLambda)
                                    {
                                        hasImplicitReturn = true;
                                    }
                                }
                            }
                        }

                        if (!hasExplicitReturn)
                        {
                            Out(hasImplicitReturn ? "return " : "/*return*/ ");
                        }

                        VisitExpression(expr); // NB: Not valid C# but trying to denote the last statement as an expression by lack of ;

                        if (!hasExplicitReturn)
                        {
                            Out(hasImplicitReturn ? ";" : "/*;*/");
                        }
                    }
                    else
                    {
                        Visit(expr);
                    }

                    if (i != n - 1)
                    {
                        NewLine();
                    }
                }

                Dedent();
                NewLine();
                Out("}");

                return node;
            }

            private LabelTarget ClosestBreak => _breaks.Count > 0 ? _breaks.Peek() : null;
            private LabelTarget ClosestContinue => _continues.Count > 0 ? _continues.Peek() : null;

            protected override Expression VisitGoto(GotoExpression node)
            {
                switch (node.Kind)
                {
                    case GotoExpressionKind.Break:
                        if (ClosestBreak != node.Target)
                            goto case GotoExpressionKind.Goto;
                        Out("break");
                        break;
                    case GotoExpressionKind.Continue:
                        if (ClosestContinue != node.Target)
                            goto case GotoExpressionKind.Goto;
                        Out("continue");
                        break;
                    case GotoExpressionKind.Goto:
                        Out($"goto {GetLabelName(node.Target)}");
                        break;
                    case GotoExpressionKind.Return: // NB: May require a target; track block scopes
                        if (node.Value != null && node.Value.Type != typeof(void))
                        {
                            Out("return ");
                            VisitExpression(node.Value);
                        }
                        else
                        {
                            Out("return");
                        }
                        break;
                }

                if (node.Kind != GotoExpressionKind.Return && node.Value != null && node.Value.Type != typeof(void))
                {
                    // NB: Not a valid C# construct, using comments instead
                    Out("/*(");
                    VisitExpression(node.Value);
                    Out(")*/");
                }

                Out(";");

                return node;
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                Out(GetLabelName(node.Target, true) + ":");

                return node;
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                Out("while (true)");

                var body = node.Body;

                var analysis = new NonLocalJumpAnalyzer(node.BreakLabel, node.ContinueLabel);
                analysis.Visit(body);

                if (analysis.HasNonLocalJump && node.ContinueLabel != null)
                {
                    var @continue = Expression.Label(node.ContinueLabel);

                    var block = body as BlockExpression;
                    if (block != null)
                    {
                        var exprs = new[] { @continue }.Concat(block.Expressions);
                        block = block.Update(block.Variables, exprs);
                    }
                    else
                    {
                        var exprs = new[] { @continue, body };
                        block = Expression.Block(node.Body.Type, exprs);
                    }

                    body = block;
                }

                PushBreak(node.BreakLabel);
                PushContinue(node.ContinueLabel);

                VisitBlockLike(body);

                PopContinue();
                PopBreak();

                if (analysis.HasNonLocalJump && node.BreakLabel != null)
                {
                    Visit(Expression.Label(node.BreakLabel));
                }

                return node;
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                // NB: Ignoring custom comparison

                Out("switch (");
                VisitExpression(node.SwitchValue);
                Out(")");

                NewLine();
                Out("{");
                Indent();
                NewLine();

                var n = node.Cases.Count;
                for (var i = 0; i < n; i++)
                {
                    var @case = node.Cases[i];

                    var m = @case.TestValues.Count;
                    for (var j = 0; j < m; j++)
                    {
                        var test = @case.TestValues[j];

                        Out("case ");

                        // NB: Not valid C#; would have to turn into if-then-else
                        var isConst = test is ConstantExpression;

                        if (!isConst)
                        {
                            Out("/* ");
                        }

                        VisitExpression(test);

                        if (!isConst)
                        {
                            Out(" */");
                        }

                        Out(":");

                        if (j != m - 1)
                        {
                            NewLine();
                        }
                    }

                    Indent();
                    NewLine();

                    Visit(@case.Body);
                    NewLine();
                    Out("break;");

                    Dedent();

                    if (i != n - 1 || node.DefaultBody != null)
                    {
                        NewLine();
                    }
                }

                if (node.DefaultBody != null)
                {
                    Out("default:");

                    Indent();
                    NewLine();

                    Visit(node.DefaultBody);
                    NewLine();
                    Out("break;");

                    Dedent();
                }

                Dedent();
                NewLine();
                Out("}");

                return node;
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                // NB: Not valid C#.

                Out("RuntimeVariables(");

                var n = node.Variables.Count;
                for (var i = 0; i < n; i++)
                {
                    Visit(node.Variables[i]);

                    if (i != n - 1)
                    {
                        Out(", ");
                    }
                }

                Out(")");

                return node;
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                // NB: Not valid C#.
                // TODO: Could decompile known binders.

                Out("Dynamic(");

                Out("/* ");
                Out(node.Binder.GetType().FullName);
                Out(" */ ");

                var n = node.Arguments.Count;
                for (var i = 0; i < n; i++)
                {
                    Visit(node.Arguments[i]);

                    if (i != n - 1)
                    {
                        Out(", ");
                    }
                }

                Out(")");

                return node;
            }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            {
                return node;
            }

            protected override Expression VisitExtension(Expression node)
            {
                var csharp = node as ICSharpPrintableExpression;

                if (csharp != null)
                {
                    csharp.Accept(this);

                    return node;
                }

                return base.VisitExtension(node);
            }

            public string GetLabelName(LabelTarget target, bool declarationSite = false)
            {
                var res = default(string);
                if (CSharpLanguageHelpers.AsValidIdentifier(target.Name, out res))
                {
                    return res;
                }

                var name = default(string);
                if (!_labels.TryGetValue(target, out name))
                {
                    name = $"L{_labels.Count}";
                    _labels.Add(target, name);
                }

                if (declarationSite)
                {
                    name = name + $" /*{target.Name ?? "(null)"}*/";
                }

                return name;
            }

            public string GetVariableName(ParameterExpression node, bool declarationSite = false)
            {
                var res = default(string);
                if (CSharpLanguageHelpers.AsValidIdentifier(node.Name, out res))
                {
                    return res;
                }

                var name = default(string);
                if (!_variables.TryGetValue(node, out name))
                {
                    name = $"p{_variables.Count}";
                    _variables.Add(node, name);
                }

                if (declarationSite)
                {
                    name = name + $" /*{node.Name ?? "(null)"}*/";
                }

                return name;
            }

            private void ArgsVisit(IList<Expression> args, MethodBase method, int startIndex = 0)
            {
                var pars = method.GetParameters();
                var n = args.Count;

                for (var i = startIndex; i < n; i++)
                {
                    var arg = args[i];
                    var par = pars[i];

                    if (par.ParameterType.IsByRef)
                    {
                        if (par.IsOut)
                        {
                            Out("out ");
                        }
                        else
                        {
                            Out("ref ");
                        }
                    }

                    Visit(arg);

                    if (i != n - 1)
                    {
                        Out(", ");
                    }
                }
            }

            private void Visit(IList<Expression> args)
            {
                var n = args.Count;

                for (var i = 0; i < n; i++)
                {
                    Visit(args[i]);

                    if (i != n - 1)
                    {
                        Out(", ");
                    }
                }
            }

            public void StaticMethodCall(MethodInfo method, params Expression[] args)
            {
                Out(ToCSharp(method.DeclaringType));
                Out(".");
                Out(method.Name);

                Out("(");
                ArgsVisit(args, method);
                Out(")");
            }

            public void ParenthesizedVisit(Expression parent, Expression nodeToVisit)
            {
                if (NeedsParentheses(parent, nodeToVisit))
                {
                    Out("(");
                    Visit(nodeToVisit);
                    Out(")");
                }
                else
                {
                    Visit(nodeToVisit);
                }
            }

            public void VisitBlockLike(Expression node, bool needsCurlies = false)
            {
                if (node.NodeType != ExpressionType.Block)
                {
                    if (needsCurlies)
                    {
                        NewLine();
                        Out("{");
                    }

                    Indent();
                    NewLine();
                    Visit(node);
                    Dedent();
                    NewLine();

                    if (needsCurlies)
                    {
                        Out("}");
                        NewLine();
                    }
                }
                else
                {
                    NewLine();
                    Visit(node);
                    NewLine();
                }
            }

            private static bool NeedsParentheses(Expression parent, Expression child)
            {
                // TODO: better extensibility needed for parentheses?

                if (child == null)
                {
                    return false;
                }

                var childOpPrec = CSharpLanguageHelpers.GetOperatorPrecedence(child);
                var parentOpPrec = CSharpLanguageHelpers.GetOperatorPrecedence(parent);

                if (childOpPrec == parentOpPrec)
                {
                    switch (parent.NodeType)
                    {
                        case ExpressionType.Subtract:
                        case ExpressionType.SubtractChecked:
                        case ExpressionType.Divide:
                        case ExpressionType.Modulo:
                            return ((BinaryExpression)parent).Right == child;
                    }

                    return false;
                }

                if (child.NodeType == ExpressionType.Constant && parent.NodeType == ExpressionType.Negate)
                {
                    return true;
                }

                return childOpPrec < parentOpPrec;
            }

            public void Out(string s)
            {
                _writer.Write(s);
            }

            public void NewLine()
            {
                _writer.WriteLine();
                _writer.Write(_indent);
            }

            public void Indent()
            {
                var len = _indent.Length + 4;
                _indent = new string(' ', len);
            }

            public void Dedent()
            {
                var len = _indent.Length - 4;
                _indent = new string(' ', len);
            }

            private static readonly Dictionary<Type, string> s_primitives = new Dictionary<Type, string>
            {
                { typeof(sbyte), "sbyte" },
                { typeof(byte), "byte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(object), "object" },
            };

            public string ToCSharp(Type type)
            {
                if (type.IsArray)
                {
                    var elem = ToCSharp(type.GetElementType());
                    var rank = type.GetArrayRank(); // NB: ignores multi-dimensional with rank 1
                    return $"{elem}[{new string(',', rank - 1)}]";
                }
                else if (type.IsGenericType)
                {
                    if (type.IsGenericTypeDefinition)
                    {
                        var name = type.FullName;

                        var tick = name.IndexOf('`');
                        if (tick >= 0)
                        {
                            name = name.Substring(0, tick);
                        }

                        return $"{name}<{new string(',', type.GetGenericArguments().Length)}>";
                    }
                    else if (type.IsGenericParameter)
                    {
                        return type.Name;
                    }
                    else
                    {
                        var def = type.GetGenericTypeDefinition();

                        if (def == typeof(Nullable<>))
                        {
                            var arg = type.GetGenericArguments()[0];
                            var val = ToCSharp(arg);
                            return $"{val}?";
                        }
                        else
                        {
                            var name = _namespaces.Contains(def.Namespace) ? def.Name : def.FullName;

                            var tick = name.IndexOf('`');
                            if (tick >= 0)
                            {
                                name = name.Substring(0, tick);
                            }

                            var args = string.Join(", ", type.GetGenericArguments().Select(ToCSharp));

                            return $"{name}<{args}>";
                        }
                    }
                }
                else
                {
                    var res = default(string);

                    if (s_primitives.TryGetValue(type, out res))
                    {
                        return res;
                    }

                    return _namespaces.Contains(type.Namespace) ? type.Name : type.FullName;
                }
            }

            private static string Escape(char ch)
            {
                var res = default(string);
                if (!TryEscape(ch, '\"', out res))
                {
                    res = ch.ToString();
                }

                return res;
            }

            private static bool TryEscape(char ch, char noEscape, out string res)
            {
                res = null;

                if (ch != noEscape)
                {
                    switch (ch)
                    {
                        case '\t':
                            res = "\\t";
                            break;
                        case '\r':
                            res = "\\r";
                            break;
                        case '\n':
                            res = "\\n";
                            break;
                        case '\b':
                            res = "\\b";
                            break;
                        case '\f':
                            res = "\\f";
                            break;
                        case '\a':
                            res = "\\a";
                            break;
                        case '\v':
                            res = "\\v";
                            break;
                        case '\0':
                            res = "\\0";
                            break;
                        case '\\':
                            res = "\\\\";
                            break;
                        case '\'':
                            res = "\\'";
                            break;
                        case '\"':
                            res = "\\\"";
                            break;
                    }
                }

                return res != null;
            }

            private static string Escape(string str)
            {
                var sb = default(StringBuilder);

                for (var i = 0; i < str.Length; i++)
                {
                    var ch = str[i];

                    var chStr = default(string);
                    if (TryEscape(ch, '\'', out chStr))
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            sb.Append(str, 0, i);
                        }

                        sb.Append(chStr);
                    }
                    else
                    {
                        if (sb != null)
                        {
                            sb.Append(ch);
                        }
                    }
                }

                if (sb != null)
                {
                    return sb.ToString();
                }

                return str;
            }

            class NonLocalJumpAnalyzer : ExpressionVisitor
            {
                private readonly Stack<LoopExpression> _stack = new Stack<LoopExpression>();
                private readonly LabelTarget _break;
                private readonly LabelTarget _continue;

                public bool HasNonLocalJump;

                public NonLocalJumpAnalyzer(LabelTarget @break, LabelTarget @continue)
                {
                    _break = @break;
                    _continue = @continue;
                }

                protected override Expression VisitLoop(LoopExpression node)
                {
                    _stack.Push(node);

                    var res = base.VisitLoop(node);

                    _stack.Pop();

                    return res;
                }

                protected override Expression VisitGoto(GotoExpression node)
                {
                    switch (node.Kind)
                    {
                        case GotoExpressionKind.Break:
                            if (node.Target == _break && _stack.Count > 0)
                                HasNonLocalJump = true;
                            break;
                        case GotoExpressionKind.Continue:
                            if (node.Target == _continue && _stack.Count > 0)
                                HasNonLocalJump = true;
                            break;
                    }

                    return base.VisitGoto(node);
                }
            }
        }
    }

    internal class CheckedOpScanner : ExpressionVisitor
    {
        public bool HasUncheckedOperation;

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Multiply:
                case ExpressionType.Negate:
                case ExpressionType.Subtract:
                case ExpressionType.AddAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.SubtractAssign:
                    HasUncheckedOperation = true;
                    return node;
                case ExpressionType.Convert:
                    var convert = (UnaryExpression)node;
                    if (!TypeUtils2.HasReferenceConversion(convert.Operand.Type, convert.Type))
                    {
                        HasUncheckedOperation = true;
                        return node;
                    }
                    break;
                case ExpressionType.Extension:
                    var csharp = node as ICSharpPrintableExpression;
                    if (csharp != null)
                    {
                        HasUncheckedOperation |= csharp.HasCheckedMode && !csharp.IsChecked;
                    }
                    break;
            }

            return base.Visit(node);
        }
    }

    internal static class CSharpLanguageHelpers
    {
        public static int GetOperatorPrecedence(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Assign:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.Lambda:
                case ExpressionType.Quote:                  // NB: don't want parens around quoted lambda
                    return 10;

                case ExpressionType.Conditional:
                    return 20;

                case ExpressionType.Coalesce:
                    return 25;

                case ExpressionType.OrElse:
                    return 30;

                case ExpressionType.AndAlso:
                    return 40;

                case ExpressionType.Or:
                    return 50;

                case ExpressionType.ExclusiveOr:
                    return 60;

                case ExpressionType.And:
                    return 70;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.IsTrue:    // NB: based on display choice
                case ExpressionType.IsFalse:   // NB: based on display choice
                case ExpressionType.TypeEqual: // NB: based on display choice
                    return 80;

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.TypeIs:
                case ExpressionType.TypeAs:
                    return 90;

                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    return 100;

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Decrement: // NB: based on display choice
                case ExpressionType.Increment: // NB: based on display choice
                    return 110;

                case ExpressionType.Divide:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Modulo:
                    return 120;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Not:
                case ExpressionType.OnesComplement:
                case ExpressionType.Throw:
                case ExpressionType.Unbox:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                    return 130;

                // NB: No infix operator for this
                //case ExpressionType.Power:
                //    return 140;

                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Call:
                case ExpressionType.Default:
                case ExpressionType.Invoke:
                case ExpressionType.Index:
                case ExpressionType.ListInit:
                case ExpressionType.MemberAccess:
                case ExpressionType.MemberInit:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.Power:               // NB: based on display choice
                case ExpressionType.Dynamic:             // NB: based on display choice
                case ExpressionType.RuntimeVariables:    // NB: based on display choice
                case ExpressionType.Extension:           // NB: will be reduced by default
                default:
                    return 150;

                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.DebugInfo:           // NB: never rendered, so don't want parens
                    return 160;

                // NB: these are statements
                //case ExpressionType.Block:
                //case ExpressionType.Goto:
                //case ExpressionType.Label:
                //case ExpressionType.Loop:
                //case ExpressionType.Switch:
                //case ExpressionType.Try:
                //    break;
            }
        }

        public static int GetOperatorPrecedence(Expression node)
        {
            if (node.NodeType == ExpressionType.Extension)
            {
                var csharp = node as ICSharpPrintableExpression;
                if (csharp != null)
                {
                    return csharp.Precedence;
                }
            }

            return GetOperatorPrecedence(node.NodeType);
        }

        public static string GetOperatorSyntax(ExpressionType nodeType)
        {
            var op = default(string);

            switch (nodeType)
            {
                case ExpressionType.Add:
                    op = "+";
                    break;
                case ExpressionType.AddChecked:
                    op = "+";
                    break;
                case ExpressionType.And:
                    op = "&";
                    break;
                case ExpressionType.AndAlso:
                    op = "&&";
                    break;
                case ExpressionType.Coalesce:
                    op = "??";
                    break;
                case ExpressionType.Divide:
                    op = "/";
                    break;
                case ExpressionType.Equal:
                    op = "==";
                    break;
                case ExpressionType.ExclusiveOr:
                    op = "^";
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case ExpressionType.LeftShift:
                    op = "<<";
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                case ExpressionType.Modulo:
                    op = "%";
                    break;
                case ExpressionType.Multiply:
                    op = "*";
                    break;
                case ExpressionType.MultiplyChecked:
                    op = "*";
                    break;
                case ExpressionType.NotEqual:
                    op = "!=";
                    break;
                case ExpressionType.Or:
                    op = "|";
                    break;
                case ExpressionType.OrElse:
                    op = "||";
                    break;
                case ExpressionType.Power:
                    op = "**"; // NB: not valid C#, but asMethod saves the day
                    break;
                case ExpressionType.RightShift:
                    op = ">>";
                    break;
                case ExpressionType.Subtract:
                    op = "-";
                    break;
                case ExpressionType.SubtractChecked:
                    op = "-";
                    break;
                case ExpressionType.Assign:
                    op = "=";
                    break;
                case ExpressionType.AddAssign:
                    op = "+=";
                    break;
                case ExpressionType.AndAssign:
                    op = "&=";
                    break;
                case ExpressionType.DivideAssign:
                    op = "/=";
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    op = "^=";
                    break;
                case ExpressionType.LeftShiftAssign:
                    op = "<<=";
                    break;
                case ExpressionType.ModuloAssign:
                    op = "%=";
                    break;
                case ExpressionType.MultiplyAssign:
                    op = "*=";
                    break;
                case ExpressionType.OrAssign:
                    op = "|=";
                    break;
                case ExpressionType.PowerAssign:
                    op = "**="; // NB: not valid C#, but asMethod saves the day
                    break;
                case ExpressionType.RightShiftAssign:
                    op = ">>=";
                    break;
                case ExpressionType.SubtractAssign:
                    op = "-=";
                    break;
                case ExpressionType.AddAssignChecked:
                    op = "+=";
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    op = "*=";
                    break;
                case ExpressionType.SubtractAssignChecked:
                    op = "-=";
                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    op = "-";
                    break;
                case ExpressionType.UnaryPlus:
                    op = "+";
                    break;
                case ExpressionType.Not:
                    op = "!";
                    break;
                case ExpressionType.OnesComplement:
                    op = "~";
                    break;
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostDecrementAssign:
                    op = "--";
                    break;
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PostIncrementAssign:
                    op = "++";
                    break;
            }

            return op;
        }

        public static string GetClsMethodName(ExpressionType nodeType)
        {
            var mtd = default(string);

            switch (nodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                    mtd = "op_Addition";
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.AndAssign:
                    mtd = "op_BitwiseAnd";
                    break;
                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                    mtd = "op_Division";
                    break;
                case ExpressionType.Equal:
                    mtd = "op_Equality";
                    break;
                case ExpressionType.ExclusiveOr:
                case ExpressionType.ExclusiveOrAssign:
                    mtd = "op_ExclusiveOr";
                    break;
                case ExpressionType.GreaterThan:
                    mtd = "op_GreaterThan";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    mtd = "op_GreaterThanOrEqual";
                    break;
                case ExpressionType.LeftShift:
                case ExpressionType.LeftShiftAssign:
                    mtd = "op_LeftShift";
                    break;
                case ExpressionType.LessThan:
                    mtd = "op_LessThan";
                    break;
                case ExpressionType.LessThanOrEqual:
                    mtd = "op_LessThanOrEqual";
                    break;
                case ExpressionType.Modulo:
                case ExpressionType.ModuloAssign:
                    mtd = "op_Modulus";
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    mtd = "op_Multiply";
                    break;
                case ExpressionType.NotEqual:
                    mtd = "op_Inequality";
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.OrAssign:
                    mtd = "op_BitwiseOr";
                    break;
                case ExpressionType.RightShift:
                case ExpressionType.RightShiftAssign:
                    mtd = "op_RightShift";
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    mtd = "op_Subtraction";
                    break;
                case ExpressionType.IsTrue:
                    mtd = "op_True";
                    break;
                case ExpressionType.IsFalse:
                    mtd = "op_False";
                    break;
                case ExpressionType.Decrement:
                    mtd = "op_Decrement";
                    break;
                case ExpressionType.Increment:
                    mtd = "op_Increment";
                    break;
                case ExpressionType.Negate:
                    mtd = "op_UnaryNegation";
                    break;
                case ExpressionType.UnaryPlus:
                    mtd = "op_UnaryPlus";
                    break;
                case ExpressionType.Not:
                    mtd = "op_LogicalNot";
                    break;
                case ExpressionType.OnesComplement:
                    mtd = "op_OnesComplement";
                    break;
                case ExpressionType.PreDecrementAssign:
                    mtd = "op_Decrement";
                    break;
                case ExpressionType.PreIncrementAssign:
                    mtd = "op_Increment";
                    break;
                case ExpressionType.PostDecrementAssign:
                    mtd = "op_Decrement";
                    break;
                case ExpressionType.PostIncrementAssign:
                    mtd = "op_Increment";
                    break;
            }

            return mtd;
        }

        public static bool CanInferGenericArguments(MethodBase method)
        {
            if (method.GetGenericArguments().Any(t => t.IsDefined(typeof(CompilerGeneratedAttribute)) && t.Name.StartsWith("<>f__AnonymousType")))
            {
                return true;
            }

            // TODO: add more heuristics, e.g. all type parameters are closed by occurrences in parameter types
            //       (though this can have false positives, need to pass in concrete arguments as well to analyze)

            return false;
        }

        private static HashSet<string> s_keywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break",
            "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum",
            "event", "explicit", "extern", "false", "finally",
            "fixed", "float", "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator", "out",
            "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", "virtual", "void",
            "volatile", "while"
        };

        public static bool AsValidIdentifier(string value, out string result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return false;
            }

            if (s_keywords.Contains(value))
            {
                result = "@" + value;
                return true;
            }

            // NB: ignores a few extremer cases from B.1.6., e.g. unicode escape character sequences

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (i == 0)
                {
                    if (c == '@' || c == '_' || char.IsLetter(c))
                    {
                        continue;
                    }

                    result = null;
                    return false;
                }
                else if (char.IsLetterOrDigit(c) || c == '_')
                {
                    continue;
                }

                result = null;
                return false;
            }

            result = value;
            return true;
        }
    }
}
