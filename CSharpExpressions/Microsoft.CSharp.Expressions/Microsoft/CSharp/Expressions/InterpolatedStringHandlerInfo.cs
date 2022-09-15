// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents the information needed to bindg an interpolated string handler conversion.
    /// </summary>
    public sealed partial class InterpolatedStringHandlerInfo
    {
        internal InterpolatedStringHandlerInfo(Type type, LambdaExpression construction, ReadOnlyCollection<int> argumentIndices, ReadOnlyCollection<LambdaExpression> append)
        {
            Type = type;
            Construction = construction;
            ArgumentIndices = argumentIndices;
            Append = append;
        }

        /// <summary>
        /// Gets the type of the interpolated string handler.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the construction of an instance of <see cref="Type"/>.
        /// </summary>
        /// <remarks>
        /// The lambda expression has two parameters for the base string length and the number of formatting holes,
        /// followed by optional parameters to fill in arguments with the given <see cref="ArgumentIndices"/>, and
        /// an optional <c>out bool</c> parameter used to check whether subsequent append calls should be made.
        /// </remarks>
        public LambdaExpression Construction { get; }

        /// <summary>
        /// Gets a collection of argument indices referring to arguments that should be passed to <see cref="Construction"/>.
        /// </summary>
        /// <remarks>
        /// The argument indices refer to arguments in a <see cref="MethodCallCSharpExpression"/> containing an interpolated
        /// string handler conversion argument. A value of -1 indicates the <c>this</c> instance of the call.
        /// </remarks>
        public ReadOnlyCollection<int> ArgumentIndices { get; }

        /// <summary>
        /// Gets a collection of <see cref="LambdaExpression"/> expressions representing the append calls made on the instance
        /// of the handler type.
        /// </summary>
        /// <remarks>
        /// The append call lambda expressions have parameters that correspond to the literal in case of a string insert. In
        /// case of an interpolation hole, the parameters repreent the value optionally followed by an alignment and/or format
        /// string. These parameters are replaced by the corresponding parts of the interpolated string being converted, as
        /// specified in <see cref="InterpolatedStringHandlerConversionCSharpExpression.Operand"/>.
        /// </remarks>
        public ReadOnlyCollection<LambdaExpression> Append { get; }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="construction">The <see cref="InterpolatedStringHandlerInfo.Construction" /> property of the result.</param>
        /// <param name="append">The <see cref="InterpolatedStringHandlerInfo.Append"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InterpolatedStringHandlerInfo Update(LambdaExpression construction, IEnumerable<LambdaExpression> append)
        {
            if (construction == Construction && SameElements(ref append!, Append))
            {
                return this;
            }

            return CSharpExpression.InterpolatedStringHandlerInfo(Type, construction, ArgumentIndices, append);
        }
    }

    partial class CSharpExpression
    {
        // REVIEW: An alternative design could be to move the append lambdas to the interpolations themselves, e.g.
        //         by having an optional Append property on InterpolationString[Insert|Literal], where the receiver
        //         is an unbound parameter that gets bound by the parent interpolated strin handler conversion. This
        //         would enable reuse of the InterpolatedStringHandlerInfo object across many use sites but makes
        //         the interpolations end up having additional info that's context dependent. Or, we could move the
        //         append lambdas to the convert node itself.

        /// <summary>
        /// Creates an object representing the binding information for an interpolated string handler conversion.
        /// </summary>
        /// <param name="type">The type of the interpolated string handler.</param>
        /// <param name="construction">The lambda expression representing the construction of an instance of <paramref name="type"/>.</param>
        /// <param name="argumentIndices">The optional index values of extra arguments passed to the <paramref name="construction"/> step.</param>
        /// <param name="append">A collection of lambda expressions representing the append calls of the interpolations to the handler instance.</param>
        /// <returns>An object representing the binding information for an interpolated string handler conversion.</returns>
        public static InterpolatedStringHandlerInfo InterpolatedStringHandlerInfo(Type type, LambdaExpression construction, int[]? argumentIndices, params LambdaExpression[] append) =>
            // NB: The Roslyn compiler binds to this method.
            InterpolatedStringHandlerInfo(type, construction, (IEnumerable<int>?)argumentIndices, (IEnumerable<LambdaExpression>)append);

        /// <summary>
        /// Creates an object representing the binding information for an interpolated string handler conversion.
        /// </summary>
        /// <param name="type">The type of the interpolated string handler.</param>
        /// <param name="construction">The lambda expression representing the construction of an instance of <paramref name="type"/>.</param>
        /// <param name="argumentIndices">The optional index values of extra arguments passed to the <paramref name="construction"/> step.</param>
        /// <param name="append">A collection of lambda expressions representing the append calls of the interpolations to the handler instance.</param>
        /// <returns>An object representing the binding information for an interpolated string handler conversion.</returns>
        public static InterpolatedStringHandlerInfo InterpolatedStringHandlerInfo(Type type, LambdaExpression construction, IEnumerable<int>? argumentIndices, IEnumerable<LambdaExpression> append)
        {
            RequiresNotNull(type, nameof(type));
            ValidateType(type, nameof(type));

            if (!type.IsDefined(typeof(InterpolatedStringHandlerAttribute), inherit: false))
                throw Error.InvalidInterpolatedStringHandlerType(type);

            RequiresCanRead(construction, nameof(construction));

            CheckConstructionLambda(construction.Body, construction.Parameters);

            var builderType = construction.ReturnType;

            if (!AreReferenceAssignable(type, builderType)) // REVIEW: Equivalent or assignable?
                throw Error.InterpolatedStringHandlerTypeNotAssignable(builderType, type);

#pragma warning disable CA1062 // Validate arguments of public methods. (See bug https://github.com/dotnet/roslyn-analyzers/issues/6163)
            var argumentIndicesCollection = argumentIndices.ToReadOnly();

            foreach (var i in argumentIndicesCollection)
            {
                if (i < -1)
                    throw Error.InvalidInterpolatedStringHandlerArgumentIndex(i);
            }
#pragma warning restore CA1062 // Validate arguments of public methods

            var n = construction.Parameters.Count;

            if (n < argumentIndicesCollection.Count + 2)
            {
                throw Error.NotEnoughInterpolatedStringHandlerConstructionParameters(n, argumentIndicesCollection.Count);
            }
            else if (n == argumentIndicesCollection.Count + 3)
            {
                var lastParam = construction.Parameters[n - 1];
                if (lastParam.Type != typeof(bool) || !lastParam.IsByRef)
                    throw Error.InvalidInterpolatedStringHandlerConstructionOutBoolParameter(lastParam.Type);
            }
            else if (n > argumentIndicesCollection.Count + 3)
            {
                throw Error.TooManyInterpolatedStringHandlerConstructionParameters(n, argumentIndicesCollection.Count);
            }

            var appendCollection = append.ToReadOnly();

            RequiresNotNullItems(appendCollection, nameof(append));
            RequiresNotEmpty(appendCollection, nameof(append));

            var appendReturnType = GetAppendType(appendCollection[0]);

            if (appendReturnType != typeof(bool) && appendReturnType != typeof(void))
                throw Error.InvalidInterpolatedStringHandlerAppendReturnType(appendReturnType);

            foreach (var appendLambda in appendCollection)
            {
                if (GetAppendType(appendLambda) != appendReturnType)
                    throw Error.InconsistentInterpolatedStringHandlerAppendReturnType();

                var m = appendLambda.Parameters.Count;

                if (m == 0)
                {
                    throw Error.InvalidInterpolatedStringHandlerAppendArgCount();
                }
                else
                {
                    var firstParam = appendLambda.Parameters[0];

                    if (firstParam.Type != builderType)
                        throw Error.InvalidInterpolatedStringHandlerAppendFirstArgType(firstParam.Type, builderType);

                    // NB: Other checks such as alignment, format, etc. types are deferred until Convert has harvested all parts of
                    //     the interpolated string being converted.
                }
            }

            return new InterpolatedStringHandlerInfo(type, construction, argumentIndicesCollection, appendCollection);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing the construction step for an interpolated string handler.
        /// </summary>
        /// <param name="body">The body of the construction lambda, returning an isntance of an interpolated string handler.</param>
        /// <param name="literalLength">The parameter representing the total length of string literals in the interpolated string being converted.</param>
        /// <param name="formattedCount">The parameter representing the number of formatting holes in the interpolated string being converted.</param>
        /// <param name="parameters">The additional parameters passed to the construction lambda where the last parameter can be an output parameter of type bool.</param>
        /// <returns>A lambda expression representing the construction step for an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerConstructionLambda(Expression body, ParameterExpression literalLength, ParameterExpression formattedCount, params ParameterExpression[] parameters) =>
            InterpolatedStringHandlerConstructionLambda(body, new[] { literalLength, formattedCount }.Concat(parameters).ToArray());

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing the construction step for an interpolated string handler.
        /// </summary>
        /// <param name="body">The body of the construction lambda, returning an isntance of an interpolated string handler.</param>
        /// <param name="parameters">The parameters passed to the construction lambda, where the first two parameters are of type int, and the last parameter can be an output parameter of type bool.</param>
        /// <returns>A lambda expression representing the construction step for an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerConstructionLambda(Expression body, ParameterExpression[] parameters)
        {
            // NB: The Roslyn compiler binds to this method.

            RequiresCanRead(body, nameof(body));
            RequiresNotNullItems(parameters, nameof(parameters));

            CheckConstructionLambda(body, parameters);

            var handlerType = body.Type;
            var n = parameters.Length;

            var hasTrailingOutBool = ParameterListHasTrailingShouldAppend(parameters);

            var argCount = (hasTrailingOutBool ? n - 1 : n) - 2 /* int literalLength, int formattedCount */;

            var types = new Type[argCount + 1];
            types[0] = handlerType;

            var hasByRefArg = false;

            for (var i = 0; i < argCount; i++)
            {
                var parameter = parameters[i + 2];

                if (parameter.IsByRef)
                {
                    hasByRefArg = true;
                    break;
                }

                types[i + 1] = parameter.Type;
            }

            Type? delegateType = null;

            if (!hasByRefArg)
            {
                delegateType = hasTrailingOutBool
                    ? ConstructInterpolatedStringHandlerDelegateHelpers.GetConstructInterpolatedStringHandlerTypeWithShouldAppend(types)
                    : ConstructInterpolatedStringHandlerDelegateHelpers.GetConstructInterpolatedStringHandlerType(types);
            }

            if (delegateType != null)
            {
                return Lambda(delegateType, body, parameters);
            }

            return Lambda(body, parameters);
        }

        private static void CheckConstructionLambda(Expression body, IList<ParameterExpression> parameters)
        {
            if (parameters.Count < 2)
                throw Error.InvalidInterpolatedStringHandlerConstructionArgCount();

            if (parameters[0].Type != typeof(int))
                throw Error.InvalidInterpolatedStringHandlerInt32ParameterType(parameters[0], "literalLength");

            if (parameters[1].Type != typeof(int))
                throw Error.InvalidInterpolatedStringHandlerInt32ParameterType(parameters[1], "formattedCount");

            if (!body.Type.IsDefined(typeof(InterpolatedStringHandlerAttribute), inherit: false))
                throw Error.InvalidInterpolatedStringHandlerType(body.Type);
        }

        internal static bool ParameterListHasTrailingShouldAppend(IList<ParameterExpression> parameters)
        {
            var lastParam = parameters[parameters.Count - 1];
            return lastParam.IsByRef && lastParam.Type == typeof(bool);
        }

        internal static Type GetAppendType(LambdaExpression append)
        {
            if (append.Body is DynamicCSharpExpression d && (d.Flags & CSharpBinderFlags.ResultDiscarded) != 0)
            {
                return typeof(void);
            }

            return append.ReturnType;
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InterpolatedStringHandlerInfo" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual InterpolatedStringHandlerInfo VisitInterpolatedStringHandlerInfo(InterpolatedStringHandlerInfo node) =>
            node.Update(
                VisitAndConvert(node.Construction, nameof(VisitInterpolatedStringHandlerInfo)),
                VisitAndConvert(node.Append, nameof(VisitInterpolatedStringHandlerInfo))
            );
    }
}
