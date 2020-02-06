// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ClrTest.Reflection
{
    public sealed class ILReader : IEnumerable<ILInstruction>, IEnumerable
    {
        #region Static members
        private static readonly Type s_runtimeMethodInfoType = Type.GetType("System.Reflection.RuntimeMethodInfo");
        private static readonly Type s_runtimeConstructorInfoType = Type.GetType("System.Reflection.RuntimeConstructorInfo");

        private static readonly OpCode[] s_OneByteOpCodes;
        private static readonly OpCode[] s_TwoByteOpCodes;

        static ILReader()
        {
            s_OneByteOpCodes = new OpCode[0x100];
            s_TwoByteOpCodes = new OpCode[0x100];

            foreach (FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode opCode = (OpCode)fi.GetValue(null);
                ushort value = (ushort)opCode.Value;
                if (value < 0x100)
                {
                    s_OneByteOpCodes[value] = opCode;
                }
                else if ((value & 0xff00) == 0xfe00)
                {
                    s_TwoByteOpCodes[value & 0xff] = opCode;
                }
            }
        }
        #endregion

        private int m_position;
        private readonly ITokenResolver m_resolver;
        private readonly IILProvider m_ilProvider;
        private readonly byte[] m_byteArray;

        public ILReader(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            Type rtType = method.GetType();
            if (rtType != s_runtimeMethodInfoType && rtType != s_runtimeConstructorInfoType)
            {
                throw new ArgumentException("method must be RuntimeMethodInfo or RuntimeConstructorInfo for this constructor.");
            }

            m_ilProvider = new MethodBaseILProvider(method);
            m_resolver = new ModuleScopeTokenResolver(method);
            m_byteArray = m_ilProvider.GetByteArray();
            m_position = 0;
        }

        public ILReader(IILProvider ilProvider, ITokenResolver tokenResolver)
        {
            m_resolver = tokenResolver;
            m_ilProvider = ilProvider ?? throw new ArgumentNullException(nameof(ilProvider));
            m_byteArray = m_ilProvider.GetByteArray();
            m_position = 0;
        }

        public IILProvider ILProvider
        {
            get { return m_ilProvider; }
        }

        public IEnumerator<ILInstruction> GetEnumerator()
        {
            while (m_position < m_byteArray.Length)
                yield return Next();

            m_position = 0;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        ILInstruction Next()
        {
            int offset = m_position;

            // read first 1 or 2 bytes as opCode
            byte code = ReadByte();

            OpCode opCode;

            if (code != 0xFE)
            {
                opCode = s_OneByteOpCodes[code];
            }
            else
            {
                code = ReadByte();
                opCode = s_TwoByteOpCodes[code];
            }

            int token;

            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    return new InlineNoneInstruction(offset, opCode);

                //The operand is an 8-bit integer branch target.
                case OperandType.ShortInlineBrTarget:
                    sbyte shortDelta = ReadSByte();
                    return new ShortInlineBrTargetInstruction(offset, opCode, shortDelta);

                //The operand is a 32-bit integer branch target.
                case OperandType.InlineBrTarget:
                    int delta = ReadInt32();
                    return new InlineBrTargetInstruction(offset, opCode, delta);

                //The operand is an 8-bit integer: 001F  ldc.i4.s, FE12  unaligned.
                case OperandType.ShortInlineI:
                    sbyte int8 = ReadSByte();
                    return new ShortInlineIInstruction(offset, opCode, int8);

                //The operand is a 32-bit integer.
                case OperandType.InlineI:
                    int int32 = ReadInt32();
                    return new InlineIInstruction(offset, opCode, int32);

                //The operand is a 64-bit integer.
                case OperandType.InlineI8:
                    long int64 = ReadInt64();
                    return new InlineI8Instruction(offset, opCode, int64);

                //The operand is a 32-bit IEEE floating point number.
                case OperandType.ShortInlineR:
                    float float32 = ReadSingle();
                    return new ShortInlineRInstruction(offset, opCode, float32);

                //The operand is a 64-bit IEEE floating point number.
                case OperandType.InlineR:
                    double float64 = ReadDouble();
                    return new InlineRInstruction(offset, opCode, float64);

                //The operand is an 8-bit integer containing the ordinal of a local variable or an argument
                case OperandType.ShortInlineVar:
                    byte index8 = ReadByte();
                    return new ShortInlineVarInstruction(offset, opCode, index8);

                //The operand is 16-bit integer containing the ordinal of a local variable or an argument.
                case OperandType.InlineVar:
                    ushort index16 = ReadUInt16();
                    return new InlineVarInstruction(offset, opCode, index16);

                //The operand is a 32-bit metadata string token.
                case OperandType.InlineString:
                    token = ReadInt32();
                    return new InlineStringInstruction(offset, opCode, token, m_resolver);

                //The operand is a 32-bit metadata signature token.
                case OperandType.InlineSig:
                    token = ReadInt32();
                    return new InlineSigInstruction(offset, opCode, token, m_resolver);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineMethod:
                    token = ReadInt32();
                    return new InlineMethodInstruction(offset, opCode, token, m_resolver);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineField:
                    token = ReadInt32();
                    return new InlineFieldInstruction(m_resolver, offset, opCode, token);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineType:
                    token = ReadInt32();
                    return new InlineTypeInstruction(offset, opCode, token, m_resolver);

                //The operand is a FieldRef, MethodRef, or TypeRef token.
                case OperandType.InlineTok:
                    token = ReadInt32();
                    return new InlineTokInstruction(offset, opCode, token, m_resolver);

                //The operand is the 32-bit integer argument to a switch instruction.
                case OperandType.InlineSwitch:
                    int cases = ReadInt32();
                    int[] deltas = new int[cases];
                    for (int i = 0; i < cases; i++)
                        deltas[i] = ReadInt32();
                    return new InlineSwitchInstruction(offset, opCode, deltas);

                default:
                    throw new BadImageFormatException("unexpected OperandType " + opCode.OperandType);
            }
        }

        public void Accept(ILInstructionVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("argument 'visitor' can not be null");

            foreach (ILInstruction instruction in this)
            {
                instruction.Accept(visitor);
            }
        }

        #region read in operands
        byte ReadByte()
        {
            return (byte)m_byteArray[m_position++];
        }

        sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        ushort ReadUInt16()
        {
            int pos = m_position;
            m_position += 2;
            return BitConverter.ToUInt16(m_byteArray, pos);
        }

        uint ReadUInt32()
        {
            int pos = m_position;
            m_position += 4;
            return BitConverter.ToUInt32(m_byteArray, pos);
        }
        ulong ReadUInt64()
        {
            int pos = m_position;
            m_position += 8;
            return BitConverter.ToUInt64(m_byteArray, pos);
        }

        int ReadInt32()
        {
            int pos = m_position;
            m_position += 4;
            return BitConverter.ToInt32(m_byteArray, pos);
        }
        long ReadInt64()
        {
            int pos = m_position;
            m_position += 8;
            return BitConverter.ToInt64(m_byteArray, pos);
        }

        float ReadSingle()
        {
            int pos = m_position;
            m_position += 4;
            return BitConverter.ToSingle(m_byteArray, pos);
        }
        double ReadDouble()
        {
            int pos = m_position;
            m_position += 8;
            return BitConverter.ToDouble(m_byteArray, pos);
        }
        #endregion
    }
}
