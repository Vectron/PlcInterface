using System;

namespace PlcInterface
{
    /// <summary>
    /// Represents error that occur during symbol handeling.
    /// </summary>
    [Serializable]
    public class SymbolException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        public SymbolException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SymbolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public SymbolException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected SymbolException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}