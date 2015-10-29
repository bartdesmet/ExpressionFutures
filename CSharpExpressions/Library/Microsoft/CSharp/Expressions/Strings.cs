// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Strongly-typed and parameterized string resources.
    /// </summary
    internal static class Strings
    {
        /// <summary>
        /// A string like "Parameter '{0}' is not defined for method '{1}'"
        /// </summary>
        internal static string ParameterNotDefinedForMethod(object p0, object p1)
        {
            return SR.Format(SR.ParameterNotDefinedForMethod, p0, p1);
        }
    }
}
