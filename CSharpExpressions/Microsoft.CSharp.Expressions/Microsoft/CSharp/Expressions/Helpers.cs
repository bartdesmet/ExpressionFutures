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

        public static MethodInfo FindDisposeMethod(this Type type)
        {
            if (type.IsInterface)
            {
                if (typeof(IDisposable).IsAssignableFrom(type))
                {
                    return typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));
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

        public static Expression BindArguments(Func<Expression, Expression[], Expression> create, Expression instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings, bool needTemps = false)
        {
            var res = default(Expression);

            var arguments = new Expression[parameters.Length];

            if (!needTemps && CheckArgumentsInOrder(bindings))
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
                        var resultVariable = Expression.Parameter(expr.Type, "__result");
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
                    var isByRef = parameter.IsByRefParameter();

                    var var = expression as ParameterExpression;

                    // REVIEW: We don't want to create a copy of a variable in a local if it's passed by
                    //         ref because it'd break atomicity. Note that Block doesn't support locals
                    //         which are ByRef types, so we can't store the reference.
                    //
                    //         See MakeArguments in LocalRewriter_Call.cs in Roslyn for cases to review.

                    if (var == null || !isByRef)
                    {
                        var = Expression.Parameter(argument.Expression.Type, parameter.Name);
                        variables.Add(var);

                        if (isByRef)
                        {
                            EnsureWriteback(var, ref expression, variables, statements, ref writebackList);
                        }

                        statements.Add(Expression.Assign(var, expression));
                    }

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

        public static bool IsPure(this Expression expression, bool readOnly = false)
        {
            // TODO: Convert and ConvertChecked can be pure under certain circumstances as well

            switch (expression.NodeType)
            {
                case ExpressionType.Default:
                case ExpressionType.Constant:
                case ExpressionType.Unbox:
                case ExpressionType.Lambda:
                case ExpressionType.Quote:
                    return true;
                    
                // NB: Parameters are only pure if used in a read-only setting, i.e. they can be dropped without
                //     loss of side-effects. If they can be assigned to, e.g. in the context of named parameter
                //     analysis where a parameter occurs in an argument and may get assigned to, it is unsafe to
                //     consider them pure for that purpose.
                case ExpressionType.Parameter:
                    return readOnly;
            }

            return false;
        }

        private static bool TryGetWriteback(ParameterExpression variable, ref Expression expression, List<ParameterExpression> variables, List<Expression> statements, out Expression writeback)
        {
            writeback = null;

            switch (expression.NodeType)
            {
                // TODO: Add support for our new C# indexer node type as well? Note that C# doesn't have this type of by-ref passing anyway, 
                //       but for equivalent VB APIs we'd need it. We could dispatch to a helper method on the node type in order to reuse
                //       its Reduce logic in emitting the required locals and updated node that supports write-back.

                case ExpressionType.Parameter:
                    {
                        writeback = Expression.Assign(expression, variable);
                    }
                    break;

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
                                newObj = Expression.Parameter(obj.Type, "__object");
                                variables.Add(newObj);
                                statements.Add(Expression.Assign(newObj, obj));
                            }

                            var n = args.Count;
                            var newArgs = new ParameterExpression[n];

                            for (var i = 0; i < n; i++)
                            {
                                var arg = args[i];
                                var newArg = Expression.Parameter(arg.Type, "__arg" + i);
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
                                newObj = Expression.Parameter(obj.Type, "__object");
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
                        var newLeft = Expression.Parameter(left.Type, "__array");
                        variables.Add(newLeft);
                        statements.Add(Expression.Assign(newLeft, left));

                        var right = binary.Right;
                        var newRight = Expression.Parameter(right.Type, "__index");
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
                                newObj = Expression.Parameter(obj.Type, "__object");
                                variables.Add(newObj);
                                statements.Add(Expression.Assign(newObj, obj));
                            }

                            var n = args.Count;
                            var newArgs = new ParameterExpression[n];

                            for (var i = 0; i < n; i++)
                            {
                                var arg = args[i];
                                var newArg = Expression.Parameter(arg.Type, "__arg" + i);
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

        public static string ToDebugString(this object o)
        {
            return o == null ? "null" : o.ToString();
        }

        private static object s_null = new object();

        public static object OrNullSentinel(this object o)
        {
            return o ?? s_null;
        }

        public static Expression GetLhs(Expression expression, string paramName)
        {
            var lhs = expression;

            var index = expression as IndexCSharpExpression;
            if (index != null)
            {
                EnsureCanWrite(index, paramName);

                lhs = Expression.Parameter(expression.Type, "__lhs");
            }

            return lhs;
        }

        public static void RequiresCanWrite(Expression expression, string paramName)
        {
            var index = expression as IndexCSharpExpression;
            if (index != null)
            {
                EnsureCanWrite(index, paramName);
            }
            else
            {
                ExpressionStubs.RequiresCanWrite(expression, paramName);
            }
        }

        private static void EnsureCanWrite(IndexCSharpExpression index, string paramName)
        {
            if (!index.Indexer.CanWrite)
            {
                throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
            }
        }

        public static Expression ReduceAssignment(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix = true, LambdaExpression leftConversion = null)
        {
            switch (lhs.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ReduceMember(lhs, functionalOp, prefix, leftConversion);
                case ExpressionType.Index:
                    return ReduceIndex(lhs, functionalOp, prefix, leftConversion);
                case ExpressionType.Parameter:
                    return ReduceVariable(lhs, functionalOp, prefix, leftConversion);
            }

            var index = lhs as IndexCSharpExpression;
            if (index != null)
            {
                return ReduceIndexCSharp(index, functionalOp, prefix, leftConversion);
            }

            throw ContractUtils.Unreachable;
        }

        private static Expression ReduceMember(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            var res = default(Expression);

            var member = (MemberExpression)lhs;

            if (member.Expression == null)
            {
                res = ReduceVariable(lhs, functionalOp, prefix, leftConversion);
            }
            else
            {
                var lhsTemp = Expression.Parameter(member.Expression.Type, "__lhs");
                var lhsAssign = Expression.Assign(lhsTemp, member.Expression);
                member = member.Update(lhsTemp);

                if (prefix)
                {
                    res =
                        Expression.Block(
                            new[] { lhsTemp },
                            lhsAssign,
                            Expression.Assign(member, functionalOp(WithLeftConversion(member, leftConversion)))
                        );
                }
                else
                {
                    var temp = Expression.Parameter(member.Type, "__temp");

                    res =
                        Expression.Block(
                            new[] { lhsTemp, temp },
                            lhsAssign,
                            Expression.Assign(temp, member),
                            Expression.Assign(member, functionalOp(WithLeftConversion(temp, leftConversion))),
                            temp
                        );
                }
            }

            return res;
        }

        private static Expression ReduceIndex(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            var index = (IndexExpression)lhs;

            var n = index.Arguments.Count;
            var args = new Expression[n];
            var block = new Expression[n + (prefix ? 2 : 4)];
            var temps = new ParameterExpression[n + (prefix ? 1 : 2)];

            var i = 0;
            temps[i] = Expression.Parameter(index.Object.Type, "__object");
            block[i] = Expression.Assign(temps[i], index.Object);
            i++;

            while (i <= n)
            {
                var arg = index.Arguments[i - 1];
                args[i - 1] = temps[i] = Expression.Parameter(arg.Type, "__arg" + i);
                block[i] = Expression.Assign(temps[i], arg);
                i++;
            }

            index = index.Update(temps[0], new TrueReadOnlyCollection<Expression>(args));

            if (prefix)
            {
                block[i++] = Expression.Assign(index, functionalOp(WithLeftConversion(index, leftConversion)));
            }
            else
            {
                var lastTemp = temps[i] = Expression.Parameter(index.Type, "__index");

                block[i] = Expression.Assign(temps[i], index);
                i++;

                block[i++] = Expression.Assign(index, functionalOp(WithLeftConversion(lastTemp, leftConversion)));
                block[i++] = lastTemp;
            }

            var res = Expression.Block(temps, block);
            return res;
        }

        private static Expression ReduceVariable(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            var res = default(Expression);

            if (prefix)
            {
                res = Expression.Assign(lhs, functionalOp(WithLeftConversion(lhs, leftConversion)));
            }
            else
            {
                var temp = Expression.Parameter(lhs.Type, "__temp");
                res =
                    Expression.Block(
                        new[] { temp },
                        Expression.Assign(temp, lhs),
                        Expression.Assign(lhs, functionalOp(WithLeftConversion(temp, leftConversion))),
                        temp
                    );
            }

            return res;
        }

        private static Expression ReduceIndexCSharp(IndexCSharpExpression index, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            return index.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
        }

        private static Expression WithLeftConversion(Expression expression, LambdaExpression leftConversion)
        {
            if (leftConversion != null)
            {
                expression = Apply(leftConversion, expression);
            }

            return expression;
        }

        public static Expression Apply(LambdaExpression lambda, Expression expression)
        {
            // TODO: We can beta reduce the lambda explicitly here, iff the specified expression is pure
            //       or is used exactly once in the lambda before any other observable side-effect (i.e.
            //       don't cause re-evalation or out-of-order evaluation of e.g. an indexer or member).
            //
            //       The code below does a basic version of this, well-suited for the conversions that
            //       get generated by the compound assignment factories. See a DESIGN note in BinaryAssign
            //       for considerations on not using a lambda for that case.
            //
            //       Note that the LINQ expression compiler also inlines Invoke(Lambda, args) expressions
            //       so this optimization is mostly to make Reduce return a nicer form.

            if (lambda.Parameters.Count == 1)
            {
                var parameter = lambda.Parameters[0];
                var body = lambda.Body;

                switch (body.NodeType)
                {
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        var convert = (UnaryExpression)body;
                        if (convert.Operand == parameter)
                        {
                            return convert.Update(expression);
                        }
                        break;
                }
            }

            return Expression.Invoke(lambda, expression);
        }

        public static Expression CreateVoid(params Expression[] expressions)
        {
            return CreateVoid((IList<Expression>)expressions);
        }

        public static Expression CreateVoid(IList<Expression> expressions)
        {
            if (expressions == null || expressions.Count == 0)
            {
                return Expression.Empty();
            }

            if (expressions.Count == 1)
            {
                var expression = expressions[0];

                if (expression.Type == typeof(void))
                {
                    return expression;
                }

                var block = expression as BlockExpression;
                if (block != null)
                {
                    return Expression.Block(typeof(void), block.Variables, block.Expressions);
                }
            }

            return Expression.Block(typeof(void), expressions);
        }
    }
}
