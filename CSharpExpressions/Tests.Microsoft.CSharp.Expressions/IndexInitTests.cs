// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class IndexInitTests
    {
        [Fact]
        public void IndexInit_Factory_ArgumentChecking()
        {
            var index = PropertyInfoOf((Dictionary<string, int> d) => d[default(string)]);
            var setter = index.GetSetMethod(true);
            var args = new[] { Expression.Constant("bar") };
            var value = Expression.Constant(42);

            var xIndex = typeof(X).GetProperty("Item");

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(default(PropertyInfo), args, value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(default(PropertyInfo), args.AsEnumerable(), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(index, default(Expression[]), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(index, default(IEnumerable<Expression>), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(index, args, default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(index, args.AsEnumerable(), default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(default(MethodInfo), args, value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(default(MethodInfo), args.AsEnumerable(), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(setter, default(Expression[]), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(setter, default(IEnumerable<Expression>), value));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(setter, args, default(Expression)));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.IndexInit(setter, args.AsEnumerable(), default(Expression)));

            // only getter
            Assert.Throws<ArgumentException>(() => CSharpExpression.IndexInit(xIndex, new[] { value }, value));

            // argument types
            Assert.Throws<ArgumentException>(() => CSharpExpression.IndexInit(index, new[] { value }, value));
            Assert.Throws<ArgumentException>(() => CSharpExpression.IndexInit(setter, new[] { value }, value));
            Assert.Throws<ArgumentException>(() => CSharpExpression.IndexInit(index, args, args[0]));
            Assert.Throws<ArgumentException>(() => CSharpExpression.IndexInit(setter, args, args[0]));
        }

        [Fact]
        public void IndexInit_ListInit_Compile_Property()
        {
            // NB: This is the only test we can write right now in order to set indexer assignment. We should
            //     add many more if and when we get custom member binding nodes.

            var index = PropertyInfoOf((Dictionary<string, int> d) => d[default(string)]);

            var create = (ListInitExpression)((Expression<Func<Dictionary<string, int>>>)(() => new Dictionary<string, int> { { "bar", 42 } })).Body;

            var newInit = CSharpExpression.IndexInit(index, new[] { Expression.Constant("foo") }, Expression.Constant(43));

            var expr = create.Update(create.NewExpression, new[] { newInit });

            var res = Expression.Lambda<Func<Dictionary<string, int>>>(expr).Compile()();

            Assert.Equal(43, res["foo"]);
        }

        [Fact]
        public void IndexInit_ListInit_Compile_Method()
        {
            // NB: This is the only test we can write right now in order to set indexer assignment. We should
            //     add many more if and when we get custom member binding nodes.

            var index = PropertyInfoOf((Dictionary<string, int> d) => d[default(string)]).GetSetMethod(true);

            var create = (ListInitExpression)((Expression<Func<Dictionary<string, int>>>)(() => new Dictionary<string, int> { { "bar", 42 } })).Body;

            var newInit = CSharpExpression.IndexInit(index, new[] { Expression.Constant("foo") }, Expression.Constant(43));

            var expr = create.Update(create.NewExpression, new[] { newInit });

            var res = Expression.Lambda<Func<Dictionary<string, int>>>(expr).Compile()();

            Assert.Equal(43, res["foo"]);
        }

        class X
        {
            public object this[int x]
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
