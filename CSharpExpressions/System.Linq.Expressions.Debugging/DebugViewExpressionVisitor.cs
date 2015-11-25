// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Expression visitors that produces a debug view.
    /// </summary>
    public class DebugViewExpressionVisitor : ExpressionVisitor, IDebugViewExpressionVisitor
    {
        private readonly Stack<XNode> _nodes = new Stack<XNode>();
        private readonly IDictionary<object, int> _instanceIds = new Dictionary<object, int>();

        /// <summary>
        /// Gets the debug view for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to get a debug view for.</param>
        /// <returns>Debug view for the specified expression.</returns>
        public XNode GetDebugView(Expression expression)
        {
            base.Visit(expression);
            return _nodes.Pop();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return Push(node, new XAttribute("Value", node.Value));
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return Push(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var id = MakeInstanceId(node);

            return Push(node, new XAttribute("Name", node.Name), new XAttribute("Id", id));
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = Visit(node.Body);

            var parameters = Visit("Parameters", node.Parameters);

            return Push(node, new XElement("Body", body), parameters);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            if (node.Conversion != null)
            {
                var convert = Visit(node.Conversion);

                return Push(node, new XElement("Left", left), new XElement("Right", right), new XElement("Conversion", convert));
            }
            else
            {
                return Push(node, new XElement("Left", left), new XElement("Right", right));
            }
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.Operand != null)
            {
                var operand = Visit(node.Operand);

                return Push(node, new XElement("Operand", operand));
            }
            else
            {
                return Push(node, new XElement(node.NodeType.ToString()));
            }
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement("Test", Visit(node.Test)));
            args.Add(new XElement("IfTrue", Visit(node.IfTrue)));

            if (node.IfFalse != null)
            {
                args.Add(new XElement("IfFalse", Visit(node.IfFalse)));
            }

            return Push(node, args);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
            {
                var expr = Visit(node.Expression);
                return Push(node, new XAttribute("Member", node.Member), new XElement("Expression", expr));
            }
            else
            {
                return Push(node, new XAttribute("Member", node.Member));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var args = Visit("Arguments", node.Arguments);

            if (node.Object != null)
            {
                var obj = Visit(node.Object);
                return Push(node, new XAttribute("Method", node.Method), new XElement("Object", obj), args);
            }
            else
            {
                return Push(node, new XAttribute("Method", node.Method), args);
            }
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            var obj = Visit(node.Object);
            var args = Visit("Arguments", node.Arguments);

            return Push(node, new XAttribute("Indexer", node.Indexer), new XElement("Object", obj), args);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            var expr = Visit(node.Expression);
            var args = Visit("Arguments", node.Arguments);

            return Push(node, new XElement("Expression", expr), args);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var nodes = new List<object>();

            if (node.Constructor != null)
            {
                nodes.Add(new XAttribute("Constructor", node.Constructor));
            }

            var args = Visit("Arguments", node.Arguments);
            nodes.Add(args);

            if (node.Members != null)
            {
                var members = new XElement("Members", node.Members.Select(m => new XElement("Member", m)));
                nodes.Add(members);
            }

            return Push(node, nodes);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var exprs = Visit("Expressions", node.Expressions);

            return Push(node, exprs);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            var expr = Visit(node.Expression);

            return Push(node, new XAttribute("TypeOperand", node.TypeOperand), new XElement("Expression", expr));
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var create = Visit(node.NewExpression);

            var bindings = Visit("Bindings", node.Bindings, VisitMemberBinding2);

            return Push(node, new XElement("NewExpression", create), bindings);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            var expr = Visit(node.Expression);

            return PushBinding(node, new XAttribute("Member", node.Member), new XElement("Expression", expr));
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            var bindings = Visit("Bindings", node.Bindings, VisitMemberBinding2);

            return PushBinding(node, new XAttribute("Member", node.Member), bindings);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            var initializers = Visit("Initializers", node.Initializers, VisitElementInit2);

            return PushBinding(node, new XAttribute("Member", node.Member), initializers);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            var create = Visit(node.NewExpression);

            var initializers = Visit("Initializers", node.Initializers, VisitElementInit2);

            return Push(node, new XElement("NewExpression", create), initializers);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            var args = Visit("Arguments", node.Arguments);

            var res = new XElement("ElementInit", new XAttribute("AddMethod", node.AddMethod), args);
            _nodes.Push(res);

            return node;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var expressions = Visit("Expressions", node.Expressions);

            if (node.Variables.Count > 0)
            {
                var variables = Visit("Variables", node.Variables);
                return Push(node, variables, expressions);
            }
            else
            {
                return Push(node, expressions);
            }
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            var args = new List<object>();

            if (node.Comparison != null)
            {
                args.Add(new XAttribute("Comparison", node.Comparison));
            }

            args.Add(new XElement("SwitchValue", Visit(node.SwitchValue)));

            args.Add(Visit("Cases", node.Cases, VisitSwitchCase2));

            if (node.DefaultBody != null)
            {
                args.Add(new XElement("DefaultBody", Visit(node.DefaultBody)));
            }

            return Push(node, args);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            _nodes.Push(new XElement("SwitchCase", new XElement("Body", Visit(node.Body)), Visit("TestValues", node.TestValues)));
            return node;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement("Body", Visit(node.Body)));

            if (node.Handlers.Count > 0)
            {
                args.Add(Visit("Handlers", node.Handlers, VisitCatchBlock2));
            }

            if (node.Finally != null)
            {
                args.Add(new XElement("Finally", Visit(node.Finally)));
            }

            if (node.Fault != null)
            {
                args.Add(new XElement("Fault", Visit(node.Fault)));
            }

            return Push(node, args);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var args = new List<object>();

            if (node.Variable != null)
            {
                args.Add(new XElement("Variable", Visit(node.Variable)));
            }
            else
            {
                args.Add(new XAttribute("Test", node.Test));
            }

            args.Add(new XElement("Body", Visit(node.Body)));

            if (node.Filter != null)
            {
                args.Add(new XElement("Filter", Visit(node.Filter)));
            }

            _nodes.Push(new XElement("CatchBlock", args));

            return node;
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            var args = new List<object>();

            args.Add(new XAttribute("Kind", node.Kind));

            args.Add(new XElement("Target", VisitLabelTarget2(node.Target)));

            if (node.Value != null)
            {
                args.Add(new XElement("Value", Visit(node.Value)));
            }

            return Push(node, args);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement("Target", VisitLabelTarget2(node.Target)));

            if (node.DefaultValue != null)
            {
                args.Add(new XElement("DefaultValue", Visit(node.DefaultValue)));
            }

            return Push(node);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            _nodes.Push(new XElement("LabelTarget", new XAttribute("Type", node.Type), new XAttribute("Name", node.Name), new XAttribute("Id", MakeInstanceId(node))));
            return node;
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement("Body", Visit(node.Body)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement("BreakLabel", VisitLabelTarget2(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement("ContinueLabel", VisitLabelTarget2(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return Push(node, Visit("Variables", node.Variables));
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            var args = new List<object>();

            // TODO: add more document info?
            args.Add(new XAttribute("FileName", node.Document.FileName));

            if (node.IsClear)
            {
                args.Add(new XAttribute("IsClear", true));
            }
            else
            {
                args.Add(new XAttribute("StartLine", node.StartLine));
                args.Add(new XAttribute("StartColumn", node.StartColumn));
                args.Add(new XAttribute("EndLine", node.EndLine));
                args.Add(new XAttribute("EndColumn", node.EndColumn));
            }

            return Push(node, args);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            var args = Visit("Arguments", node.Arguments);
            return Push(node, new XAttribute("Binder", node.Binder), new XAttribute("DelegateType", node.DelegateType), args);
        }

        protected override Expression VisitExtension(Expression node)
        {
            var dbg = node as IDebugViewExpression;
            if (dbg != null)
            {
                _nodes.Push(dbg.Accept(this));
                return node;
            }

            if (node.CanReduce)
            {
                return Push(node, new XAttribute("ExtensionType", node.GetType()), new XElement("Reduced", Visit(node.Reduce())));
            }
            else
            {
                return Push(node, new XAttribute("ExtensionType", node.GetType()));
            }
        }

        protected new XNode Visit(Expression expression)
        {
            base.Visit(expression);
            return _nodes.Pop();
        }

        protected XNode Visit(string name, IEnumerable<Expression> expressions)
        {
            var res = new List<XNode>();

            foreach (var expression in expressions)
            {
                res.Add(GetDebugView(expression));
            }

            return new XElement(name, res);
        }

        protected XNode Visit<T>(string name, IEnumerable<T> expressions, Func<T, XNode> visit)
        {
            var res = new List<XNode>();

            foreach (var expression in expressions)
            {
                res.Add(visit(expression));
            }

            return new XElement(name, res);
        }

        protected T Push<T>(T node, params object[] content)
            where T : Expression
        {
            _nodes.Push(new XElement(node.NodeType.ToString(), new XAttribute("Type", node.Type), content));
            return node;
        }

        protected T PushBinding<T>(T node, params object[] content)
            where T : MemberBinding
        {
            _nodes.Push(new XElement(node.BindingType.ToString(), content));
            return node;
        }

        protected int MakeInstanceId(object o)
        {
            var id = default(int);
            if (!_instanceIds.TryGetValue(o, out id))
            {
                _instanceIds[o] = id = _instanceIds.Count;
            }

            return id;
        }

        private XNode VisitMemberBinding2(MemberBinding node)
        {
            base.VisitMemberBinding(node);
            return _nodes.Pop();
        }

        private XNode VisitElementInit2(ElementInit node)
        {
            base.VisitElementInit(node);
            return _nodes.Pop();
        }

        private XNode VisitCatchBlock2(CatchBlock node)
        {
            base.VisitCatchBlock(node);
            return _nodes.Pop();
        }

        private XNode VisitLabelTarget2(LabelTarget node)
        {
            base.VisitLabelTarget(node);
            return _nodes.Pop();
        }

        private XNode VisitSwitchCase2(SwitchCase node)
        {
            base.VisitSwitchCase(node);
            return _nodes.Pop();
        }
    }
}
