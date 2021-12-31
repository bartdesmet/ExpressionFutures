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
using System.Threading.Tasks;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    static class Helpers
    {
        public static bool SameElements<T>(ref IEnumerable<T> replacement, IReadOnlyList<T> current) where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current.Count == 0;
            }

            // Ensure arguments is safe to enumerate twice.
            // If we have to build a collection, build a TrueReadOnlyCollection<T>
            // so it won't be built a second time if used.
            if (!(replacement is ICollection<T> replacementCol))
            {
                replacement = replacementCol = replacement.ToReadOnly();
            }

            return SameElementsInCollection(replacementCol, current);
        }

        public static bool SameElementsInCollection<T>(ICollection<T> replacement, IReadOnlyList<T> current) where T : class
        {
            int count = current.Count;

            if (replacement.Count != count)
            {
                return false;
            }

            if (count != 0)
            {
                int index = 0;

                foreach (T replacementObject in replacement)
                {
                    if (replacementObject != current[index])
                    {
                        return false;
                    }

                    index++;
                }
            }

            return true;
        }

        public static bool IsConst(Expression e, bool value)
        {
            return e is ConstantExpression { Value: var val } && val is bool b && b == value;
        }

        public static bool IsAlwaysNull(Expression e)
        {
            // NB: Could add support for no-op conversions.

            return e.NodeType switch
            {
                ExpressionType.Constant => ((ConstantExpression)e).Value == null,
                ExpressionType.Default => !e.Type.IsValueType || e.Type.IsNullableType(),
                ExpressionType.New => e.Type.IsNullableType() && ((NewExpression)e).Arguments.Count == 0, // e.g. new int?()
                _ => false,
            };
        }

        public static bool IsNeverNull(Expression e)
        {
            // NB: Could add support for no-op conversions.

            return e.NodeType switch
            {
                ExpressionType.Constant => ((ConstantExpression)e).Value != null,
                ExpressionType.New => e.Type.IsNullableType() && ((NewExpression)e).Arguments.Count == 1, // e.g. new int?(42)
                _ => false,
            };
        }

        public static Expression MakeNullableHasValue(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Property(e, "HasValue");
        }

        public static Expression MakeNullableGetValueOrDefault(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Call(e, "GetValueOrDefault", typeArguments: null);
        }

        public static Expression MakeNullableGetValue(Expression e)
        {
            Debug.Assert(e.Type.IsNullableType());

            return Expression.Property(e, "Value");
        }

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

        public static bool CheckHasSpecialNodesPassedByRef(ReadOnlyCollection<ParameterAssignment> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Parameter.ParameterType.IsByRef && argument.Expression is ArrayAccessCSharpExpression)
                {
                    return true;
                }
            }

            return false;
        }

        public static void FillOptionalParameters(ParameterInfo[] parameters, Expression[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;

                    // REVIEW: It seems this can occur when a generic parameter has a default
                    //         value of as default(T). Check whether this only applies to
                    //         closed generic parameters or could we miss proper non-trivial
                    //         default parameter values here?

                    if (parameter.DefaultValue == null)
                    {
                        args[i] = Expression.Default(parameterType);
                    }
                    else
                    {
                        args[i] = Expression.Constant(parameter.DefaultValue, parameterType);
                    }

#if ENABLE_CALLERINFO

                    // REVIEW: Should we support caller info attributes? Replacing the optional
                    //         parameters at compile time doesn't make sense given there's no
                    //         caller in an expression tree. Retrieving the information at runtime
                    //         can be expensive and relies on debugging information to be present
                    //         so it may not be as reliable. Furthermore, it is debatable what the
                    //         `caller` is in this context; this is even true for delegates:
                    //
                    //           static void CallerInfo([CallerMemberName]string member = null) => Print(member);
                    //           static Action A() => () => CallerInfo();
                    //           static void Main() => A()();
                    //
                    //         In this case, the C# compiler will emit A as the member name, even
                    //         though the caller is really the lambda (or anonymous method) that's
                    //         kept in the delegate. This is more like the definition site rather
                    //         than the call site, which is understandable given the syntactic site
                    //         is the only thing the compiler has around to substitute the optional
                    //         parameter. Retrieving the caller info for the expression equivalent
                    //         of A at runtime doesn't have that lexical information available and
                    //         would have to resort to using System.Diagnostics.StackTrace which
                    //         will return lambda_method as the caller with no file and line info
                    //         available. We could skip to the nearest non-LCG member in the call
                    //         stack, but that's strange when nested lambdas are used. Either way,
                    //         the notion of a `caller` as implemented by the C# compiler is not
                    //         reconstructable at runtime.
                    //
                    //         Based on these observations, maybe filling in the definition site
                    //         of the expression at compile time isn't that bad after all. But if
                    //         so, should all optional parameters get filled in at (Roslyn) compile
                    //         time? The deferred nature of expression compilation doesn't make this
                    //         the most obvious choice; after all, the expression can be serialized
                    //         and (runtime) compiled in a different space/time where the optional
                    //         parameter values are different. It would be indistinguishable to the
                    //         consumer of the tree that those parameters didn't get their argument
                    //         values from the user, unless we add a flag to our ParameterAssignment
                    //         to indicate it was an optional parameter that got filled in by the
                    //         (Roslyn) compiler. Obviously there are other cases where the original
                    //         user intent gets lost (e.g. the expanded form of a params argument),
                    //         but omitting optional parameters from the expression tree seems not
                    //         particularly bad, given we can get all information at runtime (to
                    //         be checked in .NET Native) and runtime expression analyzers can get
                    //         a clearer picture of the user intent.
                    //
                    //         Essentially, burning optional parameter default values at compile
                    //         time is now ambiguous because we can have different compile stages.

                    var callerInfoMethod = default(MethodInfo);

                    if (parameter.IsDefined(typeof(CallerMemberNameAttribute)))
                    {
                        callerInfoMethod = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetCallerMemberName));
                    }
                    else if (parameter.IsDefined(typeof(CallerLineNumberAttribute)))
                    {
                        callerInfoMethod = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetCallerLineNumber));
                    }
                    else if (parameter.IsDefined(typeof(CallerFilePathAttribute)))
                    {
                        callerInfoMethod = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetCallerFilePath));
                    }

                    // REVIEW: The below only takes implicit conversions into account for the
                    //         coalesce operations. Should we use a Convert expression to allow
                    //         for user-defined conversions here as well? According to 17.4.4
                    //         only standard implicit conversions are to be considered, so likely
                    //         this suffices.

                    if (callerInfoMethod != null)
                    {
                        args[i] = Expression.Coalesce(Expression.Call(callerInfoMethod), args[i]);
                    }

#endif
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
                var consts = s_constInt32 ??= new ConstantExpression[MaxConstInt32 - MinConstInt32 + 1];
                return consts[index] ?? (consts[index] = Expression.Constant(value));
            }

            return Expression.Constant(value);
        }

        private static ConstantExpression s_true, s_false;

        public static ConstantExpression ConstantTrue => s_true ??= Expression.Constant(true);
        public static ConstantExpression ConstantFalse => s_false ??= Expression.Constant(false);

        public static ConstantExpression CreateConstantBoolean(bool value) => value ? ConstantTrue : ConstantFalse;

        private static ConstantExpression s_nullConstant;

        public static ConstantExpression ConstantNull => s_nullConstant ??= Expression.Constant(null, typeof(object));

        public static Expression CreateConvert(Expression expr, Type type)
        {
            if (AreEquivalent(expr.Type, type))
            {
                return expr;
            }

            return Expression.Convert(expr, type);
        }

        public static Expression InvokeLambdaWithSingleParameter(LambdaExpression lambda, Expression argument)
        {
            var body = lambda.Body.ReduceExtensions();

            if (body is MethodCallExpression call)
            {
                var parameter = lambda.Parameters[0];
                var method = call.Method;

                if (method.IsStatic)
                {
                    if (call.Arguments.Count == 1 && call.Arguments[0] == parameter)
                    {
                        return call.Update(call.Object, new[] { argument });
                    }
                }
                else
                {
                    if (call.Arguments.Count == 0 && call.Object == parameter)
                    {
                        return call.Update(argument, call.Arguments);
                    }
                }
            }

            return Expression.Invoke(lambda, argument);
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

        public static MethodInfo FindDisposeMethod(this Type type, bool isAsync)
        {
            var disposableInterface = isAsync ? typeof(IAsyncDisposable) : typeof(IDisposable);

            if (type.IsInterface)
            {
                if (disposableInterface.IsAssignableFrom(type))
                {
                    var disposeMethodName = isAsync ? nameof(IAsyncDisposable.DisposeAsync) : nameof(IDisposable.Dispose);

                    return disposableInterface.GetMethod(disposeMethodName);
                }
            }

            // REVIEW: This may pose challenges on .NET Native
            var map = type.GetInterfaceMap(disposableInterface);
            return map.TargetMethods.Single(); // NB: I[Async]Disposable has only one method
        }

        public static bool IsVector(this Type type)
        {
            return type.IsArray && type.GetElementType().MakeArrayType() == type;
        }

        public static Expression BindArguments(Func<Expression, Expression[], Expression> create, Expression instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings, bool needTemps = false)
        {
            var res = default(Expression);

            if (!needTemps && CheckArgumentsInOrder(bindings) && !CheckHasSpecialNodesPassedByRef(bindings))
            {
                var arguments = new Expression[parameters.Length];

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

                var expr = BindArguments(create, instance, parameters, bindings, variables, statements);

                // NB: Peephole for Expression.Empty() returned below.

                if (!(expr is DefaultExpression && expr.Type == typeof(void)))
                {
                    statements.Add(expr);
                }

                res = Expression.Block(expr.Type, variables, statements);
            }

            return res;
        }

        public static Expression BindArguments(Func<Expression, Expression[], Expression> create, Expression instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings, List<ParameterExpression> variables, List<Expression> statements)
        {
            var arguments = new Expression[parameters.Length];

            var makeVariable = new Func<Type, string, ParameterExpression>((type, name) =>
            {
                var var = Expression.Parameter(type, name);
                variables.Add(var);
                return var;
            });

            var obj = default(Expression);
            RewriteArguments(instance, bindings, makeVariable, statements, ref obj, arguments, out Expression[] writebacks);

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
                    return resultVariable;
                }
                else
                {
                    statements.Add(expr);
                    statements.AddRange(writebacks);
                    return Expression.Empty();
                }
            }
            else
            {
                return expr;
            }
        }

        private static void RewriteArguments(Expression instance, ReadOnlyCollection<ParameterAssignment> bindings, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements, ref Expression obj, Expression[] arguments, out Expression[] writebacks)
        {
            var writebackList = default(List<Expression>);

            if (instance != null)
            {
                obj = instance;

                if (!instance.IsPure())
                {
                    if (IsMutableStruct(instance.Type))
                    {
                        RewriteByRefArgument(null, ref obj, makeVariable, statements, ref writebackList);
                    }
                    else
                    {
                        var var = makeVariable(instance.Type, "__obj");
                        statements.Add(Expression.Assign(var, instance));
                        obj = var;
                    }
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

                    if (isByRef)
                    {
                        // REVIEW: We don't want to create a copy of a variable in a local if it's passed by
                        //         ref because it'd break atomicity. Note that Block doesn't support locals
                        //         which are ByRef types, so we can't store the reference.
                        //
                        //         See MakeArguments in LocalRewriter_Call.cs in Roslyn for cases to review.

                        RewriteByRefArgument(parameter, ref expression, makeVariable, statements, ref writebackList);
                        arguments[parameter.Position] = expression;
                    }
                    else
                    {
                        var var = CreateTemp(makeVariable, parameter, argument.Expression.Type);
                        statements.Add(Expression.Assign(var, expression));
                        arguments[parameter.Position] = var;
                    }
                }
            }

            writebacks = writebackList?.ToArray() ?? Array.Empty<Expression>();
        }

        public static bool IsMutableStruct(Type type)
        {
            return type.IsValueType && !type.IsPrimitive /* immutable */;
        }

        public static bool IsPure(this Expression expression, bool readOnly = false)
        {
            //
            // CONSIDER: Improve purity analysis to handle more cases.
            //
            // - Convert can be pure for some cases when applied to a pure operand, e.g.
            //   - nullable lifting T -> T?
            //   - widening conversions, e.g. char to int
            //   - conversions to a base type or an interface type known to be implemented
            // - New can be pure for T? if the value is pure
            //

            switch (expression.NodeType)
            {
                case ExpressionType.Default:

                //
                // PERF: There's a caveat on allowing a ConstantExpression to be duplicated if it's a non-primitive
                //       value type, because the generated code will repeatedly unbox it from an `object` storage
                //       slot in the compiled lambda's environment. We should likely have more options on IsPure to
                //       indicate the usage intent if the expression is pure, e.g. `UseManyTimes`.
                //
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

            if (expression is ReadOnlyTemporaryVariableExpression)
            {
                return true;
            }

            return false;
        }

        public static void RewriteByRefArgument(ParameterInfo parameter, ref Expression expression, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression> writebackList)
        {
            // NB: This deals with ArrayIndex and MemberAccess nodes passed by ref, which are classified as
            //     variables according to the C# language specification, e.g.
            //
            //       Interlocked.Exchange(value: A(), location1: B()[C()])
            //
            //     In this case, we don't want a write-back; we want to pass the ArrayIndex in the parameter
            //     position:
            //
            //       t1 = A(); t2 = B(); t3 = C();
            //       Interlocked.Exchange(ref t2[t3], t1)
            //
            //     Note we'd be better off if we had the ability to store ByRef variables in the Block scope,
            //     but the LINQ APIs don't support that right now.

            expression = RewriteByRefArgument(parameter, RewriteArrayIndexes(expression), makeVariable, statements, ref writebackList);
        }

        private static Expression RewriteByRefArgument(ParameterInfo parameter, Expression expression, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression> writebackList)
        {
            // REVIEW: The semantics of writebacks here are very questionable. C# doesn't need them, but the DLR has them
            //         for by-ref property member accesses and index accesses. Retaining some for of writeback support will
            //         depend on whether we push this up to a common library so VB can use it as well.
            //
            //         Also note that the logic below doesn't support the C# indexer node which supports named and optional
            //         parameters. Given that C# doesn't support passing indexers by ref, this should be fine.

            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    // NB: Don't use a write-back here; LambdaCompiler can emit a reference for the argument.
                    break;
                case ExpressionType.Index:
                    {
                        var index = (IndexExpression)expression;

                        var isArray = index.Indexer == null; // NB: no indexer means array access, hence writeable

                        if (isArray || index.Indexer.CanWrite)
                        {
                            var obj = index.Object;
                            var args = index.Arguments;

                            var newObj = default(Expression);
                            if (obj != null)
                            {
                                newObj = RewriteByRefArgument(parameter, obj, makeVariable, statements, ref writebackList);
                            }

                            var n = args.Count;
                            var newArgs = new Expression[n];

                            for (var i = 0; i < n; i++)
                            {
                                newArgs[i] = StoreIfNeeded(args[i], "__arg" + i, makeVariable, statements);
                            }

                            var newIndex = index.Update(newObj, newArgs);
                            expression = newIndex;

                            if (!isArray)
                            {
                                EnsureWriteback(parameter, ref expression, makeVariable, statements, ref writebackList);
                            }
                            else
                            {
                                // NB: This ensures correct timing of an out-of-range exception relative to other arguments.
                                statements.Add(expression);
                            }
                        }
                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        var member = (MemberExpression)expression;

                        var writeable = false;
                        var useWriteback = false;

                        if (member.Member is PropertyInfo prop)
                        {
                            if (prop.CanWrite)
                            {
                                writeable = true;
                                useWriteback = true;
                            }
                        }
                        else
                        {
                            if (member.Member is FieldInfo field && !(field.IsInitOnly || field.IsLiteral))
                            {
                                writeable = true;

                                // NB: Don't use a write-back here; LambdaCompiler can emit a reference for the argument.
                            }
                        }

                        if (writeable)
                        {
                            var obj = member.Expression;

                            var newObj = default(Expression);
                            if (obj != null)
                            {
                                newObj = RewriteByRefArgument(parameter, obj, makeVariable, statements, ref writebackList);
                            }

                            var newMember = member.Update(newObj);
                            expression = newMember;

                            // NB: Reading from a static field has no side-effects, so we don't have to emit the expression
                            //     in the statement list like we do for array indexing operations. We emit it for instance
                            //     fields though, so we don't change the timing of NullReferenceExceptions.
                            if (obj != null && member.Member.MemberType == MemberTypes.Field)
                            {
                                statements.Add(expression);
                            }

                            if (useWriteback)
                            {
                                EnsureWriteback(parameter, ref expression, makeVariable, statements, ref writebackList);
                            }
                        }
                    }
                    break;
                default:
                    if (expression is ArrayAccessCSharpExpression arrayAccess)
                    {
                        expression = arrayAccess.Reduce(makeVariable, statements);
                    }
                    else
                    {
                        var variable = CreateTemp(makeVariable, parameter, expression.Type);
                        statements.Add(Expression.Assign(variable, expression));
                        expression = variable;
                    }
                    break;
            }

            return expression;
        }

        private static Expression RewriteArrayIndexes(Expression expression)
        {
            // NB: This simplifies other rewrite steps for by-ref argument passing by rewriting all
            //     variants of array access (ArrayIndex and Call with an array Get method) into
            //     IndexExpression nodes. It only recurses on the left side of the tree to traverse
            //     the objects being accessed in the tree.

            if (expression != null)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.ArrayIndex:
                        {
                            var binary = (BinaryExpression)expression;
                            var array = RewriteArrayIndexes(binary.Left);
                            var index = binary.Right;
                            return Expression.ArrayAccess(array, index);
                        }
                    case ExpressionType.Call:
                        {
                            var call = (MethodCallExpression)expression;
                            if (IsArrayAssignment(call))
                            {
                                var array = RewriteArrayIndexes(call.Object);
                                var indexes = call.Arguments.ToArray();
                                return Expression.ArrayAccess(array, indexes);
                            }
                        }
                        break;
                    case ExpressionType.Index:
                        {
                            var index = (IndexExpression)expression;
                            if (index.Indexer == null)
                            {
                                var array = RewriteArrayIndexes(index.Object);
                                var indexes = index.Arguments;
                                return index.Update(array, indexes);
                            }
                        }
                        break;
                    case ExpressionType.MemberAccess:
                        {
                            var member = (MemberExpression)expression;
                            var obj = RewriteArrayIndexes(member.Expression);
                            return member.Update(obj);
                        }
                }
            }

            return expression;
        }

        private static void EnsureWriteback(ParameterInfo parameter, ref Expression expression, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression> writebackList)
        {
            var variable = CreateTemp(makeVariable, parameter, expression.Type);
            statements.Add(Expression.Assign(variable, expression));

            var writeback = Expression.Assign(expression, variable);
            AddWriteback(ref writebackList, writeback);

            expression = variable;
        }

        private static ParameterExpression CreateTemp(Func<Type, string, ParameterExpression> makeVariable, ParameterInfo parameter, Type type)
        {
            var var = makeVariable(type, parameter?.Name);
            return var;
        }

        private static void AddWriteback(ref List<Expression> writebackList, Expression writeback)
        {
            if (writebackList == null)
            {
                writebackList = new List<Expression>();
            }

            writebackList.Add(writeback);
        }

        private static Expression StoreIfNeeded(Expression expression, string name, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            // TODO: Can avoid introducing a local in some cases.

            var var = makeVariable(expression.Type, name);
            statements.Add(Expression.Assign(var, expression));
            return var;
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

        private static readonly object s_null = new object();

        public static object OrNullSentinel(this object o)
        {
            return o ?? s_null;
        }

        public static void RequiresCanWrite(Expression expression, string paramName)
        {
            // NB: This does not account for dynamic member and index nodes; to make dynamically bound assignments,
            //     one should use the appropriate methods on DynamicCSharpExpression, which require the use of
            //     dynamic arguments and also allow to separate the dynamic API from the rest of the nodes without
            //     having a strange circular dependency.

            switch (expression)
            {
                case DiscardCSharpExpression _:
                    return;
                case IndexCSharpExpression index:
                    EnsureCanWrite(index, paramName);
                    break;
                case ArrayAccessCSharpExpression arrayAccess:
                    EnsureCanWrite(arrayAccess, paramName);
                    break;
                case IndexerAccessCSharpExpression indexerAccess:
                    EnsureCanWrite(indexerAccess, paramName);
                    break;
                default:
                    {
                        // NB: Our current modification of the Roslyn compiler can emit these nodes as the LHS of an
                        //     assignment. We can deal with this in reduction steps by rewriting it to ArrayAccess
                        //     using MakeWriteable below.

                        if (expression.NodeType == ExpressionType.ArrayIndex)
                        {
                            return;
                        }

                        if (expression.NodeType == ExpressionType.Call)
                        {
                            var call = (MethodCallExpression)expression;
                            if (IsArrayAssignment(call))
                            {
                                return;
                            }
                        }

                        ExpressionStubs.RequiresCanWrite(expression, paramName);
                        break;
                    }
            }
        }

        public static Expression MakeWriteable(Expression lhs)
        {
            if (lhs is DiscardCSharpExpression)
            {
                return lhs.Reduce();
            }

            if (lhs.NodeType == ExpressionType.ArrayIndex)
            {
                var arrayIndex = (BinaryExpression)lhs;
                return Expression.ArrayAccess(arrayIndex.Left, arrayIndex.Right);
            }

            if (lhs.NodeType == ExpressionType.Call)
            {
                var arrayIndex = (MethodCallExpression)lhs;
                if (IsArrayAssignment(arrayIndex))
                {
                    return Expression.ArrayAccess(arrayIndex.Object, arrayIndex.Arguments);
                }
            }

            return lhs;
        }

        private static void EnsureCanWrite(IndexCSharpExpression index, string paramName)
        {
            if (!index.Indexer.CanWrite)
            {
                throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
            }
        }

        private static void EnsureCanWrite(ArrayAccessCSharpExpression arrayAccess, string paramName)
        {
            if (arrayAccess.Indexes[0].Type == typeof(Range))
            {
                throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
            }
        }

        private static void EnsureCanWrite(IndexerAccessCSharpExpression indexerAccess, string paramName)
        {
            if (indexerAccess.Type == typeof(Range))
            {
                throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
            }
            else
            {
                var indexer = (PropertyInfo)indexerAccess.IndexOrSlice;

                if (!indexer.CanWrite)
                {
                    throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
                }
            }
        }

        private static bool IsArrayAssignment(MethodCallExpression call)
        {
            var method = call.Method;
            var obj = call.Object;
            return !method.IsStatic && obj.Type.IsArray && method == obj.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance);
        }

        public static Expression ReduceAssign(Expression lhs, Func<Expression, Expression> assign)
        {
            // NB: This is a simplified form of ReduceAssignment without support for conversions or a differentation of prefix/postfix assignments.
            //     It also assumes the lhs is not a mutable value type that needs by ref access treatment.

            var temps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var expr = ReduceAssign(lhs, temps, stmts);

            stmts.Add(assign(expr));

            return Comma(temps, stmts);
        }

        public static Expression ReduceAssign(Expression lhs, List<ParameterExpression> temps, List<Expression> stmts)
        {
            return lhs.NodeType switch
            {
                ExpressionType.MemberAccess => ReduceAssignMember(),
                ExpressionType.Index => ReduceAssignIndex(),
                ExpressionType.Parameter => lhs,
                _ => lhs switch
                {
                    IndexCSharpExpression index => index.ReduceAssign(temps, stmts),
                    ArrayAccessCSharpExpression arrayAccess => arrayAccess.ReduceAssign(temps, stmts),
                    IndexerAccessCSharpExpression indexerAccess => indexerAccess.ReduceAssign(temps, stmts),
                    _ => throw ContractUtils.Unreachable,
                },
            };

            Expression ReduceAssignMember()
            {
                var member = (MemberExpression)lhs;

                if (member.Expression == null)
                {
                    return member;
                }
                else
                {
                    if (NeedByRefAssign(member.Expression))
                    {
                        throw ContractUtils.Unreachable;
                    }

                    var lhsTemp = Expression.Parameter(member.Expression.Type, "__lhs");
                    var lhsAssign = Expression.Assign(lhsTemp, member.Expression);

                    temps.Add(lhsTemp);
                    stmts.Add(lhsAssign);

                    return member.Update(lhsTemp);
                }
            }

            Expression ReduceAssignIndex()
            {
                var index = (IndexExpression)lhs;

                if (NeedByRefAssign(index.Object))
                {
                    throw ContractUtils.Unreachable;
                }

                var n = index.Arguments.Count;
                var args = new Expression[n];

                var obj = Expression.Parameter(index.Object.Type, "__object");
                temps.Add(obj);
                stmts.Add(Expression.Assign(obj, index.Object));

                for (var j = 0; j < n; j++)
                {
                    var arg = index.Arguments[j];

                    if (IsPure(arg))
                    {
                        args[j] = arg;
                    }
                    else
                    {
                        var temp = Expression.Parameter(arg.Type, "__arg" + j);
                        temps.Add(temp);
                        stmts.Add(Expression.Assign(temp, arg));

                        args[j] = temp;
                    }
                }

                return index.Update(obj, new TrueReadOnlyCollection<Expression>(args));
            }
        }

        public static Expression ReduceAssignment(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix = true, LambdaExpression leftConversion = null)
        {
            return lhs.NodeType switch
            {
                ExpressionType.MemberAccess => ReduceMember(lhs, functionalOp, prefix, leftConversion),
                ExpressionType.Index => ReduceIndex(lhs, functionalOp, prefix, leftConversion),
                ExpressionType.Parameter => ReduceVariable(lhs, functionalOp, prefix, leftConversion),
                _ => lhs switch
                {
                    IndexCSharpExpression index => ReduceIndexCSharp(index, functionalOp, prefix, leftConversion),
                    ArrayAccessCSharpExpression arrayAccess => ReduceArrayAccessCSharp(arrayAccess, functionalOp, prefix, leftConversion),
                    IndexerAccessCSharpExpression indexerAccess => ReduceIndexerAccessCSharp(indexerAccess, functionalOp, prefix, leftConversion),
                    _ => throw ContractUtils.Unreachable,
                },
            };
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
                if (NeedByRefAssign(member.Expression))
                {
                    // NB: See https://github.com/dotnet/corefx/issues/4984 for a discussion on the need to deal
                    //     with by-ref assignment LHS expressions like this.

                    var lhsTemp = Expression.Parameter(member.Type, "__lhs");
                    var op = functionalOp(WithLeftConversion(lhsTemp, leftConversion));
                    var method = typeof(RuntimeOpsEx).GetMethod(prefix ? nameof(RuntimeOpsEx.PreAssignByRef) : nameof(RuntimeOpsEx.PostAssignByRef));
                    method = method.MakeGenericMethod(member.Type);
                    res = Expression.Call(method, member, Expression.Lambda(op, lhsTemp));
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
            }

            return res;
        }

        private static Expression ReduceIndex(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            var index = (IndexExpression)lhs;

            var isByRef = false;

            if (NeedByRefAssign(index.Object))
            {
                // NB: See https://github.com/dotnet/corefx/issues/4984 for a discussion on the need to deal
                //     with by-ref assignment LHS expressions like this.

                // NB: For arrays, the check won't pass. For value types, this means there's an indexer and
                //     we still have to perform a get and a set (i.e. we can't get a ref to the location that's
                //     being indexed into). However, we don't want to operate on a copy of the struct. It now
                //     depends on the nature of `index.Object` what we can do. If it's a variable or a field,
                //     we can get the location as a reference. We don't have to check for this though because
                //     the LINQ expression APIs allow passing anything by reference.

                isByRef = true;
            }

            var n = index.Arguments.Count;
            var args = new Expression[n];
            var block = new List<Expression>();
            var temps = new List<ParameterExpression>();

            var receiver = index.Object;

            ParameterExpression obj;

            if (!isByRef)
            {
                obj = Expression.Parameter(index.Object.Type, "__object");
                temps.Add(obj);

                block.Add(Expression.Assign(obj, index.Object));
            }
            else
            {
                obj = Expression.Parameter(index.Object.Type.MakeByRefType(), "__object");
            }

            for (var j = 0; j < n; j++)
            {
                var arg = index.Arguments[j];

                if (IsPure(arg))
                {
                    args[j] = arg;
                }
                else
                {
                    var temp = Expression.Parameter(arg.Type, "__arg" + j);
                    temps.Add(temp);

                    block.Add(Expression.Assign(temp, arg));
                    args[j] = temp;
                }
            }

            index = index.Update(obj, new TrueReadOnlyCollection<Expression>(args));

            if (prefix)
            {
                block.Add(Expression.Assign(index, functionalOp(WithLeftConversion(index, leftConversion))));
            }
            else
            {
                var lastTemp = Expression.Parameter(index.Type, "__index");
                temps.Add(lastTemp);

                block.Add(Expression.Assign(lastTemp, index));
                block.Add(Expression.Assign(index, functionalOp(WithLeftConversion(lastTemp, leftConversion))));
                block.Add(lastTemp);
            }

            var res = (Expression)Expression.Block(temps, block);

            if (isByRef)
            {
                var method = typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.WithByRef));
                method = method.MakeGenericMethod(obj.Type, res.Type);
                var delegateType = typeof(FuncByRef<,>).MakeGenericType(obj.Type, res.Type);

                // NB: The introduction of a lambda to lift the computation to the WithByRef helper method can be
                //     expensive because of closure creation. This scenario with mutable structs and indexers should
                //     be quite rare though.

                res = Expression.Call(method, receiver, Expression.Lambda(delegateType, res, obj));
            }

            return res;
        }

        private static bool NeedByRefAssign(Expression lhs)
        {
            // NB: Block doesn't support by-ref locals, so we have to use a helper method to perform the assignment
            //     to the target without causing repeated re-evaluation of the LHS.

            if (lhs.Type.IsValueType && !lhs.Type.IsNullableType())
            {
                return true;
            }

            return false;
        }

        private static Expression ReduceVariable(Expression lhs, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            Expression res;

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

        private static Expression ReduceArrayAccessCSharp(ArrayAccessCSharpExpression arrayAccess, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            return arrayAccess.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
        }

        private static Expression ReduceIndexerAccessCSharp(IndexerAccessCSharpExpression indexerAccess, Func<Expression, Expression> functionalOp, bool prefix, LambdaExpression leftConversion)
        {
            return indexerAccess.ReduceAssign(x => ReduceVariable(x, functionalOp, prefix, leftConversion));
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

                if (expression is BlockExpression block)
                {
                    return Expression.Block(typeof(void), block.Variables, block.Expressions);
                }
            }

            return Expression.Block(typeof(void), expressions);
        }

        public static Expression Comma(List<ParameterExpression> variables, List<Expression> statements)
        {
            if (variables.Count == 0 && statements.Count == 1)
            {
                return statements[0];
            }

            return Expression.Block(variables, statements);
        }

        public static bool IsTupleType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return false;
            }

            var def = type.GetGenericTypeDefinition();

            if (!TupleTypes.Contains(def))
            {
                return false;
            }

            if (def == MaxTupleType)
            {
                return IsTupleType(type.GetGenericArguments()[^1]);
            }

            return true;
        }

        public static int GetTupleArity(Type type)
        {
            Debug.Assert(IsTupleType(type));

            var def = type.GetGenericTypeDefinition();

            if (def == MaxTupleType)
            {
                return TupleArities[def] + GetTupleArity(type.GetGenericArguments()[^1]);
            }

            return TupleArities[def];
        }

        public static IEnumerable<Type> GetTupleComponentTypes(Type type)
        {
            Debug.Assert(IsTupleType(type));

            while (type != null)
            {
                var def = type.GetGenericTypeDefinition();
                var args = type.GetGenericArguments();

                if (def == MaxTupleType)
                {
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        yield return args[i];
                    }

                    type = args[^1];
                }
                else
                {
                    foreach (var arg in args)
                    {
                        yield return arg;
                    }

                    yield break;
                }
            }
        }

        public static Expression GetTupleItemAccess(Expression tuple, int item)
        {
            while (true)
            {
                if (item < TupleItemNames.Length)
                {
                    return Expression.Field(tuple, TupleItemNames[item]);
                }
                else
                {
                    tuple = Expression.Field(tuple, "Rest");
                    item -= TupleItemNames.Length;
                }
            }
        }

        private static int NumberOfValueTuples(int numElements, out int remainder)
        {
            remainder = (numElements - 1) % (ValueTupleRestPosition - 1) + 1;

            return (numElements - 1) / (ValueTupleRestPosition - 1) + 1;
        }

        public static Type MakeTupleType(Type[] elementTypes)
        {
            int chainLength = NumberOfValueTuples(elementTypes.Length, out int remainder);

            var firstTupleType = TupleTypes[remainder - 1];

            var args = new Type[remainder];
            CopyElementTypes(args, (chainLength - 1) * (ValueTupleRestPosition - 1), remainder);

            var tupleType = firstTupleType.MakeGenericType(args);

            int loop = chainLength - 1;
            
            while (loop > 0)
            {
                args = new Type[ValueTupleRestPosition];
                CopyElementTypes(args, (loop - 1) * (ValueTupleRestPosition - 1), ValueTupleRestPosition - 1);
                args[ValueTupleRestPosition - 1] = tupleType;

                tupleType = MaxTupleType.MakeGenericType(args);
            
                loop--;
            }

            return tupleType;

            void CopyElementTypes(Type[] target, int offset, int count)
            {
                for (var i = 0; i < count; i++)
                {
                    target[i] = elementTypes[offset + i];
                }
            }
        }

        public static bool IsTaskLikeType(Type type, out Type resultType)
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

        private static bool TryGetBuilderType(Type taskType, out Type builderType)
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

        private static bool TryGetBuilderTypeFromAttribute(Type type, out Type builderType)
        {
            if (type.IsDefined(typeof(AsyncMethodBuilderAttribute)))
            {
                var attr = type.GetCustomAttribute<AsyncMethodBuilderAttribute>();

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

        private static List<Type> s_tupleTypes;

        private static List<Type> TupleTypes => s_tupleTypes ??= new List<Type> {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
        };

        private static Dictionary<Type, int> s_tupleArities;

        private static Dictionary<Type, int> TupleArities => s_tupleArities ??= new Dictionary<Type, int> {
            { typeof(ValueTuple<>),        1 },
            { typeof(ValueTuple<,>),       2 },
            { typeof(ValueTuple<,,>),      3 },
            { typeof(ValueTuple<,,,>),     4 },
            { typeof(ValueTuple<,,,,>),    5 },
            { typeof(ValueTuple<,,,,,>),   6 },
            { typeof(ValueTuple<,,,,,,>),  7 },
            { typeof(ValueTuple<,,,,,,,>), 7 }, // NB: Not a mistake, there are only 7 values, and one rest slot.
        };

        private static string[] s_tupleItemNames;

        private static string[] TupleItemNames => s_tupleItemNames ??= new[] {
            "Item1",
            "Item2",
            "Item3",
            "Item4",
            "Item5",
            "Item6",
            "Item7",
        };

        public static readonly Type MaxTupleType = typeof(ValueTuple<,,,,,,,>);

        public const int ValueTupleRestPosition = 8;
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

    internal sealed class ReadOnlyTemporaryVariableExpression : Expression
    {
        private readonly ParameterExpression _variable;

        public ReadOnlyTemporaryVariableExpression(ParameterExpression variable) => _variable = variable;

        public override Type Type => _variable.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override bool CanReduce => true;

        public override Expression Reduce() => _variable;
    }
}
