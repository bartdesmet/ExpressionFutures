// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    internal sealed class ReadOnlyTemporaryVariableExpression : Expression
    {
        private readonly ParameterExpression _variable;

        public ReadOnlyTemporaryVariableExpression(ParameterExpression variable) => _variable = variable;

        public override Type Type => _variable.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override bool CanReduce => true;

        public override Expression Reduce() => _variable;
    }
}
