// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Playground.ReflectionUtils;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Call();
            Invoke();
            New();
            Index();
            NewMultidimensionalArrayInit();
            AsyncLambda();
            While();
            DoWhile();
            Using();
            ForEach();
            For();
            ConditionalMember();
            ConditionalCall();
            ConditionalIndex();
            ConditionalInvoke();
            Dynamic();
        }

        static void Call()
        {
            Call1();
            Call2();
            Call3();
            Call4();
            Call5();
            Call6();
            Call7();
            Call8();
        }

        static void Call1()
        {
            Title();

            var mtd = MethodInfoOf(() => Math.Min(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call2()
        {
            Title();

            var mtd = MethodInfoOf(() => Math.Min(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call3()
        {
            Title();

            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call4()
        {
            Title();

            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var call = CSharpExpression.Call(mtd, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call5()
        {
            Title();

            var mtd = MethodInfoOf(() => F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];
            var val3 = mtd.GetParameters()[2];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var call = CSharpExpression.Call(mtd, arg2, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call6()
        {
            Title();

            var mtd = MethodInfoOf((Bar b) => b.F(default(int), default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];
            var val3 = mtd.GetParameters()[2];

            var obj = Log(Expression.Constant(new Bar()), "O");
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var call = CSharpExpression.Call(obj, mtd, arg2, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(call).Compile()();

            Console.WriteLine(res);
        }

        static void Call7()
        {
            Title();

            var x = default(int);
            var mtd = MethodInfoOf((string s) => int.TryParse(s, out x));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var i = Expression.Parameter(typeof(int));
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant("42"), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(i, "B"));

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(Expression.Block(new[] { i }, call, i)).Compile()();

            Console.WriteLine(res);
        }

        static void Call8()
        {
            Title();

            var x = default(int);
            var mtd = MethodInfoOf((string s) => int.TryParse(s, out x));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var i = Expression.Parameter(typeof(int));
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant("42"), "A"));
            var arg1 = CSharpExpression.Bind(val2, i);

            var call = CSharpExpression.Call(mtd, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(Expression.Block(new[] { i }, call, i)).Compile()();

            Console.WriteLine(res);
        }

        static void Invoke()
        {
            Invoke1();
            Invoke2();
        }

        static void Invoke1()
        {
            Title();

            var f = new Func<int, int, int>((a, b) => a + b);
            var mtd = MethodInfoOf(() => f.Invoke(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var obj = Log(Expression.Constant(f), "O");
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var invoke = CSharpExpression.Invoke(obj, arg1, arg0);

            var res = Expression.Lambda<Func<int>>(invoke).Compile()();

            Console.WriteLine(res);
        }

        static void Invoke2()
        {
            Title();

            var f = new Func<int, int, int>((a, b) => a + b);
            var mtd = MethodInfoOf(() => f.Invoke(default(int), default(int)));

            var val1 = mtd.GetParameters()[0];
            var val2 = mtd.GetParameters()[1];

            var obj = Log(Expression.Constant(f), "O");
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));

            var invoke = CSharpExpression.Invoke(obj, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(invoke).Compile()();

            Console.WriteLine(res);
        }

        static void New()
        {
            New1();
            New2();
        }

        static void New1()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var val1 = ctor.GetParameters()[0];
            var val2 = ctor.GetParameters()[1];
            var val3 = ctor.GetParameters()[2];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var @new = CSharpExpression.New(ctor, arg2, arg0, arg1);

            var res = Expression.Lambda<Func<TimeSpan>>(@new).Compile()();

            Console.WriteLine(res);
        }

        static void New2()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new TimeSpan(default(int), default(int), default(int)));

            var val1 = ctor.GetParameters()[0];
            var val2 = ctor.GetParameters()[1];
            var val3 = ctor.GetParameters()[2];

            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var @new = CSharpExpression.New(ctor, arg0, arg1, arg2);

            var res = Expression.Lambda<Func<TimeSpan>>(@new).Compile()();

            Console.WriteLine(res);
        }

        static void Index()
        {
            Index1();
            Index2();
        }

        static void Index1()
        {
            Title();

            var get = MethodInfoOf((Field f) => f[default(int), default(int), default(int)]);
            var idx = get.DeclaringType.GetProperty(get.Name.Substring("get_".Length));

            var val1 = idx.GetIndexParameters()[0];
            var val2 = idx.GetIndexParameters()[1];
            var val3 = idx.GetIndexParameters()[2];

            var obj = Log(Expression.Constant(new Field()), "O");
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var index = CSharpExpression.Index(obj, idx, arg2, arg0, arg1);

            var res = Expression.Lambda<Func<int>>(index).Compile()();

            Console.WriteLine(res);
        }

        static void Index2()
        {
            Title();

            var get = MethodInfoOf((Field f) => f[default(int), default(int), default(int)]);
            var idx = get.DeclaringType.GetProperty(get.Name.Substring("get_".Length));

            var val1 = idx.GetIndexParameters()[0];
            var val2 = idx.GetIndexParameters()[1];
            var val3 = idx.GetIndexParameters()[2];

            var obj = Log(Expression.Constant(new Field()), "O");
            var arg0 = CSharpExpression.Bind(val1, Log(Expression.Constant(1), "A"));
            var arg1 = CSharpExpression.Bind(val2, Log(Expression.Constant(2), "B"));
            var arg2 = CSharpExpression.Bind(val3, Log(Expression.Constant(3), "C"));

            var index = CSharpExpression.Index(obj, idx, arg0, arg1, arg2);

            var res = Expression.Lambda<Func<int>>(index).Compile()();

            Console.WriteLine(res);
        }

        static void NewMultidimensionalArrayInit()
        {
            Title();

            var expr = CSharpExpression.NewMultidimensionalArrayInit(typeof(int), new[] { 2, 3, 5 }, Enumerable.Range(0, 30).Select(i => Expression.Constant(i)));

            var res = Expression.Lambda<Func<int[,,]>>(expr).Compile()();

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 5; k++)
                    {
                        var e = expr.GetExpression(i, j, k);
                        var v = res.GetValue(i, j, k);
                        Console.WriteLine(e + " = " + v);
                    }
                }
            }
        }

        static void AsyncLambda()
        {
            AsyncLambda1();
            AsyncLambda2();
            AsyncLambda3();
            AsyncLambda4();
            AsyncLambda5();
            AsyncLambda6();
            AsyncLambda7();
            AsyncLambda8();
            AsyncLambda9();
            AsyncLambda10();
            AsyncLambda11();
            AsyncLambda12();
            AsyncLambda13();
            AsyncLambda14();
            AsyncLambda15();
            AsyncLambda16();
        }

        static void AsyncLambda1()
        {
            Title();

            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(Expression.Constant(42));
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda2()
        {
            Title();

            var async = CSharpExpression.AsyncLambda(Expression.Constant(42));
            var res = (Task<int>)async.Compile().DynamicInvoke();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda3()
        {
            Title();

            var await = CSharpExpression.Await(Expression.Constant(Task.FromResult(42)));
            var async = CSharpExpression.AsyncLambda(await);
            var res = (Task<int>)async.Compile().DynamicInvoke();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda4()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(100));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    CSharpExpression.Await(delay.Body),
                    CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda5()
        {
            Title();

            var i = Expression.Parameter(typeof(int));
            var delay = (Expression<Action>)(() => Task.Delay(100));
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var brk = Expression.Label();
            var cnt = Expression.Label();
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.Block(
                    new[] { i },
                    Expression.Assign(i, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(Expression.Equal(i, Expression.Constant(10)), Expression.Break(brk)),
                            CSharpExpression.Await(delay.Body),
                            Expression.Call(cout, i),
                            Expression.PostIncrementAssign(i)
                        ), brk, cnt
                    )
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda6()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.Block(
                    Expression.TryCatch(
                        CSharpExpression.Await(delay.Body),
                        Expression.Catch(
                            Expression.Parameter(typeof(Exception)),
                            Expression.Empty()
                        )
                    ),
                    CSharpExpression.Await(Expression.Constant(Task.FromResult(42)))
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda7()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("T")),
                        Expression.Constant(42)
                    ),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("FE"))
                    )
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda8()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryFault(
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("T")),
                        Expression.Constant(42)
                    ),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("FE"))
                    )
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda9()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryCatch(
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("T")),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                        ),
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("FB")),
                            CSharpExpression.Await(delay.Body),
                            Expression.Call(cout, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Catch(
                        Expression.Parameter(typeof(Exception)),
                        Expression.Constant(0)
                    )
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda10()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task<int>>>(
                Expression.TryCatch(
                    Expression.TryFault(
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("T")),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0))
                        ),
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("FB")),
                            CSharpExpression.Await(delay.Body),
                            Expression.Call(cout, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Catch(
                        Expression.Parameter(typeof(Exception)),
                        Expression.Constant(0)
                    )
                )
            );
            var res = async.Compile()();
            Console.WriteLine(res.Result);
        }

        static void AsyncLambda11()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("TB")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("TE"))
                    ),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("FE"))
                    )
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda12()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("TB1")),
                        CSharpExpression.Await(delay.Body),
                        Expression.TryFinally(
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("TB2")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Constant("TE2"))
                            ),
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("FB2")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Constant("FE2"))
                            )
                        ),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("TE1"))
                    ),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB1")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("FE1"))
                    )
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda13()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("TB1")),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("TE1"))
                    ),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB1")),
                        CSharpExpression.Await(delay.Body),
                        Expression.TryFinally(
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("TB2")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Constant("TE2"))
                            ),
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("FB2")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Constant("FE2"))
                            )
                        ),
                        CSharpExpression.Await(delay.Body),
                        Expression.Call(cout, Expression.Constant("FE1"))
                    )
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda14()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.TryFinally(
                    Expression.Call(cout, Expression.Constant("T1")),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("FB1")),
                        Expression.TryFinally(
                            Expression.Call(cout, Expression.Constant("T2")),
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("FB2")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Constant("FE2"))
                            )
                        ),
                        Expression.Call(cout, Expression.Constant("FE1"))
                    )
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda15()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var lbl = Expression.Label();
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.Block(
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("T")),
                            Expression.Goto(lbl)
                        ),
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("FB")),
                            CSharpExpression.Await(delay.Body),
                            Expression.Call(cout, Expression.Constant("FE"))
                        )
                    ),
                    Expression.Call(cout, Expression.Constant("X")),
                    Expression.Label(lbl),
                    Expression.Call(cout, Expression.Constant("O"))
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void AsyncLambda16()
        {
            Title();

            var delay = (Expression<Action>)(() => Task.Delay(1000));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var lbl = Expression.Label();
            var ex = Expression.Parameter(typeof(DivideByZeroException));
            var async = CSharpExpression.AsyncLambda<Func<Task>>(
                Expression.Block(
                    Expression.TryCatch(
                        Expression.Block(
                            Expression.Call(cout, Expression.Constant("T")),
                            Expression.Divide(Expression.Constant(1), Expression.Constant(0)),
                            Expression.Empty()
                        ),
                        Expression.Catch(
                            ex,
                            Expression.Block(
                                Expression.Call(cout, Expression.Constant("TB")),
                                Expression.Call(cout, Expression.Property(ex, "Message")),
                                CSharpExpression.Await(delay.Body),
                                Expression.Call(cout, Expression.Property(ex, "Message")),
                                Expression.Call(cout, Expression.Constant("TE"))
                            )
                        )
                    ),
                    Expression.Call(cout, Expression.Constant("O"))
                )
            );
            var res = async.Compile()();
            res.Wait();
        }

        static void While()
        {
            Title();

            var i = Expression.Parameter(typeof(int));
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var loop = Expression.Lambda<Action>(
                Expression.Block(
                    new[] { i },
                    Expression.Assign(i, Expression.Constant(0)),
                    CSharpExpression.While(
                        Expression.LessThan(i, Expression.Constant(10)),
                        Expression.Block(
                            Expression.Call(cout, i),
                            Expression.PostIncrementAssign(i)
                        )
                    )
                )
            );
            loop.Compile()();
        }

        static void DoWhile()
        {
            Title();

            var i = Expression.Parameter(typeof(int));
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var loop = Expression.Lambda<Action>(
                Expression.Block(
                    new[] { i },
                    Expression.Assign(i, Expression.Constant(0)),
                    CSharpExpression.Do(
                        Expression.Block(
                            Expression.Call(cout, i),
                            Expression.PostIncrementAssign(i)
                        ),
                        Expression.LessThan(i, Expression.Constant(10))
                    )
                )
            );
            loop.Compile()();
        }

        static void Using()
        {
            Using1();
            Using2();
            Using3();
            Using4();
            Using5();
            Using6();
            Using7();
        }

        static void Using1()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RC(default(string)));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    Expression.New(ctor, Expression.Constant("X")),
                    Expression.Call(cout, Expression.Constant("B"))
                )
            );
            @using.Compile()();
        }

        static void Using2()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RC(default(string)));
            var prnt = MethodInfoOf((RC r) => r.Print());
            var resv = Expression.Parameter(typeof(RC));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    resv,
                    Expression.New(ctor, Expression.Constant("B")),
                    Expression.Call(resv, prnt)
                )
            );
            @using.Compile()();
        }

        static void Using3()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RC(default(string)));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var resv = Expression.Parameter(typeof(IDisposable));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    resv,
                    Expression.New(ctor, Expression.Constant("X")),
                    Expression.Call(cout, Expression.Constant("B"))
                )
            );
            @using.Compile()();
        }

        static void Using4()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RC(default(string)));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var resv = Expression.Parameter(typeof(IDisposable));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    resv,
                    Expression.New(ctor, Expression.Constant("X")),
                    Expression.Block(
                        Expression.Call(cout, Expression.Constant("N")),
                        Expression.Assign(resv, Expression.Constant(null, resv.Type))
                    )
                )
            );
            @using.Compile()();
        }

        static void Using5()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RV(default(string)));
            var prnt = MethodInfoOf((RV r) => r.Print());
            var resv = Expression.Parameter(typeof(RV));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    resv,
                    Expression.New(ctor, Expression.Constant("B")),
                    Expression.Call(resv, prnt)
                )
            );
            @using.Compile()();
        }

        static void Using6()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RV(default(string)));
            var cout = MethodInfoOf(() => Console.WriteLine(default(string))); ;
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    Expression.New(ctor, Expression.Constant("X")),
                    Expression.Call(cout, Expression.Constant("B"))
                )
            );
            @using.Compile()();
        }

        static void Using7()
        {
            Title();

            var ctor = ConstructorInfoOf(() => new RV(default(string)));
            var prnt = MethodInfoOf((RV r) => r.Print());
            var resv = Expression.Parameter(typeof(RV?));
            var @using = Expression.Lambda<Action>(
                CSharpExpression.Using(
                    resv,
                    Expression.Convert(Expression.New(ctor, Expression.Constant("B")), typeof(RV?)),
                    Expression.Call(Expression.Property(resv, "Value"), prnt)
                )
            );
            @using.Compile()();
        }

        static void ForEach()
        {
            ForEach1();
            ForEach2();
            ForEach3();
            ForEach4();
            ForEach5();
        }

        static void ForEach1()
        {
            Title();

            var x = Expression.Parameter(typeof(int));
            var xs = Expression.Constant(new[] { 2, 3, 5 });
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var loop = Expression.Lambda<Action>(
                CSharpExpression.ForEach(x, xs,
                    Expression.Call(cout, x)
                )
            );
            loop.Compile()();
        }

        static void ForEach2()
        {
            Title();

            var x = Expression.Parameter(typeof(int?));
            var xs = Expression.Constant(new[] { 2, 3, 5 });
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var loop = Expression.Lambda<Action>(
                CSharpExpression.ForEach(x, xs,
                    Expression.Call(cout, Expression.Property(x, "Value"))
                )
            );
            loop.Compile()();
        }

        static void ForEach3()
        {
            Title();

            var x = Expression.Parameter(typeof(int));
            var xs = Expression.Constant(new int?[] { 2, 3, 5 });
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var loop = Expression.Lambda<Action>(
                CSharpExpression.ForEach(x, xs,
                    Expression.Call(cout, x)
                )
            );
            loop.Compile()();
        }

        static void ForEach4()
        {
            Title();

            var x = Expression.Parameter(typeof(string));
            var xs = Expression.Constant(new object[] { "bar", "foo", "qux" });
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var loop = Expression.Lambda<Action>(
                CSharpExpression.ForEach(x, xs,
                    Expression.Call(cout, x)
                )
            );
            loop.Compile()();
        }

        static void ForEach5()
        {
            Title();

            var x = Expression.Parameter(typeof(string));
            var xs = Expression.Constant(new List<string> { "bar", "foo", "qux" });
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            var loop = Expression.Lambda<Action>(
                CSharpExpression.ForEach(x, xs,
                    Expression.Call(cout, x)
                )
            );
            loop.Compile()();
        }

        static void For()
        {
            For1();
        }

        static void For1()
        {
            Title();

            var i = Expression.Parameter(typeof(int));
            var init = Expression.Assign(i, Expression.Constant(0));
            var test = Expression.LessThan(i, Expression.Constant(5));
            var iterate = Expression.PostIncrementAssign(i);
            var cout = MethodInfoOf(() => Console.WriteLine(default(int)));
            var body = Expression.Call(cout, i);
            var loop = Expression.Lambda<Action>(
                CSharpExpression.For(new[] { init }, test, new[] { iterate },
                    body
                )
            );
            loop.Compile()();
        }

        static void ConditionalMember()
        {
            ConditionalMember1();
            ConditionalMember2();
            ConditionalMember3();
        }

        static void ConditionalMember1()
        {
            Title();

            var p = Expression.Parameter(typeof(TimeSpan?));
            var e = Expression.Lambda<Func<TimeSpan?, int?>>(CSharpExpression.ConditionalProperty(p, "Seconds"), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(TimeSpan.FromSeconds(42)));
        }

        static void ConditionalMember2()
        {
            Title();

            var p = Expression.Parameter(typeof(string));
            var e = Expression.Lambda<Func<string, int?>>(CSharpExpression.ConditionalProperty(p, "Length"), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f("bar"));
        }

        static void ConditionalMember3()
        {
            Title();

            var p = Expression.Parameter(typeof(DateTimeOffset?));
            var e = Expression.Lambda<Func<DateTimeOffset?, int?>>(CSharpExpression.ConditionalProperty(CSharpExpression.ConditionalProperty(p, "Offset"), "Hours"), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(DateTimeOffset.Now));
        }

        static void ConditionalCall()
        {
            ConditionalCall1();
            ConditionalCall2();
            ConditionalCall3();
        }

        static void ConditionalCall1()
        {
            Title();

            var addYears = MethodInfoOf((DateTime dt) => dt.AddYears(default(int)));
            var p0 = addYears.GetParameters()[0];

            var p = Expression.Parameter(typeof(DateTime?));
            var e = Expression.Lambda<Func<DateTime?, DateTime?>>(CSharpExpression.ConditionalCall(p, addYears, CSharpExpression.Bind(p0, Expression.Constant(1))), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(DateTime.Now));
        }

        static void ConditionalCall2()
        {
            Title();

            var toString = MethodInfoOf((DateTime dt) => dt.ToString());

            var p = Expression.Parameter(typeof(DateTime?));
            var e = Expression.Lambda<Func<DateTime?, string>>(CSharpExpression.ConditionalCall(p, toString), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(DateTime.Now));
        }

        static void ConditionalCall3()
        {
            Title();

            var toUpper = MethodInfoOf((string s) => s.ToUpper());
            var toLower = MethodInfoOf((string s) => s.ToLower());

            var p = Expression.Parameter(typeof(string));
            var e = Expression.Lambda<Func<string, string>>(CSharpExpression.ConditionalCall(CSharpExpression.ConditionalCall(p, toLower), toUpper), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f("bar"));
        }

        static void ConditionalIndex()
        {
            ConditionalIndex1();
        }

        static void ConditionalIndex1()
        {
            Title();

            var index = PropertyInfoOf((List<int> xs) => xs[default(int)]);
            var p0 = index.GetIndexParameters()[0];

            var p = Expression.Parameter(typeof(List<int>));
            var e = Expression.Lambda<Func<List<int>, int?>>(CSharpExpression.ConditionalIndex(p, index, CSharpExpression.Bind(p0, Expression.Constant(0))), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(new List<int> { 42 }));
        }

        static void ConditionalInvoke()
        {
            ConditionalInvoke1();
        }

        static void ConditionalInvoke1()
        {
            Title();

            var p = Expression.Parameter(typeof(Func<int>));
            var e = Expression.Lambda<Func<Func<int>, int?>>(CSharpExpression.ConditionalInvoke(p), p);
            var f = e.Compile();
            Console.WriteLine(f(null));
            Console.WriteLine(f(() => 42));
        }

        static void Dynamic()
        {
            Dynamic1();
            Dynamic2();
            Dynamic3();
            Dynamic4();
            Dynamic5();
            Dynamic6();
            Dynamic7();
            Dynamic8();
            Dynamic9();
        }

        static void Dynamic1()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object>>(DynamicCSharpExpression.DynamicInvokeMember(p, "Substring", Expression.Constant(1)), p);
            var f = e.Compile();
            Console.WriteLine(f("bar"));
        }

        static void Dynamic2()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object>>(DynamicCSharpExpression.DynamicInvoke(p, Expression.Constant(1)), p);
            var f = e.Compile();
            Console.WriteLine(f(new Func<int, int>(x => x + 41)));
        }

        static void Dynamic3()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object>>(DynamicCSharpExpression.MakeDynamicUnary(ExpressionType.Negate, p), p);
            var f = e.Compile();
            Console.WriteLine(f(-42));
            Console.WriteLine(f(TimeSpan.FromSeconds(-42)));
        }

        static void Dynamic4()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object, object>>(DynamicCSharpExpression.MakeDynamicBinary(ExpressionType.Add, p, q), p, q);
            var f = e.Compile();
            Console.WriteLine(f(1, 2));
            Console.WriteLine(f("a", "b"));
            Console.WriteLine(f(DateTime.Now, TimeSpan.FromDays(1)));
        }

        static void Dynamic5()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, TimeSpan>>(DynamicCSharpExpression.DynamicInvokeConstructor(typeof(TimeSpan), p), p);
            var f = e.Compile();
            Console.WriteLine(f(TimeSpan.FromSeconds(42).Ticks));
        }

        static void Dynamic6()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object>>(DynamicCSharpExpression.DynamicGetMember(p, "TotalSeconds"), p);
            var f = e.Compile();
            Console.WriteLine(f(TimeSpan.FromSeconds(42)));
        }

        static void Dynamic7()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var q = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, object, object>>(DynamicCSharpExpression.DynamicGetIndex(p, q), p, q);
            var f = e.Compile();
            Console.WriteLine(f(new[] { 2, 3, 5 }, 2));
            Console.WriteLine(f(new Dictionary<string, int> { { "Bart", 21 } }, "Bart"));
        }

        static void Dynamic8()
        {
            Title();

            var p = Expression.Parameter(typeof(object));
            var e = Expression.Lambda<Func<object, DateTimeOffset>>(DynamicCSharpExpression.DynamicConvert(p, typeof(DateTimeOffset)), p);
            var f = e.Compile();
            Console.WriteLine(f(DateTime.Now));
            Console.WriteLine(f(DateTimeOffset.Now));
        }

        static void Dynamic9()
        {
            Title();

            var await = DynamicCSharpExpression.DynamicAwait(Expression.Constant(Task.FromResult(42), typeof(object)));
            var async = CSharpExpression.AsyncLambda(Expression.Convert(await, typeof(int)));
            var res = (Task<int>)async.Compile().DynamicInvoke();
            Console.WriteLine(res.Result);
        }

        static int F(int x, int y, int z = 42)
        {
            return x * y - z;
        }

        static Expression Log(Expression expression, string log)
        {
            var cout = MethodInfoOf(() => Console.WriteLine(default(string)));
            return Expression.Block(Expression.Call(cout, Expression.Constant(log, typeof(string))), expression);
        }

        static void Title([CallerMemberName]string caller = null)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(caller);
            Console.WriteLine(new string('-', caller.Length));
            Console.ResetColor();
        }
    }

    class Bar
    {
        public int F(int x, int y, int z = 42)
        {
            return x * y - z;
        }
    }

    class Field
    {
        public int this[int x, int y, int z]
        {
            get
            {
                return x + y + z;
            }
        }
    }

    class RC : IDisposable
    {
        private readonly string _message;

        public RC(string message)
        {
            _message = message;
        }

        public void Print()
        {
            Console.WriteLine(_message);
        }

        public void Dispose()
        {
            Console.WriteLine("D");
        }
    }

    struct RV : IDisposable
    {
        private readonly string _message;

        public RV(string message)
        {
            _message = message;
        }

        public void Print()
        {
            Console.WriteLine(_message);
        }

        public void Dispose()
        {
            Console.WriteLine("D");
        }
    }
}