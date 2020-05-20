// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions.Compiler
{
    /// <summary>
    /// Utility to reduce all extension nodes, except for await, into more primitive forms.
    /// This step is performed early on during async lambda rewriting so we don't have to worry about custom nodes.
    /// </summary>
    internal static class Reducer
    {
        public static Expression Reduce(Expression expression) => new Impl().Visit(expression);

        private sealed class Impl : AwaitTrackingVisitor
        {
            protected override Expression VisitExtension(Expression node)
            {
                if (node is CSharpExpression csharp && csharp.CSharpNodeType == CSharpExpressionType.Await)
                {
                    var await = (AwaitCSharpExpression)csharp;
                    return base.VisitAwait(await);
                }

                return ReduceAndCheck(node);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitBinary(BinaryExpression node)
            {
                // NB: This reduces assignment operators so that the stack spiller doesn't have to worry about it.
                if (node.CanReduce)
                {
                    return ReduceAndCheck(node);
                }

                // NB: Stack spilling of short-circuiting operators would undo the short-circuiting behavior due
                //     to evaluation of left and right for assignment into temps. We can avoid having to worry
                //     about this by reducing these nodes into more primitive conditionals and bitwise operators.
                //
                // CONSIDER: This could be moved to the stack spiller. See remarks on Reduce methods below.

                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.Coalesce:
                        var hasAwait = false;

                        var left = default(Expression);
                        hasAwait |= VisitAndFindAwait(node.Left, out left);

                        var conversion = VisitAndConvert(node.Conversion, nameof(VisitBinary));

                        var right = default(Expression);
                        hasAwait |= VisitAndFindAwait(node.Right, out right);

                        var rewritten = node.Update(left, conversion, right);

                        if (hasAwait)
                        {
                            switch (node.NodeType)
                            {
                                case ExpressionType.AndAlso:
                                case ExpressionType.OrElse:
                                    return ReduceLogical(rewritten);
                                case ExpressionType.Coalesce:
                                    return ReduceCoalesce(rewritten);
                            }
                        }

                        return rewritten;
                }

                return base.VisitBinary(node);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Base class never passes null reference.")]
            protected override Expression VisitUnary(UnaryExpression node)
            {
                // NB: This reduces assignment operators so that the stack spiller doesn't have to worry about it.
                if (node.CanReduce)
                {
                    return ReduceAndCheck(node);
                }

                return base.VisitUnary(node);
            }

            private Expression ReduceAndCheck(Expression node)
            {
                return Visit(node.ReduceAndCheck());
            }
        }

        // CONSIDER: These could be called from the StackSpiller where we know whether the children contain any await
        //           expressions, thus needing a rewrite that keeps the short-circuiting behavior. We keep it here for
        //           the time being to avoid diverging the spiller too much from what we have in LINQ.

        internal static Expression ReduceLogical(BinaryExpression node)
        {
            if (node.Method != null && !node.get_IsLiftedLogical())
            {
                return ReduceLogicalMethod(node);
            }
            else if (node.Left.Type == typeof(bool?))
            {
                return ReduceLogicalLifted(node);
            }
            else if (node.get_IsLiftedLogical())
            {
                return node.ReduceUserdefinedLifted();
            }
            else
            {
                return ReduceLogicalUnlifted(node);
            }
        }
        
        internal static Expression ReduceCoalesce(BinaryExpression node)
        {
            if (TypeUtils.IsNullableType(node.Left.Type))
            {
                return ReduceNullableCoalesce(node);
            }
            else if (node.Conversion != null)
            {
                return ReduceLambdaReferenceCoalesce(node);
            }
            else
            {
                return ReduceReferenceCoalesceWithoutConversion(node);
            }
        }

        private static Expression ReduceLogicalMethod(BinaryExpression node)
        {
            var booleanOperator = TypeUtils.GetBooleanOperator(node.Method.DeclaringType, node.NodeType == ExpressionType.AndAlso ? "op_False" : "op_True");

            var left = Expression.Parameter(node.Left.Type);

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Expression.Condition(
                    Expression.Call(booleanOperator, left),
                    left,
                    Expression.Call(node.Method, left, node.Right)
                )
            );
        }

        private static Expression ReduceLogicalLifted(BinaryExpression node)
        {
            var left = Expression.Parameter(node.Left.Type);
            var right = Expression.Parameter(node.Right.Type);

            var leftValue = Helpers.MakeNullableGetValueOrDefault(left);
            var rightValue = Helpers.MakeNullableGetValueOrDefault(right);

            Expression test;
            Expression bitwise;

            if (node.NodeType == ExpressionType.AndAlso)
            {
                test = Expression.Not(leftValue);
                bitwise = Expression.And(leftValue, rightValue);
            }
            else
            {
                test = leftValue;
                bitwise = Expression.Or(leftValue, rightValue);
            }

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Expression.Condition(
                    Helpers.MakeNullableHasValue(left),
                    Expression.Condition(
                        test,
                        left,
                        Expression.Block(
                            new[] { right },
                            Expression.Assign(right, node.Right),
                            Expression.Condition(
                                Helpers.MakeNullableHasValue(right),
                                Expression.Convert(bitwise, node.Type),
                                Expression.Constant(null, node.Type)
                            )
                        )
                    ),
                    Expression.Constant(null, node.Type)
                )
            );
        }

        private static Expression ReduceLogicalUnlifted(BinaryExpression node)
        {
            var left = Expression.Parameter(node.Left.Type);

            Expression ifTrue;
            Expression ifFalse;

            if (node.NodeType == ExpressionType.AndAlso)
            {
                ifTrue = Expression.And(left, node.Right, node.Method);
                ifFalse = left;
            }
            else
            {
                ifTrue = left;
                ifFalse = Expression.Or(left, node.Right, node.Method);
            }

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Convert(
                    Expression.Condition(left, ifTrue, ifFalse),
                    node.Type
                )
            );
        }

        private static Expression ReduceNullableCoalesce(BinaryExpression node)
        {
            var left = Expression.Parameter(node.Left.Type);

            var ifNull = Helpers.MakeNullableGetValueOrDefault(left);

            if (node.Conversion != null)
            {
                ifNull = Expression.Invoke(
                    node.Conversion,
                    Convert(ifNull, node.Conversion.Parameters[0].Type)
                );
            }

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Expression.Condition(
                    Helpers.MakeNullableHasValue(left),
                    Convert(ifNull, node.Type),
                    Convert(node.Right, node.Type)
                )
            );
        }

        private static Expression ReduceLambdaReferenceCoalesce(BinaryExpression node)
        {
            var left = Expression.Parameter(node.Left.Type);

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Expression.Condition(
                    Expression.ReferenceEqual(left, Expression.Constant(null, left.Type)),
                    node.Right,
                    Expression.Invoke(
                        node.Conversion,
                        left
                    )
                )
            );
        }

        private static Expression ReduceReferenceCoalesceWithoutConversion(BinaryExpression node)
        {
            var left = Expression.Parameter(node.Left.Type);

            return Expression.Block(
                new[] { left },
                Expression.Assign(left, node.Left),
                Expression.Condition(
                    Expression.ReferenceEqual(left, Expression.Constant(null, left.Type)),
                    Convert(node.Right, node.Type),
                    Convert(left, node.Type)
                )
            );
        }

        private static Expression Convert(Expression node, Type type)
        {
            if (!TypeUtils.AreEquivalent(node.Type, type))
            {
                node = Expression.Convert(node, type);
            }

            return node;
        }
    }
}
