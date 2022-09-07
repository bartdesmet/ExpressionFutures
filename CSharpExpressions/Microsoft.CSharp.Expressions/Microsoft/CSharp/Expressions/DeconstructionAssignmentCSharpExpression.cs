// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a deconstruction assignment.
    /// </summary>
    public sealed partial class DeconstructionAssignmentCSharpExpression : CSharpExpression
    {
        internal DeconstructionAssignmentCSharpExpression(Type type, TupleLiteralCSharpExpression left, Expression right, DeconstructionConversion conversion)
        {
            Type = type;
            Left = left;
            Right = right;
            Conversion = conversion;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        /// Gets the left operand of the assignment.
        /// </summary>
        /// <returns>An <see cref="Expression" /> that represents the left operand of the assignment.</returns>
        public TupleLiteralCSharpExpression Left { get; }

        /// <summary>
        /// Gets the right operand of the assignment.
        /// </summary>
        /// <returns>An <see cref="Expression" /> that represents the left operand of the assignment.</returns>
        public Expression Right { get; }

        /// <summary>
        /// Gets the conversion required to deconstruct the object on the right side and assign the components to the left side.
        /// </summary>
        public DeconstructionConversion Conversion { get; }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.DeconstructionAssignment;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitDeconstructionAssignment(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result. </param>
        /// <param name="right">The <see cref="Right" /> property of the result. </param>
        /// <param name="conversion">The <see cref="Conversion" /> property of the result. </param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DeconstructionAssignmentCSharpExpression Update(TupleLiteralCSharpExpression left, Expression right, DeconstructionConversion conversion)
        {
            if (left == Left && right == Right && conversion == Conversion)
            {
                return this;
            }

            return CSharpExpression.DeconstructionAssignment(Type, left, right, Conversion);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            int tempCounter = 0;

            ParameterExpression CreateTemporary(Type type) => Expression.Parameter(type, "__t" + (tempCounter++));

            var lhsTemps = new List<ParameterExpression>();
            var stmts = new List<Expression>();

            var lhsTargets = GetAssignmentTargetsAndSideEffects(Left, lhsTemps, stmts, CreateTemporary);
            
            var result = RewriteDeconstruction(lhsTargets, Conversion, Left.Type, Right, CreateTemporary);

            //
            // CONSIDER: If we have a parent node that's an expression statement discarding the result, we
            //           could omit the generation of a block altogether if there are no side-effects, e.g.
            //           for cases like (_, _) = (1, 2).
            //

            stmts.Add(result);

            return Expression.Block(lhsTemps, stmts);
        }

        private Expression RewriteDeconstruction(List<DeconstructionVariable> lhsTargets, Conversion conversion, Type leftType, Expression right, Func<Type, ParameterExpression> createTemp)
        {
            if (right is ConditionalExpression conditional)
            {
                return
                    conditional.Update(
                        conditional.Test,
                        RewriteDeconstruction(lhsTargets, conversion, leftType, conditional.IfTrue, createTemp),
                        RewriteDeconstruction(lhsTargets, conversion, leftType, conditional.IfFalse, createTemp)
                    );
            }

            var temps = new List<ParameterExpression>();
            var effects = new DeconstructionSideEffects();

            var returnValue = ApplyDeconstructionConversion(lhsTargets, right, conversion, temps, effects, inInit: true, createTemp);

            var stmts = effects.Consolidate();
            stmts.Add(returnValue);

            return Expression.Block(temps, stmts);
        }

        private List<DeconstructionVariable> GetAssignmentTargetsAndSideEffects(TupleLiteralCSharpExpression variables, List<ParameterExpression> temps, List<Expression> effects, Func<Type, ParameterExpression> createTemp)
        {
            var assignmentTargets = new List<DeconstructionVariable>(variables.Arguments.Count);

            foreach (var variable in variables.Arguments)
            {
                switch (variable)
                {
                    case DiscardCSharpExpression _:
                    case ParameterExpression _:
                        assignmentTargets.Add(new DeconstructionVariable(variable));
                        break;

                    case TupleLiteralCSharpExpression tuple: // REVIEW: Handle ConvertedTupleLiteral.
                        assignmentTargets.Add(new DeconstructionVariable(GetAssignmentTargetsAndSideEffects(tuple, temps, effects, createTemp)));
                        break;

                    default:
                        // NB: Known limitation on ref locals when needed for e.g. obj.Bar.Foo = x where Bar is a mutable struct.
                        var expr = MakeWriteable(variable);
                        var v = ReduceAssign(expr, temps, effects, supportByRef: true); // CONSIDER: Wire createTemp throughout.
                        assignmentTargets.Add(new DeconstructionVariable(v));
                        break;
                }
            }

            return assignmentTargets;
        }

        private Expression ApplyDeconstructionConversion(List<DeconstructionVariable> leftTargets, Expression right, Conversion conversion, List<ParameterExpression> temps, DeconstructionSideEffects effects, bool inInit, Func<Type, ParameterExpression> createTemp)
        {
            Debug.Assert(conversion is DeconstructionConversion);

            var rightParts = GetRightParts(right, conversion, temps, effects, ref inInit, createTemp);

            var deconstruct = (DeconstructionConversion)conversion;

            var conversions = deconstruct.Conversions;

            Debug.Assert(leftTargets.Count == rightParts.Count && leftTargets.Count == conversions.Count);

            var builder = new List<Expression>(leftTargets.Count);

            for (int i = 0; i < leftTargets.Count; i++)
            {
                Expression resultPart;

                var nestedConversion = conversions[i];

                if (leftTargets[i].NestedVariables is { } nested)
                {
                    resultPart = ApplyDeconstructionConversion(nested, rightParts[i], nestedConversion, temps, effects, inInit, createTemp);
                }
                else
                {
                    var leftTarget = leftTargets[i].Single;

                    Debug.Assert(leftTarget is { });

                    var rightPart = rightParts[i];
                    if (inInit)
                    {
                        rightPart = EvaluateSideEffectingArgumentToTemp(rightPart, effects.init, temps, createTemp);
                    }

                    resultPart = EvaluateConversionToTemp(rightPart, nestedConversion, temps, effects.conversions, createTemp);

                    if (leftTarget is RefLocalAccessExpression refLocal)
                    {
                        effects.assignments.Add(refLocal.Assign(resultPart));
                    }
                    else if (!(leftTarget is DiscardCSharpExpression))
                    {
                        effects.assignments.Add(Expression.Assign(leftTarget, resultPart));
                    }
                }

                builder.Add(resultPart);
            }

            var tupleType = MakeTupleType(builder.Select(e => e.Type).ToArray());

            return new TupleLiteralCSharpExpression(tupleType, builder.ToReadOnly(), argumentNames: null);
        }

        private static IList<Expression> GetRightParts(Expression right, Conversion conversion, List<ParameterExpression> temps, DeconstructionSideEffects effects, ref bool inInit, Func<Type, ParameterExpression> createTemp)
        {
            // Example:
            // var (x, y) = new Point(1, 2);
            if (conversion is DeconstructionConversion deconstruct && deconstruct.Deconstruct is { } deconstruction)
            {
                Debug.Assert(!IsTupleExpression(right));

                var evaluationResult = EvaluateSideEffectingArgumentToTemp(right, inInit ? effects.init : effects.deconstructions, temps, createTemp);

                inInit = false;
                return InvokeDeconstructLambda(deconstruction, evaluationResult, effects.deconstructions, temps, createTemp);
            }

            // Example:
            // var (x, y) = (1, 2);
            if (IsTupleExpression(right))
            {
                var tuple = (TupleLiteralCSharpExpression)right;
                return tuple.Arguments;
            }

            // Example:
            // (byte x, byte y) = (1, 2);
            // (int x, string y) = (1, null);
            if (right is TupleConvertCSharpExpression tupleConversion && IsTupleExpression(tupleConversion.Operand))
            {
                //
                // NB: Roslyn determines implicit conversions here as well. We stick to identity conversions for now because
                //     we don't model conversions as first-class nodes (yet?).
                //

                if (tupleConversion.ElementConversions.All(c => c.Body == c.Parameters[0]))
                {
                    var tuple = (TupleLiteralCSharpExpression)tupleConversion.Operand;
                    return tuple.Arguments;
                }
            }

            // Example:
            // var (x, y) = GetTuple();
            // var (x, y) = ((byte, byte)) (1, 2);
            // var (a, _) = ((short, short))((int, int))(1L, 2L);
            if (IsTupleType(right.Type))
            {
                inInit = false;
                return AccessTupleFields(right, temps, effects.deconstructions, createTemp);
            }

            throw ContractUtils.Unreachable;
        }

        private static bool IsTupleExpression(Expression e) => e is TupleLiteralCSharpExpression; // REVIEW: ConvertedTupleLiteral

        private static List<Expression> AccessTupleFields(Expression expression, List<ParameterExpression> temps, List<Expression> effects, Func<Type, ParameterExpression> createTemp)
        {
            Debug.Assert(IsTupleType(expression.Type));

            var tupleType = expression.Type;
            var tupleElementTypes = GetTupleComponentTypes(tupleType).ToArray();

            var numElements = tupleElementTypes.Length;

            var tuple = createTemp(expression.Type);
            var assignmentToTemp = Expression.Assign(tuple, expression);
            effects.Add(assignmentToTemp);
            temps.Add(tuple);

            var builder = new List<Expression>(numElements);

            for (int i = 0; i < numElements; i++)
            {
                var fieldAccess = GetTupleItemAccess(tuple, i);
                builder.Add(fieldAccess);
            }

            return builder;
        }

        private static Expression EvaluateConversionToTemp(Expression expression, Conversion conversion, List<ParameterExpression> temps, List<Expression> effects, Func<Type, ParameterExpression> createTemp)
        {
            if (conversion is SimpleConversion simple)
            {
                Expression conversionExpr;

                if (simple.Conversion.Body == simple.Conversion.Parameters[0])
                {
                    return expression;
                }
                else if (simple.Conversion.Body is UnaryExpression u
                    && (u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.ConvertChecked)
                    && u.Operand == simple.Conversion.Parameters[0])
                {
                    conversionExpr = u.Update(expression);
                }
                else
                {
                    conversionExpr = Expression.Invoke(simple.Conversion, expression);
                }

                return EvaluateSideEffectingArgumentToTemp(conversionExpr, effects, temps, createTemp);
            }

            throw ContractUtils.Unreachable;
        }

        private static List<Expression> InvokeDeconstructLambda(LambdaExpression deconstruction, Expression target, List<Expression> effects, List<ParameterExpression> temps, Func<Type, ParameterExpression> createTemp)
        {
            var locals = new List<Expression>();

            foreach (var parameter in deconstruction.Parameters.Skip(1))
            {
                var outType = parameter.Type;
                var outArg = createTemp(outType);
                locals.Add(outArg);
                temps.Add(outArg);
            }

            var deconstruct = TryOptimize();

            if (deconstruct == null)
            {
                var args = new List<Expression> { target };
                args.AddRange(locals);

                deconstruct = Expression.Invoke(deconstruction, args);
            }

            effects.Add(deconstruct);

            return locals;

            Expression? TryOptimize()
            {
                if (deconstruction.Body is MethodCallExpression m)
                {
                    if (m.Method.IsStatic)
                    {
                        if (m.Arguments.SequenceEqual(deconstruction.Parameters))
                        {
                            var args = new List<Expression> { target };
                            args.AddRange(locals);

                            return m.Update(@object: null, args);
                        }
                    }
                    else
                    {
                        if (m.Object == deconstruction.Parameters[0] && m.Arguments.SequenceEqual(deconstruction.Parameters.Skip(1)))
                        {
                            return m.Update(target, locals);
                        }
                    }
                }

                return null;
            }
        }

        private static Expression EvaluateSideEffectingArgumentToTemp(Expression arg, List<Expression> effects, List<ParameterExpression> temps, Func<Type, ParameterExpression> createTemp)
        {
            var temp = createTemp(arg.Type);
            var assignmentToTemp = Expression.Assign(temp, arg);
            effects.Add(assignmentToTemp);
            temps.Add(temp);
            return temp;
        }

        private sealed class DeconstructionVariable
        {
            public DeconstructionVariable(Expression expression)
            {
                Single = expression;
            }

            public DeconstructionVariable(List<DeconstructionVariable> expression)
            {
                NestedVariables = expression;
            }

            public Expression? Single { get; }

            public List<DeconstructionVariable>? NestedVariables { get; }
        }

        private sealed class DeconstructionSideEffects
        {
            internal readonly List<Expression> init = new List<Expression>();
            internal readonly List<Expression> deconstructions = new List<Expression>();
            internal readonly List<Expression> conversions = new List<Expression>();
            internal readonly List<Expression> assignments = new List<Expression>();

            internal List<Expression> Consolidate()
            {
                init.AddRange(deconstructions);
                init.AddRange(conversions);
                init.AddRange(assignments);

                return init;
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="DeconstructionAssignmentCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitDeconstructionAssignment(DeconstructionAssignmentCSharpExpression node) =>
            node.Update(
                VisitAndConvert(node.Left, nameof(VisitDeconstructionAssignment)),
                Visit(node.Right),
                (DeconstructionConversion)VisitConversion(node.Conversion)
            );
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a deconstruction assignment expression.
        /// </summary>
        /// <param name="left">The tuple literal containing the assignment targets, which may involve nested tuple literals.</param>
        /// <param name="right">The expression representing the object to deconstruct.</param>
        /// <param name="conversion">The deconstruction conversion specifying the deconstruction step and the conversions to the elements obtained from deconstructing the object.</param>
        /// <returns>A <see cref="DeconstructionAssignmentCSharpExpression"/> representing the deconstruction assignment.</returns>
        public static DeconstructionAssignmentCSharpExpression DeconstructionAssignment(TupleLiteralCSharpExpression left, Expression right, DeconstructionConversion conversion) =>
            DeconstructionAssignment(type: null, left, right, conversion);

        /// <summary>
        /// Creates a deconstruction assignment expression.
        /// </summary>
        /// <param name="type">The return type of the deconstruction assignment.</param>
        /// <param name="left">The tuple literal containing the assignment targets, which may involve nested tuple literals.</param>
        /// <param name="right">The expression representing the object to deconstruct.</param>
        /// <param name="conversion">The deconstruction conversion specifying the deconstruction step and the conversions to the elements obtained from deconstructing the object.</param>
        /// <returns>A <see cref="DeconstructionAssignmentCSharpExpression"/> representing the deconstruction assignment.</returns>
        public static DeconstructionAssignmentCSharpExpression DeconstructionAssignment(Type? type, TupleLiteralCSharpExpression left, Expression right, DeconstructionConversion conversion)
        {
            // NB: The Roslyn compiler binds to this overload.

            RequiresCanRead(right, nameof(right));
            RequiresNotNull(conversion, nameof(conversion));

            var resultType = ValidateDeconstruction(left, right.Type, conversion, depth: 0, component: 0);

            if (type != null)
            {
                ValidateType(type, nameof(type));

                if (!AreEquivalent(type, resultType))
                    throw Error.DeconstructingAssignmentTypeMismatch(resultType, type);
            }
            else
            {
                type = resultType;
            }

            return new DeconstructionAssignmentCSharpExpression(type, left, right, conversion);

            static Type ValidateDeconstruction(Expression left, Type rhsType, Conversion rightConversion, int depth, int component)
            {
                if (rightConversion is DeconstructionConversion deconstruct)
                {
                    if (!(left is TupleLiteralCSharpExpression tuple))
                        throw Error.DeconstructingAssignmentStructureMismatch(depth, component);

                    var rightConversions = deconstruct.Conversions;
                    var leftArguments = tuple.Arguments;

                    if (rightConversions.Count != leftArguments.Count)
                        throw Error.DeconstructingAssignmentStructureMismatch(depth, component);

                    var n = rightConversions.Count;

                    var types = new Type[n];

                    for (var i = 0; i < n; i++)
                    {
                        var nestedRightConversion = rightConversions[i];
                        var nestedLeftArgument = leftArguments[i];

                        types[i] = ValidateDeconstruction(nestedLeftArgument, nestedRightConversion.InputType, nestedRightConversion, depth + 1, i);
                    }

                    return MakeTupleType(types);
                }
                else if (left is TupleLiteralCSharpExpression)
                {
                    throw Error.DeconstructingAssignmentStructureMismatch(depth, component);
                }
                else
                {
                    RequiresCanWrite(left, nameof(left));

                    if (!AreReferenceAssignable(rightConversion.InputType, rhsType))
                        throw Error.DeconstructingComponentAndConversionIncompatible(rightConversion.InputType, rhsType, depth, component);

                    if (!AreReferenceAssignable(left.Type, rightConversion.ResultType))
                        throw Error.DeconstructingComponentAndConversionIncompatible(left.Type, rightConversion.ResultType, depth, component);

                    return left.Type;
                }
            }
        }
    }
}
