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
    }
}
