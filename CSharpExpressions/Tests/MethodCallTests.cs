// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Tests.ReflectionUtils;
using static Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class MethodCallTests
    {
        [TestMethod]
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

        [TestMethod]
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

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Method);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(obj, substring, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.AreSame(obj, res.Object);
                Assert.AreEqual(substring, res.Method);
                Assert.AreEqual(typeof(string), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
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

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.IsNull(res.Object);
                Assert.AreEqual(min, res.Method);
                Assert.AreEqual(typeof(int), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }

            {
                var res = CSharpExpression.Call(min, new[] { arg0, arg1 }.AsEnumerable());

                Assert.AreEqual(CSharpExpressionType.Call, res.CSharpNodeType);
                Assert.IsNull(res.Object);
                Assert.AreEqual(min, res.Method);
                Assert.AreEqual(typeof(int), res.Type);
                Assert.IsTrue(res.Arguments.SequenceEqual(new[] { arg0, arg1 }));
            }
        }

        [TestMethod]
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

            Assert.AreSame(res, res.Update(res.Object, res.Arguments));

            var obj1 = Expression.Constant("foo");
            var upd1 = res.Update(obj1, res.Arguments);
            Assert.AreNotSame(upd1, res);
            Assert.AreSame(res.Arguments, upd1.Arguments);
            Assert.AreSame(obj1, upd1.Object);

            var upd2 = res.Update(obj, new[] { arg1, arg0 });
            Assert.AreNotSame(upd2, res);
            Assert.AreSame(res.Object, upd2.Object);
            Assert.IsTrue(upd2.Arguments.SequenceEqual(new[] { arg1, arg0 }));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        // TODO: tests that assert no re-evaluation of writeback arguments

        [TestMethod]
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

        private void AssertCompile<T>(Func<Func<Expression, string, Expression>, Expression> createExpression, LogAndResult<T> expected)
        {
            var res = WithLogValue<T>(createExpression).Compile()();
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void MethodCall_Visitor()
        {
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var valueParameter = cout.GetParameters()[0];
            var value = Expression.Constant(42);
            var res = CSharpExpression.Call(cout, CSharpExpression.Bind(valueParameter, value));

            var v = new V();
            Assert.AreSame(res, v.Visit(res));
            Assert.IsTrue(v.Visited);
        }

        static int F(int x, int y, int z = 42)
        {
            return x * y + z;
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
    }
}
