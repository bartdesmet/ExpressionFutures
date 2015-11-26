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
        public void Switch_Factory_ArgumentChecking()
        {
            var value = Expression.Constant(1);
            var breakLabel = Expression.Label();
            var defaultBody = Expression.Empty();
            var cases = new[] { CSharpStatement.SwitchCase(new[] { 1 }, Expression.Empty()), CSharpStatement.SwitchCase(new[] { 2 }, Expression.Empty()) };
            var empty = Expression.Empty();
            var label = Expression.Label(typeof(int));
            var dateTime = Expression.Default(typeof(DateTime));
            var nullCase = new[] { cases[0], null, cases[1] };
            var duplicateCase = new[] { cases[0], cases[1], cases[0] };
            var withNullCase = new[] { cases[0], CSharpStatement.SwitchCase(new[] { default(int?) }, Expression.Empty()) };
            var nonIntCases = new[] { CSharpStatement.SwitchCase(new[] { 1L }, Expression.Empty()) };

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
        public void Switch_Properties()
        {
            var value = Expression.Constant(1);
            var label = Expression.Label();
            var cases = new[] { CSharpStatement.SwitchCase(new[] { 42 }, Expression.Empty()) };

            var res = CSharpStatement.Switch(value, label, cases);

            Assert.AreEqual(CSharpExpressionType.Switch, res.CSharpNodeType);
            Assert.AreSame(value, res.SwitchValue);
            Assert.AreSame(label, res.BreakLabel);
            Assert.IsTrue(cases.SequenceEqual(res.Cases));
        }

        [TestMethod]
        public void Switch_Update()
        {
            var value1 = Expression.Constant(1);
            var label1 = Expression.Label();
            var cases1 = new[] { CSharpStatement.SwitchCase(new[] { 42 }, Expression.Empty()) };
            var vars1 = new[] { Expression.Parameter(typeof(int)) };

            var value2 = Expression.Constant(1);
            var label2 = Expression.Label();
            var cases2 = new[] { CSharpStatement.SwitchCase(new[] { 43 }, Expression.Empty()) };
            var vars2 = new[] { Expression.Parameter(typeof(int)) };

            var res = CSharpStatement.Switch(value1, label1, vars1, cases1);

            var u0 = res.Update(res.SwitchValue, res.BreakLabel, res.Variables, res.Cases);
            var u1 = res.Update(value2, res.BreakLabel, res.Variables, res.Cases);
            var u2 = res.Update(res.SwitchValue, label2, res.Variables, res.Cases);
            var u3 = res.Update(res.SwitchValue, res.BreakLabel, vars2, res.Cases);
            var u4 = res.Update(res.SwitchValue, res.BreakLabel, res.Variables, cases2);

            Assert.AreSame(res, u0);

            Assert.AreSame(value2, u1.SwitchValue);
            Assert.AreSame(label1, u1.BreakLabel);
            Assert.IsTrue(vars1.SequenceEqual(u1.Variables));
            Assert.IsTrue(cases1.SequenceEqual(u1.Cases));

            Assert.AreSame(value1, u2.SwitchValue);
            Assert.AreSame(label2, u2.BreakLabel);
            Assert.IsTrue(vars1.SequenceEqual(u2.Variables));
            Assert.IsTrue(cases1.SequenceEqual(u2.Cases));

            Assert.AreSame(value1, u3.SwitchValue);
            Assert.AreSame(label1, u3.BreakLabel);
            Assert.IsTrue(vars2.SequenceEqual(u3.Variables));
            Assert.IsTrue(cases1.SequenceEqual(u3.Cases));

            Assert.AreSame(value1, u4.SwitchValue);
            Assert.AreSame(label1, u4.BreakLabel);
            Assert.IsTrue(vars1.SequenceEqual(u4.Variables));
            Assert.IsTrue(cases2.SequenceEqual(u4.Cases));
        }
        
        [TestMethod]
        public void Switch_Compile_Int32()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"))
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
                    CSharpStatement.SwitchCase(new[] { 1, 3 }, log("O")),
                    CSharpStatement.SwitchCase(new[] { 2, 4 }, log("E"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"))
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
                    CSharpStatement.SwitchCase(new[] { 1, 3 }, log("O")),
                    CSharpStatement.SwitchCase(new[] { 2, 4 }, log("E"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(int?), 3 }, log("Z")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A"))
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
        public void Switch_Compile_String_Default1()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A"))
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
        public void Switch_Compile_String_Default2()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCaseDefault(log("D")),
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A"))
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
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(string) }, log("N"))
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
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(string) }, log("N"))
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
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { default(string), "3" }, log("Z")),
                    CSharpStatement.SwitchCase(new[] { "2" }, log("B"))
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
        public void Switch_Compile_String_DefaultWithCompanionship1()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { CSharpStatement.SwitchCaseDefaultValue, "3" }, log("D")),
                    CSharpStatement.SwitchCase(new[] { "2" }, log("B"))
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "B" },
                    { "3", "E", "D" },
                    { "4", "E", "D" },
                    { null, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_DefaultWithCompanionship2()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { "3", CSharpStatement.SwitchCaseDefaultValue, "4" }, log("D")),
                    CSharpStatement.SwitchCase(new[] { "2" }, log("B"))
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "B" },
                    { "3", "E", "D" },
                    { "4", "E", "D" },
                    { null, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_String_DefaultWithCompanionship3()
        {
            AssertCompile<string>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { "1" }, log("A")),
                    CSharpStatement.SwitchCase(new[] { "3", CSharpStatement.SwitchCaseDefaultValue, default(string) }, log("D")),
                    CSharpStatement.SwitchCase(new[] { "2" }, log("B"))
                ),
                new Asserts<string>
                {
                    { "0", "E", "D" },
                    { "1", "E", "A" },
                    { "2", "E", "B" },
                    { "3", "E", "D" },
                    { "4", "E", "D" },
                    { null, "E", "D" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase1()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"), CSharpStatement.GotoCase(2), log("X")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B")),
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"), CSharpStatement.GotoCase(2), log("X"))
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
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
        public void Switch_Compile_GotoCase_Null1()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    Expression.Block(log("D"), CSharpStatement.GotoCase(null), log("X")),
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N"))
                ),
                new Asserts<int?>
                {
                    { 0, "E", "D", "N" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E", "D", "N" },
                    { null, "E", "N" }
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase_Null2()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"), CSharpStatement.GotoCase(null), log("X")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N"))
                ),
                new Asserts<int?>
                {
                    { 0, "E" },
                    { 1, "E", "A" },
                    { 2, "E", "B", "N" },
                    { 3, "E" },
                    { null, "E","N" }
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase_Null3()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N"), CSharpStatement.GotoCase(1), log("X"))
                ),
                new Asserts<int?>
                {
                    { 0, "E" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E" },
                    { null, "E", "N", "A" }
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoCase_Error()
        {
            var res = CSharpStatement.Switch(Expression.Constant(1), Expression.Label(), CSharpStatement.SwitchCase(new[] { 1 }, CSharpStatement.GotoCase(2)));
            AssertEx.Throws<InvalidOperationException>(() => res.Reduce(), ex => ex.Message.Contains("goto case"));
        }

        [TestMethod]
        public void Switch_Compile_GotoDefault1()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"), CSharpStatement.GotoDefault(), log("X")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B"))
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
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B")),
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A"), CSharpStatement.GotoDefault(), log("X"))
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
        public void Switch_Compile_GotoDefault_Null()
        {
            AssertCompile<int?>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("D"),
                    CSharpStatement.SwitchCase(new[] { 1 }, log("A")),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("B")),
                    CSharpStatement.SwitchCase(new[] { default(int?) }, log("N"), CSharpStatement.GotoDefault(), log("X"))
                ),
                new Asserts<int?>
                {
                    { 0, "E", "D" },
                    { 1, "E", "A" },
                    { 2, "E", "B" },
                    { 3, "E", "D" },
                    { null, "E", "N", "D" }
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_GotoDefault_Error()
        {
            var res = CSharpStatement.Switch(Expression.Constant(1), Expression.Label(), CSharpStatement.SwitchCase(new[] { 1 }, CSharpStatement.GotoDefault()));
            AssertEx.Throws<InvalidOperationException>(() => res.Reduce(), ex => ex.Message.Contains("goto default"));
        }

        [TestMethod]
        public void Switch_Compile_VoidAllCases1()
        {
            var p = Expression.Parameter(typeof(int));

            var res = CSharpStatement.Switch(
                p,
                Expression.Label(),
                Expression.Constant("foo"),
                CSharpStatement.SwitchCase(new[] { 3 }, Expression.Constant(2))
            );

            var f = Expression.Lambda<Action<int>>(res, p).Compile(); // no error despite non-void cases
            f(1);
            f(2);
            f(3);
        }

        [TestMethod]
        public void Switch_Compile_VoidAllCases2()
        {
            var p = Expression.Parameter(typeof(int?));

            var res = CSharpStatement.Switch(
                p,
                Expression.Label(),
                Expression.Constant("foo"),
                CSharpStatement.SwitchCase(new[] { 3 }, Expression.Constant(2))
            );

            var f = Expression.Lambda<Action<int?>>(res, p).Compile(); // no error despite non-void cases
            f(1);
            f(2);
            f(3);
            f(null);
        }

        [TestMethod]
        public void Switch_Compile_VoidAllCases3()
        {
            var p = Expression.Parameter(typeof(int?));

            var res = CSharpStatement.Switch(
                p,
                Expression.Label(),
                Expression.Constant("foo"),
                CSharpStatement.SwitchCase(new[] { 3 }, Expression.Constant(2)),
                CSharpStatement.SwitchCase(new[] { default(int?) }, Expression.Default(typeof(DateTime)))
            );

            var f = Expression.Lambda<Action<int?>>(res, p).Compile(); // no error despite non-void cases
            f(1);
            f(2);
            f(3);
            f(null);
        }

        [TestMethod]
        public void Switch_Compile_NestedSwitch1()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    log("XD"),
                    CSharpStatement.SwitchCase(
                        new[] { 1 },
                        CSharpStatement.Switch(
                            Expression.Constant(3),
                            Expression.Label(),
                            log("D"),
                            CSharpStatement.SwitchCase(new[] { 3 }, log("A"), CSharpStatement.GotoDefault(), log("X"))
                        )
                    ),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("XC")),
                    CSharpStatement.SwitchCase(new[] { 3 }, log("B"), CSharpStatement.GotoDefault())
                ),
                new Asserts<int>
                {
                    { 0, "E", "XD" },
                    { 1, "E", "A", "D" },
                    { 2, "E", "XC" },
                    { 3, "E", "B", "XD" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NestedSwitch2()
        {
            AssertCompile<int>((log, v) =>
                SwitchLogValue(log,
                    v,
                    CSharpStatement.SwitchCase(
                        new[] { 1 },
                        CSharpStatement.Switch(
                            Expression.Constant(3),
                            Expression.Label(),
                            CSharpStatement.SwitchCase(new[] { 2 }, log("C")),
                            CSharpStatement.SwitchCase(new[] { 3 }, log("A"), CSharpStatement.GotoCase(2), log("X"))
                        )
                    ),
                    CSharpStatement.SwitchCase(new[] { 2 }, log("XC")),
                    CSharpStatement.SwitchCase(new[] { 3 }, log("B"), CSharpStatement.GotoCase(2))
                ),
                new Asserts<int>
                {
                    { 0, "E" },
                    { 1, "E", "A", "C" },
                    { 2, "E", "XC" },
                    { 3, "E", "B", "XC" },
                }
            );
        }

        [TestMethod]
        public void Switch_Compile_NestedSwitch3()
        {
            // NB: See design remark in code.

            var res =
                CSharpStatement.Switch(
                    Expression.Constant(1),
                    Expression.Label(),
                    CSharpStatement.SwitchCase(
                        new[] { 5 },
                        Expression.Switch(
                            Expression.Constant(2),
                            Expression.SwitchCase(CSharpStatement.GotoCase(3), Expression.Constant(4))
                        )
                    )
                );

            var red = res.Reduce(); // This doesn't throw because we don't recurse into the nested Switch.
            Assert.AreNotSame(red, res);

            var f = Expression.Lambda<Action>(res);

            AssertEx.Throws<ArgumentException>(() => f.Compile()); // must be reducible node
        }

        // TODO: tests for break behavior

        [TestMethod]
        public void Switch_Visitor()
        {
            var value = Expression.Constant(1);
            var label = Expression.Label();
            var defaultBody = Expression.Empty();
            var cases = new[] { CSharpStatement.SwitchCase(new[] { 42 }, Expression.Empty()) };

            var res = CSharpStatement.Switch(value, label, defaultBody, cases);

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitSwitch(SwitchCSharpStatement node)
            {
                Visited = true;

                return base.VisitSwitch(node);
            }
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
