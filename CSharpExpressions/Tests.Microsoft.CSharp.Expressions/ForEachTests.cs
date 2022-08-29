// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;
using static Tests.TestHelpers;

namespace Tests
{
    public class ForEachTests
    {
        [Fact]
        public void ForEach_Factory_ArgumentChecking()
        {
            var variable = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int[] { 2, 3, 5 });
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();

            // null
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ForEach(default(ParameterExpression), collection, body));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ForEach(variable, default(Expression), body));
            Assert.Throws<ArgumentNullException>(() => CSharpExpression.ForEach(variable, collection, default(Expression)));

            // labels must be void
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, collection, body, Expression.Label(typeof(int))));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, collection, body, breakLabel, Expression.Label(typeof(int))));

            // conversion checks
            var p1_int32 = Expression.Parameter(typeof(int));
            var p2_int32 = Expression.Parameter(typeof(int));
            var p3_string = Expression.Parameter(typeof(string));
            var conv1 = Expression.Lambda(Expression.Default(typeof(int)), p1_int32, p2_int32);
            var conv2 = Expression.Lambda(Expression.Default(typeof(int)), p3_string);
            var conv3 = Expression.Lambda(Expression.Default(typeof(string)), p1_int32);
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conv1));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conv2));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conv3));

            // pattern mismatch
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(int)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(Enumerable<NoPattern1>)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(Enumerable<NoPattern2>)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(Enumerable<NoPattern3>)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(Enumerable<NoPattern4>)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(Enumerable<NoPattern5>)), body));
            Assert.Throws<ArgumentException>(() => CSharpExpression.ForEach(variable, Expression.Default(typeof(MoreThanOneEnumerable)), body));
        }

        class Enumerable<T>
        {
            public T GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        class NoPattern1
        {
        }

        class NoPattern2
        {
            public object Current { set { } }
        }

        class NoPattern3
        {
            [IndexerName("Current")]
            public object this[int x] { get { return null; } }
        }

        class NoPattern4
        {
            public object Current { get; }
            public void MoveNext(int x) { }
        }

        class NoPattern5
        {
            public object Current { get; }
            public void MoveNext<T>() { }
        }

        class MoreThanOneEnumerable : MoreThanOneEnumerableBase, IEnumerable<int>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        class MoreThanOneEnumerableBase : IEnumerable<string>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ForEach_Properties()
        {
            var variable = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int[] { 2, 3, 5 });
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var x = Expression.Parameter(variable.Type);
            var conversion = Expression.Lambda(x, x);

            {
                var res = CSharpExpression.ForEach(variable, collection, body);

                Assert.Equal(CSharpExpressionType.ForEach, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { variable }.SequenceEqual(res.Variables));
                Assert.Same(collection, res.Collection);
                Assert.Same(body, res.Body);
                Assert.Null(res.BreakLabel);
                Assert.Null(res.ContinueLabel);
                Assert.Null(res.Conversion);
                Assert.Null(res.Deconstruction);
                Assert.Null(res.AwaitInfo);
            }

            {
                var res = CSharpExpression.ForEach(variable, collection, body, breakLabel);

                Assert.Equal(CSharpExpressionType.ForEach, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { variable }.SequenceEqual(res.Variables));
                Assert.Same(collection, res.Collection);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Null(res.ContinueLabel);
                Assert.Null(res.Conversion);
                Assert.Null(res.Deconstruction);
                Assert.Null(res.AwaitInfo);
            }

            {
                var res = CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel);

                Assert.Equal(CSharpExpressionType.ForEach, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { variable }.SequenceEqual(res.Variables));
                Assert.Same(collection, res.Collection);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
                Assert.Null(res.Conversion);
                Assert.Null(res.Deconstruction);
                Assert.Null(res.AwaitInfo);
            }

            {
                var res = CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conversion);

                Assert.Equal(CSharpExpressionType.ForEach, res.CSharpNodeType);
                Assert.Equal(typeof(void), res.Type);
                Assert.True(new[] { variable }.SequenceEqual(res.Variables));
                Assert.Same(collection, res.Collection);
                Assert.Same(body, res.Body);
                Assert.Same(breakLabel, res.BreakLabel);
                Assert.Same(continueLabel, res.ContinueLabel);
                Assert.Same(conversion, res.Conversion);
                Assert.Null(res.Deconstruction);
                Assert.Null(res.AwaitInfo);
            }
        }

        [Fact]
        public void ForEach_Update()
        {
            var variable = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int[] { 2, 3, 5 });
            var body = Expression.Empty();
            var breakLabel = Expression.Label();
            var continueLabel = Expression.Label();
            var x = Expression.Parameter(variable.Type);
            var conversion = Expression.Lambda(x, x);
            var res = CSharpExpression.ForEach(variable, collection, body, breakLabel, continueLabel, conversion);

            Assert.Same(res, res.Update(res.EnumeratorInfo, res.BreakLabel, res.ContinueLabel, res.Variables, res.Collection, res.Conversion, res.Body, res.Deconstruction, res.AwaitInfo));

            var newVariable = Expression.Parameter(typeof(int));
            var newCollection = Expression.Constant(new int[] { 2, 3, 5 });
            var newBody = Expression.Empty();
            var newBreakLabel = Expression.Label();
            var newContinueLabel = Expression.Label();
            var newConversion = Expression.Lambda(x, x);

            var upd1 = res.Update(res.EnumeratorInfo, newBreakLabel, res.ContinueLabel, res.Variables, res.Collection, res.Conversion, res.Body, res.Deconstruction, res.AwaitInfo);
            var upd2 = res.Update(res.EnumeratorInfo, res.BreakLabel, newContinueLabel, res.Variables, res.Collection, res.Conversion, res.Body, res.Deconstruction, res.AwaitInfo);
            var upd3 = res.Update(res.EnumeratorInfo, res.BreakLabel, res.ContinueLabel, new[] { newVariable }, res.Collection, res.Conversion, res.Body, res.Deconstruction, res.AwaitInfo);
            var upd4 = res.Update(res.EnumeratorInfo, res.BreakLabel, res.ContinueLabel, res.Variables, newCollection, res.Conversion, res.Body, res.Deconstruction, res.AwaitInfo);
            var upd5 = res.Update(res.EnumeratorInfo, res.BreakLabel, res.ContinueLabel, res.Variables, res.Collection, newConversion, res.Body, res.Deconstruction, res.AwaitInfo);
            var upd6 = res.Update(res.EnumeratorInfo, res.BreakLabel, res.ContinueLabel, res.Variables, res.Collection, res.Conversion, newBody, res.Deconstruction, res.AwaitInfo);

            Assert.Same(newBreakLabel, upd1.BreakLabel);
            Assert.Same(res.ContinueLabel, upd1.ContinueLabel);
            Assert.True(res.Variables.SequenceEqual(upd1.Variables));
            Assert.Same(res.Collection, upd1.Collection);
            Assert.Same(res.Conversion, upd1.Conversion);
            Assert.Same(res.Body, upd1.Body);

            Assert.Same(res.BreakLabel, upd2.BreakLabel);
            Assert.Same(newContinueLabel, upd2.ContinueLabel);
            Assert.True(res.Variables.SequenceEqual(upd1.Variables));
            Assert.Same(res.Collection, upd2.Collection);
            Assert.Same(res.Conversion, upd2.Conversion);
            Assert.Same(res.Body, upd2.Body);

            Assert.Same(res.BreakLabel, upd3.BreakLabel);
            Assert.Same(res.ContinueLabel, upd3.ContinueLabel);
            Assert.True(new[] { newVariable }.SequenceEqual(upd3.Variables));
            Assert.Same(res.Collection, upd3.Collection);
            Assert.Same(res.Conversion, upd3.Conversion);
            Assert.Same(res.Body, upd3.Body);

            Assert.Same(res.BreakLabel, upd4.BreakLabel);
            Assert.Same(res.ContinueLabel, upd4.ContinueLabel);
            Assert.True(res.Variables.SequenceEqual(upd4.Variables));
            Assert.Same(newCollection, upd4.Collection);
            Assert.Same(res.Conversion, upd4.Conversion);
            Assert.Same(res.Body, upd4.Body);

            Assert.Same(res.BreakLabel, upd5.BreakLabel);
            Assert.Same(res.ContinueLabel, upd5.ContinueLabel);
            Assert.True(res.Variables.SequenceEqual(upd5.Variables));
            Assert.Same(res.Collection, upd5.Collection);
            Assert.Same(newConversion, upd5.Conversion);
            Assert.Same(res.Body, upd5.Body);

            Assert.Same(res.BreakLabel, upd6.BreakLabel);
            Assert.Same(res.ContinueLabel, upd6.ContinueLabel);
            Assert.True(res.Variables.SequenceEqual(upd6.Variables));
            Assert.Same(res.Collection, upd6.Collection);
            Assert.Same(res.Conversion, upd6.Conversion);
            Assert.Same(newBody, upd6.Body);
        }

        [Fact]
        public void ForEach_Compile_Array1()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new[] { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Array2()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new object[] { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Array3()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new object[] { 2, 3, 5, "oops" });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" }, ErrorCheck = ex => ex is InvalidCastException }
            );
        }

        [Fact]
        public void ForEach_Compile_Array4()
        {
            var i = Expression.Parameter(typeof(int?));
            var collection = Expression.Constant(new[] { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(Expression.Property(i, "Value"), typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Array5()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int?[] { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Array6()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int?[] { 2, 3, 5, null });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" }, ErrorCheck = ex => ex is InvalidOperationException }
            );
        }

        [Fact]
        public void ForEach_Compile_Array7()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new[] { 2, 3, 5 });
            var x = Expression.Parameter(typeof(int));
            var conv = Expression.Lambda(Expression.Add(x, Expression.Constant(1)), x);

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>()))),
                    null,
                    null,
                    conv
                ),
                new LogAndResult<object> { Log = { "3", "4", "6" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Array8()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new[] { 2, 3, 5, 0 });
            var x = Expression.Parameter(typeof(int));
            var conv = Expression.Lambda(Expression.Divide(Expression.Constant(30), x), x);

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>()))),
                    null,
                    null,
                    conv
                ),
                new LogAndResult<object> { Log = { "15", "10", "6" }, ErrorCheck = ex => ex is DivideByZeroException }
            );
        }

        [Fact]
        public void ForEach_Compile_Array9()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(Expression.Modulo(i, Expression.Constant(2)), Expression.Constant(0)),
                            Expression.Continue(cnt)
                        ),
                        Expression.IfThen(
                            Expression.GreaterThan(i, Expression.Constant(5)),
                            Expression.Break(brk)
                        ),
                        Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                    ),
                    brk,
                    cnt
                ),
                new LogAndResult<object> { Log = { "1", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_String1()
        {
            var c = Expression.Parameter(typeof(char));
            var str = Expression.Constant("bar");

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    c,
                    str,
                    Expression.Invoke(append, Expression.Call(c, typeof(char).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "b", "a", "r" } }
            );
        }

        [Fact]
        public void ForEach_Compile_String2()
        {
            var c = Expression.Parameter(typeof(char));
            var collection = Expression.Constant("foobar");
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    c,
                    collection,
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(c, Expression.Constant('o')),
                            Expression.Continue(cnt)
                        ),
                        Expression.IfThen(
                            Expression.Equal(c, Expression.Constant('a')),
                            Expression.Break(brk)
                        ),
                        Expression.Invoke(append, Expression.Call(c, typeof(char).GetMethod("ToString", Array.Empty<Type>())))
                    ),
                    brk,
                    cnt
                ),
                new LogAndResult<object> { Log = { "f", "b" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List1()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int> { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List2()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<object> { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List3()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<object> { 2, 3, 5, "oops" });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" }, ErrorCheck = ex => ex is InvalidCastException }
            );
        }

        [Fact]
        public void ForEach_Compile_List4()
        {
            var i = Expression.Parameter(typeof(int?));
            var collection = Expression.Constant(new List<int> { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(Expression.Property(i, "Value"), typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List5()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int?> { 2, 3, 5 });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List6()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int?> { 2, 3, 5, null });

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "2", "3", "5" }, ErrorCheck = ex => ex is InvalidOperationException }
            );
        }

        [Fact]
        public void ForEach_Compile_List7()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int> { 2, 3, 5 });
            var x = Expression.Parameter(typeof(int));
            var conv = Expression.Lambda(Expression.Add(x, Expression.Constant(1)), x);

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>()))),
                    null,
                    null,
                    conv
                ),
                new LogAndResult<object> { Log = { "3", "4", "6" } }
            );
        }

        [Fact]
        public void ForEach_Compile_List8()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int> { 2, 3, 5, 0 });
            var x = Expression.Parameter(typeof(int));
            var conv = Expression.Lambda(Expression.Divide(Expression.Constant(30), x), x);

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>()))),
                    null,
                    null,
                    conv
                ),
                new LogAndResult<object> { Log = { "15", "10", "6" }, ErrorCheck = ex => ex is DivideByZeroException }
            );
        }

        [Fact]
        public void ForEach_Compile_List9()
        {
            var i = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
            var brk = Expression.Label();
            var cnt = Expression.Label();

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    collection,
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(Expression.Modulo(i, Expression.Constant(2)), Expression.Constant(0)),
                            Expression.Continue(cnt)
                        ),
                        Expression.IfThen(
                            Expression.GreaterThan(i, Expression.Constant(5)),
                            Expression.Break(brk)
                        ),
                        Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                    ),
                    brk,
                    cnt
                ),
                new LogAndResult<object> { Log = { "1", "3", "5" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern1()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable1).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern2()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable2).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern3()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable3).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern4()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable4).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern5()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable5).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern6()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable6).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern7()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable7).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern8()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable8).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        [Fact]
        public void ForEach_Compile_Pattern9()
        {
            var i = Expression.Parameter(typeof(int));

            AssertCompile((log, append) =>
                CSharpExpression.ForEach(
                    i,
                    Expression.New(typeof(MyEnumerable8).GetConstructor(new[] { typeof(Action<string>) }), append),
                    Expression.Invoke(append, Expression.Call(i, typeof(int).GetMethod("ToString", Array.Empty<Type>())))
                ),
                new LogAndResult<object> { Log = { "C", "E", "M", "C", "1", "M", "C", "2", "M", "C", "3", "M", "D" } }
            );
        }

        class MyEnumerable1
        {
            private readonly Action<string> _log;

            public MyEnumerable1(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator1 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator1(_log);
            }
        }

        class MyEnumerator1
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator1(Action<string> log)
            {
                _log = log;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerable2
        {
            private readonly Action<string> _log;

            public MyEnumerable2(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator2 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator2(_log);
            }
        }

        sealed class MyEnumerator2
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator2(Action<string> log)
            {
                _log = log;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerable3
        {
            private readonly Action<string> _log;

            public MyEnumerable3(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator3 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator3(_log);
            }
        }

        class MyEnumerator3 : IDisposable
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator3(Action<string> log)
            {
                _log = log;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public void Dispose()
            {
                _log("D");
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerable4
        {
            private readonly Action<string> _log;

            public MyEnumerable4(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator4 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator4(_log);
            }
        }

        class MyEnumerator4Base
        {
            protected readonly Action<string> _log;
            private int _current;

            public MyEnumerator4Base(Action<string> log)
            {
                _log = log;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerator4 : MyEnumerator4Base, IDisposable
        {
            public MyEnumerator4(Action<string> log)
                : base(log)
            {
            }

            public void Dispose()
            {
                _log("D");
            }
        }

        class MyEnumerable5
        {
            private readonly Action<string> _log;

            public MyEnumerable5(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator5 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator5(_log);
            }
        }

        abstract class MyEnumerator5Base
        {
            protected readonly Action<string> _log;

            public MyEnumerator5Base(Action<string> log)
            {
                _log = log;
            }

            public abstract bool MoveNext();

            public abstract int Current
            {
                get;
            }
        }

        class MyEnumerator5 : MyEnumerator5Base, IDisposable
        {
            private int _current;

            public MyEnumerator5(Action<string> log)
                : base(log)
            {
            }

            public override bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public override int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }

            public void Dispose()
            {
                _log("D");
            }
        }

        class MyEnumerable6
        {
            private readonly Action<string> _log;

            public MyEnumerable6(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator6 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator6(_log);
            }
        }

        struct MyEnumerator6
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator6(Action<string> log)
            {
                _log = log;
                _current = 0;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerable7
        {
            private readonly Action<string> _log;

            public MyEnumerable7(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            public MyEnumerator7 GetEnumerator()
            {
                _log("E");
                return new MyEnumerator7(_log);
            }
        }

        struct MyEnumerator7 : IDisposable
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator7(Action<string> log)
            {
                _log = log;
                _current = 0;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public void Dispose()
            {
                _log("D");
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        class MyEnumerable8 : IEnumerable<int>
        {
            private readonly Action<string> _log;

            public MyEnumerable8(Action<string> log)
            {
                _log = log;
                _log("C");
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                _log("E");
                return new MyEnumerator8(_log);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotSupportedException();
            }
        }

        struct MyEnumerator8 : IEnumerator<int>
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator8(Action<string> log)
            {
                _log = log;
                _current = 0;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public void Dispose()
            {
                _log("D");
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public int Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    throw new NotSupportedException();
                }
            }
        }

        class MyEnumerable9 : IEnumerable
        {
            private readonly Action<string> _log;

            public MyEnumerable9(Action<string> log)
            {
                _log = log;
                _log("C");
            }


            IEnumerator IEnumerable.GetEnumerator()
            {
                _log("E");
                return new MyEnumerator9(_log);
            }
        }

        struct MyEnumerator9 : IEnumerator
        {
            private readonly Action<string> _log;
            private int _current;

            public MyEnumerator9(Action<string> log)
            {
                _log = log;
                _current = 0;
            }

            public bool MoveNext()
            {
                _log("M");
                return _current++ < 3;
            }

            public void Dispose()
            {
                _log("D");
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator.Current
            {
                get
                {
                    _log("C");
                    return _current;
                }
            }
        }

        private void AssertCompile(Func<Func<string, Expression>, Expression, Expression> createExpression, LogAndResult<object> expected)
        {
            var res = WithLog(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void ForEach_Visitor()
        {
            var variable = Expression.Parameter(typeof(int));
            var collection = Expression.Constant(new int[] { 2, 3, 5 });
            var body = Expression.Empty();
            var res = CSharpExpression.ForEach(variable, collection, body);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitForEach(ForEachCSharpStatement node)
            {
                Visited = true;

                return base.VisitForEach(node);
            }
        }
    }
}
