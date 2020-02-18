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
using System.Text;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Represents an interpolated string expression.
    /// </summary>
    public sealed partial class InterpolatedStringCSharpExpression : CSharpExpression
    {
        private static MethodInfo s_format_params;
        private static MethodInfo[] s_format_args;

        internal InterpolatedStringCSharpExpression(ReadOnlyCollection<Interpolation> interpolations)
        {
            Interpolations = interpolations;
        }

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
        protected internal override Expression Accept(CSharpExpressionVisitor visitor)
        {
            return visitor.VisitInterpolatedString(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="interpolations">The <see cref="Interpolations" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InterpolatedStringCSharpExpression Update(IEnumerable<Interpolation> interpolations)
        {
            if (interpolations == Interpolations)
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
                        sb.Append(literal.Value);
                        break;
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            return MakeStringFormat(sb.ToString(), values);
        }

        private static Expression MakeStringFormat(string format, List<Expression> args)
        {
            //
            // NB: Roslyn has strange behavior for e.g. $"foo", where it leaks optimization logic in the expression tree, causing it to produce
            //
            //       Expression.Coalesce(Expression.Constant("foo"), Expression.Constant(""))
            //
            //     We don't port that behavior here.
            //

            var n = args.Count;

            if (n == 0)
            {
                return Expression.Constant(format);
            }

            EnsureStringFormatInfo();

            var formatString = Expression.Constant(format);

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
                    if (!method.IsGenericMethodDefinition && method.Name == "Format")
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

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(params Interpolation[] interpolations)
        {
            return InterpolatedString((IEnumerable<Interpolation>)interpolations);
        }

        /// <summary>
        /// Creates a <see cref="InterpolatedStringCSharpExpression"/> that represents an interpolated string expression.
        /// </summary>
        /// <param name="interpolations">The interpolations that make up the interpolated string.</param>
        /// <returns>An instance of the <see cref="InterpolatedStringCSharpExpression"/>.</returns>
        public static InterpolatedStringCSharpExpression InterpolatedString(IEnumerable<Interpolation> interpolations)
        {
            return new InterpolatedStringCSharpExpression(interpolations.ToReadOnly());
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
        protected internal virtual Expression VisitInterpolatedString(InterpolatedStringCSharpExpression node)
        {
            return node.Update(Visit(node.Interpolations, VisitInterpolation));
        }
    }
}
