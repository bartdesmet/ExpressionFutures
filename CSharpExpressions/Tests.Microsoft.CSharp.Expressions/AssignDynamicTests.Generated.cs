// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    partial class DynamicTests
    {
        [TestMethod]
        public void Dynamic_BinaryAssign_GeneratedFactories()
        {
            var le = Expression.Parameter(typeof(object));
            var re = Expression.Constant(2);

            var ld = DynamicCSharpExpression.DynamicArgument(le);
            var rd = DynamicCSharpExpression.DynamicArgument(re);

            var be0 = DynamicCSharpExpression.DynamicAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.Assign, be0.OperationNodeType);
            Assert.AreSame(le, be0.Left.Expression);
            Assert.AreSame(re, be0.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be0.Flags);

            var bd0 = DynamicCSharpExpression.DynamicAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.Assign, bd0.OperationNodeType);
            Assert.AreSame(ld, bd0.Left);
            Assert.AreSame(rd, bd0.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd0.Flags);

            var bf0 = DynamicCSharpExpression.DynamicAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.Assign, bf0.OperationNodeType);
            Assert.AreSame(ld, bf0.Left);
            Assert.AreSame(rd, bf0.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf0.Flags);

            var bc0 = DynamicCSharpExpression.DynamicAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.Assign, bc0.OperationNodeType);
            Assert.AreSame(ld, bc0.Left);
            Assert.AreSame(rd, bc0.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc0.Flags);
            Assert.AreEqual(typeof(int), bc0.Context);

            var be1 = DynamicCSharpExpression.DynamicAddAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.AddAssign, be1.OperationNodeType);
            Assert.AreSame(le, be1.Left.Expression);
            Assert.AreSame(re, be1.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be1.Flags);

            var bd1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.AddAssign, bd1.OperationNodeType);
            Assert.AreSame(ld, bd1.Left);
            Assert.AreSame(rd, bd1.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd1.Flags);

            var bf1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.AddAssign, bf1.OperationNodeType);
            Assert.AreSame(ld, bf1.Left);
            Assert.AreSame(rd, bf1.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf1.Flags);

            var bc1 = DynamicCSharpExpression.DynamicAddAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.AddAssign, bc1.OperationNodeType);
            Assert.AreSame(ld, bc1.Left);
            Assert.AreSame(rd, bc1.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc1.Flags);
            Assert.AreEqual(typeof(int), bc1.Context);

            var be2 = DynamicCSharpExpression.DynamicAndAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.AndAssign, be2.OperationNodeType);
            Assert.AreSame(le, be2.Left.Expression);
            Assert.AreSame(re, be2.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be2.Flags);

            var bd2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.AndAssign, bd2.OperationNodeType);
            Assert.AreSame(ld, bd2.Left);
            Assert.AreSame(rd, bd2.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd2.Flags);

            var bf2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.AndAssign, bf2.OperationNodeType);
            Assert.AreSame(ld, bf2.Left);
            Assert.AreSame(rd, bf2.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf2.Flags);

            var bc2 = DynamicCSharpExpression.DynamicAndAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.AndAssign, bc2.OperationNodeType);
            Assert.AreSame(ld, bc2.Left);
            Assert.AreSame(rd, bc2.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc2.Flags);
            Assert.AreEqual(typeof(int), bc2.Context);

            var be3 = DynamicCSharpExpression.DynamicDivideAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.DivideAssign, be3.OperationNodeType);
            Assert.AreSame(le, be3.Left.Expression);
            Assert.AreSame(re, be3.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be3.Flags);

            var bd3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.DivideAssign, bd3.OperationNodeType);
            Assert.AreSame(ld, bd3.Left);
            Assert.AreSame(rd, bd3.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd3.Flags);

            var bf3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.DivideAssign, bf3.OperationNodeType);
            Assert.AreSame(ld, bf3.Left);
            Assert.AreSame(rd, bf3.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf3.Flags);

            var bc3 = DynamicCSharpExpression.DynamicDivideAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.DivideAssign, bc3.OperationNodeType);
            Assert.AreSame(ld, bc3.Left);
            Assert.AreSame(rd, bc3.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc3.Flags);
            Assert.AreEqual(typeof(int), bc3.Context);

            var be4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.ExclusiveOrAssign, be4.OperationNodeType);
            Assert.AreSame(le, be4.Left.Expression);
            Assert.AreSame(re, be4.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be4.Flags);

            var bd4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.ExclusiveOrAssign, bd4.OperationNodeType);
            Assert.AreSame(ld, bd4.Left);
            Assert.AreSame(rd, bd4.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd4.Flags);

            var bf4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.ExclusiveOrAssign, bf4.OperationNodeType);
            Assert.AreSame(ld, bf4.Left);
            Assert.AreSame(rd, bf4.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf4.Flags);

            var bc4 = DynamicCSharpExpression.DynamicExclusiveOrAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.ExclusiveOrAssign, bc4.OperationNodeType);
            Assert.AreSame(ld, bc4.Left);
            Assert.AreSame(rd, bc4.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc4.Flags);
            Assert.AreEqual(typeof(int), bc4.Context);

            var be5 = DynamicCSharpExpression.DynamicLeftShiftAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.LeftShiftAssign, be5.OperationNodeType);
            Assert.AreSame(le, be5.Left.Expression);
            Assert.AreSame(re, be5.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be5.Flags);

            var bd5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.LeftShiftAssign, bd5.OperationNodeType);
            Assert.AreSame(ld, bd5.Left);
            Assert.AreSame(rd, bd5.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd5.Flags);

            var bf5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.LeftShiftAssign, bf5.OperationNodeType);
            Assert.AreSame(ld, bf5.Left);
            Assert.AreSame(rd, bf5.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf5.Flags);

            var bc5 = DynamicCSharpExpression.DynamicLeftShiftAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.LeftShiftAssign, bc5.OperationNodeType);
            Assert.AreSame(ld, bc5.Left);
            Assert.AreSame(rd, bc5.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc5.Flags);
            Assert.AreEqual(typeof(int), bc5.Context);

            var be6 = DynamicCSharpExpression.DynamicModuloAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.ModuloAssign, be6.OperationNodeType);
            Assert.AreSame(le, be6.Left.Expression);
            Assert.AreSame(re, be6.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be6.Flags);

            var bd6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.ModuloAssign, bd6.OperationNodeType);
            Assert.AreSame(ld, bd6.Left);
            Assert.AreSame(rd, bd6.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd6.Flags);

            var bf6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.ModuloAssign, bf6.OperationNodeType);
            Assert.AreSame(ld, bf6.Left);
            Assert.AreSame(rd, bf6.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf6.Flags);

            var bc6 = DynamicCSharpExpression.DynamicModuloAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.ModuloAssign, bc6.OperationNodeType);
            Assert.AreSame(ld, bc6.Left);
            Assert.AreSame(rd, bc6.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc6.Flags);
            Assert.AreEqual(typeof(int), bc6.Context);

            var be7 = DynamicCSharpExpression.DynamicMultiplyAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssign, be7.OperationNodeType);
            Assert.AreSame(le, be7.Left.Expression);
            Assert.AreSame(re, be7.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be7.Flags);

            var bd7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssign, bd7.OperationNodeType);
            Assert.AreSame(ld, bd7.Left);
            Assert.AreSame(rd, bd7.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd7.Flags);

            var bf7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssign, bf7.OperationNodeType);
            Assert.AreSame(ld, bf7.Left);
            Assert.AreSame(rd, bf7.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf7.Flags);

            var bc7 = DynamicCSharpExpression.DynamicMultiplyAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.MultiplyAssign, bc7.OperationNodeType);
            Assert.AreSame(ld, bc7.Left);
            Assert.AreSame(rd, bc7.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc7.Flags);
            Assert.AreEqual(typeof(int), bc7.Context);

            var be8 = DynamicCSharpExpression.DynamicOrAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.OrAssign, be8.OperationNodeType);
            Assert.AreSame(le, be8.Left.Expression);
            Assert.AreSame(re, be8.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be8.Flags);

            var bd8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.OrAssign, bd8.OperationNodeType);
            Assert.AreSame(ld, bd8.Left);
            Assert.AreSame(rd, bd8.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd8.Flags);

            var bf8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.OrAssign, bf8.OperationNodeType);
            Assert.AreSame(ld, bf8.Left);
            Assert.AreSame(rd, bf8.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf8.Flags);

            var bc8 = DynamicCSharpExpression.DynamicOrAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.OrAssign, bc8.OperationNodeType);
            Assert.AreSame(ld, bc8.Left);
            Assert.AreSame(rd, bc8.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc8.Flags);
            Assert.AreEqual(typeof(int), bc8.Context);

            var be9 = DynamicCSharpExpression.DynamicRightShiftAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.RightShiftAssign, be9.OperationNodeType);
            Assert.AreSame(le, be9.Left.Expression);
            Assert.AreSame(re, be9.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be9.Flags);

            var bd9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.RightShiftAssign, bd9.OperationNodeType);
            Assert.AreSame(ld, bd9.Left);
            Assert.AreSame(rd, bd9.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd9.Flags);

            var bf9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.RightShiftAssign, bf9.OperationNodeType);
            Assert.AreSame(ld, bf9.Left);
            Assert.AreSame(rd, bf9.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf9.Flags);

            var bc9 = DynamicCSharpExpression.DynamicRightShiftAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.RightShiftAssign, bc9.OperationNodeType);
            Assert.AreSame(ld, bc9.Left);
            Assert.AreSame(rd, bc9.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc9.Flags);
            Assert.AreEqual(typeof(int), bc9.Context);

            var be10 = DynamicCSharpExpression.DynamicSubtractAssign(le, re);
            Assert.AreEqual(CSharpExpressionType.SubtractAssign, be10.OperationNodeType);
            Assert.AreSame(le, be10.Left.Expression);
            Assert.AreSame(re, be10.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, be10.Flags);

            var bd10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd);
            Assert.AreEqual(CSharpExpressionType.SubtractAssign, bd10.OperationNodeType);
            Assert.AreSame(ld, bd10.Left);
            Assert.AreSame(rd, bd10.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bd10.Flags);

            var bf10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.SubtractAssign, bf10.OperationNodeType);
            Assert.AreSame(ld, bf10.Left);
            Assert.AreSame(rd, bf10.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf10.Flags);

            var bc10 = DynamicCSharpExpression.DynamicSubtractAssign(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.SubtractAssign, bc10.OperationNodeType);
            Assert.AreSame(ld, bc10.Left);
            Assert.AreSame(rd, bc10.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc10.Flags);
            Assert.AreEqual(typeof(int), bc10.Context);

            var be11 = DynamicCSharpExpression.DynamicAddAssignChecked(le, re);
            Assert.AreEqual(CSharpExpressionType.AddAssignChecked, be11.OperationNodeType);
            Assert.AreSame(le, be11.Left.Expression);
            Assert.AreSame(re, be11.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, be11.Flags);

            var bd11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd);
            Assert.AreEqual(CSharpExpressionType.AddAssignChecked, bd11.OperationNodeType);
            Assert.AreSame(ld, bd11.Left);
            Assert.AreSame(rd, bd11.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd11.Flags);

            var bf11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.AddAssignChecked, bf11.OperationNodeType);
            Assert.AreSame(ld, bf11.Left);
            Assert.AreSame(rd, bf11.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf11.Flags);

            var bc11 = DynamicCSharpExpression.DynamicAddAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.AddAssignChecked, bc11.OperationNodeType);
            Assert.AreSame(ld, bc11.Left);
            Assert.AreSame(rd, bc11.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc11.Flags);
            Assert.AreEqual(typeof(int), bc11.Context);

            var be12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(le, re);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssignChecked, be12.OperationNodeType);
            Assert.AreSame(le, be12.Left.Expression);
            Assert.AreSame(re, be12.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, be12.Flags);

            var bd12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssignChecked, bd12.OperationNodeType);
            Assert.AreSame(ld, bd12.Left);
            Assert.AreSame(rd, bd12.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd12.Flags);

            var bf12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.MultiplyAssignChecked, bf12.OperationNodeType);
            Assert.AreSame(ld, bf12.Left);
            Assert.AreSame(rd, bf12.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf12.Flags);

            var bc12 = DynamicCSharpExpression.DynamicMultiplyAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.MultiplyAssignChecked, bc12.OperationNodeType);
            Assert.AreSame(ld, bc12.Left);
            Assert.AreSame(rd, bc12.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc12.Flags);
            Assert.AreEqual(typeof(int), bc12.Context);

            var be13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(le, re);
            Assert.AreEqual(CSharpExpressionType.SubtractAssignChecked, be13.OperationNodeType);
            Assert.AreSame(le, be13.Left.Expression);
            Assert.AreSame(re, be13.Right.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, be13.Flags);

            var bd13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd);
            Assert.AreEqual(CSharpExpressionType.SubtractAssignChecked, bd13.OperationNodeType);
            Assert.AreSame(ld, bd13.Left);
            Assert.AreSame(rd, bd13.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd13.Flags);

            var bf13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.SubtractAssignChecked, bf13.OperationNodeType);
            Assert.AreSame(ld, bf13.Left);
            Assert.AreSame(rd, bf13.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf13.Flags);

            var bc13 = DynamicCSharpExpression.DynamicSubtractAssignChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.SubtractAssignChecked, bc13.OperationNodeType);
            Assert.AreSame(ld, bc13.Left);
            Assert.AreSame(rd, bc13.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc13.Flags);
            Assert.AreEqual(typeof(int), bc13.Context);

        }

        [TestMethod]
        public void Dynamic_UnaryAssign_GeneratedFactories()
        {
            var oe = Expression.Parameter(typeof(object));

            var od = DynamicCSharpExpression.DynamicArgument(oe);

            var ue0 = DynamicCSharpExpression.DynamicPreIncrementAssign(oe);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssign, ue0.OperationNodeType);
            Assert.AreSame(oe, ue0.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, ue0.Flags);

            var ud0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssign, ud0.OperationNodeType);
            Assert.AreSame(od, ud0.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, ud0.Flags);

            var uf0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssign, uf0.OperationNodeType);
            Assert.AreSame(od, uf0.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf0.Flags);

            var uc0 = DynamicCSharpExpression.DynamicPreIncrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssign, uc0.OperationNodeType);
            Assert.AreSame(od, uc0.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc0.Flags);
            Assert.AreEqual(typeof(int), uc0.Context);

            var ue1 = DynamicCSharpExpression.DynamicPreDecrementAssign(oe);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssign, ue1.OperationNodeType);
            Assert.AreSame(oe, ue1.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, ue1.Flags);

            var ud1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssign, ud1.OperationNodeType);
            Assert.AreSame(od, ud1.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, ud1.Flags);

            var uf1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssign, uf1.OperationNodeType);
            Assert.AreSame(od, uf1.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf1.Flags);

            var uc1 = DynamicCSharpExpression.DynamicPreDecrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssign, uc1.OperationNodeType);
            Assert.AreSame(od, uc1.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc1.Flags);
            Assert.AreEqual(typeof(int), uc1.Context);

            var ue2 = DynamicCSharpExpression.DynamicPostIncrementAssign(oe);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssign, ue2.OperationNodeType);
            Assert.AreSame(oe, ue2.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, ue2.Flags);

            var ud2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssign, ud2.OperationNodeType);
            Assert.AreSame(od, ud2.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, ud2.Flags);

            var uf2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssign, uf2.OperationNodeType);
            Assert.AreSame(od, uf2.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf2.Flags);

            var uc2 = DynamicCSharpExpression.DynamicPostIncrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssign, uc2.OperationNodeType);
            Assert.AreSame(od, uc2.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc2.Flags);
            Assert.AreEqual(typeof(int), uc2.Context);

            var ue3 = DynamicCSharpExpression.DynamicPostDecrementAssign(oe);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssign, ue3.OperationNodeType);
            Assert.AreSame(oe, ue3.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.None, ue3.Flags);

            var ud3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssign, ud3.OperationNodeType);
            Assert.AreSame(od, ud3.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, ud3.Flags);

            var uf3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od, CSharpBinderFlags.None);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssign, uf3.OperationNodeType);
            Assert.AreSame(od, uf3.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf3.Flags);

            var uc3 = DynamicCSharpExpression.DynamicPostDecrementAssign(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssign, uc3.OperationNodeType);
            Assert.AreSame(od, uc3.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc3.Flags);
            Assert.AreEqual(typeof(int), uc3.Context);

            var ue4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(oe);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssignChecked, ue4.OperationNodeType);
            Assert.AreSame(oe, ue4.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ue4.Flags);

            var ud4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssignChecked, ud4.OperationNodeType);
            Assert.AreSame(od, ud4.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ud4.Flags);

            var uf4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssignChecked, uf4.OperationNodeType);
            Assert.AreSame(od, uf4.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf4.Flags);

            var uc4 = DynamicCSharpExpression.DynamicPreIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PreIncrementAssignChecked, uc4.OperationNodeType);
            Assert.AreSame(od, uc4.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc4.Flags);
            Assert.AreEqual(typeof(int), uc4.Context);

            var ue5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(oe);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssignChecked, ue5.OperationNodeType);
            Assert.AreSame(oe, ue5.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ue5.Flags);

            var ud5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssignChecked, ud5.OperationNodeType);
            Assert.AreSame(od, ud5.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ud5.Flags);

            var uf5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssignChecked, uf5.OperationNodeType);
            Assert.AreSame(od, uf5.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf5.Flags);

            var uc5 = DynamicCSharpExpression.DynamicPreDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PreDecrementAssignChecked, uc5.OperationNodeType);
            Assert.AreSame(od, uc5.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc5.Flags);
            Assert.AreEqual(typeof(int), uc5.Context);

            var ue6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(oe);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssignChecked, ue6.OperationNodeType);
            Assert.AreSame(oe, ue6.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ue6.Flags);

            var ud6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssignChecked, ud6.OperationNodeType);
            Assert.AreSame(od, ud6.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ud6.Flags);

            var uf6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssignChecked, uf6.OperationNodeType);
            Assert.AreSame(od, uf6.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf6.Flags);

            var uc6 = DynamicCSharpExpression.DynamicPostIncrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PostIncrementAssignChecked, uc6.OperationNodeType);
            Assert.AreSame(od, uc6.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc6.Flags);
            Assert.AreEqual(typeof(int), uc6.Context);

            var ue7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(oe);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssignChecked, ue7.OperationNodeType);
            Assert.AreSame(oe, ue7.Operand.Expression);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ue7.Flags);

            var ud7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssignChecked, ud7.OperationNodeType);
            Assert.AreSame(od, ud7.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, ud7.Flags);

            var uf7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssignChecked, uf7.OperationNodeType);
            Assert.AreSame(od, uf7.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf7.Flags);

            var uc7 = DynamicCSharpExpression.DynamicPostDecrementAssignChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(CSharpExpressionType.PostDecrementAssignChecked, uc7.OperationNodeType);
            Assert.AreSame(od, uc7.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc7.Flags);
            Assert.AreEqual(typeof(int), uc7.Context);

        }
    }
}
