// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Threading.Tasks;
using static System.Linq.Expressions.ExpressionStubs;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an asynchronous lambda expression.
    /// </summary>
    public abstract class AsyncLambdaCSharpExpression : CSharpExpression
    {
        internal AsyncLambdaCSharpExpression(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            Body = body;
            Parameters = parameters;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.AsyncLambda;

        /// <summary>
        /// Gets the body of the lambda expression.
        /// </summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the body of the lambda expression.</returns>
        public Expression Body { get; }

        /// <summary>
        /// Gets the parameters of the lambda expression.
        /// </summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects that represent the parameters of the lambda expression.</returns>
        public ReadOnlyCollection<ParameterExpression> Parameters { get; }
    }

    /// <summary>
    /// Represents an asynchronous lambda expression.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate represented by this expression.</typeparam>
    public sealed class AsyncCSharpExpression<TDelegate> : AsyncLambdaCSharpExpression
    {
        internal AsyncCSharpExpression(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
            : base(body, parameters)
        {
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitAsyncLambda(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="body">The <see cref="P:System.Linq.Expressions.LambdaExpression.Body" /> property of the result.</param>
        /// <param name="parameters">The <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public AsyncCSharpExpression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == base.Body && parameters == base.Parameters)
            {
                return this;
            }

            return CSharpExpression.AsyncLambda<TDelegate>(body, parameters);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static AsyncLambdaCSharpExpression AsyncLambda(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));

            var parameterList = parameters.ToReadOnly<ParameterExpression>();

            var count = parameterList.Count;
            var types = new Type[count + 1];

            if (count > 0)
            {
                var set = new Set<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameterList[i];

                    ValidateAsyncParameter(parameter);

                    types[i] = parameter.Type;

                    if (set.Contains(parameter))
                    {
                        throw LinqError.DuplicateVariable(parameter);
                    }

                    set.Add(parameter);
                }
            }

            if (body.Type == typeof(void))
            {
                types[count] = typeof(Task); // REVIEW: OK to default to Task?
            }
            else
            {
                types[count] = typeof(Task<>).MakeGenericType(body.Type);
            }

            return CreateAsyncLambda(DelegateHelpers.MakeDelegateType(types), body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda(delegateType, body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> using the specified delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="T:System.Type" /> that represents a delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncLambdaCSharpExpression AsyncLambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidAsyncLambdaArgs(delegateType, ref body, parameterList);

            return CreateAsyncLambda(delegateType, body, parameterList);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncCSharpExpression`1" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return AsyncLambda<TDelegate>(body, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.CSharp.Expressions.AsyncCSharpExpression`1" /> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression" /> to set the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> property equal to.</param>
        /// <param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="T:System.Linq.Expressions.ParameterExpression" /> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters" /> collection.</param>
        /// <returns>An <see cref="T:AsyncLambdaCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.NodeType" /> property equal to AsyncLambda and the <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Body" /> and <see cref="P:Microsoft.CSharp.Expressions.AsyncLambdaCSharpExpression.Parameters" /> properties set to the specified values.</returns>
        public static AsyncCSharpExpression<TDelegate> AsyncLambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();

            ValidAsyncLambdaArgs(typeof(TDelegate), ref body, parameterList);

            return new AsyncCSharpExpression<TDelegate>(body, parameterList);
        }

        private static void ValidAsyncLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            RequiresCanRead(body, "body");

            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
            {
                throw LinqError.LambdaTypeMustBeDerivedFromSystemDelegate();
            }

            var count = parameters.Count;

            var method = delegateType.GetMethod("Invoke"); // TODO: use cache from LINQ
            var parametersCached = method.GetParametersCached();

            if (parametersCached.Length != 0)
            {
                if (parametersCached.Length != count)
                {
                    throw LinqError.IncorrectNumberOfLambdaDeclarationParameters();
                }

                var set = new Set<ParameterExpression>(count);

                for (var i = 0; i < count; i++)
                {
                    var parameter = parameters[i];
                    ValidateAsyncParameter(parameter);

                    var parameterType = parametersCached[i].ParameterType;

                    if (!TypeUtils.AreReferenceAssignable(parameter.Type, parameterType))
                    {
                        throw LinqError.ParameterExpressionNotValidAsDelegate(parameter.Type, parameterType);
                    }

                    if (set.Contains(parameter))
                    {
                        throw LinqError.DuplicateVariable(parameter);
                    }

                    set.Add(parameter);
                }
            }
            else if (count > 0)
            {
                throw LinqError.IncorrectNumberOfLambdaDeclarationParameters();
            }

            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];

                if (!TypeUtils.AreReferenceAssignable(resultType, body.Type) && !TryQuote(resultType, ref body))
                {
                    throw LinqError.ExpressionTypeDoesNotMatchReturn(body.Type, method.ReturnType);
                }
            }
            else if (returnType != typeof(void) && returnType == typeof(Task))
            {
                throw Error.AsyncLambdaInvalidReturnType(returnType);
            }
        }

        private static AsyncLambdaCSharpExpression CreateAsyncLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            throw new NotImplementedException();
        }

        private static void ValidateAsyncParameter(ParameterExpression parameter)
        {
            RequiresCanRead(parameter, "parameters");

            if (parameter.IsByRef)
            {
                throw Error.AsyncLambdaCantHaveByRefParameter(parameter);
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AsyncCSharpExpression{TDelegate}" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitAsyncLambda<TDelegate>(AsyncCSharpExpression<TDelegate> node)
        {
            return node.Update(Visit(node.Body), VisitAndConvert<ParameterExpression>(node.Parameters, nameof(VisitAsyncLambda)));
        }
    }
}
