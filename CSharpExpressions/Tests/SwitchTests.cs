// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class SwitchTests
    {
        [TestMethod]
        public void Switch_Factory_ArgumentCheckin()
        {
            var value = Expression.Constant(1);
            var breakLabel = Expression.Label();
            var defaultBody = Expression.Empty();
            var cases = new[] { CSharpStatement.SwitchCase(Expression.Empty(), 1), CSharpStatement.SwitchCase(Expression.Empty(), 2) };
            var empty = Expression.Empty();
            var label = Expression.Label(typeof(int));
            var dateTime = Expression.Default(typeof(DateTime));
            var nullCase = new[] { cases[0], null, cases[1] };
            var duplicateCase = new[] { cases[0], cases[1], cases[0] };
            var withNullCase = new[] { cases[0], CSharpStatement.SwitchCase(Expression.Empty(), default(int?)) };
            var nonIntCases = new[] { CSharpStatement.SwitchCase(Expression.Empty(), 1L) };

            // null
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(default(Expression), breakLabel, cases));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(default(Expression), breakLabel, defaultBody, cases));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(default(Expression), breakLabel, defaultBody, cases.AsEnumerable()));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, default(LabelTarget), cases));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, default(LabelTarget), defaultBody, cases));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, default(LabelTarget), defaultBody, cases.AsEnumerable()));

            // switch type void
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(empty, breakLabel, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(empty, breakLabel, defaultBody, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(empty, breakLabel, defaultBody, cases.AsEnumerable()));

            // non-void break label
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, label, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, label, defaultBody, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, label, defaultBody, cases.AsEnumerable()));

            // invalid switch type
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(dateTime, breakLabel, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(dateTime, breakLabel, defaultBody, cases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(dateTime, breakLabel, defaultBody, cases.AsEnumerable()));

            // null case
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, breakLabel, nullCase));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, nullCase));
            AssertEx.Throws<ArgumentNullException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, nullCase.AsEnumerable()));

            // duplicate values
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, duplicateCase));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, duplicateCase));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, duplicateCase.AsEnumerable()));

            // null not allowed
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, withNullCase));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, withNullCase));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, withNullCase.AsEnumerable()));

            // incompatible types
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, nonIntCases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, nonIntCases));
            AssertEx.Throws<ArgumentException>(() => CSharpStatement.Switch(value, breakLabel, defaultBody, nonIntCases.AsEnumerable()));
        }

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

        [TestMethod]
        public void Switch_Compile_Int32_Default()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), 1)
                ),
                new Asserts<int>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A" },
                    { 2, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_Int32_Many()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("O"), 1, 3),
                    CSharpStatement.SwitchCase(log("E"), 2, 4)
                ),
                new Asserts<int>
                {
                    { 0, "E", "D" },
                    { 1, "E", "O" },
                    { 2, "E", "E" },
                    { 3, "E", "O" },
                    { 4, "E", "E" },
                    { 5, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("A"), 1)
                ),
                new Asserts<int?>
                {
                    { 0, "E" },
                    { 1, "E", "A" },
                    { 2, "E" },
                    { null, "E" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32_Default()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), 1)
                ),
                new Asserts<int?>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A" },
                    { 2, "E", "D" },
                    { null, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32_Many()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("O"), 1, 3),
                    CSharpStatement.SwitchCase(log("E"), 2, 4)
                ),
                new Asserts<int?>
                {
                    { 0, "E" },
                    { 1, "E", "O" },
                    { 2, "E", "E" },
                    { 3, "E", "O" },
                    { 4, "E", "E" },
                    { null, "E" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32_NullCase()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("A"), 1),
                    CSharpStatement.SwitchCase(log("N"), default(int?)),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int?>
                {
                    { 0, "E" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E" },
                    { null, "E", "N" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32_Default_NullCase()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), 1),
                    CSharpStatement.SwitchCase(log("N"), default(int?)),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int?>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E", "D" },
                    { null, "E", "N" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NullableInt32_NullCaseWithCompanionship()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), 1),
                    CSharpStatement.SwitchCase(log("Z"), default(int?), 3),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int?>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E", "Z" },
                    { 4, "E", "D" },
                    { null, "E", "Z" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("A"), "1")
                ),
                new Asserts<string>
                {
                    { "0", "E" },
                    { "1", "E", "A" },
                    { "2", "E" },
                    { null, "E" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_Default()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), "1")
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "D" },
                    { null, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_NullCase()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("A"), "1"),
                    CSharpStatement.SwitchCase(log("N"), default(string))
                ),
                new Asserts<string>
                {
                    { "0", "E" },
                    { "1", "E", "A" },
                    { "2", "E" },
                    { null, "E", "N" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_Default_NullCase()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), "1"),
                    CSharpStatement.SwitchCase(log("N"), default(string))
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "D" },
                    { null, "E", "N" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_NullCaseWithCompanionship()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("A"), "1"),
                    CSharpStatement.SwitchCase(log("Z"), default(string), "3"),
                    CSharpStatement.SwitchCase(log("B"), "2")
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "B" },
                    { "3", "E", "Z" },
                    { "4", "E", "D" },
                    { null, "E", "Z" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase1()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(Expression.Block(log("A"), CSharpStatement.GotoCase(2), log("X")), 1),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int>
                {
                    { 0, "E" },
                    { 1, "E", "A", "B" },
                    { 2, "E", "B" },
                    { 3, "E" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase2()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(log("B"), 2),
                    CSharpStatement.SwitchCase(Expression.Block(log("A"), CSharpStatement.GotoCase(2), log("X")), 1)
                ),
                new Asserts<int>
                {
                    { 0, "E" },
                    { 1, "E", "A", "B" },
                    { 2, "E", "B" },
                    { 3, "E" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase3()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    Expression.Block(log("D"), CSharpStatement.GotoCase(2), log("X")),
                    CSharpStatement.SwitchCase(log("A"), 1),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int>
                {
                    { 0, "E", "D", "B" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E", "D", "B" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase_Error()
        {
            var res = CSharpStatement.Switch(Expression.Constant(1), Expression.Label(), CSharpStatement.SwitchCase(CSharpStatement.GotoCase(2), 1));
            AssertEx.Throws<InvalidOperationException>(() => res.Reduce(), ex => ex.Message.Contains("goto case"));
        }

        [TestMethod]
        public void Switch_Compile_GotoDefault1()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(Expression.Block(log("A"), CSharpStatement.GotoDefault(), log("X")), 1),
                    CSharpStatement.SwitchCase(log("B"), 2)
                ),
                new Asserts<int>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A", "D" },
                    { 2, "E", "B" },
                    { 3, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoDefault2()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(log("B"), 2),
                    CSharpStatement.SwitchCase(Expression.Block(log("A"), CSharpStatement.GotoDefault(), log("X")), 1)
                ),
                new Asserts<int>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A", "D" },
                    { 2, "E", "B" },
                    { 3, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoDefault_Error()
        {
            var res = CSharpStatement.Switch(Expression.Constant(1), Expression.Label(), CSharpStatement.SwitchCase(CSharpStatement.GotoDefault(), 1));
            AssertEx.Throws<InvalidOperationException>(() => res.Reduce(), ex => ex.Message.Contains("goto default"));
        }

        private static SwitchCSharpStatement SwitchLogValue(Func<string, Expression> log, Expression expression, params CSharpSwitchCase[] cases)
        {
            var b = Expression.Label();
            var eval = Expression.Block(log("E"), expression);
            return CSharpStatement.Switch(eval, b, cases);
        }

        private static SwitchCSharpStatement SwitchLogValue(Func<string, Expression> log, Expression expression, Expression defaultBody, params CSharpSwitchCase[] cases)
        {
            var b = Expression.Label();
            var eval = Expression.Block(log("E"), expression);
            return CSharpStatement.Switch(eval, b, defaultBody, cases);
        }

        private void AssertCompile<TSwitchValue>(Func<Func<string, Expression>, ParameterExpression, Expression> createExpression, Asserts<TSwitchValue> expected)
        {
            var res = WithLog<TSwitchValue>(createExpression).Compile();

            foreach (var c in expected.Cases)
            {
                Assert.AreEqual(new LogAndResult<object>(c.Value, null, null), res(c.Key));
            }

            if (expected.Null != null)
            {
                Assert.AreEqual(new LogAndResult<object>(expected.Null, null, null), res(default(TSwitchValue)));
            }
        }

        private void AssertCompile<TSwitchValue>(Func<Func<string, Expression>, Expression, ParameterExpression, Expression> createExpression, Asserts<TSwitchValue> expected)
        {
            var res = WithLog<TSwitchValue>(createExpression).Compile();

            foreach (var c in expected.Cases)
            {
                Assert.AreEqual(new LogAndResult<object>(c.Value, null, null), res(c.Key));
            }

            if (expected.Null != null)
            {
                Assert.AreEqual(new LogAndResult<object>(expected.Null, null, null), res(default(TSwitchValue)));
            }
        }

        class Asserts<TValue> : IEnumerable<object>
        {
            public Dictionary<TValue, List<string>> Cases = new Dictionary<TValue, List<string>>();
            public List<string> Null;

            public void Add(TValue value, params string[] logs)
            {
                if (value == null)
                {
                    Null = logs.ToList();
                }
                else
                {
                    Cases.Add(value, logs.ToList());
                }
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
