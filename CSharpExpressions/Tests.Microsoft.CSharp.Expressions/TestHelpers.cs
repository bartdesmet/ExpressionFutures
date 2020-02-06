// Prototyping extended expression trees for C#.
//
// bartde - October 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests
{
    static class TestHelpers
    {
        public static Expression<Func<LogAndResult<object>>> WithLog(Func<Func<string, Expression>, Expression, Expression> createExpression)
        {
            return WithLog<object>((log, append) => Expression.Block(createExpression(log, append), Expression.Default(typeof(object))));
        }

        public static Expression<Func<LogAndResult<object>>> WithLog(Func<Func<string, Expression>, Expression> createExpression)
        {
            return WithLog<object>(log => Expression.Block(createExpression(log), Expression.Default(typeof(object))));
        }

        public static Expression<Func<TArgs, LogAndResult<object>>> WithLog<TArgs>(Func<Func<string, Expression>, Expression, ParameterExpression, Expression> createExpression)
        {
            return WithLog<TArgs, object>((log, append, param) => Expression.Block(createExpression(log, append, param), Expression.Default(typeof(object))));
        }

        public static Expression<Func<TArgs, LogAndResult<object>>> WithLog<TArgs>(Func<Func<string, Expression>, ParameterExpression, Expression> createExpression)
        {
            return WithLog<TArgs, object>((log, param) => Expression.Block(createExpression(log, param), Expression.Default(typeof(object))));
        }

        public static Expression<Func<LogAndResult<T>>> WithLog<T>(Func<Func<string, Expression>, Expression, Expression> createExpression)
        {
            return WithLogValue<T>((log, append) => createExpression(s => log(Expression.Empty(), s), append));
        }

        public static Expression<Func<LogAndResult<T>>> WithLog<T>(Func<Func<string, Expression>, Expression> createExpression)
        {
            return WithLogValue<T>(log => createExpression(s => log(Expression.Empty(), s)));
        }

        public static Expression<Func<TArgs, LogAndResult<TResult>>> WithLog<TArgs, TResult>(Func<Func<string, Expression>, Expression, ParameterExpression, Expression> createExpression)
        {
            return WithLogValue<TArgs, TResult>((log, append, param) => createExpression(s => log(Expression.Empty(), s), append, param));
        }

        public static Expression<Func<TArgs, LogAndResult<TResult>>> WithLog<TArgs, TResult>(Func<Func<string, Expression>, ParameterExpression, Expression> createExpression)
        {
            return WithLogValue<TArgs, TResult>((log, param) => createExpression(s => log(Expression.Empty(), s), param));
        }

        public static Expression<Func<LogAndResult<object>>> WithLogValue(Func<Func<Expression, string, Expression>, Expression, Expression> createExpression)
        {
            return WithLogValue<object>((log, append) => Expression.Block(createExpression(log, append), Expression.Default(typeof(object)), append));
        }

        public static Expression<Func<LogAndResult<object>>> WithLogValue(Func<Func<Expression, string, Expression>, Expression> createExpression)
        {
            return WithLogValue<object>(log => Expression.Block(createExpression(log), Expression.Default(typeof(object))));
        }

        public static Expression<Func<LogAndResult<T>>> WithLogValue<T>(Func<Func<Expression, string, Expression>, Expression> createExpression)
        {
            return WithLogValue<T>((log, append) => createExpression(log));
        }

        public static Expression<Func<TArg, LogAndResult<TResult>>> WithLogValue<TArg, TResult>(Func<Func<Expression, string, Expression>, ParameterExpression, Expression> createExpression)
        {
            return WithLogValue<TArg, TResult>((log, append, p) => createExpression(log, p));
        }

        public static Expression<Func<LogAndResult<TResult>>> WithLogValue<TResult>(Func<Func<Expression, string, Expression>, Expression, Expression> createExpression)
        {
            var logParam = Expression.Parameter(typeof(List<string>), "log");
            var valueParam = Expression.Parameter(typeof(TResult), "result");
            var errorParam = Expression.Parameter(typeof(Exception), "error");
            var exParam = Expression.Parameter(typeof(Exception), "ex");
            var addMethod = logParam.Type.GetMethod("Add", new[] { typeof(string) });
            var logAndResultCtor = typeof(LogAndResult<TResult>).GetConstructor(new[] { typeof(List<string>), typeof(TResult), typeof(Exception) });

            var getLogEntry = new Func<Expression, string, Expression>((e, s) => Expression.Block(Expression.Call(logParam, addMethod, Expression.Constant(s, typeof(string))), e));
            var entryParam = Expression.Parameter(typeof(string));
            var append = Expression.Lambda<Action<string>>(Expression.Call(logParam, logParam.Type.GetMethod("Add"), entryParam), entryParam);

            // NB: InvalidProgramException when we expose the Lambda directly to the caller; e.g. repro on ForEach_Compile_Pattern1.
            //     This is likely due to the Invoke(Lambda) optimization. VSadov may already have solved (cf. recursive lambdas) but
            //     needs to be double-checked for this particular case. Simply #define REPRO to see it.
#if REPRO
            var body = createExpression(getLogEntry, append);
#else
            var appendVar = Expression.Parameter(append.Type);
            var body = createExpression(getLogEntry, appendVar);
#endif

            var res =
                Expression.Lambda<Func<LogAndResult<TResult>>>(
                    Expression.Block(
                        new[]
                        {
                            logParam, valueParam, errorParam,
#if !REPRO
                            appendVar
#endif
                        },
#if !REPRO
                        Expression.Assign(appendVar, append),
#endif
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

            return res;
        }

        public static Expression<Func<TArgs, LogAndResult<TResult>>> WithLogValue<TArgs, TResult>(Func<Func<Expression, string, Expression>, Expression, ParameterExpression, Expression> createExpression)
        {
            var argParam = Expression.Parameter(typeof(TArgs), "arg");
            var logParam = Expression.Parameter(typeof(List<string>), "log");
            var valueParam = Expression.Parameter(typeof(TResult), "result");
            var errorParam = Expression.Parameter(typeof(Exception), "error");
            var exParam = Expression.Parameter(typeof(Exception), "ex");
            var addMethod = logParam.Type.GetMethod("Add", new[] { typeof(string) });
            var logAndResultCtor = typeof(LogAndResult<TResult>).GetConstructor(new[] { typeof(List<string>), typeof(TResult), typeof(Exception) });

            var getLogEntry = new Func<Expression, string, Expression>((e, s) => Expression.Block(Expression.Call(logParam, addMethod, Expression.Constant(s, typeof(string))), e));
            var entryParam = Expression.Parameter(typeof(string));
            var append = Expression.Lambda<Action<string>>(Expression.Call(logParam, logParam.Type.GetMethod("Add"), entryParam), entryParam);

            // NB: InvalidProgramException when we expose the Lambda directly to the caller; e.g. repro on ForEach_Compile_Pattern1.
            //     This is likely due to the Invoke(Lambda) optimization. VSadov may already have solved (cf. recursive lambdas) but
            //     needs to be double-checked for this particular case. Simply #define REPRO to see it.
#if REPRO
            var body = createExpression(getLogEntry, append);
#else
            var appendVar = Expression.Parameter(append.Type);
            var body = createExpression(getLogEntry, appendVar, argParam);
#endif

            var res =
                Expression.Lambda<Func<TArgs, LogAndResult<TResult>>>(
                    Expression.Block(
                        new[]
                        {
                            logParam, valueParam, errorParam,
#if !REPRO
                            appendVar
#endif
                        },
#if !REPRO
                        Expression.Assign(appendVar, append),
#endif
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
                    ),
                    argParam
                );

            return res;
        }
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
        public Func<Exception, bool> ErrorCheck { get; set; }

        public bool Equals(LogAndResult<T> other)
        {
            var err = false;

            if (this.ErrorCheck != null || other.ErrorCheck != null)
            {
                if (this.ErrorCheck != null && other.ErrorCheck != null)
                {
                    throw new InvalidOperationException();
                }

                Func<Exception, bool> check;
                Exception error;

                if (this.ErrorCheck != null)
                {
                    check = this.ErrorCheck;
                    error = other.Error;
                }
                else
                {
                    check = other.ErrorCheck;
                    error = this.Error;
                }

                err = check(error);
            }
            else
            {
                err = object.ReferenceEquals(Error, other.Error);
            }

            return Log.SequenceEqual(other.Log) && EqualityComparer<T>.Default.Equals(Value, other.Value) && err;
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
