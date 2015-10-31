// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Reflection;

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
        /// <returns></returns>
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
    }
}
