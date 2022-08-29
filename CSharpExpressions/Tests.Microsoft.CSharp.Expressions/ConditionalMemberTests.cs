// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class ConditionalMemberTests
    {
        [Fact]
        public void ConditionalMember_Factory_ArgumentChecking()
        {
            var expr = Expression.Default(typeof(Bar));
            var other = Expression.Default(typeof(string));
            var propName = "P";
            var propInfo = typeof(Bar).GetProperty(propName);
            var getInfo = propInfo.GetGetMethod(true);
            var fieldName = "F";
            var fieldInfo = typeof(Bar).GetField(fieldName);

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(default(Expression), propName));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(expr, default(string)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(default(Expression), propInfo));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(expr, default(PropertyInfo)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(default(Expression), getInfo));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalProperty(expr, default(MethodInfo)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalField(default(Expression), fieldName));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalField(expr, default(string)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalField(default(Expression), fieldInfo));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ConditionalField(expr, default(FieldInfo)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.MakeConditionalMemberAccess(default(Expression), propInfo));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.MakeConditionalMemberAccess(expr, default(MemberInfo)));

            // not exist
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, "X"));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalField(expr, "X"));

            // static
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, expr.Type.GetProperty("SP")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalField(expr, expr.Type.GetField("SF")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, "SP"));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalField(expr, "SF"));

            // set-only
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, expr.Type.GetProperty("XP")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, "XP"));

            // indexer
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, expr.Type.GetProperty("Item")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(expr, "Item"));

            // wrong declaring type
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(other, expr.Type.GetProperty("P")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalProperty(other, "P"));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalField(other, expr.Type.GetField("F")));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ConditionalField(other, "F"));

            // not field or property
            Assert.Throws<ArgumentException>(() => CSharpExpression.MakeConditionalMemberAccess(expr, expr.Type.GetConstructors()[0]));
        }

        [Fact]
        public void ConditionalMember_Properties()
        {
            var expr = Expression.Default(typeof(Bar));
            var propName = "P";
            var propInfo = typeof(Bar).GetProperty(propName);
            var getInfo = propInfo.GetGetMethod(true);
            var fieldName = "F";
            var fieldInfo = typeof(Bar).GetField(fieldName);

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalField(expr, fieldInfo),
                CSharpExpression.ConditionalField(expr, fieldName),
                CSharpExpression.MakeConditionalMemberAccess(expr, fieldInfo),
            })
            {
                Assert.Same(expr, e.Expression);
                Assert.Equal(fieldInfo, e.Member);
                Assert.Equal(typeof(int?), e.Type);
                Assert.Equal(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
            }

            foreach (var e in new[]
            {
                CSharpExpression.ConditionalProperty(expr, propInfo),
                CSharpExpression.ConditionalProperty(expr, getInfo),
                CSharpExpression.ConditionalProperty(expr, propName),
                CSharpExpression.MakeConditionalMemberAccess(expr, propInfo),
            })
            {
                Assert.Same(expr, e.Expression);
                Assert.Equal(propInfo, e.Member);
                Assert.Equal(typeof(int?), e.Type);
                Assert.Equal(CSharpExpressionType.ConditionalAccess, e.CSharpNodeType);
            }
        }

        [Fact]
        public void ConditionalMember_Update()
        {
            var expr1 = Expression.Default(typeof(Bar));
            var expr2 = Expression.Default(typeof(Bar));
            var propName = "P";
            var propInfo = typeof(Bar).GetProperty(propName);
            var fieldName = "F";
            var fieldInfo = typeof(Bar).GetField(fieldName);

            var res1 = CSharpExpression.ConditionalProperty(expr1, propInfo);
            var res2 = CSharpExpression.ConditionalField(expr1, fieldInfo);

            Assert.Same(res1, res1.Update(res1.Expression));
            Assert.Same(res2, res2.Update(res2.Expression));

            var upd1 = res1.Update(expr2);
            var upd2 = res2.Update(expr2);

            Assert.Same(expr2, upd1.Expression);
            Assert.Same(expr2, upd2.Expression);
        }

        [Fact]
        public void ConditionalMember_Compile_Property_Ref()
        {
            var p = Expression.Parameter(typeof(Qux));
            var q = new Qux();

            var m1 = CSharpExpression.ConditionalProperty(p, "X");
            var f1 = Expression.Lambda<Func<Qux, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalProperty(p, "N");
            var f2 = Expression.Lambda<Func<Qux, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalProperty(p, "S");
            var f3 = Expression.Lambda<Func<Qux, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));
        }

        [Fact]
        public void ConditionalMember_Compile_Field_Ref()
        {
            var p = Expression.Parameter(typeof(Qux));
            var q = new Qux();

            var m1 = CSharpExpression.ConditionalField(p, "Y");
            var f1 = Expression.Lambda<Func<Qux, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalField(p, "O");
            var f2 = Expression.Lambda<Func<Qux, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalField(p, "T");
            var f3 = Expression.Lambda<Func<Qux, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));
        }

        [Fact]
        public void ConditionalMember_Compile_Property_Val()
        {
            var p = Expression.Parameter(typeof(Quz?));
            var q = new Quz(false);

            var m1 = CSharpExpression.ConditionalProperty(p, "X");
            var f1 = Expression.Lambda<Func<Quz?, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalProperty(p, "N");
            var f2 = Expression.Lambda<Func<Quz?, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalProperty(p, "S");
            var f3 = Expression.Lambda<Func<Quz?, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));
        }

        [Fact]
        public void ConditionalMember_Compile_Field_Val()
        {
            var p = Expression.Parameter(typeof(Quz?));
            var q = new Quz(false);

            var m1 = CSharpExpression.ConditionalField(p, "Y");
            var f1 = Expression.Lambda<Func<Quz?, int?>>(m1, p);
            var d1 = f1.Compile();

            Assert.Equal(42, d1(q));
            Assert.Null(d1(null));

            var m2 = CSharpExpression.ConditionalField(p, "O");
            var f2 = Expression.Lambda<Func<Quz?, int?>>(m2, p);
            var d2 = f2.Compile();

            Assert.Equal(42, d2(q));
            Assert.Null(d2(null));

            var m3 = CSharpExpression.ConditionalField(p, "T");
            var f3 = Expression.Lambda<Func<Quz?, string>>(m3, p);
            var d3 = f3.Compile();

            Assert.Equal("bar", d3(q));
            Assert.Null(d3(null));
        }

#pragma warning disable CS0649
        class Bar
        {
            public bool this[int x] { get { return false; } }
            public int F;
            public int P { get; set; }
            public string XP { set { } }

            public static int SF;
            public static string SP { get; set; }
        }
#pragma warning restore

        class Qux
        {
            public int X => 42;
            public int? N => 42;
            public string S => "bar";

            public int Y = 42;
            public int? O = 42;
            public string T = "bar";
        }

        struct Quz
        {
            public Quz(bool b)
            {
                Y = 42;
                O = 42;
                T = "bar";
            }

            public int X => 42;
            public int? N => 42;
            public string S => "bar";

            public int Y;
            public int? O;
            public string T;
        }
    }
}
