// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

namespace System.Runtime.CompilerServices
{
    public struct LeaveHandlerData
    {
        public LeaveHandlerData(int index, object value)
        {
            Index = index;
            Value = value;
        }

        public readonly int Index;
        public readonly object Value;
    }
}
