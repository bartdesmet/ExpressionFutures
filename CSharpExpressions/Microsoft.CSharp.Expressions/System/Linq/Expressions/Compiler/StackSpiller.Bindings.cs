﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// NB: This is a port of the LINQ stack spiller but with a number of changes. See Spiller.cs for some design notes.
//     Changes are clearly marked with #if LINQ conditions in order to make it possible to reuse across codebases.

using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions.Compiler
{
    using Error = System.Dynamic.Utils.ErrorUtils;

    internal partial class StackSpiller
    {
        private abstract class BindingRewriter
        {
            protected MemberBinding _binding;
            protected RewriteAction _action;
            protected StackSpiller _spiller;

            internal BindingRewriter(MemberBinding binding, StackSpiller spiller)
            {
                _binding = binding;
                _spiller = spiller;
            }

            internal RewriteAction Action
            {
                get { return _action; }
            }

#if LINQ // The C# spiller never returns a Copy action, which results in copying of member bindings
            internal abstract MemberBinding AsBinding();
#endif
            internal abstract Expression AsExpression(Expression target);

            internal static BindingRewriter Create(MemberBinding binding, StackSpiller spiller, Stack stack)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        MemberAssignment assign = (MemberAssignment)binding;
                        return new MemberAssignmentRewriter(assign, spiller, stack);
                    case MemberBindingType.ListBinding:
                        MemberListBinding list = (MemberListBinding)binding;
                        return new ListBindingRewriter(list, spiller, stack);
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding member = (MemberMemberBinding)binding;
                        return new MemberMemberBindingRewriter(member, spiller, stack);
                }
                throw Error.UnhandledBinding();
            }
        }

        private class MemberMemberBindingRewriter : BindingRewriter
        {
            private readonly ReadOnlyCollection<MemberBinding> _bindings;
            private readonly BindingRewriter[] _bindingRewriters;

            internal MemberMemberBindingRewriter(MemberMemberBinding binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                _bindings = binding.Bindings;
                _bindingRewriters = new BindingRewriter[_bindings.Count];
                for (int i = 0; i < _bindings.Count; i++)
                {
                    BindingRewriter br = BindingRewriter.Create(_bindings[i], spiller, stack);
                    _action |= br.Action;
                    _bindingRewriters[i] = br;
                }
            }

#if LINQ // The C# spiller never returns a Copy action, which results in copying of member bindings
            internal override MemberBinding AsBinding()
            {
                switch (_action)
                {
                    case RewriteAction.None:
                        return _binding;
                    case RewriteAction.Copy:
                        MemberBinding[] newBindings = new MemberBinding[_bindings.Count];
                        for (int i = 0; i < _bindings.Count; i++)
                        {
                            newBindings[i] = _bindingRewriters[i].AsBinding();
                        }
                        return Expression.MemberBind(_binding.Member, new TrueReadOnlyCollection<MemberBinding>(newBindings));
                }
                throw ContractUtils.Unreachable;
            }
#endif

            internal override Expression AsExpression(Expression target)
            {
                if (target.Type.GetTypeInfo().IsValueType && _binding.Member is System.Reflection.PropertyInfo)
                {
                    throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(_binding.Member);
                }
#if LINQ
                RequireNotRefInstance(target);
#endif
                MemberExpression member = Expression.MakeMemberAccess(target, _binding.Member);
                ParameterExpression memberTemp = _spiller.MakeTemp(member.Type);

                Expression[] block = new Expression[_bindings.Count + 2];
                block[0] = Expression.Assign(memberTemp, member);

                for (int i = 0; i < _bindings.Count; i++)
                {
                    BindingRewriter br = _bindingRewriters[i];
                    block[i + 1] = br.AsExpression(memberTemp);
                }

                // We need to copy back value types
                if (memberTemp.Type.GetTypeInfo().IsValueType)
                {
                    block[_bindings.Count + 1] = Expression.Block(
                        typeof(void),
                        Expression.Assign(Expression.MakeMemberAccess(target, _binding.Member), memberTemp)
                    );
                }
                else
                {
                    block[_bindings.Count + 1] = Expression.Empty();
                }
                return MakeBlock(block);
            }
        }

        private class ListBindingRewriter : BindingRewriter
        {
            private readonly ReadOnlyCollection<ElementInit> _inits;
            private readonly ChildRewriter[] _childRewriters;

            internal ListBindingRewriter(MemberListBinding binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                _inits = binding.Initializers;

                _childRewriters = new ChildRewriter[_inits.Count];
                for (int i = 0; i < _inits.Count; i++)
                {
                    ElementInit init = _inits[i];

                    ChildRewriter cr = new(spiller, stack, init.Arguments.Count);
                    cr.Add(init.Arguments);

                    _action |= cr.Action;
                    _childRewriters[i] = cr;
                }
            }

#if LINQ // The C# spiller never returns a Copy action, which results in copying of member bindings
            internal override MemberBinding AsBinding()
            {
                switch (_action)
                {
                    case RewriteAction.None:
                        return _binding;
                    case RewriteAction.Copy:
                        ElementInit[] newInits = new ElementInit[_inits.Count];
                        for (int i = 0; i < _inits.Count; i++)
                        {
                            ChildRewriter cr = _childRewriters[i];
                            if (cr.Action == RewriteAction.None)
                            {
                                newInits[i] = _inits[i];
                            }
                            else
                            {
                                newInits[i] = Expression.ElementInit(_inits[i].AddMethod, cr[0, -1]);
                            }
                        }
                        return Expression.ListBind(_binding.Member, new TrueReadOnlyCollection<ElementInit>(newInits));
                }
                throw ContractUtils.Unreachable;
            }
#endif

            internal override Expression AsExpression(Expression target)
            {
                if (target.Type.GetTypeInfo().IsValueType && _binding.Member is System.Reflection.PropertyInfo)
                {
                    throw Error.CannotAutoInitializeValueTypeElementThroughProperty(_binding.Member);
                }
#if LINQ
                RequireNotRefInstance(target);
#endif
                MemberExpression member = Expression.MakeMemberAccess(target, _binding.Member);
                ParameterExpression memberTemp = _spiller.MakeTemp(member.Type);

                Expression[] block = new Expression[_inits.Count + 2];
                block[0] = Expression.Assign(memberTemp, member);

                for (int i = 0; i < _inits.Count; i++)
                {
                    ChildRewriter cr = _childRewriters[i];
                    Result add = cr.Finish(Expression.Call(memberTemp, _inits[i].AddMethod, cr[0, -1]));
                    block[i + 1] = add.Node;
                }

                // We need to copy back value types
                if (memberTemp.Type.GetTypeInfo().IsValueType)
                {
                    block[_inits.Count + 1] = Expression.Block(
                        typeof(void),
                        Expression.Assign(Expression.MakeMemberAccess(target, _binding.Member), memberTemp)
                    );
                }
                else
                {
                    block[_inits.Count + 1] = Expression.Empty();
                }
                return MakeBlock(block);
            }
        }

        private class MemberAssignmentRewriter : BindingRewriter
        {
            private readonly Expression _rhs;

            internal MemberAssignmentRewriter(MemberAssignment binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                Result result = spiller.RewriteExpression(binding.Expression, stack);
                _action = result.Action;
                _rhs = result.Node;
            }

#if LINQ // The C# spiller never returns a Copy action, which results in copying of member bindings
            internal override MemberBinding AsBinding()
            {
                switch (_action)
                {
                    case RewriteAction.None:
                        return _binding;
                    case RewriteAction.Copy:
                        return Expression.Bind(_binding.Member, _rhs);
                }
                throw ContractUtils.Unreachable;
            }
#endif

            internal override Expression AsExpression(Expression target)
            {
#if LINQ
                RequireNotRefInstance(target);
#endif
                MemberExpression member = Expression.MakeMemberAccess(target, _binding.Member);
                ParameterExpression memberTemp = _spiller.MakeTemp(member.Type);

                return MakeBlock(
                    Expression.Assign(memberTemp, _rhs),
                    Expression.Assign(member, memberTemp),
                    Expression.Empty()
                );
            }
        }
    }
}
