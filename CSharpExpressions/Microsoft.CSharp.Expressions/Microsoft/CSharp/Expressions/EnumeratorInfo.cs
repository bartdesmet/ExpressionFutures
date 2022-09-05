// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Provides binding information for foreach operations.
    /// </summary>
    public sealed partial class EnumeratorInfo
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
        /// Creates a new <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.
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
        /// <returns>A <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.</returns>
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
                GetProperty(currentPropertyGetMethod, nameof(currentPropertyGetMethod)),
                currentConversion,
                elementType,
                needsDisposal,
                disposeAwaitInfo,
                patternDispose
            );

        /// <summary>
        /// Creates a new <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.
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
        /// <returns>A <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.</returns>
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
            ValidateType(collectionType, nameof(collectionType));

            RequiresCanRead(getEnumerator, nameof(getEnumerator));

            if (getEnumerator.Parameters.Count != 1)
                throw Error.GetEnumeratorShouldHaveSingleParameter();
            if (!AreReferenceAssignable(getEnumerator.Parameters[0].Type, collectionType))
                throw Error.InvalidGetEnumeratorFirstArgType(getEnumerator.Parameters[0].Type, collectionType);

            var enumeratorType = getEnumerator.ReturnType;

            RequiresCanRead(moveNext, nameof(moveNext));

            if (moveNext.Parameters.Count != 1)
                throw Error.MoveNextShouldHaveSingleParameter();
            if (!AreReferenceAssignable(moveNext.Parameters[0].Type, enumeratorType))
                throw Error.InvalidMoveNextFirstArgType(moveNext.Parameters[0].Type, enumeratorType);
            if (!isAsync && moveNext.ReturnType != typeof(bool))
                throw Error.MoveNextShouldHaveBooleanReturnType();

            //
            // REVIEW: We don't have info about the await operation on MoveNextAsync here, so can't validate the return type.
            //
            //         Note we can't call CSharpExpression.Await to attempt to infer an await operation, because GetAwaiter
            //         could come in through an extension method. We'll have the ForEach factory do the final check.
            //

            RequiresNotNull(current, nameof(current));

            var currentGetMethod = current.GetGetMethod(nonPublic: false);
            if (currentGetMethod == null)
                throw Error.PropertyDoesNotHaveGetAccessor(current);
            if (currentGetMethod.IsStatic)
                throw Error.AccessorCannotBeStatic(current);
            if (current.GetIndexParameters().Length != 0)
                throw Error.PropertyShouldNotBeIndexer(current);

            var currentType = current.PropertyType;

            if (currentType == typeof(void))
                throw Error.PropertyShouldNotReturnVoid(current);

            if (currentConversion != null)
            {
                RequiresCanRead(currentConversion, nameof(currentConversion));

                if (currentConversion.Parameters.Count != 1)
                    throw Error.CurrentConversionShouldHaveSingleParameter();
                if (!AreReferenceAssignable(currentConversion.Parameters[0].Type, currentType))
                    throw Error.InvalidCurrentConversionFirstArgType(currentConversion.Parameters[0].Type, currentType);
                if (!AreReferenceAssignable(elementType, currentConversion.ReturnType))
                    throw Error.InvalidCurrentReturnType(elementType, currentConversion.ReturnType);
            }
            else
            {
                if (!AreReferenceAssignable(elementType, currentType))
                    throw Error.InvalidCurrentReturnType(elementType, currentType);
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

        /// <summary>
        /// Creates a new <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.
        /// </summary>
        /// <param name="isAsync">A Boolean indicating whether the enumeration is asynchronous.</param>
        /// <param name="collectionType">The type of the collection being enumerated.</param>
        /// <returns>A <see cref="Microsoft.CSharp.Expressions.EnumeratorInfo"/> object providing binding information for foreach operations.</returns>
        public static EnumeratorInfo EnumeratorInfo(bool isAsync, Type collectionType)
        {
            RequiresNotNull(collectionType, nameof(collectionType));
            ValidateType(collectionType, nameof(collectionType));

            if (collectionType == typeof(string))
            {
                if (isAsync)
                    throw Error.AsyncEnumerationNotSupportedForString();

                return StringEnumeratorInfo;
            }
            else if (collectionType.IsArray)
            {
                if (isAsync)
                    throw Error.AsyncEnumerationNotSupportedForArray();

                return CreateArrayEnumeratorInfo(collectionType);
            }
            else
            {
                static MethodInfo FindInstanceMethod(Type type, string name, bool allowOptionalParams)
                {
                    var result = default(MethodInfo);

                    foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (!m.IsGenericMethod && m.Name == name)
                        {
                            var parameters = m.GetParametersCached();

                            if (!allowOptionalParams)
                            {
                                if (parameters.Length == 0)
                                {
                                    if (result != null)
                                        throw Error.AmbiguousEnumeratorMethod(type, name);

                                    result = m;
                                }
                            }
                            else
                            {
                                // TODO: Implement betterness rules if there's more than one candidate?
                                //       Add support for params.
                                if (parameters.Length == 0 || parameters.All(p => p.HasDefaultValue))
                                {
                                    if (result != null)
                                        throw Error.AmbiguousEnumeratorMethod(type, name);

                                    result = m;
                                }
                            }
                        }
                    }

                    return result;
                }

                MethodInfo FindGetEnumerator(Type collectionType) => FindInstanceMethod(collectionType, isAsync ? "GetAsyncEnumerator" : "GetEnumerator", allowOptionalParams: isAsync);
                MethodInfo FindMoveNext(Type enumeratorType) => FindInstanceMethod(enumeratorType, isAsync ? "MoveNextAsync" : "MoveNext", allowOptionalParams: isAsync);
                MethodInfo FindDispose(Type enumeratorType) => FindInstanceMethod(enumeratorType, isAsync ? "DisposeAsync" : "Dispose", allowOptionalParams: false);

                var getEnumeratorMethod = FindGetEnumerator(collectionType);
                if (getEnumeratorMethod != null)
                {
                    var collectionParameter = Expression.Parameter(collectionType, "c");

                    var getEnumeratorBody = CSharpExpression.Call(collectionParameter, getEnumeratorMethod, Array.Empty<ParameterAssignment>());
                    var getEnumerator = Expression.Lambda(getEnumeratorBody, collectionParameter);

                    var enumeratorType = getEnumeratorMethod.ReturnType;

                    var enumeratorParameter = Expression.Parameter(enumeratorType, "e");

                    var moveNextMethod = FindMoveNext(enumeratorType);
                    if (moveNextMethod == null)
                        throw Error.EnumeratorShouldHaveMoveNextMethod(enumeratorType);

                    var moveNextReturnType = isAsync
                        ? CSharpExpression.AwaitInfo(moveNextMethod.ReturnType).Type
                        : moveNextMethod.ReturnType;

                    if (moveNextReturnType != typeof(bool))
                        throw Error.MoveNextShouldHaveBooleanReturnType();

                    var moveNextBody = CSharpExpression.Call(enumeratorParameter, moveNextMethod, Array.Empty<ParameterAssignment>());
                    var moveNext = Expression.Lambda(moveNextBody, enumeratorParameter);

                    var currentProperty = enumeratorType.GetProperty(nameof(IEnumerator.Current), BindingFlags.Public | BindingFlags.Instance);
                    if (currentProperty == null || currentProperty.GetGetMethod(nonPublic: false) == null)
                        throw Error.EnumeratorShouldHaveCurrentProperty(enumeratorType);

                    var currentParameter = Expression.Parameter(currentProperty.PropertyType, "x");
                    var currentConversion = Expression.Lambda(currentParameter, currentParameter);

                    var elementType = currentProperty.PropertyType;

                    bool needsDisposal = false;
                    LambdaExpression patternDispose = null;
                    AwaitInfo disposeAwaitInfo = null;

                    bool CanConvertToDisposeInterface()
                    {
                        var disposeInterface = isAsync ? typeof(IAsyncDisposable) : typeof(IDisposable);
                        return enumeratorType.HasReferenceConversionTo(disposeInterface); // REVIEW - Roslyn checks for implicit conversion
                    }

                    if ((!enumeratorType.IsSealed && !isAsync) || CanConvertToDisposeInterface())
                    {
                        needsDisposal = true;

                        if (isAsync)
                        {
                            disposeAwaitInfo = AwaitInfo(typeof(ValueTask<bool>));
                        }
                    }
                    else
                    {
                        var dispose = FindDispose(enumeratorType);

                        if (dispose != null)
                        {
                            needsDisposal = true;
                            patternDispose = Expression.Lambda(Expression.Call(enumeratorParameter, dispose), enumeratorParameter);

                            if (isAsync)
                            {
                                disposeAwaitInfo = AwaitInfo(dispose.ReturnType);
                            }
                        }
                    }

                    return new EnumeratorInfo(
                        isAsync,
                        collectionType,
                        getEnumerator,
                        moveNext,
                        currentProperty,
                        currentConversion,
                        elementType,
                        needsDisposal,
                        disposeAwaitInfo,
                        patternDispose);
                }
                else
                {
                    var enumerableInterface = isAsync ? typeof(IAsyncEnumerable<>) : typeof(IEnumerable<>);

                    // NB: We don't check for implicit conversions to IE<T> or IE; the caller is responsible to insert a convert if needed.
                    //     As such, we limit the checks to checks for implemented interfaces on the collection type.
                    var collectionInterfaceTypes = collectionType.GetInterfaces();

                    var enumerableOfT = default(Type);
                    var enumerable = default(Type);
                    foreach (var ifType in collectionInterfaceTypes)
                    {
                        if (ifType.IsGenericType && ifType.GetGenericTypeDefinition() == enumerableInterface)
                        {
                            if (enumerableOfT != null && ifType != enumerableOfT)
                            {
                                throw Error.MoreThanOneIEnumerableFound(collectionType);
                            }

                            enumerableOfT = ifType;
                        }
                        else if (!isAsync && ifType == typeof(IEnumerable))
                        {
                            enumerable = ifType;
                        }
                    }

                    var enumerableType = enumerableOfT ?? enumerable;

                    if (enumerableType != null)
                    {
                        return CreateEnumerableEnumeratorInfo(enumerableType);
                    }
                }

                throw Error.NoEnumerablePattern(collectionType);
            }
        }

        private static EnumeratorInfo s_stringEnumeratorInfo;
        private static EnumeratorInfo StringEnumeratorInfo => s_stringEnumeratorInfo ??= CreateStringEnumeratorInfo();

        private static EnumeratorInfo CreateStringEnumeratorInfo()
        {
            var getEnumeratorMethod = typeof(string).GetMethod(nameof(string.GetEnumerator));

            // NB: See remarks on array. We'll lower using a For loop but populate info nonetheless.

            var collectionParameter = Expression.Parameter(typeof(string), "s");
            var getEnumeratorBody = Expression.Call(collectionParameter, getEnumeratorMethod);
            var getEnumerator = Expression.Lambda(getEnumeratorBody, collectionParameter);

            var enumeratorType = getEnumeratorBody.Type;
            var enumeratorVariable = Expression.Variable(enumeratorType, "e");

            var moveNextMethod = enumeratorType.GetMethod(nameof(IEnumerator.MoveNext), BindingFlags.Public | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null);
            var moveNext = Expression.Lambda(Expression.Call(enumeratorVariable, moveNextMethod), enumeratorVariable);

            var currentProperty = enumeratorType.GetProperty(nameof(IEnumerator.Current), BindingFlags.Public | BindingFlags.Instance);

            var currentVariable = Expression.Parameter(currentProperty.PropertyType, "c");
            var currentConversion = Expression.Lambda(CreateConvert(currentVariable, typeof(char)), currentVariable);

            return new EnumeratorInfo(
                isAsync: false,
                collectionType: typeof(string),
                getEnumerator,
                moveNext,
                currentProperty,
                currentConversion,
                elementType: typeof(char),
                needsDisposal: true,
                disposeAwaitInfo: null,
                patternDispose: null);
        }

        private static MethodInfo s_IEnumerable_GetEnumerator;
        private static MethodInfo IEnumerable_GetEnumerator => s_IEnumerable_GetEnumerator ??= typeof(IEnumerable).GetMethod(nameof(IEnumerable.GetEnumerator));

        private static MethodInfo s_IEnumerator_MoveNext;
        private static MethodInfo IEnumerator_MoveNext => s_IEnumerator_MoveNext ??= typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
        
        private static PropertyInfo s_IEnumerator_Current;
        private static PropertyInfo IEnumerator_Current => s_IEnumerator_Current ??= typeof(IEnumerator).GetProperty(nameof(IEnumerator.Current));

        private static EnumeratorInfo CreateArrayEnumeratorInfo(Type arrayType)
        {
            var elementType = arrayType.GetElementType();

            // NB: In practice, we won't need some of these because we'll reduce ForEach over arrays to
            //     use For loops instead. However, we'll pretend to go through IEnumerable interfaces
            //     and be consistent with the enumerator info emitted by Roslyn.

            var arrayParameter = Expression.Parameter(arrayType, "arr");
            var getEnumerator = Expression.Lambda(Expression.Call(arrayParameter, IEnumerable_GetEnumerator), arrayParameter);

            var enumeratorVariable = Expression.Parameter(typeof(IEnumerator), "e");
            var moveNext = Expression.Lambda(Expression.Call(enumeratorVariable, IEnumerator_MoveNext), enumeratorVariable);

            var currentVariable = Expression.Parameter(typeof(object), "c");
            var currentConversion = Expression.Lambda(CreateConvert(currentVariable, elementType), currentVariable);

            return new EnumeratorInfo(
                isAsync: false,
                collectionType: arrayType,
                getEnumerator,
                moveNext,
                IEnumerator_Current,
                currentConversion,
                elementType,
                needsDisposal: true,
                disposeAwaitInfo: null,
                patternDispose: null);
        }

        private static EnumeratorInfo s_nonGenericEnumeratorInfo;
        private static EnumeratorInfo NonGenericEnumeratorInfo => s_nonGenericEnumeratorInfo ??= CreateNonGenericEnumeratorInfo();

        private static EnumeratorInfo CreateNonGenericEnumeratorInfo()
        {
            var collectionParameter = Expression.Parameter(typeof(IEnumerable), "c");
            var getEnumeratorBody = Expression.Call(collectionParameter, IEnumerable_GetEnumerator);
            var getEnumerator = Expression.Lambda(getEnumeratorBody, collectionParameter);

            var enumeratorType = getEnumeratorBody.Type;
            var enumeratorVariable = Expression.Variable(enumeratorType, "e");

            var moveNext = Expression.Lambda(Expression.Call(enumeratorVariable, IEnumerator_MoveNext), enumeratorVariable);

            var currentProperty = IEnumerator_Current;

            var currentVariable = Expression.Parameter(typeof(object), "o");
            var currentConversion = Expression.Lambda(currentVariable, currentVariable);

            return new EnumeratorInfo(
                isAsync: false,
                collectionType: typeof(IEnumerable),
                getEnumerator,
                moveNext,
                currentProperty,
                currentConversion,
                elementType: typeof(object),
                needsDisposal: true,
                disposeAwaitInfo: null,
                patternDispose: null);
        }

        private static EnumeratorInfo CreateEnumerableEnumeratorInfo(Type enumerableType)
        {
            if (enumerableType == typeof(IEnumerable))
            {
                return NonGenericEnumeratorInfo;
            }

            var genericEnumerable = enumerableType.GetGenericTypeDefinition();

            var isAsync = genericEnumerable == typeof(IAsyncEnumerable<>);

            var elementType = enumerableType.GetGenericArguments()[0];

            var collectionParameter = Expression.Parameter(enumerableType, "c");

            var getEnumeratorMethod = isAsync
                ? enumerableType.GetNonGenericMethod(nameof(IAsyncEnumerable<object>.GetAsyncEnumerator), BindingFlags.Public | BindingFlags.Instance, new[] { typeof(CancellationToken) })
                : enumerableType.GetNonGenericMethod(nameof(IEnumerable<object>.GetEnumerator), BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);

            var getEnumeratorBody = Expression.Call(collectionParameter, getEnumeratorMethod);
            var getEnumerator = Expression.Lambda(getEnumeratorBody, collectionParameter);

            var enumeratorType = getEnumeratorBody.Type;
            var enumeratorVariable = Expression.Variable(enumeratorType, "e");

            var moveNextMethod = isAsync
                ? enumerableType.GetNonGenericMethod(nameof(IAsyncEnumerator<object>.MoveNextAsync), BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes)
                : IEnumerator_MoveNext;

            var moveNext = Expression.Lambda(Expression.Call(enumeratorVariable, moveNextMethod), enumeratorVariable);

            var currentProperty = enumeratorType.GetProperty(nameof(IEnumerator.Current));

            var currentVariable = Expression.Parameter(elementType, "o");
            var currentConversion = Expression.Lambda(currentVariable, currentVariable);

            var disposeAwaitInfo = isAsync ? AwaitInfo(typeof(ValueTask<bool>)) : null;

            return new EnumeratorInfo(
                isAsync,
                collectionType: enumerableType,
                getEnumerator,
                moveNext,
                currentProperty,
                currentConversion,
                elementType,
                needsDisposal: true,
                disposeAwaitInfo,
                patternDispose: null);
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
