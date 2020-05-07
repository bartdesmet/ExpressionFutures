// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an array access operation.
    /// </summary>
    public sealed partial class ArrayAccessCSharpExpression : CSharpExpression
    {
        internal ArrayAccessCSharpExpression(Expression array, ReadOnlyCollection<Expression> indexes)
        {
            Array = array;
            Indexes = indexes;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />.
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ArrayAccess;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Indexes[0].Type == typeof(Range) ? Array.Type : Array.Type.GetElementType();

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the array getting accessed.
        /// </summary>
        public Expression Array { get; }

        /// <summary>
        /// Gets the indexes that will be used to index the array.
        /// </summary>
        public ReadOnlyCollection<Expression> Indexes { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitArrayAccess(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="array">The <see cref="Array" /> property of the result.</param>
        /// <param name="indexes">The <see cref="Indexes" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ArrayAccessCSharpExpression Update(Expression array, IEnumerable<Expression> indexes)
        {
            if (array == Array && indexes == Indexes)
            {
                return this;
            }

            return CSharpExpression.ArrayAccess(array, indexes);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            if (IsSimpleArrayAccess())
            {
                return ReduceSimple();
            }
            else if (IsIndexArrayAccess())
            {
                return ReduceIndex();
            }
            else if (IsRangeArrayAccess())
            {
                return ReduceRange();
            }

            throw ContractUtils.Unreachable;
        }

        internal Expression Reduce(Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            if (IsSimpleArrayAccess())
            {
                return ReduceSimple();
            }
            else if (IsIndexArrayAccess())
            {
                return ReduceIndex(makeVariable, statements);
            }
            else if (IsRangeArrayAccess())
            {
                return ReduceRange();
            }

            throw ContractUtils.Unreachable;
        }

        private bool IsSimpleArrayAccess() => Indexes.All(index => index.Type == typeof(int));

        private bool IsIndexArrayAccess() => Indexes.Count == 1 && Indexes[0].Type == typeof(Index);

        private bool IsRangeArrayAccess() => Indexes.Count == 1 && Indexes[0].Type == typeof(Range);

        private Expression ReduceSimple()
        {
            return Expression.ArrayAccess(Array, Indexes);
        }

        private Expression ReduceIndex()
        {
            // Lowered code:
            // ref var receiver = receiverExpr;
            // int length = receiver.length;
            // int index = argument.GetOffset(length);
            // receiver[index];

            var arrayVariable = Expression.Parameter(Array.Type, "__arr");

            return Expression.Block(
                new[] { arrayVariable },
                Expression.Assign(arrayVariable, Array),
                MakeArrayAccess(arrayVariable)
            );
        }

        private Expression ReduceIndex(Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            //
            // NB: This is called for the CSharpExpression variants of MethodCall, Invocation, and New. It correctly supports
            //     passing an ArrayAccessCSharpExpression by reference.
            //

            // Lowered code:
            // ref var receiver = receiverExpr;
            // int length = receiver.length;
            // int index = argument.GetOffset(length);
            // receiver[index];

            var arrayVariable = makeVariable(Array.Type, "__arr");

            statements.Add(Expression.Assign(arrayVariable, Array));

            return MakeArrayAccess(arrayVariable);
        }

        private Expression MakeArrayAccess(Expression arrayVariable)
        {
            return Expression.ArrayAccess(
                arrayVariable,
                GetIntIndexExpression(arrayVariable, Indexes[0])
            );
        }

        private Expression ReduceRange()
        {
            // System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray(array, Range)

            var elemType = Array.Type.GetElementType(); // REVIEW
            var getSubArrayMethod = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetSubArray), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).MakeGenericMethod(elemType);

            return Expression.Call(getSubArrayMethod, Array, Indexes[0]);
        }

        private static Expression GetIntIndexExpression(Expression array, Expression index)
        {
            if (index is FromEndIndexCSharpExpression hat)
            {
                return Expression.Subtract(Expression.ArrayLength(array), hat.Operand);
            }
            else
            {
                var getOffsetMethod = typeof(Index).GetNonGenericMethod(nameof(System.Index.GetOffset), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, new[] { typeof(int) });

                return Expression.Call(index, getOffsetMethod, Expression.ArrayLength(array));
            }
        }

        internal Expression ReduceAssign(Func<Expression, Expression> assign)
        {
            var firstIndex = Indexes[0];
            var indexType = firstIndex.Type;

            if (Indexes.Count > 1 || indexType == typeof(int))
            {
                var temps = new List<ParameterExpression>(Indexes.Count + 1);
                var stmts = new List<Expression>(Indexes.Count + 2);

                var array = Array;

                if (!Helpers.IsPure(array))
                {
                    var arrayVariable = Expression.Parameter(Array.Type, "__arr");

                    temps.Add(arrayVariable);
                    stmts.Add(Expression.Assign(arrayVariable, Array));

                    array = arrayVariable;
                }

                var indexes = new List<Expression>(Indexes.Count);

                for (int i = 0, n = Indexes.Count; i < n; i++)
                {
                    var index = Indexes[i];

                    if (Helpers.IsPure(index))
                    {
                        indexes.Add(index);
                    }
                    else
                    {
                        var indexVariable = Expression.Parameter(index.Type, "__idx" + i);

                        temps.Add(indexVariable);
                        stmts.Add(Expression.Assign(indexVariable, index));

                        indexes.Add(indexVariable);
                    }
                }

                var access = assign(Expression.ArrayAccess(array, indexes));

                if (stmts.Count > 0)
                {
                    stmts.Add(access);

                    return Expression.Block(temps, stmts);
                }
                else
                {
                    return access;
                }
            }

            if (indexType == typeof(Index))
            {
                // Lowered code:
                // ref var receiver = receiverExpr;
                // int length = receiver.length;
                // int index = argument.GetOffset(length);
                // receiver[index] = rhs;

                var arrayVariable = Expression.Parameter(Array.Type, "__arr");
                var indexVariable = Expression.Parameter(typeof(int), "__idx");

                // NB: We don't have ref locals in expression trees, so we may end up with
                //
                //     receiver[index] = ... receiver[index] ...
                //
                //     for compound assignments, which is benign but may incur multiple bounds checks. Alternatively, we could
                //     dispatch into the RuntimeOpsEx.WithByRef helper method.

                return Expression.Block(
                    new[] { arrayVariable, indexVariable },
                    Expression.Assign(arrayVariable, Array),
                    Expression.Assign(
                        indexVariable,
                        GetIntIndexExpression(arrayVariable, firstIndex)
                    ),
                    assign(
                        Expression.ArrayAccess(
                            arrayVariable,
                            indexVariable
                        )
                    )
                );
            }

            throw ContractUtils.Unreachable;
        }

        //
        // BUG: This node can't be passed to a ref parameter. E.g. Interlocked.Exchange(ref xs[^i], val)
        //      See test code for an example and explanation.
        //

        // REVIEW: Writes to a field in an array of mutable structs. E.g. xs[^i].foo = 42
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an expression representing an array access operation.
        /// </summary>
        /// <param name="array">The array to access.</param>
        /// <param name="index">The index to access.</param>
        /// <returns>A new <see cref="ArrayAccessCSharpExpression"/> instance representing the array access operation.</returns>
        public static ArrayAccessCSharpExpression ArrayAccess(Expression array, Expression index)
        {
            return ArrayAccess(array, new[] { index });
        }

        /// <summary>
        /// Creates an expression representing an array access operation.
        /// </summary>
        /// <param name="array">The array to access.</param>
        /// <param name="indexes">The indexes used to access the array.</param>
        /// <returns>A new <see cref="ArrayAccessCSharpExpression"/> instance representing the array access operation.</returns>
        public static new ArrayAccessCSharpExpression ArrayAccess(Expression array, params Expression[] indexes)
        {
            return ArrayAccess(array, (IEnumerable<Expression>)indexes);
        }

        /// <summary>
        /// Creates an expression representing an array access operation.
        /// </summary>
        /// <param name="array">The array to access.</param>
        /// <param name="indexes">The indexes used to access the array.</param>
        /// <returns>A new <see cref="ArrayAccessCSharpExpression"/> instance representing the array access operation.</returns>
        public static new ArrayAccessCSharpExpression ArrayAccess(Expression array, IEnumerable<Expression> indexes)
        {
            RequiresCanRead(array, nameof(array));

            var arrayType = array.Type;

            if (!arrayType.IsArray)
            {
                throw LinqError.ArgumentMustBeArray();
            }

            var indexesList = indexes.ToReadOnly();

            if (arrayType.GetArrayRank() != indexesList.Count)
            {
                throw LinqError.IncorrectNumberOfIndexes();
            }

            foreach (var index in indexesList)
            {
                RequiresCanRead(index, nameof(indexes));
            }

            if (indexesList.Count == 1)
            {
                var index = indexesList[0];

                if (index.Type == typeof(Index) || index.Type == typeof(Range))
                {
                    return new ArrayAccessCSharpExpression(array, indexesList);
                }
            }

            foreach (var index in indexesList)
            {
                if (index.Type != typeof(int))
                {
                    throw LinqError.ArgumentMustBeArrayIndexType();
                }
            }

            return new ArrayAccessCSharpExpression(array, indexesList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ArrayAccessCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitArrayAccess(ArrayAccessCSharpExpression node)
        {
            return node.Update(Visit(node.Array), Visit(node.Indexes));
        }
    }
}
