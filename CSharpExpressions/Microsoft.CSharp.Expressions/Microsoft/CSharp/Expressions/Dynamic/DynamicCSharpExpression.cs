// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.CSharp.Expressions
{
    // TODO: Support for assignments, setter operations, and need for IsEvent

    /// <summary>
    /// Base class for dynamically bound C# expressions.
    /// </summary>
    public abstract partial class DynamicCSharpExpression : CSharpExpression
    {
        internal DynamicCSharpExpression(Type? context, CSharpBinderFlags binderFlags)
        {
            Context = context;
            Flags = binderFlags;
        }

        /// <summary>
        /// Gets the <see cref="System.Type" /> that indicates where this operation is used.
        /// </summary>
        public Type? Context { get; }

        /// <summary>
        /// Gets the flags with the dynamic operation binder will be initialized.
        /// </summary>
        public CSharpBinderFlags Flags { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents. (Inherited from <see cref="System.Linq.Expressions.Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => typeof(object);

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[]? argumentTypes);

            return DynamicHelpers.MakeDynamic(Type, binder, arguments, argumentTypes);
        }

        /// <summary>
        /// Reduces the dynamic expression to a binder and a set of arguments to apply the operation to.
        /// </summary>
        /// <param name="binder">The binder used to perform the dynamic operation.</param>
        /// <param name="arguments">The arguments to apply the dynamic operation to.</param>
        /// <param name="argumentTypes">The types of the arguments to use for the dynamic call site. Return null to infer types.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "For internal use only.")]
        protected abstract void ReduceDynamic(out CallSiteBinder binder, out IEnumerable<Expression> arguments, out Type[]? argumentTypes);
    }
}
