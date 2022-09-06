// Prototyping extended expression trees for C#.
//
// bartde - May 2020

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a tuple literal.
    /// </summary>
    public sealed partial class TupleLiteralCSharpExpression : CSharpExpression
    {
        internal TupleLiteralCSharpExpression(Type type, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<string>? argumentNames)
        {
            Type = type;
            Arguments = arguments;
            ArgumentNames = argumentNames;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.TupleLiteral;

        /// <summary>
        /// Gets a collection of arguments used to construct the components of the tuple.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; }

        /// <summary>
        /// Gets a collection of names of the components of the tuple, or <c>null</c> if no names were specified.
        /// </summary>
        public ReadOnlyCollection<string>? ArgumentNames { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitTupleLiteral(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TupleLiteralCSharpExpression Update(IEnumerable<Expression> arguments)
        {
            if (SameElements(ref arguments, Arguments))
            {
                return this;
            }

            return CSharpExpression.TupleLiteral(Type, arguments, ArgumentNames);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            Stack<(ConstructorInfo ctor, MemberInfo[] members)> GetConstructorChain()
            {
                var res = new Stack<(ConstructorInfo ctor, MemberInfo[] members)>();

                void Push(Type type)
                {
                    var arity = type.GetGenericArguments().Length;

                    var ctor = type.GetConstructors().Single(ctor => ctor.GetParametersCached().Length == arity);

                    var members = new MemberInfo[arity];

                    for (int i = 0; i < arity; i++)
                    {
                        var member = type.GetField(TupleItemNames[i], BindingFlags.Public | BindingFlags.Instance);
                        Debug.Assert(member != null);

                        members[i] = member;
                    }

                    res.Push((ctor, members));
                }

                var current = Type;
                Push(current);

                while (current.GetGenericTypeDefinition() == MaxTupleType)
                {
                    current = current.GetGenericArguments()[^1];
                    Push(current);
                }

                return res;
            }

            NewExpression CreateTuple(ConstructorInfo ctor, MemberInfo[] members, int firstArg, Expression? rest)
            {
                var n = members.Length;

                var args = new Expression[n];

                if (rest != null)
                {
                    n--;
                }

                for (int i = 0; i < n; i++)
                {
                    args[i] = Arguments[firstArg + i];
                }

                if (rest != null)
                {
                    args[^1] = rest;
                }

                return Expression.New(ctor, args, members);
            }

            // E.g. Tuple`8<int, int, int, int, int, int, int, Tuple`3<int, int, int>>
            //               1    2    3    4    5    6    7            8    9    10

            var chain = GetConstructorChain();

            var argIndex = Arguments.Count;

            Expression? res = null;

            Debug.Assert(chain.Count > 0);

            do
            {
                var (ctor, members) = chain.Pop();

                argIndex -= (members.Length - (res != null ? 1 : 0));

                res = CreateTuple(ctor, members, argIndex, rest: res);
            }
            while (chain.Count > 0);

            return res;
        }

        private static readonly string[] TupleItemNames = { "Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Rest" };
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(params Expression[] arguments) => TupleLiteral(arguments, argumentNames: null);

        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(IEnumerable<Expression> arguments) => TupleLiteral(arguments, argumentNames: null);

        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <param name="argumentNames">An array of names corresponding to the tuple components, or <c>null</c> if no names were specified.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(IEnumerable<Expression> arguments, IEnumerable<string>? argumentNames)
        {
            var args = arguments.ToReadOnly();
            RequiresNotEmpty(args, nameof(arguments));

            var n = args.Count;
            var types = new Type[n];

            for (int i = 0; i < n; i++)
            {
                var arg = args[i];

                RequiresNotNull(arg, nameof(arguments));

                if (arg.Type == typeof(void))
                    throw Error.TupleComponentCannotBeVoid();

                types[i] = arg.Type;
            }

            var type = MakeTupleType(types);

            var argNames = argumentNames?.ToReadOnly();

            if (argNames != null && argNames.Count != n)
                throw Error.InvalidTupleArgumentNamesCount(type);

            return new TupleLiteralCSharpExpression(type, args, argNames);
        }

        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(Type type, params Expression[] arguments) => TupleLiteral(type, arguments, argumentNames: null);

        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(Type type, IEnumerable<Expression> arguments) => TupleLiteral(type, arguments, argumentNames: null);

        /// <summary>
        /// Creates a <see cref="TupleLiteralCSharpExpression" /> that represents a tuple literal.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that represents the tuple type.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the components of the tuple.</param>
        /// <param name="argumentNames">An array of names corresponding to the tuple components, or <c>null</c> if no names were specified.</param>
        /// <returns>A <see cref="TupleLiteralCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.TupleLiteral" /> and the <see cref="TupleLiteralCSharpExpression.Arguments" /> and <see cref="TupleLiteralCSharpExpression.ArgumentNames" /> properties set to the specified values.</returns>
        public static TupleLiteralCSharpExpression TupleLiteral(Type type, IEnumerable<Expression> arguments, IEnumerable<string>? argumentNames)
        {
            RequiresNotNull(type, nameof(type));

            if (!IsTupleType(type))
                throw Error.InvalidTupleType(type);

            static List<ParameterInfo> GetTupleConstructorParameters(Type type, int n)
            {
                var res = new List<ParameterInfo>(n);

                while (type != null)
                {
                    var parameters = type.GetConstructors().Single().GetParameters();

                    var def = type.GetGenericTypeDefinition();

                    if (def == MaxTupleType)
                    {
                        for (int i = 0; i < parameters.Length - 1; i++)
                        {
                            res.Add(parameters[i]);
                        }

                        type = type.GetGenericArguments()[^1];
                    }
                    else
                    {
                        res.AddRange(parameters);

                        break;
                    }
                }

                return res;
            }

            var args = arguments.ToReadOnly();

            var n = args.Count;

            var parameters = GetTupleConstructorParameters(type, n);

            if (parameters.Count != n)
                throw Error.InvalidTupleArgumentCount(type);

            for (int i = 0; i < n; i++)
            {
                //
                // NB: With deconstructing assignment, a tuple literal can occur as an lhs. The checks below will
                //     only check assignability but will not enforce RequiresCanRead(args[i]) to avoid rejecting
                //     set-only properties. These will fail during a call to Reduce if the tuple literal is used
                //     as an rvalue.
                //

                var parameterType = parameters[i].ParameterType;
                var argumentType = args[i].Type;

                ValidateType(type, nameof(type), i);
                if (!AreReferenceAssignable(parameterType, argumentType))
                    throw Error.ExpressionTypeDoesNotMatchParameter(argumentType, parameterType);
            }

            var argNames = argumentNames?.ToReadOnly();

            if (argNames != null && argNames.Count != n)
                throw Error.InvalidTupleArgumentNamesCount(type);

            return new TupleLiteralCSharpExpression(type, args, argNames);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="TupleLiteralCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitTupleLiteral(TupleLiteralCSharpExpression node) =>
            node.Update(
                Visit(node.Arguments)
            );
    }
}
