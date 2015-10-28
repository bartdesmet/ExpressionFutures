// Prototyping support for interpretation of TryExpression with a fault handler or exception filters.
//
// bartde - October 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void LambdaExpressionExtensions_ArgumentChecking()
        {
            AssertEx.Throws<ArgumentNullException>(() => LambdaExpressionExtensions.CompileWithExceptionHandling(default(LambdaExpression)));
            AssertEx.Throws<ArgumentNullException>(() => LambdaExpressionExtensions.CompileWithExceptionHandling(default(Expression<Action>)));
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFault_Simple_Success()
        {
            Verify(
                new LogAndResult<int>
                {
                    Value = 42,
                    Error = null,
                    Log =
                    {
                        "T"
                    }
                },
                addLog =>
                    Expression.TryFault(
                        Expression.Block(
                            addLog("T"),
                            Expression.Constant(42)
                        ),
                        Expression.Block(
                            addLog("F")
                        )
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFault_Simple_Error()
        {
            var ex = new Exception("Oops!");

            Verify(
                new LogAndResult<int>
                {
                    Value = 0,
                    Error = ex,
                    Log =
                    {
                        "T",
                        "F"
                    }
                },
                addLog =>
                    Expression.TryFault(
                        Expression.Block(
                            addLog("T"),
                            Expression.Throw(Expression.Constant(ex), typeof(int))
                        ),
                        Expression.Block(
                            addLog("F")
                        )
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFault_Goto_Success()
        {
            var lblRet = Expression.Label(typeof(int), "lblRet");

            Verify(
                new LogAndResult<int>
                {
                    Value = 42,
                    Error = null,
                    Log =
                    {
                        "TB"
                    }
                },
                addLog =>
                    Expression.Block(
                        Expression.TryFault(
                            Expression.Block(
                                addLog("TB"),
                                Expression.Goto(lblRet, Expression.Constant(42)),
                                addLog("TE")
                            ),
                            Expression.Block(
                                addLog("F")
                            )
                        ),
                        Expression.Label(lblRet, Expression.Constant(-1))
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFault_GotoScoping()
        {
            var lbl = Expression.Label(typeof(void));
            var lblCont = Expression.Label(typeof(void));
            var lblBreak = Expression.Label(typeof(void));
            var lblRet = Expression.Label(typeof(int), "lblRet");

            Verify(
                new LogAndResult<int>
                {
                    Value = 42,
                    Error = null,
                    Log =
                    {
                        "TB"
                    }
                },
                addLog =>
                    Expression.Block(
                        Expression.TryFault(
                            Expression.Block(
                                addLog("TB"),
                                Expression.Goto(lbl),
                                addLog("TX"),
                                Expression.Label(lbl),
                                Expression.Loop(
                                    Expression.Break(lblBreak),
                                    lblBreak, lblCont
                                ),
                                Expression.Goto(lblRet, Expression.Constant(42)),
                                addLog("TE")
                            ),
                            Expression.Block(
                                addLog("F")
                            )
                        ),
                        Expression.Label(lblRet, Expression.Constant(-1))
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFault_Goto_Many()
        {
            var lbl1 = Expression.Label(typeof(void), "lbl1");
            var lbl2 = Expression.Label(typeof(void), "lbl2");
            var lbl3 = Expression.Label(typeof(void), "lbl3");
            var lblRet = Expression.Label(typeof(void), "lblRet");

            var create = new Func<Func<string, Expression>, int, Expression>((addLog, value) =>
                Expression.Block(
                    Expression.TryFault(
                        Expression.Block(
                            addLog("TB"),
                            Expression.Switch(Expression.Constant(value),
                                Expression.SwitchCase(Expression.Goto(lbl1), Expression.Constant(0)),
                                Expression.SwitchCase(Expression.Goto(lbl2), Expression.Constant(1)),
                                Expression.SwitchCase(Expression.Goto(lbl3), Expression.Constant(2))
                            ),
                            addLog("TE")
                        ),
                        Expression.Block(
                            addLog("F")
                        )
                    ),
                    addLog("E"),
                    Expression.Goto(lblRet),
                    Expression.Label(lbl1),
                    addLog("L1"),
                    Expression.Goto(lblRet),
                    Expression.Label(lbl2),
                    addLog("L2"),
                    Expression.Goto(lblRet),
                    Expression.Label(lbl3),
                    addLog("L3"),
                    Expression.Goto(lblRet),
                    Expression.Label(lblRet),
                    addLog("R"),
                    Expression.Constant(null)
                ));

            Verify(
                new LogAndResult<object> { Log = { "TB", "L1", "R" } },
                addLog => create(addLog, 0)
            );

            Verify(
                new LogAndResult<object> { Log = { "TB", "L2", "R" } },
                addLog => create(addLog, 1)
            );

            Verify(
                new LogAndResult<object> { Log = { "TB", "L3", "R" } },
                addLog => create(addLog, 2)
            );

            Verify(
                new LogAndResult<object> { Log = { "TB", "TE", "E", "R" } },
                addLog => create(addLog, 4)
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFilter_Simple_Success()
        {
            var ex = Expression.Parameter(typeof(InvalidOperationException));

            Verify(
                new LogAndResult<int>
                {
                    Value = 42,
                    Error = null,
                    Log =
                    {
                        "T"
                    }
                },
                addLog =>
                    Expression.TryCatch(
                        Expression.Block(
                            addLog("T"),
                            Expression.Constant(42)
                        ),
                        Expression.Catch(
                            ex,
                            Expression.Block(
                                addLog("C"),
                                Expression.Constant(42)
                            ),
                            Expression.Block(
                                addLog("F"),
                                Expression.Constant(true)
                            )
                        )
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFilter_Simple_Error_Handled()
        {
            var err = new InvalidOperationException("Oops!");
            var ex = Expression.Parameter(typeof(InvalidOperationException));

            Verify(
                new LogAndResult<int>
                {
                    Value = 42,
                    Error = null,
                    Log =
                    {
                        "T",
                        "F",
                        "C"
                    }
                },
                addLog =>
                    Expression.TryCatch(
                        Expression.Block(
                            addLog("T"),
                            Expression.Throw(Expression.Constant(err), typeof(int))
                        ),
                        Expression.Catch(
                            ex,
                            Expression.Block(
                                addLog("C"),
                                Expression.Constant(42)
                            ),
                            Expression.Block(
                                addLog("F"),
                                Expression.Constant(true)
                            )
                        )
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFilter_Simple_Error_Unhandled()
        {
            var err = new InvalidOperationException("Oops!");
            var ex = Expression.Parameter(typeof(InvalidOperationException));

            Verify(
                new LogAndResult<int>
                {
                    Value = 0,
                    Error = err,
                    Log =
                    {
                        "T",
                        "F"
                    }
                },
                addLog =>
                    Expression.TryCatch(
                        Expression.Block(
                            addLog("T"),
                            Expression.Throw(Expression.Constant(err), typeof(int))
                        ),
                        Expression.Catch(
                            ex,
                            Expression.Block(
                                addLog("C"),
                                Expression.Constant(42)
                            ),
                            Expression.Block(
                                addLog("F"),
                                Expression.Constant(false)
                            )
                        )
                    )
            );
        }

        [TestMethod]
        public void LambdaExpressionExtensions_TryFilter_Simple_Error_Incompatible()
        {
            var err = new DivideByZeroException("Oops!");
            var ex = Expression.Parameter(typeof(InvalidOperationException));

            Verify(
                new LogAndResult<int>
                {
                    Value = 0,
                    Error = err,
                    Log =
                    {
                        "T"
                    }
                },
                addLog =>
                    Expression.TryCatch(
                        Expression.Block(
                            addLog("T"),
                            Expression.Throw(Expression.Constant(err), typeof(int))
                        ),
                        Expression.Catch(
                            ex,
                            Expression.Block(
                                addLog("C"),
                                Expression.Constant(42)
                            ),
                            Expression.Block(
                                addLog("F"),
                                Expression.Constant(true)
                            )
                        )
                    )
            );
        }

        private static void Verify<T>(LogAndResult<T> expected, Func<Func<string, Expression>, Expression> createExpression)
        {
            var res = WithLog<T>(createExpression).CompileWithExceptionHandling();
            var actual = res();
            Assert.AreEqual(expected, actual);
        }

        private static Expression<Func<LogAndResult<T>>> WithLog<T>(Func<Func<string, Expression>, Expression> createExpression)
        {
            var logParam = Expression.Parameter(typeof(List<string>), "log");
            var valueParam = Expression.Parameter(typeof(T), "result");
            var errorParam = Expression.Parameter(typeof(Exception), "error");
            var exParam = Expression.Parameter(typeof(Exception), "ex");
            var addMethod = logParam.Type.GetMethod("Add", new[] { typeof(string) });
            var logAndResultCtor = typeof(LogAndResult<T>).GetConstructor(new[] { typeof(List<string>), typeof(T), typeof(Exception) });

            var getLogEntry = new Func<string, Expression>(s => Expression.Call(logParam, addMethod, Expression.Constant(s, typeof(string))));
            var body = createExpression(getLogEntry);

            return
                Expression.Lambda<Func<LogAndResult<T>>>(
                    Expression.Block(
                        new[] { logParam, valueParam, errorParam },
                        Expression.Assign(logParam, Expression.New(typeof(List<string>))),
                        Expression.TryCatch(
                            Expression.Block(typeof(void),
                                Expression.Assign(valueParam, body)
                            ),
                            Expression.Catch(exParam,
                                Expression.Block(typeof(void),
                                    Expression.Assign(errorParam, exParam)
                                )
                            )
                        ),
                        Expression.New(logAndResultCtor, logParam, valueParam, errorParam)
                    )
                );
        }

        class LogAndResult<T> : IEquatable<LogAndResult<T>>
        {
            public LogAndResult()
            {
                Log = new List<string>();
            }

            public LogAndResult(List<string> log, T value, Exception error)
            {
                Log = log;
                Value = value;
                Error = error;
            }

            public List<string> Log { get; set; }
            public T Value { get; set; }
            public Exception Error { get; set; }

            public bool Equals(LogAndResult<T> other)
            {
                return Log.SequenceEqual(other.Log) && EqualityComparer<T>.Default.Equals(Value, other.Value) && object.ReferenceEquals(Error, other.Error);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is LogAndResult<T>))
                {
                    return false;
                }

                return Equals((LogAndResult<T>)obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return $@"
Value: {Value}
Error: {Error}
Log: {string.Join(";", Log)}";
            }
        }
    }
}
