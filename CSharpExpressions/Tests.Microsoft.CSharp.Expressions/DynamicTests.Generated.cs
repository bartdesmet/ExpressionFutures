// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    partial class DynamicTests
    {
        [TestMethod]
        public void Dynamic_Binary_GeneratedFactories()
        {
            var le = Expression.Constant(1);
            var re = Expression.Constant(2);

            var ld = DynamicCSharpExpression.DynamicArgument(le);
            var rd = DynamicCSharpExpression.DynamicArgument(re);

            var be0 = DynamicCSharpExpression.DynamicAdd(le, re);
            Assert.AreEqual(ExpressionType.Add, be0.OperationNodeType);
            Assert.AreSame(le, be0.Left.Expression);
            Assert.AreSame(re, be0.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be0.Flags);

            var bd0 = DynamicCSharpExpression.DynamicAdd(ld, rd);
            Assert.AreEqual(ExpressionType.Add, bd0.OperationNodeType);
            Assert.AreSame(ld, bd0.Left);
            Assert.AreSame(rd, bd0.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd0.Flags);

            var bf0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Add, bf0.OperationNodeType);
            Assert.AreSame(ld, bf0.Left);
            Assert.AreSame(rd, bf0.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf0.Flags);

            var bc0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Add, bc0.OperationNodeType);
            Assert.AreSame(ld, bc0.Left);
            Assert.AreSame(rd, bc0.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc0.Flags);
            Assert.AreEqual(typeof(int), bc0.Context);

            var be1 = DynamicCSharpExpression.DynamicAddChecked(le, re);
            Assert.AreEqual(ExpressionType.AddChecked, be1.OperationNodeType);
            Assert.AreSame(le, be1.Left.Expression);
            Assert.AreSame(re, be1.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, be1.Flags);

            var bd1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd);
            Assert.AreEqual(ExpressionType.AddChecked, bd1.OperationNodeType);
            Assert.AreSame(ld, bd1.Left);
            Assert.AreSame(rd, bd1.Right);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd1.Flags);

            var bf1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.AddChecked, bf1.OperationNodeType);
            Assert.AreSame(ld, bf1.Left);
            Assert.AreSame(rd, bf1.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf1.Flags);

            var bc1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.AddChecked, bc1.OperationNodeType);
            Assert.AreSame(ld, bc1.Left);
            Assert.AreSame(rd, bc1.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc1.Flags);
            Assert.AreEqual(typeof(int), bc1.Context);

            var be2 = DynamicCSharpExpression.DynamicAnd(le, re);
            Assert.AreEqual(ExpressionType.And, be2.OperationNodeType);
            Assert.AreSame(le, be2.Left.Expression);
            Assert.AreSame(re, be2.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be2.Flags);

            var bd2 = DynamicCSharpExpression.DynamicAnd(ld, rd);
            Assert.AreEqual(ExpressionType.And, bd2.OperationNodeType);
            Assert.AreSame(ld, bd2.Left);
            Assert.AreSame(rd, bd2.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd2.Flags);

            var bf2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.And, bf2.OperationNodeType);
            Assert.AreSame(ld, bf2.Left);
            Assert.AreSame(rd, bf2.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf2.Flags);

            var bc2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.And, bc2.OperationNodeType);
            Assert.AreSame(ld, bc2.Left);
            Assert.AreSame(rd, bc2.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc2.Flags);
            Assert.AreEqual(typeof(int), bc2.Context);

            var be3 = DynamicCSharpExpression.DynamicAndAlso(le, re);
            Assert.AreEqual(ExpressionType.AndAlso, be3.OperationNodeType);
            Assert.AreSame(le, be3.Left.Expression);
            Assert.AreSame(re, be3.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, be3.Flags);

            var bd3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd);
            Assert.AreEqual(ExpressionType.AndAlso, bd3.OperationNodeType);
            Assert.AreSame(ld, bd3.Left);
            Assert.AreSame(rd, bd3.Right);
			Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bd3.Flags);

            var bf3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.BinaryOperationLogical);
            Assert.AreEqual(ExpressionType.AndAlso, bf3.OperationNodeType);
            Assert.AreSame(ld, bf3.Left);
            Assert.AreSame(rd, bf3.Right);
            Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bf3.Flags);

            var bc3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.BinaryOperationLogical, typeof(int));
            Assert.AreEqual(ExpressionType.AndAlso, bc3.OperationNodeType);
            Assert.AreSame(ld, bc3.Left);
            Assert.AreSame(rd, bc3.Right);
            Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bc3.Flags);
            Assert.AreEqual(typeof(int), bc3.Context);

            var be4 = DynamicCSharpExpression.DynamicCoalesce(le, re);
            Assert.AreEqual(ExpressionType.Coalesce, be4.OperationNodeType);
            Assert.AreSame(le, be4.Left.Expression);
            Assert.AreSame(re, be4.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be4.Flags);

            var bd4 = DynamicCSharpExpression.DynamicCoalesce(ld, rd);
            Assert.AreEqual(ExpressionType.Coalesce, bd4.OperationNodeType);
            Assert.AreSame(ld, bd4.Left);
            Assert.AreSame(rd, bd4.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd4.Flags);

            var bf4 = DynamicCSharpExpression.DynamicCoalesce(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Coalesce, bf4.OperationNodeType);
            Assert.AreSame(ld, bf4.Left);
            Assert.AreSame(rd, bf4.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf4.Flags);

            var bc4 = DynamicCSharpExpression.DynamicCoalesce(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Coalesce, bc4.OperationNodeType);
            Assert.AreSame(ld, bc4.Left);
            Assert.AreSame(rd, bc4.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc4.Flags);
            Assert.AreEqual(typeof(int), bc4.Context);

            var be5 = DynamicCSharpExpression.DynamicDivide(le, re);
            Assert.AreEqual(ExpressionType.Divide, be5.OperationNodeType);
            Assert.AreSame(le, be5.Left.Expression);
            Assert.AreSame(re, be5.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be5.Flags);

            var bd5 = DynamicCSharpExpression.DynamicDivide(ld, rd);
            Assert.AreEqual(ExpressionType.Divide, bd5.OperationNodeType);
            Assert.AreSame(ld, bd5.Left);
            Assert.AreSame(rd, bd5.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd5.Flags);

            var bf5 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Divide, bf5.OperationNodeType);
            Assert.AreSame(ld, bf5.Left);
            Assert.AreSame(rd, bf5.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf5.Flags);

            var bc5 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Divide, bc5.OperationNodeType);
            Assert.AreSame(ld, bc5.Left);
            Assert.AreSame(rd, bc5.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc5.Flags);
            Assert.AreEqual(typeof(int), bc5.Context);

            var be6 = DynamicCSharpExpression.DynamicEqual(le, re);
            Assert.AreEqual(ExpressionType.Equal, be6.OperationNodeType);
            Assert.AreSame(le, be6.Left.Expression);
            Assert.AreSame(re, be6.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be6.Flags);

            var bd6 = DynamicCSharpExpression.DynamicEqual(ld, rd);
            Assert.AreEqual(ExpressionType.Equal, bd6.OperationNodeType);
            Assert.AreSame(ld, bd6.Left);
            Assert.AreSame(rd, bd6.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd6.Flags);

            var bf6 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Equal, bf6.OperationNodeType);
            Assert.AreSame(ld, bf6.Left);
            Assert.AreSame(rd, bf6.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf6.Flags);

            var bc6 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Equal, bc6.OperationNodeType);
            Assert.AreSame(ld, bc6.Left);
            Assert.AreSame(rd, bc6.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc6.Flags);
            Assert.AreEqual(typeof(int), bc6.Context);

            var be7 = DynamicCSharpExpression.DynamicExclusiveOr(le, re);
            Assert.AreEqual(ExpressionType.ExclusiveOr, be7.OperationNodeType);
            Assert.AreSame(le, be7.Left.Expression);
            Assert.AreSame(re, be7.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be7.Flags);

            var bd7 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd);
            Assert.AreEqual(ExpressionType.ExclusiveOr, bd7.OperationNodeType);
            Assert.AreSame(ld, bd7.Left);
            Assert.AreSame(rd, bd7.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd7.Flags);

            var bf7 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.ExclusiveOr, bf7.OperationNodeType);
            Assert.AreSame(ld, bf7.Left);
            Assert.AreSame(rd, bf7.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf7.Flags);

            var bc7 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.ExclusiveOr, bc7.OperationNodeType);
            Assert.AreSame(ld, bc7.Left);
            Assert.AreSame(rd, bc7.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc7.Flags);
            Assert.AreEqual(typeof(int), bc7.Context);

            var be8 = DynamicCSharpExpression.DynamicGreaterThan(le, re);
            Assert.AreEqual(ExpressionType.GreaterThan, be8.OperationNodeType);
            Assert.AreSame(le, be8.Left.Expression);
            Assert.AreSame(re, be8.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be8.Flags);

            var bd8 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd);
            Assert.AreEqual(ExpressionType.GreaterThan, bd8.OperationNodeType);
            Assert.AreSame(ld, bd8.Left);
            Assert.AreSame(rd, bd8.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd8.Flags);

            var bf8 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.GreaterThan, bf8.OperationNodeType);
            Assert.AreSame(ld, bf8.Left);
            Assert.AreSame(rd, bf8.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf8.Flags);

            var bc8 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.GreaterThan, bc8.OperationNodeType);
            Assert.AreSame(ld, bc8.Left);
            Assert.AreSame(rd, bc8.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc8.Flags);
            Assert.AreEqual(typeof(int), bc8.Context);

            var be9 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(le, re);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, be9.OperationNodeType);
            Assert.AreSame(le, be9.Left.Expression);
            Assert.AreSame(re, be9.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be9.Flags);

            var bd9 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bd9.OperationNodeType);
            Assert.AreSame(ld, bd9.Left);
            Assert.AreSame(rd, bd9.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd9.Flags);

            var bf9 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bf9.OperationNodeType);
            Assert.AreSame(ld, bf9.Left);
            Assert.AreSame(rd, bf9.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf9.Flags);

            var bc9 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bc9.OperationNodeType);
            Assert.AreSame(ld, bc9.Left);
            Assert.AreSame(rd, bc9.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc9.Flags);
            Assert.AreEqual(typeof(int), bc9.Context);

            var be10 = DynamicCSharpExpression.DynamicLeftShift(le, re);
            Assert.AreEqual(ExpressionType.LeftShift, be10.OperationNodeType);
            Assert.AreSame(le, be10.Left.Expression);
            Assert.AreSame(re, be10.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be10.Flags);

            var bd10 = DynamicCSharpExpression.DynamicLeftShift(ld, rd);
            Assert.AreEqual(ExpressionType.LeftShift, bd10.OperationNodeType);
            Assert.AreSame(ld, bd10.Left);
            Assert.AreSame(rd, bd10.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd10.Flags);

            var bf10 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.LeftShift, bf10.OperationNodeType);
            Assert.AreSame(ld, bf10.Left);
            Assert.AreSame(rd, bf10.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf10.Flags);

            var bc10 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.LeftShift, bc10.OperationNodeType);
            Assert.AreSame(ld, bc10.Left);
            Assert.AreSame(rd, bc10.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc10.Flags);
            Assert.AreEqual(typeof(int), bc10.Context);

            var be11 = DynamicCSharpExpression.DynamicLessThan(le, re);
            Assert.AreEqual(ExpressionType.LessThan, be11.OperationNodeType);
            Assert.AreSame(le, be11.Left.Expression);
            Assert.AreSame(re, be11.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be11.Flags);

            var bd11 = DynamicCSharpExpression.DynamicLessThan(ld, rd);
            Assert.AreEqual(ExpressionType.LessThan, bd11.OperationNodeType);
            Assert.AreSame(ld, bd11.Left);
            Assert.AreSame(rd, bd11.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd11.Flags);

            var bf11 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.LessThan, bf11.OperationNodeType);
            Assert.AreSame(ld, bf11.Left);
            Assert.AreSame(rd, bf11.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf11.Flags);

            var bc11 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.LessThan, bc11.OperationNodeType);
            Assert.AreSame(ld, bc11.Left);
            Assert.AreSame(rd, bc11.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc11.Flags);
            Assert.AreEqual(typeof(int), bc11.Context);

            var be12 = DynamicCSharpExpression.DynamicLessThanOrEqual(le, re);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, be12.OperationNodeType);
            Assert.AreSame(le, be12.Left.Expression);
            Assert.AreSame(re, be12.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be12.Flags);

            var bd12 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bd12.OperationNodeType);
            Assert.AreSame(ld, bd12.Left);
            Assert.AreSame(rd, bd12.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd12.Flags);

            var bf12 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bf12.OperationNodeType);
            Assert.AreSame(ld, bf12.Left);
            Assert.AreSame(rd, bf12.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf12.Flags);

            var bc12 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bc12.OperationNodeType);
            Assert.AreSame(ld, bc12.Left);
            Assert.AreSame(rd, bc12.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc12.Flags);
            Assert.AreEqual(typeof(int), bc12.Context);

            var be13 = DynamicCSharpExpression.DynamicModulo(le, re);
            Assert.AreEqual(ExpressionType.Modulo, be13.OperationNodeType);
            Assert.AreSame(le, be13.Left.Expression);
            Assert.AreSame(re, be13.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be13.Flags);

            var bd13 = DynamicCSharpExpression.DynamicModulo(ld, rd);
            Assert.AreEqual(ExpressionType.Modulo, bd13.OperationNodeType);
            Assert.AreSame(ld, bd13.Left);
            Assert.AreSame(rd, bd13.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd13.Flags);

            var bf13 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Modulo, bf13.OperationNodeType);
            Assert.AreSame(ld, bf13.Left);
            Assert.AreSame(rd, bf13.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf13.Flags);

            var bc13 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Modulo, bc13.OperationNodeType);
            Assert.AreSame(ld, bc13.Left);
            Assert.AreSame(rd, bc13.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc13.Flags);
            Assert.AreEqual(typeof(int), bc13.Context);

            var be14 = DynamicCSharpExpression.DynamicMultiply(le, re);
            Assert.AreEqual(ExpressionType.Multiply, be14.OperationNodeType);
            Assert.AreSame(le, be14.Left.Expression);
            Assert.AreSame(re, be14.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be14.Flags);

            var bd14 = DynamicCSharpExpression.DynamicMultiply(ld, rd);
            Assert.AreEqual(ExpressionType.Multiply, bd14.OperationNodeType);
            Assert.AreSame(ld, bd14.Left);
            Assert.AreSame(rd, bd14.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd14.Flags);

            var bf14 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Multiply, bf14.OperationNodeType);
            Assert.AreSame(ld, bf14.Left);
            Assert.AreSame(rd, bf14.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf14.Flags);

            var bc14 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Multiply, bc14.OperationNodeType);
            Assert.AreSame(ld, bc14.Left);
            Assert.AreSame(rd, bc14.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc14.Flags);
            Assert.AreEqual(typeof(int), bc14.Context);

            var be15 = DynamicCSharpExpression.DynamicMultiplyChecked(le, re);
            Assert.AreEqual(ExpressionType.MultiplyChecked, be15.OperationNodeType);
            Assert.AreSame(le, be15.Left.Expression);
            Assert.AreSame(re, be15.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, be15.Flags);

            var bd15 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd);
            Assert.AreEqual(ExpressionType.MultiplyChecked, bd15.OperationNodeType);
            Assert.AreSame(ld, bd15.Left);
            Assert.AreSame(rd, bd15.Right);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd15.Flags);

            var bf15 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.MultiplyChecked, bf15.OperationNodeType);
            Assert.AreSame(ld, bf15.Left);
            Assert.AreSame(rd, bf15.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf15.Flags);

            var bc15 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.MultiplyChecked, bc15.OperationNodeType);
            Assert.AreSame(ld, bc15.Left);
            Assert.AreSame(rd, bc15.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc15.Flags);
            Assert.AreEqual(typeof(int), bc15.Context);

            var be16 = DynamicCSharpExpression.DynamicNotEqual(le, re);
            Assert.AreEqual(ExpressionType.NotEqual, be16.OperationNodeType);
            Assert.AreSame(le, be16.Left.Expression);
            Assert.AreSame(re, be16.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be16.Flags);

            var bd16 = DynamicCSharpExpression.DynamicNotEqual(ld, rd);
            Assert.AreEqual(ExpressionType.NotEqual, bd16.OperationNodeType);
            Assert.AreSame(ld, bd16.Left);
            Assert.AreSame(rd, bd16.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd16.Flags);

            var bf16 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.NotEqual, bf16.OperationNodeType);
            Assert.AreSame(ld, bf16.Left);
            Assert.AreSame(rd, bf16.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf16.Flags);

            var bc16 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.NotEqual, bc16.OperationNodeType);
            Assert.AreSame(ld, bc16.Left);
            Assert.AreSame(rd, bc16.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc16.Flags);
            Assert.AreEqual(typeof(int), bc16.Context);

            var be17 = DynamicCSharpExpression.DynamicOr(le, re);
            Assert.AreEqual(ExpressionType.Or, be17.OperationNodeType);
            Assert.AreSame(le, be17.Left.Expression);
            Assert.AreSame(re, be17.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be17.Flags);

            var bd17 = DynamicCSharpExpression.DynamicOr(ld, rd);
            Assert.AreEqual(ExpressionType.Or, bd17.OperationNodeType);
            Assert.AreSame(ld, bd17.Left);
            Assert.AreSame(rd, bd17.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd17.Flags);

            var bf17 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Or, bf17.OperationNodeType);
            Assert.AreSame(ld, bf17.Left);
            Assert.AreSame(rd, bf17.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf17.Flags);

            var bc17 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Or, bc17.OperationNodeType);
            Assert.AreSame(ld, bc17.Left);
            Assert.AreSame(rd, bc17.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc17.Flags);
            Assert.AreEqual(typeof(int), bc17.Context);

            var be18 = DynamicCSharpExpression.DynamicOrElse(le, re);
            Assert.AreEqual(ExpressionType.OrElse, be18.OperationNodeType);
            Assert.AreSame(le, be18.Left.Expression);
            Assert.AreSame(re, be18.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, be18.Flags);

            var bd18 = DynamicCSharpExpression.DynamicOrElse(ld, rd);
            Assert.AreEqual(ExpressionType.OrElse, bd18.OperationNodeType);
            Assert.AreSame(ld, bd18.Left);
            Assert.AreSame(rd, bd18.Right);
			Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bd18.Flags);

            var bf18 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.BinaryOperationLogical);
            Assert.AreEqual(ExpressionType.OrElse, bf18.OperationNodeType);
            Assert.AreSame(ld, bf18.Left);
            Assert.AreSame(rd, bf18.Right);
            Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bf18.Flags);

            var bc18 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.BinaryOperationLogical, typeof(int));
            Assert.AreEqual(ExpressionType.OrElse, bc18.OperationNodeType);
            Assert.AreSame(ld, bc18.Left);
            Assert.AreSame(rd, bc18.Right);
            Assert.AreEqual(CSharpBinderFlags.BinaryOperationLogical, bc18.Flags);
            Assert.AreEqual(typeof(int), bc18.Context);

            var be19 = DynamicCSharpExpression.DynamicRightShift(le, re);
            Assert.AreEqual(ExpressionType.RightShift, be19.OperationNodeType);
            Assert.AreSame(le, be19.Left.Expression);
            Assert.AreSame(re, be19.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be19.Flags);

            var bd19 = DynamicCSharpExpression.DynamicRightShift(ld, rd);
            Assert.AreEqual(ExpressionType.RightShift, bd19.OperationNodeType);
            Assert.AreSame(ld, bd19.Left);
            Assert.AreSame(rd, bd19.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd19.Flags);

            var bf19 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.RightShift, bf19.OperationNodeType);
            Assert.AreSame(ld, bf19.Left);
            Assert.AreSame(rd, bf19.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf19.Flags);

            var bc19 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.RightShift, bc19.OperationNodeType);
            Assert.AreSame(ld, bc19.Left);
            Assert.AreSame(rd, bc19.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc19.Flags);
            Assert.AreEqual(typeof(int), bc19.Context);

            var be20 = DynamicCSharpExpression.DynamicSubtract(le, re);
            Assert.AreEqual(ExpressionType.Subtract, be20.OperationNodeType);
            Assert.AreSame(le, be20.Left.Expression);
            Assert.AreSame(re, be20.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, be20.Flags);

            var bd20 = DynamicCSharpExpression.DynamicSubtract(ld, rd);
            Assert.AreEqual(ExpressionType.Subtract, bd20.OperationNodeType);
            Assert.AreSame(ld, bd20.Left);
            Assert.AreSame(rd, bd20.Right);
			Assert.AreEqual(CSharpBinderFlags.None, bd20.Flags);

            var bf20 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Subtract, bf20.OperationNodeType);
            Assert.AreSame(ld, bf20.Left);
            Assert.AreSame(rd, bf20.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bf20.Flags);

            var bc20 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Subtract, bc20.OperationNodeType);
            Assert.AreSame(ld, bc20.Left);
            Assert.AreSame(rd, bc20.Right);
            Assert.AreEqual(CSharpBinderFlags.None, bc20.Flags);
            Assert.AreEqual(typeof(int), bc20.Context);

            var be21 = DynamicCSharpExpression.DynamicSubtractChecked(le, re);
            Assert.AreEqual(ExpressionType.SubtractChecked, be21.OperationNodeType);
            Assert.AreSame(le, be21.Left.Expression);
            Assert.AreSame(re, be21.Right.Expression);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, be21.Flags);

            var bd21 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd);
            Assert.AreEqual(ExpressionType.SubtractChecked, bd21.OperationNodeType);
            Assert.AreSame(ld, bd21.Left);
            Assert.AreSame(rd, bd21.Right);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, bd21.Flags);

            var bf21 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.SubtractChecked, bf21.OperationNodeType);
            Assert.AreSame(ld, bf21.Left);
            Assert.AreSame(rd, bf21.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf21.Flags);

            var bc21 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.SubtractChecked, bc21.OperationNodeType);
            Assert.AreSame(ld, bc21.Left);
            Assert.AreSame(rd, bc21.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc21.Flags);
            Assert.AreEqual(typeof(int), bc21.Context);

        }

        [TestMethod]
        public void Dynamic_Unary_GeneratedFactories()
        {
            var oe = Expression.Constant(1);

            var od = DynamicCSharpExpression.DynamicArgument(oe);

            var ue0 = DynamicCSharpExpression.DynamicNegate(oe);
            Assert.AreEqual(ExpressionType.Negate, ue0.OperationNodeType);
            Assert.AreSame(oe, ue0.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue0.Flags);

            var ud0 = DynamicCSharpExpression.DynamicNegate(od);
            Assert.AreEqual(ExpressionType.Negate, ud0.OperationNodeType);
            Assert.AreSame(od, ud0.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud0.Flags);

            var uf0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Negate, uf0.OperationNodeType);
            Assert.AreSame(od, uf0.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf0.Flags);

            var uc0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Negate, uc0.OperationNodeType);
            Assert.AreSame(od, uc0.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc0.Flags);
            Assert.AreEqual(typeof(int), uc0.Context);

            var ue1 = DynamicCSharpExpression.DynamicUnaryPlus(oe);
            Assert.AreEqual(ExpressionType.UnaryPlus, ue1.OperationNodeType);
            Assert.AreSame(oe, ue1.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue1.Flags);

            var ud1 = DynamicCSharpExpression.DynamicUnaryPlus(od);
            Assert.AreEqual(ExpressionType.UnaryPlus, ud1.OperationNodeType);
            Assert.AreSame(od, ud1.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud1.Flags);

            var uf1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.UnaryPlus, uf1.OperationNodeType);
            Assert.AreSame(od, uf1.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf1.Flags);

            var uc1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.UnaryPlus, uc1.OperationNodeType);
            Assert.AreSame(od, uc1.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc1.Flags);
            Assert.AreEqual(typeof(int), uc1.Context);

            var ue2 = DynamicCSharpExpression.DynamicNegateChecked(oe);
            Assert.AreEqual(ExpressionType.NegateChecked, ue2.OperationNodeType);
            Assert.AreSame(oe, ue2.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, ue2.Flags);

            var ud2 = DynamicCSharpExpression.DynamicNegateChecked(od);
            Assert.AreEqual(ExpressionType.NegateChecked, ud2.OperationNodeType);
            Assert.AreSame(od, ud2.Operand);
			Assert.AreEqual(CSharpBinderFlags.CheckedContext, ud2.Flags);

            var uf2 = DynamicCSharpExpression.DynamicNegateChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.NegateChecked, uf2.OperationNodeType);
            Assert.AreSame(od, uf2.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf2.Flags);

            var uc2 = DynamicCSharpExpression.DynamicNegateChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.NegateChecked, uc2.OperationNodeType);
            Assert.AreSame(od, uc2.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc2.Flags);
            Assert.AreEqual(typeof(int), uc2.Context);

            var ue3 = DynamicCSharpExpression.DynamicNot(oe);
            Assert.AreEqual(ExpressionType.Not, ue3.OperationNodeType);
            Assert.AreSame(oe, ue3.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue3.Flags);

            var ud3 = DynamicCSharpExpression.DynamicNot(od);
            Assert.AreEqual(ExpressionType.Not, ud3.OperationNodeType);
            Assert.AreSame(od, ud3.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud3.Flags);

            var uf3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Not, uf3.OperationNodeType);
            Assert.AreSame(od, uf3.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf3.Flags);

            var uc3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Not, uc3.OperationNodeType);
            Assert.AreSame(od, uc3.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc3.Flags);
            Assert.AreEqual(typeof(int), uc3.Context);

            var ue4 = DynamicCSharpExpression.DynamicDecrement(oe);
            Assert.AreEqual(ExpressionType.Decrement, ue4.OperationNodeType);
            Assert.AreSame(oe, ue4.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue4.Flags);

            var ud4 = DynamicCSharpExpression.DynamicDecrement(od);
            Assert.AreEqual(ExpressionType.Decrement, ud4.OperationNodeType);
            Assert.AreSame(od, ud4.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud4.Flags);

            var uf4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Decrement, uf4.OperationNodeType);
            Assert.AreSame(od, uf4.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf4.Flags);

            var uc4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Decrement, uc4.OperationNodeType);
            Assert.AreSame(od, uc4.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc4.Flags);
            Assert.AreEqual(typeof(int), uc4.Context);

            var ue5 = DynamicCSharpExpression.DynamicIncrement(oe);
            Assert.AreEqual(ExpressionType.Increment, ue5.OperationNodeType);
            Assert.AreSame(oe, ue5.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue5.Flags);

            var ud5 = DynamicCSharpExpression.DynamicIncrement(od);
            Assert.AreEqual(ExpressionType.Increment, ud5.OperationNodeType);
            Assert.AreSame(od, ud5.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud5.Flags);

            var uf5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.Increment, uf5.OperationNodeType);
            Assert.AreSame(od, uf5.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf5.Flags);

            var uc5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.Increment, uc5.OperationNodeType);
            Assert.AreSame(od, uc5.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc5.Flags);
            Assert.AreEqual(typeof(int), uc5.Context);

            var ue6 = DynamicCSharpExpression.DynamicOnesComplement(oe);
            Assert.AreEqual(ExpressionType.OnesComplement, ue6.OperationNodeType);
            Assert.AreSame(oe, ue6.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue6.Flags);

            var ud6 = DynamicCSharpExpression.DynamicOnesComplement(od);
            Assert.AreEqual(ExpressionType.OnesComplement, ud6.OperationNodeType);
            Assert.AreSame(od, ud6.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud6.Flags);

            var uf6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.OnesComplement, uf6.OperationNodeType);
            Assert.AreSame(od, uf6.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf6.Flags);

            var uc6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.OnesComplement, uc6.OperationNodeType);
            Assert.AreSame(od, uc6.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc6.Flags);
            Assert.AreEqual(typeof(int), uc6.Context);

            var ue7 = DynamicCSharpExpression.DynamicIsTrue(oe);
            Assert.AreEqual(ExpressionType.IsTrue, ue7.OperationNodeType);
            Assert.AreSame(oe, ue7.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue7.Flags);

            var ud7 = DynamicCSharpExpression.DynamicIsTrue(od);
            Assert.AreEqual(ExpressionType.IsTrue, ud7.OperationNodeType);
            Assert.AreSame(od, ud7.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud7.Flags);

            var uf7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.IsTrue, uf7.OperationNodeType);
            Assert.AreSame(od, uf7.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf7.Flags);

            var uc7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.IsTrue, uc7.OperationNodeType);
            Assert.AreSame(od, uc7.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc7.Flags);
            Assert.AreEqual(typeof(int), uc7.Context);

            var ue8 = DynamicCSharpExpression.DynamicIsFalse(oe);
            Assert.AreEqual(ExpressionType.IsFalse, ue8.OperationNodeType);
            Assert.AreSame(oe, ue8.Operand.Expression);
			Assert.AreEqual(CSharpBinderFlags.None, ue8.Flags);

            var ud8 = DynamicCSharpExpression.DynamicIsFalse(od);
            Assert.AreEqual(ExpressionType.IsFalse, ud8.OperationNodeType);
            Assert.AreSame(od, ud8.Operand);
			Assert.AreEqual(CSharpBinderFlags.None, ud8.Flags);

            var uf8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.None);
            Assert.AreEqual(ExpressionType.IsFalse, uf8.OperationNodeType);
            Assert.AreSame(od, uf8.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uf8.Flags);

            var uc8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.None, typeof(int));
            Assert.AreEqual(ExpressionType.IsFalse, uc8.OperationNodeType);
            Assert.AreSame(od, uc8.Operand);
            Assert.AreEqual(CSharpBinderFlags.None, uc8.Flags);
            Assert.AreEqual(typeof(int), uc8.Context);

        }
    }
}