using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PlcInterface;

/// <summary>
/// Represents error that occur during symbol handeling.
/// </summary>
[Serializable]
public sealed class SymbolException : Exception
{
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
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    [ExcludeFromCodeCoverage]
    private SymbolException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}