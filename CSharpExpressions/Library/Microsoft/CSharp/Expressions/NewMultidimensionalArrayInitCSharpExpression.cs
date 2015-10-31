// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.ExpressionStubs;
using static Microsoft.CSharp.Expressions.Helpers;
using LinqError = System.Linq.Expressions.Error;
using System.Linq;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents creating a new multi-dimensional array and possibly initializing the elements of the new array.
    /// </summary>
    public sealed class NewMultidimensionalArrayInitCSharpExpression : CSharpExpression
    {
        private readonly int[] _bounds;

        internal NewMultidimensionalArrayInitCSharpExpression(Type type, int[] bounds, ReadOnlyCollection<Expression> expressions)
        {
            Type = type;
            _bounds = bounds;
            Expressions = expressions;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.NewMultidimensionalArrayInit;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        /// Gets the values to initialize the elements of the new array.
        /// </summary>
        public ReadOnlyCollection<Expression> Expressions { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the element of the array the specified <paramref name="indices"/>.
        /// </summary>
        /// <param name="indices">The indices of the element to retrieve.</param>
        /// <returns>An <see cref="Expression" /> representing the element of the array the specified <paramref name="indices"/>.</returns>
        public Expression GetExpression(params int[] indices)
        {
            ContractUtils.RequiresNotNull(indices, nameof(indices));

            if (indices.Length != _bounds.Length)
            {
                throw Error.RankMismatch();
            }

            var index = 0;
            for (var i = 0; i < indices.Length; i++)
            {
                var idx = indices[i];
                var bound = _bounds[i];

                if (idx < 0 || idx >= bound)
                {
                    throw Error.IndexOutOfRange();
                }

                index = index * bound + idx;
            }

            return Expressions[index];
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitNewMultidimensionalArrayInit(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="expressions">The <see cref="Expressions" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public NewMultidimensionalArrayInitCSharpExpression Update(IEnumerable<Expression> expressions)
        {
            if (expressions == Expressions)
            {
                return this;
            }

            return CSharpExpression.NewMultiDimensionalArrayInit(Type.GetElementType(), _bounds, expressions);
        }

        /// <summary>
        /// Reduces the call expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var n = Expressions.Count;
            var rank = _bounds.Length;

            var res = Expression.Parameter(Type);
            var exprs = new Expression[n + 2];

            // NB: We need the bounds to NewArrayBounds and all values from 0 to each bound for ArrayAccess.
            var consts = Enumerable.Range(0, _bounds.Max() + 1).Select(i => Expression.Constant(i)).ToArray();

            exprs[0] = Expression.Assign(res, Expression.NewArrayBounds(Type.GetElementType(), _bounds.Map(i => consts[i])));

            var indexValues = new int[rank];

            for (var i = 1; i <= n; i++)
            {
                var idx = i - 1;
                var value = Expressions[idx];

                for (var j = rank - 1; j >= 0; j--)
                {
                    var bound = _bounds[j];
                    indexValues[j] = idx % bound;
                    idx /= bound;
                }

                var indices = new TrueReadOnlyCollection<Expression>(indexValues.Map(j => consts[j]));
                var element = Expression.ArrayAccess(res, indices);

                exprs[i] = Expression.Assign(element, value);
            }

            exprs[n + 1] = res;

            return Expression.Block(new[] { res }, exprs);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="NewMultidimensionalArrayInitCSharpExpression"/> that represents creating a multi-dimensional array that has the specified bounds and elements. 
        /// </summary>
        /// <param name="type">A Type that represents the element type of the array.</param>
        /// <param name="bounds">The bounds of the array.</param>
        /// <param name="initializers">An IEnumerable{T} that contains Expression objects that represent the elements in the array.</param>
        /// <returns>An instance of the <see cref="NewMultidimensionalArrayInitCSharpExpression"/>.</returns>
        public static NewMultidimensionalArrayInitCSharpExpression NewMultiDimensionalArrayInit(Type type, int[] bounds, IEnumerable<Expression> initializers)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            ContractUtils.RequiresNotNull(bounds, nameof(bounds));
            ContractUtils.RequiresNotNull(initializers, nameof(initializers));

            if (type.Equals(typeof(void)))
            {
                throw LinqError.ArgumentCannotBeOfTypeVoid();
            }

            var boundsList = bounds.Copy();

            int dimensions = boundsList.Length;
            if (dimensions <= 0)
            {
                throw LinqError.BoundsCannotBeLessThanOne();
            }

            var length = 1;

            foreach (var bound in boundsList)
            {
                if (bound < 0)
                {
                    throw Error.BoundCannotBeLessThanZero();
                }

                checked
                {
                    length *= bound;
                }
            }

            var initializerList = initializers.ToReadOnly();

            if (initializerList.Count != length)
            {
                throw Error.ArrayBoundsElementCountMismatch();
            }

            var newList = default(Expression[]);
            for (int i = 0, n = initializerList.Count; i < n; i++)
            {
                var expr = initializerList[i];
                RequiresCanRead(expr, nameof(initializers));

                if (!TypeUtils.AreReferenceAssignable(type, expr.Type))
                {
                    if (!TryQuote(type, ref expr))
                    {
                        throw LinqError.ExpressionTypeCannotInitializeArrayType(expr.Type, type);
                    }

                    if (newList == null)
                    {
                        newList = new Expression[initializerList.Count];
                        for (int j = 0; j < i; j++)
                        {
                            newList[j] = initializerList[j];
                        }
                    }
                }

                if (newList != null)
                {
                    newList[i] = expr;
                }
            }

            if (newList != null)
            {
                initializerList = new TrueReadOnlyCollection<Expression>(newList);
            }

            return new NewMultidimensionalArrayInitCSharpExpression(type.MakeArrayType(boundsList.Length), boundsList, initializerList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="NewMultidimensionalArrayInitCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitNewMultidimensionalArrayInit(NewMultidimensionalArrayInitCSharpExpression node)
        {
            return node.Update(Visit(node.Expressions));
        }
    }
}
