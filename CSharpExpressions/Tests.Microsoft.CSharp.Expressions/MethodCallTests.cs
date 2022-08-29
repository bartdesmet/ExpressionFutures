// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    public class MethodCallTests
    {
        [Fact]
        public void MethodCall_Factory_ArgumentChecking()
        {
            // NB: A lot of checks are performed by LINQ helpers, so we omit tests for those cases.

            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var parameters = substring.GetParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var argStartIndex = CSharpExpression.Bind(startIndexParameter, startIndex);
            var argLength = CSharpExpression.Bind(lengthParameter, length);

            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));

            var valueParameter = cout.GetParameters()[0];

            var value = Expression.Constant(42);
            var argValue = CSharpExpression.Bind(valueParameter, value);

            // duplicate
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argStartIndex, argStartIndex));

            // unbound
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argStartIndex));

            // wrong member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.Call(obj, substring, argValue));

            // null
            var bindings = new[] { argStartIndex, argLength };
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(default(MethodInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(default(MethodInfo), bindings.AsEnumerable()));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(obj, default(MethodInfo), bindings));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.Call(obj, default(MethodInfo), bindings.AsEnumerable()));
        }

        [Fact]
        public void MethodCall_Factory_Expression()
        {
            var method = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var args = new[] { Expression.Constant(1), Expression.Constant(2) };

            foreach (var e in new[]
            {
                CSharpExpression.Call(method, args),
                CSharpExpression.Call(method, args.AsEnumerable()),
                CSharpExpression.Call(null, method, args),
                CSharpExpression.Call(null, method, args.AsEnumerable()),
            })
            {
                Assert.Null(e.Object);

                Assert.Equal(method, e.Method);

                Assert.Equal(2, e.Arguments.Count);

                Assert.Equal(method.GetParameters()[0], e.Arguments[0].Parameter);
                Assert.Equal(method.GetParameters()[1], e.Arguments[1].Parameter);

                Assert.Same(args[0], e.Arguments[0].Expression);
                Assert.Same(args[1], e.Arguments[1].Expression);
            }

            var tooLittle = args.Take(1);

            foreach (var f in new Func<MethodCallCSharpExpression>[]
            {
                () => CSharpExpression.Call(method, tooLittle),
                () => CSharpExpression.Call(method, tooLittle.AsEnumerable()),
                () => CSharpExpression.Call(null, method, tooLittle),
                () => CSharpExpression.Call(null, method, tooLittle.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }

            var tooMany = args.Concat(args).ToArray();

            foreach (var f in new Func<MethodCallCSharpExpression>[]
            {
                () => CSharpExpression.Call(method, tooMany),
                () => CSharpExpression.Call(method, tooMany.AsEnumerable()),
                () => CSharpExpression.Call(null, method, tooMany),
                () => CSharpExpression.Call(null, method, tooMany.AsEnumerable()),
            })
            {
                AssertEx.Throws<ArgumentException>(() => f());
            }
        }

        [Fact]
        public void MethodCall_Properties_Instance()
        {
            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var parameters = substring.GetParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            {
                var res = CSharpExpression.Call(obj, substring, arg0, arg1);

                Assert.Equal(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.Same(obj, res.Object);
                Assert.Equal(substring, res.Method);
                Assert.Equal(typeof(string), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(obj, substring, new[] { arg0, arg1 }.AsEnumerable());

                Assert.Equal(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.Same(obj, res.Object);
                Assert.Equal(substring, res.Method);
                Assert.Equal(typeof(string), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [Fact]
        public void MethodCall_Properties_Static()
        {
            var min = MethodInfoOf(() => Math.Min(default(int), default(int)));

            var parameters = min.GetParameters();

            var val1Parameter = parameters[0];
            var val2Parameter = parameters[1];

            var val1 = Expression.Constant(0);
            var val2 = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(val1Parameter, val1);
            var arg1 = CSharpExpression.Bind(val2Parameter, val2);

            {
                var res = CSharpExpression.Call(min, arg0, arg1);

                Assert.Equal(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.Null(res.Object);
                Assert.Equal(min, res.Method);
                Assert.Equal(typeof(int), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(min, new[] { arg0, arg1 }.AsEnumerable());

                Assert.Equal(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.Null(res.Object);
                Assert.Equal(min, res.Method);
                Assert.Equal(typeof(int), res.Type);
                Assert.True(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [Fact]
        public void MethodCall_Update()
        {
            var substring = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var parameters = substring.GetParameters();

            var startIndexParameter = parameters[0];
            var lengthParameter = parameters[1];

            var obj = Expression.Constant("bar");
            var startIndex = Expression.Constant(0);
            var length = Expression.Constant(1);

            var arg0 = CSharpExpression.Bind(startIndexParameter, startIndex);
            var arg1 = CSharpExpression.Bind(lengthParameter, length);

            var res = CSharpExpression.Call(obj, substring, arg0, arg1);

            Assert.Same(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Constant("foo");
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.NotSame(upd1, res);
            Assert.Same(res.Arguments, upd1.Arguments);
            Assert.Same(obj1, upd1.Object);

            var upd2 = res.Update(obj, new[] { arg1, arg0 });
            Assert.NotSame(upd2, res);
            Assert.Same(res.Object, upd2.Object);
            Assert.True(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [Fact]
        public void MethodCall_Compile_Static()
        {
            var method = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];
            var parameterZ = parameters[2];

            var valueX = Expression.Constant(1);
            var valueY = Expression.Constant(2);
            var valueZ = Expression.Constant(3);

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterY, log(valueY, "Y"))
                ),
                new LogAndResult<int> { Value = F(1, 2), Log = { "X", "Y" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, log(valueX, "X"))
                ),
                new LogAndResult<int> { Value = F(1, 2), Log = { "Y", "X" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(1, 2, 3), Log = { "Y", "X", "Z" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, log(valueX, "X")),
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(1, 2, 3), Log = { "X", "Y", "Z" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_Instance()
        {
            var method = MethodInfoOf((string s) => s.Substring(default(int), default(int)));

            var parameters = method.GetParameters();

            var parameterStartIndex = parameters[0];
            var parameterLength = parameters[1];

            var obj = Expression.Constant("foobar");
            var valueStartIndex = Expression.Constant(1);
            var valueLength = Expression.Constant(2);

            AssertCompile<string>(log =>
                CSharpExpression.Call(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S")),
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "S", "L" } }
            );

            AssertCompile<string>(log =>
                CSharpExpression.Call(log(obj, "O"), method,
                    CSharpExpression.Bind(parameterLength, log(valueLength, "L")),
                    CSharpExpression.Bind(parameterStartIndex, log(valueStartIndex, "S"))
                ),
                new LogAndResult<string> { Value = "foobar".Substring(1, 2), Log = { "O", "L", "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_NoCopyOptimization()
        {
            var method = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];
            var parameterZ = parameters[2];

            var valueX = Expression.Constant(1);
            var valueY = Expression.Constant(2);
            var valueZ = Expression.Constant(3);

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, valueX),
                    CSharpExpression.Bind(parameterY, valueY)
                ),
                new LogAndResult<int> { Value = F(1, 2) }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, valueY),
                    CSharpExpression.Bind(parameterX, valueX)
                ),
                new LogAndResult<int> { Value = F(1, 2) }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, valueY),
                    CSharpExpression.Bind(parameterX, valueX),
                    CSharpExpression.Bind(parameterZ, valueZ)
                ),
                new LogAndResult<int> { Value = F(1, 2, 3) }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterX, valueX),
                    CSharpExpression.Bind(parameterY, valueY),
                    CSharpExpression.Bind(parameterZ, valueZ)
                ),
                new LogAndResult<int> { Value = F(1, 2, 3) }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, valueX),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(1, 2, 3), Log = { "Y", "Z" } }
            );

            AssertCompile<int>(log =>
                CSharpExpression.Call(method,
                    CSharpExpression.Bind(parameterY, log(valueY, "Y")),
                    CSharpExpression.Bind(parameterX, Expression.Default(typeof(int))),
                    CSharpExpression.Bind(parameterZ, log(valueZ, "Z"))
                ),
                new LogAndResult<int> { Value = F(0, 2, 3), Log = { "Y", "Z" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Variable()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            AssertCompile<int>(log =>
                Expression.Block(new[] { valueY },
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterX, log(valueX, "S")),
                        CSharpExpression.Bind(parameterY, valueY)
                    ),
                    valueY
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );

            AssertCompile<int>(log =>
                Expression.Block(new[] { valueY },
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterX, log(valueX, "S")),
                        CSharpExpression.Bind(parameterY, log(valueY, "X"))
                    ),
                    valueY
                ),
                new LogAndResult<int> { Value = 0, Log = { "S", "X" } } // log(...) is not writeable
            );

            AssertCompile<int>(log =>
                Expression.Block(new[] { valueY },
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, valueY),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    valueY
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );

            AssertCompile<int>(log =>
                Expression.Block(new[] { valueY },
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, log(valueY, "X")),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    valueY
                ),
                new LogAndResult<int> { Value = 0, Log = { "X", "S" } } // log(...) is not writeable
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Field()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var box = Expression.Parameter(typeof(StrongBox<int>));
            var newBox = Expression.Assign(box, Expression.New(box.Type));
            var boxValue = Expression.Field(box, "Value");

            AssertCompile<int>(log =>
                Expression.Block(new[] { box },
                    newBox,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, boxValue),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    boxValue
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Property()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var box = Expression.Parameter(typeof(MyBox<int>));
            var newBox = Expression.Assign(box, Expression.New(box.Type));
            var boxValue = Expression.Property(box, "Value");

            AssertCompile<int>(log =>
                Expression.Block(new[] { box },
                    newBox,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, boxValue),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    boxValue
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Array1()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var array = Expression.Parameter(typeof(int[]));
            var newArray = Expression.Assign(array, Expression.NewArrayBounds(typeof(int), Expression.Constant(1)));
            var element = Expression.ArrayAccess(array, Expression.Constant(0));

            AssertCompile<int>(log =>
                Expression.Block(new[] { array },
                    newArray,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, element),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    element
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Array2()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var array = Expression.Parameter(typeof(int[]));
            var newArray = Expression.Assign(array, Expression.NewArrayBounds(typeof(int), Expression.Constant(1)));
            var element = Expression.ArrayIndex(array, Expression.Constant(0));

            AssertCompile<int>(log =>
                Expression.Block(new[] { array },
                    newArray,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, element),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    element
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Array3()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var array = Expression.Parameter(typeof(int[,]));
            var newArray = Expression.Assign(array, Expression.NewArrayBounds(typeof(int), Expression.Constant(1), Expression.Constant(1)));
            var element = Expression.ArrayIndex(array, Expression.Constant(0), Expression.Constant(0));

            AssertCompile<int>(log =>
                Expression.Block(new[] { array },
                    newArray,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, element),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    element
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_MutableStruct1()
        {
            var y = default(int);
            var method = MethodInfoOf((S s) => s.F(default(int), ref y));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant(12);
            var valueY = Expression.Parameter(typeof(int));

            var value = Expression.Parameter(typeof(S));
            var newValue = Expression.Assign(value, Expression.New(typeof(S)));
            var valueZ = Expression.Field(value, "Z");

            AssertCompile<int>(log =>
                Expression.Block(new[] { value, valueY },
                    newValue,
                    Expression.Assign(valueY, Expression.Constant(30)),
                    CSharpExpression.Call(value, method,
                        CSharpExpression.Bind(parameterX, valueX),
                        CSharpExpression.Bind(parameterY, valueY)
                    ),
                    Expression.Subtract(valueZ, valueY)
                ),
                new LogAndResult<int> { Value = 0 }
            );

            AssertCompile<int>(log =>
                Expression.Block(new[] { value, valueY },
                    newValue,
                    Expression.Assign(valueY, Expression.Constant(30)),
                    CSharpExpression.Call(value, method,
                        CSharpExpression.Bind(parameterY, valueY),
                        CSharpExpression.Bind(parameterX, valueX)
                    ),
                    Expression.Subtract(valueZ, valueY)
                ),
                new LogAndResult<int> { Value = 0 }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_MutableStruct2()
        {
            var y = default(int);
            var method = MethodInfoOf((S s) => s.F(default(int), ref y));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant(12);
            var valueY = Expression.Parameter(typeof(int));

            var value = Expression.Parameter(typeof(StrongBox<S>));
            var prop = Expression.Field(value, "Value");
            var newValue = Expression.Assign(value, Expression.New(typeof(StrongBox<S>)));
            var valueZ = Expression.Field(prop, "Z");

            AssertCompile<int>(log =>
                Expression.Block(new[] { value, valueY },
                    newValue,
                    Expression.Assign(valueY, Expression.Constant(30)),
                    CSharpExpression.Call(prop, method,
                        CSharpExpression.Bind(parameterX, valueX),
                        CSharpExpression.Bind(parameterY, valueY)
                    ),
                    Expression.Subtract(valueZ, valueY)
                ),
                new LogAndResult<int> { Value = 0 }
            );

            AssertCompile<int>(log =>
                Expression.Block(new[] { value, valueY },
                    newValue,
                    Expression.Assign(valueY, Expression.Constant(30)),
                    CSharpExpression.Call(prop, method,
                        CSharpExpression.Bind(parameterY, valueY),
                        CSharpExpression.Bind(parameterX, valueX)
                    ),
                    Expression.Subtract(valueZ, valueY)
                ),
                new LogAndResult<int> { Value = 0 }
            );
        }

        [Fact]
        public void MethodCall_Compile_ByRef_Index()
        {
            var method = MethodInfoOf((int x) => int.TryParse("", out x));

            var parameters = method.GetParameters();

            var parameterX = parameters[0];
            var parameterY = parameters[1];

            var valueX = Expression.Constant("42");
            var valueY = Expression.Parameter(typeof(int));

            var list = Expression.Parameter(typeof(List<int>));
            var newList = Expression.Assign(list, Expression.ListInit(Expression.New(typeof(List<int>)), Expression.Constant(1)));
            var element = Expression.MakeIndex(list, typeof(List<int>).GetProperty("Item"), new[] { Expression.Constant(0) });

            AssertCompile<int>(log =>
                Expression.Block(new[] { list },
                    newList,
                    CSharpExpression.Call(method,
                        CSharpExpression.Bind(parameterY, element),
                        CSharpExpression.Bind(parameterX, log(valueX, "S"))
                    ),
                    element
                ),
                new LogAndResult<int> { Value = 42, Log = { "S" } }
            );
        }

        [Fact]
        public void MethodCall_Compile_EvalOrder()
        {
            var a = Expression.Parameter(typeof(int));
            var m = MethodInfoOf(() => G(0, 0, 0));
            var ps = m.GetParameters();

            var a1 = CSharpExpression.Bind(ps[1], Expression.Assign(a, Expression.Constant(1)));
            var a2 = CSharpExpression.Bind(ps[0], a);
            var a3 = CSharpExpression.Bind(ps[2], Expression.Assign(a, Expression.Constant(2)));

            var e =
                Expression.Block(
                    new[] { a },
                    // G(y: a = 1, x: a, z: a = 2)
                    CSharpExpression.Call(null, m, a1, a2, a3)
                );

            var res = Expression.Lambda<Func<string>>(e).Compile()();
            Assert.Equal("1,1,2", res);
        }

        // TODO: tests that assert no re-evaluation of writeback arguments

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.Equal(expected, res);
        }

        [Fact]
        public void MethodCall_Visitor()
        {
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var valueParameter = cout.GetParameters()[0];
            var value = Expression.Constant(42);
            var res = CSharpExpression.Call(cout, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        static int F(int x, int y, int z = 42)
        {
            return x * y + z;
        }

        static string G(int x, int y, int z)
        {
            return $"{x},{y},{z}";
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitMethodCall(MethodCallCSharpExpression node)
            {
                Visited = true;

                return base.VisitMethodCall(node);
            }
        }

        class MyBox<T>
        {
            public T Value { get; set; }
        }

        struct S
        {
            public int Z;

            public void F(int x, ref int y)
            {
                Z = x + y;
                y = Z;
            }
        }
    }
}
