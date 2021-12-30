// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Provides binding information for foreach operations.
    /// </summary>
    public sealed class EnumeratorInfo
    {
    }

    partial class CSharpExpression
    {
        public static EnumeratorInfo EnumeratorInfo(
            bool isAsync,
            Type collectionType,
            LambdaExpression getEnumerator,
            LambdaExpression moveNext,
            MethodInfo currentPropertyGetter,
            LambdaExpression currentConversion,
            Type elementType,
            bool needsDisposal,
            AwaitInfo disposeAwaitInfo,
            LambdaExpression patternDispose)
        {
            return new EnumeratorInfo();
        }
    }
}
