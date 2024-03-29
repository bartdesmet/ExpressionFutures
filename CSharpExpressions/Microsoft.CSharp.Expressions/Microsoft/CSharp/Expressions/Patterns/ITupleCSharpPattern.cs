﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;
    using static PatternHelpers;

    // REVIEW: Consider merging this with RecursiveCSharpPattern.

    /// <summary>
    /// Represents a pattern that matches a tuple using the <see cref="ITuple"/> interface.
    /// </summary>
    public sealed partial class ITupleCSharpPattern : CSharpPattern, IPositionalCSharpPattern
    {
        internal ITupleCSharpPattern(CSharpPatternInfo info, MethodInfo getLengthMethod, MethodInfo getItemMethod, ReadOnlyCollection<PositionalCSharpSubpattern> deconstruction)
            : base(info)
        {
            GetLengthMethod = getLengthMethod;
            GetItemMethod = getItemMethod;
            Deconstruction = deconstruction;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.ITuple;

        /// <summary>
        /// Gets the method used to obtain the tuple element count.
        /// </summary>
        public MethodInfo GetLengthMethod { get; }

        /// <summary>
        /// Gets the method used to access a tuple element.
        /// </summary>
        public MethodInfo GetItemMethod { get; }

        /// <summary>
        /// Gets the subpatterns to apply to the tuple elements.
        /// </summary>
        public ReadOnlyCollection<PositionalCSharpSubpattern> Deconstruction { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitITuplePattern(this);

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public override CSharpPattern ChangeType(Type inputType)
        {
            if (inputType == InputType)
            {
                return this;
            }

            return CSharpPattern.ITuple(PatternInfo(inputType, NarrowedType), GetLengthMethod, GetItemMethod, Deconstruction);
        }

        internal override Expression Reduce(Expression @object)
        {
            Expression createTest(Expression obj)
            {
                var exit = Expression.Label(typeof(bool), "__return");

                var stmts = new List<Expression>();

                AddFailIfNot(Expression.TypeIs(obj, typeof(ITuple)), exit, stmts);

                var temp = Expression.Parameter(typeof(ITuple), "__objT");
                stmts.Add(Expression.Assign(temp, Expression.Convert(obj, typeof(ITuple))));
                obj = temp;

                var deconstructionCount = Deconstruction.Count;

                AddFailIfNot(Expression.Equal(Expression.Call(obj, GetLengthMethod), CreateConstantInt32(deconstructionCount)), exit, stmts);

                for (var i = 0; i < deconstructionCount; i++)
                {
                    var deconstruction = Deconstruction[i];

                    var item = Expression.Call(obj, GetItemMethod, CreateConstantInt32(i));
                    var test = deconstruction.Pattern.Reduce(item);

                    AddFailIfNot(test, exit, stmts);
                }

                stmts.Add(Expression.Label(exit, ConstantTrue));

                return Expression.Block(new[] { temp }, stmts);
            }

            return PatternHelpers.Reduce(@object, createTest);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="deconstruction">The <see cref="Deconstruction" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ITupleCSharpPattern Update(IEnumerable<PositionalCSharpSubpattern> deconstruction)
        {
            if (SameElements(ref deconstruction!, Deconstruction))
            {
                return this;
            }

            return CSharpPattern.ITuple(_info, GetLengthMethod, GetItemMethod, deconstruction);
        }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates a pattern that matches a tuple using the <see cref="System.Runtime.CompilerServices.ITuple"/> interface.
        /// </summary>
        /// <param name="deconstruction">The subpatterns to apply to the tuple elements.</param>
        /// <returns>A <see cref="ITupleCSharpPattern" /> representing a tuple pattern.</returns>
        public static ITupleCSharpPattern ITuple(IEnumerable<PositionalCSharpSubpattern> deconstruction) => ITuple(info: null, getLengthMethod: null, getItemMethod: null, deconstruction);

        /// <summary>
        /// Creates a pattern that matches a tuple using the <see cref="System.Runtime.CompilerServices.ITuple"/> interface.
        /// </summary>
        /// <param name="deconstruction">The subpatterns to apply to the tuple elements.</param>
        /// <returns>A <see cref="ITupleCSharpPattern" /> representing a tuple pattern.</returns>
        public static ITupleCSharpPattern ITuple(params PositionalCSharpSubpattern[] deconstruction) => ITuple(info: null, getLengthMethod: null, getItemMethod: null, deconstruction);

        /// <summary>
        /// Creates a pattern that matches a tuple using the <see cref="System.Runtime.CompilerServices.ITuple"/> interface.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="getLengthMethod">The method used to obtain the tuple element count.</param>
        /// <param name="getItemMethod">The method used to access a tuple element.</param>
        /// <param name="deconstruction">The subpatterns to apply to the tuple elements.</param>
        /// <returns>A <see cref="ITupleCSharpPattern" /> representing a tuple pattern.</returns>
        public static ITupleCSharpPattern ITuple(CSharpPatternInfo? info, MethodInfo? getLengthMethod, MethodInfo? getItemMethod, IEnumerable<PositionalCSharpSubpattern> deconstruction)
        {
            if (info != null)
            {
                RequiresCompatiblePatternTypes(info.NarrowedType, typeof(ITuple));
            }
            else
            {
                info = PatternInfo(typeof(object), typeof(ITuple));
            }

            if (getLengthMethod != null)
            {
                ValidateMethodInfo(getLengthMethod, nameof(getLengthMethod));

                // NB: The check for an instance method with no args is part of the Call factory.
                var checkCallGetLength = Expression.Call(Expression.Default(info.NarrowedType), getLengthMethod);

                if (checkCallGetLength.Type != typeof(int))
                    throw Error.ITupleGetLengthShouldReturnInt32(nameof(getLengthMethod));
            }
            else
            {
                getLengthMethod = WellKnownMembers.ITupleGetLength;
            }

            if (getItemMethod != null)
            {
                ValidateMethodInfo(getItemMethod, nameof(getItemMethod));

                // NB: The check for an instance method with a single integer args is part of the Call factory.
                var checkCallGetItem = Expression.Call(Expression.Default(info.NarrowedType), getItemMethod, Expression.Default(typeof(int)));

                if (checkCallGetItem.Type != typeof(object))
                    throw Error.ITupleGetItemShouldReturnObject(nameof(getItemMethod));
            }
            else
            {
                getItemMethod = WellKnownMembers.ITupleGetItem;
            }

#pragma warning disable CA1062 // Validate arguments of public methods. (See bug https://github.com/dotnet/roslyn-analyzers/issues/6163)
            var deconstructionCollection = deconstruction.ToReadOnly();

            for (int i = 0, n = deconstructionCollection.Count; i < n; i++)
            {
                var positionalPattern = deconstructionCollection[i];

                RequiresNotNull(positionalPattern, nameof(deconstruction), i);

                if (positionalPattern.Field != null)
                    throw Error.ITuplePositionalPatternCannotHaveField(i, nameof(deconstruction), i);
                if (positionalPattern.Parameter != null)
                    throw Error.ITuplePositionalPatternCannotHaveParameter(i, nameof(deconstruction), i);

                //
                // CONSIDER: If the input type does not match, we could trigger ChangeType (which would do the opposite of narrowing,
                //           i.e. generalize to 'object').
                //

                if (positionalPattern.Pattern.InputType != typeof(object))
                    throw Error.ITuplePositionalPatternInvalidInputType(i, positionalPattern.Pattern.InputType, nameof(deconstruction), i);
            }
#pragma warning restore CA1062 // Validate arguments of public methods

            return new ITupleCSharpPattern(info, getLengthMethod, getItemMethod, deconstructionCollection);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ITupleCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitITuplePattern(ITupleCSharpPattern node) =>
            node.Update(
                Visit(node.Deconstruction, VisitPositionalSubpattern)
            );
    }
}
