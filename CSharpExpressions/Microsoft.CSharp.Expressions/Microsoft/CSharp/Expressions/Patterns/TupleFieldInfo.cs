// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace Microsoft.CSharp.Expressions
{
    public sealed class TupleFieldInfo
    {
        internal TupleFieldInfo(string name, int index)
        {
            Name = name;
            Index = index;
        }

        public string Name { get; }
        public int Index { get; }
    }
}
