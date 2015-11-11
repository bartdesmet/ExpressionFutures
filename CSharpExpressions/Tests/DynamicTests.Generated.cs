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

            var bd0 = DynamicCSharpExpression.DynamicAdd(ld, rd);
            Assert.AreEqual(ExpressionType.Add, bd0.OperationNodeType);
            Assert.AreSame(ld, bd0.Left);
            Assert.AreSame(rd, bd0.Right);

            var bf0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Add, bf0.OperationNodeType);
            Assert.AreSame(ld, bf0.Left);
            Assert.AreSame(rd, bf0.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf0.Flags);

            var bc0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Add, bc0.OperationNodeType);
            Assert.AreSame(ld, bc0.Left);
            Assert.AreSame(rd, bc0.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc0.Flags);
            Assert.AreEqual(typeof(int), bc0.Context);

            var be1 = DynamicCSharpExpression.DynamicAddChecked(le, re);
            Assert.AreEqual(ExpressionType.AddChecked, be1.OperationNodeType);
            Assert.AreSame(le, be1.Left.Expression);
            Assert.AreSame(re, be1.Right.Expression);

            var bd1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd);
            Assert.AreEqual(ExpressionType.AddChecked, bd1.OperationNodeType);
            Assert.AreSame(ld, bd1.Left);
            Assert.AreSame(rd, bd1.Right);

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

            var bd2 = DynamicCSharpExpression.DynamicAnd(ld, rd);
            Assert.AreEqual(ExpressionType.And, bd2.OperationNodeType);
            Assert.AreSame(ld, bd2.Left);
            Assert.AreSame(rd, bd2.Right);

            var bf2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.And, bf2.OperationNodeType);
            Assert.AreSame(ld, bf2.Left);
            Assert.AreSame(rd, bf2.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf2.Flags);

            var bc2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.And, bc2.OperationNodeType);
            Assert.AreSame(ld, bc2.Left);
            Assert.AreSame(rd, bc2.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc2.Flags);
            Assert.AreEqual(typeof(int), bc2.Context);

            var be3 = DynamicCSharpExpression.DynamicAndAlso(le, re);
            Assert.AreEqual(ExpressionType.AndAlso, be3.OperationNodeType);
            Assert.AreSame(le, be3.Left.Expression);
            Assert.AreSame(re, be3.Right.Expression);

            var bd3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd);
            Assert.AreEqual(ExpressionType.AndAlso, bd3.OperationNodeType);
            Assert.AreSame(ld, bd3.Left);
            Assert.AreSame(rd, bd3.Right);

            var bf3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.AndAlso, bf3.OperationNodeType);
            Assert.AreSame(ld, bf3.Left);
            Assert.AreSame(rd, bf3.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf3.Flags);

            var bc3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.AndAlso, bc3.OperationNodeType);
            Assert.AreSame(ld, bc3.Left);
            Assert.AreSame(rd, bc3.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc3.Flags);
            Assert.AreEqual(typeof(int), bc3.Context);

            var be4 = DynamicCSharpExpression.DynamicArrayIndex(le, re);
            Assert.AreEqual(ExpressionType.ArrayIndex, be4.OperationNodeType);
            Assert.AreSame(le, be4.Left.Expression);
            Assert.AreSame(re, be4.Right.Expression);

            var bd4 = DynamicCSharpExpression.DynamicArrayIndex(ld, rd);
            Assert.AreEqual(ExpressionType.ArrayIndex, bd4.OperationNodeType);
            Assert.AreSame(ld, bd4.Left);
            Assert.AreSame(rd, bd4.Right);

            var bf4 = DynamicCSharpExpression.DynamicArrayIndex(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.ArrayIndex, bf4.OperationNodeType);
            Assert.AreSame(ld, bf4.Left);
            Assert.AreSame(rd, bf4.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf4.Flags);

            var bc4 = DynamicCSharpExpression.DynamicArrayIndex(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.ArrayIndex, bc4.OperationNodeType);
            Assert.AreSame(ld, bc4.Left);
            Assert.AreSame(rd, bc4.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc4.Flags);
            Assert.AreEqual(typeof(int), bc4.Context);

            var be5 = DynamicCSharpExpression.DynamicCoalesce(le, re);
            Assert.AreEqual(ExpressionType.Coalesce, be5.OperationNodeType);
            Assert.AreSame(le, be5.Left.Expression);
            Assert.AreSame(re, be5.Right.Expression);

            var bd5 = DynamicCSharpExpression.DynamicCoalesce(ld, rd);
            Assert.AreEqual(ExpressionType.Coalesce, bd5.OperationNodeType);
            Assert.AreSame(ld, bd5.Left);
            Assert.AreSame(rd, bd5.Right);

            var bf5 = DynamicCSharpExpression.DynamicCoalesce(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Coalesce, bf5.OperationNodeType);
            Assert.AreSame(ld, bf5.Left);
            Assert.AreSame(rd, bf5.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf5.Flags);

            var bc5 = DynamicCSharpExpression.DynamicCoalesce(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Coalesce, bc5.OperationNodeType);
            Assert.AreSame(ld, bc5.Left);
            Assert.AreSame(rd, bc5.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc5.Flags);
            Assert.AreEqual(typeof(int), bc5.Context);

            var be6 = DynamicCSharpExpression.DynamicDivide(le, re);
            Assert.AreEqual(ExpressionType.Divide, be6.OperationNodeType);
            Assert.AreSame(le, be6.Left.Expression);
            Assert.AreSame(re, be6.Right.Expression);

            var bd6 = DynamicCSharpExpression.DynamicDivide(ld, rd);
            Assert.AreEqual(ExpressionType.Divide, bd6.OperationNodeType);
            Assert.AreSame(ld, bd6.Left);
            Assert.AreSame(rd, bd6.Right);

            var bf6 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Divide, bf6.OperationNodeType);
            Assert.AreSame(ld, bf6.Left);
            Assert.AreSame(rd, bf6.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf6.Flags);

            var bc6 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Divide, bc6.OperationNodeType);
            Assert.AreSame(ld, bc6.Left);
            Assert.AreSame(rd, bc6.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc6.Flags);
            Assert.AreEqual(typeof(int), bc6.Context);

            var be7 = DynamicCSharpExpression.DynamicEqual(le, re);
            Assert.AreEqual(ExpressionType.Equal, be7.OperationNodeType);
            Assert.AreSame(le, be7.Left.Expression);
            Assert.AreSame(re, be7.Right.Expression);

            var bd7 = DynamicCSharpExpression.DynamicEqual(ld, rd);
            Assert.AreEqual(ExpressionType.Equal, bd7.OperationNodeType);
            Assert.AreSame(ld, bd7.Left);
            Assert.AreSame(rd, bd7.Right);

            var bf7 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Equal, bf7.OperationNodeType);
            Assert.AreSame(ld, bf7.Left);
            Assert.AreSame(rd, bf7.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf7.Flags);

            var bc7 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Equal, bc7.OperationNodeType);
            Assert.AreSame(ld, bc7.Left);
            Assert.AreSame(rd, bc7.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc7.Flags);
            Assert.AreEqual(typeof(int), bc7.Context);

            var be8 = DynamicCSharpExpression.DynamicExclusiveOr(le, re);
            Assert.AreEqual(ExpressionType.ExclusiveOr, be8.OperationNodeType);
            Assert.AreSame(le, be8.Left.Expression);
            Assert.AreSame(re, be8.Right.Expression);

            var bd8 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd);
            Assert.AreEqual(ExpressionType.ExclusiveOr, bd8.OperationNodeType);
            Assert.AreSame(ld, bd8.Left);
            Assert.AreSame(rd, bd8.Right);

            var bf8 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.ExclusiveOr, bf8.OperationNodeType);
            Assert.AreSame(ld, bf8.Left);
            Assert.AreSame(rd, bf8.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf8.Flags);

            var bc8 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.ExclusiveOr, bc8.OperationNodeType);
            Assert.AreSame(ld, bc8.Left);
            Assert.AreSame(rd, bc8.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc8.Flags);
            Assert.AreEqual(typeof(int), bc8.Context);

            var be9 = DynamicCSharpExpression.DynamicGreaterThan(le, re);
            Assert.AreEqual(ExpressionType.GreaterThan, be9.OperationNodeType);
            Assert.AreSame(le, be9.Left.Expression);
            Assert.AreSame(re, be9.Right.Expression);

            var bd9 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd);
            Assert.AreEqual(ExpressionType.GreaterThan, bd9.OperationNodeType);
            Assert.AreSame(ld, bd9.Left);
            Assert.AreSame(rd, bd9.Right);

            var bf9 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.GreaterThan, bf9.OperationNodeType);
            Assert.AreSame(ld, bf9.Left);
            Assert.AreSame(rd, bf9.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf9.Flags);

            var bc9 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.GreaterThan, bc9.OperationNodeType);
            Assert.AreSame(ld, bc9.Left);
            Assert.AreSame(rd, bc9.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc9.Flags);
            Assert.AreEqual(typeof(int), bc9.Context);

            var be10 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(le, re);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, be10.OperationNodeType);
            Assert.AreSame(le, be10.Left.Expression);
            Assert.AreSame(re, be10.Right.Expression);

            var bd10 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bd10.OperationNodeType);
            Assert.AreSame(ld, bd10.Left);
            Assert.AreSame(rd, bd10.Right);

            var bf10 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bf10.OperationNodeType);
            Assert.AreSame(ld, bf10.Left);
            Assert.AreSame(rd, bf10.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf10.Flags);

            var bc10 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, bc10.OperationNodeType);
            Assert.AreSame(ld, bc10.Left);
            Assert.AreSame(rd, bc10.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc10.Flags);
            Assert.AreEqual(typeof(int), bc10.Context);

            var be11 = DynamicCSharpExpression.DynamicLeftShift(le, re);
            Assert.AreEqual(ExpressionType.LeftShift, be11.OperationNodeType);
            Assert.AreSame(le, be11.Left.Expression);
            Assert.AreSame(re, be11.Right.Expression);

            var bd11 = DynamicCSharpExpression.DynamicLeftShift(ld, rd);
            Assert.AreEqual(ExpressionType.LeftShift, bd11.OperationNodeType);
            Assert.AreSame(ld, bd11.Left);
            Assert.AreSame(rd, bd11.Right);

            var bf11 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.LeftShift, bf11.OperationNodeType);
            Assert.AreSame(ld, bf11.Left);
            Assert.AreSame(rd, bf11.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf11.Flags);

            var bc11 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.LeftShift, bc11.OperationNodeType);
            Assert.AreSame(ld, bc11.Left);
            Assert.AreSame(rd, bc11.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc11.Flags);
            Assert.AreEqual(typeof(int), bc11.Context);

            var be12 = DynamicCSharpExpression.DynamicLessThan(le, re);
            Assert.AreEqual(ExpressionType.LessThan, be12.OperationNodeType);
            Assert.AreSame(le, be12.Left.Expression);
            Assert.AreSame(re, be12.Right.Expression);

            var bd12 = DynamicCSharpExpression.DynamicLessThan(ld, rd);
            Assert.AreEqual(ExpressionType.LessThan, bd12.OperationNodeType);
            Assert.AreSame(ld, bd12.Left);
            Assert.AreSame(rd, bd12.Right);

            var bf12 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.LessThan, bf12.OperationNodeType);
            Assert.AreSame(ld, bf12.Left);
            Assert.AreSame(rd, bf12.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf12.Flags);

            var bc12 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.LessThan, bc12.OperationNodeType);
            Assert.AreSame(ld, bc12.Left);
            Assert.AreSame(rd, bc12.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc12.Flags);
            Assert.AreEqual(typeof(int), bc12.Context);

            var be13 = DynamicCSharpExpression.DynamicLessThanOrEqual(le, re);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, be13.OperationNodeType);
            Assert.AreSame(le, be13.Left.Expression);
            Assert.AreSame(re, be13.Right.Expression);

            var bd13 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bd13.OperationNodeType);
            Assert.AreSame(ld, bd13.Left);
            Assert.AreSame(rd, bd13.Right);

            var bf13 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bf13.OperationNodeType);
            Assert.AreSame(ld, bf13.Left);
            Assert.AreSame(rd, bf13.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf13.Flags);

            var bc13 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.LessThanOrEqual, bc13.OperationNodeType);
            Assert.AreSame(ld, bc13.Left);
            Assert.AreSame(rd, bc13.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc13.Flags);
            Assert.AreEqual(typeof(int), bc13.Context);

            var be14 = DynamicCSharpExpression.DynamicModulo(le, re);
            Assert.AreEqual(ExpressionType.Modulo, be14.OperationNodeType);
            Assert.AreSame(le, be14.Left.Expression);
            Assert.AreSame(re, be14.Right.Expression);

            var bd14 = DynamicCSharpExpression.DynamicModulo(ld, rd);
            Assert.AreEqual(ExpressionType.Modulo, bd14.OperationNodeType);
            Assert.AreSame(ld, bd14.Left);
            Assert.AreSame(rd, bd14.Right);

            var bf14 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Modulo, bf14.OperationNodeType);
            Assert.AreSame(ld, bf14.Left);
            Assert.AreSame(rd, bf14.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf14.Flags);

            var bc14 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Modulo, bc14.OperationNodeType);
            Assert.AreSame(ld, bc14.Left);
            Assert.AreSame(rd, bc14.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc14.Flags);
            Assert.AreEqual(typeof(int), bc14.Context);

            var be15 = DynamicCSharpExpression.DynamicMultiply(le, re);
            Assert.AreEqual(ExpressionType.Multiply, be15.OperationNodeType);
            Assert.AreSame(le, be15.Left.Expression);
            Assert.AreSame(re, be15.Right.Expression);

            var bd15 = DynamicCSharpExpression.DynamicMultiply(ld, rd);
            Assert.AreEqual(ExpressionType.Multiply, bd15.OperationNodeType);
            Assert.AreSame(ld, bd15.Left);
            Assert.AreSame(rd, bd15.Right);

            var bf15 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Multiply, bf15.OperationNodeType);
            Assert.AreSame(ld, bf15.Left);
            Assert.AreSame(rd, bf15.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf15.Flags);

            var bc15 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Multiply, bc15.OperationNodeType);
            Assert.AreSame(ld, bc15.Left);
            Assert.AreSame(rd, bc15.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc15.Flags);
            Assert.AreEqual(typeof(int), bc15.Context);

            var be16 = DynamicCSharpExpression.DynamicMultiplyChecked(le, re);
            Assert.AreEqual(ExpressionType.MultiplyChecked, be16.OperationNodeType);
            Assert.AreSame(le, be16.Left.Expression);
            Assert.AreSame(re, be16.Right.Expression);

            var bd16 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd);
            Assert.AreEqual(ExpressionType.MultiplyChecked, bd16.OperationNodeType);
            Assert.AreSame(ld, bd16.Left);
            Assert.AreSame(rd, bd16.Right);

            var bf16 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.MultiplyChecked, bf16.OperationNodeType);
            Assert.AreSame(ld, bf16.Left);
            Assert.AreSame(rd, bf16.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf16.Flags);

            var bc16 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.MultiplyChecked, bc16.OperationNodeType);
            Assert.AreSame(ld, bc16.Left);
            Assert.AreSame(rd, bc16.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc16.Flags);
            Assert.AreEqual(typeof(int), bc16.Context);

            var be17 = DynamicCSharpExpression.DynamicNotEqual(le, re);
            Assert.AreEqual(ExpressionType.NotEqual, be17.OperationNodeType);
            Assert.AreSame(le, be17.Left.Expression);
            Assert.AreSame(re, be17.Right.Expression);

            var bd17 = DynamicCSharpExpression.DynamicNotEqual(ld, rd);
            Assert.AreEqual(ExpressionType.NotEqual, bd17.OperationNodeType);
            Assert.AreSame(ld, bd17.Left);
            Assert.AreSame(rd, bd17.Right);

            var bf17 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.NotEqual, bf17.OperationNodeType);
            Assert.AreSame(ld, bf17.Left);
            Assert.AreSame(rd, bf17.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf17.Flags);

            var bc17 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.NotEqual, bc17.OperationNodeType);
            Assert.AreSame(ld, bc17.Left);
            Assert.AreSame(rd, bc17.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc17.Flags);
            Assert.AreEqual(typeof(int), bc17.Context);

            var be18 = DynamicCSharpExpression.DynamicOr(le, re);
            Assert.AreEqual(ExpressionType.Or, be18.OperationNodeType);
            Assert.AreSame(le, be18.Left.Expression);
            Assert.AreSame(re, be18.Right.Expression);

            var bd18 = DynamicCSharpExpression.DynamicOr(ld, rd);
            Assert.AreEqual(ExpressionType.Or, bd18.OperationNodeType);
            Assert.AreSame(ld, bd18.Left);
            Assert.AreSame(rd, bd18.Right);

            var bf18 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Or, bf18.OperationNodeType);
            Assert.AreSame(ld, bf18.Left);
            Assert.AreSame(rd, bf18.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf18.Flags);

            var bc18 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Or, bc18.OperationNodeType);
            Assert.AreSame(ld, bc18.Left);
            Assert.AreSame(rd, bc18.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc18.Flags);
            Assert.AreEqual(typeof(int), bc18.Context);

            var be19 = DynamicCSharpExpression.DynamicOrElse(le, re);
            Assert.AreEqual(ExpressionType.OrElse, be19.OperationNodeType);
            Assert.AreSame(le, be19.Left.Expression);
            Assert.AreSame(re, be19.Right.Expression);

            var bd19 = DynamicCSharpExpression.DynamicOrElse(ld, rd);
            Assert.AreEqual(ExpressionType.OrElse, bd19.OperationNodeType);
            Assert.AreSame(ld, bd19.Left);
            Assert.AreSame(rd, bd19.Right);

            var bf19 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.OrElse, bf19.OperationNodeType);
            Assert.AreSame(ld, bf19.Left);
            Assert.AreSame(rd, bf19.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf19.Flags);

            var bc19 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.OrElse, bc19.OperationNodeType);
            Assert.AreSame(ld, bc19.Left);
            Assert.AreSame(rd, bc19.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc19.Flags);
            Assert.AreEqual(typeof(int), bc19.Context);

            var be20 = DynamicCSharpExpression.DynamicPower(le, re);
            Assert.AreEqual(ExpressionType.Power, be20.OperationNodeType);
            Assert.AreSame(le, be20.Left.Expression);
            Assert.AreSame(re, be20.Right.Expression);

            var bd20 = DynamicCSharpExpression.DynamicPower(ld, rd);
            Assert.AreEqual(ExpressionType.Power, bd20.OperationNodeType);
            Assert.AreSame(ld, bd20.Left);
            Assert.AreSame(rd, bd20.Right);

            var bf20 = DynamicCSharpExpression.DynamicPower(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Power, bf20.OperationNodeType);
            Assert.AreSame(ld, bf20.Left);
            Assert.AreSame(rd, bf20.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf20.Flags);

            var bc20 = DynamicCSharpExpression.DynamicPower(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Power, bc20.OperationNodeType);
            Assert.AreSame(ld, bc20.Left);
            Assert.AreSame(rd, bc20.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc20.Flags);
            Assert.AreEqual(typeof(int), bc20.Context);

            var be21 = DynamicCSharpExpression.DynamicRightShift(le, re);
            Assert.AreEqual(ExpressionType.RightShift, be21.OperationNodeType);
            Assert.AreSame(le, be21.Left.Expression);
            Assert.AreSame(re, be21.Right.Expression);

            var bd21 = DynamicCSharpExpression.DynamicRightShift(ld, rd);
            Assert.AreEqual(ExpressionType.RightShift, bd21.OperationNodeType);
            Assert.AreSame(ld, bd21.Left);
            Assert.AreSame(rd, bd21.Right);

            var bf21 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.RightShift, bf21.OperationNodeType);
            Assert.AreSame(ld, bf21.Left);
            Assert.AreSame(rd, bf21.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf21.Flags);

            var bc21 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.RightShift, bc21.OperationNodeType);
            Assert.AreSame(ld, bc21.Left);
            Assert.AreSame(rd, bc21.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc21.Flags);
            Assert.AreEqual(typeof(int), bc21.Context);

            var be22 = DynamicCSharpExpression.DynamicSubtract(le, re);
            Assert.AreEqual(ExpressionType.Subtract, be22.OperationNodeType);
            Assert.AreSame(le, be22.Left.Expression);
            Assert.AreSame(re, be22.Right.Expression);

            var bd22 = DynamicCSharpExpression.DynamicSubtract(ld, rd);
            Assert.AreEqual(ExpressionType.Subtract, bd22.OperationNodeType);
            Assert.AreSame(ld, bd22.Left);
            Assert.AreSame(rd, bd22.Right);

            var bf22 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Subtract, bf22.OperationNodeType);
            Assert.AreSame(ld, bf22.Left);
            Assert.AreSame(rd, bf22.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf22.Flags);

            var bc22 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Subtract, bc22.OperationNodeType);
            Assert.AreSame(ld, bc22.Left);
            Assert.AreSame(rd, bc22.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc22.Flags);
            Assert.AreEqual(typeof(int), bc22.Context);

            var be23 = DynamicCSharpExpression.DynamicSubtractChecked(le, re);
            Assert.AreEqual(ExpressionType.SubtractChecked, be23.OperationNodeType);
            Assert.AreSame(le, be23.Left.Expression);
            Assert.AreSame(re, be23.Right.Expression);

            var bd23 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd);
            Assert.AreEqual(ExpressionType.SubtractChecked, bd23.OperationNodeType);
            Assert.AreSame(ld, bd23.Left);
            Assert.AreSame(rd, bd23.Right);

            var bf23 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.SubtractChecked, bf23.OperationNodeType);
            Assert.AreSame(ld, bf23.Left);
            Assert.AreSame(rd, bf23.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bf23.Flags);

            var bc23 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.SubtractChecked, bc23.OperationNodeType);
            Assert.AreSame(ld, bc23.Left);
            Assert.AreSame(rd, bc23.Right);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, bc23.Flags);
            Assert.AreEqual(typeof(int), bc23.Context);

        }

        [TestMethod]
        public void Dynamic_Unary_GeneratedFactories()
        {
            var oe = Expression.Constant(1);

            var od = DynamicCSharpExpression.DynamicArgument(oe);

            var ue0 = DynamicCSharpExpression.DynamicNegate(oe);
            Assert.AreEqual(ExpressionType.Negate, ue0.OperationNodeType);
            Assert.AreSame(oe, ue0.Operand.Expression);

            var ud0 = DynamicCSharpExpression.DynamicNegate(od);
            Assert.AreEqual(ExpressionType.Negate, ud0.OperationNodeType);
            Assert.AreSame(od, ud0.Operand);

            var uf0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Negate, uf0.OperationNodeType);
            Assert.AreSame(od, uf0.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf0.Flags);

            var uc0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Negate, uc0.OperationNodeType);
            Assert.AreSame(od, uc0.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc0.Flags);
            Assert.AreEqual(typeof(int), uc0.Context);
            var ue1 = DynamicCSharpExpression.DynamicUnaryPlus(oe);
            Assert.AreEqual(ExpressionType.UnaryPlus, ue1.OperationNodeType);
            Assert.AreSame(oe, ue1.Operand.Expression);

            var ud1 = DynamicCSharpExpression.DynamicUnaryPlus(od);
            Assert.AreEqual(ExpressionType.UnaryPlus, ud1.OperationNodeType);
            Assert.AreSame(od, ud1.Operand);

            var uf1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.UnaryPlus, uf1.OperationNodeType);
            Assert.AreSame(od, uf1.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf1.Flags);

            var uc1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.UnaryPlus, uc1.OperationNodeType);
            Assert.AreSame(od, uc1.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc1.Flags);
            Assert.AreEqual(typeof(int), uc1.Context);
            var ue2 = DynamicCSharpExpression.DynamicNegateChecked(oe);
            Assert.AreEqual(ExpressionType.NegateChecked, ue2.OperationNodeType);
            Assert.AreSame(oe, ue2.Operand.Expression);

            var ud2 = DynamicCSharpExpression.DynamicNegateChecked(od);
            Assert.AreEqual(ExpressionType.NegateChecked, ud2.OperationNodeType);
            Assert.AreSame(od, ud2.Operand);

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

            var ud3 = DynamicCSharpExpression.DynamicNot(od);
            Assert.AreEqual(ExpressionType.Not, ud3.OperationNodeType);
            Assert.AreSame(od, ud3.Operand);

            var uf3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Not, uf3.OperationNodeType);
            Assert.AreSame(od, uf3.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf3.Flags);

            var uc3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Not, uc3.OperationNodeType);
            Assert.AreSame(od, uc3.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc3.Flags);
            Assert.AreEqual(typeof(int), uc3.Context);
            var ue4 = DynamicCSharpExpression.DynamicDecrement(oe);
            Assert.AreEqual(ExpressionType.Decrement, ue4.OperationNodeType);
            Assert.AreSame(oe, ue4.Operand.Expression);

            var ud4 = DynamicCSharpExpression.DynamicDecrement(od);
            Assert.AreEqual(ExpressionType.Decrement, ud4.OperationNodeType);
            Assert.AreSame(od, ud4.Operand);

            var uf4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Decrement, uf4.OperationNodeType);
            Assert.AreSame(od, uf4.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf4.Flags);

            var uc4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Decrement, uc4.OperationNodeType);
            Assert.AreSame(od, uc4.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc4.Flags);
            Assert.AreEqual(typeof(int), uc4.Context);
            var ue5 = DynamicCSharpExpression.DynamicIncrement(oe);
            Assert.AreEqual(ExpressionType.Increment, ue5.OperationNodeType);
            Assert.AreSame(oe, ue5.Operand.Expression);

            var ud5 = DynamicCSharpExpression.DynamicIncrement(od);
            Assert.AreEqual(ExpressionType.Increment, ud5.OperationNodeType);
            Assert.AreSame(od, ud5.Operand);

            var uf5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.Increment, uf5.OperationNodeType);
            Assert.AreSame(od, uf5.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf5.Flags);

            var uc5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.Increment, uc5.OperationNodeType);
            Assert.AreSame(od, uc5.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc5.Flags);
            Assert.AreEqual(typeof(int), uc5.Context);
            var ue6 = DynamicCSharpExpression.DynamicOnesComplement(oe);
            Assert.AreEqual(ExpressionType.OnesComplement, ue6.OperationNodeType);
            Assert.AreSame(oe, ue6.Operand.Expression);

            var ud6 = DynamicCSharpExpression.DynamicOnesComplement(od);
            Assert.AreEqual(ExpressionType.OnesComplement, ud6.OperationNodeType);
            Assert.AreSame(od, ud6.Operand);

            var uf6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.OnesComplement, uf6.OperationNodeType);
            Assert.AreSame(od, uf6.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf6.Flags);

            var uc6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.OnesComplement, uc6.OperationNodeType);
            Assert.AreSame(od, uc6.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc6.Flags);
            Assert.AreEqual(typeof(int), uc6.Context);
            var ue7 = DynamicCSharpExpression.DynamicIsTrue(oe);
            Assert.AreEqual(ExpressionType.IsTrue, ue7.OperationNodeType);
            Assert.AreSame(oe, ue7.Operand.Expression);

            var ud7 = DynamicCSharpExpression.DynamicIsTrue(od);
            Assert.AreEqual(ExpressionType.IsTrue, ud7.OperationNodeType);
            Assert.AreSame(od, ud7.Operand);

            var uf7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.IsTrue, uf7.OperationNodeType);
            Assert.AreSame(od, uf7.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf7.Flags);

            var uc7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.IsTrue, uc7.OperationNodeType);
            Assert.AreSame(od, uc7.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc7.Flags);
            Assert.AreEqual(typeof(int), uc7.Context);
            var ue8 = DynamicCSharpExpression.DynamicIsFalse(oe);
            Assert.AreEqual(ExpressionType.IsFalse, ue8.OperationNodeType);
            Assert.AreSame(oe, ue8.Operand.Expression);

            var ud8 = DynamicCSharpExpression.DynamicIsFalse(od);
            Assert.AreEqual(ExpressionType.IsFalse, ud8.OperationNodeType);
            Assert.AreSame(od, ud8.Operand);

            var uf8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.CheckedContext);
            Assert.AreEqual(ExpressionType.IsFalse, uf8.OperationNodeType);
            Assert.AreSame(od, uf8.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uf8.Flags);

            var uc8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.AreEqual(ExpressionType.IsFalse, uc8.OperationNodeType);
            Assert.AreSame(od, uc8.Operand);
            Assert.AreEqual(CSharpBinderFlags.CheckedContext, uc8.Flags);
            Assert.AreEqual(typeof(int), uc8.Context);
        }
    }
}