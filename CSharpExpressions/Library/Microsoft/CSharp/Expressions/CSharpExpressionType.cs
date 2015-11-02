// Prototyping extended expression trees for C#.
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
        /// <summary>
        /// A node that represents an index operation.
        /// </summary>
        Index,
        /// <summary>
        /// A node that represents creating a new multi-dimensional array and initializing it from a list of elements.
        /// </summary>
        NewMultidimensionalArrayInit,
        /// <summary>
        /// A node that represents an await operation.
        /// </summary>
        Await,
        /// <summary>
        /// A node that represents an asynchronous lambda.
        /// </summary>
        AsyncLambda,
        /// <summary>
        /// A node that represents a while loop.
        /// </summary>
        While,
        /// <summary>
        /// A node that represents a do...while loop.
        /// </summary>
        Do,
        /// <summary>
        /// A node that represents a using statement.
        /// </summary>
        Using,
        /// <summary>
        /// A node that represents a lock statement.
        /// </summary>
        Lock,
        /// <summary>
        /// A node that represents a foreach statement.
        /// </summary>
        ForEach,
    }
}
