// Prototyping extended expression trees for C#.
//
// bartde - February 2020

#nullable enable

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents binding information for statically bound await operations.
    /// </summary>
    public sealed partial class StaticAwaitInfo : AwaitInfo
    {
        internal StaticAwaitInfo(LambdaExpression getAwaiter, PropertyInfo isCompleted, MethodInfo getResult)
        {
            GetAwaiter = getAwaiter;
            IsCompleted = isCompleted;
            GetResult = getResult;
        }

        /// <summary>
        /// Indicates whether the await operation is dynamically bound.
        /// </summary>
        public override bool IsDynamic => false;

        /// <summary>
        /// Gets the type of the object returned by the await operation.
        /// </summary>
        public override Type Type => GetResult.ReturnType;

        /// <summary>
        /// Gets the expression used to obtain an awaiter from an awaitable object.
        /// </summary>
        public LambdaExpression GetAwaiter { get; }

        /// <summary>
        /// Gets the property used to check whether the asynchronous operation has completed.
        /// </summary>
        public PropertyInfo IsCompleted { get; }

        /// <summary>
        /// Gets the method used to obtain the result returned by the asynchronous operation.
        /// </summary>
        public MethodInfo GetResult { get; }

        /// <summary>
        /// Creates a new object that is like this one, but using the supplied children. If all of the children are the same, it will return this object.
        /// </summary>
        /// <param name="getAwaiter">The <see cref="StaticAwaitInfo.GetAwaiter"/> property of the result.</param>
        /// <returns>This object if no children changed, or an object with the updated children.</returns>
        public AwaitInfo Update(LambdaExpression getAwaiter)
        {
            if (getAwaiter != GetAwaiter)
            {
                return CSharpExpression.AwaitInfo(getAwaiter, IsCompleted, GetResult);
            }

            return this;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override AwaitInfo Accept(CSharpExpressionVisitor visitor) => visitor.VisitAwaitInfo(this);

        internal override void RequiresCanBind(Expression operand)
        {
            // NB: This will throw if we can't bind.

            _ = Expression.Invoke(GetAwaiter, operand);
        }

        internal override Expression ReduceGetAwaiter(Expression operand) => InvokeLambdaWithSingleParameter(GetAwaiter, operand);

        internal override Expression ReduceGetResult(Expression awaiter) => Expression.Call(awaiter, GetResult);

        internal override Expression ReduceIsCompleted(Expression awaiter) => Expression.Property(awaiter, IsCompleted);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an object representing binding information for await operations.
        /// </summary>
        /// <param name="awaitableType">The type of the awaitable object.</param>
        /// <returns>An object representing binding information for await operations.</returns>
        public static StaticAwaitInfo AwaitInfo(Type awaitableType)
        {
            RequiresNotNull(awaitableType, nameof(awaitableType));

            var getAwaiterMethod = GetGetAwaiter(awaitableType);

            if (getAwaiterMethod == null)
                throw Error.AwaitableTypeShouldHaveGetAwaiterMethod(awaitableType);

            return AwaitInfo(awaitableType, getAwaiterMethod);
        }

        /// <summary>
        /// Creates an object representing binding information for await operations.
        /// </summary>
        /// <param name="awaitableType">The type of the awaitable object.</param>
        /// <param name="getAwaiterMethod">The GetAwaiter method used to await the asynchronous operation.</param>
        /// <returns>An object representing binding information for await operations.</returns>
        public static StaticAwaitInfo AwaitInfo(Type awaitableType, MethodInfo getAwaiterMethod)
        {
            RequiresNotNull(awaitableType, nameof(awaitableType));
            RequiresNotNull(getAwaiterMethod, nameof(getAwaiterMethod));

            ValidateGetAwaiterMethod(awaitableType, getAwaiterMethod);

            var getAwaiter = GetGetAwaiterExpression(awaitableType, getAwaiterMethod);

            return AwaitInfo(getAwaiter, default(PropertyInfo), default(MethodInfo));
        }

        /// <summary>
        /// Creates an object representing binding information for await operations.
        /// </summary>
        /// <param name="getAwaiter">Expression used to obtain an awaiter from an awaitable object.</param>
        /// <param name="isCompleted">The accessor method of the property used to check whether the asynchronous operation has completed.</param>
        /// <param name="getResult">The method used to obtain the result returned by the asynchronous operation.</param>
        /// <returns>An object representing binding information for await operations.</returns>
        public static StaticAwaitInfo AwaitInfo(LambdaExpression getAwaiter, MethodInfo isCompleted, MethodInfo getResult)
        {
            // NB: This is the overload the C# compiler binds to.

            RequiresNotNull(isCompleted, nameof(isCompleted));
            ValidateMethodInfo(isCompleted, nameof(isCompleted));

            return AwaitInfo(getAwaiter, GetProperty(isCompleted, nameof(isCompleted)), getResult);
        }

        /// <summary>
        /// Creates an object representing binding information for await operations.
        /// </summary>
        /// <param name="getAwaiter">Expression used to obtain an awaiter from an awaitable object.</param>
        /// <param name="isCompleted">The property used to check whether the asynchronous operation has completed.</param>
        /// <param name="getResult">The method used to obtain the result returned by the asynchronous operation.</param>
        /// <returns>An object representing binding information for await operations.</returns>
        public static StaticAwaitInfo AwaitInfo(LambdaExpression getAwaiter, PropertyInfo? isCompleted, MethodInfo? getResult)
        {
            RequiresNotNull(getAwaiter, nameof(getAwaiter));

            if (getAwaiter.Parameters.Count != 1)
                throw Error.GetAwaiterExpressionOneParameter();

            //
            // Resolve awaiter members if not specified.
            //

            var awaiterType = getAwaiter.ReturnType;

            ResolveAwaiterInfo(awaiterType, ref isCompleted, ref getResult);

            ValidateAwaiterType(awaiterType, isCompleted, getResult);

            //
            // Validate we can construct the IsCompleted and GetResult nodes.
            //
            // NB: ValidateAwaiterType ensures these are non-null.
            //

            var awaiterExpression = Expression.Parameter(awaiterType);

            _ = Expression.Property(awaiterExpression, isCompleted!);
            _ = Expression.Call(awaiterExpression, getResult!);

            return new StaticAwaitInfo(getAwaiter, isCompleted!, getResult!);
        }

        private static void ResolveAwaiterInfo(Type awaiterType, ref PropertyInfo? isCompleted, ref MethodInfo? getResult)
        {
            isCompleted ??= awaiterType.GetProperty("IsCompleted", BindingFlags.Public | BindingFlags.Instance);
            getResult ??= awaiterType.GetNonGenericMethod("GetResult", BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
        }

        internal static MethodInfo? GetGetAwaiter(Type awaiterType) =>
            awaiterType.GetNonGenericMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());

        private static void ValidateGetAwaiterMethod(Type operandType, MethodInfo getAwaiterMethod)
        {
            ValidateMethodInfo(getAwaiterMethod, nameof(getAwaiterMethod));

            // NB: We don't check whether the name of the method is GetAwaiter, just like we don't check the name of
            //     operator op_* methods in Binary and Unary node factories in LINQ. We could tighten this, but there
            //     is no harm in letting an advanced used specify another method that obeys to the awaiter pattern
            //     other than the predescribed method name. The C# compiler will always specify the MethodInfo in the
            //     emitted factory call.

            var getAwaiterParams = getAwaiterMethod.GetParametersCached();

            if (getAwaiterMethod.IsStatic)
            {
                if (getAwaiterParams.Length != 1)
                    throw Error.GetAwaiterShouldTakeZeroParameters();

                var firstParam = getAwaiterParams[0];

                if (!TypeUtils.AreReferenceAssignable(firstParam.ParameterType, operandType))
                    throw Error.ExpressionTypeDoesNotMatchParameter(operandType, firstParam.ParameterType);
            }
            else
            {
                if (getAwaiterParams.Length != 0)
                    throw Error.GetAwaiterShouldTakeZeroParameters();

                if (getAwaiterMethod.IsGenericMethod)
                    throw Error.GetAwaiterShouldNotBeGeneric();
            }

            var returnType = getAwaiterMethod.ReturnType;

            if (returnType == typeof(void) || returnType.IsByRef || returnType.IsPointer)
                throw Error.GetAwaiterShouldReturnAwaiterType();
        }

        private static void ValidateAwaiterType(Type awaiterType, PropertyInfo? isCompleted, MethodInfo? getResult)
        {
            if (!typeof(INotifyCompletion).IsAssignableFrom(awaiterType))
                throw Error.AwaiterTypeShouldImplementINotifyCompletion(awaiterType);

            if (isCompleted == null || isCompleted.GetMethod == null)
                throw Error.AwaiterTypeShouldHaveIsCompletedProperty(awaiterType);

            if (isCompleted.PropertyType != typeof(bool))
                throw Error.AwaiterIsCompletedShouldReturnBool(awaiterType);

            if (isCompleted.GetIndexParameters().Length != 0)
                throw Error.AwaiterIsCompletedShouldNotBeIndexer(awaiterType);

            if (getResult == null || getResult.IsGenericMethodDefinition)
                throw Error.AwaiterTypeShouldHaveGetResultMethod(awaiterType);

            var returnType = getResult.ReturnType;

            if (returnType.IsByRef || returnType.IsPointer)
                throw Error.AwaiterGetResultTypeInvalid(awaiterType);
        }

        private static LambdaExpression GetGetAwaiterExpression(Type awaitableType, MethodInfo getAwaiterMethod)
        {
            var p = Expression.Parameter(awaitableType);

            var call = getAwaiterMethod.IsStatic ? Expression.Call(getAwaiterMethod, new[] { p }) : Expression.Call(p, getAwaiterMethod);

            return Expression.Lambda(call, p);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="StaticAwaitInfo" />.
        /// </summary>
        /// <param name="node">The object to visit.</param>
        /// <returns>The modified object, if it or any subexpression was modified; otherwise, returns the original object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual AwaitInfo VisitAwaitInfo(StaticAwaitInfo node) =>
            node.Update(
                VisitAndConvert(node.GetAwaiter, nameof(VisitAwaitInfo))
            );
    }
}
