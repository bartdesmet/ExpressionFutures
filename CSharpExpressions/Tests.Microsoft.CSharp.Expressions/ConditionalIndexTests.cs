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
    public class ConditionalIndexTests
    {
        [Fact]
        public void ConditionalIndex_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Bar));
            var other = Expression.Default(typeof(string));
            var propName = "Item";
            var propInfo = typeof(Bar).GetProperty(propName);
            var getInfo = propInfo.GetGetMethod(true);

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalIndex(default(Expression), propInfo));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalIndex(expr, default(PropertyInfo)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalIndex(default(Expression), getInfo));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalIndex(expr, default(MethodInfo)));

            // property - NB: this is allowed (and safe) in LINQ and it is here, too
            // AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalIndex(expr, expr.Type.GetProperty("P")));

            // wrong declaring type
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.ConditionalIndex(other, propInfo));
        }

        [Fact]
        public void ConditionalIndex_Factory_Expression()
        {
            var obj = Expression.Constant(new S("foo"));
            var substring = PropertyInfoOf((S s) => s[default(int), default(int)]);
            var substringGet = substring.GetGetMethod(true);

            var args = new[] { Expression.Constant(1) };

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalIndex(obj, substring, args),
                CSharpExpression.ConditionalIndex(obj, substring, args.AsEnumerable()),
            })
            {
                Assert.Same(obj, e.Object);

                Assert.Single(e.Arguments);

                Assert.Equal(substring.GetIndexParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalIndex(obj, substringGet, args),
                CSharpExpression.ConditionalIndex(obj, substringGet, args.AsEnumerable()),
            })
            {
                Assert.Same(obj, e.Object);

                Assert.Single(e.Arguments);

                Assert.Equal(substringGet.GetParameters()[0], e.Arguments[0].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
            }

            var tooLittle = new Expression[0];

            foreach (var f in new Func<ConditionalIndexCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalIndex(obj, substring, tooLittle),
                () => CSharpExpression.ConditionalIndex(obj, substring, tooLittle.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }

            var tooMany = new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) };

            foreach (var f in new Func<ConditionalIndexCSharpExpression>[]
            {
                () => CSharpExpression.ConditionalIndex(obj, substring, tooMany),
                () => CSharpExpression.ConditionalIndex(obj, substring, tooMany.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void ConditionalIndex_Properties()
        {
            var item = PropertyInfoOf((Bar b) => b[default(int)]);
            var parameterp = item.GetIndexParameters()[0];

            var getter = item.GetGetMethod(true);
            var parameterm = getter.GetParameters()[0];

            var obj = Expression.Default(typeof(Bar));
            var index = Expression.Constant(0);

            var argp = CSharpExpression.Bind(parameterp, index);
            var argm = CSharpExpression.Bind(parameterm, index);

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalIndex(obj, item, argp),
                CSharpExpression.ConditionalIndex(obj, item, new[] { argp }.AsEnumerable()),
            })
            {
                Assert.Equal(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
                Assert.Same(obj, e.Object);
                Assert.Equal(item, e.Indexer);
                Assert.Equal(typeof(bool?), e.Type);
                Assert.True(e.Arguments.SequenceEqual(new[] { argp }));
            }

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalIndex(obj, getter, argm),
                CSharpExpression.ConditionalIndex(obj, getter, new[] { argm }.AsEnumerable())
            })
            {
                Assert.Equal(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
                Assert.Same(obj, e.Object);
                Assert.Equal(item, e.Indexer);
                Assert.Equal(typeof(bool?), e.Type);
                Assert.True(e.Arguments.SequenceEqual(new[] { argm }));
            }
        }

        [Fact]
        public void ConditionalIndex_Update()
        {
            var item = PropertyInfoOf((Bar b) => b[default(int)]);

            var parameters = item.GetIndexParameters();

            var parameter = parameters[0];

            var obj = Expression.Default(typeof(Bar));
            var index = Expression.Constant(0);

            var arg = CSharpExpression.Bind(parameter, index);

            var res = CSharpExpression.ConditionalIndex(obj, item, arg);

            Assert.Same(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Default(typeof(Bar));
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.NotSame(upd1, res);
            Assert.Same(res.Arguments, upd1.Arguments);
            Assert.Same(obj1, upd1.Object);

            var newValue = Expression.Constant(1);
            var newArg = CSharpExpression.Bind(parameter, newValue);

            var upd2 = res.Update(obj, new[] { newArg });
            Assert.NotSame(upd2, res);
            Assert.Same(res.Object, upd2.Object);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { newArg }));
        }

        [Fact]
        public void ConditionalIndex_Compile_Ref()
        {
            var px = Expression.Parameter(typeof(QuxX));
            var qx = new QuxX();
            var ix = typeof(QuxX).GetProperty("Item");
            var ax = CSharpExpression.Bind(ix.GetIndexParameters()[0], Expression.Constant(0));
            var mx = CSharpExpression.ConditionalIndex(px, ix, ax);
            var fx = Expression.Lambda<Func<QuxX, int?>>(mx, px);
            var dx = fx.Compile();
            Assert.Equal(42, dx(qx));
            Assert.Null(dx(null));

            var pn = Expression.Parameter(typeof(QuxN));
            var qn = new QuxN();
            var jn = typeof(QuxN).GetProperty("Item");
            var an = CSharpExpression.Bind(jn.GetIndexParameters()[0], Expression.Constant(0));
            var mn = CSharpExpression.ConditionalIndex(pn, jn, an);
            var fn = Expression.Lambda<Func<QuxN, int?>>(mn, pn);
            var dn = fn.Compile();
            Assert.Equal(42, dn(qn));
            Assert.Null(dn(null));

            var ps = Expression.Parameter(typeof(QuxS));
            var qs = new QuxS();
            var js = typeof(QuxS).GetProperty("Item");
            var bs = CSharpExpression.Bind(js.GetIndexParameters()[0], Expression.Constant(0));
            var ms = CSharpExpression.ConditionalIndex(ps, js, bs);
            var fs = Expression.Lambda<Func<QuxS, string>>(ms, ps);
            var ds = fs.Compile();
            Assert.Equal("bar", ds(qs));
            Assert.Null(ds(null));
        }

        [Fact]
        public void ConditionalIndex_Compile_Val()
        {
            var px = Expression.Parameter(typeof(QuzX?));
            var qx = new QuzX();
            var ix = typeof(QuzX).GetProperty("Item");
            var ax = CSharpExpression.Bind(ix.GetIndexParameters()[0], Expression.Constant(0));
            var mx = CSharpExpression.ConditionalIndex(px, ix, ax);
            var fx = Expression.Lambda<Func<QuzX?, int?>>(mx, px);
            var dx = fx.Compile();
            Assert.Equal(42, dx(qx));
            Assert.Null(dx(null));

            var pn = Expression.Parameter(typeof(QuzN?));
            var qn = new QuzN();
            var jn = typeof(QuzN).GetProperty("Item");
            var an = CSharpExpression.Bind(jn.GetIndexParameters()[0], Expression.Constant(0));
            var mn = CSharpExpression.ConditionalIndex(pn, jn, an);
            var fn = Expression.Lambda<Func<QuzN?, int?>>(mn, pn);
            var dn = fn.Compile();
            Assert.Equal(42, dn(qn));
            Assert.Null(dn(null));

            var ps = Expression.Parameter(typeof(QuzS?));
            var qs = new QuzS();
            var js = typeof(QuzS).GetProperty("Item");
            var bs = CSharpExpression.Bind(js.GetIndexParameters()[0], Expression.Constant(0));
            var ms = CSharpExpression.ConditionalIndex(ps, js, bs);
            var fs = Expression.Lambda<Func<QuzS?, string>>(ms, ps);
            var ds = fs.Compile();
            Assert.Equal("bar", ds(qs));
            Assert.Null(ds(null));
        }

        // TODO: tests to assert args are not evaluated if receiver is null
        // TODO: tests to assert receiver is only evaluated once

        class Bar
        {
            public bool this[int x] { get { return false; } }
            public int P { get; set; }
        }

        class QuxX
        {
            public int this[int x] => 42;
        }

        class QuxN
        {
            public int? this[int x] => 42;
        }

        class QuxS
        {
            public string this[int x] => "bar";
        }

        struct QuzX
        {
            public int this[int x] => 42;
        }

        struct QuzN
        {
            public int? this[int x] => 42;
        }

        struct QuzS
        {
            public string this[int x] => "bar";
        }

        class S
        {
            private readonly string _s;

            public S(string s)
            {
                _s = s;
            }

            public string this[int startIndex, int length = -1]
            {
                get
                {
                    if (length == -1)
                    {
                        return _s.Substring(startIndex);
                    }

                    return _s.Substring(startIndex, length);
                }
            }
        }
    }
}
