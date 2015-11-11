// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class DynamicTests
    {
        [TestMethod]
        public void Dynamic_InvokeMember()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicInvokeMember(p, "Substring", Expression.Constant(1));
            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual("ar", f("bar"));
        }

        [TestMethod]
        public void Dynamic_Invoke()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicInvoke(p, Expression.Constant(1));
            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42, f(new Func<int, int>(x => x + 41)));
        }

        [TestMethod]
        public void Dynamic_Unary()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, p);
            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42, f(-42));
            Assert.AreEqual(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(-42)));
        }

        [TestMethod]
        public void Dynamic_Binary()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, p, q);
            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(3, f(1, 2));
            Assert.AreEqual("ab", f("a", "b"));
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 10), TimeSpan.FromDays(1)));
        }

        [TestMethod]
        public void Dynamic_InvokeConstructor()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), p);
            var e = Expression.Lambda<Func<object, TimeSpan>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(TimeSpan.FromSeconds(42), f(TimeSpan.FromSeconds(42).Ticks));
        }

        [TestMethod]
        public void Dynamic_GetMember()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicGetMember(p, "TotalSeconds");
            var e = Expression.Lambda<Func<object, object>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(42.0, f(TimeSpan.FromSeconds(42)));
        }

        [TestMethod]
        public void Dynamic_GetIndex()
        {
            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicGetIndex(p, q);
            var e = Expression.Lambda<Func<object, object, object>>(d, p, q);
            var f = e.Compile();
            Assert.AreEqual(5, f(new[] { 2, 3, 5 }, 2));
            Assert.AreEqual(21, f(new Dictionary<string, int> { { "Bart", 21 } }, "Bart"));
        }

        [TestMethod]
        public void Dynamic_Convert()
        {
            var p = Expression.Parameter(typeof(object));
            var d = DynamicCSharpExpression.DynamicConvert(p, typeof(DateTimeOffset));
            var e = Expression.Lambda<Func<object, DateTimeOffset>>(d, p);
            var f = e.Compile();
            Assert.AreEqual(new DateTime(1983, 2, 11), f(new DateTime(1983, 2, 11)));
            Assert.AreEqual(new DateTime(1983, 2, 11), f((DateTimeOffset)new DateTime(1983, 2, 11)));
        }
    }
}
