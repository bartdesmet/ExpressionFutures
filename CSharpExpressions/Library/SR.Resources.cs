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
    }
}
