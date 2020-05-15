// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    [DebuggerTypeProxy(typeof(ArrayAccessCSharpExpressionProxy))]
    partial class ArrayAccessCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ArrayAccessCSharpExpressionProxy
    {
        private readonly ArrayAccessCSharpExpression _node;

        public ArrayAccessCSharpExpressionProxy(ArrayAccessCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Array => _node.Array;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Indexes => _node.Indexes;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AssignBinaryCSharpExpressionProxy))]
    partial class AssignBinaryCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AssignBinaryCSharpExpressionProxy
    {
        private readonly AssignBinaryCSharpExpression _node;

        public AssignBinaryCSharpExpressionProxy(AssignBinaryCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.LambdaExpression FinalConversion => _node.FinalConversion;
        public System.Boolean IsLifted => _node.IsLifted;
        public System.Boolean IsLiftedToNull => _node.IsLiftedToNull;
        public System.Linq.Expressions.Expression Left => _node.Left;
        public System.Linq.Expressions.LambdaExpression LeftConversion => _node.LeftConversion;
        public System.Reflection.MethodInfo Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Right => _node.Right;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AssignBinaryDynamicCSharpExpressionProxy))]
    partial class AssignBinaryDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AssignBinaryDynamicCSharpExpressionProxy
    {
        private readonly AssignBinaryDynamicCSharpExpression _node;

        public AssignBinaryDynamicCSharpExpressionProxy(AssignBinaryDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Left => _node.Left;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.CSharpExpressionType OperationNodeType => _node.OperationNodeType;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Right => _node.Right;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AssignUnaryCSharpExpressionProxy))]
    partial class AssignUnaryCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AssignUnaryCSharpExpressionProxy
    {
        private readonly AssignUnaryCSharpExpression _node;

        public AssignUnaryCSharpExpressionProxy(AssignUnaryCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Reflection.MethodInfo Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Operand => _node.Operand;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AssignUnaryDynamicCSharpExpressionProxy))]
    partial class AssignUnaryDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AssignUnaryDynamicCSharpExpressionProxy
    {
        private readonly AssignUnaryDynamicCSharpExpression _node;

        public AssignUnaryDynamicCSharpExpressionProxy(AssignUnaryDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Operand => _node.Operand;
        public Microsoft.CSharp.Expressions.CSharpExpressionType OperationNodeType => _node.OperationNodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AsyncLambdaCSharpExpressionProxy))]
    partial class AsyncLambdaCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AsyncLambdaCSharpExpressionProxy
    {
        private readonly AsyncLambdaCSharpExpression _node;

        public AsyncLambdaCSharpExpressionProxy(AsyncLambdaCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression> Parameters => _node.Parameters;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(AwaitCSharpExpressionProxy))]
    partial class AwaitCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class AwaitCSharpExpressionProxy
    {
        private readonly AwaitCSharpExpression _node;

        public AwaitCSharpExpressionProxy(AwaitCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.Expressions.AwaitInfo Info => _node.Info;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Operand => _node.Operand;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(BinaryCSharpExpressionProxy))]
    partial class BinaryCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class BinaryCSharpExpressionProxy
    {
        private readonly BinaryCSharpExpression _node;

        public BinaryCSharpExpressionProxy(BinaryCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Left => _node.Left;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Right => _node.Right;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(BinaryDynamicCSharpExpressionProxy))]
    partial class BinaryDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class BinaryDynamicCSharpExpressionProxy
    {
        private readonly BinaryDynamicCSharpExpression _node;

        public BinaryDynamicCSharpExpressionProxy(BinaryDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Left => _node.Left;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.ExpressionType OperationNodeType => _node.OperationNodeType;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Right => _node.Right;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(BlockCSharpExpressionProxy))]
    partial class BlockCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class BlockCSharpExpressionProxy
    {
        private readonly BlockCSharpExpression _node;

        public BlockCSharpExpressionProxy(BlockCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.LabelTarget ReturnLabel => _node.ReturnLabel;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Statements => _node.Statements;
        public System.Type Type => _node.Type;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression> Variables => _node.Variables;
    }

    [DebuggerTypeProxy(typeof(ConditionalAccessCSharpExpressionProxy))]
    partial class ConditionalAccessCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalAccessCSharpExpressionProxy
    {
        private readonly ConditionalAccessCSharpExpression _node;

        public ConditionalAccessCSharpExpressionProxy(ConditionalAccessCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public System.Linq.Expressions.Expression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalArrayIndexCSharpExpressionProxy))]
    partial class ConditionalArrayIndexCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalArrayIndexCSharpExpressionProxy
    {
        private readonly ConditionalArrayIndexCSharpExpression _node;

        public ConditionalArrayIndexCSharpExpressionProxy(ConditionalArrayIndexCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Array => _node.Array;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Indexes => _node.Indexes;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public System.Linq.Expressions.IndexExpression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalIndexCSharpExpressionProxy))]
    partial class ConditionalIndexCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalIndexCSharpExpressionProxy
    {
        private readonly ConditionalIndexCSharpExpression _node;

        public ConditionalIndexCSharpExpressionProxy(ConditionalIndexCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Reflection.PropertyInfo Indexer => _node.Indexer;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public Microsoft.CSharp.Expressions.IndexCSharpExpression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalInvocationCSharpExpressionProxy))]
    partial class ConditionalInvocationCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalInvocationCSharpExpressionProxy
    {
        private readonly ConditionalInvocationCSharpExpression _node;

        public ConditionalInvocationCSharpExpressionProxy(ConditionalInvocationCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public Microsoft.CSharp.Expressions.InvocationCSharpExpression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalLoopCSharpStatementProxy))]
    partial class ConditionalLoopCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalLoopCSharpStatementProxy
    {
        private readonly ConditionalLoopCSharpStatement _node;

        public ConditionalLoopCSharpStatementProxy(ConditionalLoopCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Test => _node.Test;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(ConditionalMemberCSharpExpressionProxy))]
    partial class ConditionalMemberCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalMemberCSharpExpressionProxy
    {
        private readonly ConditionalMemberCSharpExpression _node;

        public ConditionalMemberCSharpExpressionProxy(ConditionalMemberCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Reflection.MemberInfo Member => _node.Member;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public System.Linq.Expressions.MemberExpression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalMethodCallCSharpExpressionProxy))]
    partial class ConditionalMethodCallCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalMethodCallCSharpExpressionProxy
    {
        private readonly ConditionalMethodCallCSharpExpression _node;

        public ConditionalMethodCallCSharpExpressionProxy(ConditionalMethodCallCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Reflection.MethodInfo Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.ConditionalReceiver NonNullReceiver => _node.NonNullReceiver;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Linq.Expressions.Expression Receiver => _node.Receiver;
        public System.Type Type => _node.Type;
        public Microsoft.CSharp.Expressions.MethodCallCSharpExpression WhenNotNull => _node.WhenNotNull;
    }

    [DebuggerTypeProxy(typeof(ConditionalReceiverProxy))]
    partial class ConditionalReceiver
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConditionalReceiverProxy
    {
        private readonly ConditionalReceiver _node;

        public ConditionalReceiverProxy(ConditionalReceiver node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(ConvertDynamicCSharpExpressionProxy))]
    partial class ConvertDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ConvertDynamicCSharpExpressionProxy
    {
        private readonly ConvertDynamicCSharpExpression _node;

        public ConvertDynamicCSharpExpressionProxy(ConvertDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(CSharpExpressionProxy))]
    partial class CSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class CSharpExpressionProxy
    {
        private readonly CSharpExpression _node;

        public CSharpExpressionProxy(CSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(CSharpStatementProxy))]
    partial class CSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class CSharpStatementProxy
    {
        private readonly CSharpStatement _node;

        public CSharpStatementProxy(CSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(CSharpSwitchCaseProxy))]
    partial class CSharpSwitchCase
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class CSharpSwitchCaseProxy
    {
        private readonly CSharpSwitchCase _node;

        public CSharpSwitchCaseProxy(CSharpSwitchCase node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Statements => _node.Statements;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Object> TestValues => _node.TestValues;
    }

    [DebuggerTypeProxy(typeof(DiscardCSharpExpressionProxy))]
    partial class DiscardCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class DiscardCSharpExpressionProxy
    {
        private readonly DiscardCSharpExpression _node;

        public DiscardCSharpExpressionProxy(DiscardCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(DoCSharpStatementProxy))]
    partial class DoCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class DoCSharpStatementProxy
    {
        private readonly DoCSharpStatement _node;

        public DoCSharpStatementProxy(DoCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Test => _node.Test;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(DynamicCSharpArgumentProxy))]
    partial class DynamicCSharpArgument
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class DynamicCSharpArgumentProxy
    {
        private readonly DynamicCSharpArgument _node;

        public DynamicCSharpArgumentProxy(DynamicCSharpArgument node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags Flags => _node.Flags;
        public System.String Name => _node.Name;
    }

    [DebuggerTypeProxy(typeof(DynamicCSharpExpressionProxy))]
    partial class DynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class DynamicCSharpExpressionProxy
    {
        private readonly DynamicCSharpExpression _node;

        public DynamicCSharpExpressionProxy(DynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(ForCSharpStatementProxy))]
    partial class ForCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ForCSharpStatementProxy
    {
        private readonly ForCSharpStatement _node;

        public ForCSharpStatementProxy(ForCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Initializers => _node.Initializers;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Iterators => _node.Iterators;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Test => _node.Test;
        public System.Type Type => _node.Type;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression> Variables => _node.Variables;
    }

    [DebuggerTypeProxy(typeof(ForEachCSharpStatementProxy))]
    partial class ForEachCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ForEachCSharpStatementProxy
    {
        private readonly ForEachCSharpStatement _node;

        public ForEachCSharpStatementProxy(ForEachCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.Expression Collection => _node.Collection;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public System.Linq.Expressions.LambdaExpression Conversion => _node.Conversion;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
        public System.Linq.Expressions.ParameterExpression Variable => _node.Variable;
    }

    [DebuggerTypeProxy(typeof(FromEndIndexCSharpExpressionProxy))]
    partial class FromEndIndexCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class FromEndIndexCSharpExpressionProxy
    {
        private readonly FromEndIndexCSharpExpression _node;

        public FromEndIndexCSharpExpressionProxy(FromEndIndexCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Boolean IsLifted => _node.IsLifted;
        public System.Reflection.MethodBase Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Operand => _node.Operand;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(GetIndexDynamicCSharpExpressionProxy))]
    partial class GetIndexDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GetIndexDynamicCSharpExpressionProxy
    {
        private readonly GetIndexDynamicCSharpExpression _node;

        public GetIndexDynamicCSharpExpressionProxy(GetIndexDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.DynamicCSharpArgument> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(GetMemberDynamicCSharpExpressionProxy))]
    partial class GetMemberDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GetMemberDynamicCSharpExpressionProxy
    {
        private readonly GetMemberDynamicCSharpExpression _node;

        public GetMemberDynamicCSharpExpressionProxy(GetMemberDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.DynamicCSharpArgument> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.String Name => _node.Name;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(GotoCaseCSharpStatementProxy))]
    partial class GotoCaseCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GotoCaseCSharpStatementProxy
    {
        private readonly GotoCaseCSharpStatement _node;

        public GotoCaseCSharpStatementProxy(GotoCaseCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.Expressions.CSharpGotoKind Kind => _node.Kind;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
        public System.Object Value => _node.Value;
    }

    [DebuggerTypeProxy(typeof(GotoCSharpStatementProxy))]
    partial class GotoCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GotoCSharpStatementProxy
    {
        private readonly GotoCSharpStatement _node;

        public GotoCSharpStatementProxy(GotoCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.Expressions.CSharpGotoKind Kind => _node.Kind;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(GotoDefaultCSharpStatementProxy))]
    partial class GotoDefaultCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GotoDefaultCSharpStatementProxy
    {
        private readonly GotoDefaultCSharpStatement _node;

        public GotoDefaultCSharpStatementProxy(GotoDefaultCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.Expressions.CSharpGotoKind Kind => _node.Kind;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(GotoLabelCSharpStatementProxy))]
    partial class GotoLabelCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class GotoLabelCSharpStatementProxy
    {
        private readonly GotoLabelCSharpStatement _node;

        public GotoLabelCSharpStatementProxy(GotoLabelCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.Expressions.CSharpGotoKind Kind => _node.Kind;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.LabelTarget Target => _node.Target;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(IndexCSharpExpressionProxy))]
    partial class IndexCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class IndexCSharpExpressionProxy
    {
        private readonly IndexCSharpExpression _node;

        public IndexCSharpExpressionProxy(IndexCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Reflection.PropertyInfo Indexer => _node.Indexer;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(IndexerAccessCSharpExpressionProxy))]
    partial class IndexerAccessCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class IndexerAccessCSharpExpressionProxy
    {
        private readonly IndexerAccessCSharpExpression _node;

        public IndexerAccessCSharpExpressionProxy(IndexerAccessCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Argument => _node.Argument;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Reflection.MemberInfo IndexOrSlice => _node.IndexOrSlice;
        public System.Reflection.PropertyInfo LengthOrCount => _node.LengthOrCount;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(InterpolatedStringCSharpExpressionProxy))]
    partial class InterpolatedStringCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InterpolatedStringCSharpExpressionProxy
    {
        private readonly InterpolatedStringCSharpExpression _node;

        public InterpolatedStringCSharpExpressionProxy(InterpolatedStringCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.Interpolation> Interpolations => _node.Interpolations;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(InterpolationProxy))]
    partial class Interpolation
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InterpolationProxy
    {
        private readonly Interpolation _node;

        public InterpolationProxy(Interpolation node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

    }

    [DebuggerTypeProxy(typeof(InterpolationStringInsertProxy))]
    partial class InterpolationStringInsert
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InterpolationStringInsertProxy
    {
        private readonly InterpolationStringInsert _node;

        public InterpolationStringInsertProxy(InterpolationStringInsert node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Nullable<System.Int32> Alignment => _node.Alignment;
        public System.String Format => _node.Format;
        public System.Linq.Expressions.Expression Value => _node.Value;
    }

    [DebuggerTypeProxy(typeof(InterpolationStringLiteralProxy))]
    partial class InterpolationStringLiteral
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InterpolationStringLiteralProxy
    {
        private readonly InterpolationStringLiteral _node;

        public InterpolationStringLiteralProxy(InterpolationStringLiteral node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.String Value => _node.Value;
    }

    [DebuggerTypeProxy(typeof(InvocationCSharpExpressionProxy))]
    partial class InvocationCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InvocationCSharpExpressionProxy
    {
        private readonly InvocationCSharpExpression _node;

        public InvocationCSharpExpressionProxy(InvocationCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(InvokeConstructorDynamicCSharpExpressionProxy))]
    partial class InvokeConstructorDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InvokeConstructorDynamicCSharpExpressionProxy
    {
        private readonly InvokeConstructorDynamicCSharpExpression _node;

        public InvokeConstructorDynamicCSharpExpressionProxy(InvokeConstructorDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.DynamicCSharpArgument> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(InvokeDynamicCSharpExpressionProxy))]
    partial class InvokeDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InvokeDynamicCSharpExpressionProxy
    {
        private readonly InvokeDynamicCSharpExpression _node;

        public InvokeDynamicCSharpExpressionProxy(InvokeDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.DynamicCSharpArgument> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(InvokeMemberDynamicCSharpExpressionProxy))]
    partial class InvokeMemberDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class InvokeMemberDynamicCSharpExpressionProxy
    {
        private readonly InvokeMemberDynamicCSharpExpression _node;

        public InvokeMemberDynamicCSharpExpressionProxy(InvokeMemberDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.DynamicCSharpArgument> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.String Name => _node.Name;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Target => _node.Target;
        public System.Type Type => _node.Type;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Type> TypeArguments => _node.TypeArguments;
    }

    [DebuggerTypeProxy(typeof(LockCSharpStatementProxy))]
    partial class LockCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class LockCSharpStatementProxy
    {
        private readonly LockCSharpStatement _node;

        public LockCSharpStatementProxy(LockCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(LoopCSharpStatementProxy))]
    partial class LoopCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class LoopCSharpStatementProxy
    {
        private readonly LoopCSharpStatement _node;

        public LoopCSharpStatementProxy(LoopCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(MethodCallCSharpExpressionProxy))]
    partial class MethodCallCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class MethodCallCSharpExpressionProxy
    {
        private readonly MethodCallCSharpExpression _node;

        public MethodCallCSharpExpressionProxy(MethodCallCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Reflection.MethodInfo Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Object => _node.Object;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(NewCSharpExpressionProxy))]
    partial class NewCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class NewCSharpExpressionProxy
    {
        private readonly NewCSharpExpression _node;

        public NewCSharpExpressionProxy(NewCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.ParameterAssignment> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Reflection.ConstructorInfo Constructor => _node.Constructor;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(NewMultidimensionalArrayInitCSharpExpressionProxy))]
    partial class NewMultidimensionalArrayInitCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class NewMultidimensionalArrayInitCSharpExpressionProxy
    {
        private readonly NewMultidimensionalArrayInitCSharpExpression _node;

        public NewMultidimensionalArrayInitCSharpExpressionProxy(NewMultidimensionalArrayInitCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<System.Int32> Bounds => _node.Bounds;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Expressions => _node.Expressions;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(ParameterAssignmentProxy))]
    partial class ParameterAssignment
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class ParameterAssignmentProxy
    {
        private readonly ParameterAssignment _node;

        public ParameterAssignmentProxy(ParameterAssignment node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Expression => _node.Expression;
        public System.Reflection.ParameterInfo Parameter => _node.Parameter;
    }

    [DebuggerTypeProxy(typeof(RangeCSharpExpressionProxy))]
    partial class RangeCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class RangeCSharpExpressionProxy
    {
        private readonly RangeCSharpExpression _node;

        public RangeCSharpExpressionProxy(RangeCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Boolean IsLifted => _node.IsLifted;
        public System.Linq.Expressions.Expression Left => _node.Left;
        public System.Reflection.MethodBase Method => _node.Method;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Right => _node.Right;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(SwitchCSharpStatementProxy))]
    partial class SwitchCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class SwitchCSharpStatementProxy
    {
        private readonly SwitchCSharpStatement _node;

        public SwitchCSharpStatementProxy(SwitchCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.CSharp.Expressions.CSharpSwitchCase> Cases => _node.Cases;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression SwitchValue => _node.SwitchValue;
        public System.Type Type => _node.Type;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression> Variables => _node.Variables;
    }

    [DebuggerTypeProxy(typeof(TupleConvertCSharpExpressionProxy))]
    partial class TupleConvertCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class TupleConvertCSharpExpressionProxy
    {
        private readonly TupleConvertCSharpExpression _node;

        public TupleConvertCSharpExpressionProxy(TupleConvertCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.LambdaExpression> ElementConversions => _node.ElementConversions;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Operand => _node.Operand;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(TupleLiteralCSharpExpressionProxy))]
    partial class TupleLiteralCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class TupleLiteralCSharpExpressionProxy
    {
        private readonly TupleLiteralCSharpExpression _node;

        public TupleLiteralCSharpExpressionProxy(TupleLiteralCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Collections.ObjectModel.ReadOnlyCollection<System.String> ArgumentNames => _node.ArgumentNames;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> Arguments => _node.Arguments;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(UnaryCSharpExpressionProxy))]
    partial class UnaryCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class UnaryCSharpExpressionProxy
    {
        private readonly UnaryCSharpExpression _node;

        public UnaryCSharpExpressionProxy(UnaryCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Operand => _node.Operand;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(UnaryDynamicCSharpExpressionProxy))]
    partial class UnaryDynamicCSharpExpression
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class UnaryDynamicCSharpExpressionProxy
    {
        private readonly UnaryDynamicCSharpExpression _node;

        public UnaryDynamicCSharpExpressionProxy(UnaryDynamicCSharpExpression node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Boolean CanReduce => _node.CanReduce;
        public System.Type Context => _node.Context;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags Flags => _node.Flags;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public Microsoft.CSharp.Expressions.DynamicCSharpArgument Operand => _node.Operand;
        public System.Linq.Expressions.ExpressionType OperationNodeType => _node.OperationNodeType;
        public System.Type Type => _node.Type;
    }

    [DebuggerTypeProxy(typeof(UsingCSharpStatementProxy))]
    partial class UsingCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class UsingCSharpStatementProxy
    {
        private readonly UsingCSharpStatement _node;

        public UsingCSharpStatementProxy(UsingCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Boolean CanReduce => _node.CanReduce;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Resource => _node.Resource;
        public System.Type Type => _node.Type;
        public System.Linq.Expressions.ParameterExpression Variable => _node.Variable;
    }

    [DebuggerTypeProxy(typeof(WhileCSharpStatementProxy))]
    partial class WhileCSharpStatement
    {
    }
    
    [ExcludeFromCodeCoverage]
    internal class WhileCSharpStatementProxy
    {
        private readonly WhileCSharpStatement _node;

        public WhileCSharpStatementProxy(WhileCSharpStatement node)
        {
            _node = node;
        }

        public string DebugView => _node.DebugView;

        public System.Linq.Expressions.Expression Body => _node.Body;
        public System.Linq.Expressions.LabelTarget BreakLabel => _node.BreakLabel;
        public System.Boolean CanReduce => _node.CanReduce;
        public System.Linq.Expressions.LabelTarget ContinueLabel => _node.ContinueLabel;
        public Microsoft.CSharp.Expressions.CSharpExpressionType CSharpNodeType => _node.CSharpNodeType;
        public System.Linq.Expressions.ExpressionType NodeType => _node.NodeType;
        public System.Linq.Expressions.Expression Test => _node.Test;
        public System.Type Type => _node.Type;
    }

}