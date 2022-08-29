// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    partial class DynamicTests
    {
        [Fact]
        public void Dynamic_Binary_GeneratedFactories()
        {
            var le = Expression.Constant(1);
            var re = Expression.Constant(2);

            var ld = DynamicCSharpExpression.DynamicArgument(le);
            var rd = DynamicCSharpExpression.DynamicArgument(re);

            var be0 = DynamicCSharpExpression.DynamicAdd(le, re);
            Assert.Equal(ExpressionType.Add, be0.OperationNodeType);
            Assert.Same(le, be0.Left.Expression);
            Assert.Same(re, be0.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be0.Flags);

            var bd0 = DynamicCSharpExpression.DynamicAdd(ld, rd);
            Assert.Equal(ExpressionType.Add, bd0.OperationNodeType);
            Assert.Same(ld, bd0.Left);
            Assert.Same(rd, bd0.Right);
            Assert.Equal(CSharpBinderFlags.None, bd0.Flags);

            var bf0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Add, bf0.OperationNodeType);
            Assert.Same(ld, bf0.Left);
            Assert.Same(rd, bf0.Right);
            Assert.Equal(CSharpBinderFlags.None, bf0.Flags);

            var bc0 = DynamicCSharpExpression.DynamicAdd(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Add, bc0.OperationNodeType);
            Assert.Same(ld, bc0.Left);
            Assert.Same(rd, bc0.Right);
            Assert.Equal(CSharpBinderFlags.None, bc0.Flags);
            Assert.Equal(typeof(int), bc0.Context);

            var be1 = DynamicCSharpExpression.DynamicAddChecked(le, re);
            Assert.Equal(ExpressionType.AddChecked, be1.OperationNodeType);
            Assert.Same(le, be1.Left.Expression);
            Assert.Same(re, be1.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be1.Flags);

            var bd1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd);
            Assert.Equal(ExpressionType.AddChecked, bd1.OperationNodeType);
            Assert.Same(ld, bd1.Left);
            Assert.Same(rd, bd1.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd1.Flags);

            var bf1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(ExpressionType.AddChecked, bf1.OperationNodeType);
            Assert.Same(ld, bf1.Left);
            Assert.Same(rd, bf1.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf1.Flags);

            var bc1 = DynamicCSharpExpression.DynamicAddChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(ExpressionType.AddChecked, bc1.OperationNodeType);
            Assert.Same(ld, bc1.Left);
            Assert.Same(rd, bc1.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc1.Flags);
            Assert.Equal(typeof(int), bc1.Context);

            var be2 = DynamicCSharpExpression.DynamicAnd(le, re);
            Assert.Equal(ExpressionType.And, be2.OperationNodeType);
            Assert.Same(le, be2.Left.Expression);
            Assert.Same(re, be2.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be2.Flags);

            var bd2 = DynamicCSharpExpression.DynamicAnd(ld, rd);
            Assert.Equal(ExpressionType.And, bd2.OperationNodeType);
            Assert.Same(ld, bd2.Left);
            Assert.Same(rd, bd2.Right);
            Assert.Equal(CSharpBinderFlags.None, bd2.Flags);

            var bf2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.And, bf2.OperationNodeType);
            Assert.Same(ld, bf2.Left);
            Assert.Same(rd, bf2.Right);
            Assert.Equal(CSharpBinderFlags.None, bf2.Flags);

            var bc2 = DynamicCSharpExpression.DynamicAnd(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.And, bc2.OperationNodeType);
            Assert.Same(ld, bc2.Left);
            Assert.Same(rd, bc2.Right);
            Assert.Equal(CSharpBinderFlags.None, bc2.Flags);
            Assert.Equal(typeof(int), bc2.Context);

            var be3 = DynamicCSharpExpression.DynamicAndAlso(le, re);
            Assert.Equal(ExpressionType.AndAlso, be3.OperationNodeType);
            Assert.Same(le, be3.Left.Expression);
            Assert.Same(re, be3.Right.Expression);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, be3.Flags);

            var bd3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd);
            Assert.Equal(ExpressionType.AndAlso, bd3.OperationNodeType);
            Assert.Same(ld, bd3.Left);
            Assert.Same(rd, bd3.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bd3.Flags);

            var bf3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.BinaryOperationLogical);
            Assert.Equal(ExpressionType.AndAlso, bf3.OperationNodeType);
            Assert.Same(ld, bf3.Left);
            Assert.Same(rd, bf3.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bf3.Flags);

            var bc3 = DynamicCSharpExpression.DynamicAndAlso(ld, rd, CSharpBinderFlags.BinaryOperationLogical, typeof(int));
            Assert.Equal(ExpressionType.AndAlso, bc3.OperationNodeType);
            Assert.Same(ld, bc3.Left);
            Assert.Same(rd, bc3.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bc3.Flags);
            Assert.Equal(typeof(int), bc3.Context);

            var be4 = DynamicCSharpExpression.DynamicDivide(le, re);
            Assert.Equal(ExpressionType.Divide, be4.OperationNodeType);
            Assert.Same(le, be4.Left.Expression);
            Assert.Same(re, be4.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be4.Flags);

            var bd4 = DynamicCSharpExpression.DynamicDivide(ld, rd);
            Assert.Equal(ExpressionType.Divide, bd4.OperationNodeType);
            Assert.Same(ld, bd4.Left);
            Assert.Same(rd, bd4.Right);
            Assert.Equal(CSharpBinderFlags.None, bd4.Flags);

            var bf4 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Divide, bf4.OperationNodeType);
            Assert.Same(ld, bf4.Left);
            Assert.Same(rd, bf4.Right);
            Assert.Equal(CSharpBinderFlags.None, bf4.Flags);

            var bc4 = DynamicCSharpExpression.DynamicDivide(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Divide, bc4.OperationNodeType);
            Assert.Same(ld, bc4.Left);
            Assert.Same(rd, bc4.Right);
            Assert.Equal(CSharpBinderFlags.None, bc4.Flags);
            Assert.Equal(typeof(int), bc4.Context);

            var be5 = DynamicCSharpExpression.DynamicEqual(le, re);
            Assert.Equal(ExpressionType.Equal, be5.OperationNodeType);
            Assert.Same(le, be5.Left.Expression);
            Assert.Same(re, be5.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be5.Flags);

            var bd5 = DynamicCSharpExpression.DynamicEqual(ld, rd);
            Assert.Equal(ExpressionType.Equal, bd5.OperationNodeType);
            Assert.Same(ld, bd5.Left);
            Assert.Same(rd, bd5.Right);
            Assert.Equal(CSharpBinderFlags.None, bd5.Flags);

            var bf5 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Equal, bf5.OperationNodeType);
            Assert.Same(ld, bf5.Left);
            Assert.Same(rd, bf5.Right);
            Assert.Equal(CSharpBinderFlags.None, bf5.Flags);

            var bc5 = DynamicCSharpExpression.DynamicEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Equal, bc5.OperationNodeType);
            Assert.Same(ld, bc5.Left);
            Assert.Same(rd, bc5.Right);
            Assert.Equal(CSharpBinderFlags.None, bc5.Flags);
            Assert.Equal(typeof(int), bc5.Context);

            var be6 = DynamicCSharpExpression.DynamicExclusiveOr(le, re);
            Assert.Equal(ExpressionType.ExclusiveOr, be6.OperationNodeType);
            Assert.Same(le, be6.Left.Expression);
            Assert.Same(re, be6.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be6.Flags);

            var bd6 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd);
            Assert.Equal(ExpressionType.ExclusiveOr, bd6.OperationNodeType);
            Assert.Same(ld, bd6.Left);
            Assert.Same(rd, bd6.Right);
            Assert.Equal(CSharpBinderFlags.None, bd6.Flags);

            var bf6 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.ExclusiveOr, bf6.OperationNodeType);
            Assert.Same(ld, bf6.Left);
            Assert.Same(rd, bf6.Right);
            Assert.Equal(CSharpBinderFlags.None, bf6.Flags);

            var bc6 = DynamicCSharpExpression.DynamicExclusiveOr(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.ExclusiveOr, bc6.OperationNodeType);
            Assert.Same(ld, bc6.Left);
            Assert.Same(rd, bc6.Right);
            Assert.Equal(CSharpBinderFlags.None, bc6.Flags);
            Assert.Equal(typeof(int), bc6.Context);

            var be7 = DynamicCSharpExpression.DynamicGreaterThan(le, re);
            Assert.Equal(ExpressionType.GreaterThan, be7.OperationNodeType);
            Assert.Same(le, be7.Left.Expression);
            Assert.Same(re, be7.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be7.Flags);

            var bd7 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd);
            Assert.Equal(ExpressionType.GreaterThan, bd7.OperationNodeType);
            Assert.Same(ld, bd7.Left);
            Assert.Same(rd, bd7.Right);
            Assert.Equal(CSharpBinderFlags.None, bd7.Flags);

            var bf7 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.GreaterThan, bf7.OperationNodeType);
            Assert.Same(ld, bf7.Left);
            Assert.Same(rd, bf7.Right);
            Assert.Equal(CSharpBinderFlags.None, bf7.Flags);

            var bc7 = DynamicCSharpExpression.DynamicGreaterThan(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.GreaterThan, bc7.OperationNodeType);
            Assert.Same(ld, bc7.Left);
            Assert.Same(rd, bc7.Right);
            Assert.Equal(CSharpBinderFlags.None, bc7.Flags);
            Assert.Equal(typeof(int), bc7.Context);

            var be8 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(le, re);
            Assert.Equal(ExpressionType.GreaterThanOrEqual, be8.OperationNodeType);
            Assert.Same(le, be8.Left.Expression);
            Assert.Same(re, be8.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be8.Flags);

            var bd8 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd);
            Assert.Equal(ExpressionType.GreaterThanOrEqual, bd8.OperationNodeType);
            Assert.Same(ld, bd8.Left);
            Assert.Same(rd, bd8.Right);
            Assert.Equal(CSharpBinderFlags.None, bd8.Flags);

            var bf8 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.GreaterThanOrEqual, bf8.OperationNodeType);
            Assert.Same(ld, bf8.Left);
            Assert.Same(rd, bf8.Right);
            Assert.Equal(CSharpBinderFlags.None, bf8.Flags);

            var bc8 = DynamicCSharpExpression.DynamicGreaterThanOrEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.GreaterThanOrEqual, bc8.OperationNodeType);
            Assert.Same(ld, bc8.Left);
            Assert.Same(rd, bc8.Right);
            Assert.Equal(CSharpBinderFlags.None, bc8.Flags);
            Assert.Equal(typeof(int), bc8.Context);

            var be9 = DynamicCSharpExpression.DynamicLeftShift(le, re);
            Assert.Equal(ExpressionType.LeftShift, be9.OperationNodeType);
            Assert.Same(le, be9.Left.Expression);
            Assert.Same(re, be9.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be9.Flags);

            var bd9 = DynamicCSharpExpression.DynamicLeftShift(ld, rd);
            Assert.Equal(ExpressionType.LeftShift, bd9.OperationNodeType);
            Assert.Same(ld, bd9.Left);
            Assert.Same(rd, bd9.Right);
            Assert.Equal(CSharpBinderFlags.None, bd9.Flags);

            var bf9 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.LeftShift, bf9.OperationNodeType);
            Assert.Same(ld, bf9.Left);
            Assert.Same(rd, bf9.Right);
            Assert.Equal(CSharpBinderFlags.None, bf9.Flags);

            var bc9 = DynamicCSharpExpression.DynamicLeftShift(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.LeftShift, bc9.OperationNodeType);
            Assert.Same(ld, bc9.Left);
            Assert.Same(rd, bc9.Right);
            Assert.Equal(CSharpBinderFlags.None, bc9.Flags);
            Assert.Equal(typeof(int), bc9.Context);

            var be10 = DynamicCSharpExpression.DynamicLessThan(le, re);
            Assert.Equal(ExpressionType.LessThan, be10.OperationNodeType);
            Assert.Same(le, be10.Left.Expression);
            Assert.Same(re, be10.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be10.Flags);

            var bd10 = DynamicCSharpExpression.DynamicLessThan(ld, rd);
            Assert.Equal(ExpressionType.LessThan, bd10.OperationNodeType);
            Assert.Same(ld, bd10.Left);
            Assert.Same(rd, bd10.Right);
            Assert.Equal(CSharpBinderFlags.None, bd10.Flags);

            var bf10 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.LessThan, bf10.OperationNodeType);
            Assert.Same(ld, bf10.Left);
            Assert.Same(rd, bf10.Right);
            Assert.Equal(CSharpBinderFlags.None, bf10.Flags);

            var bc10 = DynamicCSharpExpression.DynamicLessThan(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.LessThan, bc10.OperationNodeType);
            Assert.Same(ld, bc10.Left);
            Assert.Same(rd, bc10.Right);
            Assert.Equal(CSharpBinderFlags.None, bc10.Flags);
            Assert.Equal(typeof(int), bc10.Context);

            var be11 = DynamicCSharpExpression.DynamicLessThanOrEqual(le, re);
            Assert.Equal(ExpressionType.LessThanOrEqual, be11.OperationNodeType);
            Assert.Same(le, be11.Left.Expression);
            Assert.Same(re, be11.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be11.Flags);

            var bd11 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd);
            Assert.Equal(ExpressionType.LessThanOrEqual, bd11.OperationNodeType);
            Assert.Same(ld, bd11.Left);
            Assert.Same(rd, bd11.Right);
            Assert.Equal(CSharpBinderFlags.None, bd11.Flags);

            var bf11 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.LessThanOrEqual, bf11.OperationNodeType);
            Assert.Same(ld, bf11.Left);
            Assert.Same(rd, bf11.Right);
            Assert.Equal(CSharpBinderFlags.None, bf11.Flags);

            var bc11 = DynamicCSharpExpression.DynamicLessThanOrEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.LessThanOrEqual, bc11.OperationNodeType);
            Assert.Same(ld, bc11.Left);
            Assert.Same(rd, bc11.Right);
            Assert.Equal(CSharpBinderFlags.None, bc11.Flags);
            Assert.Equal(typeof(int), bc11.Context);

            var be12 = DynamicCSharpExpression.DynamicModulo(le, re);
            Assert.Equal(ExpressionType.Modulo, be12.OperationNodeType);
            Assert.Same(le, be12.Left.Expression);
            Assert.Same(re, be12.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be12.Flags);

            var bd12 = DynamicCSharpExpression.DynamicModulo(ld, rd);
            Assert.Equal(ExpressionType.Modulo, bd12.OperationNodeType);
            Assert.Same(ld, bd12.Left);
            Assert.Same(rd, bd12.Right);
            Assert.Equal(CSharpBinderFlags.None, bd12.Flags);

            var bf12 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Modulo, bf12.OperationNodeType);
            Assert.Same(ld, bf12.Left);
            Assert.Same(rd, bf12.Right);
            Assert.Equal(CSharpBinderFlags.None, bf12.Flags);

            var bc12 = DynamicCSharpExpression.DynamicModulo(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Modulo, bc12.OperationNodeType);
            Assert.Same(ld, bc12.Left);
            Assert.Same(rd, bc12.Right);
            Assert.Equal(CSharpBinderFlags.None, bc12.Flags);
            Assert.Equal(typeof(int), bc12.Context);

            var be13 = DynamicCSharpExpression.DynamicMultiply(le, re);
            Assert.Equal(ExpressionType.Multiply, be13.OperationNodeType);
            Assert.Same(le, be13.Left.Expression);
            Assert.Same(re, be13.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be13.Flags);

            var bd13 = DynamicCSharpExpression.DynamicMultiply(ld, rd);
            Assert.Equal(ExpressionType.Multiply, bd13.OperationNodeType);
            Assert.Same(ld, bd13.Left);
            Assert.Same(rd, bd13.Right);
            Assert.Equal(CSharpBinderFlags.None, bd13.Flags);

            var bf13 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Multiply, bf13.OperationNodeType);
            Assert.Same(ld, bf13.Left);
            Assert.Same(rd, bf13.Right);
            Assert.Equal(CSharpBinderFlags.None, bf13.Flags);

            var bc13 = DynamicCSharpExpression.DynamicMultiply(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Multiply, bc13.OperationNodeType);
            Assert.Same(ld, bc13.Left);
            Assert.Same(rd, bc13.Right);
            Assert.Equal(CSharpBinderFlags.None, bc13.Flags);
            Assert.Equal(typeof(int), bc13.Context);

            var be14 = DynamicCSharpExpression.DynamicMultiplyChecked(le, re);
            Assert.Equal(ExpressionType.MultiplyChecked, be14.OperationNodeType);
            Assert.Same(le, be14.Left.Expression);
            Assert.Same(re, be14.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be14.Flags);

            var bd14 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd);
            Assert.Equal(ExpressionType.MultiplyChecked, bd14.OperationNodeType);
            Assert.Same(ld, bd14.Left);
            Assert.Same(rd, bd14.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd14.Flags);

            var bf14 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(ExpressionType.MultiplyChecked, bf14.OperationNodeType);
            Assert.Same(ld, bf14.Left);
            Assert.Same(rd, bf14.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf14.Flags);

            var bc14 = DynamicCSharpExpression.DynamicMultiplyChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(ExpressionType.MultiplyChecked, bc14.OperationNodeType);
            Assert.Same(ld, bc14.Left);
            Assert.Same(rd, bc14.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc14.Flags);
            Assert.Equal(typeof(int), bc14.Context);

            var be15 = DynamicCSharpExpression.DynamicNotEqual(le, re);
            Assert.Equal(ExpressionType.NotEqual, be15.OperationNodeType);
            Assert.Same(le, be15.Left.Expression);
            Assert.Same(re, be15.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be15.Flags);

            var bd15 = DynamicCSharpExpression.DynamicNotEqual(ld, rd);
            Assert.Equal(ExpressionType.NotEqual, bd15.OperationNodeType);
            Assert.Same(ld, bd15.Left);
            Assert.Same(rd, bd15.Right);
            Assert.Equal(CSharpBinderFlags.None, bd15.Flags);

            var bf15 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.NotEqual, bf15.OperationNodeType);
            Assert.Same(ld, bf15.Left);
            Assert.Same(rd, bf15.Right);
            Assert.Equal(CSharpBinderFlags.None, bf15.Flags);

            var bc15 = DynamicCSharpExpression.DynamicNotEqual(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.NotEqual, bc15.OperationNodeType);
            Assert.Same(ld, bc15.Left);
            Assert.Same(rd, bc15.Right);
            Assert.Equal(CSharpBinderFlags.None, bc15.Flags);
            Assert.Equal(typeof(int), bc15.Context);

            var be16 = DynamicCSharpExpression.DynamicOr(le, re);
            Assert.Equal(ExpressionType.Or, be16.OperationNodeType);
            Assert.Same(le, be16.Left.Expression);
            Assert.Same(re, be16.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be16.Flags);

            var bd16 = DynamicCSharpExpression.DynamicOr(ld, rd);
            Assert.Equal(ExpressionType.Or, bd16.OperationNodeType);
            Assert.Same(ld, bd16.Left);
            Assert.Same(rd, bd16.Right);
            Assert.Equal(CSharpBinderFlags.None, bd16.Flags);

            var bf16 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Or, bf16.OperationNodeType);
            Assert.Same(ld, bf16.Left);
            Assert.Same(rd, bf16.Right);
            Assert.Equal(CSharpBinderFlags.None, bf16.Flags);

            var bc16 = DynamicCSharpExpression.DynamicOr(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Or, bc16.OperationNodeType);
            Assert.Same(ld, bc16.Left);
            Assert.Same(rd, bc16.Right);
            Assert.Equal(CSharpBinderFlags.None, bc16.Flags);
            Assert.Equal(typeof(int), bc16.Context);

            var be17 = DynamicCSharpExpression.DynamicOrElse(le, re);
            Assert.Equal(ExpressionType.OrElse, be17.OperationNodeType);
            Assert.Same(le, be17.Left.Expression);
            Assert.Same(re, be17.Right.Expression);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, be17.Flags);

            var bd17 = DynamicCSharpExpression.DynamicOrElse(ld, rd);
            Assert.Equal(ExpressionType.OrElse, bd17.OperationNodeType);
            Assert.Same(ld, bd17.Left);
            Assert.Same(rd, bd17.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bd17.Flags);

            var bf17 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.BinaryOperationLogical);
            Assert.Equal(ExpressionType.OrElse, bf17.OperationNodeType);
            Assert.Same(ld, bf17.Left);
            Assert.Same(rd, bf17.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bf17.Flags);

            var bc17 = DynamicCSharpExpression.DynamicOrElse(ld, rd, CSharpBinderFlags.BinaryOperationLogical, typeof(int));
            Assert.Equal(ExpressionType.OrElse, bc17.OperationNodeType);
            Assert.Same(ld, bc17.Left);
            Assert.Same(rd, bc17.Right);
            Assert.Equal(CSharpBinderFlags.BinaryOperationLogical, bc17.Flags);
            Assert.Equal(typeof(int), bc17.Context);

            var be18 = DynamicCSharpExpression.DynamicRightShift(le, re);
            Assert.Equal(ExpressionType.RightShift, be18.OperationNodeType);
            Assert.Same(le, be18.Left.Expression);
            Assert.Same(re, be18.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be18.Flags);

            var bd18 = DynamicCSharpExpression.DynamicRightShift(ld, rd);
            Assert.Equal(ExpressionType.RightShift, bd18.OperationNodeType);
            Assert.Same(ld, bd18.Left);
            Assert.Same(rd, bd18.Right);
            Assert.Equal(CSharpBinderFlags.None, bd18.Flags);

            var bf18 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.RightShift, bf18.OperationNodeType);
            Assert.Same(ld, bf18.Left);
            Assert.Same(rd, bf18.Right);
            Assert.Equal(CSharpBinderFlags.None, bf18.Flags);

            var bc18 = DynamicCSharpExpression.DynamicRightShift(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.RightShift, bc18.OperationNodeType);
            Assert.Same(ld, bc18.Left);
            Assert.Same(rd, bc18.Right);
            Assert.Equal(CSharpBinderFlags.None, bc18.Flags);
            Assert.Equal(typeof(int), bc18.Context);

            var be19 = DynamicCSharpExpression.DynamicSubtract(le, re);
            Assert.Equal(ExpressionType.Subtract, be19.OperationNodeType);
            Assert.Same(le, be19.Left.Expression);
            Assert.Same(re, be19.Right.Expression);
            Assert.Equal(CSharpBinderFlags.None, be19.Flags);

            var bd19 = DynamicCSharpExpression.DynamicSubtract(ld, rd);
            Assert.Equal(ExpressionType.Subtract, bd19.OperationNodeType);
            Assert.Same(ld, bd19.Left);
            Assert.Same(rd, bd19.Right);
            Assert.Equal(CSharpBinderFlags.None, bd19.Flags);

            var bf19 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Subtract, bf19.OperationNodeType);
            Assert.Same(ld, bf19.Left);
            Assert.Same(rd, bf19.Right);
            Assert.Equal(CSharpBinderFlags.None, bf19.Flags);

            var bc19 = DynamicCSharpExpression.DynamicSubtract(ld, rd, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Subtract, bc19.OperationNodeType);
            Assert.Same(ld, bc19.Left);
            Assert.Same(rd, bc19.Right);
            Assert.Equal(CSharpBinderFlags.None, bc19.Flags);
            Assert.Equal(typeof(int), bc19.Context);

            var be20 = DynamicCSharpExpression.DynamicSubtractChecked(le, re);
            Assert.Equal(ExpressionType.SubtractChecked, be20.OperationNodeType);
            Assert.Same(le, be20.Left.Expression);
            Assert.Same(re, be20.Right.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, be20.Flags);

            var bd20 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd);
            Assert.Equal(ExpressionType.SubtractChecked, bd20.OperationNodeType);
            Assert.Same(ld, bd20.Left);
            Assert.Same(rd, bd20.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bd20.Flags);

            var bf20 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext);
            Assert.Equal(ExpressionType.SubtractChecked, bf20.OperationNodeType);
            Assert.Same(ld, bf20.Left);
            Assert.Same(rd, bf20.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bf20.Flags);

            var bc20 = DynamicCSharpExpression.DynamicSubtractChecked(ld, rd, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(ExpressionType.SubtractChecked, bc20.OperationNodeType);
            Assert.Same(ld, bc20.Left);
            Assert.Same(rd, bc20.Right);
            Assert.Equal(CSharpBinderFlags.CheckedContext, bc20.Flags);
            Assert.Equal(typeof(int), bc20.Context);

        }

        [Fact]
        public void Dynamic_Unary_GeneratedFactories()
        {
            var oe = Expression.Constant(1);

            var od = DynamicCSharpExpression.DynamicArgument(oe);

            var ue0 = DynamicCSharpExpression.DynamicNegate(oe);
            Assert.Equal(ExpressionType.Negate, ue0.OperationNodeType);
            Assert.Same(oe, ue0.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue0.Flags);

            var ud0 = DynamicCSharpExpression.DynamicNegate(od);
            Assert.Equal(ExpressionType.Negate, ud0.OperationNodeType);
            Assert.Same(od, ud0.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud0.Flags);

            var uf0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Negate, uf0.OperationNodeType);
            Assert.Same(od, uf0.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf0.Flags);

            var uc0 = DynamicCSharpExpression.DynamicNegate(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Negate, uc0.OperationNodeType);
            Assert.Same(od, uc0.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc0.Flags);
            Assert.Equal(typeof(int), uc0.Context);

            var ue1 = DynamicCSharpExpression.DynamicUnaryPlus(oe);
            Assert.Equal(ExpressionType.UnaryPlus, ue1.OperationNodeType);
            Assert.Same(oe, ue1.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue1.Flags);

            var ud1 = DynamicCSharpExpression.DynamicUnaryPlus(od);
            Assert.Equal(ExpressionType.UnaryPlus, ud1.OperationNodeType);
            Assert.Same(od, ud1.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud1.Flags);

            var uf1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.UnaryPlus, uf1.OperationNodeType);
            Assert.Same(od, uf1.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf1.Flags);

            var uc1 = DynamicCSharpExpression.DynamicUnaryPlus(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.UnaryPlus, uc1.OperationNodeType);
            Assert.Same(od, uc1.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc1.Flags);
            Assert.Equal(typeof(int), uc1.Context);

            var ue2 = DynamicCSharpExpression.DynamicNegateChecked(oe);
            Assert.Equal(ExpressionType.NegateChecked, ue2.OperationNodeType);
            Assert.Same(oe, ue2.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ue2.Flags);

            var ud2 = DynamicCSharpExpression.DynamicNegateChecked(od);
            Assert.Equal(ExpressionType.NegateChecked, ud2.OperationNodeType);
            Assert.Same(od, ud2.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, ud2.Flags);

            var uf2 = DynamicCSharpExpression.DynamicNegateChecked(od, CSharpBinderFlags.CheckedContext);
            Assert.Equal(ExpressionType.NegateChecked, uf2.OperationNodeType);
            Assert.Same(od, uf2.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uf2.Flags);

            var uc2 = DynamicCSharpExpression.DynamicNegateChecked(od, CSharpBinderFlags.CheckedContext, typeof(int));
            Assert.Equal(ExpressionType.NegateChecked, uc2.OperationNodeType);
            Assert.Same(od, uc2.Operand);
            Assert.Equal(CSharpBinderFlags.CheckedContext, uc2.Flags);
            Assert.Equal(typeof(int), uc2.Context);

            var ue3 = DynamicCSharpExpression.DynamicNot(oe);
            Assert.Equal(ExpressionType.Not, ue3.OperationNodeType);
            Assert.Same(oe, ue3.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue3.Flags);

            var ud3 = DynamicCSharpExpression.DynamicNot(od);
            Assert.Equal(ExpressionType.Not, ud3.OperationNodeType);
            Assert.Same(od, ud3.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud3.Flags);

            var uf3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Not, uf3.OperationNodeType);
            Assert.Same(od, uf3.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf3.Flags);

            var uc3 = DynamicCSharpExpression.DynamicNot(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Not, uc3.OperationNodeType);
            Assert.Same(od, uc3.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc3.Flags);
            Assert.Equal(typeof(int), uc3.Context);

            var ue4 = DynamicCSharpExpression.DynamicDecrement(oe);
            Assert.Equal(ExpressionType.Decrement, ue4.OperationNodeType);
            Assert.Same(oe, ue4.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue4.Flags);

            var ud4 = DynamicCSharpExpression.DynamicDecrement(od);
            Assert.Equal(ExpressionType.Decrement, ud4.OperationNodeType);
            Assert.Same(od, ud4.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud4.Flags);

            var uf4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Decrement, uf4.OperationNodeType);
            Assert.Same(od, uf4.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf4.Flags);

            var uc4 = DynamicCSharpExpression.DynamicDecrement(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Decrement, uc4.OperationNodeType);
            Assert.Same(od, uc4.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc4.Flags);
            Assert.Equal(typeof(int), uc4.Context);

            var ue5 = DynamicCSharpExpression.DynamicIncrement(oe);
            Assert.Equal(ExpressionType.Increment, ue5.OperationNodeType);
            Assert.Same(oe, ue5.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue5.Flags);

            var ud5 = DynamicCSharpExpression.DynamicIncrement(od);
            Assert.Equal(ExpressionType.Increment, ud5.OperationNodeType);
            Assert.Same(od, ud5.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud5.Flags);

            var uf5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.Increment, uf5.OperationNodeType);
            Assert.Same(od, uf5.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf5.Flags);

            var uc5 = DynamicCSharpExpression.DynamicIncrement(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.Increment, uc5.OperationNodeType);
            Assert.Same(od, uc5.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc5.Flags);
            Assert.Equal(typeof(int), uc5.Context);

            var ue6 = DynamicCSharpExpression.DynamicOnesComplement(oe);
            Assert.Equal(ExpressionType.OnesComplement, ue6.OperationNodeType);
            Assert.Same(oe, ue6.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue6.Flags);

            var ud6 = DynamicCSharpExpression.DynamicOnesComplement(od);
            Assert.Equal(ExpressionType.OnesComplement, ud6.OperationNodeType);
            Assert.Same(od, ud6.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud6.Flags);

            var uf6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.OnesComplement, uf6.OperationNodeType);
            Assert.Same(od, uf6.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf6.Flags);

            var uc6 = DynamicCSharpExpression.DynamicOnesComplement(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.OnesComplement, uc6.OperationNodeType);
            Assert.Same(od, uc6.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc6.Flags);
            Assert.Equal(typeof(int), uc6.Context);

            var ue7 = DynamicCSharpExpression.DynamicIsTrue(oe);
            Assert.Equal(ExpressionType.IsTrue, ue7.OperationNodeType);
            Assert.Same(oe, ue7.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue7.Flags);

            var ud7 = DynamicCSharpExpression.DynamicIsTrue(od);
            Assert.Equal(ExpressionType.IsTrue, ud7.OperationNodeType);
            Assert.Same(od, ud7.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud7.Flags);

            var uf7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.IsTrue, uf7.OperationNodeType);
            Assert.Same(od, uf7.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf7.Flags);

            var uc7 = DynamicCSharpExpression.DynamicIsTrue(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.IsTrue, uc7.OperationNodeType);
            Assert.Same(od, uc7.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc7.Flags);
            Assert.Equal(typeof(int), uc7.Context);

            var ue8 = DynamicCSharpExpression.DynamicIsFalse(oe);
            Assert.Equal(ExpressionType.IsFalse, ue8.OperationNodeType);
            Assert.Same(oe, ue8.Operand.Expression);
            Assert.Equal(CSharpBinderFlags.None, ue8.Flags);

            var ud8 = DynamicCSharpExpression.DynamicIsFalse(od);
            Assert.Equal(ExpressionType.IsFalse, ud8.OperationNodeType);
            Assert.Same(od, ud8.Operand);
            Assert.Equal(CSharpBinderFlags.None, ud8.Flags);

            var uf8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.None);
            Assert.Equal(ExpressionType.IsFalse, uf8.OperationNodeType);
            Assert.Same(od, uf8.Operand);
            Assert.Equal(CSharpBinderFlags.None, uf8.Flags);

            var uc8 = DynamicCSharpExpression.DynamicIsFalse(od, CSharpBinderFlags.None, typeof(int));
            Assert.Equal(ExpressionType.IsFalse, uc8.OperationNodeType);
            Assert.Same(od, uc8.Operand);
            Assert.Equal(CSharpBinderFlags.None, uc8.Flags);
            Assert.Equal(typeof(int), uc8.Context);

        }
    }
}
