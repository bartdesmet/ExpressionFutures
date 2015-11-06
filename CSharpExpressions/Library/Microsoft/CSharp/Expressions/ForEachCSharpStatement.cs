// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using static System.Dynamic.Utils.TypeUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents a foreach loop.
    /// </summary>
    public abstract class ForEachCSharpStatement : LoopCSharpStatement
    {
        internal ForEachCSharpStatement(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
            : base(body, breakLabel, continueLabel)
        {
            Variable = variable;
            Collection = collection;
        }

        /// <summary>
        /// Gets the <see cref="ParameterExpression" /> representing an element in the collection.
        /// </summary>
        public new ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression" /> representing the collection.
        /// </summary>
        public Expression Collection { get; }

        /// <summary>
        /// Gets the <see cref="LambdaExpression"/> representing the conversion of an element to the iteration variable.
        /// </summary>
        public virtual LambdaExpression Conversion => null;

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitForEach(this);
        }

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
        public ForEachCSharpStatement Update(LabelTarget breakLabel, LabelTarget continueLabel, ParameterExpression variable, Expression collection, LambdaExpression conversion, Expression body)
        {
            if (breakLabel == this.BreakLabel && continueLabel == this.ContinueLabel && variable == this.Variable && collection == this.Collection && conversion == this.Conversion && body == this.Body)
            {
                return this;
            }

            return CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conversion);
        }

        internal static ForEachCSharpStatement Make(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion)
        {
            RequiresCanWrite(variable, nameof(variable));
            RequiresCanRead(collection, nameof(collection));
            RequiresCanRead(body, nameof(body));

            ValidateLoop(body, breakLabel, continueLabel);

            var variableType = variable.Type;
            var collectionType = collection.Type;

            if (collectionType == typeof(string) && variableType == typeof(char) && conversion == null)
            {
                return new StringForEachStatement(variable, collection, body, breakLabel, continueLabel);
            }
            else if (collectionType.IsVector())
            {
                var elementType = collectionType.GetElementType();

                ValidateConversion(variableType, elementType, ref conversion);

                if (conversion != null)
                {
                    return new ArrayForEachCSharpStatement(variable, collection, body, breakLabel, continueLabel, conversion);
                }
                else
                {
                    return new SimpleArrayForEachCSharpStatement(variable, collection, body, breakLabel, continueLabel);
                }
            }

            var getEnumeratorMethod = collectionType.GetNonGenericMethod("GetEnumerator", BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
            if (getEnumeratorMethod != null)
            {
                var enumeratorType = getEnumeratorMethod.ReturnType;

                var currentProperty = enumeratorType.GetProperty("Current", BindingFlags.Public | BindingFlags.Instance);
                if (currentProperty == null || currentProperty.GetGetMethod(true) == null)
                {
                    throw Error.EnumeratorShouldHaveCurrentProperty(enumeratorType);
                }

                var moveNextMethod = enumeratorType.GetNonGenericMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
                if (moveNextMethod == null || moveNextMethod.ReturnType != typeof(bool))
                {
                    throw Error.EnumeratorShouldHaveMoveNextMethod(enumeratorType);
                }

                var elementType = currentProperty.PropertyType;

                ValidateConversion(variableType, elementType, ref conversion);

                return new BoundForEachCSharpStatement(variable, collection, body, breakLabel, continueLabel, conversion, getEnumeratorMethod, moveNextMethod, currentProperty);
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
                var moveNext = typeof(IEnumerator).GetMethod("MoveNext");

                var getEnumerator = enumerableType.GetMethod("GetEnumerator");
                var current = getEnumerator.ReturnType.GetProperty("Current");

                return new BoundForEachCSharpStatement(variable, collection, body, breakLabel, continueLabel, conversion, getEnumerator, moveNext, current);
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
                var element = Expression.Parameter(elementType);

                // NB: The LINQ factory will perform the necessary checks.
                // NB: If checked conversion is needed, one should explicitly specify a conversion lambda.
                var convert = Expression.Convert(element, variableType);

                conversion = Expression.Lambda(convert, element);
            }
        }

        class StringForEachStatement : ForEachCSharpStatement
        {
            internal StringForEachStatement(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variable, collection, body, breakLabel, continueLabel)
            {
            }

            protected override Expression ReduceCore()
            {
                var collection = Expression.Parameter(Collection.Type);
                var variable = Variable;

                var index = Expression.Parameter(typeof(int));
                var length = Expression.Property(collection, "Length"); // NB: The C# compiler invokes Length over and over; could we store it in a local?

                var @break = BreakLabel ?? Expression.Label();
                var @continue = ContinueLabel ?? Expression.Label();

                var check = Expression.Label();
                var iterate = Expression.Label();

                var indexer = typeof(string).GetProperty("Chars");

                var res =
                    Expression.Block(
                        new[] { collection, index, variable },
                        Expression.Assign(collection, Collection),
                        Expression.Assign(index, Helpers.CreateConstantInt32(0)),
                        Expression.Goto(check),
                        Expression.Label(iterate),
                        Expression.Assign(variable, Expression.MakeIndex(collection, indexer, new[] { index })),
                        Body,
                        Expression.Label(@continue),
                        Expression.PostIncrementAssign(index),
                        Expression.Label(check),
                        Expression.IfThen(
                            Expression.LessThan(index, length),
                            Expression.Goto(iterate)
                        ),
                        Expression.Label(@break)
                    );

                return res;
            }
        }

        abstract class ArrayForEachCSharpStatementBase : ForEachCSharpStatement
        {
            protected ArrayForEachCSharpStatementBase(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variable, collection, body, breakLabel, continueLabel)
            {
            }

            protected abstract Expression CreateConvert(Expression element);

            protected override Expression ReduceCore()
            {
                var collection = Expression.Parameter(Collection.Type);
                var variable = Variable;

                var index = Expression.Parameter(typeof(int));
                var length = Expression.ArrayLength(collection); // NB: The C# compiler executes `ldlen` over and over; could we store it in a local?

                var @break = BreakLabel ?? Expression.Label();
                var @continue = ContinueLabel ?? Expression.Label();

                var check = Expression.Label();
                var iterate = Expression.Label();

                var res =
                    Expression.Block(
                        new[] { collection, index, variable },
                        Expression.Assign(collection, Collection),
                        Expression.Assign(index, Helpers.CreateConstantInt32(0)),
                        Expression.Goto(check),
                        Expression.Label(iterate),
                        Expression.Assign(variable, CreateConvert(Expression.ArrayIndex(collection, index))),
                        Body,
                        Expression.Label(@continue),
                        Expression.PostIncrementAssign(index),
                        Expression.Label(check),
                        Expression.IfThen(
                            Expression.LessThan(index, length),
                            Expression.Goto(iterate)
                        ),
                        Expression.Label(@break)
                    );

                return res;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Only part of the hierarchy is visible publicly.")]
        class ArrayForEachCSharpStatement : ArrayForEachCSharpStatementBase
        {
            private readonly LambdaExpression _conversion;

            public ArrayForEachCSharpStatement(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion)
                : base(variable, collection, body, breakLabel, continueLabel)
            {
                _conversion = conversion;
            }

            public override LambdaExpression Conversion
            {
                get
                {
                    return _conversion;
                }
            }

            protected override Expression CreateConvert(Expression element)
            {
                return Expression.Invoke(_conversion, element);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Only part of the hierarchy is visible publicly.")]
        class SimpleArrayForEachCSharpStatement : ArrayForEachCSharpStatementBase
        {
            public SimpleArrayForEachCSharpStatement(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
                : base(variable, collection, body, breakLabel, continueLabel)
            {
            }

            protected override Expression CreateConvert(Expression element)
            {
                return Expression.Convert(element, Variable.Type);
            }
        }

        class BoundForEachCSharpStatement : ForEachCSharpStatement
        {
            private readonly PropertyInfo _current;
            private readonly MethodInfo _getEnumerator;
            private readonly MethodInfo _moveNext;

            internal BoundForEachCSharpStatement(ParameterExpression variable, Expression collection, Expression body, LabelTarget breakLabel, LabelTarget continueLabel, LambdaExpression conversion, MethodInfo getEnumerator, MethodInfo moveNext, PropertyInfo current)
                : base(variable, collection, body, breakLabel, continueLabel)
            {
                Conversion = conversion;
                _getEnumerator = getEnumerator;
                _moveNext = moveNext;
                _current = current;
            }

            public override LambdaExpression Conversion
            {
                get;
            }

            protected override Expression ReduceCore()
            {
                var getEnumerator = Expression.Call(Collection, _getEnumerator);
                var enumeratorType = getEnumerator.Type;
                var enumeratorVariable = Expression.Parameter(enumeratorType);

                var moveNext = Expression.Call(enumeratorVariable, _moveNext);
                var current = (Expression)Expression.Property(enumeratorVariable, _current);

                if (Conversion != null)
                {
                    current = Expression.Invoke(Conversion, current);
                }

                var @break = BreakLabel ?? Expression.Label();
                var @continue = ContinueLabel ?? Expression.Label();

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

                        var dispose = enumeratorType.FindDisposeMethod();
                        cleanup = Expression.Call(enumeratorVariable, dispose);
                    }
                    else
                    {
                        var dispose = enumeratorType.FindDisposeMethod();
                        cleanup =
                            Expression.IfThen(
                                Expression.ReferenceNotEqual(enumeratorVariable, Expression.Constant(null, typeof(IDisposable))),
                                Expression.Call(enumeratorVariable, dispose)
                            );
                    }
                }
                else if (!enumeratorType.IsSealed)
                {
                    var d = Expression.Parameter(typeof(IDisposable));
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
                                Expression.Block(
                                    new[] { Variable },
                                    Expression.Assign(Variable, current),
                                    Body
                                ),
                                @break,
                                @continue
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
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body)
        {
            return ForEach(variable, collection, body, null, null, null);
        }

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break)
        {
            return ForEach(variable, collection, body, @break, null, null);
        }

        /// <summary>
        /// Creates a <see cref="ForEachCSharpStatement"/> that represents a foreach loop.
        /// </summary>
        /// <param name="variable">The iteration variable.</param>
        /// <param name="collection">The collection to iterate over.</param>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="ForEachCSharpStatement"/>.</returns>
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue)
        {
            return ForEach(variable, collection, body, @break, @continue, null);
        }

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
        public static ForEachCSharpStatement ForEach(ParameterExpression variable, Expression collection, Expression body, LabelTarget @break, LabelTarget @continue, LambdaExpression conversion)
        {
            // NB: This is the overload the C# compiler can bind to. Note, however, that a bound foreach node in Roslyn has
            //     information about GetEnumerator, MoveNext, Current, etc. as well. We can infer the same information at
            //     runtime, but could also add an overload that has all of these.
            // NB: Conversion of the collection should be inserted as a Convert node by the compiler.
            //
            // TODO: Do we need to add a means to perform an enumerator conversion? Cf. ForEachEnumeratorInfo in Roslyn.

            return ForEachCSharpStatement.Make(variable, collection, body, @break, @continue, conversion);
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
        protected internal virtual Expression VisitForEach(ForEachCSharpStatement node)
        {
            return node.Update(VisitLabelTarget(node.BreakLabel), VisitLabelTarget(node.ContinueLabel), VisitAndConvert(node.Variable, nameof(VisitForEach)), Visit(node.Collection), VisitAndConvert(node.Conversion, nameof(VisitForEach)), Visit(node.Body));
        }
    }
}
