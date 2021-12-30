// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Describes the pattern types for the C# expression tree pattern nodes.
    /// </summary>
    public enum CSharpPatternType
    {
        /// <summary>
        /// A constant pattern, e.g. '42'.
        /// </summary>
        Constant,

        /// <summary>
        /// A discard pattern, e.g. '_'.
        /// </summary>
        Discard,

        /// <summary>
        /// A type pattern, e.g. 'int'.
        /// </summary>
        Type,

        /// <summary>
        /// An and pattern, e.g. 'p and q'.
        /// </summary>
        And,

        /// <summary>
        /// An or pattern, e.g. 'p or q'.
        /// </summary>
        Or,

        /// <summary>
        /// A not pattern, e.g. `not p`.
        /// </summary>
        Not,

        /// <summary>
        /// A relational less than pattern, e.g. '< 0'.
        /// </summary>
        LessThan,

        /// <summary>
        /// A relational less than or equal pattern, e.g. '<= 0'.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// A relational greater than pattern, e.g. '> 0'.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// A relational greater than or equal pattern, e.g. '>= 0'.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// A declaration pattern, e.g. 'int x'.
        /// </summary>
        Declaration,

        /// <summary>
        /// A var pattern, e.g. 'var x'.
        /// </summary>
        Var,

        /// <summary>
        /// A positional pattern using ITuple, e.g. '(p, q)'.
        /// </summary>
        /// <remarks>
        /// This pattern is used when the input type is not a tuple type or does not support deconstruction.
        /// </remarks>
        ITuple,

        /// <summary>
        /// A positional and/or property pattern, e.g. '(p, q)', or '(x: p, y: q)', or '{ X: p, Y: q }'.
        /// </summary>
        Recursive,

        /// <summary>
        /// A list pattern, e.g. '[p0, p1, p2]'.
        /// </summary>
        List,

        /// <summary>
        /// A slice pattern, e.g. '..' used within a list pattern '[p0, ..]'.
        /// </summary>
        Slice,
    }
}
