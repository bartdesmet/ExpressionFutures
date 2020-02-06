// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Reflection.Emit;
using System.Reflection;

namespace ClrTest.Reflection
{
    public abstract class ILInstruction
    {
        protected int m_offset;
        protected OpCode m_opCode;

        internal ILInstruction(int offset, OpCode opCode)
        {
            this.m_offset = offset;
            this.m_opCode = opCode;
        }

        public int Offset { get { return m_offset; } }
        public OpCode OpCode { get { return m_opCode; } }

        public abstract void Accept(ILInstructionVisitor vistor);
    }

    public class InlineNoneInstruction : ILInstruction
    {
        internal InlineNoneInstruction(int offset, OpCode opCode)
            : base(offset, opCode)
        { }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineNoneInstruction(this); }
    }

    public class InlineBrTargetInstruction : ILInstruction
    {
        private readonly int m_delta;

        internal InlineBrTargetInstruction(int offset, OpCode opCode, int delta)
            : base(offset, opCode)
        {
            this.m_delta = delta;
        }

        public int Delta { get { return m_delta; } }
        public int TargetOffset { get { return m_offset + m_delta + 1 + 4; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineBrTargetInstruction(this); }
    }

    public class ShortInlineBrTargetInstruction : ILInstruction
    {
        private readonly sbyte m_delta;

        internal ShortInlineBrTargetInstruction(int offset, OpCode opCode, sbyte delta)
            : base(offset, opCode)
        {
            this.m_delta = delta;
        }

        public sbyte Delta { get { return m_delta; } }
        public int TargetOffset { get { return m_offset + m_delta + 1 + 1; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineBrTargetInstruction(this); }
    }

    public class InlineSwitchInstruction : ILInstruction
    {
        private readonly int[] m_deltas;
        private int[] m_targetOffsets;

        internal InlineSwitchInstruction(int offset, OpCode opCode, int[] deltas)
            : base(offset, opCode)
        {
            this.m_deltas = deltas;
        }

        public int[] Deltas { get { return (int[])m_deltas.Clone(); } }
        public int[] TargetOffsets
        {
            get
            {
                if (m_targetOffsets == null)
                {
                    int cases = m_deltas.Length;
                    int itself = 1 + 4 + 4 * cases;
                    m_targetOffsets = new int[cases];
                    for (int i = 0; i < cases; i++)
                        m_targetOffsets[i] = m_offset + m_deltas[i] + itself;
                }
                return m_targetOffsets;
            }
        }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSwitchInstruction(this); }
    }

    public class InlineIInstruction : ILInstruction
    {
        private readonly int m_int32;

        internal InlineIInstruction(int offset, OpCode opCode, int value)
            : base(offset, opCode)
        {
            this.m_int32 = value;
        }

        public int Int32 { get { return m_int32; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineIInstruction(this); }
    }
    public class InlineI8Instruction : ILInstruction
    {
        private readonly long m_int64;

        internal InlineI8Instruction(int offset, OpCode opCode, long value)
            : base(offset, opCode)
        {
            this.m_int64 = value;
        }

        public long Int64 { get { return m_int64; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineI8Instruction(this); }

    }
    public class ShortInlineIInstruction : ILInstruction
    {
        private readonly sbyte m_int8;

        internal ShortInlineIInstruction(int offset, OpCode opCode, sbyte value)
            : base(offset, opCode)
        {
            this.m_int8 = value;
        }

        public sbyte Byte { get { return m_int8; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineIInstruction(this); }
    }

    public class InlineRInstruction : ILInstruction
    {
        private readonly double m_value;

        internal InlineRInstruction(int offset, OpCode opCode, double value)
            : base(offset, opCode)
        {
            this.m_value = value;
        }

        public double Double { get { return m_value; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineRInstruction(this); }
    }

    public class ShortInlineRInstruction : ILInstruction
    {
        private readonly float m_value;

        internal ShortInlineRInstruction(int offset, OpCode opCode, float value)
            : base(offset, opCode)
        {
            this.m_value = value;
        }

        public float Single { get { return m_value; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineRInstruction(this); }
    }

    public class InlineFieldInstruction : ILInstruction
    {
        readonly ITokenResolver m_resolver;
        readonly int m_token;
        FieldInfo m_field;

        internal InlineFieldInstruction(ITokenResolver resolver, int offset, OpCode opCode, int token)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public FieldInfo Field
        {
            get
            {
                if (m_field == null)
                {
                    m_field = m_resolver.AsField(m_token);
                }
                return m_field;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineFieldInstruction(this); }
    }
    public class InlineMethodInstruction : ILInstruction
    {
        private readonly ITokenResolver m_resolver;
        private readonly int m_token;
        private MethodBase m_method;

        internal InlineMethodInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public MethodBase Method
        {
            get
            {
                if (m_method == null)
                {
                    m_method = m_resolver.AsMethod(m_token);
                }
                return m_method;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineMethodInstruction(this); }
    }
    public class InlineTypeInstruction : ILInstruction
    {
        private readonly ITokenResolver m_resolver;
        private readonly int m_token;
        private Type m_type;

        internal InlineTypeInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public Type Type
        {
            get
            {
                if (m_type == null)
                {
                    m_type = m_resolver.AsType(m_token);
                }
                return m_type;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTypeInstruction(this); }
    }
    public class InlineSigInstruction : ILInstruction
    {
        private readonly ITokenResolver m_resolver;
        private readonly int m_token;
        private byte[] m_signature;

        internal InlineSigInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public byte[] Signature
        {
            get
            {
                if (m_signature == null)
                {
                    m_signature = m_resolver.AsSignature(m_token);
                }
                return m_signature;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSigInstruction(this); }
    }
    public class InlineTokInstruction : ILInstruction
    {
        private readonly ITokenResolver m_resolver;
        private readonly int m_token;
        private MemberInfo m_member;

        internal InlineTokInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public MemberInfo Member
        {
            get
            {
                if (m_member == null)
                {
                    m_member = m_resolver.AsMember(Token);
                }
                return m_member;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTokInstruction(this); }
    }

    public class InlineStringInstruction : ILInstruction
    {
        private readonly ITokenResolver m_resolver;
        private readonly int m_token;
        private string m_string;

        internal InlineStringInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
            : base(offset, opCode)
        {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public string String
        {
            get
            {
                if (m_string == null) m_string = m_resolver.AsString(Token);
                return m_string;
            }
        }
        public int Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineStringInstruction(this); }
    }

    public class InlineVarInstruction : ILInstruction
    {
        private readonly ushort m_ordinal;

        internal InlineVarInstruction(int offset, OpCode opCode, ushort ordinal)
            : base(offset, opCode)
        {
            this.m_ordinal = ordinal;
        }

        public ushort Ordinal { get { return m_ordinal; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineVarInstruction(this); }
    }

    public class ShortInlineVarInstruction : ILInstruction
    {
        private readonly byte m_ordinal;

        internal ShortInlineVarInstruction(int offset, OpCode opCode, byte ordinal)
            : base(offset, opCode)
        {
            this.m_ordinal = ordinal;
        }

        public byte Ordinal { get { return m_ordinal; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineVarInstruction(this); }
    }
}
