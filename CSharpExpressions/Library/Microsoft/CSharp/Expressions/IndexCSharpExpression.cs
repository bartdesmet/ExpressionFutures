// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;
using static Microsoft.CSharp.Expressions.Helpers;
using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents indexing a property.
    /// </summary>
    public sealed class IndexCSharpExpression : CSharpExpression
    {
        internal IndexCSharpExpression(Expression @object, PropertyInfo indexer, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            Object = @object;
            Indexer = indexer;
            Arguments = arguments;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.Index;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Indexer.PropertyType;

        /// <summary>
        /// Gets the <see cref="Expression" /> that represents the object to index.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Gets the <see cref="PropertyInfo" /> for the indexer property.
        /// </summary>
        public PropertyInfo Indexer { get; }

        /// <summary>
        /// Gets a collection of argument assignments.
        /// </summary>
        public ReadOnlyCollection<ParameterAssignment> Arguments { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitIndex(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public IndexCSharpExpression Update(Expression @object, IEnumerable<ParameterAssignment> arguments)
        {
            if (@object == Object && arguments == Arguments)
            {
                return this;
            }

            return CSharpExpression.Index(@object, Indexer, arguments);
        }

        /// <summary>
        /// Reduces the call expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var method = Indexer.GetGetMethod(true);
            var parameters = method.GetParametersCached();

            if (CheckArgumentsInOrder(Arguments))
            {
                var args = new Expression[parameters.Length];

                foreach (var argument in Arguments)
                {
                    args[argument.Parameter.Position] = argument.Expression;
                }

                FillOptionalParameters(parameters, args);

                return Expression.Property(Object, Indexer, args);
            }
            else
            {
                var vars = new ParameterExpression[Arguments.Count + 1];
                var exprs = new Expression[vars.Length + 1];
                var args = new Expression[parameters.Length];

                var obj = Expression.Parameter(Object.Type, "obj");
                vars[0] = obj;
                exprs[0] = Expression.Assign(obj, Object);

                var i = 1;

                foreach (var argument in Arguments)
                {
                    var parameter = argument.Parameter;
                    var expression = argument.Expression;

                    var var = Expression.Parameter(argument.Expression.Type, parameter.Name);
                    vars[i] = var;
                    exprs[i] = Expression.Assign(var, expression);

                    args[parameter.Position] = var;

                    i++;
                }

                FillOptionalParameters(parameters, args);

                exprs[i] = Expression.Property(obj, Indexer, args);

                return Expression.Block(vars, exprs);
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> that represents the indexer arguments.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Call" /> and the <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Indexer" />, and <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, params ParameterAssignment[] arguments)
        {
            return Index(instance, indexer, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="P:Microsoft.CSharp.Expressions.CSharpExpression.CSharpNodeType" /> property equal to <see cref="F:Microsoft.CSharp.Expressions.CSharpExpressionType.Index" /> and the <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Object" />, <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Indexer" />, and <see cref="P:Microsoft.CSharp.Expressions.IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(instance, nameof(instance));
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            RequiresCanRead(instance, nameof(instance));

            if (indexer.PropertyType.IsByRef) throw LinqError.PropertyCannotHaveRefType();
            if (indexer.PropertyType == typeof(void)) throw LinqError.PropertyTypeCannotBeVoid();

            // We ignore validating the setter. C# has no assignment expression support yet and the LINQ API
            // won't consider our node as assignable, so it can't occur in assignment targets. As such, the
            // compiler won't encounter our node in assignment positions either, so doesn't have to reduce
            // the node to a setter invocation. Our Reduce method will assume getter access.

            var getter = indexer.GetGetMethod(true);
            if (getter == null)
            {
                throw Error.PropertyDoesNotHaveGetAccessor(indexer);
            }

            if (getter.IsStatic)
            {
                throw Error.AccessorCannotBeStatic(indexer);
            }

            ValidateMethodInfo(getter);
            ValidateCallInstanceType(instance.Type, getter);

            var argList = arguments.ToReadOnly();

            foreach (var arg in argList)
            {
                if (arg.Parameter.ParameterType.IsByRef)
                {
                    throw LinqError.AccessorsCannotHaveByRefArgs();
                }
            }

            ValidateParameterBindings(getter, indexer.GetIndexParameters(), argList);

            return new IndexCSharpExpression(instance, indexer, argList);
        }

        // TODO: Add overload with MethodInfo for the C# compiler to emit a call to using `ldtoken` for the getter method.
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="IndexCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitIndex(IndexCSharpExpression node)
        {
            return node.Update(Visit(node.Object), Visit(node.Arguments, VisitParameterAssignment));
        }
    }
}
