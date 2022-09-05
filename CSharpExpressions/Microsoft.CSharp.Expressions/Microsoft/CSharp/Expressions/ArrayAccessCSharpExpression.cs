// Prototyping extended expression trees for C#.
//
// bartde - February 2020

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

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
        public override Type Type => Indexes[0].Type == typeof(Range) ? Array.Type : Array.Type.GetElementType()!;

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitArrayAccess(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="array">The <see cref="Array" /> property of the result.</param>
        /// <param name="indexes">The <see cref="Indexes" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ArrayAccessCSharpExpression Update(Expression array, IEnumerable<Expression> indexes)
        {
            if (array == Array && SameElements(ref indexes, Indexes))
            {
                return this;
            }

            return CSharpExpression.ArrayAccess(array, indexes);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce() => Reduce(length: null);

        internal Expression Reduce(Expression? length)
        {
            if (IsSimpleArrayAccess())
            {
                return ReduceSimple();
            }

            if (Indexes.Count == 1)
            {
                var indexType = Indexes[0].Type;

                if (indexType == typeof(Index))
                {
                    return ReduceIndex();
                }
                else if (indexType == typeof(Range))
                {
                    return ReduceRange();
                }
            }

            throw ContractUtils.Unreachable;

            Expression ReduceIndex()
            {
                // REVIEW: Evaluate array.Length before index argument? May throw NullReferenceException.

                var temps = new List<ParameterExpression>(1);
                var stmts = new List<Expression>(2);

                var array = GetArrayExpression(temps, stmts);

                var index = GetIntIndexExpression(array, length, Indexes[0]);

                stmts.Add(Expression.ArrayAccess(array, index));

                return Comma(temps, stmts);
            }

            Expression ReduceRange()
            {
                // System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray(array, Range)

                var elemType = Array.Type.GetElementType()!; // REVIEW
                var getSubArrayMethod = GetSubArrayMethod.MakeGenericMethod(elemType);

                return Expression.Call(getSubArrayMethod, Array, Indexes[0]);
            }
        }

        internal Expression Reduce(Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            //
            // NB: This is called for the CSharpExpression variants of MethodCall, Invocation, and New. It correctly supports
            //     passing an ArrayAccessCSharpExpression by reference.
            //

            if (IsSimpleArrayAccess())
            {
                return ReduceSimple();
            }

            if (Indexes.Count == 1)
            {
                var indexType = Indexes[0].Type;

                if (indexType == typeof(Index))
                {
                    return ReduceIndex();
                }
            }

            throw ContractUtils.Unreachable;

            Expression ReduceIndex()
            {
                var array = GetArrayExpression(makeVariable, statements);

                var index = GetIntIndexExpression(array, length: null, Indexes[0]);

                var indexVariable = makeVariable(index.Type, "__idx");

                statements.Add(Expression.Assign(indexVariable, index));

                return Expression.ArrayAccess(array, indexVariable);
            }
        }

        internal Expression ReduceAssign(Func<Expression, Expression> assign)
        {
            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var arrayAccess = ReduceAssign(temps, stmts);

            stmts.Add(assign(arrayAccess));

            return Helpers.Comma(temps, stmts);
        }

        internal IndexExpression ReduceAssign(List<ParameterExpression> temps, List<Expression> stmts)
        {
            var firstIndex = Indexes[0];
            var indexType = firstIndex.Type;

            if (Indexes.Count > 1 || indexType == typeof(int))
            {
                var array = GetArrayExpression(temps, stmts);

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

                return Expression.ArrayAccess(array, indexes);
            }

            if (indexType == typeof(Index))
            {
                var array = GetArrayExpression(temps, stmts);

                var index = GetIntIndexExpression(array, length: null, firstIndex);

                if (!Helpers.IsPure(index))
                {
                    var indexVariable = Expression.Parameter(typeof(int), "__idx");

                    temps.Add(indexVariable);
                    stmts.Add(Expression.Assign(indexVariable, index));

                    index = indexVariable;
                }

                // NB: We don't have ref locals in expression trees, so we may end up with
                //
                //     receiver[index] = ... receiver[index] ...
                //
                //     for compound assignments, which is benign but may incur multiple bounds checks. Alternatively, we could
                //     dispatch into the RuntimeOpsEx.WithByRef helper method.

                return Expression.ArrayAccess(array, index);
            }

            throw ContractUtils.Unreachable;
        }

        private bool IsSimpleArrayAccess() => Indexes.All(index => index.Type == typeof(int));

        private Expression ReduceSimple() => Expression.ArrayAccess(Array, Indexes);

        private Expression GetArrayExpression(List<ParameterExpression> variables, List<Expression> statements)
        {
            return GetArrayExpression((type, name) =>
            {
                var variable = Expression.Parameter(type, name);
                variables.Add(variable);
                return variable;
            }, statements);
        }

        private Expression GetArrayExpression(Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            var array = Array;

            if (!Helpers.IsPure(array))
            {
                var arrayVariable = makeVariable(Array.Type, "__arr");

                statements.Add(Expression.Assign(arrayVariable, Array));

                array = arrayVariable;
            }

            return array;
        }

        private static Expression GetIntIndexExpression(Expression array, Expression? length, Expression index)
        {
            //
            // NB: The Roslyn compiler also allows the array.Length expression to be evaluated after evaluating the index expression,
            //     so we follow suit here. This is different from IndexerAccess behavior where, when needed, the length is evaluated
            //     prior to evaluating the index. Arguably, the only side-effect that can take place here is a NullReferenceException
            //     when the array is null.
            //

            length ??= Expression.ArrayLength(array);

            return IndexerAccessCSharpExpression.GetIndexOffset(index, length, out _);
        }

        private static MethodInfo? s_getSubArray;
        private static MethodInfo GetSubArrayMethod => s_getSubArray ??= typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetSubArray), BindingFlags.Public | BindingFlags.Static)!;

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
        public static ArrayAccessCSharpExpression ArrayAccess(Expression array, Expression index) => ArrayAccess(array, new[] { index });

        /// <summary>
        /// Creates an expression representing an array access operation.
        /// </summary>
        /// <param name="array">The array to access.</param>
        /// <param name="indexes">The indexes used to access the array.</param>
        /// <returns>A new <see cref="ArrayAccessCSharpExpression"/> instance representing the array access operation.</returns>
        public static new ArrayAccessCSharpExpression ArrayAccess(Expression array, params Expression[] indexes) => ArrayAccess(array, (IEnumerable<Expression>)indexes);

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
                throw ArgumentMustBeArray(nameof(array));

            var indexesList = indexes.ToReadOnly();

            if (arrayType.GetArrayRank() != indexesList.Count)
                throw IncorrectNumberOfIndexes();

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
                    throw ArgumentMustBeArrayIndexType(nameof(indexes));
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
        protected internal virtual Expression VisitArrayAccess(ArrayAccessCSharpExpression node) =>
            node.Update(
                Visit(node.Array),
                Visit(node.Indexes)
            );
    }
}
