// Taken from http://blogs.msdn.com/b/haibo_luo/archive/2010/04/19/9998595.aspx

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ClrTest.Reflection
{
    public interface IILProvider
    {
        byte[] GetByteArray();
        ExceptionInfo[] GetExceptionInfos();
    }

    public class MethodBaseILProvider : IILProvider
    {
        private readonly MethodBase m_method;
        private byte[] m_byteArray;

        public MethodBaseILProvider(MethodBase method)
        {
            m_method = method;
        }

        public byte[] GetByteArray()
        {
            if (m_byteArray == null)
            {
                MethodBody methodBody = m_method.GetMethodBody();
                m_byteArray = (methodBody == null) ? new Byte[0] : methodBody.GetILAsByteArray();
            }
            return m_byteArray;
        }

        public ExceptionInfo[] GetExceptionInfos()
        {
            return Array.Empty<ExceptionInfo>();
        }
    }

    public class ExceptionInfo
    {
        private static readonly Type s_tyExceptionInfo = typeof(ILGenerator).Assembly.GetType(" System.Reflection.Emit.__ExceptionInfo");
        private static readonly MethodInfo s_miGetStartAddress = s_tyExceptionInfo.GetMethod("GetStartAddress", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetEndAddress = s_tyExceptionInfo.GetMethod("GetEndAddress", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetNumberOfCatches = s_tyExceptionInfo.GetMethod("GetNumberOfCatches", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetCatchAddresses = s_tyExceptionInfo.GetMethod("GetCatchAddresses", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetCatchEndAddresses = s_tyExceptionInfo.GetMethod("GetCatchEndAddresses", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetCatchClass = s_tyExceptionInfo.GetMethod("GetCatchClass", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo s_miGetExceptionTypes = s_tyExceptionInfo.GetMethod("GetExceptionTypes", BindingFlags.NonPublic | BindingFlags.Instance);

        public ExceptionInfo(object o)
        {
            StartAddress = (int)s_miGetStartAddress.Invoke(o, null);
            EndAddress = (int)s_miGetEndAddress.Invoke(o, null);

            var n = (int)s_miGetNumberOfCatches.Invoke(o, null);
            if (n > 0)
            {
                var handlerStart = (int[])s_miGetCatchAddresses.Invoke(o, null);
                var handlerEnd = (int[])s_miGetCatchEndAddresses.Invoke(o, null);
                var catchType = (Type[])s_miGetCatchClass.Invoke(o, null);
                var types = (int[])s_miGetExceptionTypes.Invoke(o, null);

                Handlers = new HandlerInfo[n];

                for (var i = 0; i < n; i++)
                {
                    Handlers[i] = new HandlerInfo(handlerStart[i], handlerEnd[i], catchType[i], types[i]);
                }
            }
            else
            {
                Handlers = Array.Empty<HandlerInfo>();
            }
        }

        public int StartAddress { get; }
        public int EndAddress { get; }
        public HandlerInfo[] Handlers { get; }
    }

    public class HandlerInfo
    {
        public HandlerInfo(int startAddress, int endAddress, Type type, int kind)
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            Type = type;
            Kind = (HandlerKind)kind;
        }

        public int StartAddress { get; }
        public int EndAddress { get; }
        public Type Type { get; }
        public HandlerKind Kind { get; }
    }

    public enum HandlerKind
    {
        None,
        Filter,
        Finally,
        Fault,
        PreserveStack
    }
}
