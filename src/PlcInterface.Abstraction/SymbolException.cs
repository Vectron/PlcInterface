namespace PlcInterface;

/// <summary>
/// Represents error that occur during symbol handling.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SymbolException"/> class.
/// </remarks>
/// <param name="message">The message that describes the error.</param>
public sealed class SymbolException(string message) : Exception(message);
