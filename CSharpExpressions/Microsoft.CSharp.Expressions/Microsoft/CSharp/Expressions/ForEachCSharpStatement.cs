// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents a foreach loop.
    /// </summary>
    public abstract partial class ForEachCSharpStatement : LoopCSharpStatement
    {
        internal ForEachCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
            : base(body, breakLabel, continueLabel)
        {
            Variables = variables;
            Collection = collection;
        }

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
        public virtual LambdaExpression Conversion => null;

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the deconstruction of an element to the iteration variables.
        /// </summary>
        public virtual LambdaExpression Deconstruction => null;

        /// <summary>
        /// Gets the information required to await the MoveNextAsync operation for await foreach statements.
        /// </summary>
        public new virtual AwaitInfo AwaitInfo => null;

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
        /// <param name="breakLabel">The <see cref="LoopCSharpStatement.BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="LoopCSharpStatement.ContinueLabel" /> property of the result.</param>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="collection">The <see cref="Collection" /> property of the result.</param>
        /// <param name="conversion">The <see cref="Conversion"/> property of the result.</param>
        /// <param name="body">The <see cref="LoopCSharpStatement.Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ForEachCSharpStatement Update(LabelTarget breakLabel, LabelTarget continueLabel, IEnumerable<ParameterExpression> variables, Expression collection, LambdaExpression conversion, Expression body, LambdaExpression deconstruction, AwaitInfo awaitInfo)
        {
            if (breakLabel == BreakLabel &&
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

        internal static ForEachCSharpStatement Make(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion, LambdaExpression deconstruction, AwaitInfo awaitInfo)
        {
            // TODO: Support pattern-based approach.

            if (variables.Count == 0)
                throw new Exception(); // TODO
            
            RequiresNotNullItems(variables, nameof(variables));
            RequiresCanRead(body, nameof(body));

            ValidateLoop(body, breakLabel, continueLabel);

            var firstVariable = variables[0];
            var firstVariableType = firstVariable.Type;

            var collectionType = collection.Type;

            if (awaitInfo == null)
            {
                if (collectionType == typeof(string) && variables.Count == 1 && firstVariableType == typeof(char) && conversion == null && deconstruction == null)
                {
                    return new StringForEachStatement(variables, collection, body, breakLabel, continueLabel);
                }
                else if (collectionType.IsVector())
                {
                    var elementType = collectionType.GetElementType();

                    if (variables.Count == 1)
                    {
                        if (deconstruction != null)
                            throw new Exception(); // TODO

                        ValidateConversion(firstVariableType, elementType, ref conversion);
                    }
                    else
                    {
                        if (conversion != null)
                            throw new Exception(); // TODO
                        if (deconstruction == null)
                            throw new Exception(); // TODO

                        // TODO: validate deconstruction
                    }

                    if (conversion == null && deconstruction == null)
                    {
                        return new SimpleArrayForEachCSharpStatement(variables, collection, body, breakLabel, continueLabel);
                    }
                    else
                    {
                        return new ArrayForEachCSharpStatement(variables, collection, body, breakLabel, continueLabel, conversion, deconstruction);
                    }
                }
            }

            if (awaitInfo != null)
                throw new NotImplementedException(); // TODO

            var getEnumeratorMethod = collectionType.GetNonGenericMethod(nameof(IEnumerable.GetEnumerator), BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
            if (getEnumeratorMethod != null)
            {
                var enumeratorType = getEnumeratorMethod.ReturnType;

                var currentProperty = enumeratorType.GetProperty(nameof(IEnumerator.Current), BindingFlags.Public | BindingFlags.Instance);
                if (currentProperty == null || currentProperty.GetGetMethod(true) == null)
                {
                    throw Error.EnumeratorShouldHaveCurrentProperty(enumeratorType);
                }

                var moveNextMethod = enumeratorType.GetNonGenericMethod(nameof(IEnumerator.MoveNext), BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
                if (moveNextMethod == null || moveNextMethod.ReturnType != typeof(bool))
                {
                    throw Error.EnumeratorShouldHaveMoveNextMethod(enumeratorType);
                }

                var enumeratorInfo = new BasicEnumeratorInfo
                {
                    GetEnumerator = getEnumeratorMethod,
                    MoveNext = moveNextMethod,
                    Current = currentProperty,
                };

                var elementType = currentProperty.PropertyType;

                if (variables.Count == 1)
                {
                    if (deconstruction != null)
                        throw new Exception(); // TODO

                    var variable = variables[0];

                    ValidateConversion(variable.Type, elementType, ref conversion);
                }

                // TODO: More validation for the deconstruction case.

                return new BoundForEachCSharpStatement(variables, collection, body, breakLabel, continueLabel, conversion, enumeratorInfo, deconstruction, awaitInfo);
            }

            // NB: We don't check for implicit conversions to IE<T> or IE; the caller is responsible to insert a convert if needed.
            //     As such, we limit the checks to checks for implemented interfaces on the collection type.
            var collectionInterfaceTypes = collectionType.GetInterfaces();

            var enumerableOfT = default(Type);
            var enumerable = default(Type);
            foreach (var ifType in collectionInterfaceTypes)
            {
                if (ifType.IsGenericType && ifType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    if (enumerableOfT != null && ifType != enumerableOfT)
                    {
                        throw Error.MoreThanOneIEnumerableFound(collectionType);
                    }

                    enumerableOfT = ifType;
                }
                else if (ifType == typeof(IEnumerable))
                {
                    enumerable = ifType;
                }
            }

            var enumerableType = enumerableOfT ?? enumerable;

            if (enumerableType != null)
            {
                var getEnumerator = enumerableType.GetMethod(nameof(IEnumerable.GetEnumerator));
                var moveNext = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
                var current = getEnumerator.ReturnType.GetProperty(nameof(IEnumerator.Current));

                var enumeratorInfo = new BasicEnumeratorInfo
                {
                    GetEnumerator = getEnumerator,
                    MoveNext = moveNext,
                    Current = current,
                };

                if (variables.Count == 1)
                {
                    if (deconstruction != null)
                        throw new Exception(); // TODO
                }

                // TODO: More validation for the deconstruction case.

                return new BoundForEachCSharpStatement(variables, collection, body, breakLabel, continueLabel, conversion, enumeratorInfo, deconstruction, awaitInfo);
            }

            throw Error.NoEnumerablePattern(collectionType);
        }

        private static void ValidateConversion(Type variableType, Type elementType, ref LambdaExpression conversion)
        {
            if (conversion != null)
            {
                if (conversion.Parameters.Count != 1)
                {
                    throw Error.ConversionNeedsOneParameter();
                }

                var convertParameterType = conversion.Parameters[0].Type;
                var convertResultType = conversion.Body.Type;

                if (!AreReferenceAssignable(convertParameterType, elementType))
                {
                    throw Error.ConversionInvalidArgument(elementType, convertParameterType);
                }

                if (!AreReferenceAssignable(variableType, convertResultType))
                {
                    throw Error.ConversionInvalidResult(convertResultType, variableType);
                }
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

        private static Expression InvokeConversion(LambdaExpression conversion, Expression operand)
        {
            if (conversion.Body is UnaryExpression u && u.Operand == conversion.Parameters[0] && u.Type == conversion.ReturnType)
            {
                return u.Update(operand);
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
            internal StringForEachStatement(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variables, collection, body, breakLabel, continueLabel)
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
            protected ArrayForEachCSharpStatementBase(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variables, collection, body, breakLabel, continueLabel)
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
                        : InvokeDeconstruction(Deconstruction, getElement);
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
            public ArrayForEachCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion, LambdaExpression deconstruction)
                : base(variables, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                Deconstruction = deconstruction;
            }

            public override LambdaExpression Conversion { get; }
            public override LambdaExpression Deconstruction { get; }

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
            public SimpleArrayForEachCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variables, collection, body, breakLabel, continueLabel)
            {
            }

            protected override Expression CreateConvert(Expression element)
            {
                return Expression.Convert(element, Variables[0].Type);
            }
        }

        private struct BasicEnumeratorInfo
        {
            public PropertyInfo Current;
            public MethodInfo GetEnumerator;
            public MethodInfo MoveNext;
        }

        private sealed class BoundForEachCSharpStatement : ForEachCSharpStatement
        {
            private readonly BasicEnumeratorInfo _enumeratorInfo;

            internal BoundForEachCSharpStatement(ReadOnlyCollection<ParameterExpression> variables, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion, BasicEnumeratorInfo enumeratorInfo, LambdaExpression deconstruction, AwaitInfo awaitInfo)
                : base(variables, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                _enumeratorInfo = enumeratorInfo;
                Deconstruction = deconstruction;
                AwaitInfo = awaitInfo;
            }

            public override LambdaExpression Conversion { get; }

            public override LambdaExpression Deconstruction { get; }

            public override AwaitInfo AwaitInfo { get; }

            protected override Expression ReduceCore()
            {
                if (AwaitInfo != null)
                    throw new NotImplementedException(); // TODO

                var getEnumerator = Expression.Call(Collection, _enumeratorInfo.GetEnumerator);
                var enumeratorType = getEnumerator.Type;
                var enumeratorVariable = Expression.Parameter(enumeratorType, "__enumerator");

                var moveNext = Expression.Call(enumeratorVariable, _enumeratorInfo.MoveNext);
                var current = (Expression)Expression.Property(enumeratorVariable, _enumeratorInfo.Current);

                if (Conversion != null)
                {
                    current = InvokeConversion(Conversion, current);
                }

                var cleanup = (Expression)Expression.Empty();
                if (typeof(IDisposable).IsAssignableFrom(enumeratorType))
                {
                    if (enumeratorType.IsValueType)
                    {
                        // NB: C# spec section 8.8.4 specifies a case for E being a nullable value type;
                        //     however, it seems this case can't occur because the check for the foreach
                        //     pattern would fail if E is a nullable value type (no Current property).
                        //     Double-check this.
                        Debug.Assert(!enumeratorType.IsNullableType());

                        var dispose = enumeratorType.FindDisposeMethod(isAsync: false);
                        cleanup = Expression.Call(enumeratorVariable, dispose);
                    }
                    else
                    {
                        var dispose = enumeratorType.FindDisposeMethod(isAsync: false);
                        cleanup =
                            Expression.IfThen(
                                Expression.ReferenceNotEqual(enumeratorVariable, Expression.Constant(null, typeof(IDisposable))),
                                Expression.Call(enumeratorVariable, dispose)
                            );
                    }
                }
                else if (!enumeratorType.IsSealed)
                {
                    var d = Expression.Parameter(typeof(IDisposable), "__disposable");
                    cleanup =
                        Expression.Block(
                            new[] { d },
                            Expression.Assign(d, Expression.TypeAs(enumeratorVariable, typeof(IDisposable))),
                            Expression.IfThen(
                                Expression.ReferenceNotEqual(d, Expression.Constant(null, typeof(IDisposable))),
                                Expression.Call(d, typeof(IDisposable).GetMethod("Dispose"))
                            )
                        );
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
                                        : InvokeDeconstruction(Deconstruction, current),
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
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break) =>
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
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue) =>
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
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion) =>
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
        public static ForEachCSharpStatement ForEach(IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion, LambdaExpression deconstruction) =>
            ForEach(awaitInfo: null, variables, collection, body, @break, @continue, conversion, deconstruction);

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents an await foreach loop.
        /// </summary>
        /// <param name="awaitInfo">The information required to await the MoveNextAsync operation.</param>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo awaitInfo, ParameterExpression variable, Expression collection, Expression body) =>
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
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget @break) =>
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
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue) =>
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
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo awaitInfo, ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion) =>
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
        public static ForEachCSharpStatement AwaitForEach(AwaitInfo awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion, LambdaExpression deconstruction)
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
        public static ForEachCSharpStatement ForEach(AwaitInfo awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion, LambdaExpression deconstruction) =>
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
        public static ForEachCSharpStatement ForEach(EnumeratorInfo enumeratorInfo, AwaitInfo awaitInfo, IEnumerable<ParameterExpression> variables, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion, LambdaExpression deconstruction)
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

            // TODO: Validate elements from the enumeration can be bound to iteration variables (with optional deconstruction)
            //       and feed the EnumeratorInfo object down to the factory.

            return ForEachCSharpStatement.Make(variablesCollection, collection, body, @break, @continue, conversion, deconstruction, awaitInfo);
        }

        private static void AssertForEachAwaitInfo(ref AwaitInfo awaitInfo, Expression collection)
        {
            awaitInfo ??= new InferredAwaitInfo();
        }

        private sealed class InferredAwaitInfo : AwaitInfo
        {
            private AwaitInfo _info;

            public void Bind(EnumeratorInfo info)
            {
                _info = CSharpExpression.AwaitInfo(info.MoveNext.ReturnType);
            }

            public override bool IsDynamic => _info.IsDynamic;

            public override Type Type => _info.Type;

            protected internal override AwaitInfo Accept(CSharpExpressionVisitor visitor) => _info.Accept(visitor);

            internal override Expression ReduceGetAwaiter(Expression operand) => _info.ReduceGetAwaiter(operand);

            internal override Expression ReduceGetResult(Expression awaiter) => _info.ReduceGetResult(awaiter);

            internal override Expression ReduceIsCompleted(Expression awaiter) => _info.ReduceIsCompleted(awaiter);

            internal override void RequiresCanBind(Expression operand) => _info.RequiresCanBind(operand);
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
