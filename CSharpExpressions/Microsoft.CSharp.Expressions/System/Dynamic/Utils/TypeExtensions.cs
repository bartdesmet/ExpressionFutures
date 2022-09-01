// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NB: This is a pruned copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/TypeExtensions.cs.

#nullable enable

using System.Reflection;

namespace System.Dynamic.Utils
{
    internal static class TypeExtensions
    {
        public static Type GetReturnType(this MethodBase mi) => mi.IsConstructor ? mi.DeclaringType! : ((MethodInfo)mi).ReturnType;

        internal static ParameterInfo[] GetParametersCached(this MethodBase method)
        {
            // NB: Disabled caching in this pruned down copy.
            return method.GetParameters();
        }

        internal static bool IsByRefParameter(this ParameterInfo pi)
        {
            if (pi.ParameterType.IsByRef)
                return true;

            return (pi.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
        }

        public static MethodInfo? GetAnyStaticMethodValidated(this Type type, string name, Type[] types)
        {
            MethodInfo method = type.GetMethod(name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
            if (!method.MatchesArgumentTypes(types))
            {
                return null;
            }
            return method;
        }

        private static bool MatchesArgumentTypes(this MethodInfo mi, Type[] argTypes)
        {
            if (mi == null)
            {
                return false;
            }
            ParameterInfo[] parametersCached = mi.GetParametersCached();
            if (parametersCached.Length != argTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < parametersCached.Length; i++)
            {
                if (!TypeUtils.AreReferenceAssignable(parametersCached[i].ParameterType, argTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }

    }
}