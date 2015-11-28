// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Expression visitor that produces a debug view.
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
            return Visit(expression);
        }

        /// <summary>
        /// Gets the debug view for the specified label.
        /// </summary>
        /// <param name="label">The label to get a debug view for.</param>
        /// <returns>Debug view for the specified label.</returns>
        public XNode GetDebugView(LabelTarget label)
        {
            return Visit(label);
        }

        /// <summary>
        /// Gets the debug view for the specified member binding.
        /// </summary>
        /// <param name="binding">The member binding to get a debug view for.</param>
        /// <returns>Debug view for the specified member binding.</returns>
        public XNode GetDebugView(MemberBinding binding)
        {
            return Visit(binding);
        }

        /// <summary>
        /// Gets the debug view for the specified element initializer.
        /// </summary>
        /// <param name="initializer">The element initializer to get a debug view for.</param>
        /// <returns>Debug view for the specified element initializer.</returns>
        public XNode GetDebugView(ElementInit initializer)
        {
            return Visit(initializer);
        }

        /// <summary>
        /// Gets the debug view for the specified catch block.
        /// </summary>
        /// <param name="catchBlock">The catch block to get a debug view for.</param>
        /// <returns>Debug view for the specified catch block.</returns>
        public XNode GetDebugView(CatchBlock catchBlock)
        {
            return Visit(catchBlock);
        }

        /// <summary>
        /// Gets the debug view for the specified switch case.
        /// </summary>
        /// <param name="switchCase">The switch case to get a debug view for.</param>
        /// <returns>Debug view for the specified switch case.</returns>
        public XNode GetDebugView(SwitchCase switchCase)
        {
            return Visit(switchCase);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return Push(node, new XAttribute(nameof(node.Value), node.Value));
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return Push(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var args = new List<object>();

            var id = MakeInstanceId(node);
            args.Add(new XAttribute("Id", id));

            if (node.Name != null)
            {
                args.Add(new XAttribute(nameof(node.Name), node.Name));
            }

            return Push(node, args);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var parameters = Visit(nameof(Expression<T>.Parameters), node.Parameters);

            var body = Visit(node.Body);

            return Push(node, parameters, new XElement(nameof(Expression<T>.Body), body));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var args = new List<object>();

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            args.Add(new XElement(nameof(node.Right), Visit(node.Right)));

            if (node.Conversion != null)
            {
                args.Add(new XElement(nameof(node.Conversion), Visit(node.Conversion)));
            }

            return Push(node, args);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var args = new List<object>();

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            if (node.Operand != null)
            {
                args.Add(new XElement(nameof(node.Operand), Visit(node.Operand)));
            }

            return Push(node, args);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement(nameof(node.Test), Visit(node.Test)));
            args.Add(new XElement(nameof(node.IfTrue), Visit(node.IfTrue)));
            args.Add(new XElement(nameof(node.IfFalse), Visit(node.IfFalse)));

            return Push(node, args);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
            {
                var expr = Visit(node.Expression);
                return Push(node, new XAttribute(nameof(node.Member), node.Member), new XElement(nameof(node.Expression), expr));
            }
            else
            {
                return Push(node, new XAttribute(nameof(node.Member), node.Member));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object != null)
            {
                var obj = Visit(node.Object);
                var args = Visit(nameof(node.Arguments), node.Arguments);

                return Push(node, new XAttribute(nameof(node.Method), node.Method), new XElement(nameof(node.Object), obj), args);
            }
            else
            {
                var args = Visit(nameof(node.Arguments), node.Arguments);

                return Push(node, new XAttribute(nameof(node.Method), node.Method), args);
            }
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            var obj = Visit(node.Object);
            var args = Visit(nameof(node.Arguments), node.Arguments);

            if (node.Indexer != null)
            {
                return Push(node, new XAttribute(nameof(node.Indexer), node.Indexer), new XElement(nameof(node.Object), obj), args);
            }
            else
            {
                // NB: This is the case for arrays
                return Push(node, new XElement(nameof(node.Object), obj), args);
            }
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            var expr = Visit(node.Expression);
            var args = Visit(nameof(node.Arguments), node.Arguments);

            return Push(node, new XElement(nameof(node.Expression), expr), args);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var nodes = new List<object>();

            if (node.Constructor != null)
            {
                nodes.Add(new XAttribute(nameof(node.Constructor), node.Constructor));
            }

            var args = Visit(nameof(node.Arguments), node.Arguments);
            nodes.Add(args);

            if (node.Members != null)
            {
                var members = new XElement(nameof(node.Members), node.Members.Select(m => new XElement("Member", m)));
                nodes.Add(members);
            }

            return Push(node, nodes);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var exprs = Visit(nameof(node.Expressions), node.Expressions);

            return Push(node, exprs);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            var expr = Visit(node.Expression);

            return Push(node, new XAttribute(nameof(node.TypeOperand), node.TypeOperand), new XElement(nameof(node.Expression), expr));
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var create = Visit(node.NewExpression);

            var bindings = Visit(nameof(node.Bindings), node.Bindings, Visit);

            return Push(node, new XElement(nameof(node.NewExpression), create), bindings);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            var expr = Visit(node.Expression);

            return PushBinding(node, new XAttribute(nameof(node.Member), node.Member), new XElement(nameof(node.Expression), expr));
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            var bindings = Visit(nameof(node.Bindings), node.Bindings, Visit);

            return PushBinding(node, new XAttribute(nameof(node.Member), node.Member), bindings);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            var initializers = Visit(nameof(node.Initializers), node.Initializers, Visit);

            return PushBinding(node, new XAttribute(nameof(node.Member), node.Member), initializers);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            var create = Visit(node.NewExpression);

            var initializers = Visit(nameof(node.Initializers), node.Initializers, Visit);

            return Push(node, new XElement(nameof(node.NewExpression), create), initializers);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            var args = Visit(nameof(node.Arguments), node.Arguments);

            var res = new XElement(nameof(ElementInit), new XAttribute(nameof(node.AddMethod), node.AddMethod), args);
            _nodes.Push(res);

            return node;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var expressions = Visit(nameof(node.Expressions), node.Expressions);

            if (node.Variables.Count > 0)
            {
                var variables = Visit(nameof(node.Variables), node.Variables);
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
                args.Add(new XAttribute(nameof(node.Comparison), node.Comparison));
            }

            args.Add(new XElement(nameof(node.SwitchValue), Visit(node.SwitchValue)));

            args.Add(Visit(nameof(node.Cases), node.Cases, Visit));

            if (node.DefaultBody != null)
            {
                args.Add(new XElement(nameof(node.DefaultBody), Visit(node.DefaultBody)));
            }

            return Push(node, args);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            _nodes.Push(new XElement(nameof(SwitchCase), new XElement(nameof(node.Body), Visit(node.Body)), Visit(nameof(node.TestValues), node.TestValues)));
            return node;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.Handlers.Count > 0)
            {
                args.Add(Visit(nameof(node.Handlers), node.Handlers, Visit));
            }

            if (node.Finally != null)
            {
                args.Add(new XElement(nameof(node.Finally), Visit(node.Finally)));
            }

            if (node.Fault != null)
            {
                args.Add(new XElement(nameof(node.Fault), Visit(node.Fault)));
            }

            return Push(node, args);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var args = new List<object>();

            if (node.Variable != null)
            {
                args.Add(new XElement(nameof(node.Variable), Visit(node.Variable)));
            }
            else
            {
                args.Add(new XAttribute(nameof(node.Test), node.Test));
            }

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.Filter != null)
            {
                args.Add(new XElement(nameof(node.Filter), Visit(node.Filter)));
            }

            _nodes.Push(new XElement(nameof(CatchBlock), args));

            return node;
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            var args = new List<object>();

            args.Add(new XAttribute(nameof(node.Kind), node.Kind));

            args.Add(new XElement(nameof(node.Target), Visit(node.Target)));

            if (node.Value != null)
            {
                args.Add(new XElement(nameof(node.Value), Visit(node.Value)));
            }

            return Push(node, args);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement(nameof(node.Target), Visit(node.Target)));

            if (node.DefaultValue != null)
            {
                args.Add(new XElement(nameof(node.DefaultValue), Visit(node.DefaultValue)));
            }

            return Push(node, args);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            var args = new List<object>();

            args.Add(new XAttribute(nameof(node.Type), node.Type));

            var id = MakeInstanceId(node);
            args.Add(new XAttribute("Id", id));

            if (node.Name != null)
            {
                args.Add(new XAttribute(nameof(node.Name), node.Name));
            }

            _nodes.Push(new XElement(nameof(LabelTarget), args));
            return node;
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            var args = new List<object>();

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement(nameof(node.BreakLabel), Visit(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement(nameof(node.ContinueLabel), Visit(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return Push(node, Visit(nameof(node.Variables), node.Variables));
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            var args = new List<object>();

            // TODO: add more document info?
            args.Add(new XAttribute(nameof(node.Document.FileName), node.Document.FileName));

            if (node.IsClear)
            {
                args.Add(new XAttribute(nameof(node.IsClear), true));
            }
            else
            {
                args.Add(new XAttribute(nameof(node.StartLine), node.StartLine));
                args.Add(new XAttribute(nameof(node.StartColumn), node.StartColumn));
                args.Add(new XAttribute(nameof(node.EndLine), node.EndLine));
                args.Add(new XAttribute(nameof(node.EndColumn), node.EndColumn));
            }

            return Push(node, args);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            var args = Visit(nameof(node.Arguments), node.Arguments);
            return Push(node, new XAttribute(nameof(node.Binder), node.Binder), new XAttribute(nameof(node.DelegateType), node.DelegateType), args);
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

        protected XNode Visit(LabelTarget node)
        {
            VisitLabelTarget(node);
            return _nodes.Pop();
        }

        protected XNode Visit(MemberBinding node)
        {
            VisitMemberBinding(node);
            return _nodes.Pop();
        }

        protected XNode Visit(ElementInit node)
        {
            VisitElementInit(node);
            return _nodes.Pop();
        }

        protected XNode Visit(CatchBlock node)
        {
            VisitCatchBlock(node);
            return _nodes.Pop();
        }

        protected XNode Visit(SwitchCase node)
        {
            VisitSwitchCase(node);
            return _nodes.Pop();
        }

        protected XNode Visit(string name, IEnumerable<Expression> expressions)
        {
            var res = new List<XNode>();

            foreach (var expression in expressions)
            {
                res.Add(Visit(expression));
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
            _nodes.Push(new XElement(node.NodeType.ToString(), new XAttribute(nameof(node.Type), node.Type), content));
            return node;
        }

        protected T PushBinding<T>(T node, params object[] content)
            where T : MemberBinding
        {
            _nodes.Push(new XElement(node.BindingType.ToString(), content));
            return node;
        }

        public int MakeInstanceId(object o)
        {
            var id = default(int);
            if (!_instanceIds.TryGetValue(o, out id))
            {
                _instanceIds[o] = id = _instanceIds.Count;
            }

            return id;
        }
    }
}
