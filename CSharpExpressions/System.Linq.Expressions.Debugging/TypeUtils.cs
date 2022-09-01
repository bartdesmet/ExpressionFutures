// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NB: This is a pruned copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/TypeUtils.cs.

using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System.Dynamic.Utils
{
    internal static class TypeUtils2
    {
        private static readonly Type[] s_arrayAssignableInterfaces = typeof(int[]).GetInterfaces()
            .Where(i => i.IsGenericType)
            .Select(i => i.GetGenericTypeDefinition())
            .ToArray();

        public static Type GetNonNullableType(Type type) => IsNullableType(type) ? type.GetGenericArguments()[0] : type;

        public static bool IsNullableType(Type type) => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool AreEquivalent(Type t1, Type t2) => t1 != null && t1.IsEquivalentTo(t2);

        public static bool HasReferenceConversionTo(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || dest == typeof(void))
            {
                return false;
            }

            Type nnSourceType = GetNonNullableType(source);
            Type nnDestType = GetNonNullableType(dest);

            // Down conversion
            if (nnSourceType.IsAssignableFrom(nnDestType))
            {
                return true;
            }

            // Up conversion
            if (nnDestType.IsAssignableFrom(nnSourceType))
            {
                return true;
            }

            // Interface conversion
            if (source.IsInterface || dest.IsInterface)
            {
                return true;
            }

            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, dest))
            {
                return true;
            }

            // Object conversion handled by assignable above.
            Debug.Assert(source != typeof(object) && dest != typeof(object));

            return (source.IsArray || dest.IsArray) && StrictHasReferenceConversionTo(source, dest, true);
        }

        private static bool StrictHasReferenceConversionTo(Type source, Type dest, bool skipNonArray)
        {
            // HasReferenceConversionTo was both too strict and too lax. It was too strict in prohibiting
            // some valid conversions involving arrays, and too lax in allowing casts between interfaces
            // and sealed classes that don't implement them. Unfortunately fixing the lax cases would be
            // a breaking change, especially since such expressions will even work if only given null
            // arguments.
            // This method catches the cases that were incorrectly disallowed, but when it needs to
            // examine possible conversions of element or type parameters it applies stricter rules.

            while (true)
            {
                if (!skipNonArray) // Skip if we just came from HasReferenceConversionTo and have just tested these
                {
                    if (source.IsValueType | dest.IsValueType)
                    {
                        return false;
                    }

                    // Includes to case of either being typeof(object)
                    if (source.IsAssignableFrom(dest) || dest.IsAssignableFrom(source))
                    {
                        return true;
                    }

                    if (source.IsInterface)
                    {
                        if (dest.IsInterface || dest.IsClass && !dest.IsSealed)
                        {
                            return true;
                        }
                    }
                    else if (dest.IsInterface)
                    {
                        if (source.IsClass && !source.IsSealed)
                        {
                            return true;
                        }
                    }
                }

                if (source.IsArray)
                {
                    if (dest.IsArray)
                    {
                        if (source.GetArrayRank() != dest.GetArrayRank() || IsSZArray(source) != IsSZArray(dest))
                        {
                            return false;
                        }

                        source = source.GetElementType();
                        dest = dest.GetElementType();
                        skipNonArray = false;
                    }
                    else
                    {
                        return HasArrayToInterfaceConversion(source, dest);
                    }
                }
                else if (dest.IsArray)
                {
                    if (HasInterfaceToArrayConversion(source, dest))
                    {
                        return true;
                    }

                    return IsImplicitReferenceConversion(typeof(Array), source);
                }
                else
                {
                    return IsLegalExplicitVariantDelegateConversion(source, dest);
                }
            }
        }

        private static bool HasArrayToInterfaceConversion(Type source, Type dest)
        {
            Debug.Assert(source.IsArray);
            if (!IsSZArray(source) || !dest.IsInterface || !dest.IsGenericType)
            {
                return false;
            }

            Type[] destParams = dest.GetGenericArguments();
            if (destParams.Length != 1)
            {
                return false;
            }

            Type destGen = dest.GetGenericTypeDefinition();

            foreach (Type iface in s_arrayAssignableInterfaces)
            {
                if (AreEquivalent(destGen, iface))
                {
                    return StrictHasReferenceConversionTo(source.GetElementType(), destParams[0], false);
                }
            }

            return false;
        }

        private static bool HasInterfaceToArrayConversion(Type source, Type dest)
        {
            Debug.Assert(IsSZArray(dest));
            if (!IsSZArray(dest) || !source.IsInterface || !source.IsGenericType)
            {
                return false;
            }

            Type[] sourceParams = source.GetGenericArguments();
            if (sourceParams.Length != 1)
            {
                return false;
            }

            Type sourceGen = source.GetGenericTypeDefinition();

            foreach (Type iface in s_arrayAssignableInterfaces)
            {
                if (AreEquivalent(sourceGen, iface))
                {
                    return StrictHasReferenceConversionTo(sourceParams[0], dest.GetElementType(), false);
                }
            }

            return false;
        }

        private static bool IsCovariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool IsContravariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool IsInvariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }

        private static bool IsDelegate(Type t)
        {
            Debug.Assert(t != null);
            return t.IsSubclassOf(typeof(MulticastDelegate));
        }

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // There *might* be a legal conversion from a generic delegate type S to generic delegate type  T,
            // provided all of the follow are true:
            //   o Both types are constructed generic types of the same generic delegate type, D<X1,... Xk>.
            //     That is, S = D<S1...>, T = D<T1...>.
            //   o If type parameter Xi is declared to be invariant then Si must be identical to Ti.
            //   o If type parameter Xi is declared to be covariant ("out") then Si must be convertible
            //     to Ti via an identify conversion,  implicit reference conversion, or explicit reference conversion.
            //   o If type parameter Xi is declared to be contravariant ("in") then either Si must be identical to Ti,
            //     or Si and Ti must both be reference types.

            if (!IsDelegate(source) || !IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
            {
                return false;
            }

            Type genericDelegate = source.GetGenericTypeDefinition();

            if (dest.GetGenericTypeDefinition() != genericDelegate)
            {
                return false;
            }

            Type[] genericParameters = genericDelegate.GetGenericArguments();
            Type[] sourceArguments = source.GetGenericArguments();
            Type[] destArguments = dest.GetGenericArguments();

            Debug.Assert(genericParameters != null);
            Debug.Assert(sourceArguments != null);
            Debug.Assert(destArguments != null);
            Debug.Assert(genericParameters.Length == sourceArguments.Length);
            Debug.Assert(genericParameters.Length == destArguments.Length);

            for (int iParam = 0; iParam < genericParameters.Length; ++iParam)
            {
                Type sourceArgument = sourceArguments[iParam];
                Type destArgument = destArguments[iParam];

                Debug.Assert(sourceArgument != null && destArgument != null);

                // If the arguments are identical then this one is automatically good, so skip it.
                if (AreEquivalent(sourceArgument, destArgument))
                {
                    continue;
                }

                Type genericParameter = genericParameters[iParam];

                Debug.Assert(genericParameter != null);

                if (IsInvariant(genericParameter))
                {
                    return false;
                }

                if (IsCovariant(genericParameter))
                {
                    if (!HasReferenceConversionTo(sourceArgument, destArgument))
                    {
                        return false;
                    }
                }
                else if (IsContravariant(genericParameter) && (sourceArgument.IsValueType || destArgument.IsValueType))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsImplicitReferenceConversion(Type source, Type destination) => destination.IsAssignableFrom(source);

        private static bool IsSZArray(Type t) => t.IsArray && t.GetElementType().MakeArrayType() == t;
    }
}
