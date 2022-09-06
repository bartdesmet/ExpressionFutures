// Prototyping extended expression trees for C#.
//
// bartde - October 2015

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a foreach loop.
    /// </summary>
    public abstract partial class ForEachCSharpStatement : LoopCSharpStatement
    {
        internal ForEachCSharpStatement(EnumeratorInfo enumeratorInfo, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel)
            : base(body, breakLabel, continueLabel)
        {
            EnumeratorInfo = enumeratorInfo;
            Variables = variables;
            Collection = collection;
        }

        /// <summary>
        /// Gets the <see cref="EnumeratorInfo"/> that provides binding information for the enumeration of the <see cref="Collection"/>.
        /// </summary>
        public new EnumeratorInfo EnumeratorInfo { get; }

        /// <summary>
        /// Gets the collection of <see cref="ParameterExpression" /> objects representing the iteration variables.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the collection.
        /// </summary>
        public Expression Collection { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the conversion of an element to the iteration variable.
        /// </summary>
        public virtual LambdaExpression? Conversion => null;

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the deconstruction of an element to the iteration variables.
        /// </summary>
        public virtual LambdaExpression? Deconstruction => null;

        /// <summary>
        /// Gets the information required to await the MoveNextAsync operation for await foreach statements.
        /// </summary>
        public new virtual AwaitInfo? AwaitInfo => null;

        /// <summary>
        /// Gets a Boolean indicating whether the foreach statement is asynchronous.
        /// </summary>
        public bool IsAsync => AwaitInfo != null;

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType => CSharpExpressionType.ForEach;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitForEach(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="enumeratorInfo">The <see cref="EnumeratorInfo" /> property of the result.</param>
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="variables">The <see cref="Variables" /> property of the result.</param>
        /// <param name="collection">The <see cref="Collection" /> property of the result.</param>
        /// <param name="conversion">The <see cref="Conversion"/> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <param name="deconstruction">The <see cref="Deconstruction"/> property of the result.</param>
        /// <param name="awaitInfo">The <see cref="AwaitInfo"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ForEachCSharpStatement Update(EnumeratorInfo enumeratorInfo, LabelTarget? breakLabel, LabelTarget? continueLabel, IEnumerable<ParameterExpression> variables, Expression collection, LambdaExpression? conversion, Expression body, LambdaExpression? deconstruction, AwaitInfo? awaitInfo)
        {
            if (enumeratorInfo == EnumeratorInfo &&
                breakLabel == BreakLabel &&
                continueLabel == ContinueLabel &&
                SameElements(ref variables, Variables) &&
                collection == Collection &&
                conversion == Conversion &&
                body == Body &&
                deconstruction == Deconstruction &&
                awaitInfo == AwaitInfo)
            {
                return this;
            }

            return CSharpExpression.ForEach(awaitInfo, variables, collection, body, breakLabel, continueLabel, conversion, deconstruction);
        }

        internal static ForEachCSharpStatement Make(EnumeratorInfo enumeratorInfo, AwaitInfo? awaitInfo, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, LambdaExpression? conversion, LambdaExpression? deconstruction)
        {
            if (variables.Count == 0)
                throw Error.ForEachNeedsOneOrMoreVariables();

            RequiresNotNullItems(variables, nameof(variables));

            if (!AreReferenceAssignable(enumeratorInfo.CollectionType, collection.Type))
                throw Error.ForEachCollectionTypeNotCompatibleWithCollectionExpression(enumeratorInfo.CollectionType, collection.Type);

            RequiresCanRead(body, nameof(body));

            ValidateLoop(body, breakLabel, continueLabel);

            var firstVariable = variables[0];
            var firstVariableType = firstVariable.Type;

            if (variables.Count == 1)
            {
                if (deconstruction != null)
                    throw Error.ForEachDeconstructionNotSupportedWithOneVariable();

                ValidateConversion(firstVariableType, enumeratorInfo.ElementType, ref conversion);
            }
            else
            {
                if (deconstruction == null)
                    throw Error.ForEachDeconstructionRequiredForMultipleVariables();

                ValidateDeconstruction(enumeratorInfo.ElementType, ref conversion, deconstruction, variables);
            }

            if (awaitInfo == null)
            {
                if (collection.Type == typeof(string) && variables.Count == 1 && firstVariableType == typeof(char) && conversion == null && deconstruction == null)
                {
                    return new StringForEachStatement(enumeratorInfo, variables, collection, body, breakLabel, continueLabel);
                }
                else if (collection.Type.IsArray)
                {
                    if (collection.Type.IsVector())
                    {
                        if (conversion == null && deconstruction == null)
                        {
                            return new SimpleArrayForEachCSharpStatement(enumeratorInfo, variables, collection, body, breakLabel, continueLabel);
                        }
                        else
                        {
                            return new ArrayForEachCSharpStatement(enumeratorInfo, variables, collection, body, breakLabel, continueLabel, conversion, deconstruction);
                        }
                    }
                    else
                    {
                        return new MultiDimensionalArrayForEachCSharpStatement(enumeratorInfo, variables, collection, body, breakLabel, continueLabel, conversion, deconstruction);
                    }
                }
            }
            else
            {
                awaitInfo.RequiresCanBind(enumeratorInfo.MoveNext.Body);
            }

            return new BoundForEachCSharpStatement(enumeratorInfo, variables, collection, body, breakLabel, continueLabel, conversion, deconstruction, awaitInfo);
        }

        private static void ValidateConversion(Type variableType, Type elementType, ref LambdaExpression? conversion)
        {
            if (conversion != null)
            {
                if (conversion.Parameters.Count != 1)
                    throw Error.ConversionNeedsOneParameter();

                var convertParameterType = conversion.Parameters[0].Type;
                var convertResultType = conversion.Body.Type;

                if (!AreReferenceAssignable(convertParameterType, elementType))
                    throw Error.ConversionInvalidArgument(elementType, convertParameterType);

                if (!AreReferenceAssignable(variableType, convertResultType))
                    throw Error.ConversionInvalidResult(convertResultType, variableType);
            }
            else if (!AreReferenceAssignable(variableType, elementType))
            {
                var element = Expression.Parameter(elementType, "__element");

                // NB: The LINQ factory will perform the necessary checks.
                // NB: If checked conversion is needed, one should explicitly specify a conversion lambda.
                var convert = Expression.Convert(element, variableType);

                conversion = Expression.Lambda(convert, element);
            }
        }

        private static void ValidateDeconstruction(Type elementType, ref LambdaExpression? conversion, LambdaExpression deconstruction, ReadOnlyCollection<ParameterExpression> variables)
        {
            if (deconstruction.Parameters.Count != 1)
                throw Error.ForEachDeconstructionShouldHaveOneParameter();

            var variableType = deconstruction.Parameters[0].Type;

            ValidateConversion(variableType, elementType, ref conversion);

            if (!IsTupleType(deconstruction.ReturnType))
                throw Error.ForEachDeconstructionShouldReturnTuple(deconstruction.ReturnType);

            var componentTypes = GetTupleComponentTypes(deconstruction.ReturnType).ToArray();

            if (componentTypes.Length != variables.Count)
                throw Error.ForEachDeconstructionComponentMismatch(deconstruction.ReturnType, componentTypes.Length);

            for (var i = 0; i < componentTypes.Length; i++)
            {
                if (!AreReferenceAssignable(variables[i].Type, componentTypes[i]))
                    throw Error.ForEachDeconstructionComponentNotAssignableToVariable(componentTypes[i], i, variables[i], variables[i].Type);
            }
        }

        private static void ApplyConversion(LambdaExpression? conversion, ref Expression operand)
        {
            if (conversion != null)
            {
                operand = InvokeConversion(conversion, operand);
            }
        }

        private static Expression InvokeConversion(LambdaExpression conversion, Expression operand)
        {
            var parameter = conversion.Parameters[0];
            var body = conversion.Body;

            if (body.Type == conversion.ReturnType)
            {
                if (body == parameter)
                {
                    return operand;
                }
                else if (body is UnaryExpression u && u.Operand == parameter)
                {
                    return u.Update(operand);
                }
            }

            return Expression.Invoke(conversion, operand);
        }

        private static Expression InvokeDeconstruction(LambdaExpression deconstruction, Expression operand)
        {
            // NB: The expression tree conversion in Roslyn ends up constructing a lambda to represent the deconstruction step,
            //     which upon reduction leads to some nested blocks. We recognize the typical pattern here and do some inlining
            //     to make the reduced code more sane.

            if (deconstruction.Body is DeconstructionAssignmentCSharpExpression d && d.Right == deconstruction.Parameters[0])
            {
                var res = d.Update(d.Left, operand, d.Conversion);

                var reduced = res.Reduce();

                reduced = FlattenBlocks(reduced);
                reduced = DropDiscardedTupleLiteralCreationStep(reduced);

                return reduced;

                static Expression FlattenBlocks(Expression expr)
                {
                    while (expr is BlockExpression b && b.Variables.Count == 0 && b.Expressions.Count == 1 && b.Type == b.Result.Type)
                    {
                        expr = b.Result;
                    }

                    return expr;
                }

                static Expression DropDiscardedTupleLiteralCreationStep(Expression expr)
                {
                    if (expr is BlockExpression b && b.Expressions.Count > 1 && b.Result is TupleLiteralCSharpExpression)
                    {
                        return Expression.Block(typeof(void), b.Variables, b.Expressions.Take(b.Expressions.Count - 1));
                    }

                    return expr;
                }
            }

            return Expression.Invoke(deconstruction, operand);
        }

        private sealed class StringForEachStatement : ForEachCSharpStatement
        {
            internal StringForEachStatement(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
            }

            protected override Expression ReduceCore()
            {
                var collection = Expression.Parameter(Collection.Type, "__collection");
                var variable = Variables[0];

                var index = Expression.Parameter(typeof(int), "__index");
                var length = Expression.Property(collection, "Length"); // NB: The C# compiler invokes Length over and over; could we store it in a local?

                var n = 1 /* collection */ + 1 /* index */ + 1 /* goto check */ + 1 /* iterate */ + 1 /* element */ + 1 /* body */ + 1 /* index++ */ + 1 /* check */ + 1 /* test */;

                if (BreakLabel != null)
                {
                    n++;
                }

                if (ContinueLabel != null)
                {
                    n++;
                }

                var exprs = new Expression[n];

                var i = 0;

                var check = Expression.Label("__check");
                var iterate = Expression.Label("__iterate");

                var indexer = typeof(string).GetProperty("Chars");

                exprs[i++] =
                    Expression.Assign(collection, Collection);
                exprs[i++] =
                    Expression.Assign(index, CreateConstantInt32(0));
                exprs[i++] =
                    Expression.Goto(check);
                exprs[i++] =
                    Expression.Label(iterate);
                exprs[i++] =
                    Expression.Assign(variable, Expression.MakeIndex(collection, indexer, new[] { index }));
                exprs[i++] =
                    Body;

                if (ContinueLabel != null)
                {
                    exprs[i++] =
                        Expression.Label(ContinueLabel);
                }

                exprs[i++] =
                    Expression.PreIncrementAssign(index); // NB: Less locals than PostIncrementAssign upon reduction.
                exprs[i++] =
                    Expression.Label(check);
                exprs[i++] =
                    Expression.IfThen(
                        Expression.LessThan(index, length),
                        Expression.Goto(iterate)
                    );

                if (BreakLabel != null)
                {
                    exprs[i++] =
                        Expression.Label(BreakLabel);
                }

                var res =
                    Expression.Block(
                        new[] { collection, index, variable },
                        exprs
                    );

                return res;
            }
        }

        private abstract class ArrayForEachCSharpStatementBase : ForEachCSharpStatement
        {
            protected ArrayForEachCSharpStatementBase(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
            }

            protected abstract Expression CreateConvert(Expression element);

            protected override Expression ReduceCore()
            {
                var collection = Expression.Parameter(Collection.Type, "__collection");

                var index = Expression.Parameter(typeof(int), "__index");
                var length = Expression.ArrayLength(collection); // NB: The C# compiler executes `ldlen` over and over; could we store it in a local?

                var n = 1 /* collection */ + 1 /* index */ + 1 /* goto check */ + 1 /* iterate */ + 1 /* element */ + 1 /* body */ + 1 /* index++ */ + 1 /* check */ + 1 /* test */;

                if (BreakLabel != null)
                {
                    n++;
                }

                if (ContinueLabel != null)
                {
                    n++;
                }

                var getElement = CreateConvert(Expression.ArrayIndex(collection, index));

                var exprs = new Expression[n];

                var i = 0;

                var check = Expression.Label("__check");
                var iterate = Expression.Label("__iterate");

                exprs[i++] =
                    Expression.Assign(collection, Collection);
                exprs[i++] =
                    Expression.Assign(index, CreateConstantInt32(0));
                exprs[i++] =
                    Expression.Goto(check);
                exprs[i++] =
                    Expression.Label(iterate);
                exprs[i++] =
                    Variables.Count == 1
                        ? Expression.Assign(Variables[0], getElement)
                        : InvokeDeconstruction(Deconstruction!, getElement); // NB: More than one variable requires deconstruction.
                exprs[i++] =
                    Body;

                if (ContinueLabel != null)
                {
                    exprs[i++] =
                        Expression.Label(ContinueLabel);
                }

                exprs[i++] =
                    Expression.PreIncrementAssign(index); // NB: Less locals than PostIncrementAssign upon reduction.
                exprs[i++] =
                    Expression.Label(check);
                exprs[i++] =
                    Expression.IfThen(
                        Expression.LessThan(index, length),
                        Expression.Goto(iterate)
                    );

                if (BreakLabel != null)
                {
                    exprs[i++] =
                        Expression.Label(BreakLabel);
                }

                var res =
                    Expression.Block(
                        new[] { collection, index }.Concat(Variables),
                        exprs
                    );

                return res;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Only part of the hierarchy is visible publicly.")]
        private sealed class ArrayForEachCSharpStatement : ArrayForEachCSharpStatementBase
        {
            public ArrayForEachCSharpStatement(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, LambdaExpression? conversion, LambdaExpression? deconstruction)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                Deconstruction = deconstruction;
            }

            public override LambdaExpression? Conversion { get; }
            public override LambdaExpression? Deconstruction { get; }

            protected override Expression CreateConvert(Expression element)
            {
                if (Conversion != null)
                {
                    return InvokeConversion(Conversion, element);
                }

                return element;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Only part of the hierarchy is visible publicly.")]
        private sealed class SimpleArrayForEachCSharpStatement : ArrayForEachCSharpStatementBase
        {
            public SimpleArrayForEachCSharpStatement(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
            }

            protected override Expression CreateConvert(Expression element)
            {
                return Expression.Convert(element, Variables[0].Type);
            }
        }

        private sealed class MultiDimensionalArrayForEachCSharpStatement : ForEachCSharpStatement
        {
            public MultiDimensionalArrayForEachCSharpStatement(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, LambdaExpression? conversion, LambdaExpression? deconstruction)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                Deconstruction = deconstruction;
            }

            public override LambdaExpression? Conversion { get; }
            public override LambdaExpression? Deconstruction { get; }

            protected override Expression ReduceCore()
            {
                var temps = new List<ParameterExpression>();
                var stmts = new List<Expression>();

                var array = Expression.Parameter(Collection.Type, "__array");

                temps.Add(array);
                stmts.Add(Expression.Assign(array, Collection));

                var rank = Collection.Type.GetArrayRank();

                var rangeVariables = new List<ParameterExpression>(rank);
                var loops = new List<LoopInfo>(rank);

                for (var i = 0; i < rank; i++)
                {
                    var upperBound = Expression.Parameter(typeof(int), "__u" + i);
                    temps.Add(upperBound);
                    stmts.Add(Expression.Assign(upperBound, Expression.Call(array, GetUpperBound, CreateConstantInt32(i))));

                    var rangeVariable = Expression.Parameter(typeof(int), "__i" + i);
                    rangeVariables.Add(rangeVariable);

                    var loopInfo = new LoopInfo(
                        variable: rangeVariable,
                        initializer: Expression.Assign(rangeVariable, Expression.Call(array, GetLowerBound, CreateConstantInt32(i))),
                        condition: Expression.LessThanOrEqual(rangeVariable, upperBound),
                        increment: Expression.Assign(rangeVariable, Expression.Add(rangeVariable, CreateConstantInt32(1)))
                    );

                    loops.Add(loopInfo);
                }

                var element = (Expression)Expression.ArrayAccess(array, rangeVariables);

                if (Conversion != null)
                {
                    element = InvokeConversion(Conversion, element);
                }

                var bodyStmts = new List<Expression>(2);

                if (Variables.Count == 1)
                {
                    bodyStmts.Add(Expression.Assign(Variables[0], element));
                }
                else
                {
                    Debug.Assert(Deconstruction != null); // NB: More than one variable requires deconstruction.
                    bodyStmts.Add(InvokeDeconstruction(Deconstruction, element));
                }

                bodyStmts.Add(Body);

                var body = Expression.Block(typeof(void), Variables, bodyStmts);

                var loop = default(Expression);

                for (var i = rank - 1; i >= 0; i--)
                {
                    var info = loops[i];

                    var breakLabel = i == 0 ? BreakLabel : Expression.Label("__break" + i);

                    Expression loopBody;
                    LabelTarget? continueLabel;

                    if (loop == null)
                    {
                        loopBody = body;
                        continueLabel = ContinueLabel;
                    }
                    else
                    {
                        loopBody = loop;
                        continueLabel = Expression.Label("__continue" + i);
                    }

                    loop = CSharpExpression.For(new[] { info.Variable }, new[] { info.Initializer }, info.Condition, new[] { info.Increment }, loopBody, breakLabel, continueLabel);
                }

                stmts.Add(loop!); // NB: Rank is always > 0.

                return Expression.Block(typeof(void), temps, stmts);
            }

            private static MethodInfo? s_getUpperBound, s_getLowerBound;

            private static MethodInfo GetUpperBound => s_getUpperBound ??= typeof(Array).GetMethod(nameof(Array.GetUpperBound))!; // TODO: well-known members
            private static MethodInfo GetLowerBound => s_getLowerBound ??= typeof(Array).GetMethod(nameof(Array.GetLowerBound))!; // TODO: well-known members

            private sealed class LoopInfo
            {
                public readonly ParameterExpression Variable;
                public readonly Expression Initializer;
                public readonly Expression Condition;
                public readonly Expression Increment;

                public LoopInfo(ParameterExpression variable, Expression initializer, Expression condition, Expression increment)
                {
                    Variable = variable;
                    Initializer = initializer;
                    Condition = condition;
                    Increment = increment;
                }
            }
        }

        private sealed class BoundForEachCSharpStatement : ForEachCSharpStatement
        {
            internal BoundForEachCSharpStatement(EnumeratorInfo info, ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? breakLabel, LabelTarget? continueLabel, LambdaExpression? conversion, LambdaExpression? deconstruction, AwaitInfo? awaitInfo)
                : base(info, variables, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                Deconstruction = deconstruction;
                AwaitInfo = awaitInfo;
            }

            public override LambdaExpression? Conversion { get; }

            public override LambdaExpression? Deconstruction { get; }

            public override AwaitInfo? AwaitInfo { get; }

            protected override Expression ReduceCore()
            {
                Expression MakeGetEnumerator(Expression collection) =>
                    InvokeLambdaWithSingleParameter(EnumeratorInfo.GetEnumerator, collection);

                Expression MakeMoveNext(Expression enumeratorVariable)
                {
                    var moveNext = InvokeLambdaWithSingleParameter(EnumeratorInfo.MoveNext, enumeratorVariable);

                    if (AwaitInfo is { } awaitInfo)
                    {
                        return CSharpExpression.Await(moveNext, awaitInfo);
                    }

                    return moveNext;
                }

                Expression MakeCurrent(Expression enumeratorVariable) =>
                    Expression.Property(enumeratorVariable, EnumeratorInfo.Current);

                var getEnumerator = MakeGetEnumerator(Collection);
                var enumeratorType = getEnumerator.Type;
                var enumeratorVariable = Expression.Parameter(enumeratorType, "__enumerator");

                var moveNext = MakeMoveNext(enumeratorVariable);
                var current = MakeCurrent(enumeratorVariable);

                ApplyConversion(EnumeratorInfo.CurrentConversion, ref current); // REVIEW: Two conversions possible?
                ApplyConversion(Conversion, ref current);

                var cleanup = (Expression)Expression.Empty();

                if (EnumeratorInfo.NeedsDisposal)
                {
                    Expression AwaitDisposeIfNeeded(Expression dispose)
                    {
                        if (EnumeratorInfo.DisposeAwaitInfo is { } awaitInfo)
                        {
                            return CSharpExpression.Await(dispose, awaitInfo);
                        }

                        return dispose;
                    }

                    Expression InvokeDispose(Expression obj, MethodInfo method)
                    {
                        var dispose = Expression.Call(obj, method);
                        return AwaitDisposeIfNeeded(dispose);
                    }

                    if (EnumeratorInfo.PatternDispose is { } patternDispose)
                    {
                        var dispose = InvokeLambdaWithSingleParameter(patternDispose, enumeratorVariable);
                        cleanup = AwaitDisposeIfNeeded(dispose);
                    }
                    else
                    {
                        var disposableInterface = EnumeratorInfo.IsAsync
                            ? typeof(IAsyncDisposable)
                            : typeof(IDisposable);

                        if (disposableInterface.IsAssignableFrom(enumeratorType))
                        {
                            if (enumeratorType.IsValueType)
                            {
                                // NB: C# spec section 8.8.4 specifies a case for E being a nullable value type;
                                //     however, it seems this case can't occur because the check for the foreach
                                //     pattern would fail if E is a nullable value type (no Current property).
                                //     Double-check this.
                                Debug.Assert(!enumeratorType.IsNullableType());

                                var dispose = enumeratorType.FindDisposeMethod(EnumeratorInfo.IsAsync);
                                cleanup = InvokeDispose(enumeratorVariable, dispose);
                            }
                            else
                            {
                                var dispose = enumeratorType.FindDisposeMethod(EnumeratorInfo.IsAsync);
                                cleanup =
                                    Expression.IfThen(
                                        Expression.ReferenceNotEqual(enumeratorVariable, Expression.Constant(null, disposableInterface)),
                                        InvokeDispose(enumeratorVariable, dispose)
                                    );
                            }
                        }
                        else if (!enumeratorType.IsSealed)
                        {
                            var disposeMethod = EnumeratorInfo.IsAsync
                                ? UsingCSharpStatement.DisposeAsyncMethod
                                : UsingCSharpStatement.DisposeMethod;

                            var d = Expression.Parameter(disposableInterface, "__disposable");
                            cleanup =
                                Expression.Block(
                                    new[] { d },
                                    Expression.Assign(d, Expression.TypeAs(enumeratorVariable, disposableInterface)),
                                    Expression.IfThen(
                                        Expression.ReferenceNotEqual(d, Expression.Constant(null, disposableInterface)),
                                        InvokeDispose(d, disposeMethod)
                                    )
                                );
                        }
                    }
                }

                var res =
                    Expression.Block(
                        new[] { enumeratorVariable },
                        Expression.Assign(enumeratorVariable, getEnumerator),
                        Expression.TryFinally(
                            CSharpExpression.While(
                                moveNext,
                                // NB: This is using C# 5.0 scoping rules for the loop variable.
                                Expression.Block(
                                    Variables,
                                    Variables.Count == 1
                                        ? Expression.Assign(Variables[0], current)
                                        : InvokeDeconstruction(Deconstruction!, current), // NB: More than one variable requires deconstruction.
                                    Body
                                ),
                                BreakLabel,
                                ContinueLabel
                            ),
                            cleanup
                        )
                    );

                return res;
            }
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body) =>
            ForEach(variable, collection, body, @break: null, @continue: null, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break) =>
            ForEach(variable, collection, body, @break, @continue: null, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue) =>
            ForEach(variable, collection, body, @break, @continue, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion) =>
            ForEach(awaitInfo: null, new[] { variable }, collection, body, @break, @continue, conversion, deconstruction: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variables">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <param name="deconstruction">The deconstruction step used to deconstruct elements in the collection and assign to the iteration variables.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion, LambdaExpression deconstruction) =>
            ForEach(awaitInfo: null, variables, collection, body, @break, @continue, conversion, deconstruction);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo? awaitInfo, ParameterExpression variable, Expression collection, Expression body) =>
            AwaitForEach(awaitInfo, variable, collection, body, @break: null, @continue: null, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo? awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break) =>
            AwaitForEach(awaitInfo, variable, collection, body, @break, @continue: null, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo? awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue) =>
            AwaitForEach(awaitInfo, variable, collection, body, @break, @continue, conversion: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo? awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion) =>
            AwaitForEach(awaitInfo, new[] { variable }, collection, body, @break, @continue, conversion, deconstruction: null);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variables">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <param name="deconstruction">The deconstruction step used to deconstruct elements in the collection and assign to the iteration variables.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion, LambdaExpression? deconstruction)
        {
            AssertForEachAwaitInfo(ref awaitInfo, collection);

            return ForEach(awaitInfo, variables, collection, body, @break, @continue, conversion, deconstruction);
        }

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variables">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <param name="deconstruction">The deconstruction step used to deconstruct elements in the collection and assign to the iteration variables.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(AwaitInfo? awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion, LambdaExpression? deconstruction) =>
            ForEach(enumeratorInfo: null, awaitInfo, variables, collection, body, @break, @continue, conversion, deconstruction);

        // NB: The Roslyn compiler binds to the overload below.

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="enumeratorInfo">The information required to perform the enumeration.</param>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variables">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <param name="conversion">The conversion function used to convert elements in the collection to the iteration variable type.</param>
        /// <param name="deconstruction">The deconstruction step used to deconstruct elements in the collection and assign to the iteration variables.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(EnumeratorInfo? enumeratorInfo, AwaitInfo? awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget? @break, LabelTarget? @continue, LambdaExpression? conversion, LambdaExpression? deconstruction)
        {
            // NB: Conversion of the collection should be inserted as a Convert node by the compiler.

            RequiresCanRead(collection, nameof(collection));

            enumeratorInfo ??= CSharpExpression.EnumeratorInfo(isAsync: awaitInfo != null, collection.Type);

            if (awaitInfo is InferredAwaitInfo inferAwait)
            {
                inferAwait.Bind(enumeratorInfo);
            }
            else
            {
                awaitInfo?.RequiresCanBind(enumeratorInfo.MoveNext.Body);
            }

            var variablesCollection = variables.ToReadOnly();

            return ForEachCSharpStatement.Make(enumeratorInfo, awaitInfo, variablesCollection, collection, body, @break, @continue, conversion, deconstruction);
        }

        private static void AssertForEachAwaitInfo(ref AwaitInfo? awaitInfo, Expression collection)
        {
            awaitInfo ??= new InferredAwaitInfo();
        }

        private sealed class InferredAwaitInfo : AwaitInfo
        {
            private AwaitInfo? _info;

            public void Bind(EnumeratorInfo info)
            {
                _info = CSharpExpression.AwaitInfo(info.MoveNext.ReturnType);
            }

            private AwaitInfo Info
            {
                get
                {
                    Debug.Assert(_info != null, "Bind should have been called.");
                    return _info;
                }
            }

            public override bool IsDynamic => Info.IsDynamic;

            public override Type Type => Info.Type;

            protected internal override AwaitInfo Accept(CSharpExpressionVisitor visitor) => Info.Accept(visitor);

            internal override Expression ReduceGetAwaiter(Expression operand) => Info.ReduceGetAwaiter(operand);

            internal override Expression ReduceGetResult(Expression awaiter) => Info.ReduceGetResult(awaiter);

            internal override Expression ReduceIsCompleted(Expression awaiter) => Info.ReduceIsCompleted(awaiter);

            internal override void RequiresCanBind(Expression operand) => Info.RequiresCanBind(operand);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="ForEachCSharpStatement" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitForEach(ForEachCSharpStatement node) =>
            node.Update(
                VisitEnumeratorInfo(node.EnumeratorInfo),
                VisitLabelTarget(node.BreakLabel),
                VisitLabelTarget(node.ContinueLabel),
                VisitAndConvert(node.Variables, nameof(VisitForEach)),
                Visit(node.Collection),
                VisitAndConvert(node.Conversion, nameof(VisitForEach)),
                Visit(node.Body),
                VisitAndConvert(node.Deconstruction, nameof(VisitForEach)),
                VisitAwaitInfo(node.AwaitInfo)
            );
    }
}
