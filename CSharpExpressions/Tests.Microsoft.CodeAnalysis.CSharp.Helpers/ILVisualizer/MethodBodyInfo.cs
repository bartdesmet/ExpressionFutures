// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using System.Linq;

namespace ClrTest.Reflection
{
    [Serializable]
    public class MethodBodyInfo
    {
        private int m_methodId;

        private string m_typeName;
        private string m_methodToString;

        private List<string> m_instructions = new List<string>();

        public int Identity
        {
            get { return m_methodId; }
            set { m_methodId = value; }
        }

        public string TypeName
        {
            get { return m_typeName; }
            set { m_typeName = value; }
        }

        public string MethodToString
        {
            get { return m_methodToString; }
            set { m_methodToString = value; }
        }

        public List<string> Instructions
        {
            get { return m_instructions; }
        }

        public ExceptionInfo[] Exceptions
        {
            get; set;
        }

        void AddInstruction(string inst)
        {
            m_instructions.Add(inst);
        }

        public static MethodBodyInfo Create(MethodBase method)
        {
            MethodBodyInfo mbi = new MethodBodyInfo();

            mbi.Identity = method.GetHashCode();
            mbi.TypeName = method.GetType().Name;
            mbi.MethodToString = ".method " + method.ToIL();

            ILReader reader = ILReaderFactory.Create(method);
            mbi.Exceptions = reader.ILProvider.GetExceptionInfos();

            ReadableILStringVisitor visitor = new ReadableILStringVisitor(
                new MethodBodyInfoBuilder(mbi),
                DefaultFormatProvider.Instance);

            reader.Accept(visitor);

            return mbi;
        }

        class MethodBodyInfoBuilder : IILStringCollector
        {
            MethodBodyInfo m_mbi;
            string m_indent;
            readonly HashSet<int> m_starts;
            readonly Dictionary<int, Type> m_startCatch;
            readonly HashSet<int> m_endCatch;
            readonly HashSet<int> m_ends;
            readonly HashSet<int> m_startFinally;
            readonly HashSet<int> m_startFault;
            readonly HashSet<int> m_startFilter;

            public MethodBodyInfoBuilder(MethodBodyInfo mbi)
            {
                m_mbi = mbi;
                m_indent = "";

                m_starts = new HashSet<int>();
                m_ends = new HashSet<int>();
                m_startFinally = new HashSet<int>();
                m_startFault = new HashSet<int>();
                m_startFilter = new HashSet<int>();
                m_startCatch = new Dictionary<int, Type>();
                m_endCatch = new HashSet<int>();

                foreach (var e in mbi.Exceptions)
                {
                    var finallyOrFault = e.Handlers.Count(h => h.Kind == HandlerKind.Finally || h.Kind == HandlerKind.Fault);
                    var numberOfCatch = e.Handlers.Length - finallyOrFault;

                    m_starts.Add(e.StartAddress);
                    m_ends.Add(e.EndAddress);

                    foreach (var c in e.Handlers)
                    {
                        if (c.Kind == HandlerKind.Finally)
                        {
                            m_startFinally.Add(c.StartAddress);
                        }
                        else if (c.Kind == HandlerKind.Fault)
                        {
                            m_startFault.Add(c.StartAddress);
                        }
                        else if (c.Kind == HandlerKind.Filter)
                        {
                            m_startFilter.Add(c.StartAddress);
                        }
                        else
                        {
                            m_startCatch.Add(c.StartAddress, c.Type);
                        }
                        m_endCatch.Add(c.EndAddress);
                    }
                }
            }

            public void Process(ILInstruction instruction, string operandString)
            {
                if (m_starts.Contains(instruction.Offset))
                {
                    m_mbi.AddInstruction(m_indent + ".try");
                    m_mbi.AddInstruction(m_indent + "{");
                    Indent();
                }

                if (m_ends.Contains(instruction.Offset))
                {
                    Dedent();
                    m_mbi.AddInstruction(m_indent + "}");
                }

                if (m_endCatch.Contains(instruction.Offset))
                {
                    Dedent();
                    m_mbi.AddInstruction(m_indent + "}");
                }

                var t = default(Type);
                if (m_startCatch.TryGetValue(instruction.Offset, out t))
                {
                    m_mbi.AddInstruction(m_indent + $"catch {t.ToIL()}");
                    m_mbi.AddInstruction(m_indent + "{");
                    Indent();
                }

                if (m_startFilter.Contains(instruction.Offset))
                {
                    m_mbi.AddInstruction(m_indent + "filter");
                    m_mbi.AddInstruction(m_indent + "{");
                    Indent();
                }

                if (m_startFinally.Contains(instruction.Offset))
                {
                    m_mbi.AddInstruction(m_indent + "finally");
                    m_mbi.AddInstruction(m_indent + "{");
                    Indent();
                }

                if (m_startFault.Contains(instruction.Offset))
                {
                    m_mbi.AddInstruction(m_indent + "fault");
                    m_mbi.AddInstruction(m_indent + "{");
                    Indent();
                }

                m_mbi.AddInstruction(string.Format("{3}IL_{0:x4}: {1,-10} {2}",
                    instruction.Offset,
                    instruction.OpCode.Name,
                    operandString,
                    m_indent));
            }

            private void Indent()
            {
                m_indent = new string(' ', m_indent.Length + 2);
            }

            private void Dedent()
            {
                m_indent = new string(' ', m_indent.Length - 2);
            }
        }
    }

}
