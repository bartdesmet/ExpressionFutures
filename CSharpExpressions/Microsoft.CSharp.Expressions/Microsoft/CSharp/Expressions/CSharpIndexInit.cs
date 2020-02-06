// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    partial class CSharpExpression
    {
        // NB: We hijack ElementInit to achieve the effect of setting indexers. This is to work around the limitations
        //     imposed by the MemberInit family of LINQ expression nodes, which are not extensible. The initial goal
        //     is to have factory methods available for the compiler to bind to.
        
        // NB: ElementInit does not derive from MemberBinding, so this temporary workaround only works in combination
        //     with ListInit expressions. Ideally, IndexInit (or another name) should be a fourth kind of MemberBinding
        //     so it can be used in MemberInit expressions. Maybe MemberBinding needs an Extension kind as well, so
        //     languages like C# and VB can contribute their own nodes (though it's unclear what Reduce would mean for
        //     such extensions given member bindings are special-cased in the LambdaCompiler).

        // NB: We don't have overloads with ParameterAssignment because we can't forward that to ElementInit nodes while
        //     keeping the desired evaluation order. In order to solve this, we need extensibility of member bindings
        //     with Reduce capabilities.

        // DESIGN: Combining all remarks above, we definitely need a custom node type to represent indexer assignments
        //         in member initialization expressions.

        /// <summary>
        /// Creates an <see cref="ElementInit" /> that represents assignment to an indexer.
        /// </summary>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the indexer to set.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <param name="value">The <see cref="Expression"/> that represents the value to assign to the indexer.</param>
        /// <returns>An <see cref="ElementInit" /> that assigns the specified value to the specified indexer using the specified arguments.</returns>
        public static ElementInit IndexInit(MethodInfo indexer, Expression[] arguments, Expression value)
        {
            return IndexInit(indexer, (IEnumerable<Expression>)arguments, value);
        }

        /// <summary>
        /// Creates an <see cref="ElementInit" /> that represents assignment to an indexer.
        /// </summary>
        /// <param name="indexer">The <see cref="MethodInfo" /> representing an accessor of the indexer to set.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <param name="value">The <see cref="Expression"/> that represents the value to assign to the indexer.</param>
        /// <returns>An <see cref="ElementInit" /> that assigns the specified value to the specified indexer using the specified arguments.</returns>
        public static ElementInit IndexInit(MethodInfo indexer, IEnumerable<Expression> arguments, Expression value)
        {
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));

            return IndexInit(GetProperty(indexer), arguments, value);
        }

        /// <summary>
        /// Creates an <see cref="ElementInit" /> that represents assignment to an indexer.
        /// </summary>
        /// <param name="indexer">The <see cref="PropertyInfo" /> that represents the indexer to set.</param>
        /// <param name="arguments">An array of one or more of <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <param name="value">The <see cref="Expression"/> that represents the value to assign to the indexer.</param>
        /// <returns>An <see cref="ElementInit" /> that assigns the specified value to the specified indexer using the specified arguments.</returns>
        public static ElementInit IndexInit(PropertyInfo indexer, Expression[] arguments, Expression value)
        {
            return IndexInit(indexer, (IEnumerable<Expression>)arguments, value);
        }

        /// <summary>
        /// Creates an <see cref="ElementInit" /> that represents assignment to an indexer.
        /// </summary>
        /// <param name="indexer">The <see cref="PropertyInfo" /> that represents the indexer to set.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}" /> that contains <see cref="Expression" /> objects that represent the indexer arguments.</param>
        /// <param name="value">The <see cref="Expression"/> that represents the value to assign to the indexer.</param>
        /// <returns>An <see cref="ElementInit" /> that assigns the specified value to the specified indexer using the specified arguments.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Done by helper method.")]
        public static ElementInit IndexInit(PropertyInfo indexer, IEnumerable<Expression> arguments, Expression value)
        {
            ContractUtils.RequiresNotNull(indexer, nameof(indexer));
            ContractUtils.RequiresNotNull(arguments, nameof(arguments));
            ContractUtils.RequiresNotNull(value, nameof(value));

            var argList = arguments.Concat(new[] { value }).ToReadOnly();
            RequiresCanRead(argList, nameof(arguments));

            var setter = indexer.GetSetMethod(true);
            if (setter == null)
            {
                throw Error.PropertyDoesNotHaveSetAccessor(indexer);
            }

            ValidateIndexer(indexer.DeclaringType, indexer);

            ValidateArgumentTypes(setter, ExpressionType.Call, ref argList);

            return ElementInitStub.Create(setter, argList);
        }
    }
}
