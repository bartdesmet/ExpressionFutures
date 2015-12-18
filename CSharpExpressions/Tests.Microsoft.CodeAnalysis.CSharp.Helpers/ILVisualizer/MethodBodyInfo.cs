// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;

namespace ClrTest.Reflection {
    [Serializable]
    public class MethodBodyInfo {
        private int m_methodId;

        private string m_typeName;
        private string m_methodToString;

        private List<string> m_instructions = new List<string>();

        public int Identity {
            get { return m_methodId; }
            set { m_methodId = value; }
        }

        public string TypeName {
            get { return m_typeName; }
            set { m_typeName = value; }
        }

        public string MethodToString {
            get { return m_methodToString; }
            set { m_methodToString = value; }
        }

        public List<string> Instructions {
            get { return m_instructions; }
        }

        void AddInstruction(string inst) {
            m_instructions.Add(inst);
        }

        public static MethodBodyInfo Create(MethodBase method) {
            MethodBodyInfo mbi = new MethodBodyInfo();

            mbi.Identity = method.GetHashCode();
            mbi.TypeName = method.GetType().Name;
            mbi.MethodToString = method.ToString();

            ReadableILStringVisitor visitor = new ReadableILStringVisitor(
                new MethodBodyInfoBuilder(mbi),
                DefaultFormatProvider.Instance);

            ILReader reader = ILReaderFactory.Create(method);
            reader.Accept(visitor);

            return mbi;
        }

        class MethodBodyInfoBuilder : IILStringCollector {
            MethodBodyInfo m_mbi;

            public MethodBodyInfoBuilder(MethodBodyInfo mbi) {
                m_mbi = mbi;
            }

            public void Process(ILInstruction instruction, string operandString) {
                m_mbi.AddInstruction(string.Format("IL_{0:x4}: {1,-10} {2}",
                    instruction.Offset,
                    instruction.OpCode.Name,
                    operandString));
            }
        }
    }

}
