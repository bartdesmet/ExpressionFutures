// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' is not defined for method '{1}'"
        /// </summary>
        internal static Exception ParameterNotDefinedForMethod(object p0, object p1)
        {
            return new ArgumentException(Strings.ParameterNotDefinedForMethod(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Parameter index '{0}' is out of bounds for method '{1}'"
        /// </summary>
        internal static Exception ParameterIndexOutOfBounds(object p0, object p1)
        {
            return new ArgumentException(Strings.ParameterIndexOutOfBounds(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Expression of type '{0}' cannot be used for parameter of type '{1}'"
        /// </summary>
        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' has multiple bindings"
        /// </summary>
        internal static Exception DuplicateParameterBinding(object p0)
        {
            return new ArgumentException(Strings.DuplicateParameterBinding(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Non-optional parameter '{0}' has no binding"
        /// </summary>
        internal static Exception UnboundParameter(object p0)
        {
            return new ArgumentException(Strings.UnboundParameter(p0));
        }

        /// <summary>
        /// ArgumentException with message like "A non-static constructor is required"
        /// </summary>
        internal static Exception NonStaticConstructorRequired()
        {
            return new ArgumentException(Strings.NonStaticConstructorRequired);
        }

        /// <summary>
        /// ArgumentException with message like "The property '{0}' has no 'get' accessor"
        /// </summary>
        internal static Exception PropertyDoesNotHaveGetAccessor(object p0)
        {
            return new ArgumentException(Strings.PropertyDoesNotHaveGetAccessor(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The property '{0}' has no 'set' accessor"
        /// </summary>
        internal static Exception PropertyDoesNotHaveSetAccessor(object p0)
        {
            return new ArgumentException(Strings.PropertyDoesNotHaveSetAccessor(p0));
        }

        /// <summary>
        /// ArgumentException with message like "A non-static 'get' accessor is required for property '{0}'"
        /// </summary>
        internal static Exception AccessorCannotBeStatic(object p0)
        {
            return new ArgumentException(Strings.AccessorCannotBeStatic(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The number of indexes specified does not match the array rank"
        /// </summary>
        internal static Exception RankMismatch()
        {
            return new ArgumentException(Strings.RankMismatch);
        }

        /// <summary>
        /// ArgumentOutOfRangeException with message like "The specified index is out of range"
        /// </summary>
        internal static Exception IndexOutOfRange()
        {
            return new ArgumentOutOfRangeException(Strings.IndexOutOfRange);
        }

        /// <summary>
        /// ArgumentException with message like "An array dimension cannot be less than 0"
        /// </summary>
        internal static Exception BoundCannotBeLessThanZero()
        {
            return new ArgumentException(Strings.BoundCannotBeLessThanZero);
        }

        /// <summary>
        /// ArgumentException with message like "The number of elements does not match the length of the array"
        /// </summary>
        internal static Exception ArrayBoundsElementCountMismatch()
        {
            return new ArgumentException(Strings.ArrayBoundsElementCountMismatch);
        }

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method should take zero parameters"
        /// </summary>
        internal static Exception GetAwaiterShouldTakeZeroParameters()
        {
            return new ArgumentException(Strings.GetAwaiterShouldTakeZeroParameters);
        }

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method should not be generic"
        /// </summary>
        internal static Exception GetAwaiterShouldNotBeGeneric()
        {
            return new ArgumentException(Strings.GetAwaiterShouldNotBeGeneric);
        }

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' method has an unsupported return type"
        /// </summary>
        internal static Exception GetAwaiterShouldReturnAwaiterType()
        {
            return new ArgumentException(Strings.GetAwaiterShouldReturnAwaiterType);
        }

        /// <summary>
        /// ArgumentException with message like "Awaitable type '{0}' should have a 'GetAwaiter' method."
        /// </summary>
        internal static Exception AwaitableTypeShouldHaveGetAwaiterMethod(object p0)
        {
            return new ArgumentException(Strings.AwaitableTypeShouldHaveGetAwaiterMethod(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should implement 'INotifyCompletion'"
        /// </summary>
        internal static Exception AwaiterTypeShouldImplementINotifyCompletion(object p0)
        {
            return new ArgumentException(Strings.AwaiterTypeShouldImplementINotifyCompletion(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor"
        /// </summary>
        internal static Exception AwaiterTypeShouldHaveIsCompletedProperty(object p0)
        {
            return new ArgumentException(Strings.AwaiterTypeShouldHaveIsCompletedProperty(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'"
        /// </summary>
        internal static Exception AwaiterIsCompletedShouldReturnBool(object p0)
        {
            return new ArgumentException(Strings.AwaiterIsCompletedShouldReturnBool(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters"
        /// </summary>
        internal static Exception AwaiterIsCompletedShouldNotBeIndexer(object p0)
        {
            return new ArgumentException(Strings.AwaiterIsCompletedShouldNotBeIndexer(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Awaiter type '{0}' should have a 'GetResult' method"
        /// </summary>
        internal static Exception AwaiterTypeShouldHaveGetResultMethod(object p0)
        {
            return new ArgumentException(Strings.AwaiterTypeShouldHaveGetResultMethod(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The 'GetResult' method on awaiter type '{0}' has an unsupported return type"
        /// </summary>
        internal static Exception AwaiterGetResultTypeInvalid(object p0)
        {
            return new ArgumentException(Strings.AwaiterGetResultTypeInvalid(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have a 'GetAwaiter' expression."
        /// </summary>
        internal static Exception DynamicAwaitNoGetAwaiter()
        {
            return new ArgumentException(Strings.DynamicAwaitNoGetAwaiter);
        }

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have an 'IsCompleted' property."
        /// </summary>
        internal static Exception DynamicAwaitNoIsCompleted()
        {
            return new ArgumentException(Strings.DynamicAwaitNoIsCompleted);
        }

        /// <summary>
        /// ArgumentException with message like "Dynamically bound await operations cannot have a 'GetResult' method."
        /// </summary>
        internal static Exception DynamicAwaitNoGetResult()
        {
            return new ArgumentException(Strings.DynamicAwaitNoGetResult);
        }

        /// <summary>
        /// ArgumentException with message like "The 'GetAwaiter' expression should have one parameter."
        /// </summary>
        internal static Exception GetAwaiterExpressionOneParameter()
        {
            return new ArgumentException(Strings.GetAwaiterExpressionOneParameter);
        }

        /// <summary>
        /// ArgumentException with message like "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions"
        /// </summary>
        internal static Exception AsyncLambdaCantHaveByRefParameter(object p0)
        {
            return new ArgumentException(Strings.AsyncLambdaCantHaveByRefParameter(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Return type '{0}' is not valid for an asynchronous lambda expression"
        /// </summary>
        internal static Exception AsyncLambdaInvalidReturnType(object p0)
        {
            return new ArgumentException(Strings.AsyncLambdaInvalidReturnType(p0));
        }

        /// <summary>
        /// InvalidOperationException with message like "Await expression cannot occur in '{0}'"
        /// </summary>
        internal static Exception AwaitForbiddenHere(object p0)
        {
            return new InvalidOperationException(Strings.AwaitForbiddenHere(p0));
        }

        /// <summary>
        /// ArgumentException with message like "An expression of type '{0}' can't be used as a lock"
        /// </summary>
        internal static Exception LockNeedsReferenceType(object p0)
        {
            return new ArgumentException(Strings.LockNeedsReferenceType(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The conversion lambda should have one parameter"
        /// </summary>
        internal static Exception ConversionNeedsOneParameter()
        {
            return new ArgumentException(Strings.ConversionNeedsOneParameter);
        }

        /// <summary>
        /// ArgumentException with message like "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'"
        /// </summary>
        internal static Exception ConversionInvalidArgument(object p0, object p1)
        {
            return new ArgumentException(Strings.ConversionInvalidArgument(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'"
        /// </summary>
        internal static Exception ConversionInvalidResult(object p0, object p1)
        {
            return new ArgumentException(Strings.ConversionInvalidResult(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor"
        /// </summary>
        internal static Exception EnumeratorShouldHaveCurrentProperty(object p0)
        {
            return new ArgumentException(Strings.EnumeratorShouldHaveCurrentProperty(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type"
        /// </summary>
        internal static Exception EnumeratorShouldHaveMoveNextMethod(object p0)
        {
            return new ArgumentException(Strings.EnumeratorShouldHaveMoveNextMethod(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'"
        /// </summary>
        internal static Exception MoreThanOneIEnumerableFound(object p0)
        {
            return new ArgumentException(Strings.MoreThanOneIEnumerableFound(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Collection type '{0}' has no valid enumerable pattern"
        /// </summary>
        internal static Exception NoEnumerablePattern(object p0)
        {
            return new ArgumentException(Strings.NoEnumerablePattern(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Initializers should be assignments to variables"
        /// </summary>
        internal static Exception InvalidInitializer()
        {
            return new ArgumentException(Strings.InvalidInitializer);
        }

        /// <summary>
        /// ArgumentException with message like "Break and continue lables should be different"
        /// </summary>
        internal static Exception DuplicateLabels()
        {
            return new ArgumentException(Strings.DuplicateLabels);
        }

        /// <summary>
        /// ArgumentException with message like "Conditional access expressions require non-static members or extension methods."
        /// </summary>
        internal static Exception ConditionalAccessRequiresNonStaticMember()
        {
            return new ArgumentException(Strings.ConditionalAccessRequiresNonStaticMember);
        }

        /// <summary>
        /// ArgumentException with message like "Conditional access expressions require readable properties."
        /// </summary>
        internal static Exception ConditionalAccessRequiresReadableProperty()
        {
            return new ArgumentException(Strings.ConditionalAccessRequiresReadableProperty);
        }

        /// <summary>
        /// ArgumentException with message like "Too many arguments have been specified."
        /// </summary>
        internal static Exception TooManyArguments()
        {
            return new ArgumentException(Strings.TooManyArguments);
        }

        /// <summary>
        /// ArgumentException with message like "Conditional call expressions for extensions methods should specify an instance expression."
        /// </summary>
        internal static Exception ExtensionMethodRequiresInstance()
        {
            return new ArgumentException(Strings.ExtensionMethodRequiresInstance);
        }

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid governing type for a switch statement."
        /// </summary>
        internal static Exception InvalidSwitchType(object p0)
        {
            return new ArgumentException(Strings.InvalidSwitchType(p0));
        }

        /// <summary>
        /// ArgumentException with message like "The test value '{0}' occurs more than once."
        /// </summary>
        internal static Exception DuplicateTestValue(object p0)
        {
            return new ArgumentException(Strings.DuplicateTestValue(p0));
        }

        /// <summary>
        /// ArgumentException with message like "A 'null' test value cannot be used in a switch statement with governing type '{0}'."
        /// </summary>
        internal static Exception SwitchCantHaveNullCase(object p0)
        {
            return new ArgumentException(Strings.SwitchCantHaveNullCase(p0));
        }

        /// <summary>
        /// ArgumentException with message like "A test value with type '{0}' cannot be used in a swich statement with governing type '{1}'."
        /// </summary>
        internal static Exception SwitchCaseHasIncompatibleType(object p0, object p1)
        {
            return new ArgumentException(Strings.SwitchCaseHasIncompatibleType(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "All specified test values should have the same type."
        /// </summary>
        internal static Exception TestValuesShouldHaveConsistentType()
        {
            return new ArgumentException(Strings.TestValuesShouldHaveConsistentType);
        }

        /// <summary>
        /// ArgumentException with message like "The break label of a switch statement should be of type 'void'."
        /// </summary>
        internal static Exception SwitchBreakLabelShouldBeVoid()
        {
            return new ArgumentException(Strings.SwitchBreakLabelShouldBeVoid);
        }

        /// <summary>
        /// InvalidOperationException with message like "A 'goto case {0}' statement was found but the containing switch statement has no such label."
        /// </summary>
        internal static Exception InvalidGotoCase(object p0)
        {
            return new InvalidOperationException(Strings.InvalidGotoCase(p0));
        }

        /// <summary>
        /// InvalidOperationException with message like "A 'goto default' statement was found but the containing switch statement has no default label."
        /// </summary>
        internal static Exception InvalidGotoDefault()
        {
            return new InvalidOperationException(Strings.InvalidGotoDefault);
        }

        /// <summary>
        /// InvalidOperationException with message like "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node."
        /// </summary>
        internal static Exception GotoCanOnlyBeReducedInSwitch()
        {
            return new InvalidOperationException(Strings.GotoCanOnlyBeReducedInSwitch);
        }

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for a conditional receiver."
        /// </summary>
        internal static Exception InvalidConditionalReceiverType(object p0)
        {
            return new ArgumentException(Strings.InvalidConditionalReceiverType(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for a receiver in a conditional access expression."
        /// </summary>
        internal static Exception InvalidConditionalReceiverExpressionType(object p0)
        {
            return new ArgumentException(Strings.InvalidConditionalReceiverExpressionType(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver."
        /// </summary>
        internal static Exception ConditionalReceiverTypeMismatch(object p0, object p1)
        {
            return new ArgumentException(Strings.ConditionalReceiverTypeMismatch(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation '{0}' is not supported for type '{1}'."
        /// </summary>
        internal static Exception InvalidCompoundAssignment(object p0, object p1)
        {
            return new ArgumentException(Strings.InvalidCompoundAssignment(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'."
        /// </summary>
        internal static Exception InvalidCompoundAssignmentWithOperands(object p0, object p1, object p2)
        {
            return new ArgumentException(Strings.InvalidCompoundAssignmentWithOperands(p0, p1, p2));
        }

        /// <summary>
        /// ArgumentException with message like "Unary assignment operation '{0}' is not supported for an operand of type '{1}'."
        /// </summary>
        internal static Exception InvalidUnaryAssignmentWithOperands(object p0, object p1)
        {
            return new ArgumentException(Strings.InvalidUnaryAssignmentWithOperands(p0, p1));
        }

        /// <summary>
        /// ArgumentException with message like "Type must not be ByRef"
        /// </summary>
        internal static Exception TypeMustNotBeByRef()
        {
            return new ArgumentException(Strings.TypeMustNotBeByRef);
        }

        /// <summary>
        /// ArgumentException with message like "Type must not be a pointer type"
        /// </summary>
        internal static Exception TypeMustNotBePointer()
        {
            return new ArgumentException(Strings.TypeMustNotBePointer);
        }

        /// <summary>
        /// ArgumentException with message like "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations."
        /// </summary>
        internal static Exception InvalidNullCoalescingAssignmentArguments()
        {
            return new ArgumentException(Strings.InvalidNullCoalescingAssignmentArguments);
        }

        /// <summary>
        /// ArgumentException with message like "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable."
        /// </summary>
        internal static Exception InvalidInterpolatedStringType(object p0)
        {
            return new ArgumentException(Strings.InvalidInterpolatedStringType(p0));
        }

    }

    /// <summary>
    /// Strongly-typed and parameterized string resources.
    /// </summary>
    internal static partial class Strings
    {
        /// <summary>
        /// A string like "Parameter '{0}' is not defined for method '{1}'"
        /// </summary>
        internal static string ParameterNotDefinedForMethod(object p0, object p1)
        {
            return SR.Format(SR.ParameterNotDefinedForMethod, p0, p1);
        }

        /// <summary>
        /// A string like "Parameter index '{0}' is out of bounds for method '{1}'"
        /// </summary>
        internal static string ParameterIndexOutOfBounds(object p0, object p1)
        {
            return SR.Format(SR.ParameterIndexOutOfBounds, p0, p1);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' cannot be used for parameter of type '{1}'"
        /// </summary>
        internal static string ExpressionTypeDoesNotMatchParameter(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchParameter, p0, p1);
        }

        /// <summary>
        /// A string like "Parameter '{0}' has multiple bindings"
        /// </summary>
        internal static string DuplicateParameterBinding(object p0)
        {
            return SR.Format(SR.DuplicateParameterBinding, p0);
        }

        /// <summary>
        /// A string like "Non-optional parameter '{0}' has no binding"
        /// </summary>
        internal static string UnboundParameter(object p0)
        {
            return SR.Format(SR.UnboundParameter, p0);
        }

        /// <summary>
        /// A string like "A non-static constructor is required"
        /// </summary>
        internal static string NonStaticConstructorRequired
        {
            get
            {
                return SR.NonStaticConstructorRequired;
            }
        }

        /// <summary>
        /// A string like "The property '{0}' has no 'get' accessor"
        /// </summary>
        internal static string PropertyDoesNotHaveGetAccessor(object p0)
        {
            return SR.Format(SR.PropertyDoesNotHaveGetAccessor, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' has no 'set' accessor"
        /// </summary>
        internal static string PropertyDoesNotHaveSetAccessor(object p0)
        {
            return SR.Format(SR.PropertyDoesNotHaveSetAccessor, p0);
        }

        /// <summary>
        /// A string like "A non-static 'get' accessor is required for property '{0}'"
        /// </summary>
        internal static string AccessorCannotBeStatic(object p0)
        {
            return SR.Format(SR.AccessorCannotBeStatic, p0);
        }

        /// <summary>
        /// A string like "The number of indexes specified does not match the array rank"
        /// </summary>
        internal static string RankMismatch
        {
            get
            {
                return SR.RankMismatch;
            }
        }

        /// <summary>
        /// A string like "The specified index is out of range"
        /// </summary>
        internal static string IndexOutOfRange
        {
            get
            {
                return SR.IndexOutOfRange;
            }
        }

        /// <summary>
        /// A string like "An array dimension cannot be less than 0"
        /// </summary>
        internal static string BoundCannotBeLessThanZero
        {
            get
            {
                return SR.BoundCannotBeLessThanZero;
            }
        }

        /// <summary>
        /// A string like "The number of elements does not match the length of the array"
        /// </summary>
        internal static string ArrayBoundsElementCountMismatch
        {
            get
            {
                return SR.ArrayBoundsElementCountMismatch;
            }
        }

        /// <summary>
        /// A string like "The 'GetAwaiter' method should take zero parameters"
        /// </summary>
        internal static string GetAwaiterShouldTakeZeroParameters
        {
            get
            {
                return SR.GetAwaiterShouldTakeZeroParameters;
            }
        }

        /// <summary>
        /// A string like "The 'GetAwaiter' method should not be generic"
        /// </summary>
        internal static string GetAwaiterShouldNotBeGeneric
        {
            get
            {
                return SR.GetAwaiterShouldNotBeGeneric;
            }
        }

        /// <summary>
        /// A string like "The 'GetAwaiter' method has an unsupported return type"
        /// </summary>
        internal static string GetAwaiterShouldReturnAwaiterType
        {
            get
            {
                return SR.GetAwaiterShouldReturnAwaiterType;
            }
        }

        /// <summary>
        /// A string like "Awaitable type '{0}' should have a 'GetAwaiter' method."
        /// </summary>
        internal static string AwaitableTypeShouldHaveGetAwaiterMethod(object p0)
        {
            return SR.Format(SR.AwaitableTypeShouldHaveGetAwaiterMethod, p0);
        }

        /// <summary>
        /// A string like "Awaiter type '{0}' should implement 'INotifyCompletion'"
        /// </summary>
        internal static string AwaiterTypeShouldImplementINotifyCompletion(object p0)
        {
            return SR.Format(SR.AwaiterTypeShouldImplementINotifyCompletion, p0);
        }

        /// <summary>
        /// A string like "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor"
        /// </summary>
        internal static string AwaiterTypeShouldHaveIsCompletedProperty(object p0)
        {
            return SR.Format(SR.AwaiterTypeShouldHaveIsCompletedProperty, p0);
        }

        /// <summary>
        /// A string like "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'"
        /// </summary>
        internal static string AwaiterIsCompletedShouldReturnBool(object p0)
        {
            return SR.Format(SR.AwaiterIsCompletedShouldReturnBool, p0);
        }

        /// <summary>
        /// A string like "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters"
        /// </summary>
        internal static string AwaiterIsCompletedShouldNotBeIndexer(object p0)
        {
            return SR.Format(SR.AwaiterIsCompletedShouldNotBeIndexer, p0);
        }

        /// <summary>
        /// A string like "Awaiter type '{0}' should have a 'GetResult' method"
        /// </summary>
        internal static string AwaiterTypeShouldHaveGetResultMethod(object p0)
        {
            return SR.Format(SR.AwaiterTypeShouldHaveGetResultMethod, p0);
        }

        /// <summary>
        /// A string like "The 'GetResult' method on awaiter type '{0}' has an unsupported return type"
        /// </summary>
        internal static string AwaiterGetResultTypeInvalid(object p0)
        {
            return SR.Format(SR.AwaiterGetResultTypeInvalid, p0);
        }

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have a 'GetAwaiter' expression."
        /// </summary>
        internal static string DynamicAwaitNoGetAwaiter
        {
            get
            {
                return SR.DynamicAwaitNoGetAwaiter;
            }
        }

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have an 'IsCompleted' property."
        /// </summary>
        internal static string DynamicAwaitNoIsCompleted
        {
            get
            {
                return SR.DynamicAwaitNoIsCompleted;
            }
        }

        /// <summary>
        /// A string like "Dynamically bound await operations cannot have a 'GetResult' method."
        /// </summary>
        internal static string DynamicAwaitNoGetResult
        {
            get
            {
                return SR.DynamicAwaitNoGetResult;
            }
        }

        /// <summary>
        /// A string like "The 'GetAwaiter' expression should have one parameter."
        /// </summary>
        internal static string GetAwaiterExpressionOneParameter
        {
            get
            {
                return SR.GetAwaiterExpressionOneParameter;
            }
        }

        /// <summary>
        /// A string like "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions"
        /// </summary>
        internal static string AsyncLambdaCantHaveByRefParameter(object p0)
        {
            return SR.Format(SR.AsyncLambdaCantHaveByRefParameter, p0);
        }

        /// <summary>
        /// A string like "Return type '{0}' is not valid for an asynchronous lambda expression"
        /// </summary>
        internal static string AsyncLambdaInvalidReturnType(object p0)
        {
            return SR.Format(SR.AsyncLambdaInvalidReturnType, p0);
        }

        /// <summary>
        /// A string like "Await expression cannot occur in '{0}'"
        /// </summary>
        internal static string AwaitForbiddenHere(object p0)
        {
            return SR.Format(SR.AwaitForbiddenHere, p0);
        }

        /// <summary>
        /// A string like "An expression of type '{0}' can't be used as a lock"
        /// </summary>
        internal static string LockNeedsReferenceType(object p0)
        {
            return SR.Format(SR.LockNeedsReferenceType, p0);
        }

        /// <summary>
        /// A string like "The conversion lambda should have one parameter"
        /// </summary>
        internal static string ConversionNeedsOneParameter
        {
            get
            {
                return SR.ConversionNeedsOneParameter;
            }
        }

        /// <summary>
        /// A string like "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'"
        /// </summary>
        internal static string ConversionInvalidArgument(object p0, object p1)
        {
            return SR.Format(SR.ConversionInvalidArgument, p0, p1);
        }

        /// <summary>
        /// A string like "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'"
        /// </summary>
        internal static string ConversionInvalidResult(object p0, object p1)
        {
            return SR.Format(SR.ConversionInvalidResult, p0, p1);
        }

        /// <summary>
        /// A string like "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor"
        /// </summary>
        internal static string EnumeratorShouldHaveCurrentProperty(object p0)
        {
            return SR.Format(SR.EnumeratorShouldHaveCurrentProperty, p0);
        }

        /// <summary>
        /// A string like "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type"
        /// </summary>
        internal static string EnumeratorShouldHaveMoveNextMethod(object p0)
        {
            return SR.Format(SR.EnumeratorShouldHaveMoveNextMethod, p0);
        }

        /// <summary>
        /// A string like "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'"
        /// </summary>
        internal static string MoreThanOneIEnumerableFound(object p0)
        {
            return SR.Format(SR.MoreThanOneIEnumerableFound, p0);
        }

        /// <summary>
        /// A string like "Collection type '{0}' has no valid enumerable pattern"
        /// </summary>
        internal static string NoEnumerablePattern(object p0)
        {
            return SR.Format(SR.NoEnumerablePattern, p0);
        }

        /// <summary>
        /// A string like "Initializers should be assignments to variables"
        /// </summary>
        internal static string InvalidInitializer
        {
            get
            {
                return SR.InvalidInitializer;
            }
        }

        /// <summary>
        /// A string like "Break and continue lables should be different"
        /// </summary>
        internal static string DuplicateLabels
        {
            get
            {
                return SR.DuplicateLabels;
            }
        }

        /// <summary>
        /// A string like "Conditional access expressions require non-static members or extension methods."
        /// </summary>
        internal static string ConditionalAccessRequiresNonStaticMember
        {
            get
            {
                return SR.ConditionalAccessRequiresNonStaticMember;
            }
        }

        /// <summary>
        /// A string like "Conditional access expressions require readable properties."
        /// </summary>
        internal static string ConditionalAccessRequiresReadableProperty
        {
            get
            {
                return SR.ConditionalAccessRequiresReadableProperty;
            }
        }

        /// <summary>
        /// A string like "Too many arguments have been specified."
        /// </summary>
        internal static string TooManyArguments
        {
            get
            {
                return SR.TooManyArguments;
            }
        }

        /// <summary>
        /// A string like "Conditional call expressions for extensions methods should specify an instance expression."
        /// </summary>
        internal static string ExtensionMethodRequiresInstance
        {
            get
            {
                return SR.ExtensionMethodRequiresInstance;
            }
        }

        /// <summary>
        /// A string like "Type '{0}' is not a valid governing type for a switch statement."
        /// </summary>
        internal static string InvalidSwitchType(object p0)
        {
            return SR.Format(SR.InvalidSwitchType, p0);
        }

        /// <summary>
        /// A string like "The test value '{0}' occurs more than once."
        /// </summary>
        internal static string DuplicateTestValue(object p0)
        {
            return SR.Format(SR.DuplicateTestValue, p0);
        }

        /// <summary>
        /// A string like "A 'null' test value cannot be used in a switch statement with governing type '{0}'."
        /// </summary>
        internal static string SwitchCantHaveNullCase(object p0)
        {
            return SR.Format(SR.SwitchCantHaveNullCase, p0);
        }

        /// <summary>
        /// A string like "A test value with type '{0}' cannot be used in a swich statement with governing type '{1}'."
        /// </summary>
        internal static string SwitchCaseHasIncompatibleType(object p0, object p1)
        {
            return SR.Format(SR.SwitchCaseHasIncompatibleType, p0, p1);
        }

        /// <summary>
        /// A string like "All specified test values should have the same type."
        /// </summary>
        internal static string TestValuesShouldHaveConsistentType
        {
            get
            {
                return SR.TestValuesShouldHaveConsistentType;
            }
        }

        /// <summary>
        /// A string like "The break label of a switch statement should be of type 'void'."
        /// </summary>
        internal static string SwitchBreakLabelShouldBeVoid
        {
            get
            {
                return SR.SwitchBreakLabelShouldBeVoid;
            }
        }

        /// <summary>
        /// A string like "A 'goto case {0}' statement was found but the containing switch statement has no such label."
        /// </summary>
        internal static string InvalidGotoCase(object p0)
        {
            return SR.Format(SR.InvalidGotoCase, p0);
        }

        /// <summary>
        /// A string like "A 'goto default' statement was found but the containing switch statement has no default label."
        /// </summary>
        internal static string InvalidGotoDefault
        {
            get
            {
                return SR.InvalidGotoDefault;
            }
        }

        /// <summary>
        /// A string like "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node."
        /// </summary>
        internal static string GotoCanOnlyBeReducedInSwitch
        {
            get
            {
                return SR.GotoCanOnlyBeReducedInSwitch;
            }
        }

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for a conditional receiver."
        /// </summary>
        internal static string InvalidConditionalReceiverType(object p0)
        {
            return SR.Format(SR.InvalidConditionalReceiverType, p0);
        }

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for a receiver in a conditional access expression."
        /// </summary>
        internal static string InvalidConditionalReceiverExpressionType(object p0)
        {
            return SR.Format(SR.InvalidConditionalReceiverExpressionType, p0);
        }

        /// <summary>
        /// A string like "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver."
        /// </summary>
        internal static string ConditionalReceiverTypeMismatch(object p0, object p1)
        {
            return SR.Format(SR.ConditionalReceiverTypeMismatch, p0, p1);
        }

        /// <summary>
        /// A string like "Compound assignment operation '{0}' is not supported for type '{1}'."
        /// </summary>
        internal static string InvalidCompoundAssignment(object p0, object p1)
        {
            return SR.Format(SR.InvalidCompoundAssignment, p0, p1);
        }

        /// <summary>
        /// A string like "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'."
        /// </summary>
        internal static string InvalidCompoundAssignmentWithOperands(object p0, object p1, object p2)
        {
            return SR.Format(SR.InvalidCompoundAssignmentWithOperands, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Unary assignment operation '{0}' is not supported for an operand of type '{1}'."
        /// </summary>
        internal static string InvalidUnaryAssignmentWithOperands(object p0, object p1)
        {
            return SR.Format(SR.InvalidUnaryAssignmentWithOperands, p0, p1);
        }

        /// <summary>
        /// A string like "Type must not be ByRef"
        /// </summary>
        internal static string TypeMustNotBeByRef
        {
            get
            {
                return SR.TypeMustNotBeByRef;
            }
        }

        /// <summary>
        /// A string like "Type must not be a pointer type"
        /// </summary>
        internal static string TypeMustNotBePointer
        {
            get
            {
                return SR.TypeMustNotBePointer;
            }
        }

        /// <summary>
        /// A string like "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations."
        /// </summary>
        internal static string InvalidNullCoalescingAssignmentArguments
        {
            get
            {
                return SR.InvalidNullCoalescingAssignmentArguments;
            }
        }

        /// <summary>
        /// A string like "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable."
        /// </summary>
        internal static string InvalidInterpolatedStringType(object p0)
        {
            return SR.Format(SR.InvalidInterpolatedStringType, p0);
        }

    }
}

namespace System
{
    internal static partial class SR
    {
        public const string ParameterNotDefinedForMethod = "Parameter '{0}' is not defined for method '{1}'";
        public const string ParameterIndexOutOfBounds = "Parameter index '{0}' is out of bounds for method '{1}'";
        public const string ExpressionTypeDoesNotMatchParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}'";
        public const string DuplicateParameterBinding = "Parameter '{0}' has multiple bindings";
        public const string UnboundParameter = "Non-optional parameter '{0}' has no binding";
        public const string NonStaticConstructorRequired = "A non-static constructor is required";
        public const string PropertyDoesNotHaveGetAccessor = "The property '{0}' has no 'get' accessor";
        public const string PropertyDoesNotHaveSetAccessor = "The property '{0}' has no 'set' accessor";
        public const string AccessorCannotBeStatic = "A non-static 'get' accessor is required for property '{0}'";
        public const string RankMismatch = "The number of indexes specified does not match the array rank";
        public const string IndexOutOfRange = "The specified index is out of range";
        public const string BoundCannotBeLessThanZero = "An array dimension cannot be less than 0";
        public const string ArrayBoundsElementCountMismatch = "The number of elements does not match the length of the array";
        public const string GetAwaiterShouldTakeZeroParameters = "The 'GetAwaiter' method should take zero parameters";
        public const string GetAwaiterShouldNotBeGeneric = "The 'GetAwaiter' method should not be generic";
        public const string GetAwaiterShouldReturnAwaiterType = "The 'GetAwaiter' method has an unsupported return type";
        public const string AwaitableTypeShouldHaveGetAwaiterMethod = "Awaitable type '{0}' should have a 'GetAwaiter' method.";
        public const string AwaiterTypeShouldImplementINotifyCompletion = "Awaiter type '{0}' should implement 'INotifyCompletion'";
        public const string AwaiterTypeShouldHaveIsCompletedProperty = "Awaiter type '{0}' should have an 'IsCompleted' property with a 'get' accessor";
        public const string AwaiterIsCompletedShouldReturnBool = "The 'IsCompleted' property on awaiter type '{0}' should return 'Boolean'";
        public const string AwaiterIsCompletedShouldNotBeIndexer = "The 'IsCompleted' property on awaiter type '{0}' should not have indexer parameters";
        public const string AwaiterTypeShouldHaveGetResultMethod = "Awaiter type '{0}' should have a 'GetResult' method";
        public const string AwaiterGetResultTypeInvalid = "The 'GetResult' method on awaiter type '{0}' has an unsupported return type";
        public const string DynamicAwaitNoGetAwaiter = "Dynamically bound await operations cannot have a 'GetAwaiter' expression.";
        public const string DynamicAwaitNoIsCompleted = "Dynamically bound await operations cannot have an 'IsCompleted' property.";
        public const string DynamicAwaitNoGetResult = "Dynamically bound await operations cannot have a 'GetResult' method.";
        public const string GetAwaiterExpressionOneParameter = "The 'GetAwaiter' expression should have one parameter.";
        public const string AsyncLambdaCantHaveByRefParameter = "Parameter '{0}' is passed by reference which is not supported in asynchronous lambda expressions";
        public const string AsyncLambdaInvalidReturnType = "Return type '{0}' is not valid for an asynchronous lambda expression";
        public const string AwaitForbiddenHere = "Await expression cannot occur in '{0}'";
        public const string LockNeedsReferenceType = "An expression of type '{0}' can't be used as a lock";
        public const string ConversionNeedsOneParameter = "The conversion lambda should have one parameter";
        public const string ConversionInvalidArgument = "A collection element of type '{0}' cannot be assigned to the conversion lambda parameter of type '{1}'";
        public const string ConversionInvalidResult = "The conversion lambda result type '{0}' cannot be assigned to loop iteration variable type '{1}'";
        public const string EnumeratorShouldHaveCurrentProperty = "Enumerator type '{0}' should have a 'Current' property with a 'get' accessor";
        public const string EnumeratorShouldHaveMoveNextMethod = "Enumerator type '{0}' should have a 'MoveNext' method with a 'Boolean' return type";
        public const string MoreThanOneIEnumerableFound = "Collection type '{0}' has multiple implementations of 'IEnumerable&lt;T&gt;'";
        public const string NoEnumerablePattern = "Collection type '{0}' has no valid enumerable pattern";
        public const string InvalidInitializer = "Initializers should be assignments to variables";
        public const string DuplicateLabels = "Break and continue lables should be different";
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
        public const string InvalidGotoCase = "A 'goto case {0}' statement was found but the containing switch statement has no such label.";
        public const string InvalidGotoDefault = "A 'goto default' statement was found but the containing switch statement has no default label.";
        public const string GotoCanOnlyBeReducedInSwitch = "A 'goto case' or 'goto default' statement node can only be reduced when embedded in a switch statement node.";
        public const string InvalidConditionalReceiverType = "Type '{0}' is not a valid type for a conditional receiver.";
        public const string InvalidConditionalReceiverExpressionType = "Type '{0}' is not a valid type for a receiver in a conditional access expression.";
        public const string ConditionalReceiverTypeMismatch = "Type '{0}' of the receiver expression is not compatible with non-null type '{1}' of the conditional receiver.";
        public const string InvalidCompoundAssignment = "Compound assignment operation '{0}' is not supported for type '{1}'.";
        public const string InvalidCompoundAssignmentWithOperands = "Compound assignment operation '{0}' is not supported for operands of type '{1}' and '{2}'.";
        public const string InvalidUnaryAssignmentWithOperands = "Unary assignment operation '{0}' is not supported for an operand of type '{1}'.";
        public const string TypeMustNotBeByRef = "Type must not be ByRef";
        public const string TypeMustNotBePointer = "Type must not be a pointer type";
        public const string InvalidNullCoalescingAssignmentArguments = "Compound assignment operation of type 'NullCoalescingAssign' does not support custom methods or conversion operations.";
        public const string InvalidInterpolatedStringType = "Type '{0}' is not a valid type for an interpolated string. Supported types are string, FormattableString, or IFormattable.";
    }
}