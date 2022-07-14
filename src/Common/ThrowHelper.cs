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
    /// <param name="ioname">The name of the io that was being read.</param>
    /// <exception cref="InvalidOperationException">Throws this always.</exception>
    [DoesNotReturn]
    internal static void ThrowInvallidOperationException_FailedToRead(string ioname)
        => throw new InvalidOperationException($"Failed to read {ioname}");
}