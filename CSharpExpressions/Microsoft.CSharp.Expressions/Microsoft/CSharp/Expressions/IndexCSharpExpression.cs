// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.CSharp.Expressions.Helpers;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents indexing a property.
    /// </summary>
    public abstract partial class IndexCSharpExpression : CSharpExpression
    {
        internal IndexCSharpExpression(Expression @object, ReadOnlyCollection<ParameterAssignment> arguments)
        {
            Object = @object;
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
        public abstract PropertyInfo Indexer { get; }

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

            return Rewrite(@object, arguments);
        }

        internal abstract IndexCSharpExpression Rewrite(Expression @object, IEnumerable<ParameterAssignment> arguments);

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            // TODO: Throw a proper exception if the indexer is set-only, which can happen when our node is used in
            //       a LINQ expression where RequiresCanRead just passes fine.

            var method = Indexer.GetGetMethod(true);
            var parameters = method.GetParametersCached();

            var res = BindArguments((obj, args) => Expression.Property(obj, Indexer, args), Object, parameters, Arguments);

            return res;
        }

        internal Expression ReduceAssign(Func<Expression, Expression> assign)
        {
            var method = Indexer.GetGetMethod(true);
            var parameters = method.GetParametersCached();

            // TODO: Check all writeback cases with mutable structs.
            var res = BindArguments((obj, args) => assign(Expression.Property(obj, Indexer, args)), Object, parameters, Arguments, needTemps: true);

            return res;
        }

        internal class MethodBased : IndexCSharpExpression
        {
            private readonly MethodInfo _method;

            public MethodBased(Expression @object, MethodInfo method, ReadOnlyCollection<ParameterAssignment> arguments)
                : base(@object, arguments)
            {
                _method = method;
            }

            public override PropertyInfo Indexer => GetProperty(_method);

            internal override IndexCSharpExpression Rewrite(Expression @object, IEnumerable<ParameterAssignment> arguments)
            {
                return CSharpExpression.Index(@object, _method, arguments);
            }
        }

        internal class PropertyBased : IndexCSharpExpression
        {
            public PropertyBased(Expression @object, PropertyInfo indexer, ReadOnlyCollection<ParameterAssignment> arguments)
                : base(@object, arguments)
            {
                Indexer = indexer;
            }

            public override PropertyInfo Indexer { get; }

            internal override IndexCSharpExpression Rewrite(Expression @object, IEnumerable<ParameterAssignment> arguments)
            {
                return CSharpExpression.Index(@object, Indexer, arguments);
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the indexer arguments.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static IndexCSharpExpression Index(Expression instance, MethodInfo indexer, params ParameterAssignment[] arguments)
        {
            return Index(instance, indexer, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="IndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static IndexCSharpExpression Index(Expression instance, MethodInfo indexer, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            var property = GetProperty(indexer);
            return IndexCore(instance, property, indexer, indexer.GetParametersCached(), arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="ParameterAssignment" /> objects that represent the indexer arguments.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, params ParameterAssignment[] arguments)
        {
            return Index(instance, indexer, (IEnumerable<ParameterAssignment>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="ParameterAssignment" /> objects to use to populate the <see cref="IndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, IEnumerable<ParameterAssignment> arguments)
        {
            return IndexCore(instance, indexer, null, null, arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static IndexCSharpExpression Index(Expression instance, MethodInfo indexer, Expression[] arguments)
        {
            // NB: no params array to avoid overload resolution ambiguity
            return Index(instance, indexer, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="IndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static IndexCSharpExpression Index(Expression instance, MethodInfo indexer, IEnumerable<Expression> arguments)
        {
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            var property = GetProperty(indexer);
            return IndexCore(instance, property, indexer, indexer.GetParametersCached(), arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, Expression[] arguments)
        {
            // NB: no params array to avoid overload resolution ambiguity
            return Index(instance, indexer, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        /// Creates a <see cref="IndexCSharpExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">An <see cref="Expression" /> that specifies the instance to index.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> representing the property to index.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects to use to populate the <see cref="IndexCSharpExpression.Arguments" /> collection.</param>
        /// <returns>A <see cref="IndexCSharpExpression" /> that has the <see cref="CSharpNodeType" /> property equal to <see cref="CSharpExpressionType.Index" /> and the <see cref="IndexCSharpExpression.Object" />, <see cref="IndexCSharpExpression.Indexer" />, and <see cref="IndexCSharpExpression.Arguments" /> properties set to the specified values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static IndexCSharpExpression Index(Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments)
        {
            return IndexCore(instance, indexer, null, null, arguments);
        }

        private static IndexCSharpExpression IndexCore(Expression instance, PropertyInfo indexer, MethodInfo method, ParameterInfo[] parameters, IEnumerable<ParameterAssignment> arguments)
        {
            ContractUtils.RequiresNotNull(instance, nameof(instance));
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            parameters = GetParameters(indexer, parameters);

            return MakeIndex(instance, indexer, method, parameters, arguments);
        }

        private static IndexCSharpExpression IndexCore(Expression instance, PropertyInfo indexer, MethodInfo method, ParameterInfo[] parameters, IEnumerable<Expression> arguments)
        {
            ContractUtils.RequiresNotNull(instance, nameof(instance));
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            parameters = GetParameters(indexer, parameters);

            var bindings = GetParameterBindings(parameters, arguments);

            return MakeIndex(instance, indexer, method, parameters, bindings);
        }

        private static IndexCSharpExpression MakeIndex(Expression instance, PropertyInfo indexer, MethodInfo method, ParameterInfo[] parameters, IEnumerable<ParameterAssignment> arguments)
        {
            RequiresCanRead(instance, nameof(instance));

            var argList = arguments.ToReadOnly();

            ValidateIndexer(instance.Type, indexer, parameters, argList);

            if (method != null)
            {
                return new IndexCSharpExpression.MethodBased(instance, method, argList);
            }
            else
            {
                return new IndexCSharpExpression.PropertyBased(instance, indexer, argList);
            }
        }

        private static ParameterInfo[] GetParameters(PropertyInfo indexer, ParameterInfo[] parameters)
        {
            return parameters ?? indexer.GetIndexParameters();
        }

        private static void ValidateIndexer(Type instanceType, PropertyInfo indexer, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> argList)
        {
            ValidateIndexer(instanceType, indexer);

            // TODO: Lift this restriction and allow a set-only indexer; also need a custom RequiresCanRead
            //       for our own checks, and need a proper exception in Reduce.

            // We ignore validating the setter. C# has no assignment expression support yet and the LINQ API
            // won't consider our node as assignable, so it can't occur in assignment targets. As such, the
            // compiler won't encounter our node in assignment positions either, so doesn't have to reduce
            // the node to a setter invocation. Our Reduce method will assume getter access.

            var getter = indexer.GetGetMethod(true);
            if (getter == null)
            {
                throw Error.PropertyDoesNotHaveGetAccessor(indexer);
            }

            ValidateParameterBindings(getter, parameters, argList);
        }

        private static void ValidateIndexer(Type instanceType, PropertyInfo indexer)
        {
            // NB: We rely on the LINQ API to do validation of the indexer, including the setter (if any). We
            //     have to validate the setter as well because this node could reduce into an assignment when
            //     used in combination with C# assignment expressions. Note that the LINQ API won't treat our
            //     node as assignable when used with LINQ assignment expressions. Our reduction of assignment
            //     will properly call the setter, thus we have to make sure things are well-typed.

            var parameters = indexer.GetIndexParameters();

            var n = parameters.Length;
            var args = new Expression[n];
            for (var i = 0; i < n; i++)
            {
                // NB: This is just a trick to be compatible with the signature of ValidateIndexeProperty but
                //     we could change the LINQ APIs to have a variant that just takes in types. Shouldn't be
                //     a big deal though, given that most indexers only have a few parameters.
                args[i] = Expression.Default(parameters[i].ParameterType);
            }

            var original = new TrueReadOnlyCollection<Expression>(args);
            var argList = (ReadOnlyCollection<Expression>)original;
            ValidateIndexedProperty(Expression.Default(instanceType), indexer, ref argList);

            // NB: We don't expect mutations because all expressions match the corresponding indexer parameter
            //     type. As such, we shouldn't end up quoting any argument.
            Debug.Assert(argList == original);
        }
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
