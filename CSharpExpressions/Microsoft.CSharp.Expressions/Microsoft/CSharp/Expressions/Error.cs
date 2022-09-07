// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' is not defined for method '{1}'."
        /// </summary>
        internal static Exception ParameterNotDefinedForMethod(object? p0, object? p1) => new ArgumentException(Strings.ParameterNotDefinedForMethod(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Parameter index '{0}' is out of bounds for method '{1}'."
        /// </summary>
        internal static Exception ParameterIndexOutOfBounds(object? p0, object? p1) => new ArgumentException(Strings.ParameterIndexOutOfBounds(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Expression of type '{0}' cannot be used for parameter of type '{1}'."
        /// </summary>
        internal static Exception ExpressionTypeDoesNotMatchParameter(object? p0, object? p1) => new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' has multiple bindings."
        /// </summary>
        internal static Exception DuplicateParameterBinding(object? p0) => new ArgumentException(Strings.DuplicateParameterBinding(p0));

        /// <summary>
        /// ArgumentException with message like "Non-optional parameter '{0}' has no binding."
        /// </summary>
        internal static Exception UnboundParameter(object? p0) => new ArgumentException(Strings.UnboundParameter(p0));

        /// <summary>
        /// ArgumentException with message like "A non-static constructor is required."
        /// </summary>
        internal static Exception NonStaticConstructorRequired() => new ArgumentException(Strings.NonStaticConstructorRequired);

        /// <summary>
        /// ArgumentException with message like "The property '{0}' has no 'get' accessor."
        /// </summary>
        internal static Exception PropertyDoesNotHaveGetAccessor(object? p0) => new ArgumentException(Strings.PropertyDoesNotHaveGetAccessor(p0));

        /// <summary>
        /// ArgumentException with message like "The property '{0}' has no 'set' accessor."
        /// </summary>
        internal static Exception PropertyDoesNotHaveSetAccessor(object? p0) => new ArgumentException(Strings.PropertyDoesNotHaveSetAccessor(p0));

        /// <summary>
        /// ArgumentException with message like "A non-static 'get' accessor is required for property '{0}'."
        /// </summary>
        internal static Exception AccessorCannotBeStatic(object? p0) => new ArgumentException(Strings.AccessorCannotBeStatic(p0));

        /// <summary>
        /// ArgumentException with message like "The number of indexes specified does not match the array rank."
        /// </summary>
        internal static Exception RankMismatch() => new ArgumentException(Strings.RankMismatch);

        /// <summary>
        /// ArgumentOutOfRangeException with message like "The specified index is out of range."
        /// </summary>
        internal static Exception IndexOutOfRange() => new ArgumentOutOfRangeException(Strings.IndexOutOfRange);

        /// <summary>
        /// ArgumentException with message like "An array dimension cannot be less than 0."
        /// </summary>
        internal static Exception BoundCannotBeLessThanZero() => new ArgumentException(Strings.BoundCannotBeLessThanZero);

        /// <summary>
        /// ArgumentException with message like "The number of elements does not match the length of the array."
        /// </summary>
        internal static Exception ArrayBoundsElementCountMismatch() => new ArgumentException(Strings.ArrayBoundsElementCountMismatch);

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method should take zero parameters."
        /// </summary>
        internal static Exception GetAwaiterShouldTakeZeroParameters() => new ArgumentException(Strings.GetAwaiterShouldTakeZeroParameters);

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method should not be generic."
        /// </summary>
        internal static Exception GetAwaiterShouldNotBeGeneric() => new ArgumentException(Strings.GetAwaiterShouldNotBeGeneric);

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method has an unsupported return type."
        /// </summary>
        internal static Exception GetAwaiterShouldReturnAwaiterType() => new ArgumentException(Strings.GetAwaiterShouldReturnAwaiterType);

        /// <summary>
        /// ArgumentException with message like "Awaitable type '{0}' should have a 'GetAwaiter' method."
        /// </summary>
        internal static Exception AwaitableTypeShouldHaveGetAwaiterMethod(object? p0) => new ArgumentException(Strings.AwaitableTypeShouldHaveGetAwaiterMethod(p0));

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should implement 'INotifyCompletion'."
        /// </summary>
        internal static Exception AwaiterTypeShouldImplementINotifyCompletion(object? p0) => new ArgumentException(Strings.AwaiterTypeShouldImplementINotifyCompletion(p0));

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor."
        /// </summary>
        internal static Exception AwaiterTypeShouldHaveIsCompletedProperty(object? p0) => new ArgumentException(Strings.AwaiterTypeShouldHaveIsCompletedProperty(p0));

        /// <summary>
        /// ArgumentException with message like "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'."
        /// </summary>
        internal static Exception AwaiterIsCompletedShouldReturnBool(object? p0) => new ArgumentException(Strings.AwaiterIsCompletedShouldReturnBool(p0));

        /// <summary>
        /// ArgumentException with message like "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters."
        /// </summary>
        internal static Exception AwaiterIsCompletedShouldNotBeIndexer(object? p0) => new ArgumentException(Strings.AwaiterIsCompletedShouldNotBeIndexer(p0));

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should have a 'GetResult' method."
        /// </summary>
        internal static Exception AwaiterTypeShouldHaveGetResultMethod(object? p0) => new ArgumentException(Strings.AwaiterTypeShouldHaveGetResultMethod(p0));

        /// <summary>
        /// ArgumentException with message like "The 'GetResult' method on awaiter type '{0}' has an unsupported return type."
        /// </summary>
        internal static Exception AwaiterGetResultTypeInvalid(object? p0) => new ArgumentException(Strings.AwaiterGetResultTypeInvalid(p0));

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have a 'GetAwaiter' expression."
        /// </summary>
        internal static Exception DynamicAwaitNoGetAwaiter() => new ArgumentException(Strings.DynamicAwaitNoGetAwaiter);

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have an 'IsCompleted' property."
        /// </summary>
        internal static Exception DynamicAwaitNoIsCompleted() => new ArgumentException(Strings.DynamicAwaitNoIsCompleted);

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have a 'GetResult' method."
        /// </summary>
        internal static Exception DynamicAwaitNoGetResult() => new ArgumentException(Strings.DynamicAwaitNoGetResult);

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' expression should have one parameter."
        /// </summary>
        internal static Exception GetAwaiterExpressionOneParameter() => new ArgumentException(Strings.GetAwaiterExpressionOneParameter);

        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions."
        /// </summary>
        internal static Exception AsyncLambdaCantHaveByRefParameter(object? p0) => new ArgumentException(Strings.AsyncLambdaCantHaveByRefParameter(p0));

        /// <summary>
        /// ArgumentException with message like "Return type '{0}' is not valid for an asynchronous lambda expression."
        /// </summary>
        internal static Exception AsyncLambdaInvalidReturnType(object? p0) => new ArgumentException(Strings.AsyncLambdaInvalidReturnType(p0));

        /// <summary>
        /// InvalidOperationException with message like "Await expression cannot occur in '{0}'."
        /// </summary>
        internal static Exception AwaitForbiddenHere(object? p0) => new InvalidOperationException(Strings.AwaitForbiddenHere(p0));

        /// <summary>
        /// ArgumentException with message like "An expression of type '{0}' can't be used as a lock."
        /// </summary>
        internal static Exception LockNeedsReferenceType(object? p0) => new ArgumentException(Strings.LockNeedsReferenceType(p0));

        /// <summary>
        /// ArgumentException with message like "The conversion lambda should have one parameter."
        /// </summary>
        internal static Exception ConversionNeedsOneParameter() => new ArgumentException(Strings.ConversionNeedsOneParameter);

        /// <summary>
        /// ArgumentException with message like "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'."
        /// </summary>
        internal static Exception ConversionInvalidArgument(object? p0, object? p1) => new ArgumentException(Strings.ConversionInvalidArgument(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'."
        /// </summary>
        internal static Exception ConversionInvalidResult(object? p0, object? p1) => new ArgumentException(Strings.ConversionInvalidResult(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor."
        /// </summary>
        internal static Exception EnumeratorShouldHaveCurrentProperty(object? p0) => new ArgumentException(Strings.EnumeratorShouldHaveCurrentProperty(p0));

        /// <summary>
        /// ArgumentException with message like "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type."
        /// </summary>
        internal static Exception EnumeratorShouldHaveMoveNextMethod(object? p0) => new ArgumentException(Strings.EnumeratorShouldHaveMoveNextMethod(p0));

        /// <summary>
        /// ArgumentException with message like "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'."
        /// </summary>
        internal static Exception MoreThanOneIEnumerableFound(object? p0) => new ArgumentException(Strings.MoreThanOneIEnumerableFound(p0));

        /// <summary>
        /// ArgumentException with message like "Collection type '{0}' has no valid enumerable pattern."
        /// </summary>
        internal static Exception NoEnumerablePattern(object? p0) => new ArgumentException(Strings.NoEnumerablePattern(p0));

        /// <summary>
        /// ArgumentException with message like "Initializers should be assignments to variables."
        /// </summary>
        internal static Exception InvalidInitializer() => new ArgumentException(Strings.InvalidInitializer);

        /// <summary>
        /// ArgumentException with message like "Break and continue lables should be different."
        /// </summary>
        internal static Exception DuplicateLabels() => new ArgumentException(Strings.DuplicateLabels);

        /// <summary>
        /// ArgumentException with message like "Conditional access expressions require non-static members or extension methods."
        /// </summary>
        internal static Exception ConditionalAccessRequiresNonStaticMember() => new ArgumentException(Strings.ConditionalAccessRequiresNonStaticMember);

        /// <summary>
        /// ArgumentException with message like "Conditional access expressions require readable properties."
        /// </summary>
        internal static Exception ConditionalAccessRequiresReadableProperty() => new ArgumentException(Strings.ConditionalAccessRequiresReadableProperty);

        /// <summary>
        /// ArgumentException with message like "Too many arguments have been specified."
        /// </summary>
        internal static Exception TooManyArguments() => new ArgumentException(Strings.TooManyArguments);

        /// <summary>
        /// ArgumentException with message like "Conditional call expressions for extensions methods should specify an instance expression."
        /// </summary>
        internal static Exception ExtensionMethodRequiresInstance() => new ArgumentException(Strings.ExtensionMethodRequiresInstance);

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid governing type for a switch statement."
        /// </summary>
        internal static Exception InvalidSwitchType(object? p0) => new ArgumentException(Strings.InvalidSwitchType(p0));

        /// <summary>
        /// ArgumentException with message like "The test value '{0}' occurs more than once."
        /// </summary>
        internal static Exception DuplicateTestValue(object? p0) => new ArgumentException(Strings.DuplicateTestValue(p0));

        /// <summary>
        /// ArgumentException with message like "A 'null' test value cannot be used in a switch statement with governing type '{0}'."
        /// </summary>
        internal static Exception SwitchCantHaveNullCase(object? p0) => new ArgumentException(Strings.SwitchCantHaveNullCase(p0));

        /// <summary>
        /// ArgumentException with message like "A test value with type '{0}' cannot be used in a swich statement with governing type '{1}'."
        /// </summary>
        internal static Exception SwitchCaseHasIncompatibleType(object? p0, object? p1) => new ArgumentException(Strings.SwitchCaseHasIncompatibleType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "All specified test values should have the same type."
        /// </summary>
        internal static Exception TestValuesShouldHaveConsistentType() => new ArgumentException(Strings.TestValuesShouldHaveConsistentType);

        /// <summary>
        /// ArgumentException with message like "The break label of a switch statement should be of type 'void'."
        /// </summary>
        internal static Exception SwitchBreakLabelShouldBeVoid() => new ArgumentException(Strings.SwitchBreakLabelShouldBeVoid);

        /// <summary>
        /// ArgumentException with message like "The label of a switch case should be of type 'void'."
        /// </summary>
        internal static Exception SwitchLabelTargetShouldBeVoid() => new ArgumentException(Strings.SwitchLabelTargetShouldBeVoid);

        /// <summary>
        /// InvalidOperationException with message like "A 'goto case {0}' statement was found but the containing switch statement has no such label."
        /// </summary>
        internal static Exception InvalidGotoCase(object? p0) => new InvalidOperationException(Strings.InvalidGotoCase(p0));

        /// <summary>
        /// InvalidOperationException with message like "A 'goto default' statement was found but the containing switch statement has no default label."
        /// </summary>
        internal static Exception InvalidGotoDefault() => new InvalidOperationException(Strings.InvalidGotoDefault);

        /// <summary>
        /// InvalidOperationException with message like "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node."
        /// </summary>
        internal static Exception GotoCanOnlyBeReducedInSwitch() => new InvalidOperationException(Strings.GotoCanOnlyBeReducedInSwitch);

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for a conditional receiver."
        /// </summary>
        internal static Exception InvalidConditionalReceiverType(object? p0) => new ArgumentException(Strings.InvalidConditionalReceiverType(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for a receiver in a conditional access expression."
        /// </summary>
        internal static Exception InvalidConditionalReceiverExpressionType(object? p0) => new ArgumentException(Strings.InvalidConditionalReceiverExpressionType(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver."
        /// </summary>
        internal static Exception ConditionalReceiverTypeMismatch(object? p0, object? p1) => new ArgumentException(Strings.ConditionalReceiverTypeMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation '{0}' is not supported for type '{1}'."
        /// </summary>
        internal static Exception InvalidCompoundAssignment(object? p0, object? p1) => new ArgumentException(Strings.InvalidCompoundAssignment(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'."
        /// </summary>
        internal static Exception InvalidCompoundAssignmentWithOperands(object? p0, object? p1, object? p2) => new ArgumentException(Strings.InvalidCompoundAssignmentWithOperands(p0, p1, p2));

        /// <summary>
        /// ArgumentException with message like "Unary assignment operation '{0}' is not supported for an operand of type '{1}'."
        /// </summary>
        internal static Exception InvalidUnaryAssignmentWithOperands(object? p0, object? p1) => new ArgumentException(Strings.InvalidUnaryAssignmentWithOperands(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Type must not be ByRef."
        /// </summary>
        internal static Exception TypeMustNotBeByRef() => new ArgumentException(Strings.TypeMustNotBeByRef);

        /// <summary>
        /// ArgumentException with message like "Type must not be a pointer type."
        /// </summary>
        internal static Exception TypeMustNotBePointer() => new ArgumentException(Strings.TypeMustNotBePointer);

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations."
        /// </summary>
        internal static Exception InvalidNullCoalescingAssignmentArguments() => new ArgumentException(Strings.InvalidNullCoalescingAssignmentArguments);

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable."
        /// </summary>
        internal static Exception InvalidInterpolatedStringType(object? p0) => new ArgumentException(Strings.InvalidInterpolatedStringType(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for an index operand. Supported types are int or int?."
        /// </summary>
        internal static Exception InvalidFromEndIndexOperandType(object? p0) => new ArgumentException(Strings.InvalidFromEndIndexOperandType(p0));

        /// <summary>
        /// ArgumentException with message like "The specified method is not valid to construct an object of type Index."
        /// </summary>
        internal static Exception InvalidFromEndIndexMethod() => new ArgumentException(Strings.InvalidFromEndIndexMethod);

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid index type. Supported types are Index or Index?."
        /// </summary>
        internal static Exception InvalidIndexType(object? p0) => new ArgumentException(Strings.InvalidIndexType(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for a range operand. Supported types are Index or Index?."
        /// </summary>
        internal static Exception InvalidRangeOperandType(object? p0) => new ArgumentException(Strings.InvalidRangeOperandType(p0));

        /// <summary>
        /// ArgumentException with message like "The specified method is not valid to construct an object of type Range."
        /// </summary>
        internal static Exception InvalidRangeMethod() => new ArgumentException(Strings.InvalidRangeMethod);

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid range type. Supported types are Range or Range?."
        /// </summary>
        internal static Exception InvalidRangeType(object? p0) => new ArgumentException(Strings.InvalidRangeType(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for an 'IndexerAccess' argument. Supported types are Index or Range."
        /// </summary>
        internal static Exception InvalidIndexerAccessArgumentType(object? p0) => new ArgumentException(Strings.InvalidIndexerAccessArgumentType(p0));

        /// <summary>
        /// ArgumentException with message like "Property '{0}' should be of type int."
        /// </summary>
        internal static Exception InvalidLengthOrCountPropertyType(object? p0) => new ArgumentException(Strings.InvalidLengthOrCountPropertyType(p0));

        /// <summary>
        /// ArgumentException with message like "Member '{0}' is not a valid member for an indexer. Supported member types are MethodInfo or PropertyInfo."
        /// </summary>
        internal static Exception InvalidIndexMember(object? p0) => new ArgumentException(Strings.InvalidIndexMember(p0));

        /// <summary>
        /// ArgumentException with message like "Member '{0}' is not a valid member for a slice method."
        /// </summary>
        internal static Exception InvalidSliceMember(object? p0) => new ArgumentException(Strings.InvalidSliceMember(p0));

        /// <summary>
        /// ArgumentException with message like "Indexer '{0}' does not have an 'int' parameter type."
        /// </summary>
        internal static Exception InvalidIndexerParameterType(object? p0) => new ArgumentException(Strings.InvalidIndexerParameterType(p0));

        /// <summary>
        /// ArgumentException with message like "Slice method '{0}' should be an instance method."
        /// </summary>
        internal static Exception SliceMethodMustNotBeStatic(object? p0) => new ArgumentException(Strings.SliceMethodMustNotBeStatic(p0));

        /// <summary>
        /// ArgumentException with message like "Slice method '{0}' should be have exactly two parameters of type 'int'."
        /// </summary>
        internal static Exception InvalidSliceParameters(object? p0) => new ArgumentException(Strings.InvalidSliceParameters(p0));

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid tuple type."
        /// </summary>
        internal static Exception InvalidTupleType(object? p0) => new ArgumentException(Strings.InvalidTupleType(p0));

        /// <summary>
        /// ArgumentException with message like "The number of arguments does not match the number of components of tuple type '{0}'."
        /// </summary>
        internal static Exception InvalidTupleArgumentCount(object? p0) => new ArgumentException(Strings.InvalidTupleArgumentCount(p0));

        /// <summary>
        /// ArgumentException with message like "The number of argument names does not match the number of components of tuple type '{0}'."
        /// </summary>
        internal static Exception InvalidTupleArgumentNamesCount(object? p0) => new ArgumentException(Strings.InvalidTupleArgumentNamesCount(p0));

        /// <summary>
        /// ArgumentException with message like "The type of a tuple component cannot be void."
        /// </summary>
        internal static Exception TupleComponentCannotBeVoid() => new ArgumentException(Strings.TupleComponentCannotBeVoid);

        /// <summary>
        /// ArgumentException with message like "The arity of tuple type '{0}' does not match the arity of tuple type '{1}'."
        /// </summary>
        internal static Exception TupleComponentCountMismatch(object? p0, object? p1) => new ArgumentException(Strings.TupleComponentCountMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The number of element conversion expressions does not match tuple arity '{0}'."
        /// </summary>
        internal static Exception InvalidElementConversionCount(object? p0) => new ArgumentException(Strings.InvalidElementConversionCount(p0));

        /// <summary>
        /// ArgumentException with message like "The number of equality check expressions does not match tuple arity '{0}'."
        /// </summary>
        internal static Exception InvalidEqualityCheckCount(object? p0) => new ArgumentException(Strings.InvalidEqualityCheckCount(p0));

        /// <summary>
        /// ArgumentException with message like "'{0}' is not a member of any type."
        /// </summary>
        internal static Exception NotAMemberOfAnyType(object? p0) => new ArgumentException(Strings.NotAMemberOfAnyType(p0));

        /// <summary>
        /// ArgumentException with message like "A 'with' expression for value type '{0}' cannot specify a 'Clone' method."
        /// </summary>
        internal static Exception WithExpressionCannotHaveCloneForValueType(object? p0) => new ArgumentException(Strings.WithExpressionCannotHaveCloneForValueType(p0));

        /// <summary>
        /// ArgumentException with message like "A 'with' expression for type '{0}' should specify a 'Clone' method."
        /// </summary>
        internal static Exception WithExpressionShouldHaveClone(object? p0) => new ArgumentException(Strings.WithExpressionShouldHaveClone(p0));

        /// <summary>
        /// ArgumentException with message like "Clone method '{0}' should have no parameters."
        /// </summary>
        internal static Exception CloneMethodShouldHaveNoParameters(object? p0) => new ArgumentException(Strings.CloneMethodShouldHaveNoParameters(p0));

        /// <summary>
        /// ArgumentException with message like "Clone method '{0}' should be an instance method."
        /// </summary>
        internal static Exception CloneMethodMustNotBeStatic(object? p0) => new ArgumentException(Strings.CloneMethodMustNotBeStatic(p0));

        /// <summary>
        /// ArgumentException with message like "Clone method '{0}' should return a type that can be converted to '{1}'."
        /// </summary>
        internal static Exception CloneMethodShouldReturnCompatibleType(object? p0, object? p1) => new ArgumentException(Strings.CloneMethodShouldReturnCompatibleType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Member '{0}' used in a 'MemberInitializer' cannot be static."
        /// </summary>
        internal static Exception MemberInitializerMemberMustNotBeStatic(object? p0) => new ArgumentException(Strings.MemberInitializerMemberMustNotBeStatic(p0));

        /// <summary>
        /// ArgumentException with message like "Member '{0}' used in a 'MemberInitializer' cannot be an indexer."
        /// </summary>
        internal static Exception MemberInitializerMemberMustNotBeIndexer(object? p0) => new ArgumentException(Strings.MemberInitializerMemberMustNotBeIndexer(p0));

        /// <summary>
        /// ArgumentException with message like "Member '{0}' used in a 'MemberInitializer' must be writeable."
        /// </summary>
        internal static Exception MemberInitializerMemberMustBeWriteable(object? p0) => new ArgumentException(Strings.MemberInitializerMemberMustBeWriteable(p0));

        /// <summary>
        /// ArgumentException with message like "No suitable constructor found for type '{0}' using the specified members."
        /// </summary>
        internal static Exception NoAnonymousTypeConstructorFound(object? p0) => new ArgumentException(Strings.NoAnonymousTypeConstructorFound(p0));

        /// <summary>
        /// ArgumentException with message like "A pattern can never produce a value of a nullable type."
        /// </summary>
        internal static Exception CannotHaveNullablePatternType() => new ArgumentException(Strings.CannotHaveNullablePatternType);

        /// <summary>
        /// ArgumentException with message like "The input and narrowed type for a pattern of type '{0}' should be equal."
        /// </summary>
        internal static Exception PatternInputAndNarrowedTypeShouldMatch(object? p0) => new ArgumentException(Strings.PatternInputAndNarrowedTypeShouldMatch(p0));

        /// <summary>
        /// ArgumentException with message like "The variable type '{0}' should be equal to the pattern result type '{1}'."
        /// </summary>
        internal static Exception CannotAssignPatternResultToVariable(object? p0, object? p1) => new ArgumentException(Strings.CannotAssignPatternResultToVariable(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The pattern type '{0}' is not compatible with a subpattern type '{1}'."
        /// </summary>
        internal static Exception PatternTypeMismatch(object? p0, object? p1) => new ArgumentException(Strings.PatternTypeMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The pattern type '{0}' is not a valid binary pattern type."
        /// </summary>
        internal static Exception InvalidBinaryPatternType(object? p0) => new ArgumentException(Strings.InvalidBinaryPatternType(p0));

        /// <summary>
        /// ArgumentException with message like "The pattern type '{0}' is not a valid relational pattern type."
        /// </summary>
        internal static Exception InvalidRelationalPatternType(object? p0) => new ArgumentException(Strings.InvalidRelationalPatternType(p0));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' cannot be used for a constant in a pattern."
        /// </summary>
        internal static Exception InvalidPatternConstantType(object? p0) => new ArgumentException(Strings.InvalidPatternConstantType(p0));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' cannot be used for a constant in a relational pattern."
        /// </summary>
        internal static Exception InvalidRelationalPatternConstantType(object? p0) => new ArgumentException(Strings.InvalidRelationalPatternConstantType(p0));

        /// <summary>
        /// ArgumentException with message like "The value NaN cannot be used for a constant in a relational pattern."
        /// </summary>
        internal static Exception CannotUsePatternConstantNaN() => new ArgumentException(Strings.CannotUsePatternConstantNaN);

        /// <summary>
        /// ArgumentException with message like "A null pattern should use a constant of type object."
        /// </summary>
        internal static Exception NullValueShouldUseObjectType() => new ArgumentException(Strings.NullValueShouldUseObjectType);

        /// <summary>
        /// ArgumentException with message like "A null value cannot be used in a relational pattern."
        /// </summary>
        internal static Exception CannotUseNullValueInRelationalPattern() => new ArgumentException(Strings.CannotUseNullValueInRelationalPattern);

        /// <summary>
        /// ArgumentException with message like "The 'GetLengthMethod' of an 'ITuple' pattern should return an integer value."
        /// </summary>
        internal static Exception ITupleGetLengthShouldReturnInt32() => new ArgumentException(Strings.ITupleGetLengthShouldReturnInt32);

        /// <summary>
        /// ArgumentException with message like "The 'GetItemMethod' of an 'ITuple' pattern should return an object of type Object."
        /// </summary>
        internal static Exception ITupleGetItemShouldReturnObject() => new ArgumentException(Strings.ITupleGetItemShouldReturnObject);

        /// <summary>
        /// ArgumentException with message like "The 'ITuple' positional subpattern with index '{0}' cannot have a field specified."
        /// </summary>
        internal static Exception ITuplePositionalPatternCannotHaveField(object? p0) => new ArgumentException(Strings.ITuplePositionalPatternCannotHaveField(p0));

        /// <summary>
        /// ArgumentException with message like "The 'ITuple' positional subpattern with index '{0}' cannot have a parameter specified."
        /// </summary>
        internal static Exception ITuplePositionalPatternCannotHaveParameter(object? p0) => new ArgumentException(Strings.ITuplePositionalPatternCannotHaveParameter(p0));

        /// <summary>
        /// ArgumentException with message like "The 'ITuple' positional subpattern with index '{0}' has type '{1}'. Only type Object is supported."
        /// </summary>
        internal static Exception ITuplePositionalPatternInvalidInputType(object? p0, object? p1) => new ArgumentException(Strings.ITuplePositionalPatternInvalidInputType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "A positional pattern should either be applied to a tuple type or provide a Deconstruct method."
        /// </summary>
        internal static Exception InvalidPositionalPattern() => new ArgumentException(Strings.InvalidPositionalPattern);

        /// <summary>
        /// ArgumentException with message like "The number of positional subpatterns does not match the number of components in input type '{0}'."
        /// </summary>
        internal static Exception InvalidPositionalPatternCount(object? p0) => new ArgumentException(Strings.InvalidPositionalPatternCount(p0));

        /// <summary>
        /// ArgumentException with message like "Deconstruct method '{0}' should return void."
        /// </summary>
        internal static Exception DeconstructShouldReturnVoid(object? p0) => new ArgumentException(Strings.DeconstructShouldReturnVoid(p0));

        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' on Deconstruct method '{1}' should be an out parameter."
        /// </summary>
        internal static Exception DeconstructParameterShouldBeOut(object? p0, object? p1) => new ArgumentException(Strings.DeconstructParameterShouldBeOut(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Deconstruct method '{0}' should have at least one parameter."
        /// </summary>
        internal static Exception DeconstructExtensionMethodMissingThis(object? p0) => new ArgumentException(Strings.DeconstructExtensionMethodMissingThis(p0));

        /// <summary>
        /// ArgumentException with message like "A tuple field index must be positive."
        /// </summary>
        internal static Exception TupleFieldIndexMustBePositive() => new ArgumentException(Strings.TupleFieldIndexMustBePositive);

        /// <summary>
        /// ArgumentException with message like "The '{0}' parameter must be declared on a method used for deconstruction."
        /// </summary>
        internal static Exception PositionalPatternParameterMustBeOnMethod(object? p0) => new ArgumentException(Strings.PositionalPatternParameterMustBeOnMethod(p0));

        /// <summary>
        /// ArgumentException with message like "The '{0}' parameter must be an out parameter used for deconstruction."
        /// </summary>
        internal static Exception PositionalPatternParameterMustBeOut(object? p0) => new ArgumentException(Strings.PositionalPatternParameterMustBeOut(p0));

        /// <summary>
        /// ArgumentException with message like "The property pattern member '{0}' should not be static."
        /// </summary>
        internal static Exception PropertyPatternMemberShouldNotBeStatic(object? p0) => new ArgumentException(Strings.PropertyPatternMemberShouldNotBeStatic(p0));

        /// <summary>
        /// ArgumentException with message like "The property pattern member '{0}' should be readable."
        /// </summary>
        internal static Exception PropertyPatternMemberShouldBeReadable(object? p0) => new ArgumentException(Strings.PropertyPatternMemberShouldBeReadable(p0));

        /// <summary>
        /// ArgumentException with message like "The property pattern member '{0}' should not be an indexer property."
        /// </summary>
        internal static Exception PropertyPatternMemberShouldNotBeIndexer(object? p0) => new ArgumentException(Strings.PropertyPatternMemberShouldNotBeIndexer(p0));

        /// <summary>
        /// ArgumentException with message like "The property pattern member '{0}' is not compatible with a receiver of type '{1}'."
        /// </summary>
        internal static Exception PropertyPatternMemberIsNotCompatibleWithReceiver(object? p0, object? p1) => new ArgumentException(Strings.PropertyPatternMemberIsNotCompatibleWithReceiver(p0, p1));

        /// <summary>
        /// ArgumentException with message like "A positional pattern using a Deconstruct method cannot specify a tuple field."
        /// </summary>
        internal static Exception PositionalPatternWithDeconstructMethodCannotSpecifyField() => new ArgumentException(Strings.PositionalPatternWithDeconstructMethodCannotSpecifyField);

        /// <summary>
        /// ArgumentException with message like "The '{0}' parameter is not declared on the '{1}' method."
        /// </summary>
        internal static Exception PositionalPatternParameterIsNotDeclaredOnDeconstructMethod(object? p0, object? p1) => new ArgumentException(Strings.PositionalPatternParameterIsNotDeclaredOnDeconstructMethod(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The '{0}' parameter is used more than once."
        /// </summary>
        internal static Exception PositionalPatternParameterShouldOnlyBeUsedOnce(object? p0) => new ArgumentException(Strings.PositionalPatternParameterShouldOnlyBeUsedOnce(p0));

        /// <summary>
        /// ArgumentException with message like "Either all or none of the Deconstruct method parameters should be specified."
        /// </summary>
        internal static Exception PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters() => new ArgumentException(Strings.PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters);

        /// <summary>
        /// ArgumentException with message like "A positional pattern for a tuple type cannot specify a Deconstruct method parameter."
        /// </summary>
        internal static Exception PositionalPatternWithTupleCannotSpecifyParameter() => new ArgumentException(Strings.PositionalPatternWithTupleCannotSpecifyParameter);

        /// <summary>
        /// ArgumentException with message like "The tuple field index '{0}' is out of range for a tuple of cardinality '{1}'."
        /// </summary>
        internal static Exception PositionalPatternTupleIndexOutOfRange(object? p0, object? p1) => new ArgumentException(Strings.PositionalPatternTupleIndexOutOfRange(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The tuple field index '{0}' is used more than once."
        /// </summary>
        internal static Exception PositionalPatternTupleIndexShouldOnlyBeUsedOnce(object? p0) => new ArgumentException(Strings.PositionalPatternTupleIndexShouldOnlyBeUsedOnce(p0));

        /// <summary>
        /// ArgumentException with message like "Either all or none of the tuple fields should be specified."
        /// </summary>
        internal static Exception PositionalPatternWithTupleShouldSpecifyAllIndices() => new ArgumentException(Strings.PositionalPatternWithTupleShouldSpecifyAllIndices);

        /// <summary>
        /// ArgumentException with message like "The type of a switch expression should not be void."
        /// </summary>
        internal static Exception SwitchExpressionTypeShouldNotBeVoid() => new ArgumentException(Strings.SwitchExpressionTypeShouldNotBeVoid);

        /// <summary>
        /// ArgumentException with message like "The switch expression arm at index '{0}' has a pattern input type '{1}' which is not compatible with the switch expression input type '{2}'."
        /// </summary>
        internal static Exception SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput(object? p0, object? p1, object? p2) => new ArgumentException(Strings.SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput(p0, p1, p2));

        /// <summary>
        /// ArgumentException with message like "The switch expression arm at index '{0}' has a value of type '{1}' which is not compatible with the switch expression result type '{2}'."
        /// </summary>
        internal static Exception SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult(object? p0, object? p1, object? p2) => new ArgumentException(Strings.SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult(p0, p1, p2));

        /// <summary>
        /// ArgumentException with message like "The type of a when clause should be Boolean."
        /// </summary>
        internal static Exception WhenClauseShouldBeBoolean() => new ArgumentException(Strings.WhenClauseShouldBeBoolean);

        /// <summary>
        /// ArgumentException with message like "The value of a switch expression arm should not be void."
        /// </summary>
        internal static Exception SwitchExpressionArmValueShouldNotBeVoid() => new ArgumentException(Strings.SwitchExpressionArmValueShouldNotBeVoid);

        /// <summary>
        /// ArgumentException with message like "A conversion cannot return void."
        /// </summary>
        internal static Exception ConversionCannotReturnVoid() => new ArgumentException(Strings.ConversionCannotReturnVoid);

        /// <summary>
        /// ArgumentException with message like "A conversion lambda expression should have a single parameter."
        /// </summary>
        internal static Exception ConversionShouldHaveOneParameter() => new ArgumentException(Strings.ConversionShouldHaveOneParameter);

        /// <summary>
        /// ArgumentException with message like "A deconstruction lambda expression should return void."
        /// </summary>
        internal static Exception DeconstructionShouldReturnVoid() => new ArgumentException(Strings.DeconstructionShouldReturnVoid);

        /// <summary>
        /// ArgumentException with message like "A deconstruction lambda expression should have at least three parameters, i.e. one for the input, and at least two for components returned by the deconstruction."
        /// </summary>
        internal static Exception DeconstructionShouldHaveThreeOrMoreParameters() => new ArgumentException(Strings.DeconstructionShouldHaveThreeOrMoreParameters);

        /// <summary>
        /// ArgumentException with message like "The deconstruction lambda expression parameter at position '{0}' represents an output of the deconstruction and should be passed by reference."
        /// </summary>
        internal static Exception DeconstructionParameterShouldBeByRef(object? p0) => new ArgumentException(Strings.DeconstructionParameterShouldBeByRef(p0));

        /// <summary>
        /// ArgumentException with message like "The number of deconstruction output parameter should match the number of elements conversions."
        /// </summary>
        internal static Exception DeconstructionParameterCountShouldMatchConversionCount() => new ArgumentException(Strings.DeconstructionParameterCountShouldMatchConversionCount);

        /// <summary>
        /// ArgumentException with message like "The deconstruction output parameter at index '{0}' of type '{1}' is not assignable to the corresponding element conversion's input type '{2}'."
        /// </summary>
        internal static Exception DeconstructionParameterNotAssignableToConversion(object? p0, object? p1, object? p2) => new ArgumentException(Strings.DeconstructionParameterNotAssignableToConversion(p0, p1, p2));

        /// <summary>
        /// ArgumentException with message like "The left hand side and the deconstructing conversion of the assignment do not match structurally at depth '{0}' and component '{1}'."
        /// </summary>
        internal static Exception DeconstructingAssignmentStructureMismatch(object? p0, object? p1) => new ArgumentException(Strings.DeconstructingAssignmentStructureMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The computed result tuple type '{0}' does not match the specified expression type '{1}'."
        /// </summary>
        internal static Exception DeconstructingAssignmentTypeMismatch(object? p0, object? p1) => new ArgumentException(Strings.DeconstructingAssignmentTypeMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The left hand side of type '{0}' and the right hand side of type '{1}' are not assignment compatible in the deconstruction assignment at depth '{2}' and component '{3}'."
        /// </summary>
        internal static Exception DeconstructingComponentAndConversionIncompatible(object? p0, object? p1, object? p2, object? p3) => new ArgumentException(Strings.DeconstructingComponentAndConversionIncompatible(p0, p1, p2, p3));

        /// <summary>
        /// ArgumentException with message like "A using statement should either have a single expression or a declaration list."
        /// </summary>
        internal static Exception InvalidUsingStatement() => new ArgumentException(Strings.InvalidUsingStatement);

        /// <summary>
        /// ArgumentException with message like "All variables declared in a using statement should have the same type."
        /// </summary>
        internal static Exception UsingVariableDeclarationsShouldBeConsistentlyTyped() => new ArgumentException(Strings.UsingVariableDeclarationsShouldBeConsistentlyTyped);

        /// <summary>
        /// ArgumentException with message like "The variable '{0}' specified in the local declaration should be explicitly included in the variables of the using statement."
        /// </summary>
        internal static Exception UsingVariableNotInScope(object? p0) => new ArgumentException(Strings.UsingVariableNotInScope(p0));

        /// <summary>
        /// ArgumentException with message like "The Dispose method of a using statement should return void."
        /// </summary>
        internal static Exception UsingDisposeShouldReturnVoid() => new ArgumentException(Strings.UsingDisposeShouldReturnVoid);

        /// <summary>
        /// ArgumentException with message like "A pattern dispose lambda for a using statement should have one parameter."
        /// </summary>
        internal static Exception UsingPatternDisposeShouldHaveOneParameter() => new ArgumentException(Strings.UsingPatternDisposeShouldHaveOneParameter);

        /// <summary>
        /// ArgumentException with message like "The input type '{0}' of the pattern dispose lambda is not compatible with the resource type '{1}' of the using statement."
        /// </summary>
        internal static Exception UsingPatternDisposeInputNotCompatibleWithResource(object? p0, object? p1) => new ArgumentException(Strings.UsingPatternDisposeInputNotCompatibleWithResource(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The variable '{0}' specified in the catch block should be explicitly included in the variables of the catch block."
        /// </summary>
        internal static Exception CatchVariableNotInScope(object? p0) => new ArgumentException(Strings.CatchVariableNotInScope(p0));

        /// <summary>
        /// ArgumentException with message like "The catch block exception type '{0}' is not equivalent to the variable type '{1}'."
        /// </summary>
        internal static Exception CatchTypeNotEquivalentWithVariableType(object? p0, object? p1) => new ArgumentException(Strings.CatchTypeNotEquivalentWithVariableType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The label '{0}' is used in multiple switch sections."
        /// </summary>
        internal static Exception DuplicateLabelInSwitchStatement(object? p0) => new ArgumentException(Strings.DuplicateLabelInSwitchStatement(p0));

        /// <summary>
        /// ArgumentException with message like "The label '{0}' is used more than once in the switch section."
        /// </summary>
        internal static Exception DuplicateLabelInSwitchSection(object? p0) => new ArgumentException(Strings.DuplicateLabelInSwitchSection(p0));

        /// <summary>
        /// ArgumentException with message like "The pattern input type '{0}' is not compatible with the switch value type '{1}'."
        /// </summary>
        internal static Exception SwitchValueTypeDoesNotMatchPatternInputType(object? p0, object? p1) => new ArgumentException(Strings.SwitchValueTypeDoesNotMatchPatternInputType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The pattern input type '{0}' is not consistent with other pattern input types '{1}' in the same switch section."
        /// </summary>
        internal static Exception InconsistentPatternInputType(object? p0, object? p1) => new ArgumentException(Strings.InconsistentPatternInputType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "A switch statement should contain at most one default case."
        /// </summary>
        internal static Exception FoundMoreThanOneDefaultLabel() => new ArgumentException(Strings.FoundMoreThanOneDefaultLabel);

        /// <summary>
        /// ArgumentException with message like "A string interpolation format string cannot be empty."
        /// </summary>
        internal static Exception EmptyFormatSpecifier() => new ArgumentException(Strings.EmptyFormatSpecifier);

        /// <summary>
        /// ArgumentException with message like "A list pattern can have at most one slice subpattern."
        /// </summary>
        internal static Exception MoreThanOneSlicePattern() => new ArgumentException(Strings.MoreThanOneSlicePattern);

        /// <summary>
        /// ArgumentException with message like "The type returned by the index access expression cannot be void."
        /// </summary>
        internal static Exception ElementTypeCannotBeVoid() => new ArgumentException(Strings.ElementTypeCannotBeVoid);

        /// <summary>
        /// ArgumentException with message like "The length access lambda expression should have a single parameter."
        /// </summary>
        internal static Exception LengthAccessShouldHaveOneParameter() => new ArgumentException(Strings.LengthAccessShouldHaveOneParameter);

        /// <summary>
        /// ArgumentException with message like "The length access lambda expression should have an 'Int32' return type."
        /// </summary>
        internal static Exception LengthAccessShouldReturnInt32() => new ArgumentException(Strings.LengthAccessShouldReturnInt32);

        /// <summary>
        /// ArgumentException with message like "The indexer access lambda expression should have two parameters."
        /// </summary>
        internal static Exception IndexerAccessShouldHaveTwoParameters() => new ArgumentException(Strings.IndexerAccessShouldHaveTwoParameters);

        /// <summary>
        /// ArgumentException with message like "The parameter of the length access lambda expression should match the collection type '{0}'."
        /// </summary>
        internal static Exception LengthAccessParameterShouldHaveCollectionType(object? p0) => new ArgumentException(Strings.LengthAccessParameterShouldHaveCollectionType(p0));

        /// <summary>
        /// ArgumentException with message like "The first parameter of the indexer access lambda expression should match the collection type '{0}'."
        /// </summary>
        internal static Exception IndexerAccessFirstParameterShouldHaveCollectionType(object? p0) => new ArgumentException(Strings.IndexerAccessFirstParameterShouldHaveCollectionType(p0));

        /// <summary>
        /// ArgumentException with message like "The second parameter of the indexer access lambda expression should be of type '{0}'."
        /// </summary>
        internal static Exception IndexerAccessSecondParameterInvalidType(object? p0) => new ArgumentException(Strings.IndexerAccessSecondParameterInvalidType(p0));

        /// <summary>
        /// ArgumentException with message like "The non-nullable list pattern input type '{0}' should match collection type '{1}'."
        /// </summary>
        internal static Exception ListPatternInputTypeInvalid(object? p0, object? p1) => new ArgumentException(Strings.ListPatternInputTypeInvalid(p0, p1));

        /// <summary>
        /// ArgumentException with message like "A foreach statement requires at least one iteration variable."
        /// </summary>
        internal static Exception ForEachNeedsOneOrMoreVariables() => new ArgumentException(Strings.ForEachNeedsOneOrMoreVariables);

        /// <summary>
        /// ArgumentException with message like "The collection type '{0}' is not compatible with the type '{1}' of the collection expression."
        /// </summary>
        internal static Exception ForEachCollectionTypeNotCompatibleWithCollectionExpression(object? p0, object? p1) => new ArgumentException(Strings.ForEachCollectionTypeNotCompatibleWithCollectionExpression(p0, p1));

        /// <summary>
        /// ArgumentException with message like "A foreach statement with a deconstruction step requires more than one iteration variable."
        /// </summary>
        internal static Exception ForEachDeconstructionNotSupportedWithOneVariable() => new ArgumentException(Strings.ForEachDeconstructionNotSupportedWithOneVariable);

        /// <summary>
        /// ArgumentException with message like "A foreach statement with more than one iteration variables requires a deconstruction step."
        /// </summary>
        internal static Exception ForEachDeconstructionRequiredForMultipleVariables() => new ArgumentException(Strings.ForEachDeconstructionRequiredForMultipleVariables);

        /// <summary>
        /// ArgumentException with message like "The deconstruction lambda expression for a foreach statement should have one parameter."
        /// </summary>
        internal static Exception ForEachDeconstructionShouldHaveOneParameter() => new ArgumentException(Strings.ForEachDeconstructionShouldHaveOneParameter);

        /// <summary>
        /// ArgumentException with message like "The type '{0}' returned by the deconstruction lambda expression is not a tuple type."
        /// </summary>
        internal static Exception ForEachDeconstructionShouldReturnTuple(object? p0) => new ArgumentException(Strings.ForEachDeconstructionShouldReturnTuple(p0));

        /// <summary>
        /// ArgumentException with message like "The tuple type '{0}' returned by the deconstruction lambda expression has an arity '{1}' that does not match the number of iteration variables."
        /// </summary>
        internal static Exception ForEachDeconstructionComponentMismatch(object? p0, object? p1) => new ArgumentException(Strings.ForEachDeconstructionComponentMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the tuple component at index '{1}' returned by the deconstruction lambda cannot be assigned to variable '{2}' of type '{3}'."
        /// </summary>
        internal static Exception ForEachDeconstructionComponentNotAssignableToVariable(object? p0, object? p1, object? p2, object? p3) => new ArgumentException(Strings.ForEachDeconstructionComponentNotAssignableToVariable(p0, p1, p2, p3));

        /// <summary>
        /// ArgumentException with message like "The method '{1}' on type '{0}' is not an event accessor."
        /// </summary>
        internal static Exception MethodNotEventAccessor(object? p0, object? p1) => new ArgumentException(Strings.MethodNotEventAccessor(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The event '{0}' does not have an accessor."
        /// </summary>
        internal static Exception EventDoesNotHaveAccessor(object? p0) => new ArgumentException(Strings.EventDoesNotHaveAccessor(p0));

        /// <summary>
        /// ArgumentException with message like "Only static events have an object expression."
        /// </summary>
        internal static Exception OnlyStaticEventsHaveNullInstance() => new ArgumentException(Strings.OnlyStaticEventsHaveNullInstance);

        /// <summary>
        /// ArgumentException with message like "The event '{0}' is not declared on type '{1}'."
        /// </summary>
        internal static Exception EventNotDefinedForType(object? p0, object? p1) => new ArgumentException(Strings.EventNotDefinedForType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "An event accessor method should return void."
        /// </summary>
        internal static Exception EventAccessorShouldReturnVoid() => new ArgumentException(Strings.EventAccessorShouldReturnVoid);

        /// <summary>
        /// ArgumentException with message like "An event accessor method should have one parameter."
        /// </summary>
        internal static Exception EventAccessorShouldHaveOneParameter() => new ArgumentException(Strings.EventAccessorShouldHaveOneParameter);

        /// <summary>
        /// ArgumentException with message like "The handler expression type '{0}' is not assignable to the event accessor parameter of type '{1}'."
        /// </summary>
        internal static Exception EventAccessorParameterTypeMismatch(object? p0, object? p1) => new ArgumentException(Strings.EventAccessorParameterTypeMismatch(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The '{0}' is not a valid interpolated string handler type."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerType(object? p0) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerType(p0));

        /// <summary>
        /// ArgumentException with message like "The construction lambda return type '{0}' is not assignable to interpolated string handler type '{1}'."
        /// </summary>
        internal static Exception InterpolatedStringHandlerTypeNotAssignable(object? p0, object? p1) => new ArgumentException(Strings.InterpolatedStringHandlerTypeNotAssignable(p0, p1));

        /// <summary>
        /// ArgumentException with message like "An interpolated string handler construction should have at least two parameters for 'literalLength' and 'formattedCount'."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerConstructionArgCount() => new ArgumentException(Strings.InvalidInterpolatedStringHandlerConstructionArgCount);

        /// <summary>
        /// ArgumentException with message like "The '{0}' parameter representing '{1}' should be of type Int32."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerInt32ParameterType(object? p0, object? p1) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerInt32ParameterType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' is not a valid return type for an append call. Only 'void' and 'bool' are supported."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerAppendReturnType(object? p0) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerAppendReturnType(p0));

        /// <summary>
        /// ArgumentException with message like "The return types of the append calls is inconsistent."
        /// </summary>
        internal static Exception InconsistentInterpolatedStringHandlerAppendReturnType() => new ArgumentException(Strings.InconsistentInterpolatedStringHandlerAppendReturnType);

        /// <summary>
        /// ArgumentException with message like "An interpolated string handler append call should have at least one parameter for the handler instance."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerAppendArgCount() => new ArgumentException(Strings.InvalidInterpolatedStringHandlerAppendArgCount);

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the first parameter of the interpolated string handler append call is not compatible with the interpolated string handler type '{1}'."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerAppendFirstArgType(object? p0, object? p1) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerAppendFirstArgType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' is not valid for an interpolated string handler conversion."
        /// </summary>
        internal static Exception InvalidStringHandlerConversionOperandType(object? p0) => new ArgumentException(Strings.InvalidStringHandlerConversionOperandType(p0));

        /// <summary>
        /// ArgumentException with message like "The node of type '{0}' is not valid as the operand for an interpolated string handler conversion."
        /// </summary>
        internal static Exception InvalidStringHandlerConversionOperandNodeType(object? p0) => new ArgumentException(Strings.InvalidStringHandlerConversionOperandNodeType(p0));

        /// <summary>
        /// ArgumentException with message like "The argument index '{0}' is not valid."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerArgumentIndex(object? p0) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerArgumentIndex(p0));

        /// <summary>
        /// ArgumentException with message like "The number of parameters '{0}' for the interpolated string handler construction is insufficient for an argument count of '{1}' (need at least 'literalLength' and 'formattedCount')."
        /// </summary>
        internal static Exception NotEnoughInterpolatedStringHandlerConstructionParameters(object? p0, object? p1) => new ArgumentException(Strings.NotEnoughInterpolatedStringHandlerConstructionParameters(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The number of parameters '{0}' for the interpolated string handler construction is too large for an argument count of '{1}' (can have at most one extra 'out bool' parameter)."
        /// </summary>
        internal static Exception TooManyInterpolatedStringHandlerConstructionParameters(object? p0, object? p1) => new ArgumentException(Strings.TooManyInterpolatedStringHandlerConstructionParameters(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The last parameter of type '{0}' for the interpolated string handler construction is not valid for an 'out bool shouldAppend' trailing parameter."
        /// </summary>
        internal static Exception InvalidInterpolatedStringHandlerConstructionOutBoolParameter(object? p0) => new ArgumentException(Strings.InvalidInterpolatedStringHandlerConstructionOutBoolParameter(p0));

        /// <summary>
        /// ArgumentException with message like "The append lambda expression's first parameter '{0}' denoting the interpolated string handler should be passed by reference."
        /// </summary>
        internal static Exception AppendLambdaShouldHaveFirstByRefParameter(object? p0) => new ArgumentException(Strings.AppendLambdaShouldHaveFirstByRefParameter(p0));

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendLiteral' operation should take 2 parameters."
        /// </summary>
        internal static Exception AppendLiteralLambdaShouldHaveTwoParameters() => new ArgumentException(Strings.AppendLiteralLambdaShouldHaveTwoParameters);

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendLiteral' operation has a second parameter of type '{0}' which is invalid and should be 'string'."
        /// </summary>
        internal static Exception AppendLiteralLambdaShouldTakeStringParameter(object? p0) => new ArgumentException(Strings.AppendLiteralLambdaShouldTakeStringParameter(p0));

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendFormatted' operation should take 2, 3, or 4 parameters."
        /// </summary>
        internal static Exception AppendFormattedLambdaInvalidParameterCount() => new ArgumentException(Strings.AppendFormattedLambdaInvalidParameterCount);

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendFormatted' operation should have a second parameter denoting a non-void value."
        /// </summary>
        internal static Exception AppendFormattedLambdaSecondParameterShouldBeNonVoid() => new ArgumentException(Strings.AppendFormattedLambdaSecondParameterShouldBeNonVoid);

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment or 'string' to denote a format."
        /// </summary>
        internal static Exception AppendFormattedLambdaThirdParameterShouldBeIntOrString(object? p0) => new ArgumentException(Strings.AppendFormattedLambdaThirdParameterShouldBeIntOrString(p0));

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment."
        /// </summary>
        internal static Exception AppendFormattedLambdaThirdParameterShouldBeInt(object? p0) => new ArgumentException(Strings.AppendFormattedLambdaThirdParameterShouldBeInt(p0));

        /// <summary>
        /// ArgumentException with message like "The lambda expression representing the 'AppendFormatted' operation has a fourth parameter of type '{0}' and should be 'string' to denote a format."
        /// </summary>
        internal static Exception AppendFormattedLambdaFourthParameterShouldBeString(object? p0) => new ArgumentException(Strings.AppendFormattedLambdaFourthParameterShouldBeString(p0));

        /// <summary>
        /// ArgumentException with message like "The number of append operations '{0}' does not match the number of interpolations '{1}' in the interpolated string operand."
        /// </summary>
        internal static Exception IncorrectNumberOfAppendsForInterpolatedString(object? p0, object? p1) => new ArgumentException(Strings.IncorrectNumberOfAppendsForInterpolatedString(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The number of parameters '{0}' for the 'AppendFormatted' operation does not match the expected number '{1}' for the interpolated string's interpolation."
        /// </summary>
        internal static Exception InvalidAppendFormattedParameterCount(object? p0, object? p1) => new ArgumentException(Strings.InvalidAppendFormattedParameterCount(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the 'value' parameter for the 'AppendFormatted' operation is not compatible with the expected type '{1}' for the interpolated string's interpolation value."
        /// </summary>
        internal static Exception InvalidAppendFormattedValueType(object? p0, object? p1) => new ArgumentException(Strings.InvalidAppendFormattedValueType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' is invalid for the alignment parameter and should be 'int'."
        /// </summary>
        internal static Exception InvalidAlignmentParameterType(object? p0) => new ArgumentException(Strings.InvalidAlignmentParameterType(p0));

        /// <summary>
        /// ArgumentException with message like "The type '{0}' is invalid for the format parameter and should be 'string'."
        /// </summary>
        internal static Exception InvalidFormatParameterType(object? p0) => new ArgumentException(Strings.InvalidFormatParameterType(p0));

        /// <summary>
        /// ArgumentException with message like "The 'getEnumerator' lambda should have a single parameter."
        /// </summary>
        internal static Exception GetEnumeratorShouldHaveSingleParameter() => new ArgumentException(Strings.GetEnumeratorShouldHaveSingleParameter);

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the first parameter of the 'getEnumerator' lambda is not compatible with the collection type '{1}'."
        /// </summary>
        internal static Exception InvalidGetEnumeratorFirstArgType(object? p0, object? p1) => new ArgumentException(Strings.InvalidGetEnumeratorFirstArgType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The 'moveNext' lambda should have a single parameter."
        /// </summary>
        internal static Exception MoveNextShouldHaveSingleParameter() => new ArgumentException(Strings.MoveNextShouldHaveSingleParameter);

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the first parameter of the 'moveNext' lambda is not compatible with the enumerator type '{1}'."
        /// </summary>
        internal static Exception InvalidMoveNextFirstArgType(object? p0, object? p1) => new ArgumentException(Strings.InvalidMoveNextFirstArgType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The 'moveNext' lambda should return 'bool'."
        /// </summary>
        internal static Exception MoveNextShouldHaveBooleanReturnType() => new ArgumentException(Strings.MoveNextShouldHaveBooleanReturnType);

        /// <summary>
        /// ArgumentException with message like "The '{0}' property should not have indexer parameters."
        /// </summary>
        internal static Exception PropertyShouldNotBeIndexer(object? p0) => new ArgumentException(Strings.PropertyShouldNotBeIndexer(p0));

        /// <summary>
        /// ArgumentException with message like "The '{0}' property should not have a 'void' type."
        /// </summary>
        internal static Exception PropertyShouldNotReturnVoid(object? p0) => new ArgumentException(Strings.PropertyShouldNotReturnVoid(p0));

        /// <summary>
        /// ArgumentException with message like "The 'currentConversion' lambda should have a single parameter."
        /// </summary>
        internal static Exception CurrentConversionShouldHaveSingleParameter() => new ArgumentException(Strings.CurrentConversionShouldHaveSingleParameter);

        /// <summary>
        /// ArgumentException with message like "The type '{0}' of the first parameter of the 'currentConversion' lambda is not compatible with the collection type '{1}'."
        /// </summary>
        internal static Exception InvalidCurrentConversionFirstArgType(object? p0, object? p1) => new ArgumentException(Strings.InvalidCurrentConversionFirstArgType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "The element type '{0}' is not compatible with the type '{1}' returned by the 'Current' property."
        /// </summary>
        internal static Exception InvalidCurrentReturnType(object? p0, object? p1) => new ArgumentException(Strings.InvalidCurrentReturnType(p0, p1));

        /// <summary>
        /// ArgumentException with message like "Asynchronous enumeration is not supported for array types."
        /// </summary>
        internal static Exception AsyncEnumerationNotSupportedForArray() => new ArgumentException(Strings.AsyncEnumerationNotSupportedForArray);

        /// <summary>
        /// ArgumentException with message like "Asynchronous enumeration is not supported on type 'String'."
        /// </summary>
        internal static Exception AsyncEnumerationNotSupportedForString() => new ArgumentException(Strings.AsyncEnumerationNotSupportedForString);

        /// <summary>
        /// ArgumentException with message like "The '{0}.{1}' method is ambiguous."
        /// </summary>
        internal static Exception AmbiguousEnumeratorMethod(object? p0, object? p1) => new ArgumentException(Strings.AmbiguousEnumeratorMethod(p0, p1));

        /// <summary>
        /// ArgumentException with message like "List pattern should have a collection type or a variable."
        /// </summary>
        internal static Exception ListPatternShouldHaveCollectionTypeOrVariable() => new ArgumentException(Strings.ListPatternShouldHaveCollectionTypeOrVariable);

    }

    /// <summary>
    /// Strongly-typed and parameterized string resources.
    /// </summary>
    internal static partial class Strings
    {
        /// <summary>
        /// A string like "Parameter '{0}' is not defined for method '{1}'."
        /// </summary>
        internal static string ParameterNotDefinedForMethod(object? p0, object? p1) => SR.Format(SR.ParameterNotDefinedForMethod, p0, p1);

        /// <summary>
        /// A string like "Parameter index '{0}' is out of bounds for method '{1}'."
        /// </summary>
        internal static string ParameterIndexOutOfBounds(object? p0, object? p1) => SR.Format(SR.ParameterIndexOutOfBounds, p0, p1);

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be used for parameter of type '{1}'."
        /// </summary>
        internal static string ExpressionTypeDoesNotMatchParameter(object? p0, object? p1) => SR.Format(SR.ExpressionTypeDoesNotMatchParameter, p0, p1);

        /// <summary>
        /// A string like "Parameter '{0}' has multiple bindings."
        /// </summary>
        internal static string DuplicateParameterBinding(object? p0) => SR.Format(SR.DuplicateParameterBinding, p0);

        /// <summary>
        /// A string like "Non-optional parameter '{0}' has no binding."
        /// </summary>
        internal static string UnboundParameter(object? p0) => SR.Format(SR.UnboundParameter, p0);

        /// <summary>
        /// A string like "A non-static constructor is required."
        /// </summary>
        internal static string NonStaticConstructorRequired => SR.NonStaticConstructorRequired;

        /// <summary>
        /// A string like "The property '{0}' has no 'get' accessor."
        /// </summary>
        internal static string PropertyDoesNotHaveGetAccessor(object? p0) => SR.Format(SR.PropertyDoesNotHaveGetAccessor, p0);

        /// <summary>
        /// A string like "The property '{0}' has no 'set' accessor."
        /// </summary>
        internal static string PropertyDoesNotHaveSetAccessor(object? p0) => SR.Format(SR.PropertyDoesNotHaveSetAccessor, p0);

        /// <summary>
        /// A string like "A non-static 'get' accessor is required for property '{0}'."
        /// </summary>
        internal static string AccessorCannotBeStatic(object? p0) => SR.Format(SR.AccessorCannotBeStatic, p0);

        /// <summary>
        /// A string like "The number of indexes specified does not match the array rank."
        /// </summary>
        internal static string RankMismatch => SR.RankMismatch;

        /// <summary>
        /// A string like "The specified index is out of range."
        /// </summary>
        internal static string IndexOutOfRange => SR.IndexOutOfRange;

        /// <summary>
        /// A string like "An array dimension cannot be less than 0."
        /// </summary>
        internal static string BoundCannotBeLessThanZero => SR.BoundCannotBeLessThanZero;

        /// <summary>
        /// A string like "The number of elements does not match the length of the array."
        /// </summary>
        internal static string ArrayBoundsElementCountMismatch => SR.ArrayBoundsElementCountMismatch;

        /// <summary>
        /// A string like "The 'GetAwaiter' method should take zero parameters."
        /// </summary>
        internal static string GetAwaiterShouldTakeZeroParameters => SR.GetAwaiterShouldTakeZeroParameters;

        /// <summary>
        /// A string like "The 'GetAwaiter' method should not be generic."
        /// </summary>
        internal static string GetAwaiterShouldNotBeGeneric => SR.GetAwaiterShouldNotBeGeneric;

        /// <summary>
        /// A string like "The 'GetAwaiter' method has an unsupported return type."
        /// </summary>
        internal static string GetAwaiterShouldReturnAwaiterType => SR.GetAwaiterShouldReturnAwaiterType;

        /// <summary>
        /// A string like "Awaitable type '{0}' should have a 'GetAwaiter' method."
        /// </summary>
        internal static string AwaitableTypeShouldHaveGetAwaiterMethod(object? p0) => SR.Format(SR.AwaitableTypeShouldHaveGetAwaiterMethod, p0);

        /// <summary>
        /// A string like "Awaiter type '{0}' should implement 'INotifyCompletion'."
        /// </summary>
        internal static string AwaiterTypeShouldImplementINotifyCompletion(object? p0) => SR.Format(SR.AwaiterTypeShouldImplementINotifyCompletion, p0);

        /// <summary>
        /// A string like "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor."
        /// </summary>
        internal static string AwaiterTypeShouldHaveIsCompletedProperty(object? p0) => SR.Format(SR.AwaiterTypeShouldHaveIsCompletedProperty, p0);

        /// <summary>
        /// A string like "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'."
        /// </summary>
        internal static string AwaiterIsCompletedShouldReturnBool(object? p0) => SR.Format(SR.AwaiterIsCompletedShouldReturnBool, p0);

        /// <summary>
        /// A string like "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters."
        /// </summary>
        internal static string AwaiterIsCompletedShouldNotBeIndexer(object? p0) => SR.Format(SR.AwaiterIsCompletedShouldNotBeIndexer, p0);

        /// <summary>
        /// A string like "Awaiter type '{0}' should have a 'GetResult' method."
        /// </summary>
        internal static string AwaiterTypeShouldHaveGetResultMethod(object? p0) => SR.Format(SR.AwaiterTypeShouldHaveGetResultMethod, p0);

        /// <summary>
        /// A string like "The 'GetResult' method on awaiter type '{0}' has an unsupported return type."
        /// </summary>
        internal static string AwaiterGetResultTypeInvalid(object? p0) => SR.Format(SR.AwaiterGetResultTypeInvalid, p0);

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have a 'GetAwaiter' expression."
        /// </summary>
        internal static string DynamicAwaitNoGetAwaiter => SR.DynamicAwaitNoGetAwaiter;

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have an 'IsCompleted' property."
        /// </summary>
        internal static string DynamicAwaitNoIsCompleted => SR.DynamicAwaitNoIsCompleted;

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have a 'GetResult' method."
        /// </summary>
        internal static string DynamicAwaitNoGetResult => SR.DynamicAwaitNoGetResult;

        /// <summary>
        /// A string like "The 'GetAwaiter' expression should have one parameter."
        /// </summary>
        internal static string GetAwaiterExpressionOneParameter => SR.GetAwaiterExpressionOneParameter;

        /// <summary>
        /// A string like "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions."
        /// </summary>
        internal static string AsyncLambdaCantHaveByRefParameter(object? p0) => SR.Format(SR.AsyncLambdaCantHaveByRefParameter, p0);

        /// <summary>
        /// A string like "Return type '{0}' is not valid for an asynchronous lambda expression."
        /// </summary>
        internal static string AsyncLambdaInvalidReturnType(object? p0) => SR.Format(SR.AsyncLambdaInvalidReturnType, p0);

        /// <summary>
        /// A string like "Await expression cannot occur in '{0}'."
        /// </summary>
        internal static string AwaitForbiddenHere(object? p0) => SR.Format(SR.AwaitForbiddenHere, p0);

        /// <summary>
        /// A string like "An expression of type '{0}' can't be used as a lock."
        /// </summary>
        internal static string LockNeedsReferenceType(object? p0) => SR.Format(SR.LockNeedsReferenceType, p0);

        /// <summary>
        /// A string like "The conversion lambda should have one parameter."
        /// </summary>
        internal static string ConversionNeedsOneParameter => SR.ConversionNeedsOneParameter;

        /// <summary>
        /// A string like "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'."
        /// </summary>
        internal static string ConversionInvalidArgument(object? p0, object? p1) => SR.Format(SR.ConversionInvalidArgument, p0, p1);

        /// <summary>
        /// A string like "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'."
        /// </summary>
        internal static string ConversionInvalidResult(object? p0, object? p1) => SR.Format(SR.ConversionInvalidResult, p0, p1);

        /// <summary>
        /// A string like "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor."
        /// </summary>
        internal static string EnumeratorShouldHaveCurrentProperty(object? p0) => SR.Format(SR.EnumeratorShouldHaveCurrentProperty, p0);

        /// <summary>
        /// A string like "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type."
        /// </summary>
        internal static string EnumeratorShouldHaveMoveNextMethod(object? p0) => SR.Format(SR.EnumeratorShouldHaveMoveNextMethod, p0);

        /// <summary>
        /// A string like "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'."
        /// </summary>
        internal static string MoreThanOneIEnumerableFound(object? p0) => SR.Format(SR.MoreThanOneIEnumerableFound, p0);

        /// <summary>
        /// A string like "Collection type '{0}' has no valid enumerable pattern."
        /// </summary>
        internal static string NoEnumerablePattern(object? p0) => SR.Format(SR.NoEnumerablePattern, p0);

        /// <summary>
        /// A string like "Initializers should be assignments to variables."
        /// </summary>
        internal static string InvalidInitializer => SR.InvalidInitializer;

        /// <summary>
        /// A string like "Break and continue lables should be different."
        /// </summary>
        internal static string DuplicateLabels => SR.DuplicateLabels;

        /// <summary>
        /// A string like "Conditional access expressions require non-static members or extension methods."
        /// </summary>
        internal static string ConditionalAccessRequiresNonStaticMember => SR.ConditionalAccessRequiresNonStaticMember;

        /// <summary>
        /// A string like "Conditional access expressions require readable properties."
        /// </summary>
        internal static string ConditionalAccessRequiresReadableProperty => SR.ConditionalAccessRequiresReadableProperty;

        /// <summary>
        /// A string like "Too many arguments have been specified."
        /// </summary>
        internal static string TooManyArguments => SR.TooManyArguments;

        /// <summary>
        /// A string like "Conditional call expressions for extensions methods should specify an instance expression."
        /// </summary>
        internal static string ExtensionMethodRequiresInstance => SR.ExtensionMethodRequiresInstance;

        /// <summary>
        /// A string like "Type '{0}' is not a valid governing type for a switch statement."
        /// </summary>
        internal static string InvalidSwitchType(object? p0) => SR.Format(SR.InvalidSwitchType, p0);

        /// <summary>
        /// A string like "The test value '{0}' occurs more than once."
        /// </summary>
        internal static string DuplicateTestValue(object? p0) => SR.Format(SR.DuplicateTestValue, p0);

        /// <summary>
        /// A string like "A 'null' test value cannot be used in a switch statement with governing type '{0}'."
        /// </summary>
        internal static string SwitchCantHaveNullCase(object? p0) => SR.Format(SR.SwitchCantHaveNullCase, p0);

        /// <summary>
        /// A string like "A test value with type '{0}' cannot be used in a swich statement with governing type '{1}'."
        /// </summary>
        internal static string SwitchCaseHasIncompatibleType(object? p0, object? p1) => SR.Format(SR.SwitchCaseHasIncompatibleType, p0, p1);

        /// <summary>
        /// A string like "All specified test values should have the same type."
        /// </summary>
        internal static string TestValuesShouldHaveConsistentType => SR.TestValuesShouldHaveConsistentType;

        /// <summary>
        /// A string like "The break label of a switch statement should be of type 'void'."
        /// </summary>
        internal static string SwitchBreakLabelShouldBeVoid => SR.SwitchBreakLabelShouldBeVoid;

        /// <summary>
        /// A string like "The label of a switch case should be of type 'void'."
        /// </summary>
        internal static string SwitchLabelTargetShouldBeVoid => SR.SwitchLabelTargetShouldBeVoid;

        /// <summary>
        /// A string like "A 'goto case {0}' statement was found but the containing switch statement has no such label."
        /// </summary>
        internal static string InvalidGotoCase(object? p0) => SR.Format(SR.InvalidGotoCase, p0);

        /// <summary>
        /// A string like "A 'goto default' statement was found but the containing switch statement has no default label."
        /// </summary>
        internal static string InvalidGotoDefault => SR.InvalidGotoDefault;

        /// <summary>
        /// A string like "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node."
        /// </summary>
        internal static string GotoCanOnlyBeReducedInSwitch => SR.GotoCanOnlyBeReducedInSwitch;

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for a conditional receiver."
        /// </summary>
        internal static string InvalidConditionalReceiverType(object? p0) => SR.Format(SR.InvalidConditionalReceiverType, p0);

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for a receiver in a conditional access expression."
        /// </summary>
        internal static string InvalidConditionalReceiverExpressionType(object? p0) => SR.Format(SR.InvalidConditionalReceiverExpressionType, p0);

        /// <summary>
        /// A string like "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver."
        /// </summary>
        internal static string ConditionalReceiverTypeMismatch(object? p0, object? p1) => SR.Format(SR.ConditionalReceiverTypeMismatch, p0, p1);

        /// <summary>
        /// A string like "Compound assignment operation '{0}' is not supported for type '{1}'."
        /// </summary>
        internal static string InvalidCompoundAssignment(object? p0, object? p1) => SR.Format(SR.InvalidCompoundAssignment, p0, p1);

        /// <summary>
        /// A string like "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'."
        /// </summary>
        internal static string InvalidCompoundAssignmentWithOperands(object? p0, object? p1, object? p2) => SR.Format(SR.InvalidCompoundAssignmentWithOperands, p0, p1, p2);

        /// <summary>
        /// A string like "Unary assignment operation '{0}' is not supported for an operand of type '{1}'."
        /// </summary>
        internal static string InvalidUnaryAssignmentWithOperands(object? p0, object? p1) => SR.Format(SR.InvalidUnaryAssignmentWithOperands, p0, p1);

        /// <summary>
        /// A string like "Type must not be ByRef."
        /// </summary>
        internal static string TypeMustNotBeByRef => SR.TypeMustNotBeByRef;

        /// <summary>
        /// A string like "Type must not be a pointer type."
        /// </summary>
        internal static string TypeMustNotBePointer => SR.TypeMustNotBePointer;

        /// <summary>
        /// A string like "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations."
        /// </summary>
        internal static string InvalidNullCoalescingAssignmentArguments => SR.InvalidNullCoalescingAssignmentArguments;

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable."
        /// </summary>
        internal static string InvalidInterpolatedStringType(object? p0) => SR.Format(SR.InvalidInterpolatedStringType, p0);

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for an index operand. Supported types are int or int?."
        /// </summary>
        internal static string InvalidFromEndIndexOperandType(object? p0) => SR.Format(SR.InvalidFromEndIndexOperandType, p0);

        /// <summary>
        /// A string like "The specified method is not valid to construct an object of type Index."
        /// </summary>
        internal static string InvalidFromEndIndexMethod => SR.InvalidFromEndIndexMethod;

        /// <summary>
        /// A string like "Type '{0}' is not a valid index type. Supported types are Index or Index?."
        /// </summary>
        internal static string InvalidIndexType(object? p0) => SR.Format(SR.InvalidIndexType, p0);

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for a range operand. Supported types are Index or Index?."
        /// </summary>
        internal static string InvalidRangeOperandType(object? p0) => SR.Format(SR.InvalidRangeOperandType, p0);

        /// <summary>
        /// A string like "The specified method is not valid to construct an object of type Range."
        /// </summary>
        internal static string InvalidRangeMethod => SR.InvalidRangeMethod;

        /// <summary>
        /// A string like "Type '{0}' is not a valid range type. Supported types are Range or Range?."
        /// </summary>
        internal static string InvalidRangeType(object? p0) => SR.Format(SR.InvalidRangeType, p0);

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for an 'IndexerAccess' argument. Supported types are Index or Range."
        /// </summary>
        internal static string InvalidIndexerAccessArgumentType(object? p0) => SR.Format(SR.InvalidIndexerAccessArgumentType, p0);

        /// <summary>
        /// A string like "Property '{0}' should be of type int."
        /// </summary>
        internal static string InvalidLengthOrCountPropertyType(object? p0) => SR.Format(SR.InvalidLengthOrCountPropertyType, p0);

        /// <summary>
        /// A string like "Member '{0}' is not a valid member for an indexer. Supported member types are MethodInfo or PropertyInfo."
        /// </summary>
        internal static string InvalidIndexMember(object? p0) => SR.Format(SR.InvalidIndexMember, p0);

        /// <summary>
        /// A string like "Member '{0}' is not a valid member for a slice method."
        /// </summary>
        internal static string InvalidSliceMember(object? p0) => SR.Format(SR.InvalidSliceMember, p0);

        /// <summary>
        /// A string like "Indexer '{0}' does not have an 'int' parameter type."
        /// </summary>
        internal static string InvalidIndexerParameterType(object? p0) => SR.Format(SR.InvalidIndexerParameterType, p0);

        /// <summary>
        /// A string like "Slice method '{0}' should be an instance method."
        /// </summary>
        internal static string SliceMethodMustNotBeStatic(object? p0) => SR.Format(SR.SliceMethodMustNotBeStatic, p0);

        /// <summary>
        /// A string like "Slice method '{0}' should be have exactly two parameters of type 'int'."
        /// </summary>
        internal static string InvalidSliceParameters(object? p0) => SR.Format(SR.InvalidSliceParameters, p0);

        /// <summary>
        /// A string like "Type '{0}' is not a valid tuple type."
        /// </summary>
        internal static string InvalidTupleType(object? p0) => SR.Format(SR.InvalidTupleType, p0);

        /// <summary>
        /// A string like "The number of arguments does not match the number of components of tuple type '{0}'."
        /// </summary>
        internal static string InvalidTupleArgumentCount(object? p0) => SR.Format(SR.InvalidTupleArgumentCount, p0);

        /// <summary>
        /// A string like "The number of argument names does not match the number of components of tuple type '{0}'."
        /// </summary>
        internal static string InvalidTupleArgumentNamesCount(object? p0) => SR.Format(SR.InvalidTupleArgumentNamesCount, p0);

        /// <summary>
        /// A string like "The type of a tuple component cannot be void."
        /// </summary>
        internal static string TupleComponentCannotBeVoid => SR.TupleComponentCannotBeVoid;

        /// <summary>
        /// A string like "The arity of tuple type '{0}' does not match the arity of tuple type '{1}'."
        /// </summary>
        internal static string TupleComponentCountMismatch(object? p0, object? p1) => SR.Format(SR.TupleComponentCountMismatch, p0, p1);

        /// <summary>
        /// A string like "The number of element conversion expressions does not match tuple arity '{0}'."
        /// </summary>
        internal static string InvalidElementConversionCount(object? p0) => SR.Format(SR.InvalidElementConversionCount, p0);

        /// <summary>
        /// A string like "The number of equality check expressions does not match tuple arity '{0}'."
        /// </summary>
        internal static string InvalidEqualityCheckCount(object? p0) => SR.Format(SR.InvalidEqualityCheckCount, p0);

        /// <summary>
        /// A string like "'{0}' is not a member of any type."
        /// </summary>
        internal static string NotAMemberOfAnyType(object? p0) => SR.Format(SR.NotAMemberOfAnyType, p0);

        /// <summary>
        /// A string like "A 'with' expression for value type '{0}' cannot specify a 'Clone' method."
        /// </summary>
        internal static string WithExpressionCannotHaveCloneForValueType(object? p0) => SR.Format(SR.WithExpressionCannotHaveCloneForValueType, p0);

        /// <summary>
        /// A string like "A 'with' expression for type '{0}' should specify a 'Clone' method."
        /// </summary>
        internal static string WithExpressionShouldHaveClone(object? p0) => SR.Format(SR.WithExpressionShouldHaveClone, p0);

        /// <summary>
        /// A string like "Clone method '{0}' should have no parameters."
        /// </summary>
        internal static string CloneMethodShouldHaveNoParameters(object? p0) => SR.Format(SR.CloneMethodShouldHaveNoParameters, p0);

        /// <summary>
        /// A string like "Clone method '{0}' should be an instance method."
        /// </summary>
        internal static string CloneMethodMustNotBeStatic(object? p0) => SR.Format(SR.CloneMethodMustNotBeStatic, p0);

        /// <summary>
        /// A string like "Clone method '{0}' should return a type that can be converted to '{1}'."
        /// </summary>
        internal static string CloneMethodShouldReturnCompatibleType(object? p0, object? p1) => SR.Format(SR.CloneMethodShouldReturnCompatibleType, p0, p1);

        /// <summary>
        /// A string like "Member '{0}' used in a 'MemberInitializer' cannot be static."
        /// </summary>
        internal static string MemberInitializerMemberMustNotBeStatic(object? p0) => SR.Format(SR.MemberInitializerMemberMustNotBeStatic, p0);

        /// <summary>
        /// A string like "Member '{0}' used in a 'MemberInitializer' cannot be an indexer."
        /// </summary>
        internal static string MemberInitializerMemberMustNotBeIndexer(object? p0) => SR.Format(SR.MemberInitializerMemberMustNotBeIndexer, p0);

        /// <summary>
        /// A string like "Member '{0}' used in a 'MemberInitializer' must be writeable."
        /// </summary>
        internal static string MemberInitializerMemberMustBeWriteable(object? p0) => SR.Format(SR.MemberInitializerMemberMustBeWriteable, p0);

        /// <summary>
        /// A string like "No suitable constructor found for type '{0}' using the specified members."
        /// </summary>
        internal static string NoAnonymousTypeConstructorFound(object? p0) => SR.Format(SR.NoAnonymousTypeConstructorFound, p0);

        /// <summary>
        /// A string like "A pattern can never produce a value of a nullable type."
        /// </summary>
        internal static string CannotHaveNullablePatternType => SR.CannotHaveNullablePatternType;

        /// <summary>
        /// A string like "The input and narrowed type for a pattern of type '{0}' should be equal."
        /// </summary>
        internal static string PatternInputAndNarrowedTypeShouldMatch(object? p0) => SR.Format(SR.PatternInputAndNarrowedTypeShouldMatch, p0);

        /// <summary>
        /// A string like "The variable type '{0}' should be equal to the pattern result type '{1}'."
        /// </summary>
        internal static string CannotAssignPatternResultToVariable(object? p0, object? p1) => SR.Format(SR.CannotAssignPatternResultToVariable, p0, p1);

        /// <summary>
        /// A string like "The pattern type '{0}' is not compatible with a subpattern type '{1}'."
        /// </summary>
        internal static string PatternTypeMismatch(object? p0, object? p1) => SR.Format(SR.PatternTypeMismatch, p0, p1);

        /// <summary>
        /// A string like "The pattern type '{0}' is not a valid binary pattern type."
        /// </summary>
        internal static string InvalidBinaryPatternType(object? p0) => SR.Format(SR.InvalidBinaryPatternType, p0);

        /// <summary>
        /// A string like "The pattern type '{0}' is not a valid relational pattern type."
        /// </summary>
        internal static string InvalidRelationalPatternType(object? p0) => SR.Format(SR.InvalidRelationalPatternType, p0);

        /// <summary>
        /// A string like "The type '{0}' cannot be used for a constant in a pattern."
        /// </summary>
        internal static string InvalidPatternConstantType(object? p0) => SR.Format(SR.InvalidPatternConstantType, p0);

        /// <summary>
        /// A string like "The type '{0}' cannot be used for a constant in a relational pattern."
        /// </summary>
        internal static string InvalidRelationalPatternConstantType(object? p0) => SR.Format(SR.InvalidRelationalPatternConstantType, p0);

        /// <summary>
        /// A string like "The value NaN cannot be used for a constant in a relational pattern."
        /// </summary>
        internal static string CannotUsePatternConstantNaN => SR.CannotUsePatternConstantNaN;

        /// <summary>
        /// A string like "A null pattern should use a constant of type object."
        /// </summary>
        internal static string NullValueShouldUseObjectType => SR.NullValueShouldUseObjectType;

        /// <summary>
        /// A string like "A null value cannot be used in a relational pattern."
        /// </summary>
        internal static string CannotUseNullValueInRelationalPattern => SR.CannotUseNullValueInRelationalPattern;

        /// <summary>
        /// A string like "The 'GetLengthMethod' of an 'ITuple' pattern should return an integer value."
        /// </summary>
        internal static string ITupleGetLengthShouldReturnInt32 => SR.ITupleGetLengthShouldReturnInt32;

        /// <summary>
        /// A string like "The 'GetItemMethod' of an 'ITuple' pattern should return an object of type Object."
        /// </summary>
        internal static string ITupleGetItemShouldReturnObject => SR.ITupleGetItemShouldReturnObject;

        /// <summary>
        /// A string like "The 'ITuple' positional subpattern with index '{0}' cannot have a field specified."
        /// </summary>
        internal static string ITuplePositionalPatternCannotHaveField(object? p0) => SR.Format(SR.ITuplePositionalPatternCannotHaveField, p0);

        /// <summary>
        /// A string like "The 'ITuple' positional subpattern with index '{0}' cannot have a parameter specified."
        /// </summary>
        internal static string ITuplePositionalPatternCannotHaveParameter(object? p0) => SR.Format(SR.ITuplePositionalPatternCannotHaveParameter, p0);

        /// <summary>
        /// A string like "The 'ITuple' positional subpattern with index '{0}' has type '{1}'. Only type Object is supported."
        /// </summary>
        internal static string ITuplePositionalPatternInvalidInputType(object? p0, object? p1) => SR.Format(SR.ITuplePositionalPatternInvalidInputType, p0, p1);

        /// <summary>
        /// A string like "A positional pattern should either be applied to a tuple type or provide a Deconstruct method."
        /// </summary>
        internal static string InvalidPositionalPattern => SR.InvalidPositionalPattern;

        /// <summary>
        /// A string like "The number of positional subpatterns does not match the number of components in input type '{0}'."
        /// </summary>
        internal static string InvalidPositionalPatternCount(object? p0) => SR.Format(SR.InvalidPositionalPatternCount, p0);

        /// <summary>
        /// A string like "Deconstruct method '{0}' should return void."
        /// </summary>
        internal static string DeconstructShouldReturnVoid(object? p0) => SR.Format(SR.DeconstructShouldReturnVoid, p0);

        /// <summary>
        /// A string like "Parameter '{0}' on Deconstruct method '{1}' should be an out parameter."
        /// </summary>
        internal static string DeconstructParameterShouldBeOut(object? p0, object? p1) => SR.Format(SR.DeconstructParameterShouldBeOut, p0, p1);

        /// <summary>
        /// A string like "Deconstruct method '{0}' should have at least one parameter."
        /// </summary>
        internal static string DeconstructExtensionMethodMissingThis(object? p0) => SR.Format(SR.DeconstructExtensionMethodMissingThis, p0);

        /// <summary>
        /// A string like "A tuple field index must be positive."
        /// </summary>
        internal static string TupleFieldIndexMustBePositive => SR.TupleFieldIndexMustBePositive;

        /// <summary>
        /// A string like "The '{0}' parameter must be declared on a method used for deconstruction."
        /// </summary>
        internal static string PositionalPatternParameterMustBeOnMethod(object? p0) => SR.Format(SR.PositionalPatternParameterMustBeOnMethod, p0);

        /// <summary>
        /// A string like "The '{0}' parameter must be an out parameter used for deconstruction."
        /// </summary>
        internal static string PositionalPatternParameterMustBeOut(object? p0) => SR.Format(SR.PositionalPatternParameterMustBeOut, p0);

        /// <summary>
        /// A string like "The property pattern member '{0}' should not be static."
        /// </summary>
        internal static string PropertyPatternMemberShouldNotBeStatic(object? p0) => SR.Format(SR.PropertyPatternMemberShouldNotBeStatic, p0);

        /// <summary>
        /// A string like "The property pattern member '{0}' should be readable."
        /// </summary>
        internal static string PropertyPatternMemberShouldBeReadable(object? p0) => SR.Format(SR.PropertyPatternMemberShouldBeReadable, p0);

        /// <summary>
        /// A string like "The property pattern member '{0}' should not be an indexer property."
        /// </summary>
        internal static string PropertyPatternMemberShouldNotBeIndexer(object? p0) => SR.Format(SR.PropertyPatternMemberShouldNotBeIndexer, p0);

        /// <summary>
        /// A string like "The property pattern member '{0}' is not compatible with a receiver of type '{1}'."
        /// </summary>
        internal static string PropertyPatternMemberIsNotCompatibleWithReceiver(object? p0, object? p1) => SR.Format(SR.PropertyPatternMemberIsNotCompatibleWithReceiver, p0, p1);

        /// <summary>
        /// A string like "A positional pattern using a Deconstruct method cannot specify a tuple field."
        /// </summary>
        internal static string PositionalPatternWithDeconstructMethodCannotSpecifyField => SR.PositionalPatternWithDeconstructMethodCannotSpecifyField;

        /// <summary>
        /// A string like "The '{0}' parameter is not declared on the '{1}' method."
        /// </summary>
        internal static string PositionalPatternParameterIsNotDeclaredOnDeconstructMethod(object? p0, object? p1) => SR.Format(SR.PositionalPatternParameterIsNotDeclaredOnDeconstructMethod, p0, p1);

        /// <summary>
        /// A string like "The '{0}' parameter is used more than once."
        /// </summary>
        internal static string PositionalPatternParameterShouldOnlyBeUsedOnce(object? p0) => SR.Format(SR.PositionalPatternParameterShouldOnlyBeUsedOnce, p0);

        /// <summary>
        /// A string like "Either all or none of the Deconstruct method parameters should be specified."
        /// </summary>
        internal static string PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters => SR.PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters;

        /// <summary>
        /// A string like "A positional pattern for a tuple type cannot specify a Deconstruct method parameter."
        /// </summary>
        internal static string PositionalPatternWithTupleCannotSpecifyParameter => SR.PositionalPatternWithTupleCannotSpecifyParameter;

        /// <summary>
        /// A string like "The tuple field index '{0}' is out of range for a tuple of cardinality '{1}'."
        /// </summary>
        internal static string PositionalPatternTupleIndexOutOfRange(object? p0, object? p1) => SR.Format(SR.PositionalPatternTupleIndexOutOfRange, p0, p1);

        /// <summary>
        /// A string like "The tuple field index '{0}' is used more than once."
        /// </summary>
        internal static string PositionalPatternTupleIndexShouldOnlyBeUsedOnce(object? p0) => SR.Format(SR.PositionalPatternTupleIndexShouldOnlyBeUsedOnce, p0);

        /// <summary>
        /// A string like "Either all or none of the tuple fields should be specified."
        /// </summary>
        internal static string PositionalPatternWithTupleShouldSpecifyAllIndices => SR.PositionalPatternWithTupleShouldSpecifyAllIndices;

        /// <summary>
        /// A string like "The type of a switch expression should not be void."
        /// </summary>
        internal static string SwitchExpressionTypeShouldNotBeVoid => SR.SwitchExpressionTypeShouldNotBeVoid;

        /// <summary>
        /// A string like "The switch expression arm at index '{0}' has a pattern input type '{1}' which is not compatible with the switch expression input type '{2}'."
        /// </summary>
        internal static string SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput(object? p0, object? p1, object? p2) => SR.Format(SR.SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput, p0, p1, p2);

        /// <summary>
        /// A string like "The switch expression arm at index '{0}' has a value of type '{1}' which is not compatible with the switch expression result type '{2}'."
        /// </summary>
        internal static string SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult(object? p0, object? p1, object? p2) => SR.Format(SR.SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult, p0, p1, p2);

        /// <summary>
        /// A string like "The type of a when clause should be Boolean."
        /// </summary>
        internal static string WhenClauseShouldBeBoolean => SR.WhenClauseShouldBeBoolean;

        /// <summary>
        /// A string like "The value of a switch expression arm should not be void."
        /// </summary>
        internal static string SwitchExpressionArmValueShouldNotBeVoid => SR.SwitchExpressionArmValueShouldNotBeVoid;

        /// <summary>
        /// A string like "A conversion cannot return void."
        /// </summary>
        internal static string ConversionCannotReturnVoid => SR.ConversionCannotReturnVoid;

        /// <summary>
        /// A string like "A conversion lambda expression should have a single parameter."
        /// </summary>
        internal static string ConversionShouldHaveOneParameter => SR.ConversionShouldHaveOneParameter;

        /// <summary>
        /// A string like "A deconstruction lambda expression should return void."
        /// </summary>
        internal static string DeconstructionShouldReturnVoid => SR.DeconstructionShouldReturnVoid;

        /// <summary>
        /// A string like "A deconstruction lambda expression should have at least three parameters, i.e. one for the input, and at least two for components returned by the deconstruction."
        /// </summary>
        internal static string DeconstructionShouldHaveThreeOrMoreParameters => SR.DeconstructionShouldHaveThreeOrMoreParameters;

        /// <summary>
        /// A string like "The deconstruction lambda expression parameter at position '{0}' represents an output of the deconstruction and should be passed by reference."
        /// </summary>
        internal static string DeconstructionParameterShouldBeByRef(object? p0) => SR.Format(SR.DeconstructionParameterShouldBeByRef, p0);

        /// <summary>
        /// A string like "The number of deconstruction output parameter should match the number of elements conversions."
        /// </summary>
        internal static string DeconstructionParameterCountShouldMatchConversionCount => SR.DeconstructionParameterCountShouldMatchConversionCount;

        /// <summary>
        /// A string like "The deconstruction output parameter at index '{0}' of type '{1}' is not assignable to the corresponding element conversion's input type '{2}'."
        /// </summary>
        internal static string DeconstructionParameterNotAssignableToConversion(object? p0, object? p1, object? p2) => SR.Format(SR.DeconstructionParameterNotAssignableToConversion, p0, p1, p2);

        /// <summary>
        /// A string like "The left hand side and the deconstructing conversion of the assignment do not match structurally at depth '{0}' and component '{1}'."
        /// </summary>
        internal static string DeconstructingAssignmentStructureMismatch(object? p0, object? p1) => SR.Format(SR.DeconstructingAssignmentStructureMismatch, p0, p1);

        /// <summary>
        /// A string like "The computed result tuple type '{0}' does not match the specified expression type '{1}'."
        /// </summary>
        internal static string DeconstructingAssignmentTypeMismatch(object? p0, object? p1) => SR.Format(SR.DeconstructingAssignmentTypeMismatch, p0, p1);

        /// <summary>
        /// A string like "The left hand side of type '{0}' and the right hand side of type '{1}' are not assignment compatible in the deconstruction assignment at depth '{2}' and component '{3}'."
        /// </summary>
        internal static string DeconstructingComponentAndConversionIncompatible(object? p0, object? p1, object? p2, object? p3) => SR.Format(SR.DeconstructingComponentAndConversionIncompatible, p0, p1, p2, p3);

        /// <summary>
        /// A string like "A using statement should either have a single expression or a declaration list."
        /// </summary>
        internal static string InvalidUsingStatement => SR.InvalidUsingStatement;

        /// <summary>
        /// A string like "All variables declared in a using statement should have the same type."
        /// </summary>
        internal static string UsingVariableDeclarationsShouldBeConsistentlyTyped => SR.UsingVariableDeclarationsShouldBeConsistentlyTyped;

        /// <summary>
        /// A string like "The variable '{0}' specified in the local declaration should be explicitly included in the variables of the using statement."
        /// </summary>
        internal static string UsingVariableNotInScope(object? p0) => SR.Format(SR.UsingVariableNotInScope, p0);

        /// <summary>
        /// A string like "The Dispose method of a using statement should return void."
        /// </summary>
        internal static string UsingDisposeShouldReturnVoid => SR.UsingDisposeShouldReturnVoid;

        /// <summary>
        /// A string like "A pattern dispose lambda for a using statement should have one parameter."
        /// </summary>
        internal static string UsingPatternDisposeShouldHaveOneParameter => SR.UsingPatternDisposeShouldHaveOneParameter;

        /// <summary>
        /// A string like "The input type '{0}' of the pattern dispose lambda is not compatible with the resource type '{1}' of the using statement."
        /// </summary>
        internal static string UsingPatternDisposeInputNotCompatibleWithResource(object? p0, object? p1) => SR.Format(SR.UsingPatternDisposeInputNotCompatibleWithResource, p0, p1);

        /// <summary>
        /// A string like "The variable '{0}' specified in the catch block should be explicitly included in the variables of the catch block."
        /// </summary>
        internal static string CatchVariableNotInScope(object? p0) => SR.Format(SR.CatchVariableNotInScope, p0);

        /// <summary>
        /// A string like "The catch block exception type '{0}' is not equivalent to the variable type '{1}'."
        /// </summary>
        internal static string CatchTypeNotEquivalentWithVariableType(object? p0, object? p1) => SR.Format(SR.CatchTypeNotEquivalentWithVariableType, p0, p1);

        /// <summary>
        /// A string like "The label '{0}' is used in multiple switch sections."
        /// </summary>
        internal static string DuplicateLabelInSwitchStatement(object? p0) => SR.Format(SR.DuplicateLabelInSwitchStatement, p0);

        /// <summary>
        /// A string like "The label '{0}' is used more than once in the switch section."
        /// </summary>
        internal static string DuplicateLabelInSwitchSection(object? p0) => SR.Format(SR.DuplicateLabelInSwitchSection, p0);

        /// <summary>
        /// A string like "The pattern input type '{0}' is not compatible with the switch value type '{1}'."
        /// </summary>
        internal static string SwitchValueTypeDoesNotMatchPatternInputType(object? p0, object? p1) => SR.Format(SR.SwitchValueTypeDoesNotMatchPatternInputType, p0, p1);

        /// <summary>
        /// A string like "The pattern input type '{0}' is not consistent with other pattern input types '{1}' in the same switch section."
        /// </summary>
        internal static string InconsistentPatternInputType(object? p0, object? p1) => SR.Format(SR.InconsistentPatternInputType, p0, p1);

        /// <summary>
        /// A string like "A switch statement should contain at most one default case."
        /// </summary>
        internal static string FoundMoreThanOneDefaultLabel => SR.FoundMoreThanOneDefaultLabel;

        /// <summary>
        /// A string like "A string interpolation format string cannot be empty."
        /// </summary>
        internal static string EmptyFormatSpecifier => SR.EmptyFormatSpecifier;

        /// <summary>
        /// A string like "A list pattern can have at most one slice subpattern."
        /// </summary>
        internal static string MoreThanOneSlicePattern => SR.MoreThanOneSlicePattern;

        /// <summary>
        /// A string like "The type returned by the index access expression cannot be void."
        /// </summary>
        internal static string ElementTypeCannotBeVoid => SR.ElementTypeCannotBeVoid;

        /// <summary>
        /// A string like "The length access lambda expression should have a single parameter."
        /// </summary>
        internal static string LengthAccessShouldHaveOneParameter => SR.LengthAccessShouldHaveOneParameter;

        /// <summary>
        /// A string like "The length access lambda expression should have an 'Int32' return type."
        /// </summary>
        internal static string LengthAccessShouldReturnInt32 => SR.LengthAccessShouldReturnInt32;

        /// <summary>
        /// A string like "The indexer access lambda expression should have two parameters."
        /// </summary>
        internal static string IndexerAccessShouldHaveTwoParameters => SR.IndexerAccessShouldHaveTwoParameters;

        /// <summary>
        /// A string like "The parameter of the length access lambda expression should match the collection type '{0}'."
        /// </summary>
        internal static string LengthAccessParameterShouldHaveCollectionType(object? p0) => SR.Format(SR.LengthAccessParameterShouldHaveCollectionType, p0);

        /// <summary>
        /// A string like "The first parameter of the indexer access lambda expression should match the collection type '{0}'."
        /// </summary>
        internal static string IndexerAccessFirstParameterShouldHaveCollectionType(object? p0) => SR.Format(SR.IndexerAccessFirstParameterShouldHaveCollectionType, p0);

        /// <summary>
        /// A string like "The second parameter of the indexer access lambda expression should be of type '{0}'."
        /// </summary>
        internal static string IndexerAccessSecondParameterInvalidType(object? p0) => SR.Format(SR.IndexerAccessSecondParameterInvalidType, p0);

        /// <summary>
        /// A string like "The non-nullable list pattern input type '{0}' should match collection type '{1}'."
        /// </summary>
        internal static string ListPatternInputTypeInvalid(object? p0, object? p1) => SR.Format(SR.ListPatternInputTypeInvalid, p0, p1);

        /// <summary>
        /// A string like "A foreach statement requires at least one iteration variable."
        /// </summary>
        internal static string ForEachNeedsOneOrMoreVariables => SR.ForEachNeedsOneOrMoreVariables;

        /// <summary>
        /// A string like "The collection type '{0}' is not compatible with the type '{1}' of the collection expression."
        /// </summary>
        internal static string ForEachCollectionTypeNotCompatibleWithCollectionExpression(object? p0, object? p1) => SR.Format(SR.ForEachCollectionTypeNotCompatibleWithCollectionExpression, p0, p1);

        /// <summary>
        /// A string like "A foreach statement with a deconstruction step requires more than one iteration variable."
        /// </summary>
        internal static string ForEachDeconstructionNotSupportedWithOneVariable => SR.ForEachDeconstructionNotSupportedWithOneVariable;

        /// <summary>
        /// A string like "A foreach statement with more than one iteration variables requires a deconstruction step."
        /// </summary>
        internal static string ForEachDeconstructionRequiredForMultipleVariables => SR.ForEachDeconstructionRequiredForMultipleVariables;

        /// <summary>
        /// A string like "The deconstruction lambda expression for a foreach statement should have one parameter."
        /// </summary>
        internal static string ForEachDeconstructionShouldHaveOneParameter => SR.ForEachDeconstructionShouldHaveOneParameter;

        /// <summary>
        /// A string like "The type '{0}' returned by the deconstruction lambda expression is not a tuple type."
        /// </summary>
        internal static string ForEachDeconstructionShouldReturnTuple(object? p0) => SR.Format(SR.ForEachDeconstructionShouldReturnTuple, p0);

        /// <summary>
        /// A string like "The tuple type '{0}' returned by the deconstruction lambda expression has an arity '{1}' that does not match the number of iteration variables."
        /// </summary>
        internal static string ForEachDeconstructionComponentMismatch(object? p0, object? p1) => SR.Format(SR.ForEachDeconstructionComponentMismatch, p0, p1);

        /// <summary>
        /// A string like "The type '{0}' of the tuple component at index '{1}' returned by the deconstruction lambda cannot be assigned to variable '{2}' of type '{3}'."
        /// </summary>
        internal static string ForEachDeconstructionComponentNotAssignableToVariable(object? p0, object? p1, object? p2, object? p3) => SR.Format(SR.ForEachDeconstructionComponentNotAssignableToVariable, p0, p1, p2, p3);

        /// <summary>
        /// A string like "The method '{1}' on type '{0}' is not an event accessor."
        /// </summary>
        internal static string MethodNotEventAccessor(object? p0, object? p1) => SR.Format(SR.MethodNotEventAccessor, p0, p1);

        /// <summary>
        /// A string like "The event '{0}' does not have an accessor."
        /// </summary>
        internal static string EventDoesNotHaveAccessor(object? p0) => SR.Format(SR.EventDoesNotHaveAccessor, p0);

        /// <summary>
        /// A string like "Only static events have an object expression."
        /// </summary>
        internal static string OnlyStaticEventsHaveNullInstance => SR.OnlyStaticEventsHaveNullInstance;

        /// <summary>
        /// A string like "The event '{0}' is not declared on type '{1}'."
        /// </summary>
        internal static string EventNotDefinedForType(object? p0, object? p1) => SR.Format(SR.EventNotDefinedForType, p0, p1);

        /// <summary>
        /// A string like "An event accessor method should return void."
        /// </summary>
        internal static string EventAccessorShouldReturnVoid => SR.EventAccessorShouldReturnVoid;

        /// <summary>
        /// A string like "An event accessor method should have one parameter."
        /// </summary>
        internal static string EventAccessorShouldHaveOneParameter => SR.EventAccessorShouldHaveOneParameter;

        /// <summary>
        /// A string like "The handler expression type '{0}' is not assignable to the event accessor parameter of type '{1}'."
        /// </summary>
        internal static string EventAccessorParameterTypeMismatch(object? p0, object? p1) => SR.Format(SR.EventAccessorParameterTypeMismatch, p0, p1);

        /// <summary>
        /// A string like "The '{0}' is not a valid interpolated string handler type."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerType(object? p0) => SR.Format(SR.InvalidInterpolatedStringHandlerType, p0);

        /// <summary>
        /// A string like "The construction lambda return type '{0}' is not assignable to interpolated string handler type '{1}'."
        /// </summary>
        internal static string InterpolatedStringHandlerTypeNotAssignable(object? p0, object? p1) => SR.Format(SR.InterpolatedStringHandlerTypeNotAssignable, p0, p1);

        /// <summary>
        /// A string like "An interpolated string handler construction should have at least two parameters for 'literalLength' and 'formattedCount'."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerConstructionArgCount => SR.InvalidInterpolatedStringHandlerConstructionArgCount;

        /// <summary>
        /// A string like "The '{0}' parameter representing '{1}' should be of type Int32."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerInt32ParameterType(object? p0, object? p1) => SR.Format(SR.InvalidInterpolatedStringHandlerInt32ParameterType, p0, p1);

        /// <summary>
        /// A string like "The type '{0}' is not a valid return type for an append call. Only 'void' and 'bool' are supported."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerAppendReturnType(object? p0) => SR.Format(SR.InvalidInterpolatedStringHandlerAppendReturnType, p0);

        /// <summary>
        /// A string like "The return types of the append calls is inconsistent."
        /// </summary>
        internal static string InconsistentInterpolatedStringHandlerAppendReturnType => SR.InconsistentInterpolatedStringHandlerAppendReturnType;

        /// <summary>
        /// A string like "An interpolated string handler append call should have at least one parameter for the handler instance."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerAppendArgCount => SR.InvalidInterpolatedStringHandlerAppendArgCount;

        /// <summary>
        /// A string like "The type '{0}' of the first parameter of the interpolated string handler append call is not compatible with the interpolated string handler type '{1}'."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerAppendFirstArgType(object? p0, object? p1) => SR.Format(SR.InvalidInterpolatedStringHandlerAppendFirstArgType, p0, p1);

        /// <summary>
        /// A string like "The type '{0}' is not valid for an interpolated string handler conversion."
        /// </summary>
        internal static string InvalidStringHandlerConversionOperandType(object? p0) => SR.Format(SR.InvalidStringHandlerConversionOperandType, p0);

        /// <summary>
        /// A string like "The node of type '{0}' is not valid as the operand for an interpolated string handler conversion."
        /// </summary>
        internal static string InvalidStringHandlerConversionOperandNodeType(object? p0) => SR.Format(SR.InvalidStringHandlerConversionOperandNodeType, p0);

        /// <summary>
        /// A string like "The argument index '{0}' is not valid."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerArgumentIndex(object? p0) => SR.Format(SR.InvalidInterpolatedStringHandlerArgumentIndex, p0);

        /// <summary>
        /// A string like "The number of parameters '{0}' for the interpolated string handler construction is insufficient for an argument count of '{1}' (need at least 'literalLength' and 'formattedCount')."
        /// </summary>
        internal static string NotEnoughInterpolatedStringHandlerConstructionParameters(object? p0, object? p1) => SR.Format(SR.NotEnoughInterpolatedStringHandlerConstructionParameters, p0, p1);

        /// <summary>
        /// A string like "The number of parameters '{0}' for the interpolated string handler construction is too large for an argument count of '{1}' (can have at most one extra 'out bool' parameter)."
        /// </summary>
        internal static string TooManyInterpolatedStringHandlerConstructionParameters(object? p0, object? p1) => SR.Format(SR.TooManyInterpolatedStringHandlerConstructionParameters, p0, p1);

        /// <summary>
        /// A string like "The last parameter of type '{0}' for the interpolated string handler construction is not valid for an 'out bool shouldAppend' trailing parameter."
        /// </summary>
        internal static string InvalidInterpolatedStringHandlerConstructionOutBoolParameter(object? p0) => SR.Format(SR.InvalidInterpolatedStringHandlerConstructionOutBoolParameter, p0);

        /// <summary>
        /// A string like "The append lambda expression's first parameter '{0}' denoting the interpolated string handler should be passed by reference."
        /// </summary>
        internal static string AppendLambdaShouldHaveFirstByRefParameter(object? p0) => SR.Format(SR.AppendLambdaShouldHaveFirstByRefParameter, p0);

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendLiteral' operation should take 2 parameters."
        /// </summary>
        internal static string AppendLiteralLambdaShouldHaveTwoParameters => SR.AppendLiteralLambdaShouldHaveTwoParameters;

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendLiteral' operation has a second parameter of type '{0}' which is invalid and should be 'string'."
        /// </summary>
        internal static string AppendLiteralLambdaShouldTakeStringParameter(object? p0) => SR.Format(SR.AppendLiteralLambdaShouldTakeStringParameter, p0);

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendFormatted' operation should take 2, 3, or 4 parameters."
        /// </summary>
        internal static string AppendFormattedLambdaInvalidParameterCount => SR.AppendFormattedLambdaInvalidParameterCount;

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendFormatted' operation should have a second parameter denoting a non-void value."
        /// </summary>
        internal static string AppendFormattedLambdaSecondParameterShouldBeNonVoid => SR.AppendFormattedLambdaSecondParameterShouldBeNonVoid;

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment or 'string' to denote a format."
        /// </summary>
        internal static string AppendFormattedLambdaThirdParameterShouldBeIntOrString(object? p0) => SR.Format(SR.AppendFormattedLambdaThirdParameterShouldBeIntOrString, p0);

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment."
        /// </summary>
        internal static string AppendFormattedLambdaThirdParameterShouldBeInt(object? p0) => SR.Format(SR.AppendFormattedLambdaThirdParameterShouldBeInt, p0);

        /// <summary>
        /// A string like "The lambda expression representing the 'AppendFormatted' operation has a fourth parameter of type '{0}' and should be 'string' to denote a format."
        /// </summary>
        internal static string AppendFormattedLambdaFourthParameterShouldBeString(object? p0) => SR.Format(SR.AppendFormattedLambdaFourthParameterShouldBeString, p0);

        /// <summary>
        /// A string like "The number of append operations '{0}' does not match the number of interpolations '{1}' in the interpolated string operand."
        /// </summary>
        internal static string IncorrectNumberOfAppendsForInterpolatedString(object? p0, object? p1) => SR.Format(SR.IncorrectNumberOfAppendsForInterpolatedString, p0, p1);

        /// <summary>
        /// A string like "The number of parameters '{0}' for the 'AppendFormatted' operation does not match the expected number '{1}' for the interpolated string's interpolation."
        /// </summary>
        internal static string InvalidAppendFormattedParameterCount(object? p0, object? p1) => SR.Format(SR.InvalidAppendFormattedParameterCount, p0, p1);

        /// <summary>
        /// A string like "The type '{0}' of the 'value' parameter for the 'AppendFormatted' operation is not compatible with the expected type '{1}' for the interpolated string's interpolation value."
        /// </summary>
        internal static string InvalidAppendFormattedValueType(object? p0, object? p1) => SR.Format(SR.InvalidAppendFormattedValueType, p0, p1);

        /// <summary>
        /// A string like "The type '{0}' is invalid for the alignment parameter and should be 'int'."
        /// </summary>
        internal static string InvalidAlignmentParameterType(object? p0) => SR.Format(SR.InvalidAlignmentParameterType, p0);

        /// <summary>
        /// A string like "The type '{0}' is invalid for the format parameter and should be 'string'."
        /// </summary>
        internal static string InvalidFormatParameterType(object? p0) => SR.Format(SR.InvalidFormatParameterType, p0);

        /// <summary>
        /// A string like "The 'getEnumerator' lambda should have a single parameter."
        /// </summary>
        internal static string GetEnumeratorShouldHaveSingleParameter => SR.GetEnumeratorShouldHaveSingleParameter;

        /// <summary>
        /// A string like "The type '{0}' of the first parameter of the 'getEnumerator' lambda is not compatible with the collection type '{1}'."
        /// </summary>
        internal static string InvalidGetEnumeratorFirstArgType(object? p0, object? p1) => SR.Format(SR.InvalidGetEnumeratorFirstArgType, p0, p1);

        /// <summary>
        /// A string like "The 'moveNext' lambda should have a single parameter."
        /// </summary>
        internal static string MoveNextShouldHaveSingleParameter => SR.MoveNextShouldHaveSingleParameter;

        /// <summary>
        /// A string like "The type '{0}' of the first parameter of the 'moveNext' lambda is not compatible with the enumerator type '{1}'."
        /// </summary>
        internal static string InvalidMoveNextFirstArgType(object? p0, object? p1) => SR.Format(SR.InvalidMoveNextFirstArgType, p0, p1);

        /// <summary>
        /// A string like "The 'moveNext' lambda should return 'bool'."
        /// </summary>
        internal static string MoveNextShouldHaveBooleanReturnType => SR.MoveNextShouldHaveBooleanReturnType;

        /// <summary>
        /// A string like "The '{0}' property should not have indexer parameters."
        /// </summary>
        internal static string PropertyShouldNotBeIndexer(object? p0) => SR.Format(SR.PropertyShouldNotBeIndexer, p0);

        /// <summary>
        /// A string like "The '{0}' property should not have a 'void' type."
        /// </summary>
        internal static string PropertyShouldNotReturnVoid(object? p0) => SR.Format(SR.PropertyShouldNotReturnVoid, p0);

        /// <summary>
        /// A string like "The 'currentConversion' lambda should have a single parameter."
        /// </summary>
        internal static string CurrentConversionShouldHaveSingleParameter => SR.CurrentConversionShouldHaveSingleParameter;

        /// <summary>
        /// A string like "The type '{0}' of the first parameter of the 'currentConversion' lambda is not compatible with the collection type '{1}'."
        /// </summary>
        internal static string InvalidCurrentConversionFirstArgType(object? p0, object? p1) => SR.Format(SR.InvalidCurrentConversionFirstArgType, p0, p1);

        /// <summary>
        /// A string like "The element type '{0}' is not compatible with the type '{1}' returned by the 'Current' property."
        /// </summary>
        internal static string InvalidCurrentReturnType(object? p0, object? p1) => SR.Format(SR.InvalidCurrentReturnType, p0, p1);

        /// <summary>
        /// A string like "Asynchronous enumeration is not supported for array types."
        /// </summary>
        internal static string AsyncEnumerationNotSupportedForArray => SR.AsyncEnumerationNotSupportedForArray;

        /// <summary>
        /// A string like "Asynchronous enumeration is not supported on type 'String'."
        /// </summary>
        internal static string AsyncEnumerationNotSupportedForString => SR.AsyncEnumerationNotSupportedForString;

        /// <summary>
        /// A string like "The '{0}.{1}' method is ambiguous."
        /// </summary>
        internal static string AmbiguousEnumeratorMethod(object? p0, object? p1) => SR.Format(SR.AmbiguousEnumeratorMethod, p0, p1);

        /// <summary>
        /// A string like "List pattern should have a collection type or a variable."
        /// </summary>
        internal static string ListPatternShouldHaveCollectionTypeOrVariable => SR.ListPatternShouldHaveCollectionTypeOrVariable;

    }
}

namespace System
{
    internal static partial class SR
    {
        public const string ParameterNotDefinedForMethod = "Parameter '{0}' is not defined for method '{1}'.";
        public const string ParameterIndexOutOfBounds = "Parameter index '{0}' is out of bounds for method '{1}'.";
        public const string ExpressionTypeDoesNotMatchParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}'.";
        public const string DuplicateParameterBinding = "Parameter '{0}' has multiple bindings.";
        public const string UnboundParameter = "Non-optional parameter '{0}' has no binding.";
        public const string NonStaticConstructorRequired = "A non-static constructor is required.";
        public const string PropertyDoesNotHaveGetAccessor = "The property '{0}' has no 'get' accessor.";
        public const string PropertyDoesNotHaveSetAccessor = "The property '{0}' has no 'set' accessor.";
        public const string AccessorCannotBeStatic = "A non-static 'get' accessor is required for property '{0}'.";
        public const string RankMismatch = "The number of indexes specified does not match the array rank.";
        public const string IndexOutOfRange = "The specified index is out of range.";
        public const string BoundCannotBeLessThanZero = "An array dimension cannot be less than 0.";
        public const string ArrayBoundsElementCountMismatch = "The number of elements does not match the length of the array.";
        public const string GetAwaiterShouldTakeZeroParameters = "The 'GetAwaiter' method should take zero parameters.";
        public const string GetAwaiterShouldNotBeGeneric = "The 'GetAwaiter' method should not be generic.";
        public const string GetAwaiterShouldReturnAwaiterType = "The 'GetAwaiter' method has an unsupported return type.";
        public const string AwaitableTypeShouldHaveGetAwaiterMethod = "Awaitable type '{0}' should have a 'GetAwaiter' method.";
        public const string AwaiterTypeShouldImplementINotifyCompletion = "Awaiter type '{0}' should implement 'INotifyCompletion'.";
        public const string AwaiterTypeShouldHaveIsCompletedProperty = "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor.";
        public const string AwaiterIsCompletedShouldReturnBool = "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'.";
        public const string AwaiterIsCompletedShouldNotBeIndexer = "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters.";
        public const string AwaiterTypeShouldHaveGetResultMethod = "Awaiter type '{0}' should have a 'GetResult' method.";
        public const string AwaiterGetResultTypeInvalid = "The 'GetResult' method on awaiter type '{0}' has an unsupported return type.";
        public const string DynamicAwaitNoGetAwaiter = "Dynamically bound await operations cannot have a 'GetAwaiter' expression.";
        public const string DynamicAwaitNoIsCompleted = "Dynamically bound await operations cannot have an 'IsCompleted' property.";
        public const string DynamicAwaitNoGetResult = "Dynamically bound await operations cannot have a 'GetResult' method.";
        public const string GetAwaiterExpressionOneParameter = "The 'GetAwaiter' expression should have one parameter.";
        public const string AsyncLambdaCantHaveByRefParameter = "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions.";
        public const string AsyncLambdaInvalidReturnType = "Return type '{0}' is not valid for an asynchronous lambda expression.";
        public const string AwaitForbiddenHere = "Await expression cannot occur in '{0}'.";
        public const string LockNeedsReferenceType = "An expression of type '{0}' can't be used as a lock.";
        public const string ConversionNeedsOneParameter = "The conversion lambda should have one parameter.";
        public const string ConversionInvalidArgument = "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'.";
        public const string ConversionInvalidResult = "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'.";
        public const string EnumeratorShouldHaveCurrentProperty = "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor.";
        public const string EnumeratorShouldHaveMoveNextMethod = "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type.";
        public const string MoreThanOneIEnumerableFound = "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'.";
        public const string NoEnumerablePattern = "Collection type '{0}' has no valid enumerable pattern.";
        public const string InvalidInitializer = "Initializers should be assignments to variables.";
        public const string DuplicateLabels = "Break and continue lables should be different.";
        public const string ConditionalAccessRequiresNonStaticMember = "Conditional access expressions require non-static members or extension methods.";
        public const string ConditionalAccessRequiresReadableProperty = "Conditional access expressions require readable properties.";
        public const string TooManyArguments = "Too many arguments have been specified.";
        public const string ExtensionMethodRequiresInstance = "Conditional call expressions for extensions methods should specify an instance expression.";
        public const string InvalidSwitchType = "Type '{0}' is not a valid governing type for a switch statement.";
        public const string DuplicateTestValue = "The test value '{0}' occurs more than once.";
        public const string SwitchCantHaveNullCase = "A 'null' test value cannot be used in a switch statement with governing type '{0}'.";
        public const string SwitchCaseHasIncompatibleType = "A test value with type '{0}' cannot be used in a swich statement with governing type '{1}'.";
        public const string TestValuesShouldHaveConsistentType = "All specified test values should have the same type.";
        public const string SwitchBreakLabelShouldBeVoid = "The break label of a switch statement should be of type 'void'.";
        public const string SwitchLabelTargetShouldBeVoid = "The label of a switch case should be of type 'void'.";
        public const string InvalidGotoCase = "A 'goto case {0}' statement was found but the containing switch statement has no such label.";
        public const string InvalidGotoDefault = "A 'goto default' statement was found but the containing switch statement has no default label.";
        public const string GotoCanOnlyBeReducedInSwitch = "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node.";
        public const string InvalidConditionalReceiverType = "Type '{0}' is not a valid type for a conditional receiver.";
        public const string InvalidConditionalReceiverExpressionType = "Type '{0}' is not a valid type for a receiver in a conditional access expression.";
        public const string ConditionalReceiverTypeMismatch = "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver.";
        public const string InvalidCompoundAssignment = "Compound assignment operation '{0}' is not supported for type '{1}'.";
        public const string InvalidCompoundAssignmentWithOperands = "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'.";
        public const string InvalidUnaryAssignmentWithOperands = "Unary assignment operation '{0}' is not supported for an operand of type '{1}'.";
        public const string TypeMustNotBeByRef = "Type must not be ByRef.";
        public const string TypeMustNotBePointer = "Type must not be a pointer type.";
        public const string InvalidNullCoalescingAssignmentArguments = "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations.";
        public const string InvalidInterpolatedStringType = "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable.";
        public const string InvalidFromEndIndexOperandType = "Type '{0}' is not a valid type for an index operand. Supported types are int or int?.";
        public const string InvalidFromEndIndexMethod = "The specified method is not valid to construct an object of type Index.";
        public const string InvalidIndexType = "Type '{0}' is not a valid index type. Supported types are Index or Index?.";
        public const string InvalidRangeOperandType = "Type '{0}' is not a valid type for a range operand. Supported types are Index or Index?.";
        public const string InvalidRangeMethod = "The specified method is not valid to construct an object of type Range.";
        public const string InvalidRangeType = "Type '{0}' is not a valid range type. Supported types are Range or Range?.";
        public const string InvalidIndexerAccessArgumentType = "Type '{0}' is not a valid type for an 'IndexerAccess' argument. Supported types are Index or Range.";
        public const string InvalidLengthOrCountPropertyType = "Property '{0}' should be of type int.";
        public const string InvalidIndexMember = "Member '{0}' is not a valid member for an indexer. Supported member types are MethodInfo or PropertyInfo.";
        public const string InvalidSliceMember = "Member '{0}' is not a valid member for a slice method.";
        public const string InvalidIndexerParameterType = "Indexer '{0}' does not have an 'int' parameter type.";
        public const string SliceMethodMustNotBeStatic = "Slice method '{0}' should be an instance method.";
        public const string InvalidSliceParameters = "Slice method '{0}' should be have exactly two parameters of type 'int'.";
        public const string InvalidTupleType = "Type '{0}' is not a valid tuple type.";
        public const string InvalidTupleArgumentCount = "The number of arguments does not match the number of components of tuple type '{0}'.";
        public const string InvalidTupleArgumentNamesCount = "The number of argument names does not match the number of components of tuple type '{0}'.";
        public const string TupleComponentCannotBeVoid = "The type of a tuple component cannot be void.";
        public const string TupleComponentCountMismatch = "The arity of tuple type '{0}' does not match the arity of tuple type '{1}'.";
        public const string InvalidElementConversionCount = "The number of element conversion expressions does not match tuple arity '{0}'.";
        public const string InvalidEqualityCheckCount = "The number of equality check expressions does not match tuple arity '{0}'.";
        public const string NotAMemberOfAnyType = "'{0}' is not a member of any type.";
        public const string WithExpressionCannotHaveCloneForValueType = "A 'with' expression for value type '{0}' cannot specify a 'Clone' method.";
        public const string WithExpressionShouldHaveClone = "A 'with' expression for type '{0}' should specify a 'Clone' method.";
        public const string CloneMethodShouldHaveNoParameters = "Clone method '{0}' should have no parameters.";
        public const string CloneMethodMustNotBeStatic = "Clone method '{0}' should be an instance method.";
        public const string CloneMethodShouldReturnCompatibleType = "Clone method '{0}' should return a type that can be converted to '{1}'.";
        public const string MemberInitializerMemberMustNotBeStatic = "Member '{0}' used in a 'MemberInitializer' cannot be static.";
        public const string MemberInitializerMemberMustNotBeIndexer = "Member '{0}' used in a 'MemberInitializer' cannot be an indexer.";
        public const string MemberInitializerMemberMustBeWriteable = "Member '{0}' used in a 'MemberInitializer' must be writeable.";
        public const string NoAnonymousTypeConstructorFound = "No suitable constructor found for type '{0}' using the specified members.";
        public const string CannotHaveNullablePatternType = "A pattern can never produce a value of a nullable type.";
        public const string PatternInputAndNarrowedTypeShouldMatch = "The input and narrowed type for a pattern of type '{0}' should be equal.";
        public const string CannotAssignPatternResultToVariable = "The variable type '{0}' should be equal to the pattern result type '{1}'.";
        public const string PatternTypeMismatch = "The pattern type '{0}' is not compatible with a subpattern type '{1}'.";
        public const string InvalidBinaryPatternType = "The pattern type '{0}' is not a valid binary pattern type.";
        public const string InvalidRelationalPatternType = "The pattern type '{0}' is not a valid relational pattern type.";
        public const string InvalidPatternConstantType = "The type '{0}' cannot be used for a constant in a pattern.";
        public const string InvalidRelationalPatternConstantType = "The type '{0}' cannot be used for a constant in a relational pattern.";
        public const string CannotUsePatternConstantNaN = "The value NaN cannot be used for a constant in a relational pattern.";
        public const string NullValueShouldUseObjectType = "A null pattern should use a constant of type object.";
        public const string CannotUseNullValueInRelationalPattern = "A null value cannot be used in a relational pattern.";
        public const string ITupleGetLengthShouldReturnInt32 = "The 'GetLengthMethod' of an 'ITuple' pattern should return an integer value.";
        public const string ITupleGetItemShouldReturnObject = "The 'GetItemMethod' of an 'ITuple' pattern should return an object of type Object.";
        public const string ITuplePositionalPatternCannotHaveField = "The 'ITuple' positional subpattern with index '{0}' cannot have a field specified.";
        public const string ITuplePositionalPatternCannotHaveParameter = "The 'ITuple' positional subpattern with index '{0}' cannot have a parameter specified.";
        public const string ITuplePositionalPatternInvalidInputType = "The 'ITuple' positional subpattern with index '{0}' has type '{1}'. Only type Object is supported.";
        public const string InvalidPositionalPattern = "A positional pattern should either be applied to a tuple type or provide a Deconstruct method.";
        public const string InvalidPositionalPatternCount = "The number of positional subpatterns does not match the number of components in input type '{0}'.";
        public const string DeconstructShouldReturnVoid = "Deconstruct method '{0}' should return void.";
        public const string DeconstructParameterShouldBeOut = "Parameter '{0}' on Deconstruct method '{1}' should be an out parameter.";
        public const string DeconstructExtensionMethodMissingThis = "Deconstruct method '{0}' should have at least one parameter.";
        public const string TupleFieldIndexMustBePositive = "A tuple field index must be positive.";
        public const string PositionalPatternParameterMustBeOnMethod = "The '{0}' parameter must be declared on a method used for deconstruction.";
        public const string PositionalPatternParameterMustBeOut = "The '{0}' parameter must be an out parameter used for deconstruction.";
        public const string PropertyPatternMemberShouldNotBeStatic = "The property pattern member '{0}' should not be static.";
        public const string PropertyPatternMemberShouldBeReadable = "The property pattern member '{0}' should be readable.";
        public const string PropertyPatternMemberShouldNotBeIndexer = "The property pattern member '{0}' should not be an indexer property.";
        public const string PropertyPatternMemberIsNotCompatibleWithReceiver = "The property pattern member '{0}' is not compatible with a receiver of type '{1}'.";
        public const string PositionalPatternWithDeconstructMethodCannotSpecifyField = "A positional pattern using a Deconstruct method cannot specify a tuple field.";
        public const string PositionalPatternParameterIsNotDeclaredOnDeconstructMethod = "The '{0}' parameter is not declared on the '{1}' method.";
        public const string PositionalPatternParameterShouldOnlyBeUsedOnce = "The '{0}' parameter is used more than once.";
        public const string PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters = "Either all or none of the Deconstruct method parameters should be specified.";
        public const string PositionalPatternWithTupleCannotSpecifyParameter = "A positional pattern for a tuple type cannot specify a Deconstruct method parameter.";
        public const string PositionalPatternTupleIndexOutOfRange = "The tuple field index '{0}' is out of range for a tuple of cardinality '{1}'.";
        public const string PositionalPatternTupleIndexShouldOnlyBeUsedOnce = "The tuple field index '{0}' is used more than once.";
        public const string PositionalPatternWithTupleShouldSpecifyAllIndices = "Either all or none of the tuple fields should be specified.";
        public const string SwitchExpressionTypeShouldNotBeVoid = "The type of a switch expression should not be void.";
        public const string SwitchExpressionArmPatternInputNotCompatibleWithSwitchExpressionInput = "The switch expression arm at index '{0}' has a pattern input type '{1}' which is not compatible with the switch expression input type '{2}'.";
        public const string SwitchExpressionArmValueNotCompatibleWithSwitchExpressionResult = "The switch expression arm at index '{0}' has a value of type '{1}' which is not compatible with the switch expression result type '{2}'.";
        public const string WhenClauseShouldBeBoolean = "The type of a when clause should be Boolean.";
        public const string SwitchExpressionArmValueShouldNotBeVoid = "The value of a switch expression arm should not be void.";
        public const string ConversionCannotReturnVoid = "A conversion cannot return void.";
        public const string ConversionShouldHaveOneParameter = "A conversion lambda expression should have a single parameter.";
        public const string DeconstructionShouldReturnVoid = "A deconstruction lambda expression should return void.";
        public const string DeconstructionShouldHaveThreeOrMoreParameters = "A deconstruction lambda expression should have at least three parameters, i.e. one for the input, and at least two for components returned by the deconstruction.";
        public const string DeconstructionParameterShouldBeByRef = "The deconstruction lambda expression parameter at position '{0}' represents an output of the deconstruction and should be passed by reference.";
        public const string DeconstructionParameterCountShouldMatchConversionCount = "The number of deconstruction output parameter should match the number of elements conversions.";
        public const string DeconstructionParameterNotAssignableToConversion = "The deconstruction output parameter at index '{0}' of type '{1}' is not assignable to the corresponding element conversion's input type '{2}'.";
        public const string DeconstructingAssignmentStructureMismatch = "The left hand side and the deconstructing conversion of the assignment do not match structurally at depth '{0}' and component '{1}'.";
        public const string DeconstructingAssignmentTypeMismatch = "The computed result tuple type '{0}' does not match the specified expression type '{1}'.";
        public const string DeconstructingComponentAndConversionIncompatible = "The left hand side of type '{0}' and the right hand side of type '{1}' are not assignment compatible in the deconstruction assignment at depth '{2}' and component '{3}'.";
        public const string InvalidUsingStatement = "A using statement should either have a single expression or a declaration list.";
        public const string UsingVariableDeclarationsShouldBeConsistentlyTyped = "All variables declared in a using statement should have the same type.";
        public const string UsingVariableNotInScope = "The variable '{0}' specified in the local declaration should be explicitly included in the variables of the using statement.";
        public const string UsingDisposeShouldReturnVoid = "The Dispose method of a using statement should return void.";
        public const string UsingPatternDisposeShouldHaveOneParameter = "A pattern dispose lambda for a using statement should have one parameter.";
        public const string UsingPatternDisposeInputNotCompatibleWithResource = "The input type '{0}' of the pattern dispose lambda is not compatible with the resource type '{1}' of the using statement.";
        public const string CatchVariableNotInScope = "The variable '{0}' specified in the catch block should be explicitly included in the variables of the catch block.";
        public const string CatchTypeNotEquivalentWithVariableType = "The catch block exception type '{0}' is not equivalent to the variable type '{1}'.";
        public const string DuplicateLabelInSwitchStatement = "The label '{0}' is used in multiple switch sections.";
        public const string DuplicateLabelInSwitchSection = "The label '{0}' is used more than once in the switch section.";
        public const string SwitchValueTypeDoesNotMatchPatternInputType = "The pattern input type '{0}' is not compatible with the switch value type '{1}'.";
        public const string InconsistentPatternInputType = "The pattern input type '{0}' is not consistent with other pattern input types '{1}' in the same switch section.";
        public const string FoundMoreThanOneDefaultLabel = "A switch statement should contain at most one default case.";
        public const string EmptyFormatSpecifier = "A string interpolation format string cannot be empty.";
        public const string MoreThanOneSlicePattern = "A list pattern can have at most one slice subpattern.";
        public const string ElementTypeCannotBeVoid = "The type returned by the index access expression cannot be void.";
        public const string LengthAccessShouldHaveOneParameter = "The length access lambda expression should have a single parameter.";
        public const string LengthAccessShouldReturnInt32 = "The length access lambda expression should have an 'Int32' return type.";
        public const string IndexerAccessShouldHaveTwoParameters = "The indexer access lambda expression should have two parameters.";
        public const string LengthAccessParameterShouldHaveCollectionType = "The parameter of the length access lambda expression should match the collection type '{0}'.";
        public const string IndexerAccessFirstParameterShouldHaveCollectionType = "The first parameter of the indexer access lambda expression should match the collection type '{0}'.";
        public const string IndexerAccessSecondParameterInvalidType = "The second parameter of the indexer access lambda expression should be of type '{0}'.";
        public const string ListPatternInputTypeInvalid = "The non-nullable list pattern input type '{0}' should match collection type '{1}'.";
        public const string ForEachNeedsOneOrMoreVariables = "A foreach statement requires at least one iteration variable.";
        public const string ForEachCollectionTypeNotCompatibleWithCollectionExpression = "The collection type '{0}' is not compatible with the type '{1}' of the collection expression.";
        public const string ForEachDeconstructionNotSupportedWithOneVariable = "A foreach statement with a deconstruction step requires more than one iteration variable.";
        public const string ForEachDeconstructionRequiredForMultipleVariables = "A foreach statement with more than one iteration variables requires a deconstruction step.";
        public const string ForEachDeconstructionShouldHaveOneParameter = "The deconstruction lambda expression for a foreach statement should have one parameter.";
        public const string ForEachDeconstructionShouldReturnTuple = "The type '{0}' returned by the deconstruction lambda expression is not a tuple type.";
        public const string ForEachDeconstructionComponentMismatch = "The tuple type '{0}' returned by the deconstruction lambda expression has an arity '{1}' that does not match the number of iteration variables.";
        public const string ForEachDeconstructionComponentNotAssignableToVariable = "The type '{0}' of the tuple component at index '{1}' returned by the deconstruction lambda cannot be assigned to variable '{2}' of type '{3}'.";
        public const string MethodNotEventAccessor = "The method '{1}' on type '{0}' is not an event accessor.";
        public const string EventDoesNotHaveAccessor = "The event '{0}' does not have an accessor.";
        public const string OnlyStaticEventsHaveNullInstance = "Only static events have an object expression.";
        public const string EventNotDefinedForType = "The event '{0}' is not declared on type '{1}'.";
        public const string EventAccessorShouldReturnVoid = "An event accessor method should return void.";
        public const string EventAccessorShouldHaveOneParameter = "An event accessor method should have one parameter.";
        public const string EventAccessorParameterTypeMismatch = "The handler expression type '{0}' is not assignable to the event accessor parameter of type '{1}'.";
        public const string InvalidInterpolatedStringHandlerType = "The '{0}' is not a valid interpolated string handler type.";
        public const string InterpolatedStringHandlerTypeNotAssignable = "The construction lambda return type '{0}' is not assignable to interpolated string handler type '{1}'.";
        public const string InvalidInterpolatedStringHandlerConstructionArgCount = "An interpolated string handler construction should have at least two parameters for 'literalLength' and 'formattedCount'.";
        public const string InvalidInterpolatedStringHandlerInt32ParameterType = "The '{0}' parameter representing '{1}' should be of type Int32.";
        public const string InvalidInterpolatedStringHandlerAppendReturnType = "The type '{0}' is not a valid return type for an append call. Only 'void' and 'bool' are supported.";
        public const string InconsistentInterpolatedStringHandlerAppendReturnType = "The return types of the append calls is inconsistent.";
        public const string InvalidInterpolatedStringHandlerAppendArgCount = "An interpolated string handler append call should have at least one parameter for the handler instance.";
        public const string InvalidInterpolatedStringHandlerAppendFirstArgType = "The type '{0}' of the first parameter of the interpolated string handler append call is not compatible with the interpolated string handler type '{1}'.";
        public const string InvalidStringHandlerConversionOperandType = "The type '{0}' is not valid for an interpolated string handler conversion.";
        public const string InvalidStringHandlerConversionOperandNodeType = "The node of type '{0}' is not valid as the operand for an interpolated string handler conversion.";
        public const string InvalidInterpolatedStringHandlerArgumentIndex = "The argument index '{0}' is not valid.";
        public const string NotEnoughInterpolatedStringHandlerConstructionParameters = "The number of parameters '{0}' for the interpolated string handler construction is insufficient for an argument count of '{1}' (need at least 'literalLength' and 'formattedCount').";
        public const string TooManyInterpolatedStringHandlerConstructionParameters = "The number of parameters '{0}' for the interpolated string handler construction is too large for an argument count of '{1}' (can have at most one extra 'out bool' parameter).";
        public const string InvalidInterpolatedStringHandlerConstructionOutBoolParameter = "The last parameter of type '{0}' for the interpolated string handler construction is not valid for an 'out bool shouldAppend' trailing parameter.";
        public const string AppendLambdaShouldHaveFirstByRefParameter = "The append lambda expression's first parameter '{0}' denoting the interpolated string handler should be passed by reference.";
        public const string AppendLiteralLambdaShouldHaveTwoParameters = "The lambda expression representing the 'AppendLiteral' operation should take 2 parameters.";
        public const string AppendLiteralLambdaShouldTakeStringParameter = "The lambda expression representing the 'AppendLiteral' operation has a second parameter of type '{0}' which is invalid and should be 'string'.";
        public const string AppendFormattedLambdaInvalidParameterCount = "The lambda expression representing the 'AppendFormatted' operation should take 2, 3, or 4 parameters.";
        public const string AppendFormattedLambdaSecondParameterShouldBeNonVoid = "The lambda expression representing the 'AppendFormatted' operation should have a second parameter denoting a non-void value.";
        public const string AppendFormattedLambdaThirdParameterShouldBeIntOrString = "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment or 'string' to denote a format.";
        public const string AppendFormattedLambdaThirdParameterShouldBeInt = "The lambda expression representing the 'AppendFormatted' operation has a third parameter of type '{0}' and should be 'int' to denote an alignment.";
        public const string AppendFormattedLambdaFourthParameterShouldBeString = "The lambda expression representing the 'AppendFormatted' operation has a fourth parameter of type '{0}' and should be 'string' to denote a format.";
        public const string IncorrectNumberOfAppendsForInterpolatedString = "The number of append operations '{0}' does not match the number of interpolations '{1}' in the interpolated string operand.";
        public const string InvalidAppendFormattedParameterCount = "The number of parameters '{0}' for the 'AppendFormatted' operation does not match the expected number '{1}' for the interpolated string's interpolation.";
        public const string InvalidAppendFormattedValueType = "The type '{0}' of the 'value' parameter for the 'AppendFormatted' operation is not compatible with the expected type '{1}' for the interpolated string's interpolation value.";
        public const string InvalidAlignmentParameterType = "The type '{0}' is invalid for the alignment parameter and should be 'int'.";
        public const string InvalidFormatParameterType = "The type '{0}' is invalid for the format parameter and should be 'string'.";
        public const string GetEnumeratorShouldHaveSingleParameter = "The 'getEnumerator' lambda should have a single parameter.";
        public const string InvalidGetEnumeratorFirstArgType = "The type '{0}' of the first parameter of the 'getEnumerator' lambda is not compatible with the collection type '{1}'.";
        public const string MoveNextShouldHaveSingleParameter = "The 'moveNext' lambda should have a single parameter.";
        public const string InvalidMoveNextFirstArgType = "The type '{0}' of the first parameter of the 'moveNext' lambda is not compatible with the enumerator type '{1}'.";
        public const string MoveNextShouldHaveBooleanReturnType = "The 'moveNext' lambda should return 'bool'.";
        public const string PropertyShouldNotBeIndexer = "The '{0}' property should not have indexer parameters.";
        public const string PropertyShouldNotReturnVoid = "The '{0}' property should not have a 'void' type.";
        public const string CurrentConversionShouldHaveSingleParameter = "The 'currentConversion' lambda should have a single parameter.";
        public const string InvalidCurrentConversionFirstArgType = "The type '{0}' of the first parameter of the 'currentConversion' lambda is not compatible with the collection type '{1}'.";
        public const string InvalidCurrentReturnType = "The element type '{0}' is not compatible with the type '{1}' returned by the 'Current' property.";
        public const string AsyncEnumerationNotSupportedForArray = "Asynchronous enumeration is not supported for array types.";
        public const string AsyncEnumerationNotSupportedForString = "Asynchronous enumeration is not supported on type 'String'.";
        public const string AmbiguousEnumeratorMethod = "The '{0}.{1}' method is ambiguous.";
        public const string ListPatternShouldHaveCollectionTypeOrVariable = "List pattern should have a collection type or a variable.";
    }
}