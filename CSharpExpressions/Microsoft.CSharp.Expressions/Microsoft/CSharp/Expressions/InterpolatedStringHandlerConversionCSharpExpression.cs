// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents an interpolated string handler conversion expression.
    /// </summary>
    public partial class InterpolatedStringHandlerConversionCSharpExpression : CSharpExpression
    {
        internal InterpolatedStringHandlerConversionCSharpExpression(InterpolatedStringHandlerInfo info, Expression operand)
        {
            Info = info;
            Operand = operand;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => Info.Type;

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.InterpolatedStringHandlerConversion;

        /// <summary>
        /// Gets the information describing the interpolated string handler conversion.
        /// </summary>
        public InterpolatedStringHandlerInfo Info { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> representing the interpolated string to convert.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitInterpolatedStringHandlerConversion(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="InterpolatedStringHandlerConversionCSharpExpression.Operand" /> property of the result.</param>
        /// <param name="info">The <see cref="InterpolatedStringHandlerConversionCSharpExpression.Info"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InterpolatedStringHandlerConversionCSharpExpression Update(Expression operand, InterpolatedStringHandlerInfo info)
        {
            if (operand == Operand && info == Info)
            {
                return this;
            }

            return InterpolatedStringHandlerConvert(info, operand);
        }

        /// <summary>
        /// Gets a value indicating whether the node can be reduced.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if the node can only be reduced in the context of a call because the handler captures
        /// arguments for its construction. See <see cref="InterpolatedStringHandlerInfo.ArgumentIndices"/>.
        /// </remarks>
        public override bool CanReduce => Info.ArgumentIndices.Count == 0;

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            if (Info.ArgumentIndices.Count > 0)
                throw Unreachable;

            return Reduce(Array.Empty<Expression>());
        }

        internal Expression Reduce(IList<Expression> arguments)
        {
            var vars = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var builder = Expression.Variable(Info.Type, "builder");
            vars.Add(builder);

            stmts.Add(null); // NB: Placeholder for the construction and assignment.

            var usesTrailingShouldAppend = ParameterListHasTrailingShouldAppend(Info.Construction.Parameters);
            var usesBoolAppend = GetAppendType(Info.Append[0]) == typeof(bool); // NB: We ensure at least one and uniform return types.

            ParameterExpression shouldAppend = null;
            Expression appendCalls;
            List<Expression> appendStmts;

            if (usesTrailingShouldAppend)
            {
                shouldAppend = Expression.Variable(typeof(bool), "shouldAppend");
                vars.Add(shouldAppend);
                arguments = arguments.AddLast(shouldAppend);

                if (usesBoolAppend)
                {
                    appendCalls = shouldAppend;
                    appendStmts = null;
                }
                else
                {
                    appendCalls = null;
                    appendStmts = new List<Expression>(Info.Append.Count);
                }
            }
            else
            {
                if (usesBoolAppend)
                {
                    appendCalls = null; // NB: Will check for null to start the && folding with the first append call.
                    appendStmts = null;
                }
                else
                {
                    appendCalls = null;
                    appendStmts = stmts;
                }
            }

            var literalLength = 0;
            var formattedCount = 0;
            var i = 0;

            var interpolatedStrings = GetInterpolatedStrings(Operand);

            foreach (var interpolatedString in interpolatedStrings)
            {
                foreach (var interpolation in interpolatedString.Interpolations)
                {
                    var appendArgs = new List<Expression>
                    {
                        builder
                    };

                    if (interpolation is InterpolationStringLiteral { Value: var literal })
                    {
                        literalLength += literal.Length;

                        appendArgs.Add(Expression.Constant(literal, typeof(string)));
                    }
                    else
                    {
                        var insert = (InterpolationStringInsert)interpolation;

                        formattedCount++;

                        appendArgs.Add(insert.Value);

                        if (insert.Alignment is int alignment)
                        {
                            appendArgs.Add(CreateConstantInt32(alignment));
                        }

                        if (insert.Format is string format)
                        {
                            appendArgs.Add(Expression.Constant(format, typeof(string)));
                        }
                    }

                    var append = Info.Append[i++];
                    var appendCall = ApplyAppend(append, appendArgs);

                    if (usesBoolAppend)
                    {
                        appendCalls = appendCalls == null ? appendCall : Expression.AndAlso(appendCalls, appendCall);
                    }
                    else
                    {
                        appendStmts.Add(appendCall);
                    }
                }
            }

            var construct = ApplyConstruction(Info.Construction, literalLength, formattedCount, arguments);

            stmts[0] = Expression.Assign(builder, construct);

            if (usesBoolAppend)
            {
                stmts.Add(appendCalls); // NB: This is the chain of && checks.
            }
            else if (usesTrailingShouldAppend)
            {
                stmts.Add(
                    Expression.IfThen(
                        shouldAppend,
                        Expression.Block(typeof(void), appendStmts)
                    )
                );
            }

            stmts.Add(builder);

            return Expression.Block(vars, stmts);

            static Expression ApplyAppend(LambdaExpression append, IList<Expression> args)
            {
                if (append.Body is MethodCallExpression call &&
                    append.Parameters.Count == call.Arguments.Count + 1 &&
                    call.Object == append.Parameters[0] &&
                    EqualModuloConvert(call.Arguments, append.Parameters.Skip(1)))
                {
                    return call.Update(args[0], Substitute(call.Arguments, append.Parameters.Skip(1), args.Skip(1)));
                }
                
                if (append.Body is MethodCallCSharpExpression callCSharp &&
                    append.Parameters.Count == callCSharp.Arguments.Count + 1 &&
                    callCSharp.Object == append.Parameters[0] &&
                    EqualModuloConvert(callCSharp.Arguments, append.Parameters.Skip(1)))
                {
                    // NB: This optimization is safe because we know that alignment and format, if specified,
                    //     are constants and can be reordered relative to the value expression.
                    //
                    // REVIEW: This is quite narrow and doesn't account for reordering of named parameters
                    //         for alignment and format. However, given that interpolated string handler types
                    //         are non-trivial to write, let's just assume they follow the standard order in
                    //         most cases. Worst case, we fail to inline and fall back to an Invoke(Lambda).

                    return callCSharp.Update(args[0], Substitute(callCSharp.Arguments, append.Parameters.Skip(1), args.Skip(1)));
                }

                if (IsDynamicInvokeMember(append.Body, out var dynamicCall))
                {
                    return InlineDynamicInvokeMember(dynamicCall);
                }

                if (append.Body is ConvertDynamicCSharpExpression dynamicConvert &&
                    IsDynamicInvokeMember(dynamicConvert.Expression, out var innerDynamicCall))
                {
                    return dynamicConvert.Update(InlineDynamicInvokeMember(innerDynamicCall));
                }

                return Expression.Invoke(append, args);

                bool IsDynamicInvokeMember(Expression expr, out InvokeMemberDynamicCSharpExpression res)
                {
                    if (expr is InvokeMemberDynamicCSharpExpression dc &&
                        append.Parameters.Count == dc.Arguments.Count + 1 &&
                        dc.Object == append.Parameters[0] &&
                        EqualModuloConvert(dc.Arguments, append.Parameters.Skip(1)))
                    {
                        res = dc;
                        return true;
                    }

                    res = null;
                    return false;
                }

                InvokeMemberDynamicCSharpExpression InlineDynamicInvokeMember(InvokeMemberDynamicCSharpExpression expr)
                {
                    return expr.Update(args[0], Substitute(expr.Arguments, append.Parameters.Skip(1), args));
                }
            }

            static Expression ApplyConstruction(LambdaExpression construction, int literalLength, int formattedCount, IList<Expression> args)
            {
                var newArgs = new List<Expression>
                {
                    CreateConstantInt32(literalLength),
                    CreateConstantInt32(formattedCount)
                };

                newArgs.AddRange(args); // NB: Contains the `out bool` trailing parameter, if any.

                if (construction.Body is NewExpression newExpr &&
                    construction.Parameters.Count == newExpr.Arguments.Count &&
                    EqualModuloConvert(newExpr.Arguments, construction.Parameters))
                {
                    return newExpr.Update(Substitute(newExpr.Arguments, construction.Parameters, newArgs));
                }

                if (construction.Body is NewCSharpExpression csharpNewExpr &&
                    construction.Parameters.Count == csharpNewExpr.Arguments.Count &&
                    EqualModuloConvert(csharpNewExpr.Arguments, construction.Parameters))
                {
                    return csharpNewExpr.Update(Substitute(csharpNewExpr.Arguments, construction.Parameters, newArgs));
                }

                if (construction.Body is InvokeConstructorDynamicCSharpExpression dynamicNewExpr &&
                    construction.Parameters.Count == dynamicNewExpr.Arguments.Count &&
                    EqualModuloConvert(dynamicNewExpr.Arguments, construction.Parameters))
                {
                    return dynamicNewExpr.Update(Substitute(dynamicNewExpr.Arguments, construction.Parameters, newArgs));
                }

                return Expression.Invoke(construction, newArgs);
            }
        }

        private static bool EqualModuloConvert(IEnumerable<ParameterAssignment> args, IEnumerable<ParameterExpression> parameters)
        {
            return EqualModuloConvert(args.Select(a => a.Expression), parameters);
        }

        private static bool EqualModuloConvert(IEnumerable<DynamicCSharpArgument> args, IEnumerable<ParameterExpression> parameters)
        {
            return EqualModuloConvert(args.Select(a => a.Expression), parameters);
        }

        private static bool EqualModuloConvert(IEnumerable<Expression> args, IEnumerable<ParameterExpression> parameters)
        {
            return args.Zip(parameters, (arg, param) => Core(arg, param)).All(b => b);

            static bool Core(Expression arg, ParameterExpression param)
            {
                if (arg == param)
                {
                    return true;
                }

                if (arg is UnaryExpression u && (u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.ConvertChecked) && u.Operand == param)
                {
                    return true;
                }

                return false;
            }
        }

        private static IEnumerable<ParameterAssignment> Substitute(IEnumerable<ParameterAssignment> args, IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> values)
        {
            return Substitute(args, parameters, values, p => p.Expression, (p, value) => p.Update(value));
        }

        private static IEnumerable<DynamicCSharpArgument> Substitute(IEnumerable<DynamicCSharpArgument> args, IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> values)
        {
            return Substitute(args, parameters, values, p => p.Expression, (p, value) => p.Update(value));
        }

        private static IEnumerable<Expression> Substitute(IEnumerable<Expression> args, IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> values)
        {
            return Substitute(args, parameters, values, expr => expr, (expr, value) => value);
        }

        private static IEnumerable<T> Substitute<T>(IEnumerable<T> args, IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> values, Func<T, Expression> getExpression, Func<T, Expression, T> update)
        {
            return args.Zip(parameters, (arg, param) => (arg, param)).Zip(values, (t, value) =>
            {
                var expr = getExpression(t.arg);

                if (expr == t.param)
                {
                    return update(t.arg, value);
                }
                else if (expr is UnaryExpression u)
                {
                    return update(t.arg, u.Update(value));
                }
                else
                {
                    throw Unreachable;
                }
            });
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="InterpolatedStringHandlerConversionCSharpExpression"/> that represents the conversion of an interpolated string to a handler type.
        /// </summary>
        /// <param name="info">The information describing the interpolated string handler conversion.</param>
        /// <param name="expression">The expression representing the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringHandlerConversionCSharpExpression"/>.</returns>
        public static InterpolatedStringHandlerConversionCSharpExpression InterpolatedStringHandlerConvert(InterpolatedStringHandlerInfo info, Expression expression)
        {
            // NB: The Roslyn compiler binds to this method.

            RequiresNotNull(info, nameof(info));
            RequiresCanRead(expression, nameof(expression));

            var interpolatedStrings = GetInterpolatedStrings(expression, shouldCheck: true);

            var i = 0;
            var appendCount = info.Append.Count;

            foreach (var interpolatedString in interpolatedStrings)
            {
                foreach (var interpolation in interpolatedString.Interpolations)
                {
                    if (i >= appendCount)
                    {
                        i++;
                        continue; // NB: Will report error at the exit of the loop.
                    }

                    var append = info.Append[i];

                    if (interpolation is InterpolationStringLiteral literal)
                    {
                        CheckAppendLiteralLambda(append.Body, append.Parameters);
                    }
                    else
                    {
                        var insert = (InterpolationStringInsert)interpolation;

                        var expectedCount = 2;

                        if (insert.Alignment != null)
                            expectedCount++;

                        if (insert.Format != null)
                            expectedCount++;

                        if (append.Parameters.Count != expectedCount)
                            throw Error.InvalidAppendFormattedParameterCount(append.Parameters.Count, expectedCount);

                        var value = append.Parameters[1];

                        if (!AreReferenceAssignable(value.Type, insert.Value.Type))
                            throw Error.InvalidAppendFormattedValueType(value.Type, insert.Value.Type);

                        var j = 2;

                        if (insert.Alignment != null)
                        {
                            var alignment = append.Parameters[j];

                            if (alignment.Type != typeof(int))
                                throw Error.InvalidAlignmentParameterType(alignment.Type);

                            j++;
                        }

                        if (insert.Format != null)
                        {
                            var format = append.Parameters[j];

                            if (format.Type != typeof(string))
                                throw Error.InvalidFormatParameterType(format.Type);
                        }
                    }

                    i++;
                }
            }

            if (i != appendCount)
                throw Error.IncorrectNumberOfAppendsForInterpolatedString(appendCount, i);

            return new InterpolatedStringHandlerConversionCSharpExpression(info, expression);
        }

        // REVIEW: The useBoolReturn parameter feels out of place but is required to support dynamic bodies where the return type can
        //         be 'object' which could either mean 'void' (when the result is discarded) or be converted to 'bool'. However, we
        //         could also just mandate that the body inserts a Convert(typeof(void)) step to "discard" at the expense of the lambda
        //         gaining more operations compared to what the user wrote (though in this case the user didn't write anything other
        //         than the interpolated string which retains its original shape).

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a literal to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="handler">The parameter representing the handler to append to.</param>
        /// <param name="value">The parameter representing the literal to append.</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a literal to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendLiteralLambda(bool useBoolReturn, Expression body, ParameterExpression handler, ParameterExpression value) =>
            InterpolatedStringHandlerAppendLiteralLambda(useBoolReturn, body, new[] { handler, value });

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a literal to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="parameters">The parameters of the lambda (in the order handler, literal).</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a literal to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendLiteralLambda(bool useBoolReturn, Expression body, ParameterExpression[] parameters)
        {
            // NB: The Roslyn compiler binds to this method.

            RequiresCanRead(body, nameof(body));
            RequiresNotNullItems(parameters, nameof(parameters));

            CheckAppendLiteralLambda(body, parameters);

            var handlerType = parameters[0].Type;

            var delegateTypeDef = useBoolReturn ? typeof(TryAppendLiteral<>) : typeof(AppendLiteral<>);
            var delegateType = delegateTypeDef.MakeGenericType(handlerType);

            return Lambda(delegateType, body, parameters);
        }

        private static void CheckAppendLiteralLambda(Expression body, IList<ParameterExpression> parameters)
        {
            if (parameters.Count != 2)
                throw Error.AppendLiteralLambdaShouldHaveTwoParameters();

            CheckAppendLambdaHandlerParameter(parameters[0]);

            if (parameters[1].Type != typeof(string))
                throw Error.AppendLiteralLambdaShouldTakeStringParameter(parameters[1].Type);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="handler">The parameter representing the handler to append to.</param>
        /// <param name="value">The parameter representing the value to append.</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendFormattedLambda(bool useBoolReturn, Expression body, ParameterExpression handler, ParameterExpression value) =>
            InterpolatedStringHandlerAppendFormattedLambda(useBoolReturn, body, new[] { handler, value });

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="handler">The parameter representing the handler to append to.</param>
        /// <param name="value">The parameter representing the value to append.</param>
        /// <param name="alignmentOrFormat">The parameter representing the alignment or format.</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendFormattedLambda(bool useBoolReturn, Expression body, ParameterExpression handler, ParameterExpression value, ParameterExpression alignmentOrFormat) =>
            InterpolatedStringHandlerAppendFormattedLambda(useBoolReturn, body, new[] { handler, value, alignmentOrFormat });

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="handler">The parameter representing the handler to append to.</param>
        /// <param name="value">The parameter representing the value to append.</param>
        /// <param name="alignment">The parameter representing the alignment.</param>
        /// <param name="format">The parameter representing the format.</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendFormattedLambda(bool useBoolReturn, Expression body, ParameterExpression handler, ParameterExpression value, ParameterExpression alignment, ParameterExpression format) =>
            InterpolatedStringHandlerAppendFormattedLambda(useBoolReturn, body, new[] { handler, value, alignment, format });

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.
        /// </summary>
        /// <param name="useBoolReturn">A Boolean indicating whether the append operation returns <c>bool</c> or <c>void</c>.</param>
        /// <param name="body">The body of the append operation.</param>
        /// <param name="parameters">The parameters of the lambda (in the order handler, value[, alignment][, format]).</param>
        /// <returns>A <see cref="LambdaExpression"/> representing an operation to append a formatted value to an interpolated string handler.</returns>
        public static LambdaExpression InterpolatedStringHandlerAppendFormattedLambda(bool useBoolReturn, Expression body, ParameterExpression[] parameters)
        {
            // NB: The Roslyn compiler binds to this method.

            RequiresCanRead(body, nameof(body));
            RequiresNotNullItems(parameters, nameof(parameters));

            CheckAppendFormattedLambda(body, parameters);

            var returnType = useBoolReturn ? typeof(bool) : typeof(void);
            var handlerType = parameters[0].Type;
            var valueType = parameters[1].Type;

            Type delegateTypeDef = parameters.Length switch
            {
                2 => GenericAppendDelegates[(returnType, null, null)],
                3 => GenericAppendDelegates[(returnType, parameters[2].Type, null)],
                4 => GenericAppendDelegates[(returnType, parameters[2].Type, parameters[3].Type)],
                _ => throw Unreachable,
            };

            var delegateType = delegateTypeDef.MakeGenericType(handlerType, valueType);

            return Lambda(delegateType, body, parameters);
        }

        private static Dictionary<(Type, Type, Type), Type> s_genericAppendDelegates;

        private static Dictionary<(Type returnType, Type firstType, Type secondType), Type> GenericAppendDelegates => s_genericAppendDelegates ??= new Dictionary<(Type, Type, Type), Type>
        {
            { (typeof(void), null, null), typeof(AppendFormatted<,>) },
            { (typeof(bool), null, null), typeof(TryAppendFormatted<,>) },

            { (typeof(void), typeof(int), null), typeof(AppendFormattedAlignment<,>) },
            { (typeof(bool), typeof(int), null), typeof(TryAppendFormattedAlignment<,>) },

            { (typeof(void), typeof(string), null), typeof(AppendFormattedFormat<,>) },
            { (typeof(bool), typeof(string), null), typeof(TryAppendFormattedFormat<,>) },

            { (typeof(void), typeof(int), typeof(string)), typeof(AppendFormattedAlignmentFormat<,>) },
            { (typeof(bool), typeof(int), typeof(string)), typeof(TryAppendFormattedAlignmentFormat<,>) },
        };

        private static void CheckAppendFormattedLambda(Expression body, IList<ParameterExpression> parameters)
        {
            if (parameters.Count < 2 || parameters.Count > 4)
                throw Error.AppendFormattedLambdaInvalidParameterCount();

            CheckAppendLambdaHandlerParameter(parameters[0]);

            if (parameters[1].Type == typeof(void))
                throw Error.AppendFormattedLambdaSecondParameterShouldBeNonVoid();

            if (parameters.Count == 3)
            {
                var type = parameters[2].Type;

                if (type != typeof(int) && type != typeof(string))
                    throw Error.AppendFormattedLambdaThirdParameterShouldBeIntOrString(type);
            }
            else if (parameters.Count == 4)
            {
                if (parameters[2].Type != typeof(int))
                    throw Error.AppendFormattedLambdaThirdParameterShouldBeInt(parameters[2].Type);

                if (parameters[3].Type != typeof(string))
                    throw Error.AppendFormattedLambdaFourthParameterShouldBeString(parameters[3].Type);
            }
        }

        private static void CheckAppendLambdaHandlerParameter(ParameterExpression parameter)
        {
            // REVIEW: For now, we always require the first parameter to be by ref. While not needed for reference types,
            //         it simplifies the logic a bit. We can add more flexibility later.

            if (!parameter.IsByRef)
            {
                throw Error.AppendLambdaShouldHaveFirstByRefParameter(parameter);
            }
        }

        internal static List<InterpolatedStringCSharpExpression> GetInterpolatedStrings(Expression node, bool shouldCheck = false)
        {
            var res = new List<InterpolatedStringCSharpExpression>();

            Visit(node);

            return res;

            void Visit(Expression node)
            {
                if (shouldCheck)
                {
                    CheckIsString(node);
                }

                if (node is InterpolatedStringCSharpExpression s)
                {
                    res.Add(s);
                }
                else if (node is BinaryExpression b && b.NodeType == ExpressionType.Add)
                {
                    Visit(b.Left);
                    Visit(b.Right);
                }
                else
                {
                    throw Error.InvalidStringHandlerConversionOperandNodeType(node.NodeType);
                }

                static void CheckIsString(Expression node)
                {
                    if (node.Type != typeof(string))
                        throw Error.InvalidStringHandlerConversionOperandType(node.Type);
                }
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InterpolatedStringHandlerConversionCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitInterpolatedStringHandlerConversion(InterpolatedStringHandlerConversionCSharpExpression node) =>
            node.Update(
                Visit(node.Operand),
                VisitInterpolatedStringHandlerInfo(node.Info)
            );
    }
}
