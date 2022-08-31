// Prototyping extended expression trees for C#.
//
// bartde - February 2020

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using static System.Dynamic.Utils.ContractUtils;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents an interpolated string expression.
    /// </summary>
    public partial class InterpolatedStringCSharpExpression : CSharpExpression
    {
        private static MethodInfo s_format_params;
        private static MethodInfo[] s_format_args;

        internal InterpolatedStringCSharpExpression(ReadOnlyCollection<Interpolation> interpolations) => Interpolations = interpolations;

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />. (Inherited from <see cref="CSharpExpression" />.)
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public sealed override CSharpExpressionType CSharpNodeType => CSharpExpressionType.InterpolatedString;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type => typeof(string);

        /// <summary>
        /// Gets a collection of interpolations.
        /// </summary>
        public ReadOnlyCollection<Interpolation> Interpolations { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitInterpolatedString(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="interpolations">The <see cref="Interpolations" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InterpolatedStringCSharpExpression Update(IEnumerable<Interpolation> interpolations)
        {
            if (SameElements(ref interpolations, Interpolations))
            {
                return this;
            }

            return CSharpExpression.InterpolatedString(interpolations);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            // NB: This reduces to what the C# compiler would have emitted for expression tree lowreing in the absence of Microsoft.CSharp.Expressions.
            //     If we want to support a more optimal form, we have to take a breaking change, or move such optimizations to the separate opt-in optimizer.

            var sb = new StringBuilder();

            var values = new List<Expression>();
            int pos = 0;

            foreach (var interpolation in Interpolations)
            {
                switch (interpolation)
                {
                    case InterpolationStringInsert insert:
                        sb.Append('{').Append(pos++);

                        if (insert.Alignment != null)
                        {
                            sb.Append(',').Append(insert.Alignment.Value);
                        }

                        if (insert.Format != null)
                        {
                            sb.Append(':').Append(insert.Format);
                        }

                        sb.Append('}');

                        if (insert.Value.Type.IsValueType)
                        {
                            values.Add(Expression.Convert(insert.Value, typeof(object)));
                        }
                        else
                        {
                            values.Add(insert.Value);
                        }
                        break;
                    case InterpolationStringLiteral literal:
                        sb.Append(literal.Value.Replace("{", "{{").Replace("}", "}}"));
                        break;
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            return MakeStringFormat(sb.ToString(), values);
        }

        /// <summary>
        /// Builds an expression to call string.Format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The formatting arguments to pass.</param>
        /// <returns>An expression with a call to string.Format.</returns>
        protected virtual Expression MakeStringFormat(string format, List<Expression> args)
        {
            //
            // NB: Roslyn has strange behavior for e.g. $"foo", where it leaks optimization logic in the expression tree, causing it to produce
            //
            //       Expression.Coalesce(Expression.Constant("foo"), Expression.Constant(""))
            //
            //     We don't port that behavior here.
            //

            var formatString = Expression.Constant(format);

            var n = args.Count;

            if (n == 0)
            {
                return formatString;
            }

            EnsureStringFormatInfo();

            //
            // CONSIDER: Review the decision on https://github.com/dotnet/roslyn/issues/44168.
            //

            if (n - 1 < s_format_args.Length)
            {
                var method = s_format_args[n - 1];

                if (method != null)
                {
                    return Expression.Call(method, new Expression[] { formatString }.Concat(args));
                }
            }

            return Expression.Call(s_format_params, formatString, Expression.NewArrayInit(typeof(object), args));
        }

        private static void EnsureStringFormatInfo()
        {
            if (s_format_args == null)
            {
                var methods = new List<MethodInfo>();
                var maxArgs = 0;

                foreach (var method in typeof(string).GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (!method.IsGenericMethodDefinition && method.Name == nameof(string.Format))
                    {
                        var parameters = method.GetParametersCached();

                        if (parameters.Length > 1 && parameters[0].ParameterType == typeof(string))
                        {
                            if (parameters.Length == 2 && parameters[1].ParameterType == typeof(object[]))
                            {
                                s_format_params = method;
                            }
                            else
                            {
                                var isCandidate = true;

                                for (int i = 1; i < parameters.Length; i++)
                                {
                                    if (parameters[i].ParameterType != typeof(object))
                                    {
                                        isCandidate = false;
                                        break;
                                    }
                                }

                                if (isCandidate)
                                {
                                    methods.Add(method);

                                    var argCount = parameters.Length - 1;

                                    if (argCount > maxArgs)
                                    {
                                        maxArgs = argCount;
                                    }
                                }
                            }
                        }
                    }
                }

                s_format_args = new MethodInfo[maxArgs];

                foreach (var method in methods)
                {
                    s_format_args[method.GetParametersCached().Length - 2] = method;
                }
            }
        }
    }

    internal sealed class FormattableInterpolatedStringCSharpExpression : InterpolatedStringCSharpExpression
    {
        private static MethodInfo s_create;

        internal FormattableInterpolatedStringCSharpExpression(Type type, ReadOnlyCollection<Interpolation> interpolations)
            : base(interpolations)
        {
            Type = type;
        }

        public override Type Type { get; }

        protected override Expression MakeStringFormat(string format, List<Expression> args)
        {
            s_create ??= typeof(FormattableStringFactory).GetNonGenericMethod(nameof(FormattableStringFactory.Create), BindingFlags.Public | BindingFlags.Static, new[] { typeof(string), typeof(object[]) });

            var call = Expression.Call(s_create, Expression.Constant(format), Expression.NewArrayInit(typeof(object), args));

            if (Type != call.Type)
            {
                return Expression.Convert(call, Type);
            }

            return call;
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(params Interpolation[] interpolations) =>
            InterpolatedString((IEnumerable<Interpolation>)interpolations);

        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(IEnumerable<Interpolation> interpolations)
        {
            var interpolationsList = interpolations.ToReadOnly();

            RequiresNotNullItems(interpolationsList, nameof(interpolations));

            return new InterpolatedStringCSharpExpression(interpolationsList);
        }

        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="type">The type of the interpolated string. This can be any of <see cref="string"/>, <see cref="FormattableString"/>, or <see cref="IFormattable"/>.</param>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(Type type, params Interpolation[] interpolations) =>
            InterpolatedString(type, (IEnumerable<Interpolation>)interpolations);

        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="type">The type of the interpolated string. This can be any of <see cref="string"/>, <see cref="FormattableString"/>, or <see cref="IFormattable"/>.</param>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(Type type, IEnumerable<Interpolation> interpolations)
        {
            RequiresNotNull(type, nameof(type));

            if (type == typeof(string))
                return InterpolatedString(interpolations);

            if (type != typeof(FormattableString) && type != typeof(IFormattable))
                throw Error.InvalidInterpolatedStringType(type);

            var interpolationsList = interpolations.ToReadOnly();
            
            RequiresNotNullItems(interpolationsList, nameof(interpolations));

            return new FormattableInterpolatedStringCSharpExpression(type, interpolationsList);
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="InterpolatedStringCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitInterpolatedString(InterpolatedStringCSharpExpression node) =>
            node.Update(
                Visit(node.Interpolations, VisitInterpolation)
            );
    }
}
