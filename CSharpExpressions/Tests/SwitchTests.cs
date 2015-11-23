// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Tests.TestHelpers;
using System.Collections;

namespace Tests
{
    [TestClass]
    public class SwitchTests
    {
        [TestMethod]
        public void Switch_Compile_Int32()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("A"), 1)
                ),
                new Asserts<int>
                {
                    { 0, "E" },
                    { 1, "E", "A" },
                    { 2, "E" },
                }
            );
        }

        private static SwitchCSharpStatement SwitchLogValue(Func<string, Expression> log, Expression expression, params CSharpSwitchCase[] cases)
        {
            var b = Expression.Label();
            var eval = Expression.Block(log("E"), expression);
            return CSharpStatement.Switch(eval, b, cases);
        }

        private void AssertCompile<TSwitchValue>(Func<Func<string, Expression>, ParameterExpression, Expression> createExpression, Asserts<TSwitchValue> expected)
        {
            var res = WithLog<TSwitchValue>(createExpression).Compile();

            foreach (var c in expected.Cases)
            {
                Assert.AreEqual(new LogAndResult<object>(c.Value, null, null), res(c.Key));
            }
        }

        private void AssertCompile<TSwitchValue>(Func<Func<string, Expression>, Expression, ParameterExpression, Expression> createExpression, Asserts<TSwitchValue> expected)
        {
            var res = WithLog<TSwitchValue>(createExpression).Compile();

            foreach (var c in expected.Cases)
            {
                Assert.AreEqual(new LogAndResult<object>(c.Value, null, null), res(c.Key));
            }
        }

        class Asserts<TValue> : IEnumerable<object>
        {
            public Dictionary<TValue, List<string>> Cases = new Dictionary<TValue, List<string>>();

            public void Add(TValue value, params string[] logs)
            {
                Cases.Add(value, logs.ToList());
            }

            public IEnumerator<object> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
