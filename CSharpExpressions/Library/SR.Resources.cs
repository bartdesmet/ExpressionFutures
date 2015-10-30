// Prototyping extended expression trees for C#.
//
// bartde - October 2015

namespace System
{
    // NOTE: A similar file exists in System. This should get generated from a .resx ultimately.

    partial class SR
    {
        public const string ParameterNotDefinedForMethod = "Parameter '{0}' is not defined for method '{1}'";
        public const string ExpressionTypeDoesNotMatchParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}'";
        public const string DuplicateParameterBinding = "Parameter '{0}' has multiple bindings";
        public const string UnboundParameter = "Non-optional parameter '{0}' has no binding";
        public const string NonStaticConstructorRequired = "A non-static constructor is required";
        public const string PropertyDoesNotHaveGetAccessor = "The property '{0}' has no 'get' accessor";
        public const string AccessorCannotBeStatic = "A non-static 'get' accessor is required for property '{0}'";
        public const string BoundCannotBeLessThanZero = "An array dimension cannot be less than 0";
        public const string ArrayBoundsElementCountMismatch = "The number of elements does not match the length of the array";
        public const string RankMismatch = "The number of indices specified does not match the array rank";
        public const string IndexOutOfRange = "The specified index is out of range";
    }
}
