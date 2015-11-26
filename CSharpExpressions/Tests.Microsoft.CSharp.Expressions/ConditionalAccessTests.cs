// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class ConditionalAccessTests
    {
        [TestMethod]
        public void ConditionalAccess_ManOrBoy1()
        {
            var p = Expression.Parameter(typeof(C));

            var x = Expression.Constant(0);

            var F = p.Type.GetField("F");
            var P = p.Type.GetProperty("P");
            var I = p.Type.GetProperty("Item");
            var M = p.Type.GetMethod("M");

            var cF = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalField(e, F));
            var cP = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalProperty(e, P));
            var cI = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalIndex(e, I, new[] { x }));
            var cM = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalCall(e, M, new[] { x }));

            var cases = new Func<Expression, Expression>[][]
            {
                new[] { cF },
                new[] { cF, cP },
                new[] { cF, cP, cI },
                new[] { cF, cP, cI, cM },
                new[] { cM },
                new[] { cM, cF },
                new[] { cM, cF, cP },
                new[] { cM, cF, cP, cI },
                new[] { cI },
                new[] { cI, cM },
                new[] { cI, cM, cF },
                new[] { cI, cM, cF, cP },
                new[] { cP },
                new[] { cP, cI },
                new[] { cP, cI, cM },
                new[] { cP, cI, cM, cF },
            };

            foreach (var cs in cases)
            {
                var e = cs.Aggregate((Expression)p, (a, b) => b(a));

                var f = Expression.Lambda<Func<C, C>>(e, p).Compile();

                Assert.IsNull(f(null));

                for (var i = 0; i < cs.Length; i++)
                {
                    Assert.IsNull(f(new C(i)));
                }

                Assert.IsNotNull(f(new C(cs.Length)));
            }
        }

        [TestMethod]
        public void ConditionalAccess_ManOrBoy2()
        {
            var p = Expression.Parameter(typeof(S?));

            var x = Expression.Constant(0);

            var P = p.Type.GetGenericArguments()[0].GetProperty("P");
            var I = p.Type.GetGenericArguments()[0].GetProperty("Item");
            var M = p.Type.GetGenericArguments()[0].GetMethod("M");

            var cP = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalProperty(e, P));
            var cI = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalIndex(e, I, new[] { x }));
            var cM = (Func<Expression, Expression>)(e => CSharpExpression.ConditionalCall(e, M, new[] { x }));

            var cases = new Func<Expression, Expression>[][]
            {
                new[] { cP },
                new[] { cP, cI },
                new[] { cP, cI, cM },
                new[] { cM },
                new[] { cM, cP },
                new[] { cM, cP, cI },
                new[] { cI },
                new[] { cI, cM },
                new[] { cI, cM, cP },
            };

            foreach (var cs in cases)
            {
                var e = cs.Aggregate((Expression)p, (a, b) => b(a));

                var f = Expression.Lambda<Func<S?, S?>>(e, p).Compile();

                Assert.IsNull(f(null));

                for (var i = 0; i < cs.Length; i++)
                {
                    Assert.IsNull(f(new S(i)));
                }

                Assert.IsNotNull(f(new S(cs.Length)));
            }
        }

        [TestMethod]
        public void ConditionalAccess_ManOrBoy3()
        {
            var p1 = new Person { Name = "Bart" };
            var p2 = new Person { Name = "Bart", DOB = new DateTime(1983, 2, 11) };

            var p = Expression.Parameter(typeof(Person));

            var name = PropertyInfoOf((Person x) => x.Name);
            var dob = PropertyInfoOf((Person x) => x.DOB);
            var length = PropertyInfoOf((string s) => s.Length);
            var year = PropertyInfoOf((DateTime x) => x.Year);
            var toUpper = MethodInfoOf((string s) => s.ToUpper());

            var e1 = CSharpExpression.ConditionalProperty(CSharpExpression.ConditionalProperty(p, name), length);
            var f1 = Expression.Lambda<Func<Person, int?>>(e1, p).Compile();

            Assert.IsNull(f1(null));
            Assert.AreEqual(4, f1(p1).Value);

            var e2 = CSharpExpression.ConditionalCall(CSharpExpression.ConditionalProperty(p, name), toUpper);
            var f2 = Expression.Lambda<Func<Person, string>>(e2, p).Compile();

            Assert.IsNull(f2(null));
            Assert.AreEqual("BART", f2(p1));

            var e3 = CSharpExpression.ConditionalProperty(CSharpExpression.ConditionalProperty(p, dob), year);
            var f3 = Expression.Lambda<Func<Person, int?>>(e3, p).Compile();

            Assert.IsNull(f3(null));
            Assert.IsNull(f3(p1));
            Assert.AreEqual(1983, f3(p2));
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        class C
        {
            private readonly int _n;

            public C(int n)
            {
                _n = n;
                F = n == 0 ? null : new C(n - 1);
            }

            public C F;
            public C P => F;
            public C this[int x] => F;
            public C M(int x) => F;
            public override string ToString() => $"C({_n})";
        }

        struct S
        {
            private readonly int _n;

            public S(int n)
            {
                _n = n;
            }

            public S? this[int x] => P;
            public S? P => _n == 0 ? default(S?) : new S(_n - 1);
            public S? M(int x) => P;
            public override string ToString() => $"S({_n})";
        }

        class Person
        {
            public string Name { get; set; }
            public DateTime? DOB { get; set; }
        }
    }
}