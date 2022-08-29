// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class ConditionalAccessTests
    {
        [Fact]
        public void ConditionalAccess_ArgumentChecking()
        {
            var rec = Expression.Constant(1, typeof(int?));
            var nrc = CSharpExpression.ConditionalReceiver(typeof(int));
            var wnn = Expression.Constant(1);

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalAccess(default(Expression), nrc, wnn));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalAccess(rec, default(ConditionalReceiver), wnn));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalAccess(rec, nrc, default(Expression)));

            // invalid receiver type
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalAccess(Expression.Empty(), nrc, wnn));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalAccess(Expression.Default(typeof(int)), nrc, wnn));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalAccess(Expression.Default(typeof(int).MakeByRefType()), nrc, wnn));

            // type mismatch receiver
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalAccess(Expression.Default(typeof(long?)), nrc, wnn));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalAccess(Expression.Default(typeof(string)), nrc, wnn));
        }

        [Fact]
        public void ConditionalReceiver_ArgumentChecking()
        {
            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalReceiver(default(Type)));

            // invalid receiver type
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalReceiver(typeof(void)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalReceiver(typeof(int?)));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalReceiver(typeof(int).MakeByRefType()));
        }

        [Fact]
        public void ConditionalAccess_Compile_Member1()
        {
            var p = Expression.Parameter(typeof(DateTimeOffset?));
            var n = CSharpExpression.ConditionalReceiver(typeof(DateTimeOffset));
            var e = CSharpExpression.ConditionalAccess(p, n, Expression.Property(n, "Offset"));
            var f = Expression.Lambda<Func<DateTimeOffset?, TimeSpan?>>(e, p);
            var c = f.Compile();

            foreach (var d in new DateTimeOffset?[] { null, DateTimeOffset.Now })
            {
                Assert.Equal(d?.Offset, c(d));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Member2()
        {
            var p = Expression.Parameter(typeof(DateTimeOffset?));
            var n = CSharpExpression.ConditionalReceiver(typeof(DateTimeOffset));
            var e = CSharpExpression.ConditionalAccess(p, n, Expression.Property(Expression.Property(n, "Offset"), "Hours"));
            var f = Expression.Lambda<Func<DateTimeOffset?, int?>>(e, p);
            var c = f.Compile();

            foreach (var d in new DateTimeOffset?[] { null, DateTimeOffset.Now })
            {
                Assert.Equal(d?.Offset.Hours, c(d));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Member3()
        {
            var p = Expression.Parameter(typeof(DateTimeOffset?));
            var n = CSharpExpression.ConditionalReceiver(typeof(DateTimeOffset));
            var m = CSharpExpression.ConditionalReceiver(typeof(TimeSpan));
            var e = CSharpExpression.ConditionalAccess(CSharpExpression.ConditionalAccess(p, n, Expression.Property(n, "Offset")), m, Expression.Property(m, "Hours"));
            var f = Expression.Lambda<Func<DateTimeOffset?, int?>>(e, p);
            var c = f.Compile();

            foreach (var d in new DateTimeOffset?[] { null, DateTimeOffset.Now })
            {
                Assert.Equal((d?.Offset)?.Hours, c(d));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Member4()
        {
            var p = Expression.Parameter(typeof(string));
            var n = CSharpExpression.ConditionalReceiver(typeof(string));
            var e = CSharpExpression.ConditionalAccess(p, n, Expression.Property(n, "Length"));
            var f = Expression.Lambda<Func<string, int?>>(e, p);
            var c = f.Compile();

            foreach (var s in new string[] { null, "bar" })
            {
                Assert.Equal(s?.Length, c(s));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Potpourri1()
        {
            var p = Expression.Parameter(typeof(DateTimeOffset?));
            var n = CSharpExpression.ConditionalReceiver(typeof(DateTimeOffset));
            var m = CSharpExpression.ConditionalReceiver(typeof(string));
            var e = CSharpExpression.ConditionalAccess(p, n, CSharpExpression.ConditionalAccess(Expression.Call(Expression.Property(n, "Offset"), typeof(TimeSpan).GetMethod("ToString", new Type[0])), m, Expression.Property(m, "Length")));
            var f = Expression.Lambda<Func<DateTimeOffset?, int?>>(e, p);
            var c = f.Compile();

            foreach (var d in new DateTimeOffset?[] { null, DateTimeOffset.Now })
            {
                Assert.Equal(d?.Offset.ToString()?.Length, c(d));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Potpourri2()
        {
            var p = Expression.Parameter(typeof(string));
            var n = CSharpExpression.ConditionalReceiver(typeof(string));
            var e = CSharpExpression.ConditionalAccess(p, n, CSharpExpression.ConditionalAccess(Expression.Call(Expression.Call(n, typeof(string).GetMethod("ToUpper", new Type[0])), typeof(string).GetMethod("ToLower", new Type[0])), n, Expression.Property(n, "Length")));
            var f = Expression.Lambda<Func<string, int?>>(e, p);
            var c = f.Compile();

            foreach (var s in new string[] { null, "bar" })
            {
                Assert.Equal(s?.ToUpper().ToLower()?.Length, c(s));
            }
        }

        [Fact]
        public void ConditionalAccess_Compile_Invoke1()
        {
            var p = Expression.Parameter(typeof(Action));
            var n = CSharpExpression.ConditionalReceiver(typeof(Action));
            var e = CSharpExpression.ConditionalAccess(p, n, Expression.Invoke(n));
            var f = Expression.Lambda<Action<Action>>(e, p);
            var c = f.Compile();

            c(null); // doesn't throw

            var called = false;
            c(() => { called = true; });
            Assert.True(called);
        }

        [Fact]
        public void ConditionalAccess_Update()
        {
            var rec1 = Expression.Constant(1, typeof(int?));
            var nrc1 = CSharpExpression.ConditionalReceiver(typeof(int));
            var wnn1 = Expression.Constant(1);

            var rec2 = Expression.Constant(1, typeof(int?));
            var nrc2 = CSharpExpression.ConditionalReceiver(typeof(int));
            var wnn2 = Expression.Constant(1);

            var ca0 = CSharpExpression.ConditionalAccess(rec1, nrc1, wnn1);

            var ca1 = ca0.Update(rec1, nrc1, wnn1);
            Assert.Same(ca0, ca1);

            var ca2 = ca0.Update(rec2, nrc1, wnn1);
            Assert.Same(rec2, ca2.Receiver);
            Assert.Same(nrc1, ca2.NonNullReceiver);
            Assert.Same(wnn1, ca2.WhenNotNull);

            var ca3 = ca0.Update(rec1, nrc2, wnn1);
            Assert.Same(rec1, ca3.Receiver);
            Assert.Same(nrc2, ca3.NonNullReceiver);
            Assert.Same(wnn1, ca3.WhenNotNull);

            var ca4 = ca0.Update(rec1, nrc1, wnn2);
            Assert.Same(rec1, ca4.Receiver);
            Assert.Same(nrc1, ca4.NonNullReceiver);
            Assert.Same(wnn2, ca4.WhenNotNull);
        }

        [Fact]
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

                Assert.Null(f(null));

                for (var i = 0; i < cs.Length; i++)
                {
                    Assert.Null(f(new C(i)));
                }

                Assert.NotNull(f(new C(cs.Length)));
            }
        }

        [Fact]
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

                Assert.Null(f(null));

                for (var i = 0; i < cs.Length; i++)
                {
                    Assert.Null(f(new S(i)));
                }

                Assert.NotNull(f(new S(cs.Length)));
            }
        }

        [Fact]
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

            Assert.Null(f1(null));
            Assert.Equal(4, f1(p1).Value);

            var e2 = CSharpExpression.ConditionalCall(CSharpExpression.ConditionalProperty(p, name), toUpper);
            var f2 = Expression.Lambda<Func<Person, string>>(e2, p).Compile();

            Assert.Null(f2(null));
            Assert.Equal("BART", f2(p1));

            var e3 = CSharpExpression.ConditionalProperty(CSharpExpression.ConditionalProperty(p, dob), year);
            var f3 = Expression.Lambda<Func<Person, int?>>(e3, p).Compile();

            Assert.Null(f3(null));
            Assert.Null(f3(p1));
            Assert.Equal(1983, f3(p2));
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.Equal(expected, res);
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