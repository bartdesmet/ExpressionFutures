// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    internal static partial class Helpers
    {
        public static void RequiresCanWrite([NotNull] Expression expression, string paramName)
        {
            ContractUtils.RequiresNotNull(expression, paramName);

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
                            if (IsArrayAssignment(call, out _))
                            {
                                return;
                            }
                        }

                        ExpressionUtils.RequiresCanWrite(expression, paramName);
                        break;
                    }
            }
        }

        private static void EnsureCanWrite(IndexCSharpExpression index, string paramName)
        {
            if (!index.Indexer.CanWrite)
            {
                throw new ArgumentException(ErrorStrings.ExpressionMustBeWriteable, paramName);
            }
        }

        private static void EnsureCanWrite(ArrayAccessCSharpExpression arrayAccess, string paramName)
        {
            if (arrayAccess.Indexes[0].Type == typeof(Range))
            {
                throw new ArgumentException(ErrorStrings.ExpressionMustBeWriteable, paramName);
            }
        }

        private static void EnsureCanWrite(IndexerAccessCSharpExpression indexerAccess, string paramName)
        {
            if (indexerAccess.Type == typeof(Range))
            {
                throw new ArgumentException(ErrorStrings.ExpressionMustBeWriteable, paramName);
            }
            else
            {
                var indexer = (PropertyInfo)indexerAccess.IndexOrSlice;

                if (!indexer.CanWrite)
                {
                    throw new ArgumentException(ErrorStrings.ExpressionMustBeWriteable, paramName);
                }
            }
        }
    }
}
