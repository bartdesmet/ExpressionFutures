// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a dynamically bound indexing operation.
    /// </summary>
    public sealed partial class GetIndexDynamicCSharpExpression : DynamicCSharpExpression
    {
        internal GetIndexDynamicCSharpExpression(Type context, CSharpBinderFlags binderFlags, Expression @object, ReadOnlyCollection<DynamicCSharpArgument> arguments)
            : base(context, binderFlags)
        {
            Object = @object;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DynamicGetIndex;

        /// <summary>
        /// Gets the expression representing the object to index into.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Gets the arguments to pass to the indexer.
        /// </summary>
        public ReadOnlyCollection<DynamicCSharpArgument> Arguments { get; }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        protected override void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 1];
            var expressions = new Expression[n + 1];

            // NB: By-ref passing for the receiver seems to be omitted in Roslyn here; see https://github.com/dotnet/roslyn/issues/6818.
            //     We're choosing to be consistent with that behavior until further notice.
            expressions[0] = Object;
            argumentInfos[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, name: null);

            argumentTypes = null;
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            binder = Binder.GetIndex(Flags, Context, argumentInfos);
            arguments = expressions;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDynamicGetIndex(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public GetIndexDynamicCSharpExpression Update(Expression @object, IEnumerable<DynamicCSharpArgument> arguments)
        {
            if (@object == Object && SameElements(ref arguments, Arguments))
            {
                return this;
            }

            return DynamicCSharpExpression.DynamicGetIndex(@object, arguments, Flags, Context);
        }

        internal GetIndexDynamicCSharpExpression TransformToLhs(List<ParameterExpression> temps, List<Expression> stores)
        {
            var obj = Expression.Parameter(Object.Type, "__obj");

            temps.Add(obj);
            stores.Add(Expression.Assign(obj, Object));

            int n = Arguments.Count;

            var newArgs = new DynamicCSharpArgument[n];

            for (int i = 0; i < n; i++)
            {
                var arg = Arguments[i];

                if (Helpers.IsPure(arg.Expression))
                {
                    newArgs[i] = arg;
                }
                else
                {
                    var tmp = Expression.Parameter(arg.Expression.Type, "__arg" + i);

                    temps.Add(tmp);
                    stores.Add(Expression.Assign(tmp, arg.Expression));

                    newArgs[i] = arg.Update(tmp);
                }
            }

            return Update(obj, newArgs);
        }

        internal Expression ReduceAssignment(Expression value, CSharpBinderFlags flags, CSharpArgumentInfoFlags leftFlags = CSharpArgumentInfoFlags.None, CSharpArgumentInfoFlags rightFlags = CSharpArgumentInfoFlags.None)
        {
            ReduceAssignment(value, flags, leftFlags, rightFlags, out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes);

            return DynamicHelpers.MakeDynamic(Type, binder, arguments, argumentTypes);
        }

        private void ReduceAssignment(Expression value, CSharpBinderFlags flags, CSharpArgumentInfoFlags leftFlags, CSharpArgumentInfoFlags rightFlags, out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[] argumentTypes)
        {
            var n = Arguments.Count;

            var argumentInfos = new CSharpArgumentInfo[n + 2];
            var expressions = new Expression[n + 2];

            // NB: This is the only place where by-ref passing for the receiver is done in Roslyn; for more
            //     info about the apparent inconsistency see https://github.com/dotnet/roslyn/issues/6818.
            //     We're choosing to be consistent with that behavior until further notice.
            if (IsReceiverByRef(Object))
            {
                leftFlags |= CSharpArgumentInfoFlags.IsRef;
            }

            expressions[0] = Object;
            argumentInfos[0] = CSharpArgumentInfo.Create(leftFlags, null);

            argumentTypes = null;
            CopyArguments(Arguments, argumentInfos, expressions, ref argumentTypes);

            argumentInfos[n + 1] = CSharpArgumentInfo.Create(rightFlags, null); // TODO: check
            expressions[n + 1] = value;

            flags |= Flags;

            binder = Binder.SetIndex(flags, Context, argumentInfos);
            arguments = expressions;
        }
    }

    partial class DynamicCSharpExpression
    {
        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An array of expressions representing the arguments passed to the indexing operaton.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, params Expression[] arguments) =>
            DynamicGetIndex(@object, GetDynamicArguments(arguments), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An enumerable sequence of expressions representing the arguments passed to the indexing operaton.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, IEnumerable<Expression> arguments) =>
            DynamicGetIndex(@object, GetDynamicArguments(arguments), CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An array of dynamic arguments representing the arguments passed to the indexing operaton.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, DynamicCSharpArgument[] arguments) =>
            DynamicGetIndex(@object, arguments, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the indexing operaton.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, IEnumerable<DynamicCSharpArgument> arguments) =>
            DynamicGetIndex(@object, arguments, CSharpBinderFlags.None, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation with the specified binder flags.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the indexing operaton.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags) =>
            DynamicGetIndex(@object, arguments, binderFlags, context: null);

        /// <summary>
        /// Creates a new expression representing a dynamically bound indexing operation with the specified binder flags and the specified type context.
        /// </summary>
        /// <param name="object">The expression representing the object to index into.</param>
        /// <param name="arguments">An enumerable sequence of dynamic arguments representing the arguments passed to the indexing operaton.</param>
        /// <param name="binderFlags">The binder flags to use for the dynamic operation.</param>
        /// <param name="context">The type representing the context in which the dynamic operation is bound.</param>
        /// <returns>A new expression representing a dynamically bound indexing operation.</returns>
        public static GetIndexDynamicCSharpExpression DynamicGetIndex(Expression @object, IEnumerable<DynamicCSharpArgument> arguments, CSharpBinderFlags binderFlags, Type context)
        {
            RequiresCanRead(@object, nameof(@object));

            var argList = arguments.ToReadOnly();

            return new GetIndexDynamicCSharpExpression(context, binderFlags, @object, argList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="GetIndexDynamicCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDynamicGetIndex(GetIndexDynamicCSharpExpression node) =>
            node.Update(
                Visit(node.Object),
                Visit(node.Arguments, VisitDynamicArgument)
            );
    }
}
