// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Linq.Expressions;

using static System.Dynamic.Utils.ErrorUtils;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents type information about a pattern that assigns a variable.
    /// </summary>
    public sealed class CSharpObjectPatternInfo
    {
        internal CSharpObjectPatternInfo(CSharpPatternInfo info, ParameterExpression variable)
        {
            Info = info;
            Variable = variable;
        }

        /// <summary>
        /// Gets the underlying type information.
        /// </summary>
        public CSharpPatternInfo Info { get; }

        /// <summary>
        /// Gets the variable to assign to.
        /// </summary>
        public ParameterExpression Variable { get; }
    }

    partial class CSharpPattern
    {
        /// <summary>
        /// Creates an object holding type information about a pattern.
        /// </summary>
        /// <param name="info">The underlying type information.</param>
        /// <param name="variable">The CSharpObjectPatternInfo to assign to upon successfully matching the associated pattern.</param>
        /// <returns>A <see cref="CSharpPatternInfo" /> object holding type information about a pattern.</returns>
        public static CSharpObjectPatternInfo ObjectPatternInfo(CSharpPatternInfo info, ParameterExpression variable)
        {
            info ??= PatternInfo(typeof(object), variable?.Type ?? typeof(object));

            if (variable != null)
            {
                if (variable.Type.IsByRef)
                    throw VariableMustNotBeByRef(variable, variable.Type, nameof(variable));

                RequiresCompatiblePatternTypes(info.NarrowedType, variable.Type);
            }

            return new CSharpObjectPatternInfo(info, variable);
        }
    }
}
