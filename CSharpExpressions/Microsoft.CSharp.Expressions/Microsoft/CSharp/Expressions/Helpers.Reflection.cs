// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static MethodInfo? GetNonGenericMethod(this Type type, string name, BindingFlags flags, Type[] types)
        {
            var candidates = GetTypeAndBase(type).SelectMany(t => t.GetMethods(flags)).Where(m => !m.IsGenericMethod && m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types)).ToArray();

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

        private static IEnumerable<Type> GetTypeAndBase(Type type)
        {
            yield return type;

            if (type.IsInterface)
            {
                foreach (var i in type.GetInterfaces())
                {
                    yield return i;
                }
            }
        }

        public static bool IsVector(this Type type)
        {
            // TODO: Use IsSZArray.
            return type.IsArray && type.GetElementType()!.MakeArrayType() == type;
        }

        public static bool IsMutableStruct(Type type)
        {
            return type.IsValueType && !type.IsPrimitive /* immutable */;
        }

        public static Type GetConditionalType(this Type type)
        {
            if (type.IsValueType && type != typeof(void) && !type.IsNullableType())
            {
                return type.GetNullableType();
            }

            return type;
        }

        public static Type GetNonNullReceiverType(this Type type)
        {
            // DESIGN: Should we reject non-nullable value types here?
            return type.GetNonNullableType();
        }
    }
}
