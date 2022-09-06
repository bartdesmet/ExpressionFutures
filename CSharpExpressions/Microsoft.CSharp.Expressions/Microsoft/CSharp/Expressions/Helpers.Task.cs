// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static bool IsTaskLikeType(Type type, [NotNullWhen(true)] out Type? resultType)
        {
            if (TryGetAsyncMethodBuilderInfo(type, out var info))
            {
                resultType = info.ResultType;
                return true;
            }

            resultType = null;
            return false;
        }

        public static AsyncMethodBuilderInfo GetAsyncMethodBuilderInfo(Type taskType)
        {
            if (!TryGetAsyncMethodBuilderInfo(taskType, out var info))
            {
                throw ContractUtils.Unreachable;
            }

            return info;
        }

        private static bool TryGetAsyncMethodBuilderInfo(Type taskType, out AsyncMethodBuilderInfo builderInfo)
        {
            if (!TryGetBuilderType(taskType, out var builderType))
            {
                builderInfo = default;
                return false;
            }

            if (!TryGetAsyncMethodBuilderInfoFromBuilderType(builderType, out builderInfo))
            {
                return false;
            }

            return (builderInfo.Task?.PropertyType ?? typeof(void)) == taskType;
        }

        private static bool TryGetBuilderType(Type taskType, [NotNullWhen(true)] out Type? builderType)
        {
            if (taskType.IsGenericType)
            {
                var genArgs = taskType.GetGenericArguments();

                if (taskType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    builderType = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(genArgs[0]);
                    return true;
                }

                if (genArgs.Length == 1 && TryGetBuilderTypeFromAttribute(taskType, out builderType))
                {
                    if (!builderType.IsGenericType || builderType.GetGenericArguments().Length != 1)
                    {
                        return false;
                    }

                    builderType = builderType.MakeGenericType(genArgs[0]);
                    return true;
                }
            }
            else
            {
                if (taskType == typeof(void))
                {
                    builderType = typeof(AsyncVoidMethodBuilder);
                    return true;
                }
                else if (taskType == typeof(Task))
                {
                    builderType = typeof(AsyncTaskMethodBuilder);
                    return true;
                }

                return TryGetBuilderTypeFromAttribute(taskType, out builderType);
            }

            builderType = null;
            return false;
        }

        private static bool TryGetBuilderTypeFromAttribute(Type type, [NotNullWhen(true)] out Type? builderType)
        {
            if (type.IsDefined(typeof(AsyncMethodBuilderAttribute)))
            {
                var attr = type.GetCustomAttribute<AsyncMethodBuilderAttribute>()!;

                builderType = attr.BuilderType;
                return true;
            }

            builderType = null;
            return false;
        }

        private static bool TryGetAsyncMethodBuilderInfoFromBuilderType(Type builderType, out AsyncMethodBuilderInfo info)
        {
            info = new AsyncMethodBuilderInfo { BuilderType = builderType };

            //
            // TODO: We could cache this info, but need to deal with closed generic forms of builderType.
            //

            if (!TryGetCreate(ref info) || !TryGetStart(ref info) || !TryGetSetException(ref info) || !TryGetAwaitOnCompleted(ref info) || !TryGetSetResult(ref info) || !TryGetTask(ref info))
            {
                return false;
            }

            static bool TryGetCreate(ref AsyncMethodBuilderInfo info)
            {
                var create = info.BuilderType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);

                if (create == null || create.IsGenericMethod || create.GetParameters().Length != 0 || create.ReturnType != info.BuilderType)
                {
                    return false;
                }

                info.Create = create;

                return true;
            }

            static bool TryGetStart(ref AsyncMethodBuilderInfo info)
            {
                var start = info.BuilderType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance);

                if (start == null || !start.IsGenericMethod || start.ReturnType != typeof(void))
                {
                    return false;
                }

                var startGenArgs = start.GetGenericArguments();

                if (startGenArgs.Length != 1 || !startGenArgs[0].GetInterfaces().Contains(typeof(IAsyncStateMachine)))
                {
                    return false;
                }

                var startParameters = start.GetParameters();

                if (startParameters.Length != 1 || startParameters[0].ParameterType != startGenArgs[0].MakeByRefType())
                {
                    return false;
                }

                info.Start = start;

                return true;
            }

            static bool TryGetSetException(ref AsyncMethodBuilderInfo info)
            {
                var setException = info.BuilderType.GetMethod("SetException", BindingFlags.Public | BindingFlags.Instance);

                if (setException == null || setException.IsGenericMethod || setException.ReturnType != typeof(void))
                {
                    return false;
                }

                var setExceptionParameters = setException.GetParameters();

                if (setExceptionParameters.Length != 1 || setExceptionParameters[0].ParameterType != typeof(Exception))
                {
                    return false;
                }

                info.SetException = setException;

                return true;
            }

            static bool TryGetAwaitOnCompleted(ref AsyncMethodBuilderInfo info)
            {
                var awaitOnCompleted = info.BuilderType.GetMethod("AwaitOnCompleted", BindingFlags.Public | BindingFlags.Instance);

                if (awaitOnCompleted == null || awaitOnCompleted.ReturnType != typeof(void) || !awaitOnCompleted.IsGenericMethod)
                {
                    return false;
                }

                var awaitOnCompletedGenArgs = awaitOnCompleted.GetGenericArguments();

                if (awaitOnCompletedGenArgs.Length != 2 || !awaitOnCompletedGenArgs[0].GetInterfaces().Contains(typeof(INotifyCompletion)) || !awaitOnCompletedGenArgs[1].GetInterfaces().Contains(typeof(IAsyncStateMachine)))
                {
                    return false;
                }

                var awaitOnCompletedParameters = awaitOnCompleted.GetParameters();

                if (awaitOnCompletedParameters.Length != 2 || awaitOnCompletedParameters[0].ParameterType != awaitOnCompletedGenArgs[0].MakeByRefType() || awaitOnCompletedParameters[1].ParameterType != awaitOnCompletedGenArgs[1].MakeByRefType())
                {
                    return false;
                }

                info.AwaitOnCompleted = awaitOnCompleted;

                return true;
            }

            static bool TryGetSetResult(ref AsyncMethodBuilderInfo info)
            {
                var setResult = info.BuilderType.GetMethod("SetResult", BindingFlags.Public | BindingFlags.Instance);

                if (setResult == null || setResult.ReturnType != typeof(void))
                {
                    return false;
                }

                var setResultParameters = setResult.GetParameters();

                if (setResultParameters.Length == 0)
                {
                    info.ResultType = typeof(void);
                }
                else if (setResultParameters.Length == 1)
                {
                    var resultType = setResultParameters[0].ParameterType;

                    if (resultType.IsByRef)
                    {
                        return false;
                    }

                    info.ResultType = resultType;
                }
                else
                {
                    return false;
                }

                info.SetResult = setResult;

                return true;
            }

            static bool TryGetTask(ref AsyncMethodBuilderInfo info)
            {
                if (info.BuilderType != typeof(AsyncVoidMethodBuilder))
                {
                    var task = info.BuilderType.GetProperty("Task", BindingFlags.Public | BindingFlags.Instance);

                    if (task == null || task.GetIndexParameters().Length != 0 || !task.CanRead)
                    {
                        return false;
                    }

                    info.Task = task;
                }

                return true;
            }

            return true;
        }
    }

    internal struct AsyncMethodBuilderInfo
    {
        public Type BuilderType;
        public MethodInfo Create;
        public MethodInfo Start;
        public MethodInfo SetResult;
        public MethodInfo SetException;
        public MethodInfo AwaitOnCompleted;
        public PropertyInfo Task;
        public Type ResultType;
    }
}
