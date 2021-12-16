using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
    public sealed class SwitchExpressionException : InvalidOperationException
    {
        public SwitchExpressionException()
            : base("Unmatched value in switch expression.") { }

        public SwitchExpressionException(Exception innerException)
            : base("Unmatched value in switch expression.", innerException)
        {
        }

        public SwitchExpressionException(object unmatchedValue)
            : this()
        {
            UnmatchedValue = unmatchedValue;
        }

        private SwitchExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            UnmatchedValue = info.GetValue(nameof(UnmatchedValue), typeof(object));
        }

        public SwitchExpressionException(string message)
            : base(message) { }

        public SwitchExpressionException(string message, Exception innerException)
            : base(message, innerException) { }

        public object UnmatchedValue { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }

        public override string Message
        {
            get
            {
                if (UnmatchedValue is null)
                {
                    return base.Message;
                }

                string valueMessage = string.Format("Unmatched value: {0}", UnmatchedValue);
                return base.Message + Environment.NewLine + valueMessage;
            }
        }
    }
}
