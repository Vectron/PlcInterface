using System;

namespace PlcInterface
{
    [Serializable]
    public class SymbolException : Exception
    {
        public SymbolException()
        {
        }

        public SymbolException(string message)
            : base(message)
        {
        }

        public SymbolException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SymbolException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}