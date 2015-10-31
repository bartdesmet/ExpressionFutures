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
        internal static Exception GetAwaiterShouldNotReturnAwaiterType()
        {
            return new ArgumentException(Strings.GetAwaiterShouldNotReturnAwaiterType);
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
        internal static string GetAwaiterShouldNotReturnAwaiterType
        {
			get
			{
				return SR.GetAwaiterShouldNotReturnAwaiterType;
			}
        }

    }
}

namespace System
{
	internal static partial class SR
	{
		public const string ParameterNotDefinedForMethod = "Parameter '{0}' is not defined for method '{1}'";
		public const string ExpressionTypeDoesNotMatchParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}'";
		public const string DuplicateParameterBinding = "Parameter '{0}' has multiple bindings";
		public const string UnboundParameter = "Non-optional parameter '{0}' has no binding";
		public const string NonStaticConstructorRequired = "A non-static constructor is required";
		public const string PropertyDoesNotHaveGetAccessor = "The property '{0}' has no 'get' accessor";
		public const string AccessorCannotBeStatic = "A non-static 'get' accessor is required for property '{0}'";
		public const string RankMismatch = "The number of indexes specified does not match the array rank";
		public const string IndexOutOfRange = "The specified index is out of range";
		public const string BoundCannotBeLessThanZero = "An array dimension cannot be less than 0";
		public const string ArrayBoundsElementCountMismatch = "The number of elements does not match the length of the array";
		public const string GetAwaiterShouldTakeZeroParameters = "The 'GetAwaiter' method should take zero parameters";
		public const string GetAwaiterShouldNotBeGeneric = "The 'GetAwaiter' method should not be generic";
		public const string GetAwaiterShouldNotReturnAwaiterType = "The 'GetAwaiter' method has an unsupported return type";
	}
}