// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;

using LinqError = System.Linq.Expressions.Error;

namespace Microsoft.CSharp.Expressions
{
    partial class CSharpExpression
    {
        private static void ValidateCondition(Expression test, bool optionalTest = false)
        {
            if (optionalTest && test == null)
            {
                return;
            }

            RequiresCanRead(test, nameof(test));

            // TODO: We can be more flexible and allow the rules in C# spec 7.20.
            //       Note that this behavior is the same as IfThen, but we could also add C# specific nodes for those,
            //       with the more flexible construction behavior.
            if (test.Type != typeof(bool))
                throw LinqError.ArgumentMustBeBoolean();
        }

        internal static ReadOnlyCollection<ParameterExpression> CheckUniqueVariables(IEnumerable<ParameterExpression> variables, string paramName)
        {
            var variablesList = variables.ToReadOnly();

            if (variablesList.Count > 0)
            {
                RequiresNotNullItems(variablesList, paramName);

                var uniqueVariables = new HashSet<ParameterExpression>(variablesList.Count);

                foreach (var variable in variablesList)
                {
                    if (!uniqueVariables.Add(variable))
                    {
                        throw LinqError.DuplicateVariable(variable);
                    }
                }
            }

            return variablesList;
        }
    }
}
