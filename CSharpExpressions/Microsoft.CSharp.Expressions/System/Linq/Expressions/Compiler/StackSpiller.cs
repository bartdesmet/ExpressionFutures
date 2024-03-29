﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// NB: This is a port of the LINQ stack spiller but with a number of changes. See Spiller.cs for some design notes.
//     Changes are clearly marked with #if LINQ conditions in order to make it possible to reuse across codebases.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;

using static System.Dynamic.Utils.ExpressionUtils;
using static System.Linq.Expressions.ExpressionExtensions;

#if LINQ
using BinaryExpressionStubs = System.Linq.Expressions.BinaryExpression;
using MemberExpressionStubs = System.Linq.Expressions.MemberExpression;
#endif

namespace System.Linq.Expressions.Compiler
{
    using Error = System.Dynamic.Utils.ErrorUtils;

    /// <summary>
    /// Expression rewriting to spill the CLR stack into temporary variables
    /// in order to guarantee some properties of code generation, for
    /// example that we always enter try block on empty stack.
    /// </summary>
    internal partial class StackSpiller
    {
        // Is the evaluation stack empty?
        private enum Stack
        {
            Empty,
            NonEmpty
        };

        // Should the parent nodes be rewritten, and in what way?
        // Designed so bitwise-or produces the correct result when merging two
        // subtrees. In particular, SpillStack is preferred over Copy which is
        // preferred over None.
        //
        // Values:
        //   None -> no rewrite needed
        //   Copy -> copy into a new node
        //   SpillStack -> spill stack into temps
        [Flags]
        private enum RewriteAction
        {
            None = 0,
            Copy = 1,
            SpillStack = 3,
        }

        // Result of a rewrite operation. Always contains an action and a node.
        private struct Result
        {
            internal readonly RewriteAction Action;
            internal readonly Expression Node;

            internal Result(RewriteAction action, Expression node)
            {
                Action = action;
                Node = node;
            }
        }

        /// <summary>
        /// The source of temporary variables
        /// </summary>
        private readonly TempMaker _tm = new();

        /// <summary>
        /// Initial stack state. Normally empty, but when inlining the lambda
        /// we might have a non-empty starting stack state.
        /// </summary>
        private readonly Stack _startingStack;

#if LINQ // NB: our compiler doesn't inline
        /// <summary>
        /// Lambda rewrite result. We need this for inlined lambdas to figure
        /// out whether we need to guarentee it an empty stack.
        /// </summary>
        private RewriteAction _lambdaRewrite;
#endif

        /// <summary>
        /// Analyzes a lambda, producing a new one that has correct invariants
        /// for codegen. In particular, it spills the IL stack to temps in
        /// places where it's invalid to have a non-empty stack (for example,
        /// entering a try statement).
        /// </summary>
        internal static LambdaExpression AnalyzeLambda(LambdaExpression lambda)
        {
            return lambda.Accept(new StackSpiller(Stack.Empty));
        }

        private StackSpiller(Stack stack)
        {
            _startingStack = stack;
        }

        // called by Expression<T>.Accept
        internal Expression<T> Rewrite<T>(Expression<T> lambda)
        {
            VerifyTemps();

            // Lambda starts with an empty stack
            Result body = RewriteExpressionFreeTemps(lambda.Body, _startingStack);
#if LINQ // NB: our compiler doesn't inline
            _lambdaRewrite = body.Action;
#endif

            VerifyTemps();

            if (body.Action != RewriteAction.None)
            {
                // Create a new scope for temps
                // (none of these will be hoisted so there is no closure impact)
                Expression newBody = body.Node;
                if (_tm.Temps.Count > 0)
                {
                    newBody = Expression.Block(_tm.Temps, newBody);
                }

                // Clone the lambda, replacing the body & variables
                return CreateExpression<T>(newBody, lambda.Name, lambda.TailCall, lambda.Parameters);
            }

            return lambda;
        }

        #region Expressions

        [Conditional("DEBUG")]
        private static void VerifyRewrite(Result result, Expression node)
        {
            Debug.Assert(result.Node != null);

            // (result.Action == RewriteAction.None) if and only if (node == result.Node)
            Debug.Assert((result.Action == RewriteAction.None) ^ (node != result.Node), "rewrite action does not match node object identity");

#if LINQ // NB: For C#, we keep await nodes. Reduction happens in a separate step.
            // if the original node is an extension node, it should have been rewritten
            Debug.Assert(result.Node.NodeType != ExpressionType.Extension, "extension nodes must be rewritten");
#endif

#if LINQ // The C# spiller never returns a Copy action
            // if we have Copy, then node type must match
            Debug.Assert(
                result.Action != RewriteAction.Copy || node.NodeType == result.Node.NodeType || node.CanReduce,
                "rewrite action does not match node object kind"
            );
#endif

            // New type must be reference assignable to the old type
            // (our rewrites preserve type exactly, but the rules for rewriting
            // an extension node are more lenient, see Expression.ReduceAndCheck())
            Debug.Assert(
                TypeUtils.AreReferenceAssignable(node.Type, result.Node.Type),
                "rewritten object must be reference assignable to the original type"
            );
        }

        private Result RewriteExpressionFreeTemps(Expression? expression, Stack stack)
        {
            int mark = Mark();
            Result result = RewriteExpression(expression, stack);
            Free(mark);
            return result;
        }

        // DynamicExpression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stack")]
        private Result RewriteDynamicExpression(Expression expr, Stack stack)
        {
            var node = (IDynamicExpression)expr;

            // CallSite is on the stack
            ChildRewriter cr = new(this, Stack.NonEmpty, node.ArgumentCount);
            cr.AddArguments(node);
            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNoRefArgs(node.DelegateType.GetMethod("Invoke")!);
#else
                MarkRefArgs(cr, node.DelegateType.GetMethod("Invoke")!, 0);
#endif
            }
            return cr.Finish(cr.Rewrite ? node.Rewrite(cr[0, -1]) : expr);
        }

        private Result RewriteIndexAssignment(BinaryExpression node, Stack stack)
        {
            IndexExpression index = (IndexExpression)node.Left;

            ChildRewriter cr = new(this, stack, 2 + index.Arguments.Count);

            cr.Add(index.Object);
            cr.Add(index.Arguments);
            cr.Add(node.Right);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNotRefInstance(index.Object);
#else
                MarkRefInstance(cr, index.Object);
#endif
            }

            if (cr.Rewrite)
            {
                node = CreateAssignBinaryExpression(
                    CreateIndexExpression(
                        cr[0],                              // Object
                        index.Indexer,
                        cr[1, -2]                           // arguments                        
                    ),
                    cr[-1]                                  // value
                );
            }

            return cr.Finish(node);
        }

        // BinaryExpression: AndAlso, OrElse
        private Result RewriteLogicalBinaryExpression(Expression expr, Stack stack)
        {
            BinaryExpression node = (BinaryExpression)expr;

            // Left expression runs on a stack as left by parent
            Result left = RewriteExpression(node.Left, stack);
            // ... and so does the right one
            Result right = RewriteExpression(node.Right, stack);
            //conversion is a lambda. stack state will be ignored. 
            Result conversion = RewriteExpression(node.Conversion, stack);

            RewriteAction action = left.Action | right.Action | conversion.Action;
            if (action != RewriteAction.None)
            {
                // We don't have to worry about byref parameters here, because the
                // factory doesn't allow it (it requires identical parameters and
                // return type from the AndAlso/OrElse method)

                expr = BinaryExpressionStubs.Create(
                    node.NodeType,
                    left.Node,
                    right.Node,
                    node.Type,
                    node.Method,
                    (LambdaExpression)conversion.Node
                );

#if !LINQ
                /*
                 * CONSIDER: Move logic to reduce logical binary nodes from the Reducer to the stack spiller.
                 * 
                switch (expr.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        expr = Microsoft.CSharp.Expressions.Compiler.Reducer.ReduceLogical((BinaryExpression)expr);
                        break;
                    case ExpressionType.Coalesce:
                        expr = Microsoft.CSharp.Expressions.Compiler.Reducer.ReduceCoalesce((BinaryExpression)expr);
                        break;
                }
                */
#endif
            }
            return new Result(action, expr);
        }

#if LINQ // NB: We reduce nodes in an earlier stage in AsyncLambda compilation, so don't need this
        private Result RewriteReducibleExpression(Expression expr, Stack stack)
        {
            Result result = RewriteExpression(expr.Reduce(), stack);
            // it's at least Copy because we reduced the node
            return new Result(result.Action | RewriteAction.Copy, result.Node);
        }
#endif

        // BinaryExpression
        private Result RewriteBinaryExpression(Expression expr, Stack stack)
        {
            BinaryExpression node = (BinaryExpression)expr;

            ChildRewriter cr = new(this, stack, 3);
            // Left expression executes on the stack as left by parent
            cr.Add(node.Left);
            // Right expression always has non-empty stack (left is on it)
            cr.Add(node.Right);
            // conversion is a lambda, stack state will be ignored
            cr.Add(node.Conversion);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNoRefArgs(node.Method);
#else
                MarkRefArgs(cr, node.Method, 0);
#endif
            }

            return cr.Finish(cr.Rewrite ?
                                    BinaryExpressionStubs.Create(
                                            node.NodeType,
                                            cr[0],
                                            cr[1],
                                            node.Type,
                                            node.Method,
                                            (LambdaExpression)cr[2]) :
                                    expr);
        }

        // variable assignment
        private Result RewriteVariableAssignment(BinaryExpression node, Stack stack)
        {
            // Expression is evaluated on a stack in current state
            Result right = RewriteExpression(node.Right, stack);
            if (right.Action != RewriteAction.None)
            {
                node = Expression.Assign(node.Left, right.Node);
            }
            return new Result(right.Action, node);
        }

        private Result RewriteAssignBinaryExpression(Expression expr, Stack stack)
        {
            var node = (BinaryExpression)expr;

            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    return RewriteIndexAssignment(node, stack);
                case ExpressionType.MemberAccess:
                    return RewriteMemberAssignment(node, stack);
                case ExpressionType.Parameter:
                    return RewriteVariableAssignment(node, stack);
#if LINQ // NB: Reducer runs before spiller, taking away all extension nodes except for Await (which is not assignable anyway).
                case ExpressionType.Extension:
                    return RewriteExtensionAssignment(node, stack);
#endif
                default:
                    throw Error.InvalidLvalue(node.Left.NodeType);
            }
        }

#if LINQ // NB: Reducer runs before spiller, taking away all extension nodes except for Await (which is not assignable anyway).
        private Result RewriteExtensionAssignment(BinaryExpression node, Stack stack)
        {
            node = Expression.Assign(node.Left.ReduceExtensions(), node.Right);
            Result result = RewriteAssignBinaryExpression(node, stack);
            // it's at least Copy because we reduced the node
            return new Result(result.Action | RewriteAction.Copy, result.Node);
        }
#endif

        // LambdaExpression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stack")]
        private static Result RewriteLambdaExpression(Expression expr, Stack stack)
        {
#if LINQ
            LambdaExpression node = (LambdaExpression)expr;

            // Call back into the rewriter
            expr = AnalyzeLambda(node);

            // If the lambda gets rewritten, we don't need to spill the stack,
            // but we do need to rebuild the tree above us so it includes the new node.
            RewriteAction action = (expr == node) ? RewriteAction.None : RewriteAction.Copy;

            return new Result(action, expr);
#else
            // NB: We're only spilling for await sites (unlike LINQ where the whole tree is analyzed recursively).
            //     Nested async lambdas will be reduced prior to spilling, so we don't have to recurse here. We
            //     don't care about nested synchronous lambdas because those don't have await sites.
            return new Result(RewriteAction.None, expr);
#endif
        }

        // ConditionalExpression
        private Result RewriteConditionalExpression(Expression expr, Stack stack)
        {
            ConditionalExpression node = (ConditionalExpression)expr;
            // Test executes at the stack as left by parent
            Result test = RewriteExpression(node.Test, stack);
            // The test is popped by conditional jump so branches execute
            // at the stack as left by parent too.
            Result ifTrue = RewriteExpression(node.IfTrue, stack);
            Result ifFalse = RewriteExpression(node.IfFalse, stack);

            RewriteAction action = test.Action | ifTrue.Action | ifFalse.Action;
            if (action != RewriteAction.None)
            {
                expr = Expression.Condition(test.Node, ifTrue.Node, ifFalse.Node, node.Type);
            }

            return new Result(action, expr);
        }

        // member assignment
        private Result RewriteMemberAssignment(BinaryExpression node, Stack stack)
        {
            MemberExpression lvalue = (MemberExpression)node.Left;

            ChildRewriter cr = new(this, stack, 2);

            // If there's an instance, it executes on the stack in current state
            // and rest is executed on non-empty stack.
            // Otherwise the stack is left unchanged.
            cr.Add(lvalue.Expression);

            cr.Add(node.Right);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNotRefInstance(lvalue.Expression);
#else
                MarkRefInstance(cr, lvalue.Expression);
#endif
            }

            if (cr.Rewrite)
            {
                return cr.Finish(
                    CreateAssignBinaryExpression(
                        MemberExpressionStubs.Make(cr[0], lvalue.Member),
                        cr[1]
                    )
                );
            }
            return new Result(RewriteAction.None, node);
        }

        // MemberExpression
#if LINQ
        private Result RewriteMemberExpression(Expression expr, Stack stack)
        {
            MemberExpression node = (MemberExpression)expr;

            // Expression is emitted on top of the stack in current state
            Result expression = RewriteExpression(node.Expression, stack);
            if (expression.Action != RewriteAction.None)
            {
                if (expression.Action == RewriteAction.SpillStack &&
                    node.Member is PropertyInfo)
                {
                    // Only need to validate propreties because reading a field
                    // is always side-effect free.
                    RequireNotRefInstance(node.Expression);
                }
                expr = MemberExpressionStubs.Make(expression.Node, node.Member);
            }
            return new Result(expression.Action, expr);
        }
#else
        private Result RewriteMemberExpression(Expression expr, Stack stack)
        {
            MemberExpression node = (MemberExpression)expr;

            ChildRewriter cr = new(this, stack, 1);

            cr.Add(node.Expression);

            if (cr.Rewrite)
            {
                if (cr.Action == RewriteAction.SpillStack && node.Member is PropertyInfo)
                {
                    // Only need to validate propreties because reading a field
                    // is always side-effect free.
#if LINQ
                    RequireNotRefInstance(node.Expression);
#else
                    MarkRefInstance(cr, node.Expression);
#endif
                }

                expr = MemberExpressionStubs.Make(cr[0], node.Member);
            }

            return cr.Finish(expr);
        }
#endif

        //RewriteIndexExpression
        private Result RewriteIndexExpression(Expression expr, Stack stack)
        {
            IndexExpression node = (IndexExpression)expr;

            ChildRewriter cr = new(this, stack, node.Arguments.Count + 1);

            // For instance methods, the instance executes on the
            // stack as is, but stays on the stack, making it non-empty.
            cr.Add(node.Object);
            cr.Add(node.Arguments);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNotRefInstance(node.Object);
#else
                MarkRefInstance(cr, node.Object);
#endif
            }

            if (cr.Rewrite)
            {
                expr = CreateIndexExpression(
                    cr[0],
                    node.Indexer,
                    cr[1, -1]
                );
            }

            return cr.Finish(expr);
        }

        // MethodCallExpression
        private Result RewriteMethodCallExpression(Expression expr, Stack stack)
        {
            MethodCallExpression node = (MethodCallExpression)expr;

            ChildRewriter cr = new(this, stack, node.Arguments.Count + 1);

            // For instance methods, the instance executes on the
            // stack as is, but stays on the stack, making it non-empty.
            cr.Add(node.Object);

            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNotRefInstance(node.Object);
                RequireNoRefArgs(node.Method);
#else
                MarkRefInstance(cr, node.Object);
                MarkRefArgs(cr, node.Method, 1);
#endif
            }

            return cr.Finish(cr.Rewrite ? node.Update(cr[0], cr[1, -1]) : expr);
        }

        // NewArrayExpression
        private Result RewriteNewArrayExpression(Expression expr, Stack stack)
        {
            NewArrayExpression node = (NewArrayExpression)expr;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                // In a case of array construction with element initialization
                // the element expressions are never emitted on an empty stack because
                // the array reference and the index are on the stack.
                stack = Stack.NonEmpty;
            }
            else
            {
                // In a case of NewArrayBounds we make no modifications to the stack 
                // before emitting bounds expressions.
            }

            ChildRewriter cr = new(this, stack, node.Expressions.Count);
            cr.Add(node.Expressions);

            if (cr.Rewrite)
            {
                Type element = node.Type.GetElementType()!;
                if (node.NodeType == ExpressionType.NewArrayInit)
                {
                    expr = Expression.NewArrayInit(element, cr[0, -1]);
                }
                else
                {
                    expr = Expression.NewArrayBounds(element, cr[0, -1]);
                }
            }

            return cr.Finish(expr);
        }

        // InvocationExpression
        private Result RewriteInvocationExpression(Expression expr, Stack stack)
        {
            InvocationExpression node = (InvocationExpression)expr;

            ChildRewriter cr;

#if LINQ // NB: Our compiler doesn't inline; this could still happen at a later stage in the LINQ compiler after lowering async lambdas to sync ones.
            // See if the lambda will be inlined
            LambdaExpression lambda = node.LambdaOperand();
            if (lambda != null)
            {
                // Arguments execute on current stack
                cr = new ChildRewriter(this, stack, node.Arguments.Count);
                cr.Add(node.Arguments);

                if (cr.Action == RewriteAction.SpillStack)
                {
                    RequireNoRefArgs(GetInvokeMethod(node.Expression));
                }

                // Lambda body also executes on current stack 
                var spiller = new StackSpiller(stack);
                lambda = lambda.Accept(spiller);

                if (cr.Rewrite || spiller._lambdaRewrite != RewriteAction.None)
                {
                    node = node.Rewrite(lambda, cr[0, -1]);
                }

                Result result = cr.Finish(node);
                return new Result(result.Action | spiller._lambdaRewrite, result.Node);
            }
#endif

            cr = new ChildRewriter(this, stack, node.Arguments.Count + 1);

            // first argument starts on stack as provided
            cr.Add(node.Expression);

            // rest of arguments have non-empty stack (delegate instance on the stack)
            cr.Add(node.Arguments);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNoRefArgs(GetInvokeMethod(node.Expression));
#else
                MarkRefArgs(cr, GetInvokeMethod(node.Expression), 1);
#endif
            }

            return cr.Finish(cr.Rewrite ? node.Update(cr[0], cr[1, -1]) : expr);
        }

        // NewExpression
        private Result RewriteNewExpression(Expression expr, Stack stack)
        {
            NewExpression node = (NewExpression)expr;

            // The first expression starts on a stack as provided by parent,
            // rest are definitely non-empty (which ChildRewriter guarantees)
            ChildRewriter cr = new(this, stack, node.Arguments.Count);
            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNoRefArgs(node.Constructor);
#else
                MarkRefArgs(cr, node.Constructor, 0);
#endif
            }

            // NB: If rewrite occurs, there's at least one argument, which rules out
            //     the case where Constructor can be null (i.e. New for a struct's
            //     default constructor).
            return cr.Finish(cr.Rewrite ? CreateNewExpression(node.Constructor!, cr[0, -1], node.Members) : expr);
        }

        // TypeBinaryExpression
#if LINQ
        private Result RewriteTypeBinaryExpression(Expression expr, Stack stack)
        {
            TypeBinaryExpression node = (TypeBinaryExpression)expr;
            // The expression is emitted on top of current stack
            Result expression = RewriteExpression(node.Expression, stack);
            if (expression.Action != RewriteAction.None)
            {
                if (node.NodeType == ExpressionType.TypeIs)
                {
                    expr = Expression.TypeIs(expression.Node, node.TypeOperand);
                }
                else
                {
                    expr = Expression.TypeEqual(expression.Node, node.TypeOperand);
                }
            }
            return new Result(expression.Action, expr);
        }
#else
        private Result RewriteTypeBinaryExpression(Expression expr, Stack stack)
        {
            TypeBinaryExpression node = (TypeBinaryExpression)expr;

            ChildRewriter cr = new(this, stack, 1);

            cr.Add(node.Expression);

            if (cr.Rewrite)
            {
                if (node.NodeType == ExpressionType.TypeIs)
                {
                    expr = Expression.TypeIs(cr[0], node.TypeOperand);
                }
                else
                {
                    expr = Expression.TypeEqual(cr[0], node.TypeOperand);
                }
            }
            return cr.Finish(expr);
        }
#endif

        // Throw
        private Result RewriteThrowUnaryExpression(Expression expr, Stack stack)
        {
            UnaryExpression node = (UnaryExpression)expr;

            // Throw statement itself does not care about the stack
            // but it will empty the stack and it may cause stack misbalance
            // it so we need to restore stack after unconditional throw to make JIT happy
            // this has an effect of executing Throw on an empty stack.

            Result value = RewriteExpressionFreeTemps(node.Operand, Stack.Empty);
            RewriteAction action = value.Action;

            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = Expression.Throw(value.Node, node.Type);
            }

            return new Result(action, expr);
        }

        // UnaryExpression
#if LINQ
        private Result RewriteUnaryExpression(Expression expr, Stack stack)
        {
            UnaryExpression node = (UnaryExpression)expr;

            Debug.Assert(node.NodeType != ExpressionType.Quote, "unexpected Quote");
            Debug.Assert(node.NodeType != ExpressionType.Throw, "unexpected Throw");

            // Operand is emitted on top of the stack as is
            Result expression = RewriteExpression(node.Operand, stack);

            if (expression.Action == RewriteAction.SpillStack)
            {
                RequireNoRefArgs(node.Method);
            }

            if (expression.Action != RewriteAction.None)
            {
                expr = CreateUnaryExpression(node.NodeType, expression.Node, node.Type, node.Method);
            }
            return new Result(expression.Action, expr);
        }
#else
        private Result RewriteUnaryExpression(Expression expr, Stack stack)
        {
            UnaryExpression node = (UnaryExpression)expr;

            Debug.Assert(node.NodeType != ExpressionType.Quote, "unexpected Quote");
            Debug.Assert(node.NodeType != ExpressionType.Throw, "unexpected Throw");

            ChildRewriter cr = new(this, stack, 1);

            cr.Add(node.Operand);

            if (cr.Action == RewriteAction.SpillStack)
            {
#if LINQ
                RequireNoRefArgs(node.Method);
#else
                MarkRefArgs(cr, node.Method, 0);
#endif
            }

            return cr.Finish(cr.Rewrite ? CreateUnaryExpression(node.NodeType, cr[0], node.Type, node.Method) : expr);
        }
#endif

        // RewriteListInitExpression
        private Result RewriteListInitExpression(Expression expr, Stack stack)
        {
            ListInitExpression node = (ListInitExpression)expr;

            //ctor runs on initial stack
            Result newResult = RewriteExpression(node.NewExpression, stack);
            Expression rewrittenNew = newResult.Node;
            RewriteAction action = newResult.Action;

            ReadOnlyCollection<ElementInit> inits = node.Initializers;

            ChildRewriter[] cloneCrs = new ChildRewriter[inits.Count];

            for (int i = 0; i < inits.Count; i++)
            {
                ElementInit init = inits[i];

                //initializers all run on nonempty stack
                ChildRewriter cr = new(this, Stack.NonEmpty, init.Arguments.Count);
                cr.Add(init.Arguments);

                action |= cr.Action;
                cloneCrs[i] = cr;
            }

            switch (action)
            {
                case RewriteAction.None:
                    break;
#if LINQ // The C# spiller never returns a Copy action
                case RewriteAction.Copy:
                    ElementInit[] newInits = new ElementInit[inits.Count];
                    for (int i = 0; i < inits.Count; i++)
                    {
                        ChildRewriter cr = cloneCrs[i];
                        if (cr.Action == RewriteAction.None)
                        {
                            newInits[i] = inits[i];
                        }
                        else
                        {
                            newInits[i] = Expression.ElementInit(inits[i].AddMethod, cr[0, -1]);
                        }
                    }
                    expr = Expression.ListInit((NewExpression)rewrittenNew, new TrueReadOnlyCollection<ElementInit>(newInits));
                    break;
#endif
                case RewriteAction.SpillStack:
#if LINQ
                    RequireNotRefInstance(node.NewExpression);
#endif
                    ParameterExpression tempNew = MakeTemp(rewrittenNew.Type);
                    Expression[] comma = new Expression[inits.Count + 2];
                    comma[0] = Expression.Assign(tempNew, rewrittenNew);

                    for (int i = 0; i < inits.Count; i++)
                    {
                        ChildRewriter cr = cloneCrs[i];
                        Result add = cr.Finish(Expression.Call(tempNew, inits[i].AddMethod, cr[0, -1]));
                        comma[i + 1] = add.Node;
                    }
                    comma[inits.Count + 1] = tempNew;
                    expr = MakeBlock(comma);
                    break;
                default:
                    throw ContractUtils.Unreachable;
            }

            return new Result(action, expr);
        }

        // RewriteMemberInitExpression
        private Result RewriteMemberInitExpression(Expression expr, Stack stack)
        {
            MemberInitExpression node = (MemberInitExpression)expr;

            //ctor runs on original stack
            Result result = RewriteExpression(node.NewExpression, stack);
            Expression rewrittenNew = result.Node;
            RewriteAction action = result.Action;

            ReadOnlyCollection<MemberBinding> bindings = node.Bindings;
            BindingRewriter[] bindingRewriters = new BindingRewriter[bindings.Count];
            for (int i = 0; i < bindings.Count; i++)
            {
                MemberBinding binding = bindings[i];
                //bindings run on nonempty stack
                BindingRewriter rewriter = BindingRewriter.Create(binding, this, Stack.NonEmpty);
                bindingRewriters[i] = rewriter;
                action |= rewriter.Action;
            }

            switch (action)
            {
                case RewriteAction.None:
                    break;
#if LINQ // The C# spiller never returns a Copy action
                case RewriteAction.Copy:
                    MemberBinding[] newBindings = new MemberBinding[bindings.Count];
                    for (int i = 0; i < bindings.Count; i++)
                    {
                        newBindings[i] = bindingRewriters[i].AsBinding();
                    }
                    expr = Expression.MemberInit((NewExpression)rewrittenNew, new TrueReadOnlyCollection<MemberBinding>(newBindings));
                    break;
#endif
                case RewriteAction.SpillStack:
#if LINQ
                    RequireNotRefInstance(node.NewExpression);
#endif
                    ParameterExpression tempNew = MakeTemp(rewrittenNew.Type);
                    Expression[] comma = new Expression[bindings.Count + 2];
                    comma[0] = Expression.Assign(tempNew, rewrittenNew);
                    for (int i = 0; i < bindings.Count; i++)
                    {
                        BindingRewriter cr = bindingRewriters[i];
                        Expression initExpr = cr.AsExpression(tempNew);
                        comma[i + 1] = initExpr;
                    }
                    comma[bindings.Count + 1] = tempNew;
                    expr = MakeBlock(comma);
                    break;
                default:
                    throw ContractUtils.Unreachable;
            }
            return new Result(action, expr);
        }

        #endregion

        #region Statements

        // Block
        private Result RewriteBlockExpression(Expression expr, Stack stack)
        {
            BlockExpression node = (BlockExpression)expr;

            int count = node.Expressions.Count;
            RewriteAction action = RewriteAction.None;
            Expression[]? clone = null;
            for (int i = 0; i < count; i++)
            {
                Expression expression = node.Expressions[i];
                // All statements within the block execute at the
                // same stack state.
                Result rewritten = RewriteExpression(expression, stack);
                action |= rewritten.Action;

                if (clone == null && rewritten.Action != RewriteAction.None)
                {
                    clone = Clone(node.Expressions, i);
                }

                if (clone != null)
                {
                    clone[i] = rewritten.Node;
                }
            }

            if (action != RewriteAction.None)
            {
                // okay to wrap since we know no one can mutate the clone array
                expr = node.Update(node.Variables, clone!);
            }
            return new Result(action, expr);
        }

        // LabelExpression
        private Result RewriteLabelExpression(Expression expr, Stack stack)
        {
            LabelExpression node = (LabelExpression)expr;

            Result expression = RewriteExpression(node.DefaultValue, stack);
            if (expression.Action != RewriteAction.None)
            {
                expr = Expression.Label(node.Target, expression.Node);
            }
            return new Result(expression.Action, expr);
        }

        // LoopStatement
        private Result RewriteLoopExpression(Expression expr, Stack stack)
        {
            LoopExpression node = (LoopExpression)expr;

            // The loop statement requires empty stack for itself, so it
            // can guarantee it to the child nodes.
            Result body = RewriteExpression(node.Body, Stack.Empty);

            RewriteAction action = body.Action;

            // However, the loop itself requires that it executes on an empty stack
            // so we need to rewrite if the stack is not empty.
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = CreateLoopExpression(body.Node, node.BreakLabel, node.ContinueLabel);
            }
            return new Result(action, expr);
        }

        // GotoExpression
        // Note: goto does not necessarily need an empty stack. We could always
        // emit it as a "leave" which would clear the stack for us. That would
        // prevent us from doing certain optimizations we might want to do,
        // however, like the switch-case-goto pattern. For now, be conservative
        private Result RewriteGotoExpression(Expression expr, Stack stack)
        {
            GotoExpression node = (GotoExpression)expr;

            // Goto requires empty stack to execute so the expression is
            // going to execute on an empty stack.
            Result value = RewriteExpressionFreeTemps(node.Value, Stack.Empty);

            // However, the statement itself needs an empty stack for itself
            // so if stack is not empty, rewrite to empty the stack.
            RewriteAction action = value.Action;
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = Expression.MakeGoto(node.Kind, node.Target, value.Node, node.Type);
            }
            return new Result(action, expr);
        }

        // SwitchStatement
        private Result RewriteSwitchExpression(Expression expr, Stack stack)
        {
            SwitchExpression node = (SwitchExpression)expr;

            // The switch statement test is emitted on the stack in current state
            Result switchValue = RewriteExpressionFreeTemps(node.SwitchValue, stack);

            RewriteAction action = switchValue.Action;
            ReadOnlyCollection<SwitchCase> cases = node.Cases;
            SwitchCase[]? clone = null;
            for (int i = 0; i < cases.Count; i++)
            {
                SwitchCase @case = cases[i];

                Expression[]? cloneTests = null;
                ReadOnlyCollection<Expression> testValues = @case.TestValues;
                for (int j = 0; j < testValues.Count; j++)
                {
                    // All tests execute at the same stack state as the switch.
                    // This is guarenteed by the compiler (to simplify spilling)
                    Result test = RewriteExpression(testValues[j], stack);
                    action |= test.Action;

                    if (cloneTests == null && test.Action != RewriteAction.None)
                    {
                        cloneTests = Clone(testValues, j);
                    }

                    if (cloneTests != null)
                    {
                        cloneTests[j] = test.Node;
                    }
                }

                // And all the cases also run on the same stack level.
                Result body = RewriteExpression(@case.Body, stack);
                action |= body.Action;

                if (body.Action != RewriteAction.None || cloneTests != null)
                {
                    if (cloneTests != null)
                    {
                        testValues = new ReadOnlyCollection<Expression>(cloneTests);
                    }
                    @case = CreateSwitchCase(body.Node, testValues);

                    clone ??= Clone(cases, i);
                }

                if (clone != null)
                {
                    clone[i] = @case;
                }
            }

            // default body also runs on initial stack
            Result defaultBody = RewriteExpression(node.DefaultBody, stack);
            action |= defaultBody.Action;

            if (action != RewriteAction.None)
            {
                if (clone != null)
                {
                    // okay to wrap because we aren't modifying the array
                    cases = new ReadOnlyCollection<SwitchCase>(clone);
                }

                expr = CreateSwitchExpression(node.Type, switchValue.Node, defaultBody.Node, node.Comparison, cases);
            }

            return new Result(action, expr);
        }

        // TryStatement
        private Result RewriteTryExpression(Expression expr, Stack stack)
        {
            TryExpression node = (TryExpression)expr;

            // Try statement definitely needs an empty stack so its
            // child nodes execute at empty stack.
            Result body = RewriteExpression(node.Body, Stack.Empty);
            ReadOnlyCollection<CatchBlock> handlers = node.Handlers;
            CatchBlock[]? clone = null;

            RewriteAction action = body.Action;
            if (handlers != null)
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    RewriteAction curAction = body.Action;

                    CatchBlock handler = handlers[i];

                    Expression? filter = handler.Filter;
                    if (handler.Filter != null)
                    {
                        // our code gen saves the incoming filter value and provides it as a variable so the stack is empty
                        Result rfault = RewriteExpression(handler.Filter, Stack.Empty);
                        action |= rfault.Action;
                        curAction |= rfault.Action;
                        filter = rfault.Node;
                    }

                    // Catch block starts with an empty stack (guaranteed by TryStatement)
                    Result rbody = RewriteExpression(handler.Body, Stack.Empty);
                    action |= rbody.Action;
                    curAction |= rbody.Action;

                    if (curAction != RewriteAction.None)
                    {
                        handler = Expression.MakeCatchBlock(handler.Test, handler.Variable, rbody.Node, filter);

                        clone ??= Clone(handlers, i);
                    }

                    if (clone != null)
                    {
                        clone[i] = handler;
                    }
                }
            }

            Result fault = RewriteExpression(node.Fault, Stack.Empty);
            action |= fault.Action;

            Result @finally = RewriteExpression(node.Finally, Stack.Empty);
            action |= @finally.Action;

            // If the stack is initially not empty, rewrite to spill the stack
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                if (clone != null)
                {
                    // okay to wrap because we aren't modifying the array
                    handlers = new ReadOnlyCollection<CatchBlock>(clone);
                }

                expr = CreateTryExpression(node.Type, body.Node, @finally.Node, fault.Node, handlers);
            }
            return new Result(action, expr);
        }

        private Result RewriteExtensionExpression(Expression expr, Stack stack)
        {
#if !LINQ
            if (expr is Microsoft.CSharp.Expressions.AwaitCSharpExpression)
            {
                return RewriteAwaitExpression(expr, stack);
            }

            // NB: Reducer runs before the spiller, taking away all extension nodes except for Await.
            throw ContractUtils.Unreachable;
#else
            Result result = RewriteExpression(expr.ReduceExtensions(), stack);
            // it's at least Copy because we reduced the node
            return new Result(result.Action | RewriteAction.Copy, result.Node);
#endif
        }

        #endregion

        #region Cloning

        /// <summary>
        /// Will clone an IList into an array of the same size, and copy
        /// all vaues up to (and NOT including) the max index
        /// </summary>
        /// <returns>The cloned array.</returns>
        private static T[] Clone<T>(ReadOnlyCollection<T> original, int max)
        {
            Debug.Assert(original != null);
            Debug.Assert(max < original.Count);

            T[] clone = new T[original.Count];
            for (int j = 0; j < max; j++)
            {
                clone[j] = original[j];
            }
            return clone;
        }

        #endregion

#if LINQ
        /// <summary>
        /// If we are spilling, requires that there are no byref arguments to
        /// the method call.
        /// 
        /// Used for:
        ///   NewExpression,
        ///   MethodCallExpression,
        ///   InvocationExpression,
        ///   DynamicExpression,
        ///   UnaryExpression,
        ///   BinaryExpression.
        /// </summary>
        /// <remarks>
        /// We could support this if spilling happened later in the compiler.
        /// Other expressions that can emit calls with arguments (such as
        /// ListInitExpression and IndexExpression) don't allow byref arguments.
        /// </remarks>
        private static void RequireNoRefArgs(MethodBase method)
        {
            if (method != null && method.GetParametersCached().Any(p => p.ParameterType.IsByRef))
            {
                throw Error.TryNotSupportedForMethodsWithRefArgs(method);
            }
        }

        /// <summary>
        /// Requires that the instance is not a value type (primitive types are
        /// okay because they're immutable).
        /// 
        /// Used for:
        ///  MethodCallExpression,
        ///  MemberExpression (for properties),
        ///  IndexExpression,
        ///  ListInitExpression,
        ///  MemberInitExpression,
        ///  assign to MemberExpression,
        ///  assign to IndexExpression.
        /// </summary>
        /// <remarks>
        /// We could support this if spilling happened later in the compiler.
        /// </remarks>
        private static void RequireNotRefInstance(Expression instance)
        {
            // Primitive value types are okay because they are all readonly,
            // but we can't rely on this for non-primitive types. So we throw
            // NotSupported.
            if (instance != null && instance.Type.GetTypeInfo().IsValueType && instance.Type.GetTypeCode() == TypeCode.Object)
            {
                throw Error.TryNotSupportedForValueTypeInstances(instance.Type);
            }
        }
#else
        private static void MarkRefArgs(ChildRewriter cr, MethodBase? method, int firstIndex)
        {
            if (method != null)
            {
                var parameters = method.GetParametersCached();
                for (int i = 0, j = firstIndex; i < parameters.Length; i++, j++)
                {
                    var parameter = parameters[i];
                    if (parameter.ParameterType.IsByRef)
                    {
                        cr.MarkByRef(j);
                    }
                }
            }
        }

        private static void MarkRefInstance(ChildRewriter cr, Expression? instance)
        {
            // Primitive value types are okay because they are all readonly,
            // but we can't rely on this for non-primitive types. For those
            // we have to spill the by ref local.
            if (instance != null && instance.Type.GetTypeInfo().IsValueType && instance.Type.GetTypeCode() == TypeCode.Object)
            {
                cr.MarkByRef(0);
            }
        }
#endif
    }
}
