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
            var body = new[] { Expression.Empty() };
            var vals = new[] { 1, 2 };
            var objs = new object[] { 1, 2 };

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(int[]), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(IEnumerable<int>), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(object[]), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(IEnumerable<object>), body));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(vals, default(Expression[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(vals, default(IEnumerable<Expression>)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(objs, default(Expression[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(objs, default(IEnumerable<Expression>)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCaseDefault(default(Expression[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCaseDefault(default(IEnumerable<Expression>)));

            // empty
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Array.Empty<int>(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Enumerable.Empty<int>(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Array.Empty<object>(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Enumerable.Empty<object>(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(vals, Array.Empty<Expression>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(vals, Enumerable.Empty<Expression>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(objs, Array.Empty<Expression>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(objs, Enumerable.Empty<Expression>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCaseDefault(Array.Empty<Expression>()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCaseDefault(Enumerable.Empty<Expression>()));

            // switch type
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }, body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }, body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }.AsEnumerable(), body.AsEnumerable()));

            // duplicate value
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 1, 2 }, body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }, body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }, body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 1, 2 }, body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }, body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }.AsEnumerable(), body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }, body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }.AsEnumerable(), body.AsEnumerable()));

            // inconsistent typing
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, "foo", 1 }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { "foo", null, 1 }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2L }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, 1, 2L }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, null, 2L }.AsEnumerable(), body));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, "foo", 1 }.AsEnumerable(), body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { "foo", null, 1 }.AsEnumerable(), body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2L }.AsEnumerable(), body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, 1, 2L }.AsEnumerable(), body.AsEnumerable()));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, null, 2L }.AsEnumerable(), body.AsEnumerable()));

            // those are ok
            CSharpStatement.SwitchCase(new int?[] { null, 1 }, body);
            CSharpStatement.SwitchCase(new string[] { null, "bar" }, body);
            CSharpStatement.SwitchCase(new int?[] { 1, null }, body);
            CSharpStatement.SwitchCase(new string[] { "bar", null }, body);
            CSharpStatement.SwitchCase(new object[] { null }, body);
            CSharpStatement.SwitchCase(new object[] { null }.AsEnumerable(), body);
        }

        [TestMethod]
        public void SwitchCase_Properties()
        {
            var b = new[] { Expression.Empty() };
            var t = new int[] { 1, 2 };

            var s1 = CSharpStatement.SwitchCase(t, b);
            var s2 = CSharpStatement.SwitchCase(t.AsEnumerable(), b);
            var s3 = CSharpStatement.SwitchCase(t.Select(x => (object)x).ToArray(), b);
            var s4 = CSharpStatement.SwitchCase(t.Select(x => (object)x).AsEnumerable(), b);
            var s5 = CSharpStatement.SwitchCase(t, b.AsEnumerable());
            var s6 = CSharpStatement.SwitchCase(t.AsEnumerable(), b.AsEnumerable());
            var s7 = CSharpStatement.SwitchCase(t.Select(x => (object)x).ToArray(), b.AsEnumerable());
            var s8 = CSharpStatement.SwitchCase(t.Select(x => (object)x).AsEnumerable(), b.AsEnumerable());

            foreach (var s in new[] { s1, s2, s3, s4, s5, s6, s7, s8 })
            {
                Assert.IsTrue(b.SequenceEqual(s.Statements));
                Assert.IsTrue(t.SequenceEqual(s.TestValues.Cast<int>()));
            }
        }

        [TestMethod]
        public void SwitchCaseDefault_Properties()
        {
            var b = new[] { Expression.Empty() };

            var s1 = CSharpStatement.SwitchCaseDefault(b);
            var s2 = CSharpStatement.SwitchCaseDefault(b.AsEnumerable());

            foreach (var s in new[] { s1, s2 })
            {
                Assert.IsTrue(b.SequenceEqual(s.Statements));
                Assert.AreSame(CSharpStatement.SwitchCaseDefaultValue, s.TestValues.Single());
                Assert.AreEqual("default", s.TestValues.Single().ToString());
            }
        }

        [TestMethod]
        public void SwitchCase_Update()
        {
            var b = new[] { Expression.Empty() };
            var c = new[] { Expression.Empty() };
            var t = new int[] { 1, 2 };

            var s1 = CSharpStatement.SwitchCase(t, b);
            var s2 = CSharpStatement.SwitchCase(t.AsEnumerable(), b);
            var s3 = CSharpStatement.SwitchCase(t.Select(x => (object)x).ToArray(), b);
            var s4 = CSharpStatement.SwitchCase(t.Select(x => (object)x).AsEnumerable(), b);
            var s5 = CSharpStatement.SwitchCase(t, b.AsEnumerable());
            var s6 = CSharpStatement.SwitchCase(t.AsEnumerable(), b.AsEnumerable());
            var s7 = CSharpStatement.SwitchCase(t.Select(x => (object)x).ToArray(), b.AsEnumerable());
            var s8 = CSharpStatement.SwitchCase(t.Select(x => (object)x).AsEnumerable(), b.AsEnumerable());

            foreach (var s in new[] { s1, s2, s3, s4, s5, s6, s7, s8 })
            {
                Assert.AreSame(s, s.Update(s.Statements));

                var u = s.Update(c);
                Assert.IsTrue(c.SequenceEqual(u.Statements));
            }
        }

        [TestMethod]
        public void SwitchCase_Visitor()
        {
            var body = Expression.Empty();
            var res = CSharpStatement.SwitchCase(new[] { 1, 2, 3 }, body);

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
