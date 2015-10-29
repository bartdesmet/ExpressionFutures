// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings
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
    }
}
