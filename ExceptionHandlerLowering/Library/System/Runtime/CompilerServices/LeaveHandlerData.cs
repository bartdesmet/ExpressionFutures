// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Describes a control flow transfer out of a protected region, akin to a leave instruction in IL.
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Never used for equality checks by the caller; just used as a cheap bundle of two values.")]
    public struct LeaveHandlerData
    {
        /// <summary>
        /// Creates a new control flow transfer descriptor.
        /// </summary>
        /// <param name="index">The index of the control flow transfer target after leaving the protected region.</param>
        /// <param name="value">The value to transfer to the control flow transfer target.</param>
        public LeaveHandlerData(int index, object value)
        {
            Index = index;
            Value = value;
        }

        /// <summary>
        /// The index of the control flow transfer target after leaving the protected region.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Infrastructure only; just need a cheap bundle of two values.")]
        public readonly int Index;

        /// <summary>
        /// The value to transfer to the control flow transfer target.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Infrastructure only; just need a cheap bundle of two values.")]
        public readonly object Value;
    }
}
