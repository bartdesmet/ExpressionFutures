// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public class SwitchCaseTests
    {
        [Fact]
        public void SwitchCase_Factory_ArgumentChecking()
        {
            var body = new[] { Expression.Empty() };
            var vals = new[] { 1, 2 };
            var objs = new object[] { 1, 2 };

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(int[]), body));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(IEnumerable<int>), body));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(object[]), body));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(default(IEnumerable<object>), body));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(vals, default(Expression[])));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(vals, default(IEnumerable<Expression>)));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(objs, default(Expression[])));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCase(objs, default(IEnumerable<Expression>)));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCaseDefault(default(Expression[])));
            Assert.Throws<ArgumentNullException>(() => CSharpStatement.SwitchCaseDefault(default(IEnumerable<Expression>)));

            // empty
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Array.Empty<int>(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Enumerable.Empty<int>(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Array.Empty<object>(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(Enumerable.Empty<object>(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(vals, Array.Empty<Expression>()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(vals, Enumerable.Empty<Expression>()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(objs, Array.Empty<Expression>()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(objs, Enumerable.Empty<Expression>()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCaseDefault(Array.Empty<Expression>()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCaseDefault(Enumerable.Empty<Expression>()));

            // switch type
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }, body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }, body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new double[] { 0.0 }.AsEnumerable(), body.AsEnumerable()));

            // duplicate value
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 1, 2 }, body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }, body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }, body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 1, 2 }, body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }, body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new int[] { 1, 2, 1 }.AsEnumerable(), body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }, body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2, 1 }.AsEnumerable(), body.AsEnumerable()));

            // inconsistent typing
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, "foo", 1 }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { "foo", null, 1 }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2L }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, 1, 2L }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, null, 2L }.AsEnumerable(), body));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, "foo", 1 }.AsEnumerable(), body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { "foo", null, 1 }.AsEnumerable(), body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, 2L }.AsEnumerable(), body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { null, 1, 2L }.AsEnumerable(), body.AsEnumerable()));
            Assert.Throws<ArgumentException>(() => CSharpStatement.SwitchCase(new object[] { 1, null, 2L }.AsEnumerable(), body.AsEnumerable()));

            // those are ok
            CSharpStatement.SwitchCase(new int?[] { null, 1 }, body);
            CSharpStatement.SwitchCase(new string[] { null, "bar" }, body);
            CSharpStatement.SwitchCase(new int?[] { 1, null }, body);
            CSharpStatement.SwitchCase(new string[] { "bar", null }, body);
            CSharpStatement.SwitchCase(new object[] { null }, body);
            CSharpStatement.SwitchCase(new object[] { null }.AsEnumerable(), body);
        }

        [Fact]
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
                Assert.True(b.SequenceEqual(s.Statements));
                Assert.True(t.SequenceEqual(s.TestValues.Cast<int>()));
            }
        }

        [Fact]
        public void SwitchCaseDefault_Properties()
        {
            var b = new[] { Expression.Empty() };

            var s1 = CSharpStatement.SwitchCaseDefault(b);
            var s2 = CSharpStatement.SwitchCaseDefault(b.AsEnumerable());

            foreach (var s in new[] { s1, s2 })
            {
                Assert.True(b.SequenceEqual(s.Statements));
                Assert.Same(CSharpStatement.SwitchCaseDefaultValue, s.TestValues.Single());
                Assert.Equal("default", s.TestValues.Single().ToString());
            }
        }

        [Fact]
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
                Assert.Same(s, s.Update(s.Statements));

                var u = s.Update(c);
                Assert.True(c.SequenceEqual(u.Statements));
            }
        }

        [Fact]
        public void SwitchCase_Visitor()
        {
            var body = Expression.Empty();
            var res = CSharpStatement.SwitchCase(new[] { 1, 2, 3 }, body);

            var v = new V();
            Assert.Same(res, v.VisitSwitchCase(res));
            Assert.True(v.Visited);
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
