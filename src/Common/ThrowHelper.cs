using System;
using System.Diagnostics.CodeAnalysis;

namespace PlcInterface;

/// <summary>
/// Extension methods for all classes and structs.
/// </summary>
internal static class ThrowHelper
{
    /// <summary>
    /// Throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="ioName">The name of the io that was being read.</param>
    /// <exception cref="InvalidOperationException">Throws this always.</exception>
    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_FailedToRead(string ioName)
        => throw new InvalidOperationException($"Failed to read {ioName}");
}
