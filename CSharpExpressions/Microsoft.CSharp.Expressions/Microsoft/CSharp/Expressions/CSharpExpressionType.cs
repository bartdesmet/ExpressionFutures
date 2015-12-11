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
        /// A node that represents a block.
        /// </summary>
        Block,
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
        /// <summary>
        /// A node that represents a for statement.
        /// </summary>
        For,
        /// <summary>
        /// A node that represents a switch statement.
        /// </summary>
        Switch,
        /// <summary>
        /// A node that represents a goto statement.
        /// </summary>
        Goto,
        /// <summary>
        /// A node that represents a dynamically bound unary operation.
        /// </summary>
        DynamicUnary,
        /// <summary>
        /// A node that represents a dynamically bound binary operation.
        /// </summary>
        DynamicBinary,
        /// <summary>
        /// A node that represents a dynamically bound member lookup.
        /// </summary>
        DynamicGetMember,
        /// <summary>
        /// A node that represents a dynamically bound indexing operation.
        /// </summary>
        DynamicGetIndex,
        /// <summary>
        /// A node that represents a dynamically bound invocation of a lambda or a delegate.
        /// </summary>
        DynamicInvoke,
        /// <summary>
        /// A node that represents a dynamically bound invocation of a member.
        /// </summary>
        DynamicInvokeMember,
        /// <summary>
        /// A node that represents a dynamically bound invocation of a constructor.
        /// </summary>
        DynamicInvokeConstructor,
        /// <summary>
        /// A node that represents a dynamically bound conversion to a static type.
        /// </summary>
        DynamicConvert,
        /// <summary>
        /// A node that represents a conditional access operation.
        /// </summary>
        ConditionalAccess,
        /// <summary>
        /// A node that represents the receiver of a conditional access operation.
        /// </summary>
        ConditionalReceiver,

        // NB: Unfortunately, we have to clone the nodes below in order to support compound assignment with indexers
        //     that have named and/or optional parameters. See Assign[Binary|Unary]CSharpExpression.cs.

        // NB: Dropped PowerAssign because C# doesn't have it.

        // NB: Added Checked variants for increment and decrement operators.

        /// <summary>
        /// An assignment operation, such as (a = b).
        /// </summary>
        Assign,
        /// <summary>
        /// An addition compound assignment operation, such as (a += b), without overflow checking, for numeric operands.
        /// </summary>
        AddAssign,
        /// <summary>
        /// A bitwise or logical AND compound assignment operation, such as (a &amp;= b).
        /// </summary>
        AndAssign,
        /// <summary>
        /// An division compound assignment operation, such as (a /= b), for numeric operands.
        /// </summary>
        DivideAssign,
        /// <summary>
        /// A bitwise or logical XOR compound assignment operation, such as (a ^= b).
        /// </summary>
        ExclusiveOrAssign,
        /// <summary>
        /// A bitwise left-shift compound assignment, such as (a &lt;&lt;= b).
        /// </summary>
        LeftShiftAssign,
        /// <summary>
        /// An arithmetic remainder compound assignment operation, such as (a %= b).
        /// </summary>
        ModuloAssign,
        /// <summary>
        /// A multiplication compound assignment operation, such as (a *= b), without overflow checking, for numeric operands.
        /// </summary>
        MultiplyAssign,
        /// <summary>
        /// A bitwise or logical OR compound assignment, such as (a |= b).
        /// </summary>
        OrAssign,
        /// <summary>
        /// A bitwise right-shift compound assignment operation, such as (a &gt;&gt;= b).
        /// </summary>
        RightShiftAssign,
        /// <summary>
        /// A subtraction compound assignment operation, such as (a -= b), without overflow checking, for numeric operands.
        /// </summary>
        SubtractAssign,
        /// <summary>
        /// An addition compound assignment operation, such as (a += b), with overflow checking, for numeric operands.
        /// </summary>
        AddAssignChecked,
        /// <summary>
        /// A multiplication compound assignment operation, such as (a *= b), that has overflow checking, for numeric operands.
        /// </summary>
        MultiplyAssignChecked,
        /// <summary>
        /// A subtraction compound assignment operation, such as (a -= b), that has overflow checking, for numeric operands.
        /// </summary>
        SubtractAssignChecked,
        /// <summary>
        /// A unary prefix increment, such as (++a). The object a should be modified in place.
        /// </summary>
        PreIncrementAssign,
        /// <summary>
        /// A unary prefix decrement, such as (--a). The object a should be modified in place.
        /// </summary>
        PreDecrementAssign,
        /// <summary>
        /// A unary postfix increment, such as (a++). The object a should be modified in place.
        /// </summary>
        PostIncrementAssign,
        /// <summary>
        /// A unary postfix decrement, such as (a--). The object a should be modified in place.
        /// </summary>
        PostDecrementAssign,
        /// <summary>
        /// A unary prefix increment, such as (++a), that has overflow checking. The object a should be modified in place.
        /// </summary>
        PreIncrementCheckedAssign,
        /// <summary>
        /// A unary prefix decrement, such as (--a), that has overflow checking. The object a should be modified in place.
        /// </summary>
        PreDecrementCheckedAssign,
        /// <summary>
        /// A unary postfix increment, such as (a++), that has overflow checking. The object a should be modified in place.
        /// </summary>
        PostIncrementCheckedAssign,
        /// <summary>
        /// A unary postfix decrement, such as (a--), that has overflow checking. The object a should be modified in place.
        /// </summary>
        PostDecrementCheckedAssign,
    }
}
