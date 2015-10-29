// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    static class Helpers
    {
        public static bool CheckArgumentsInOrder(ReadOnlyCollection<ParameterAssignment> arguments)
        {
            var inOrder = true;
            var lastPosition = -1;

            foreach (var argument in arguments)
            {
                if (argument.Parameter.Position < lastPosition)
                {
                    inOrder = false;
                    break;
                }

                lastPosition = argument.Parameter.Position;
            }

            return inOrder;
        }

        public static void FillOptionalParameters(ParameterInfo[] parameters, Expression[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    var parameter = parameters[i];
                    args[i] = Expression.Constant(parameter.DefaultValue, parameter.ParameterType);
                }
            }
        }
    }
}
