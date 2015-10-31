// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an expression that awaits an asynchronous operation.
    /// </summary>
    public sealed class AwaitCSharpExpression : UnaryCSharpExpression
    {
        internal AwaitCSharpExpression(Expression operand, MethodInfo getAwaiterMethod)
            : base(operand)
        {
            GetAwaiterMethod = getAwaiterMethod;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Await;

        /// <summary>
        /// Gets the GetAwaiter method used to await the asynchronous operation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter", Justification = "Get a waiter :-)")]
        public MethodInfo GetAwaiterMethod { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitAwait(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="UnaryCSharpExpression.Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AwaitCSharpExpression Update(Expression operand)
        {
            if (operand == Operand)
            {
                return this;
            }

            return CSharpExpression.Await(operand, GetAwaiterMethod);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        public static AwaitCSharpExpression Await(Expression operand)
        {
            return Await(operand, null);
        }

        /// <summary>
        /// Creates an <see cref="AwaitCSharpExpression"/> that represents awaiting an asynchronous operation.
        /// </summary>
        /// <param name="operand">An <see cref="Expression" /> that specifies the asynchronous operation to await.</param>
        /// <param name="getAwaiterMethod">The GetAwaiter method used to await the asynchronous operation.</param>
        /// <returns>An instance of the <see cref="AwaitCSharpExpression"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter", Justification = "Get a waiter :-)")]
        public static AwaitCSharpExpression Await(Expression operand, MethodInfo getAwaiterMethod)
        {
            ContractUtils.RequiresNotNull(operand, nameof(operand));

            RequiresCanRead(operand, nameof(operand));

            ValidateAwaitPattern(operand.Type, ref getAwaiterMethod);

            return new AwaitCSharpExpression(operand, getAwaiterMethod);
        }

        private static void ValidateAwaitPattern(Type operandType, ref MethodInfo getAwaiterMethod)
        {
            if (getAwaiterMethod == null)
            {
                getAwaiterMethod = operandType.GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            }

            ContractUtils.RequiresNotNull(getAwaiterMethod, nameof(getAwaiterMethod));

            ValidateGetAwaiterMethod(operandType, getAwaiterMethod);
            ValidateAwaiterType(getAwaiterMethod.ReturnType);
        }

        private static void ValidateGetAwaiterMethod(Type operandType, MethodInfo getAwaiterMethod)
        {
            ValidateMethodInfo(getAwaiterMethod);

            var getAwaiterParams = getAwaiterMethod.GetParametersCached();

            if (getAwaiterMethod.IsStatic)
            {
                if (getAwaiterParams.Length != 1)
                {
                    throw Error.GetAwaiterShouldTakeZeroParameters();
                }

                var firstParam = getAwaiterParams[0];
                if (!TypeUtils.AreReferenceAssignable(firstParam.ParameterType, operandType))
                {
                    throw LinqError.ExpressionTypeDoesNotMatchParameter(operandType, firstParam.ParameterType);
                }
            }
            else
            {
                if (getAwaiterParams.Length != 0)
                {
                    throw Error.GetAwaiterShouldTakeZeroParameters();
                }
                
                if (getAwaiterMethod.IsGenericMethod)
                {
                    throw Error.GetAwaiterShouldNotBeGeneric();
                }
            }

            var returnType = getAwaiterMethod.ReturnType;

            if (returnType == typeof(void) || returnType.IsByRef || returnType.IsPointer)
            {
                throw Error.GetAwaiterShouldReturnAwaiterType();
            }
        }

        private static void ValidateAwaiterType(Type awaiterType)
        {
            if (!typeof(INotifyCompletion).IsAssignableFrom(awaiterType))
            {
                throw Error.AwaiterTypeShouldImplementINotifyCompletion(awaiterType);
            }

            var isCompleted = awaiterType.GetProperty("IsCompleted", BindingFlags.Public | BindingFlags.Instance);
            if (isCompleted == null || isCompleted.GetMethod == null)
            {
                throw Error.AwaiterTypeShouldHaveIsCompletedProperty(awaiterType);
            }

            if (isCompleted.PropertyType != typeof(bool))
            {
                throw Error.AwaiterIsCompletedShouldReturnBool(awaiterType);
            }

            if (isCompleted.GetIndexParameters().Length != 0)
            {
                throw Error.AwaiterIsCompletedShouldNotBeIndexer(awaiterType);
            }

            var getResult = awaiterType.GetMethod("GetResult", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            if (getResult == null || getResult.IsGenericMethodDefinition)
            {
                throw Error.AwaiterTypeShouldHaveGetResultMethod(awaiterType);
            }

            var returnType = getResult.ReturnType;

            if (returnType.IsByRef || returnType.IsPointer)
            {
                throw Error.AwaiterGetResultTypeInvalid(awaiterType);
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AwaitCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitAwait(AwaitCSharpExpression node)
        {
            return node.Update(Visit(node.Operand));
        }
    }
}
