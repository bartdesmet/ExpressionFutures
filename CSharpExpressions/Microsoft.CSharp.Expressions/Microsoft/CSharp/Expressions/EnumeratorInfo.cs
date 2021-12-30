// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Provides binding information for foreach operations.
    /// </summary>
    public sealed class EnumeratorInfo
    {
        internal EnumeratorInfo(
            bool isAsync,
            Type collectionType,
            LambdaExpression getEnumerator,
            LambdaExpression moveNext,
            PropertyInfo current,
            LambdaExpression currentConversion,
            Type elementType,
            bool needsDisposal,
            AwaitInfo disposeAwaitInfo,
            LambdaExpression patternDispose)
        {
            IsAsync = isAsync;
            CollectionType = collectionType;
            GetEnumerator = getEnumerator;
            MoveNext = moveNext;
            Current = current;
            CurrentConversion = currentConversion;
            ElementType = elementType;
            NeedsDisposal = needsDisposal;
            DisposeAwaitInfo = disposeAwaitInfo;
            PatternDispose = patternDispose;
        }

        /// <summary>
        /// Gets a Boolean indicating whether the enumeration is asynchronous.
        /// </summary>
        public bool IsAsync { get; }

        /// <summary>
        /// Gets the type of the collection being enumerated.
        /// </summary>
        public Type CollectionType { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the operation used to obtain an enumerator from the collection of type <see cref="CollectionType"/>.
        /// </summary>
        public LambdaExpression GetEnumerator { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the operation used to invoke MoveNext or MoveNextAsync on the enumerator instance returned by <see cref="GetEnumerator"/>.
        /// </summary>
        public LambdaExpression MoveNext { get; }

        /// <summary>
        /// Gets the Current property to access on the enumerator instance returned by <see cref="GetEnumerator"/>.
        /// </summary>
        public PropertyInfo Current { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the conversion of the object returned from the <see cref="Current"/> property to the <see cref="ElementType"/>.
        /// </summary>
        public LambdaExpression CurrentConversion { get; }

        /// <summary>
        /// Gets the type of the elements obtained by the iteration.
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// Gets a Boolean indicating whether a call to Dispose or DisposeAsync is required.
        /// </summary>
        public bool NeedsDisposal { get; }

        /// <summary>
        /// Gets the information required to await the DisposeAsync operation for await foreach statements.
        /// </summary>
        public AwaitInfo DisposeAwaitInfo { get; }

        /// <summary>
        /// Gets the (optional) <see cref="LambdaExpression"/> representing how to call the dispose method.
        /// </summary>
        public LambdaExpression PatternDispose { get; }

        /// <summary>
        /// Creates a new object that is like this one, but using the supplied children. If all of the children are the same, it will return this object.
        /// </summary>
        /// <param name="getEnumerator">The <see cref="GetEnumerator"/> property of the result.</param>
        /// <param name="moveNext">The <see cref="MoveNext"/> property of the result.</param>
        /// <param name="currentConversion">The <see cref="CurrentConversion"/> property of the result.</param>
        /// <param name="disposeAwaitInfo">The <see cref="DisposeAwaitInfo"/> property of the result.</param>
        /// <param name="patternDispose">The <see cref="PatternDispose"/> property of the result.</param>
        /// <returns>This object if no children changed, or an object with the updated children.</returns>
        public EnumeratorInfo Update(LambdaExpression getEnumerator, LambdaExpression moveNext, LambdaExpression currentConversion, AwaitInfo disposeAwaitInfo, LambdaExpression patternDispose)
        {
            if (getEnumerator == GetEnumerator && moveNext == MoveNext && currentConversion == CurrentConversion && disposeAwaitInfo == DisposeAwaitInfo && patternDispose == PatternDispose)
            {
                return this;
            }

            return CSharpExpression.EnumeratorInfo(IsAsync, CollectionType, getEnumerator, moveNext, Current, currentConversion, ElementType, NeedsDisposal, DisposeAwaitInfo, patternDispose);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        internal EnumeratorInfo Accept(CSharpExpressionVisitor visitor) => visitor.VisitEnumeratorInfo(this);
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a new <see cref="EnumeratorInfo"/> object providing binding information for foreach operations.
        /// </summary>
        /// <param name="isAsync">A Boolean indicating whether the enumeration is asynchronous.</param>
        /// <param name="collectionType">The type of the collection being enumerated.</param>
        /// <param name="getEnumerator">The <see cref="LambdaExpression"/> representing the operation used to obtain an enumerator from the collection.</param>
        /// <param name="moveNext">The <see cref="LambdaExpression"/> representing the operation used to invoke MoveNext or MoveNextAsync on the enumerator instance.</param>
        /// <param name="currentPropertyGetMethod">The get method of the Current property on the enumerator.</param>
        /// <param name="currentConversion">The <see cref="LambdaExpression"/> representing the conversion of the object returned from the Current property to the element type.</param>
        /// <param name="elementType">The type of the elements obtained by the iteration.</param>
        /// <param name="needsDisposal">A Boolean indicating whether a call to Dispose or DisposeAsync is required.</param>
        /// <param name="disposeAwaitInfo">The information required to await the DisposeAsync operation for await foreach statements.</param>
        /// <param name="patternDispose">The (optional) <see cref="LambdaExpression"/> representing how to call the dispose method.</param>
        /// <returns>A <see cref="EnumeratorInfo"/> object providing binding information for foreach operations.</returns>
        public static EnumeratorInfo EnumeratorInfo(
            bool isAsync,
            Type collectionType,
            LambdaExpression getEnumerator,
            LambdaExpression moveNext,
            MethodInfo currentPropertyGetMethod,
            LambdaExpression currentConversion,
            Type elementType,
            bool needsDisposal,
            AwaitInfo disposeAwaitInfo,
            LambdaExpression patternDispose) =>
            EnumeratorInfo(
                isAsync,
                collectionType,
                getEnumerator,
                moveNext,
                GetProperty(currentPropertyGetMethod),
                currentConversion,
                elementType,
                needsDisposal,
                disposeAwaitInfo,
                patternDispose
            );

        /// <summary>
        /// Creates a new <see cref="EnumeratorInfo"/> object providing binding information for foreach operations.
        /// </summary>
        /// <param name="isAsync">A Boolean indicating whether the enumeration is asynchronous.</param>
        /// <param name="collectionType">The type of the collection being enumerated.</param>
        /// <param name="getEnumerator">The <see cref="LambdaExpression"/> representing the operation used to obtain an enumerator from the collection.</param>
        /// <param name="moveNext">The <see cref="LambdaExpression"/> representing the operation used to invoke MoveNext or MoveNextAsync on the enumerator instance.</param>
        /// <param name="current">The Current property on the enumerator.</param>
        /// <param name="currentConversion">The <see cref="LambdaExpression"/> representing the conversion of the object returned from the Current property to the element type.</param>
        /// <param name="elementType">The type of the elements obtained by the iteration.</param>
        /// <param name="needsDisposal">A Boolean indicating whether a call to Dispose or DisposeAsync is required.</param>
        /// <param name="disposeAwaitInfo">The information required to await the DisposeAsync operation for await foreach statements.</param>
        /// <param name="patternDispose">The (optional) <see cref="LambdaExpression"/> representing how to call the dispose method.</param>
        /// <returns>A <see cref="EnumeratorInfo"/> object providing binding information for foreach operations.</returns>
        public static EnumeratorInfo EnumeratorInfo(
            bool isAsync,
            Type collectionType,
            LambdaExpression getEnumerator,
            LambdaExpression moveNext,
            PropertyInfo current,
            LambdaExpression currentConversion,
            Type elementType,
            bool needsDisposal,
            AwaitInfo disposeAwaitInfo,
            LambdaExpression patternDispose)
        {
            RequiresNotNull(collectionType, nameof(collectionType));
            ValidateType(collectionType);

            RequiresCanRead(getEnumerator, nameof(getEnumerator));

            if (getEnumerator.Parameters.Count != 1)
                throw new Exception(); // TODO
            if (!AreReferenceAssignable(getEnumerator.Parameters[0].Type, collectionType))
                throw new Exception(); // TODO

            var enumeratorType = getEnumerator.ReturnType;

            RequiresCanRead(moveNext, nameof(moveNext));

            if (moveNext.Parameters.Count != 1)
                throw new Exception(); // TODO
            if (!AreReferenceAssignable(moveNext.Parameters[0].Type, enumeratorType))
                throw new Exception(); // TODO
            if (!isAsync && moveNext.ReturnType != typeof(bool))
                throw new Exception(); // TOOD

            //
            // REVIEW: We don't have info about the await operation on MoveNextAsync here, so can't validate the return type.
            //
            //         Note we can't call CSharpExpression.Await to attempt to infer an await operation, because GetAwaiter
            //         could come in through an extension method. We'll have the ForEach factory do the final check.
            //

            RequiresNotNull(current, nameof(current));

            var currentGetMethod = current.GetGetMethod(nonPublic: false);
            if (currentGetMethod == null)
                throw new Exception(); // TOOD
            if (currentGetMethod.IsStatic)
                throw new Exception();
            if (current.GetIndexParameters().Length != 0)
                throw new Exception();

            var currentType = current.PropertyType;

            if (currentType == typeof(void))
                throw new Exception(); // TOOD

            if (currentConversion != null)
            {
                RequiresCanRead(currentConversion, nameof(currentConversion));

                if (currentConversion.Parameters.Count != 1)
                    throw new Exception(); // TODO
                if (!AreReferenceAssignable(currentConversion.Parameters[0].Type, currentType))
                    throw new Exception(); // TODO
                if (!AreReferenceAssignable(elementType, currentConversion.ReturnType))
                    throw new Exception(); // TODO
            }
            else
            {
                if (!AreReferenceAssignable(elementType, currentType))
                    throw new Exception(); // TODO
            }

            if (patternDispose != null)
            {
                if (patternDispose.Parameters.Count != 1)
                    throw Error.UsingPatternDisposeShouldHaveOneParameter();
            }

            if (isAsync)
            {
                AssertUsingAwaitInfo(ref disposeAwaitInfo, patternDispose);
            }

            CheckUsingResourceType(enumeratorType, disposeAwaitInfo, patternDispose, allowConvertToDisposable: true);

            return new EnumeratorInfo(isAsync, collectionType, getEnumerator, moveNext, current, currentConversion, elementType, needsDisposal, disposeAwaitInfo, patternDispose);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="EnumeratorInfo" />.
        /// </summary>
        /// <param name="node">The object to visit.</param>
        /// <returns>The modified object, if it or any subexpression was modified; otherwise, returns the original object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual EnumeratorInfo VisitEnumeratorInfo(EnumeratorInfo node) =>
            node.Update(
                VisitAndConvert(node.GetEnumerator, nameof(VisitEnumeratorInfo)),
                VisitAndConvert(node.MoveNext, nameof(VisitEnumeratorInfo)),
                VisitAndConvert(node.CurrentConversion, nameof(VisitEnumeratorInfo)),
                VisitAwaitInfo(node.DisposeAwaitInfo),
                VisitAndConvert(node.PatternDispose, nameof(VisitEnumeratorInfo))
            );
    }
}
