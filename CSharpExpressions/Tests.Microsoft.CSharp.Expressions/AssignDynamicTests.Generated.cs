// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    partial class DynamicTests
    {
        [Fact]
        public void Dynamic_BinaryAssign_GeneratedFactories()
        {
            var le = Expression.Parameter(typeof(object));
            var re = Expression.Constant(2);

            var ld = DynamicCSharpExpression.DynamicArgument(le);
            var rd = DynamicCSharpExpression.DynamicArgument(re);

            var be0 = DynamicCSharpExpression.DynamicAssign(le, re);
            Assert.Equal(CSharpExpressionType.Assign, be0.OperationNodeType);
            Assert.Same(le, be0.Left.Expression);
            Assert.Same(re, be0.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be0.Flags);

            var bd0 = DynamicCSharpExpression.DynamicAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.Assign, bd0.OperationNodeType);
            Assert.Same(ld, bd0.Left);
            Assert.Same(rd, bd0.Right);
            Assert.Equal(CSharpBinderFlags.None, bd0.Flags);

            var bf0 = DynamicCSharpExpression.DynamicAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.Assign, bf0.OperationNodeType);
            Assert.Same(ld, bf0.Left);
            Assert.Same(rd, bf0.Right);
            Assert.Equal(CSharpBinderFlags.None, bf0.Flags);

            var bc0 = DynamicCSharpExpression.DynamicAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.Assign, bc0.OperationNodeType);
            Assert.Same(ld, bc0.Left);
            Assert.Same(rd, bc0.Right);
            Assert.Equal(CSharpBinderFlags.None, bc0.Flags);
            Assert.Equal(typeof(int), bc0.Context);

            var be1 = DynamicCSharpExpression.DynamicAddAssign(le, re);
            Assert.Equal(CSharpExpressionType.AddAssign, be1.OperationNodeType);
            Assert.Same(le, be1.Left.Expression);
            Assert.Same(re, be1.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be1.Flags);

            var bd1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.AddAssign, bd1.OperationNodeType);
            Assert.Same(ld, bd1.Left);
            Assert.Same(rd, bd1.Right);
            Assert.Equal(CSharpBinderFlags.None, bd1.Flags);

            var bf1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.AddAssign, bf1.OperationNodeType);
            Assert.Same(ld, bf1.Left);
            Assert.Same(rd, bf1.Right);
            Assert.Equal(CSharpBinderFlags.None, bf1.Flags);

            var bc1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.AddAssign, bc1.OperationNodeType);
            Assert.Same(ld, bc1.Left);
            Assert.Same(rd, bc1.Right);
            Assert.Equal(CSharpBinderFlags.None, bc1.Flags);
            Assert.Equal(typeof(int), bc1.Context);

            var be2 = DynamicCSharpExpression.DynamicAndAssign(le, re);
            Assert.Equal(CSharpExpressionType.AndAssign, be2.OperationNodeType);
            Assert.Same(le, be2.Left.Expression);
            Assert.Same(re, be2.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be2.Flags);

            var bd2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.AndAssign, bd2.OperationNodeType);
            Assert.Same(ld, bd2.Left);
            Assert.Same(rd, bd2.Right);
            Assert.Equal(CSharpBinderFlags.None, bd2.Flags);

            var bf2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.AndAssign, bf2.OperationNodeType);
            Assert.Same(ld, bf2.Left);
            Assert.Same(rd, bf2.Right);
            Assert.Equal(CSharpBinderFlags.None, bf2.Flags);

            var bc2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.AndAssign, bc2.OperationNodeType);
            Assert.Same(ld, bc2.Left);
            Assert.Same(rd, bc2.Right);
            Assert.Equal(CSharpBinderFlags.None, bc2.Flags);
            Assert.Equal(typeof(int), bc2.Context);

            var be3 = DynamicCSharpExpression.DynamicDivideAssign(le, re);
            Assert.Equal(CSharpExpressionType.DivideAssign, be3.OperationNodeType);
            Assert.Same(le, be3.Left.Expression);
            Assert.Same(re, be3.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be3.Flags);

            var bd3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.DivideAssign, bd3.OperationNodeType);
            Assert.Same(ld, bd3.Left);
            Assert.Same(rd, bd3.Right);
            Assert.Equal(CSharpBinderFlags.None, bd3.Flags);

            var bf3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.DivideAssign, bf3.OperationNodeType);
            Assert.Same(ld, bf3.Left);
            Assert.Same(rd, bf3.Right);
            Assert.Equal(CSharpBinderFlags.None, bf3.Flags);

            var bc3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.DivideAssign, bc3.OperationNodeType);
            Assert.Same(ld, bc3.Left);
            Assert.Same(rd, bc3.Right);
            Assert.Equal(CSharpBinderFlags.None, bc3.Flags);
            Assert.Equal(typeof(int), bc3.Context);

            var be4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(le, re);
            Assert.Equal(CSharpExpressionType.ExclusiveOrAssign, be4.OperationNodeType);
            Assert.Same(le, be4.Left.Expression);
            Assert.Same(re, be4.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be4.Flags);

            var bd4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.ExclusiveOrAssign, bd4.OperationNodeType);
            Assert.Same(ld, bd4.Left);
            Assert.Same(rd, bd4.Right);
            Assert.Equal(CSharpBinderFlags.None, bd4.Flags);

            var bf4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.ExclusiveOrAssign, bf4.OperationNodeType);
            Assert.Same(ld, bf4.Left);
            Assert.Same(rd, bf4.Right);
            Assert.Equal(CSharpBinderFlags.None, bf4.Flags);

            var bc4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.ExclusiveOrAssign, bc4.OperationNodeType);
            Assert.Same(ld, bc4.Left);
            Assert.Same(rd, bc4.Right);
            Assert.Equal(CSharpBinderFlags.None, bc4.Flags);
            Assert.Equal(typeof(int), bc4.Context);

            var be5 = DynamicCSharpExpression.DynamicLeftShiftAssign(le, re);
            Assert.Equal(CSharpExpressionType.LeftShiftAssign, be5.OperationNodeType);
            Assert.Same(le, be5.Left.Expression);
            Assert.Same(re, be5.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be5.Flags);

            var bd5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.LeftShiftAssign, bd5.OperationNodeType);
            Assert.Same(ld, bd5.Left);
            Assert.Same(rd, bd5.Right);
            Assert.Equal(CSharpBinderFlags.None, bd5.Flags);

            var bf5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.LeftShiftAssign, bf5.OperationNodeType);
            Assert.Same(ld, bf5.Left);
            Assert.Same(rd, bf5.Right);
            Assert.Equal(CSharpBinderFlags.None, bf5.Flags);

            var bc5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.LeftShiftAssign, bc5.OperationNodeType);
            Assert.Same(ld, bc5.Left);
            Assert.Same(rd, bc5.Right);
            Assert.Equal(CSharpBinderFlags.None, bc5.Flags);
            Assert.Equal(typeof(int), bc5.Context);

            var be6 = DynamicCSharpExpression.DynamicModuloAssign(le, re);
            Assert.Equal(CSharpExpressionType.ModuloAssign, be6.OperationNodeType);
            Assert.Same(le, be6.Left.Expression);
            Assert.Same(re, be6.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be6.Flags);

            var bd6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.ModuloAssign, bd6.OperationNodeType);
            Assert.Same(ld, bd6.Left);
            Assert.Same(rd, bd6.Right);
            Assert.Equal(CSharpBinderFlags.None, bd6.Flags);

            var bf6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.ModuloAssign, bf6.OperationNodeType);
            Assert.Same(ld, bf6.Left);
            Assert.Same(rd, bf6.Right);
            Assert.Equal(CSharpBinderFlags.None, bf6.Flags);

            var bc6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.ModuloAssign, bc6.OperationNodeType);
            Assert.Same(ld, bc6.Left);
            Assert.Same(rd, bc6.Right);
            Assert.Equal(CSharpBinderFlags.None, bc6.Flags);
            Assert.Equal(typeof(int), bc6.Context);

            var be7 = DynamicCSharpExpression.DynamicMultiplyAssign(le, re);
            Assert.Equal(CSharpExpressionType.MultiplyAssign, be7.OperationNodeType);
            Assert.Same(le, be7.Left.Expression);
            Assert.Same(re, be7.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be7.Flags);

            var bd7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.MultiplyAssign, bd7.OperationNodeType);
            Assert.Same(ld, bd7.Left);
            Assert.Same(rd, bd7.Right);
            Assert.Equal(CSharpBinderFlags.None, bd7.Flags);

            var bf7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.MultiplyAssign, bf7.OperationNodeType);
            Assert.Same(ld, bf7.Left);
            Assert.Same(rd, bf7.Right);
            Assert.Equal(CSharpBinderFlags.None, bf7.Flags);

            var bc7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.MultiplyAssign, bc7.OperationNodeType);
            Assert.Same(ld, bc7.Left);
            Assert.Same(rd, bc7.Right);
            Assert.Equal(CSharpBinderFlags.None, bc7.Flags);
            Assert.Equal(typeof(int), bc7.Context);

            var be8 = DynamicCSharpExpression.DynamicOrAssign(le, re);
            Assert.Equal(CSharpExpressionType.OrAssign, be8.OperationNodeType);
            Assert.Same(le, be8.Left.Expression);
            Assert.Same(re, be8.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be8.Flags);

            var bd8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.OrAssign, bd8.OperationNodeType);
            Assert.Same(ld, bd8.Left);
            Assert.Same(rd, bd8.Right);
            Assert.Equal(CSharpBinderFlags.None, bd8.Flags);

            var bf8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.OrAssign, bf8.OperationNodeType);
            Assert.Same(ld, bf8.Left);
            Assert.Same(rd, bf8.Right);
            Assert.Equal(CSharpBinderFlags.None, bf8.Flags);

            var bc8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.OrAssign, bc8.OperationNodeType);
            Assert.Same(ld, bc8.Left);
            Assert.Same(rd, bc8.Right);
            Assert.Equal(CSharpBinderFlags.None, bc8.Flags);
            Assert.Equal(typeof(int), bc8.Context);

            var be9 = DynamicCSharpExpression.DynamicRightShiftAssign(le, re);
            Assert.Equal(CSharpExpressionType.RightShiftAssign, be9.OperationNodeType);
            Assert.Same(le, be9.Left.Expression);
            Assert.Same(re, be9.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be9.Flags);

            var bd9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.RightShiftAssign, bd9.OperationNodeType);
            Assert.Same(ld, bd9.Left);
            Assert.Same(rd, bd9.Right);
            Assert.Equal(CSharpBinderFlags.None, bd9.Flags);

            var bf9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.RightShiftAssign, bf9.OperationNodeType);
            Assert.Same(ld, bf9.Left);
            Assert.Same(rd, bf9.Right);
            Assert.Equal(CSharpBinderFlags.None, bf9.Flags);

            var bc9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.RightShiftAssign, bc9.OperationNodeType);
            Assert.Same(ld, bc9.Left);
            Assert.Same(rd, bc9.Right);
            Assert.Equal(CSharpBinderFlags.None, bc9.Flags);
            Assert.Equal(typeof(int), bc9.Context);

            var be10 = DynamicCSharpExpression.DynamicSubtractAssign(le, re);
            Assert.Equal(CSharpExpressionType.SubtractAssign, be10.OperationNodeType);
            Assert.Same(le, be10.Left.Expression);
            Assert.Same(re, be10.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be10.Flags);

            var bd10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd);
            Assert.Equal(CSharpExpressionType.SubtractAssign, bd10.OperationNodeType);
            Assert.Same(ld, bd10.Left);
            Assert.Same(rd, bd10.Right);
            Assert.Equal(CSharpBinderFlags.None, bd10.Flags);

            var bf10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.SubtractAssign, bf10.OperationNodeType);
            Assert.Same(ld, bf10.Left);
            Assert.Same(rd, bf10.Right);
            Assert.Equal(CSharpBinderFlags.None, bf10.Flags);

            var bc10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.SubtractAssign, bc10.OperationNodeType);
            Assert.Same(ld, bc10.Left);
            Assert.Same(rd, bc10.Right);
            Assert.Equal(CSharpBinderFlags.None, bc10.Flags);
            Assert.Equal(typeof(int), bc10.Context);

            var be11 = DynamicCSharpExpression.DynamicAddAssignChecked(le, re);
            Assert.Equal(CSharpExpressionType.AddAssignChecked, be11.OperationNodeType);
            Assert.Same(le, be11.Left.Expression);
            Assert.Same(re, be11.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be11.Flags);

            var bd11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd);
            Assert.Equal(CSharpExpressionType.AddAssignChecked, bd11.OperationNodeType);
            Assert.Same(ld, bd11.Left);
            Assert.Same(rd, bd11.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd11.Flags);

            var bf11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.AddAssignChecked, bf11.OperationNodeType);
            Assert.Same(ld, bf11.Left);
            Assert.Same(rd, bf11.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf11.Flags);

            var bc11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.AddAssignChecked, bc11.OperationNodeType);
            Assert.Same(ld, bc11.Left);
            Assert.Same(rd, bc11.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc11.Flags);
            Assert.Equal(typeof(int), bc11.Context);

            var be12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(le, re);
            Assert.Equal(CSharpExpressionType.MultiplyAssignChecked, be12.OperationNodeType);
            Assert.Same(le, be12.Left.Expression);
            Assert.Same(re, be12.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be12.Flags);

            var bd12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd);
            Assert.Equal(CSharpExpressionType.MultiplyAssignChecked, bd12.OperationNodeType);
            Assert.Same(ld, bd12.Left);
            Assert.Same(rd, bd12.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd12.Flags);

            var bf12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.MultiplyAssignChecked, bf12.OperationNodeType);
            Assert.Same(ld, bf12.Left);
            Assert.Same(rd, bf12.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf12.Flags);

            var bc12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.MultiplyAssignChecked, bc12.OperationNodeType);
            Assert.Same(ld, bc12.Left);
            Assert.Same(rd, bc12.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc12.Flags);
            Assert.Equal(typeof(int), bc12.Context);

            var be13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(le, re);
            Assert.Equal(CSharpExpressionType.SubtractAssignChecked, be13.OperationNodeType);
            Assert.Same(le, be13.Left.Expression);
            Assert.Same(re, be13.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be13.Flags);

            var bd13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd);
            Assert.Equal(CSharpExpressionType.SubtractAssignChecked, bd13.OperationNodeType);
            Assert.Same(ld, bd13.Left);
            Assert.Same(rd, bd13.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd13.Flags);

            var bf13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.SubtractAssignChecked, bf13.OperationNodeType);
            Assert.Same(ld, bf13.Left);
            Assert.Same(rd, bf13.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf13.Flags);

            var bc13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.SubtractAssignChecked, bc13.OperationNodeType);
            Assert.Same(ld, bc13.Left);
            Assert.Same(rd, bc13.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc13.Flags);
            Assert.Equal(typeof(int), bc13.Context);

        }

        [Fact]
        public void Dynamic_UnaryAssign_GeneratedFactories()
        {
            var oe = Expression.Parameter(typeof(object));

            var od = DynamicCSharpExpression.DynamicArgument(oe);

            var ue0 = DynamicCSharpExpression.DynamicPreIncrementAssign(oe);
            Assert.Equal(CSharpExpressionType.PreIncrementAssign, ue0.OperationNodeType);
            Assert.Same(oe, ue0.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue0.Flags);

            var ud0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od);
            Assert.Equal(CSharpExpressionType.PreIncrementAssign, ud0.OperationNodeType);
            Assert.Same(od, ud0.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud0.Flags);

            var uf0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.PreIncrementAssign, uf0.OperationNodeType);
            Assert.Same(od, uf0.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf0.Flags);

            var uc0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.PreIncrementAssign, uc0.OperationNodeType);
            Assert.Same(od, uc0.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc0.Flags);
            Assert.Equal(typeof(int), uc0.Context);

            var ue1 = DynamicCSharpExpression.DynamicPreDecrementAssign(oe);
            Assert.Equal(CSharpExpressionType.PreDecrementAssign, ue1.OperationNodeType);
            Assert.Same(oe, ue1.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue1.Flags);

            var ud1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od);
            Assert.Equal(CSharpExpressionType.PreDecrementAssign, ud1.OperationNodeType);
            Assert.Same(od, ud1.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud1.Flags);

            var uf1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.PreDecrementAssign, uf1.OperationNodeType);
            Assert.Same(od, uf1.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf1.Flags);

            var uc1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.PreDecrementAssign, uc1.OperationNodeType);
            Assert.Same(od, uc1.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc1.Flags);
            Assert.Equal(typeof(int), uc1.Context);

            var ue2 = DynamicCSharpExpression.DynamicPostIncrementAssign(oe);
            Assert.Equal(CSharpExpressionType.PostIncrementAssign, ue2.OperationNodeType);
            Assert.Same(oe, ue2.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue2.Flags);

            var ud2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od);
            Assert.Equal(CSharpExpressionType.PostIncrementAssign, ud2.OperationNodeType);
            Assert.Same(od, ud2.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud2.Flags);

            var uf2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.PostIncrementAssign, uf2.OperationNodeType);
            Assert.Same(od, uf2.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf2.Flags);

            var uc2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.PostIncrementAssign, uc2.OperationNodeType);
            Assert.Same(od, uc2.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc2.Flags);
            Assert.Equal(typeof(int), uc2.Context);

            var ue3 = DynamicCSharpExpression.DynamicPostDecrementAssign(oe);
            Assert.Equal(CSharpExpressionType.PostDecrementAssign, ue3.OperationNodeType);
            Assert.Same(oe, ue3.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue3.Flags);

            var ud3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od);
            Assert.Equal(CSharpExpressionType.PostDecrementAssign, ud3.OperationNodeType);
            Assert.Same(od, ud3.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud3.Flags);

            var uf3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od, CSharpBinderFlags.None);
            Assert.Equal(CSharpExpressionType.PostDecrementAssign, uf3.OperationNodeType);
            Assert.Same(od, uf3.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf3.Flags);

            var uc3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(CSharpExpressionType.PostDecrementAssign, uc3.OperationNodeType);
            Assert.Same(od, uc3.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc3.Flags);
            Assert.Equal(typeof(int), uc3.Context);

            var ue4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(oe);
            Assert.Equal(CSharpExpressionType.PreIncrementAssignChecked, ue4.OperationNodeType);
            Assert.Same(oe, ue4.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ue4.Flags);

            var ud4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od);
            Assert.Equal(CSharpExpressionType.PreIncrementAssignChecked, ud4.OperationNodeType);
            Assert.Same(od, ud4.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ud4.Flags);

            var uf4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.PreIncrementAssignChecked, uf4.OperationNodeType);
            Assert.Same(od, uf4.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uf4.Flags);

            var uc4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.PreIncrementAssignChecked, uc4.OperationNodeType);
            Assert.Same(od, uc4.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uc4.Flags);
            Assert.Equal(typeof(int), uc4.Context);

            var ue5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(oe);
            Assert.Equal(CSharpExpressionType.PreDecrementAssignChecked, ue5.OperationNodeType);
            Assert.Same(oe, ue5.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ue5.Flags);

            var ud5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od);
            Assert.Equal(CSharpExpressionType.PreDecrementAssignChecked, ud5.OperationNodeType);
            Assert.Same(od, ud5.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ud5.Flags);

            var uf5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.PreDecrementAssignChecked, uf5.OperationNodeType);
            Assert.Same(od, uf5.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uf5.Flags);

            var uc5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.PreDecrementAssignChecked, uc5.OperationNodeType);
            Assert.Same(od, uc5.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uc5.Flags);
            Assert.Equal(typeof(int), uc5.Context);

            var ue6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(oe);
            Assert.Equal(CSharpExpressionType.PostIncrementAssignChecked, ue6.OperationNodeType);
            Assert.Same(oe, ue6.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ue6.Flags);

            var ud6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od);
            Assert.Equal(CSharpExpressionType.PostIncrementAssignChecked, ud6.OperationNodeType);
            Assert.Same(od, ud6.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ud6.Flags);

            var uf6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.PostIncrementAssignChecked, uf6.OperationNodeType);
            Assert.Same(od, uf6.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uf6.Flags);

            var uc6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.PostIncrementAssignChecked, uc6.OperationNodeType);
            Assert.Same(od, uc6.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uc6.Flags);
            Assert.Equal(typeof(int), uc6.Context);

            var ue7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(oe);
            Assert.Equal(CSharpExpressionType.PostDecrementAssignChecked, ue7.OperationNodeType);
            Assert.Same(oe, ue7.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ue7.Flags);

            var ud7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od);
            Assert.Equal(CSharpExpressionType.PostDecrementAssignChecked, ud7.OperationNodeType);
            Assert.Same(od, ud7.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ud7.Flags);

            var uf7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.Equal(CSharpExpressionType.PostDecrementAssignChecked, uf7.OperationNodeType);
            Assert.Same(od, uf7.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uf7.Flags);

            var uc7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(CSharpExpressionType.PostDecrementAssignChecked, uc7.OperationNodeType);
            Assert.Same(od, uc7.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uc7.Flags);
            Assert.Equal(typeof(int), uc7.Context);

        }
    }
}
