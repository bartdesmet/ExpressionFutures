// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NB: This is a pruned copy of https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Dynamic/Utils/ExpressionUtils.cs.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ErrorUtils;

namespace System.Dynamic.Utils
{
    internal static class ExpressionUtils
    {
        public static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments, string? methodParamName)
        {
            Debug.Assert(nodeKind is ExpressionType.Invoke or ExpressionType.Call or ExpressionType.Dynamic or ExpressionType.New);

            ParameterInfo[] pis = GetParametersForValidation(method, nodeKind);

            ValidateArgumentCount(method, nodeKind, arguments.Count, pis);

            Expression[]? newArgs = null;
            for (int i = 0, n = pis.Length; i < n; i++)
            {
                Expression arg = arguments[i];
                ParameterInfo pi = pis[i];
                arg = ValidateOneArgument(method, nodeKind, arg, pi, methodParamName, nameof(arguments), i);

                if (newArgs == null && arg != arguments[i])
                {
                    newArgs = new Expression[arguments.Count];
                    for (int j = 0; j < i; j++)
                    {
                        newArgs[j] = arguments[j];
                    }
                }
                if (newArgs != null)
                {
                    newArgs[i] = arg;
                }
            }
            if (newArgs != null)
            {
                arguments = new ReadOnlyCollectionBuilder<Expression>(newArgs).ToReadOnlyCollection();
            }
        }

        public static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
        {
            if (pis.Length != count)
            {
                // Throw the right error for the node we were given
                throw nodeKind switch
                {
                    ExpressionType.New => IncorrectNumberOfConstructorArguments(),
                    ExpressionType.Invoke => IncorrectNumberOfLambdaArguments(),
                    ExpressionType.Dynamic or ExpressionType.Call => IncorrectNumberOfMethodCallArguments(method, nameof(method)),
                    _ => ContractUtils.Unreachable,
                };
            }
        }

        public static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arguments, ParameterInfo pi, string? methodParamName, string argumentParamName, int index = -1)
        {
            RequiresCanRead(arguments, argumentParamName, index);
            Type pType = pi.ParameterType;
            if (pType.IsByRef)
            {
                pType = pType.GetElementType()!;
            }

            TypeUtils.ValidateType(pType, methodParamName, allowByRef: true, allowPointer: true);
            if (!TypeUtils.AreReferenceAssignable(pType, arguments.Type))
            {
                if (!TryQuote(pType, ref arguments))
                {
                    // Throw the right error for the node we were given
                    throw nodeKind switch
                    {
                        ExpressionType.New => ExpressionTypeDoesNotMatchConstructorParameter(arguments.Type, pType, argumentParamName, index),
                        ExpressionType.Invoke => ExpressionTypeDoesNotMatchParameter(arguments.Type, pType, argumentParamName, index),
                        ExpressionType.Dynamic or ExpressionType.Call => ExpressionTypeDoesNotMatchMethodParameter(arguments.Type, pType, method, argumentParamName, index),
                        _ => ContractUtils.Unreachable,
                    };
                }
            }
            return arguments;
        }

        public static void RequiresCanRead([NotNull] Expression? expression, string paramName)
        {
            RequiresCanRead(expression, paramName, -1);
        }

        public static void RequiresCanRead([NotNull] Expression? expression, string paramName, int idx)
        {
            ContractUtils.RequiresNotNull(expression, paramName, idx);

            // validate that we can read the node
            switch (expression.NodeType)
            {
                case ExpressionType.Index:
                    IndexExpression index = (IndexExpression)expression;
                    if (index.Indexer != null && !index.Indexer.CanRead)
                    {
                        throw new ArgumentException(ErrorStrings.ExpressionMustBeReadable, GetParamName(paramName, idx));
                    }
                    break;
                case ExpressionType.MemberAccess:
                    MemberExpression member = (MemberExpression)expression;
                    if (member.Member is PropertyInfo prop)
                    {
                        if (!prop.CanRead)
                        {
                            throw new ArgumentException(ErrorStrings.ExpressionMustBeReadable, GetParamName(paramName, idx));
                        }
                    }
                    break;
            }
        }

        internal static void RequiresCanRead(IReadOnlyList<Expression> items, string paramName)
        {
            for (int i = 0, count = items.Count; i < count; i++)
            {
                RequiresCanRead(items[i], paramName, i);
            }
        }

        public static bool TryQuote(Type parameterType, ref Expression argument)
        {
            Type quoteable = typeof(LambdaExpression);

            if (TypeUtils.IsSameOrSubclass(quoteable, parameterType) && parameterType.IsInstanceOfType(argument))
            {
                argument = Expression.Quote(argument);
                return true;
            }

            return false;
        }

        internal static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
        {
            ParameterInfo[] pis = method.GetParametersCached();

            if (nodeKind == ExpressionType.Dynamic)
            {
                pis = pis.RemoveFirst(); // ignore CallSite argument
            }
            return pis;
        }

        internal static void RequiresCanWrite(Expression expression, string paramName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(paramName);
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Index:
                    PropertyInfo? indexer = ((IndexExpression)expression).Indexer;
                    if (indexer == null || indexer.CanWrite)
                    {
                        return;
                    }
                    break;
                case ExpressionType.MemberAccess:
                    MemberInfo member = ((MemberExpression)expression).Member;
                    if (member is PropertyInfo prop)
                    {
                        if (prop.CanWrite)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Debug.Assert(member is FieldInfo);
                        FieldInfo field = (FieldInfo)member;
                        if (!(field.IsInitOnly || field.IsLiteral))
                        {
                            return;
                        }
                    }
                    break;
                case ExpressionType.Parameter:
                    return;
            }

            throw new ArgumentException(ErrorStrings.ExpressionMustBeWriteable, paramName);
        }

        internal static void ValidateMethodInfo(MethodInfo method, string paramName)
        {
            if (method.ContainsGenericParameters)
            {
                throw method.IsGenericMethodDefinition ? new ArgumentException($"Method {method} is a generic method definition.", paramName) : new ArgumentException($"Method {method} contains generic parameters.", paramName);
            }
        }

        internal static PropertyInfo GetProperty(MethodInfo mi, string? paramName, int index = -1)
        {
            Type? type = mi.DeclaringType;
            if (type != null)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;
                flags |= (mi.IsStatic) ? BindingFlags.Static : BindingFlags.Instance;
                PropertyInfo[] props = type.GetProperties(flags);
                foreach (PropertyInfo pi in props)
                {
                    if (pi.CanRead && CheckMethod(mi, pi.GetGetMethod(nonPublic: true)!))
                    {
                        return pi;
                    }
                    if (pi.CanWrite && CheckMethod(mi, pi.GetSetMethod(nonPublic: true)!))
                    {
                        return pi;
                    }
                }
            }

            throw new ArgumentException($"The method {mi.DeclaringType}.{mi.Name} is not a property accessor.", GetParamName(paramName, index));

            static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
            {
                if (method.Equals(propertyMethod))
                {
                    return true;
                }

                Type type = method.DeclaringType!;
                if (type.IsInterface && method.Name == propertyMethod.Name && type.GetMethod(method.Name) == propertyMethod)
                {
                    return true;
                }

                return false;
            }
        }

        internal static bool ParameterIsAssignable(ParameterInfo pi, Type argType)
        {
            Type pType = pi.ParameterType;
            if (pType.IsByRef)
                pType = pType.GetElementType()!;
            return TypeUtils.AreReferenceAssignable(pType, argType);
        }

        internal static void ValidateIndexedProperty(Expression? instance, PropertyInfo indexer, ref ReadOnlyCollection<Expression> argList)
        {
            //
            // NB: Avoid a lot of copy-paste from IndexExpression.ValidateIndexedProperty by simply calling the public API
            //     and let it do the checks.
            //

            var res = Expression.Property(instance, indexer, argList);
            argList = res.Arguments;
        }

        internal static MethodInfo GetInvokeMethod(Expression expression)
        {
            Type delegateType = expression.Type;
            if (!expression.Type.IsSubclassOf(typeof(MulticastDelegate)))
            {
                Type? exprType = TypeUtils.FindGenericType(typeof(Expression<>), expression.Type);
                if (exprType == null)
                {
                    throw ExpressionTypeNotInvocable(expression.Type, nameof(expression));
                }
                delegateType = exprType.GetGenericArguments()[0];
            }

            return delegateType.GetInvokeMethod();
        }

        internal static void ValidateStaticOrInstanceMethod(Expression? instance, MethodInfo method)
        {
            if (method.IsStatic)
            {
                if (instance != null) throw OnlyStaticMethodsHaveNullInstance();
            }
            else
            {
                if (instance == null) throw OnlyStaticMethodsHaveNullInstance();
                RequiresCanRead(instance, nameof(instance));
                ValidateCallInstanceType(instance.Type, method);
            }
        }

        internal static void ValidateCallInstanceType(Type instanceType, MethodInfo method)
        {
            if (!TypeUtils.IsValidInstanceType(method, instanceType))
            {
                throw InstanceAndMethodTypeMismatch(method, method.DeclaringType, instanceType);
            }
        }

        internal static bool IsLiftedLogical(this BinaryExpression b)
        {
            Type left = b.Left.Type;
            Type right = b.Right.Type;
            MethodInfo? method = b.Method;
            ExpressionType kind = b.NodeType;

            return
                (kind == ExpressionType.AndAlso || kind == ExpressionType.OrElse) &&
                TypeUtils.AreEquivalent(right, left) &&
                left.IsNullableType() &&
                method != null &&
                TypeUtils.AreEquivalent(method.ReturnType, left.GetNonNullableType());
        }

        private static string? GetParamName(string? paramName, int index) => index >= 0 ? $"{paramName}[{index}]" : paramName;
    }
}
