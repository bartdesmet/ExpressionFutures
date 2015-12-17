// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
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

        public T Return()
        {
            if (Error != null)
            {
                ExceptionDispatchInfo.Capture(Error).Throw();
            }

            return Value;
        }

        public bool Equals(LogAndResult<T> other)
        {
            return Log.SequenceEqual(other.Log) && EqualityComparer<T>.Default.Equals(Value, other.Value) && Error?.GetType() == other.Error?.GetType();
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
