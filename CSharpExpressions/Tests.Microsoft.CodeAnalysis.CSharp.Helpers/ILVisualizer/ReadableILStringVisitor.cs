// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ClrTest.Reflection
{
    public interface IILStringCollector
    {
        void Process(ILInstruction ilInstruction, string operandString);
    }

    public class ReadableILStringToTextWriter : IILStringCollector
    {
        protected TextWriter writer;

        public ReadableILStringToTextWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public virtual void Process(ILInstruction ilInstruction, string operandString)
        {
            this.writer.WriteLine("IL_{0:x4}: {1,-10} {2}",
                ilInstruction.Offset,
                ilInstruction.OpCode.Name,
                operandString);
        }
    }

    public class RawILStringToTextWriter : ReadableILStringToTextWriter
    {
        public RawILStringToTextWriter(TextWriter writer)
            : base(writer)
        {
        }

        public override void Process(ILInstruction ilInstruction, string operandString)
        {
            this.writer.WriteLine("IL_{0:x4}: {1,-4}| {2, -8}",
                ilInstruction.Offset,
                ilInstruction.OpCode.Value.ToString("x2"),
                operandString);
        }
    }

    public abstract class ILInstructionVisitor
    {
        public virtual void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction) { }
        public virtual void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction) { }
        public virtual void VisitInlineIInstruction(InlineIInstruction inlineIInstruction) { }
        public virtual void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction) { }
        public virtual void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction) { }
        public virtual void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction) { }
        public virtual void VisitInlineRInstruction(InlineRInstruction inlineRInstruction) { }
        public virtual void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction) { }
        public virtual void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction) { }
        public virtual void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction) { }
        public virtual void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction) { }
        public virtual void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction) { }
        public virtual void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction) { }
        public virtual void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction) { }
        public virtual void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction) { }
        public virtual void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction) { }
        public virtual void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction) { }
    }

    public class ReadableILStringVisitor : ILInstructionVisitor
    {
        protected IFormatProvider formatProvider;
        protected IILStringCollector collector;

        public ReadableILStringVisitor(IILStringCollector collector)
            : this(collector, DefaultFormatProvider.Instance)
        {
        }

        public ReadableILStringVisitor(IILStringCollector collector, IFormatProvider formatProvider)
        {
            this.formatProvider = formatProvider;
            this.collector = collector;
        }

        public override void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction)
        {
            collector.Process(inlineBrTargetInstruction, formatProvider.Label(inlineBrTargetInstruction.TargetOffset));
        }

        public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction)
        {
            string field;
            try
            {
                field = inlineFieldInstruction.Field.ToIL();
            }
            catch (Exception ex)
            {
                field = "!" + ex.Message + "!";
            }
            collector.Process(inlineFieldInstruction, field);
        }

        public override void VisitInlineIInstruction(InlineIInstruction inlineIInstruction)
        {
            collector.Process(inlineIInstruction, inlineIInstruction.Int32.ToString());
        }

        public override void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction)
        {
            collector.Process(inlineI8Instruction, inlineI8Instruction.Int64.ToString());
        }

        public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction)
        {
            string method;
            try
            {
                method = inlineMethodInstruction.Method.ToIL();
            }
            catch (Exception ex)
            {
                method = "!" + ex.Message + "!";
            }
            collector.Process(inlineMethodInstruction, method);
        }

        public override void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction)
        {
            collector.Process(inlineNoneInstruction, string.Empty);
        }

        public override void VisitInlineRInstruction(InlineRInstruction inlineRInstruction)
        {
            collector.Process(inlineRInstruction, inlineRInstruction.Double.ToString());
        }

        public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction)
        {
            collector.Process(inlineSigInstruction, formatProvider.SigByteArrayToString(inlineSigInstruction.Signature));
        }

        public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction)
        {
            collector.Process(inlineStringInstruction, formatProvider.EscapedString(inlineStringInstruction.String));
        }

        public override void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction)
        {
            collector.Process(inlineSwitchInstruction, formatProvider.MultipleLabels(inlineSwitchInstruction.TargetOffsets));
        }

        public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction)
        {
            string member;
            try
            {
                var prefix = "";
                var token = "";
                switch (inlineTokInstruction.Member.MemberType)
                {
                    case MemberTypes.Method:
                    case MemberTypes.Constructor:
                        prefix = "method ";
                        token = ((MethodBase)inlineTokInstruction.Member).ToIL();
                        break;
                    case MemberTypes.Field:
                        prefix = "field ";
                        token = ((FieldInfo)inlineTokInstruction.Member).ToIL();
                        break;
                    default:
                        token = ((Type)inlineTokInstruction.Member).ToIL();
                        break;
                }

                member = prefix + token;
            }
            catch (Exception ex)
            {
                member = "!" + ex.Message + "!";
            }
            collector.Process(inlineTokInstruction, member);
        }

        public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction)
        {
            string type;
            try
            {
                type = inlineTypeInstruction.Type.ToIL();
            }
            catch (Exception ex)
            {
                type = "!" + ex.Message + "!";
            }
            collector.Process(inlineTypeInstruction, type);
        }

        public override void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction)
        {
            collector.Process(inlineVarInstruction, formatProvider.Argument(inlineVarInstruction.Ordinal));
        }

        public override void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction)
        {
            collector.Process(shortInlineBrTargetInstruction, formatProvider.Label(shortInlineBrTargetInstruction.TargetOffset));
        }

        public override void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction)
        {
            collector.Process(shortInlineIInstruction, shortInlineIInstruction.Byte.ToString());
        }

        public override void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction)
        {
            collector.Process(shortInlineRInstruction, shortInlineRInstruction.Single.ToString());
        }

        public override void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction)
        {
            collector.Process(shortInlineVarInstruction, formatProvider.Argument(shortInlineVarInstruction.Ordinal));
        }
    }

    public class RawILStringVisitor : ReadableILStringVisitor
    {
        public RawILStringVisitor(IILStringCollector collector)
            : this(collector, DefaultFormatProvider.Instance)
        {
        }

        public RawILStringVisitor(IILStringCollector collector, IFormatProvider formatProvider)
            : base(collector, formatProvider)
        {
        }

        public override void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction)
        {
            collector.Process(inlineBrTargetInstruction, formatProvider.Int32ToHex(inlineBrTargetInstruction.Delta));
        }

        public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction)
        {
            collector.Process(inlineFieldInstruction, formatProvider.Int32ToHex(inlineFieldInstruction.Token));
        }

        public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction)
        {
            collector.Process(inlineMethodInstruction, formatProvider.Int32ToHex(inlineMethodInstruction.Token));
        }

        public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction)
        {
            collector.Process(inlineSigInstruction, formatProvider.Int32ToHex(inlineSigInstruction.Token));
        }

        public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction)
        {
            collector.Process(inlineStringInstruction, formatProvider.Int32ToHex(inlineStringInstruction.Token));
        }

        public override void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction)
        {
            collector.Process(inlineSwitchInstruction, "...");
        }

        public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction)
        {
            collector.Process(inlineTokInstruction, formatProvider.Int32ToHex(inlineTokInstruction.Token));
        }

        public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction)
        {
            collector.Process(inlineTypeInstruction, formatProvider.Int32ToHex(inlineTypeInstruction.Token));
        }

        public override void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction)
        {
            collector.Process(inlineVarInstruction, formatProvider.Int16ToHex(inlineVarInstruction.Ordinal));
        }

        public override void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction)
        {
            collector.Process(shortInlineBrTargetInstruction, formatProvider.Int8ToHex(shortInlineBrTargetInstruction.Delta));
        }

        public override void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction)
        {
            collector.Process(shortInlineVarInstruction, formatProvider.Int8ToHex(shortInlineVarInstruction.Ordinal));
        }
    }

    static class ILHelpers
    {
        public static string ToIL(this Type type)
        {
            if (type == null)
            {
                return "";
            }

            if (type.IsArray)
            {
                if (type.GetElementType().MakeArrayType() == type)
                {
                    return ToIL(type.GetElementType()) + "[]";
                }
                else
                {
                    var bounds = string.Join(",", Enumerable.Repeat("...", type.GetArrayRank()));
                    return ToIL(type.GetElementType()) + "[" + bounds + "]";
                }
            }
            else if (type.IsGenericType && !type.IsGenericTypeDefinition && !type.IsGenericParameter /* TODO */)
            {
                var args = string.Join(",", type.GetGenericArguments().Select(ToIL));
                var def = ToIL(type.GetGenericTypeDefinition());
                return def + "<" + args + ">";
            }
            else if (type.IsByRef)
            {
                return ToIL(type.GetElementType()) + "&";
            }
            else if (type.IsPointer)
            {
                return ToIL(type.GetElementType()) + "*";
            }
            else
            {
                var res = default(string);
                if (!s_primitives.TryGetValue(type, out res))
                {
                    res = "[" +  type.Assembly.GetName().Name + "]" + type.FullName;
                    
                    if (type.IsValueType)
                    {
                        res = "valuetype " + res;
                    }
                    else
                    {
                        res = "class " + res;
                    }
                }

                return res;
            }
        }

        public static string ToIL(this MethodBase method)
        {
            if (method == null)
            {
                return "";
            }

            var res = "";

            if (!method.IsStatic)
            {
                res = "instance ";
            }

            var mtd = method as MethodInfo;
            var ret = mtd?.ReturnType ?? typeof(void);

            res += ret.ToIL() + " ";
            res += method.DeclaringType.ToIL();
            res += "::";
            res += method.Name;

            if (method.IsGenericMethod)
            {
                res += "<" + string.Join(",", method.GetGenericArguments().Select(ToIL)) + ">";
            }

            res += "(" + string.Join(",", method.GetParameters().Select(p => ToIL(p.ParameterType))) + ")";

            return res;
        }

        public static string ToIL(this FieldInfo field)
        {
            return field.DeclaringType.ToIL() + "::" + field.Name;
        }

        private static readonly Dictionary<Type, string> s_primitives = new Dictionary<Type, string>
        {
            { typeof(object), "object" },
            { typeof(void), "void" },
            { typeof(IntPtr), "native int" },
            { typeof(UIntPtr), "native uint" },
            { typeof(TypedReference), "typedref" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(float), "float32" },
            { typeof(double), "float64" },
            { typeof(sbyte), "int8" },
            { typeof(short), "int16" },
            { typeof(int), "int32" },
            { typeof(long), "int64" },
            { typeof(byte), "uint8" },
            { typeof(ushort), "uint16" },
            { typeof(uint), "uint32" },
            { typeof(ulong), "uint64" },
        };
    }

}
