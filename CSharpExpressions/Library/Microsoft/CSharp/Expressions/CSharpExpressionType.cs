﻿// Prototyping extended expression trees for C#.
//
// bartde - October 2015

namespace Microsoft.CSharp.Expressions
{
    /// <summary>
    /// Describes the node types for the nodes of a C# expression tree.
    /// </summary>
    public enum CSharpExpressionType
    {
        /// <summary>
        /// A node that represents represents a method call.
        /// </summary>
        Call,
        /// <summary>
        /// A node that represents calling a constructor to create a new object.
        /// </summary>
        New,
        /// <summary>
        /// A node that represents applying a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        Invoke,
    }
}
