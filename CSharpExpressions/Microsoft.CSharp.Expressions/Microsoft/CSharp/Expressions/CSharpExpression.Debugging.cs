﻿// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Microsoft.CSharp.Expressions
{
    // DESIGN: I don't like the reference to System.Xml.* assemblies here. Keeping it for now to unblock testing
    //         without having to create a more fancy debug view generator with indent/outdent behavior etc.
    //
    //         The best alternative would be to get extensible DebugView support in LINQ through a mechanism
    //         similar to the one used here in order to dispatch into extension nodes, but likely without using
    //         System.Xml.Linq APIs to reduce the dependency cost.

    partial class CSharpExpression : IDebugViewExpression, ICSharpPrintableExpression
    {
        /// <summary>
        /// Dispatches the current node to the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor to dispatch to.</param>
        /// <returns>The result of visiting the node.</returns>
        public XNode Accept(IDebugViewExpressionVisitor visitor)
        {
            return new CSharpDebugViewExpressionVisitor(visitor).GetDebugView(this);
        }

        internal string DebugView => this.DebugView().ToString();
    }

    partial class DynamicCSharpArgument
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class CSharpSwitchCase
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class Interpolation
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class ParameterAssignment
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class Conversion
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class SwitchExpressionArm
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class LocalDeclaration
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class CSharpCatchBlock
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class SwitchSection
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class SwitchLabel
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class AwaitInfo
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class EnumeratorInfo
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class InterpolatedStringHandlerInfo
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class CSharpDebugViewExpressionVisitor : CSharpExpressionVisitor
    {
        private readonly IDebugViewExpressionVisitor _parent;
        private readonly Stack<XNode> _nodes = new();

        public CSharpDebugViewExpressionVisitor()
            : this(new DebugViewExpressionVisitor())
        {
        }

        public CSharpDebugViewExpressionVisitor(IDebugViewExpressionVisitor parent)
        {
            _parent = parent;
        }

        public XNode GetDebugView(CSharpExpression expression)
        {
            base.Visit(expression);
            return _nodes.Pop();
        }

        public XNode GetDebugView(DynamicCSharpArgument argument) => Visit(argument);

        public XNode GetDebugView(CSharpSwitchCase switchCase) => Visit(switchCase);

        public XNode GetDebugView(ParameterAssignment assignment) => Visit(assignment);

        public XNode GetDebugView(Interpolation interpolation) => Visit(interpolation);

        public XNode GetDebugView(Conversion conversion) => Visit(conversion);

        public XNode GetDebugView(SwitchExpressionArm arm) => Visit(arm);

        public XNode GetDebugView(LocalDeclaration declaration) => Visit(declaration);

        public XNode GetDebugView(CSharpCatchBlock catchBlock) => Visit(catchBlock);

        public XNode GetDebugView(SwitchSection switchSection) => Visit(switchSection);

        public XNode GetDebugView(SwitchLabel switchLabel) => Visit(switchLabel);

        public XNode GetDebugView(AwaitInfo awaitInfo) => Visit(awaitInfo);

        public XNode GetDebugView(EnumeratorInfo enumeratorInfo) => Visit(enumeratorInfo);

        public XNode GetDebugView(InterpolatedStringHandlerInfo handlerInfo) => Visit(handlerInfo);

        protected internal override Expression VisitArrayAccess(ArrayAccessCSharpExpression node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.Array), Visit(node.Array)),
                Visit(nameof(node.Indexes), node.Indexes)
            };

            return Push(node, args);
        }

        protected internal override Expression VisitAsyncLambda<TDelegate>(AsyncCSharpExpression<TDelegate> node)
        {
            var parameters = Visit(nameof(AsyncCSharpExpression<TDelegate>.Parameters), node.Parameters);

            var body = Visit(node.Body);

            return Push(node, parameters, new XElement(nameof(AsyncCSharpExpression<TDelegate>.Body), body));
        }

        protected internal override Expression VisitAwait(AwaitCSharpExpression node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.Info), Visit(node.Info)),
                new XElement(nameof(node.Operand), Visit(node.Operand))
            };

            return Push(node, args);
        }

        protected internal override AwaitInfo VisitAwaitInfo(StaticAwaitInfo node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.GetAwaiter), Visit(node.GetAwaiter)),
                new XAttribute(nameof(node.IsCompleted), node.IsCompleted),
                new XAttribute(nameof(node.GetResult), node.GetResult)
            };

            _nodes.Push(new XElement(nameof(StaticAwaitInfo), args));

            return node;
        }

        protected internal override AwaitInfo VisitAwaitInfo(DynamicAwaitInfo node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.ResultDiscarded), node.ResultDiscarded)
            };

            if (node.Context != null)
            {
                args.Add(new XAttribute(nameof(node.Context), node.Context));
            }

            _nodes.Push(new XElement(nameof(DynamicAwaitInfo), args));

            return node;
        }

        protected internal override Expression VisitBlock(BlockCSharpExpression node)
        {
            var args = new List<object>();

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

            args.Add(Visit(nameof(node.Statements), node.Statements));

            if (node.ReturnLabel != null)
            {
                args.Add(new XElement(nameof(node.ReturnLabel), _parent.GetDebugView(node.ReturnLabel)));
            }

            return Push(node, args);
        }

        // DESIGN: We can do away with those if we decide to scrap ConditionalAccessCSharpExpression<TExpression> and/or
        //         the specialized conditional node types. We could keep the factories as a convenience to construct the
        //         underyling ConditionalAccess construct.

        protected internal override Expression VisitConditionalArrayIndex(ConditionalArrayIndexCSharpExpression node)
        {
            var array = Visit(node.Array);
            var args = Visit(nameof(node.Indexes), node.Indexes);

            return Push("CSharpConditionalArrayIndex", node, new XElement(nameof(node.Array), array), args);
        }

        protected internal override Expression VisitConditionalIndex(ConditionalIndexCSharpExpression node)
        {
            var obj = Visit(node.Object);
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            return Push("CSharpConditionalIndex", node, new XAttribute(nameof(node.Indexer), node.Indexer), new XElement(nameof(node.Object), obj), args);
        }

        protected internal override Expression VisitConditionalInvocation(ConditionalInvocationCSharpExpression node)
        {
            var expr = Visit(node.Expression);
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            return Push("CSharpConditionalInvoke", node, new XElement(nameof(node.Expression), expr), args);
        }

        protected internal override Expression VisitConditionalMember(ConditionalMemberCSharpExpression node)
        {
            var expr = Visit(node.Expression);

            return Push("CSharpConditionalMemberAccess", node, new XAttribute(nameof(node.Member), node.Member), new XElement(nameof(node.Expression), expr));
        }

        protected internal override Expression VisitConditionalMethodCall(ConditionalMethodCallCSharpExpression node)
        {
            var obj = Visit(node.Object);
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            return Push("CSharpConditionalCall", node, new XAttribute(nameof(node.Method), node.Method), new XElement(nameof(node.Object), obj), args);
        }

        protected internal override Expression VisitDeconstructionAssignment(DeconstructionAssignmentCSharpExpression node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.Left), Visit(node.Left)),
                new XElement(nameof(node.Right), Visit(node.Right)),
                new XElement(nameof(node.Conversion), Visit(node.Conversion))
            };

            return Push(node, args);
        }

        protected internal override Expression VisitDiscard(DiscardCSharpExpression node)
        {
            return Push("CSharpDiscard", node);
        }

        protected internal override Expression VisitDo(DoCSharpStatement node)
        {
            var args = new List<object>();

            if (node.Locals.Count > 0)
            {
                args.Add(Visit(nameof(node.Locals), node.Locals));
            }

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));
            args.Add(new XElement(nameof(node.Test), Visit(node.Test)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement(nameof(node.ContinueLabel), _parent.GetDebugView(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected internal override DynamicCSharpArgument VisitDynamicArgument(DynamicCSharpArgument node)
        {
            var expr = Visit(node.Expression);

            var args = new List<object>();

            if (node.Name != null)
            {
                args.Add(new XAttribute(nameof(node.Name), node.Name));
            }

            if (node.Flags != CSharpArgumentInfoFlags.None)
            {
                args.Add(new XAttribute(nameof(node.Flags), node.Flags));
            }

            args.Add(new XElement(nameof(node.Expression), expr));

            var res = new XElement(nameof(DynamicCSharpArgument), args);
            _nodes.Push(res);

            return node;
        }

        protected internal override Expression VisitDynamicBinary(BinaryDynamicCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.OperationNodeType), node.OperationNodeType)
            };

            VisitDynamicCSharpExpression(node, args);

            args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            args.Add(new XElement(nameof(node.Right), Visit(node.Right)));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicConvert(ConvertDynamicCSharpExpression node)
        {
            var args = VisitDynamicCSharpExpression(node);

            // NB: Type is always added
            args.Add(new XElement(nameof(node.Expression), Visit(node.Expression)));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicGetIndex(GetIndexDynamicCSharpExpression node)
        {
            var args = VisitDynamicCSharpExpression(node);

            args.Add(new XElement(nameof(node.Object), Visit(node.Object)));
            args.Add(Visit(nameof(node.Arguments), node.Arguments, Visit));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicGetMember(GetMemberDynamicCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.Name), node.Name)
            };

            VisitDynamicCSharpExpression(node, args);

            args.Add(new XElement(nameof(node.Object), Visit(node.Object)));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicInvoke(InvokeDynamicCSharpExpression node)
        {
            var args = VisitDynamicCSharpExpression(node);

            args.Add(new XElement(nameof(node.Expression), Visit(node.Expression)));
            args.Add(Visit(nameof(node.Arguments), node.Arguments, Visit));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicInvokeConstructor(InvokeConstructorDynamicCSharpExpression node)
        {
            var args = VisitDynamicCSharpExpression(node);

            // NB: Type is always added
            args.Add(Visit(nameof(node.Arguments), node.Arguments, Visit));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicInvokeMember(InvokeMemberDynamicCSharpExpression node)
        {
            var args = new List<object>();

            if (node.Target != null)
            {
                args.Add(new XAttribute(nameof(node.Target), node.Target));
            }

            args.Add(new XAttribute(nameof(node.Name), node.Name));

            if (node.TypeArguments.Count > 0)
            {
                args.Add(new XAttribute(nameof(node.TypeArguments), string.Join(", ", node.TypeArguments)));
            }

            VisitDynamicCSharpExpression(node, args);

            if (node.Object != null)
            {
                args.Add(new XElement(nameof(node.Object), Visit(node.Object)));
            }

            args.Add(Visit(nameof(node.Arguments), node.Arguments, Visit));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicUnary(UnaryDynamicCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.OperationNodeType), node.OperationNodeType)
            };

            VisitDynamicCSharpExpression(node, args);

            args.Add(new XElement(nameof(node.Operand), Visit(node.Operand)));

            return Push(node, args);
        }

        private static List<object> VisitDynamicCSharpExpression(DynamicCSharpExpression node)
        {
            var args = new List<object>();

            VisitDynamicCSharpExpression(node, args);

            return args;
        }

        private static void VisitDynamicCSharpExpression(DynamicCSharpExpression node, List<object> args)
        {
            if (node.Flags != CSharpBinderFlags.None)
            {
                args.Add(new XAttribute(nameof(node.Flags), node.Flags));
            }

            if (node.Context != null)
            {
                args.Add(new XAttribute(nameof(node.Context), node.Context));
            }
        }

        protected internal override Expression VisitDynamicBinaryAssign(AssignBinaryDynamicCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.OperationNodeType), node.OperationNodeType)
            };

            VisitDynamicCSharpExpression(node, args);

            args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            args.Add(new XElement(nameof(node.Right), Visit(node.Right)));

            return Push(node, args);
        }

        protected internal override Expression VisitDynamicUnaryAssign(AssignUnaryDynamicCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.OperationNodeType), node.OperationNodeType)
            };

            VisitDynamicCSharpExpression(node, args);

            args.Add(new XElement(nameof(node.Operand), Visit(node.Operand)));

            return Push(node, args);
        }

        protected internal override Expression VisitFor(ForCSharpStatement node)
        {
            var args = new List<object>();

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

            if (node.Initializers.Count > 0)
            {
                args.Add(Visit(nameof(node.Initializers), node.Initializers));
            }

            if (node.Locals.Count > 0)
            {
                args.Add(Visit(nameof(node.Locals), node.Locals));
            }

            if (node.Test != null)
            {
                args.Add(new XElement(nameof(node.Test), Visit(node.Test)));
            }

            if (node.Iterators.Count > 0)
            {
                args.Add(Visit(nameof(node.Iterators), node.Iterators));
            }

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement(nameof(node.ContinueLabel), _parent.GetDebugView(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected internal override Expression VisitForEach(ForEachCSharpStatement node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.EnumeratorInfo), Visit(node.EnumeratorInfo)),
                Visit(nameof(node.Variables), node.Variables)
            };

            if (node.AwaitInfo != null)
            {
                args.Add(new XElement(nameof(node.AwaitInfo), Visit(node.AwaitInfo)));
            }

            if (node.Conversion != null)
            {
                args.Add(new XElement(nameof(node.Conversion), Visit(node.Conversion)));
            }

            if (node.Deconstruction != null)
            {
                args.Add(new XElement(nameof(node.Deconstruction), Visit(node.Deconstruction)));
            }

            args.Add(new XElement(nameof(node.Collection), Visit(node.Collection)));

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement(nameof(node.ContinueLabel), _parent.GetDebugView(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected internal override EnumeratorInfo VisitEnumeratorInfo(EnumeratorInfo node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.IsAsync), node.IsAsync),
                new XAttribute(nameof(node.CollectionType), node.CollectionType),
                new XAttribute(nameof(node.ElementType), node.ElementType),
                new XAttribute(nameof(node.NeedsDisposal), node.NeedsDisposal),
                new XElement(nameof(node.GetEnumerator), Visit(node.GetEnumerator)),
                new XElement(nameof(node.MoveNext), Visit(node.MoveNext)),
                new XAttribute(nameof(node.Current), node.Current),
            };

            if (node.CurrentConversion != null)
            {
                args.Add(new XElement(nameof(node.CurrentConversion), Visit(node.CurrentConversion)));
            }

            if (node.DisposeAwaitInfo != null)
            {
                args.Add(new XElement(nameof(node.DisposeAwaitInfo), Visit(node.DisposeAwaitInfo)));
            }

            if (node.PatternDispose != null)
            {
                args.Add(new XElement(nameof(node.PatternDispose), Visit(node.PatternDispose)));
            }

            _nodes.Push(new XElement(nameof(EnumeratorInfo), args));

            return node;
        }

        protected internal override Expression VisitFromEndIndex(FromEndIndexCSharpExpression node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.Operand), Visit(node.Operand))
            };

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            return Push(node, args);
        }

        protected internal override Expression VisitGotoCase(GotoCaseCSharpStatement node)
        {
            return Push("CSharpGotoCase", node, new XAttribute(nameof(node.Value), node.Value ?? "null"));
        }

        protected internal override Expression VisitGotoDefault(GotoDefaultCSharpStatement node)
        {
            return Push("CSharpGotoDefault", node);
        }

        protected internal override Expression VisitGotoLabel(GotoLabelCSharpStatement node)
        {
            return Push(node, new XElement(nameof(node.Target), _parent.GetDebugView(node.Target)));
        }

        protected internal override Expression VisitIndex(IndexCSharpExpression node)
        {
            var obj = Visit(node.Object);
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            return Push(node, new XAttribute(nameof(node.Indexer), node.Indexer), new XElement(nameof(node.Object), obj), args);
        }

        protected internal override Expression VisitIndexerAccess(IndexerAccessCSharpExpression node)
        {
            var obj = Visit(node.Object);
            var arg = Visit(node.Argument);

            return Push(node, new XAttribute(nameof(node.LengthOrCount), node.LengthOrCount), new XAttribute(nameof(node.IndexOrSlice), node.IndexOrSlice), new XElement(nameof(node.Object), obj), new XElement(nameof(node.Argument), arg));
        }

        protected internal override Expression VisitInterpolatedString(InterpolatedStringCSharpExpression node)
        {
            var args = new List<object>();

            if (node.Interpolations.Count > 0)
            {
                args.Add(Visit(nameof(node.Interpolations), node.Interpolations, Visit));
            }

            return Push(node, args);
        }

        protected internal override Interpolation VisitInterpolationStringInsert(InterpolationStringInsert node)
        {
            var args = new List<object?>();

            if (node.Format != null)
            {
                args.Add(new XAttribute(nameof(node.Format), node.Format));
            }

            if (node.Alignment != null)
            {
                args.Add(new XAttribute(nameof(node.Alignment), node.Alignment.Value));
            }

            args.Add(Visit(node.Value));

            _nodes.Push(new XElement(nameof(InterpolationStringLiteral), args));
            return node;
        }

        protected internal override Interpolation VisitInterpolationStringLiteral(InterpolationStringLiteral node)
        {
            _nodes.Push(new XElement(nameof(InterpolationStringLiteral), new XElement("Value", node.Value)));
            return node;
        }

        protected internal override Expression VisitInterpolatedStringHandlerConversion(InterpolatedStringHandlerConversionCSharpExpression node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.Info), Visit(node.Info)),
                new XElement(nameof(node.Operand), Visit(node.Operand))
            };

            return Push(node, args);
        }

        protected internal override InterpolatedStringHandlerInfo VisitInterpolatedStringHandlerInfo(InterpolatedStringHandlerInfo node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.Type), node.Type),
                new XElement(nameof(node.Construction), Visit(node.Construction))
            };

            if (node.ArgumentIndices.Count > 0)
            {
                var indices = new List<object>();

                for (int i = 0, n = node.ArgumentIndices.Count; i < n; i++)
                {
                    var index = node.ArgumentIndices[i];

                    indices.Add(new XElement("Index", new XAttribute("Value", index)));
                }

                args.Add(new XElement(nameof(node.ArgumentIndices), indices));
            }

            args.Add(Visit(nameof(node.Append), node.Append, Visit));

            _nodes.Push(new XElement(nameof(InterpolatedStringHandlerInfo), args));

            return node;
        }

        protected internal override Expression VisitInvocation(InvocationCSharpExpression node)
        {
            var expr = Visit(node.Expression);
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            return Push(node, new XElement(nameof(node.Expression), expr), args);
        }

        protected internal override Expression VisitLock(LockCSharpStatement node)
        {
            var expr = Visit(node.Expression);
            var body = Visit(node.Body);

            return Push(node, new XElement(nameof(node.Expression), expr), new XElement(nameof(node.Body), body));
        }

        protected internal override Expression VisitMethodCall(MethodCallCSharpExpression node)
        {
            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);

            if (node.Object != null)
            {
                var obj = Visit(node.Object);
                return Push(node, new XAttribute(nameof(node.Method), node.Method), new XElement(nameof(node.Object), obj), args);
            }
            else
            {
                return Push(node, new XAttribute(nameof(node.Method), node.Method), args);
            }
        }

        protected internal override Expression VisitNew(NewCSharpExpression node)
        {
            var nodes = new List<object>();

            if (node.Constructor != null)
            {
                nodes.Add(new XAttribute(nameof(node.Constructor), node.Constructor));
            }

            var args = Visit(nameof(node.Arguments), node.Arguments, Visit);
            nodes.Add(args);

            return Push(node, nodes);
        }

        protected internal override Expression VisitNewMultidimensionalArrayInit(NewMultidimensionalArrayInitCSharpExpression node)
        {
            var exprs = Visit(nameof(node.Expressions), node.Expressions);
            var bounds = string.Join(", ", node.Bounds);

            return Push(node, new XAttribute("Bounds", bounds), exprs);
        }

        protected override ParameterAssignment VisitParameterAssignment(ParameterAssignment node)
        {
            var expr = Visit(node.Expression);

            var res = new XElement(nameof(ParameterAssignment), new XAttribute(nameof(node.Parameter), node.Parameter), new XElement(nameof(node.Expression), expr));
            _nodes.Push(res);

            return node;
        }

        protected internal override Expression VisitRange(RangeCSharpExpression node)
        {
            var args = new List<object>();

            if (node.Left != null)
            {
                args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            }

            if (node.Right != null)
            {
                args.Add(new XElement(nameof(node.Right), Visit(node.Right)));
            }

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            return Push(node, args);
        }

        protected internal override Expression VisitSwitch(SwitchCSharpStatement node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.SwitchValue), Visit(node.SwitchValue))
            };

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

            args.Add(Visit(nameof(node.Cases), node.Cases, Visit));

            args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));

            return Push(node, args);
        }

        protected internal override CSharpSwitchCase VisitSwitchCase(CSharpSwitchCase node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.TestValues), string.Join(", ", node.TestValues.Select(EscapeToString))),
                Visit(nameof(node.Statements), node.Statements)
            };

            _nodes.Push(new XElement(nameof(CSharpSwitchCase), args));
            return node;
        }

        private static string EscapeToString(object? obj)
        {
            return obj switch
            {
                null => "null",
                string s => "\"" + s.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"",
                _ => obj.ToString() ?? "null",
            };
        }

        protected internal override Expression VisitSwitch(PatternSwitchCSharpStatement node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.SwitchValue), Visit(node.SwitchValue))
            };

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

            args.Add(Visit(nameof(node.Sections), node.Sections, Visit));

            args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));

            return Push(node, args);
        }

        protected internal override SwitchSection VisitSwitchSection(SwitchSection node)
        {
            var args = new List<object>();

            if (node.Locals.Count > 0)
            {
                args.Add(Visit(nameof(node.Locals), node.Locals));
            }

            args.Add(Visit(nameof(node.Labels), node.Labels, Visit));

            args.Add(Visit(nameof(node.Statements), node.Statements));

            _nodes.Push(new XElement(nameof(SwitchSection), args));
            return node;
        }

        protected internal override SwitchLabel VisitSwitchLabel(SwitchLabel node)
        {
            var args = new List<object>();

            if (node.Label != null)
            {
                args.Add(new XElement(nameof(node.Label), _parent.GetDebugView(node.Label)));
            }

            args.Add(new XElement(nameof(node.Pattern), Visit(node.Pattern)));

            if (node.WhenClause != null)
            {
                args.Add(new XElement(nameof(node.WhenClause), Visit(node.WhenClause)));
            }

            _nodes.Push(new XElement(nameof(SwitchLabel), args));
            return node;
        }

        protected internal override Expression VisitWhile(WhileCSharpStatement node)
        {
            var args = new List<object>();

            if (node.Locals.Count > 0)
            {
                args.Add(Visit(nameof(node.Locals), node.Locals));
            }

            args.Add(new XElement(nameof(node.Test), Visit(node.Test)));
            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            if (node.BreakLabel != null)
            {
                args.Add(new XElement(nameof(node.BreakLabel), _parent.GetDebugView(node.BreakLabel)));
            }

            if (node.ContinueLabel != null)
            {
                args.Add(new XElement(nameof(node.ContinueLabel), _parent.GetDebugView(node.ContinueLabel)));
            }

            return Push(node, args);
        }

        protected internal override Expression VisitUsing(UsingCSharpStatement node)
        {
            var args = new List<object>();

            if (node.AwaitInfo != null)
            {
                args.Add(new XElement(nameof(node.AwaitInfo), Visit(node.AwaitInfo)));
            }

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

            if (node.Resource != null)
            {
                args.Add(new XElement(nameof(node.Resource), Visit(node.Resource)));
            }
            else
            {
                args.Add(Visit(nameof(node.Declarations), node.Declarations!, Visit));
            }

            if (node.PatternDispose != null)
            {
                args.Add(new XElement(nameof(node.PatternDispose), Visit(node.PatternDispose)));
            }

            args.Add(new XElement(nameof(node.Body), Visit(node.Body)));

            return Push(node, args);
        }

        protected internal override LocalDeclaration VisitLocalDeclaration(LocalDeclaration node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.Variable), Visit(node.Variable)),
                new XElement(nameof(node.Expression), Visit(node.Expression))
            };

            var res = new XElement(nameof(LocalDeclaration), nodes);
            _nodes.Push(res);

            return node;
        }

        protected internal override Expression VisitTry(TryCSharpStatement node)
        {
            var args = new List<object>
            {
                new XElement(nameof(node.TryBlock), Visit(node.TryBlock))
            };

            if (node.CatchBlocks.Count > 0)
            {
                args.Add(Visit(nameof(node.CatchBlocks), node.CatchBlocks, Visit));
            }

            if (node.FinallyBlock != null)
            {
                args.Add(new XElement(nameof(node.FinallyBlock), Visit(node.FinallyBlock)));
            }

            return Push(node, args);
        }

        protected internal override CSharpCatchBlock VisitCatchBlock(CSharpCatchBlock node)
        {
            var args = new List<object>();

            if (node.Variables.Count > 0)
            {
                args.Add(Visit(nameof(node.Variables), node.Variables));
            }

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

            _nodes.Push(new XElement(nameof(CSharpCatchBlock), args));

            return node;
        }

        protected internal override Expression VisitConditionalAccess(ConditionalAccessCSharpExpression node)
        {
            var receiver = new XElement(nameof(node.Receiver), Visit(node.Receiver));
            var nonNullReceiver = new XElement(nameof(node.NonNullReceiver), Visit(node.NonNullReceiver));
            var whenNotNull = new XElement(nameof(node.WhenNotNull), Visit(node.WhenNotNull));

            return Push(node, receiver, nonNullReceiver, whenNotNull);
        }

        protected internal override Expression VisitConditionalReceiver(ConditionalReceiver node)
        {
            var id = _parent.MakeInstanceId(node);

            var res = new XElement(nameof(ConditionalReceiver), new XAttribute("Id", id), new XAttribute(nameof(node.Type), node.Type));
            _nodes.Push(res);

            return node;
        }

        protected internal override Expression VisitBinaryAssign(AssignBinaryCSharpExpression node)
        {
            var args = new List<object>();

            if (node.IsLifted)
            {
                args.Add(new XAttribute(nameof(node.IsLifted), node.IsLifted));
            }

            if (node.IsLiftedToNull)
            {
                args.Add(new XAttribute(nameof(node.IsLiftedToNull), node.IsLiftedToNull));
            }

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            args.Add(new XElement(nameof(node.Right), Visit(node.Right)));

            if (node.LeftConversion != null)
            {
                args.Add(new XElement(nameof(node.LeftConversion), Visit(node.LeftConversion)));
            }

            if (node.FinalConversion != null)
            {
                args.Add(new XElement(nameof(node.FinalConversion), Visit(node.FinalConversion)));
            }

            return Push(node, args);
        }

        protected internal override Expression VisitEventAssign(EventAssignCSharpExpression node)
        {
            var args = new List<object>
            {
                new XAttribute(nameof(node.Event), node.Event),
                new XElement(nameof(node.Object), Visit(node.Object)),
                new XElement(nameof(node.Handler), Visit(node.Handler))
            };

            return Push(node, args);
        }

        protected internal override Expression VisitUnaryAssign(AssignUnaryCSharpExpression node)
        {
            var args = new List<object>();

            if (node.Method != null)
            {
                args.Add(new XAttribute(nameof(node.Method), node.Method));
            }

            args.Add(new XElement(nameof(node.Operand), Visit(node.Operand)));

            return Push(node, args);
        }

        protected internal override Expression VisitTupleLiteral(TupleLiteralCSharpExpression node)
        {
            var args = new List<XNode>();

            if (node.ArgumentNames != null)
            {
                for (int i = 0, n = node.Arguments.Count; i < n; i++)
                {
                    var name = node.ArgumentNames[i];

                    if (name != null)
                    {
                        args.Add(new XElement("Argument", new XAttribute("Name", name), Visit(node.Arguments[i])));
                    }
                    else
                    {
                        args.Add(new XElement("Argument", Visit(node.Arguments[i])));
                    }
                }
            }
            else
            {
                foreach (var expression in node.Arguments)
                {
                    args.Add(Visit(expression));
                }
            }

            return Push("TupleLiteral", node, new XElement(nameof(node.Arguments), args));
        }

        protected internal override Expression VisitTupleConvert(TupleConvertCSharpExpression node)
        {
            var args = new List<XNode>
            {
                new XElement(nameof(node.Operand), Visit(node.Operand)),
                Visit(nameof(node.ElementConversions), node.ElementConversions)
            };

            return Push(node, args);
        }

        protected internal override Expression VisitTupleBinary(TupleBinaryCSharpExpression node)
        {
            var args = new List<object>();

            if (node.IsLifted)
            {
                args.Add(new XAttribute(nameof(node.IsLifted), node.IsLifted));
            }

            args.Add(new XElement(nameof(node.Left), Visit(node.Left)));
            args.Add(new XElement(nameof(node.Right), Visit(node.Right)));
            args.Add(Visit(nameof(node.EqualityChecks), node.EqualityChecks));

            return Push(node, args);
        }

        protected internal override Expression VisitWith(WithCSharpExpression node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.Object), Visit(node.Object)),
                Visit(nameof(node.Initializers), node.Initializers, Visit),
            };

            if (node.Clone != null)
            {
                nodes.Add(new XAttribute(nameof(node.Clone), node.Clone));
            }

            if (node.Members != null)
            {
                var members = new XElement(nameof(node.Members), node.Members.Select(m => new XElement("Member", m)));
                nodes.Add(members);
            }

            return Push(node, nodes);
        }

        protected internal override Expression VisitIsPattern(IsPatternCSharpExpression node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.Expression), Visit(node.Expression)),
                new XElement(nameof(node.Pattern), Visit(node.Pattern)),
            };

            return Push(node, nodes);
        }

        protected internal override Expression VisitSwitchExpression(SwitchCSharpExpression node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.Expression), Visit(node.Expression)),
                Visit(nameof(node.Arms), node.Arms, Visit),
            };

            return Push(node, nodes);
        }

        protected internal override SwitchExpressionArm VisitSwitchExpressionArm(SwitchExpressionArm node)
        {
            var nodes = new List<object>
            {
                Visit(nameof(node.Variables), node.Variables),
                new XElement(nameof(node.Pattern), Visit(node.Pattern))
            };

            if (node.WhenClause != null)
            {
                nodes.Add(new XElement(nameof(node.WhenClause), Visit(node.WhenClause)));
            }

            nodes.Add(new XElement(nameof(node.Value), Visit(node.Value)));

            var res = new XElement(nameof(SwitchExpressionArm), nodes);
            _nodes.Push(res);

            return node;
        }

        protected internal override MemberInitializer VisitMemberInitializer(MemberInitializer node)
        {
            var expr = Visit(node.Expression);

            var res = new XElement(nameof(MemberInitializer), new XAttribute(nameof(node.Member), node.Member), new XElement(nameof(node.Expression), expr));
            _nodes.Push(res);

            return node;
        }

        protected internal override Conversion VisitSimpleConversion(SimpleConversion node)
        {
            var res = new XElement(nameof(SimpleConversion), new XElement(nameof(node.Conversion), Visit(node.Conversion)));
            _nodes.Push(res);

            return node;
        }

        protected internal override Conversion VisitDeconstructionConversion(DeconstructionConversion node)
        {
            var res =
                new XElement(nameof(DeconstructionConversion),
                new XElement(nameof(node.Deconstruct), Visit(node.Deconstruct)),
                Visit(nameof(node.Conversions), node.Conversions, Visit)
            );

            _nodes.Push(res);

            return node;
        }

        private XNode Visit(ParameterAssignment node)
        {
            VisitParameterAssignment(node);
            return _nodes.Pop();
        }

        private XNode Visit(DynamicCSharpArgument node)
        {
            VisitDynamicArgument(node);
            return _nodes.Pop();
        }

        private XNode Visit(CSharpSwitchCase node)
        {
            VisitSwitchCase(node);
            return _nodes.Pop();
        }

        private XNode Visit(ConditionalReceiver node)
        {
            VisitConditionalReceiver(node);
            return _nodes.Pop();
        }

        private XNode Visit(AwaitInfo node)
        {
            VisitAwaitInfo(node);
            return _nodes.Pop();
        }

        private XNode Visit(EnumeratorInfo node)
        {
            VisitEnumeratorInfo(node);
            return _nodes.Pop();
        }

        private XNode Visit(InterpolatedStringHandlerInfo node)
        {
            VisitInterpolatedStringHandlerInfo(node);
            return _nodes.Pop();
        }

        private XNode Visit(Interpolation node)
        {
            VisitInterpolation(node);
            return _nodes.Pop();
        }

        private XNode Visit(MemberInitializer node)
        {
            VisitMemberInitializer(node);
            return _nodes.Pop();
        }

        private XNode Visit(Conversion node)
        {
            VisitConversion(node);
            return _nodes.Pop();
        }

        private XNode Visit(SwitchExpressionArm node)
        {
            VisitSwitchExpressionArm(node);
            return _nodes.Pop();
        }

        private XNode Visit(LocalDeclaration node)
        {
            VisitLocalDeclaration(node);
            return _nodes.Pop();
        }

        private XNode Visit(CSharpCatchBlock node)
        {
            VisitCatchBlock(node);
            return _nodes.Pop();
        }

        private XNode Visit(SwitchSection node)
        {
            VisitSwitchSection(node);
            return _nodes.Pop();
        }

        private XNode Visit(SwitchLabel node)
        {
            VisitSwitchLabel(node);
            return _nodes.Pop();
        }

        [return: NotNullIfNotNull("expression")] // TODO: C# 11.0 nameof
        protected new XNode? Visit(Expression? expression)
        {
            return _parent.GetDebugView(expression);
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

        protected static XNode Visit<T>(string name, IEnumerable<T> expressions, Func<T, XNode> visit)
        {
            var res = new List<XNode>();

            foreach (var expression in expressions)
            {
                res.Add(visit(expression));
            }

            return new XElement(name, res);
        }

        protected T Push<T>(T node, params object[] content)
            where T : CSharpExpression
        {
            return Push("CSharp" + node.CSharpNodeType.ToString(), node, content);
        }

        protected T Push<T>(string name, T node, params object[] content)
            where T : CSharpExpression
        {
            _nodes.Push(new XElement(name, new XAttribute(nameof(node.Type), node.Type), content));
            return node;
        }
    }
}
