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
    }
}
