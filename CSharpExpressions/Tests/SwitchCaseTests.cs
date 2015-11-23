// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class SwitchCaseTests
    {
        [TestMethod]
        public void SwitchCase_Factory_ArgumentChecking()
        {
            var body = Expression.Empty();
            var vals = new[] { 1, 2 };
            var objs = new object[] { 1, 2 };

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(Expression), vals));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(Expression), (IEnumerable<int>)vals));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(Expression), objs));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(Expression), (IEnumerable<object>)objs));

            // empty
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, Array.Empty<int>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, Enumerable.Empty<int>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, Enumerable.Empty<object>()));

            // switch type
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { null }));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new double[] { 0.0 }));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new double[] { 0.0 }.AsEnumerable()));

            // duplicate value
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new int[] { 1, 1, 2 }));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new int[] { 1, 2, 1 }));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new int[] { 1, 2, 1 }.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { 1, 2, 1 }));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { 1, 2, 1 }.AsEnumerable()));

            // inconsistent typing
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { null, "foo", 1 }.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { "foo", null, 1 }.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { 1, 2L }.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { null, 1, 2L }.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(body, new object[] { 1, null, 2L }.AsEnumerable()));

            // those are ok
            CSharpStatement.SwitchCase(body, new int?[] { null, 1 });
            CSharpStatement.SwitchCase(body, new string[] { null, "bar" });
            CSharpStatement.SwitchCase(body, new int?[] { 1, null });
            CSharpStatement.SwitchCase(body, new string[] { "bar", null });
            CSharpStatement.SwitchCase(body, new object[] { null }.AsEnumerable()); // REVIEW: this is too subtle...
        }

        [TestMethod]
        public void SwitchCase_Properties()
        {
            var b = Expression.Empty();
            var t = new int[] { 1, 2 };

            var s1 = CSharpStatement.SwitchCase(b, t);
            var s2 = CSharpStatement.SwitchCase(b, t.AsEnumerable());
            var s3 = CSharpStatement.SwitchCase(b, t.Select(x => (object)x).AsEnumerable());

            foreach (var s in new[] { s1, s2, s3 })
            {
                Assert.AreSame(b, s.Body);
                Assert.IsTrue(t.SequenceEqual(s.TestValues.Cast<int>()));
            }
        }

        [TestMethod]
        public void SwitchCase_Update()
        {
            var b = Expression.Empty();
            var c = Expression.Empty();
            var t = new int[] { 1, 2 };

            var s1 = CSharpStatement.SwitchCase(b, t);
            var s2 = CSharpStatement.SwitchCase(b, t.AsEnumerable());
            var s3 = CSharpStatement.SwitchCase(b, t.Select(x => (object)x).AsEnumerable());

            foreach (var s in new[] { s1, s2, s3 })
            {
                Assert.AreSame(s, s.Update(b));

                var u = s.Update(c);
                Assert.AreSame(c, u.Body);
            }
        }

        [TestMethod]
        public void SwitchCase_Visitor()
        {
            var body = Expression.Empty();
            var res = CSharpStatement.SwitchCase(body, new[] { 1, 2, 3 });

            var v = new V();
            Assert.AreSame(res, v.VisitSwitchCase(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected override Expression VisitDefault(DefaultExpression node)
            {
                Visited = true;

                return base.VisitDefault(node);
            }
        }
    }
}
