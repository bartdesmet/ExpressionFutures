// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents an index from the end of an indexable object.
    /// </summary>
    public sealed partial class FromEndIndexCSharpExpression : CSharpExpression
    {
        internal FromEndIndexCSharpExpression(Expression operand, MethodBase method, Type type)
        {
            Operand = operand;
            Method = method;
            Type = type ?? (operand.Type.IsNullableType() ? typeof(Index?) : typeof(Index));
        }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the index to access from the end.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// Gets the (optional) method or constructor used to create an instance of type <see cref="Index"/>.
        /// </summary>
        public MethodBase Method { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Gets a value indicating whether the index construction is lifted to a nullable type.
        /// </summary>
        public bool IsLifted => Type.IsNullableType();

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.FromEndIndex;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitFromEndIndex(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public FromEndIndexCSharpExpression Update(Expression operand)
        {
            if (operand == Operand)
            {
                return this;
            }

            return CSharpExpression.FromEndIndex(operand, Method, Type);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            if (IsLifted)
            {
                if (IsAlwaysNull(Operand))
                {
                    return Expression.Default(Type);
                }
                
                if (IsNeverNull(Operand))
                {
                    // CONSIDER: Peek into Operand and try to extract non-null value.

                    return
                        Expression.Convert(
                            MakeFromEnd(MakeNullableGetValueOrDefault(Operand)),
                            Type
                        );
                }

                if (Operand is ParameterExpression i)
                {
                    return MakeLiftedFromEnd(i);
                }
                else
                {
                    var temp = Expression.Parameter(Operand.Type, "__t");

                    return
                        Expression.Block(
                            new[] { temp },
                            Expression.Assign(temp, Operand),
                            MakeLiftedFromEnd(temp)
                        );
                }
            }
            else
            {
                return MakeFromEnd(Operand);
            }

            Expression MakeLiftedFromEnd(ParameterExpression operand) =>
                Expression.Condition(
                    MakeNullableHasValue(operand),
                    Expression.Convert(
                        MakeFromEnd(MakeNullableGetValueOrDefault(operand)),
                        Type
                    ),
                    Expression.Default(Type)
                );

            Expression MakeFromEnd(Expression operand)
            {
                var method = Method ?? FromEnd;

                switch (method)
                {
                    case MethodInfo m:
                        switch (m.GetParametersCached().Length)
                        {
                            case 1:
                                return Expression.Call(m, operand);
                            case 2:
                                return Expression.Call(m, operand, Expression.Constant(true));
                        }
                        break;
                    case ConstructorInfo c:
                        switch (c.GetParametersCached().Length)
                        {
                            case 1:
                                return Expression.New(c, operand);
                            case 2:
                                return Expression.New(c, operand, Expression.Constant(true));
                        }
                        break;
                }
                
                throw ContractUtils.Unreachable;
            }
        }

        private static MethodInfo s_from_end;

        private static MethodInfo FromEnd => s_from_end ??= typeof(Index).GetNonGenericMethod(nameof(System.Index.FromEnd), BindingFlags.Public | BindingFlags.Static, new[] { typeof(int) });
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="FromEndIndexCSharpExpression"/> that represents an index from the end of an indexable object.
        /// </summary>
        /// <param name="operand">The expression representing the index value.</param>
        /// <returns>The created <see cref="FromEndIndexCSharpExpression"/>.</returns>
        public static FromEndIndexCSharpExpression FromEndIndex(Expression operand) => FromEndIndex(operand, null, null);

        /// <summary>
        /// Creates a <see cref="FromEndIndexCSharpExpression"/> that represents an index from the end of an indexable object.
        /// </summary>
        /// <param name="operand">The expression representing the index value.</param>
        /// <param name="method">The method or constructor used to instantiate the index.</param>
        /// <param name="type">The index type, either <see cref="System.Index"/> or <see cref="System.Index?"/>.</param>
        /// <returns>The created <see cref="FromEndIndexCSharpExpression"/>.</returns>
        public static FromEndIndexCSharpExpression FromEndIndex(Expression operand, MethodBase method, Type type)
        {
            RequiresCanRead(operand, nameof(operand));

            if (operand.Type != typeof(int) && operand.Type != typeof(int?))
                throw Error.InvalidFromEndIndexOperandType(operand.Type);

            if (method != null)
            {
                if (method.IsGenericMethodDefinition || method.GetReturnType() != typeof(Index))
                    throw Error.InvalidFromEndIndexMethod();

                if (method.MemberType == MemberTypes.Method && !method.IsStatic)
                    throw Error.InvalidFromEndIndexMethod();

                var parameters = method.GetParametersCached();

                switch (parameters.Length)
                {
                    case 1:
                        if (parameters[0].ParameterType != typeof(int))
                            throw Error.InvalidFromEndIndexMethod();
                        break;
                    case 2:
                        if (parameters[0].ParameterType != typeof(int) || parameters[1].ParameterType != typeof(bool))
                            throw Error.InvalidFromEndIndexMethod();
                        break;
                    default:
                        throw Error.InvalidFromEndIndexMethod();
                }
            }

            if (type != null)
            {
                if (type == typeof(Index))
                {
                    if (operand.Type != typeof(int))
                        throw Error.InvalidIndexType(type);
                }
                else if (type == typeof(Index?))
                {
                    if (operand.Type != typeof(int?))
                        throw Error.InvalidIndexType(type);
                }
                else
                {
                    throw Error.InvalidIndexType(type);
                }
            }

            return new FromEndIndexCSharpExpression(operand, method, type);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="FromEndIndexCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitFromEndIndex(FromEndIndexCSharpExpression node) =>
            node.Update(
                Visit(node.Operand)
            );
    }
}
