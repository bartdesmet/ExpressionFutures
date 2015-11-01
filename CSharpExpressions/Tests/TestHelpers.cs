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
        public static Expression<Func<LogAndResult<T>>> WithLog<T>(Func<Func<string, Expression>, Expression> createExpression)
        {
            return WithLogValue<T>(log => createExpression(s => log(Expression.Empty(), s)));
        }

        public static Expression<Func<LogAndResult<T>>> WithLogValue<T>(Func<Func<Expression, string, Expression>, Expression> createExpression)
        {
            var logParam = Expression.Parameter(typeof(List<string>), "log");
            var valueParam = Expression.Parameter(typeof(T), "result");
            var errorParam = Expression.Parameter(typeof(Exception), "error");
            var exParam = Expression.Parameter(typeof(Exception), "ex");
            var addMethod = logParam.Type.GetMethod("Add", new[] { typeof(string) });
            var logAndResultCtor = typeof(LogAndResult<T>).GetConstructor(new[] { typeof(List<string>), typeof(T), typeof(Exception) });

            var getLogEntry = new Func<Expression, string, Expression>((e, s) => Expression.Block(Expression.Call(logParam, addMethod, Expression.Constant(s, typeof(string))), e));
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
