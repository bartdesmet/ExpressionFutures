// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
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

        public static void ValidateParameterBindings(MethodBase method, ReadOnlyCollection<ParameterAssignment> argList)
        {
            ValidateParameterBindings(method, method.GetParametersCached(), argList);
        }

        public static void ValidateParameterBindings(MethodBase method, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> argList)
        {
            var boundParameters = new HashSet<ParameterInfo>();

            foreach (var arg in argList)
            {
                var parameter = arg.Parameter;

                var member = parameter.Member;

                var property = member as PropertyInfo;
                if (property != null)
                {
                    // NB: This supports get access via indexers.
                    member = property.GetGetMethod(true);
                }

                if (member != method)
                {
                    throw Error.ParameterNotDefinedForMethod(parameter.Name, method.Name);
                }

                if (!boundParameters.Add(parameter))
                {
                    throw Error.DuplicateParameterBinding(parameter.Name);
                }
            }

            foreach (var parameter in parameters)
            {
                if (!boundParameters.Contains(parameter) && (!parameter.IsOptional || !parameter.HasDefaultValue))
                {
                    throw Error.UnboundParameter(parameter.Name);
                }
            }
        }

        private const int MinConstInt32 = -2;
        private const int MaxConstInt32 = 7;
        private static ConstantExpression[] s_constInt32;

        public static ConstantExpression CreateConstantInt32(int value)
        {
            if (value >= MinConstInt32 && value <= MaxConstInt32)
            {
                var index = value - MinConstInt32;
                var consts = s_constInt32 ?? (s_constInt32 = new ConstantExpression[MaxConstInt32 - MinConstInt32 + 1]);
                return consts[index] ?? (consts[index] = Expression.Constant(value));
            }

            return Expression.Constant(value);
        }

        public static MethodInfo GetNonGenericMethod(this Type type, string name, BindingFlags flags, Type[] types)
        {
            var candidates = type.GetMethods(flags).Where(m => !m.IsGenericMethod && m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types)).ToArray();

            var res = default(MethodInfo);

            if (candidates.Length > 1)
            {
                // TODO: This deals with `new` hiding in a quick-n-dirty way.

                for (var t = type; t != null; t = t.BaseType)
                {
                    foreach (var candidate in candidates)
                    {
                        if (candidate.DeclaringType == t)
                        {
                            return candidate;
                        }
                    }
                }
            }
            else if (candidates.Length == 1)
            {
                res = candidates[0];
            }

            return res;
        }

        public static MethodInfo FindDisposeMethod(this Type type)
        {
            // REVIEW: This may pose challenges on .NET Native
            var map = type.GetInterfaceMap(typeof(IDisposable));
            return map.TargetMethods.Single(); // NB: IDisposable has only one method
        }
    }
}
