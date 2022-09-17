// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static ReadOnlyCollection<ParameterAssignment> GetParameterBindings(MethodBase method, IEnumerable<Expression>? expressions)
        {
            return GetParameterBindings(method.GetParametersCached(), expressions);
        }

        public static ReadOnlyCollection<ParameterAssignment> GetParameterBindings(ParameterInfo[] parameters, IEnumerable<Expression>? expressions)
        {
            var arguments = expressions.ToReadOnly();

            var n = arguments.Count;

            if (n > parameters.Length)
            {
                throw Error.TooManyArguments(nameof(expressions));
            }

            var bindings = new ParameterAssignment[n];

            for (var i = 0; i < n; i++)
            {
                bindings[i] = CSharpExpression.Bind(parameters[i], arguments[i]);
            }

            return bindings.ToReadOnlyUnsafe();
        }

        public static void ValidateParameterBindings(MethodBase method, ReadOnlyCollection<ParameterAssignment> argList, bool extensionMethod = false)
        {
            ValidateParameterBindings(method, method.GetParametersCached(), argList, extensionMethod);
        }

        public static void ValidateParameterBindings(MethodBase method, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> arguments, bool extensionMethod = false)
        {
            var boundParameters = new HashSet<ParameterInfo>();

            if (extensionMethod)
            {
                boundParameters.Add(parameters[0]);
            }

            for (int i = 0, n = arguments.Count; i < n; i++)
            {
                var arg = arguments[i];

                var parameter = arg.Parameter;

                var member = parameter.Member;

                var property = member as PropertyInfo;
                if (property != null)
                {
                    // NB: This supports get access via indexers.
                    member = property.GetGetMethod(nonPublic: true);
                }

                if (member != method)
                {
                    throw Error.ParameterNotDefinedForMethod(parameter.Name, method.Name, nameof(arguments), i);
                }

                if (!boundParameters.Add(parameter))
                {
                    throw Error.DuplicateParameterBinding(parameter.Name, nameof(arguments), i);
                }
            }

            for (int i = 0, n = parameters.Length; i < n; i++)
            {
                var parameter = parameters[i];

                if (!boundParameters.Contains(parameter) && (!parameter.IsOptional || !parameter.HasDefaultValue))
                {
                    throw Error.UnboundParameter(parameter.Name, nameof(parameters), i);
                }
            }
        }

        public static Expression BindArguments(Func<Expression?, Expression[], Expression> create, Expression? instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings, bool needTemps = false)
        {
            var res = default(Expression);

            if (!needTemps && CheckArgumentsInOrder(bindings) && !CheckHasSpecialNodesPassedByRefOrInterpolatedStringHandlerConversion(bindings))
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

        public static Expression BindArguments(Func<Expression?, Expression[], Expression> create, Expression? instance, ParameterInfo[] parameters, ReadOnlyCollection<ParameterAssignment> bindings, List<ParameterExpression> variables, List<Expression> statements)
        {
            var arguments = new Expression[parameters.Length];

            var makeVariable = new Func<Type, string?, ParameterExpression>((type, name) =>
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

        private static bool CheckArgumentsInOrder(ReadOnlyCollection<ParameterAssignment> arguments)
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

        private static bool CheckHasSpecialNodesPassedByRefOrInterpolatedStringHandlerConversion(ReadOnlyCollection<ParameterAssignment> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Parameter.ParameterType.IsByRef && argument.Expression is ArrayAccessCSharpExpression)
                {
                    return true;
                }

                if (argument.Expression is InterpolatedStringHandlerConversionCSharpExpression convert && convert.Info.ArgumentIndices.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static void RewriteArguments(Expression? instance, ReadOnlyCollection<ParameterAssignment> bindings, Func<Type, string?, ParameterExpression> makeVariable, List<Expression> statements, ref Expression? obj, Expression[] arguments, out Expression[] writebacks)
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

            var interpolatedStringConversions = new List<ParameterAssignment>();

            foreach (var argument in bindings)
            {
                var parameter = argument.Parameter;
                var expression = argument.Expression;

                if (expression.IsPure()) // NB: no side-effect to read or write
                {
                    arguments[parameter.Position] = expression;
                }
                else if (expression is InterpolatedStringHandlerConversionCSharpExpression convert && convert.Info.ArgumentIndices.Count > 0)
                {
                    interpolatedStringConversions.Add(argument);
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

            // REVIEW: By-ref treatment of various parameters when passed to interpolated string handlers.

            foreach (var argument in interpolatedStringConversions)
            {
                var parameter = argument.Parameter;
                var convert = (InterpolatedStringHandlerConversionCSharpExpression)argument.Expression;
                var indices = convert.Info.ArgumentIndices;

                var args = new List<Expression>(indices.Count);

                foreach (var index in indices)
                {
                    if (index == -1)
                    {
                        args.Add(obj!);
                    }
                    else
                    {
                        var arg = arguments[index];

                        if (arg == null)
                            throw ContractUtils.Unreachable; // NB: Can happen when having self-referential handlers (or cross-refs across multiple).

                        args.Add(arg);
                    }
                }

                arguments[parameter.Position] = convert.Reduce(args);
            }

            writebacks = writebackList?.ToArray() ?? Array.Empty<Expression>();
        }

        public static void RewriteByRefArgument(ParameterInfo? parameter, ref Expression expression, Func<Type, string?, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression>? writebackList)
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

        private static Expression RewriteByRefArgument(ParameterInfo? parameter, Expression expression, Func<Type, string?, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression>? writebackList)
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

                        // NB: no indexer means array access, hence writeable

                        if (index.Indexer == null || index.Indexer.CanWrite)
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

                            // NB: LINQ has some inconsistencies with null support for object on a static indexer. The factory
                            //     for Property supports null, but Update doesn't (though passing null would work).

                            var newIndex = index.Update(newObj!, newArgs);
                            expression = newIndex;

                            if (index.Indexer != null)
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

        [return: NotNullIfNotNull("expression")] // TODO: C# 11.0 nameof
        private static Expression? RewriteArrayIndexes(Expression? expression)
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
                            if (IsArrayAssignment(call, out var obj))
                            {
                                var array = RewriteArrayIndexes(obj);
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
                                var array = RewriteArrayIndexes(index.Object!); // NB: Never null for array.
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

        private static void FillOptionalParameters(ParameterInfo[] parameters, Expression?[] args)
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

        private static void EnsureWriteback(ParameterInfo? parameter, ref Expression expression, Func<Type, string?, ParameterExpression> makeVariable, List<Expression> statements, ref List<Expression>? writebackList)
        {
            var variable = CreateTemp(makeVariable, parameter, expression.Type);
            statements.Add(Expression.Assign(variable, expression));

            var writeback = Expression.Assign(expression, variable);
            AddWriteback(ref writebackList, writeback);

            expression = variable;
        }

        private static ParameterExpression CreateTemp(Func<Type, string?, ParameterExpression> makeVariable, ParameterInfo? parameter, Type type)
        {
            var var = makeVariable(type, parameter?.Name);
            return var;
        }

        private static void AddWriteback(ref List<Expression>? writebackList, Expression writeback)
        {
            writebackList ??= new List<Expression>();

            writebackList.Add(writeback);
        }

        private static Expression StoreIfNeeded(Expression expression, string name, Func<Type, string, ParameterExpression> makeVariable, List<Expression> statements)
        {
            // TODO: Can avoid introducing a local in some cases.

            var var = makeVariable(expression.Type, name);
            statements.Add(Expression.Assign(var, expression));
            return var;
        }
    }
}
