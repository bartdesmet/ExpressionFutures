// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Dynamic.Utils.TypeUtils;

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

        public static void ValidateParameterBindings(MethodBase method, ReadOnlyCollection<ParameterAssignment> argList, bool extensionMethod = false)
        {
            ValidateParameterBindings(method, method.GetParametersCached(), argList, extensionMethod);
        }

        public static void ValidateParameterBindings(MethodBase method, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> argList, bool extensionMethod = false)
        {
            var boundParameters = new HashSet<ParameterInfo>();

            if (extensionMethod)
            {
                boundParameters.Add(parameters[0]);
            }

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

        public static ReadOnlyCollection<ParameterAssignment> GetParameterBindings(MethodBase method, IEnumerable<Expression> expressions)
        {
            return GetParameterBindings(method.GetParametersCached(), expressions);
        }

        public static ReadOnlyCollection<ParameterAssignment> GetParameterBindings(ParameterInfo[] parameters, IEnumerable<Expression> expressions)
        {
            var arguments = expressions.ToReadOnly();

            var n = arguments.Count;

            if (n > parameters.Length)
            {
                throw Error.TooManyArguments();
            }

            var bindings = new ParameterAssignment[n];

            for (var i = 0; i < n; i++)
            {
                bindings[i] = CSharpExpression.Bind(parameters[i], arguments[i]);
            }

            return new TrueReadOnlyCollection<ParameterAssignment>(bindings);
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
            if (type.IsInterface)
            {
                if (typeof(IDisposable).IsAssignableFrom(type))
                {
                    return typeof(IDisposable).GetMethod("Dispose");
                }
            }

            // REVIEW: This may pose challenges on .NET Native
            var map = type.GetInterfaceMap(typeof(IDisposable));
            return map.TargetMethods.Single(); // NB: IDisposable has only one method
        }

        public static bool IsVector(this Type type)
        {
            return type.IsArray && type.GetElementType().MakeArrayType() == type;
        }

        public static Expression BindArguments(Func<Expression, Expression[], Expression> create, Expression instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings)
        {
            var res = default(Expression);

            var arguments = new Expression[parameters.Length];

            if (CheckArgumentsInOrder(bindings))
            {
                foreach (var binding in bindings)
                {
                    arguments[binding.Parameter.Position] = binding.Expression;
                }

                FillOptionalParameters(parameters, arguments);

                res = create(instance, arguments);
            }
            else
            {
                var variables = new List<ParameterExpression>();
                var statements = new List<Expression>();

                var obj = default(Expression);
                var writebacks = default(Expression[]);
                RewriteArguments(instance, bindings, variables, statements, ref obj, arguments, out writebacks);

                FillOptionalParameters(parameters, arguments);

                var expr = create(obj, arguments);

                if (writebacks.Length != 0)
                {
                    if (expr.Type != typeof(void))
                    {
                        var resultVariable = Expression.Parameter(expr.Type);
                        variables.Add(resultVariable);

                        statements.Add(Expression.Assign(resultVariable, expr));
                        statements.AddRange(writebacks);
                        statements.Add(resultVariable);
                    }
                    else
                    {
                        statements.Add(expr);
                        statements.AddRange(writebacks);
                    }
                }
                else
                {
                    statements.Add(expr);
                }

                res = Expression.Block(expr.Type, variables, statements);
            }

            return res;
        }

        private static void RewriteArguments(Expression instance, ReadOnlyCollection<ParameterAssignment> bindings, List<ParameterExpression> variables, List<Expression> statements, ref Expression obj, Expression[] arguments, out Expression[] writebacks)
        {
            var writebackList = default(List<Expression>);

            if (instance != null)
            {
                if (instance.IsPure())
                {
                    obj = instance;
                }
                else
                {
                    var var = Expression.Parameter(instance.Type, "obj");
                    variables.Add(var);

                    if (instance.Type.IsValueType && !instance.Type.IsPrimitive /* immutable */)
                    {
                        EnsureWriteback(var, ref instance, variables, statements, ref writebackList);
                    }

                    statements.Add(Expression.Assign(var, instance));

                    obj = var;
                }
            }

            foreach (var argument in bindings)
            {
                var parameter = argument.Parameter;
                var expression = argument.Expression;

                if (expression.IsPure()) // NB: no side-effect to read or write
                {
                    arguments[parameter.Position] = expression;
                }
                else
                {
                    var var = Expression.Parameter(argument.Expression.Type, parameter.Name);
                    variables.Add(var);

                    if (parameter.IsByRefParameter())
                    {
                        EnsureWriteback(var, ref expression, variables, statements, ref writebackList);
                    }

                    statements.Add(Expression.Assign(var, expression));

                    arguments[parameter.Position] = var;
                }
            }

            writebacks = writebackList?.ToArray() ?? Array.Empty<Expression>();
        }

        private static void EnsureWriteback(ParameterExpression variable, ref Expression expression, List<ParameterExpression> variables, List<Expression> statements, ref List<Expression> writebackList)
        {
            var writeback = default(Expression);
            if (TryGetWriteback(variable, ref expression, variables, statements, out writeback))
            {
                if (writebackList == null)
                {
                    writebackList = new List<Expression>();
                }

                writebackList.Add(writeback);
            }
        }

        private static bool IsPure(this Expression expression)
        {
            // TODO: Convert and ConvertChecked can be pure under certain circumstances as well

            switch (expression.NodeType)
            {
                case ExpressionType.Default:
                case ExpressionType.Parameter:
                case ExpressionType.Constant:
                case ExpressionType.Unbox:
                case ExpressionType.Lambda:
                case ExpressionType.Quote:
                    return true;
            }

            return false;
        }

        private static bool TryGetWriteback(ParameterExpression variable, ref Expression expression, List<ParameterExpression> variables, List<Expression> statements, out Expression writeback)
        {
            writeback = null;

            switch (expression.NodeType)
            {
                // NB: Should not encounter Parameter or Unbox; those are considered pure and will be passed straight on

                // TODO: Add support for our new C# indexer node type as well? Note that C# doesn't have this type of by-ref passing anyway, 
                //       but for equivalent VB APIs we'd need it. We could dispatch to a helper method on the node type in order to reuse
                //       its Reduce logic in emitting the required locals and updated node that supports write-back.

                case ExpressionType.Index:
                    {
                        var index = (IndexExpression)expression;

                        if (index.Indexer?.CanWrite ?? true) // NB: no indexer means array access, hence writeable
                        {
                            var obj = index.Object;
                            var args = index.Arguments;

                            var newObj = default(ParameterExpression);
                            if (obj != null)
                            {
                                newObj = Expression.Parameter(obj.Type);
                                variables.Add(newObj);
                                statements.Add(Expression.Assign(newObj, obj));
                            }

                            var n = args.Count;
                            var newArgs = new ParameterExpression[n];

                            for (var i = 0; i < n; i++)
                            {
                                var arg = args[i];
                                var newArg = Expression.Parameter(arg.Type);
                                newArgs[i] = newArg;
                                variables.Add(newArg);
                                statements.Add(Expression.Assign(newArg, arg));
                            }

                            var newIndex = index.Update(newObj, newArgs);
                            expression = newIndex;

                            writeback = Expression.Assign(newIndex, variable);
                        }
                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        var member = (MemberExpression)expression;

                        var writeable = false;

                        var prop = member.Member as PropertyInfo;
                        if (prop != null)
                        {
                            if (prop.CanWrite)
                            {
                                writeable = true;
                            }
                        }
                        else
                        {
                            var field = member.Member as FieldInfo;
                            if (field != null && !(field.IsInitOnly || field.IsLiteral))
                            {
                                writeable = true;
                            }
                        }

                        if (writeable)
                        {
                            var obj = member.Expression;

                            var newObj = default(ParameterExpression);
                            if (obj != null)
                            {
                                newObj = Expression.Parameter(obj.Type);
                                variables.Add(newObj);
                                statements.Add(Expression.Assign(newObj, obj));
                            }

                            var newMember = member.Update(newObj);
                            expression = newMember;

                            writeback = Expression.Assign(newMember, variable);
                        }
                    }
                    break;
                case ExpressionType.ArrayIndex:
                    {
                        var binary = (BinaryExpression)expression;

                        Debug.Assert(binary.Conversion == null);

                        var left = binary.Left;
                        var newLeft = Expression.Parameter(left.Type);
                        variables.Add(newLeft);
                        statements.Add(Expression.Assign(newLeft, left));

                        var right = binary.Right;
                        var newRight = Expression.Parameter(right.Type);
                        variables.Add(newRight);
                        statements.Add(Expression.Assign(newRight, right));

                        var newBinary = Expression.ArrayAccess(newLeft, newRight);
                        expression = newBinary;

                        writeback = Expression.Assign(newBinary, variable);
                    }
                    break;
                case ExpressionType.Call:
                    {
                        var call = (MethodCallExpression)expression;

                        var obj = call.Object;
                        var method = call.Method;
                        if (!method.IsStatic && obj.Type.IsArray && method == obj.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance))
                        {
                            var args = call.Arguments;

                            var newObj = default(ParameterExpression);
                            if (obj != null)
                            {
                                newObj = Expression.Parameter(obj.Type);
                                variables.Add(newObj);
                                statements.Add(Expression.Assign(newObj, obj));
                            }

                            var n = args.Count;
                            var newArgs = new ParameterExpression[n];

                            for (var i = 0; i < n; i++)
                            {
                                var arg = args[i];
                                var newArg = Expression.Parameter(arg.Type);
                                newArgs[i] = newArg;
                                variables.Add(newArg);
                                statements.Add(Expression.Assign(newArg, arg));
                            }

                            var newCall = Expression.ArrayAccess(newObj, newArgs);
                            expression = newCall;

                            writeback = Expression.Assign(newCall, variable);
                        }
                    }
                    break;
            }

            return writeback != null;
        }

        public static Type GetConditionalType(this Type type)
        {
            if (type.IsValueType && type != typeof(void) && !type.IsNullableType())
            {
                return GetNullableType(type);
            }

            return type;
        }

        public static Type GetNonNullReceiverType(this Type type)
        {
            // DESIGN: Should we reject non-nullable value types here?
            return type.GetNonNullableType();
        }

        public static void CopyReceiverArgument(Expression receiver, CSharpArgumentInfo[] argumentInfos, Expression[] expressions, ref Type[] argumentTypes)
        {
            var receiverFlags = CSharpArgumentInfoFlags.None;
            if (IsReceiverByRef(receiver))
            {
                receiverFlags |= CSharpArgumentInfoFlags.IsRef;
                argumentTypes = new Type[argumentInfos.Length];
                argumentTypes[0] = receiver.Type.MakeByRefType();
            }

            expressions[0] = receiver;
            argumentInfos[0] = CSharpArgumentInfo.Create(receiverFlags, null);
        }

        public static void CopyArguments(ReadOnlyCollection<DynamicCSharpArgument> arguments, CSharpArgumentInfo[] argumentInfos, Expression[] expressions, ref Type[] argumentTypes)
        {
            var n = arguments.Count;

            for (var i = 0; i < n; i++)
            {
                var argument = arguments[i];
                argumentInfos[i + 1] = argument.ArgumentInfo;
                expressions[i + 1] = argument.Expression;

                if ((argument.Flags & (CSharpArgumentInfoFlags.IsRef | CSharpArgumentInfoFlags.IsOut)) != 0)
                {
                    if (argumentTypes == null)
                    {
                        argumentTypes = new Type[argumentInfos.Length];
                        argumentTypes[0] = expressions[0].Type;

                        for (var j = 0; j < i; j++)
                        {
                            argumentTypes[j + 1] = arguments[j].Expression.Type;
                        }
                    }

                    argumentTypes[i + 1] = argument.Expression.Type.MakeByRefType();
                }
                else if (argumentTypes != null)
                {
                    argumentTypes[i + 1] = argument.Expression.Type;
                }
            }
        }

        public static bool IsReceiverByRef(Expression expression)
        {
            // NB: Mimics behavior of GetReceiverRefKind in the C# compiler.
            
            // DESIGN: Should we keep the ArgumentInfo object for the receiver in our dynamic nodes instead?
            //         Makes the shape of the tree wonky though, so we attempt to infer the behavior here.

            if (expression.Type.IsValueType)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Parameter:
                    case ExpressionType.ArrayIndex:
                        return true;
                }
            }

            return false;
        }
    }
}
