using System.Diagnostics.CodeAnalysis;

namespace PlcInterface;

/// <summary>
/// Encapsules the logic to create a type by constructor with parameters, or default constructor
/// with property setters.
/// </summary>
internal interface ITypeActivator
{
    /// <summary>
    /// Try to create a instance.
    /// </summary>
    /// <param name="memberValueGetter">
    /// A <see cref="Func{T, TResult}"/> for getting the value of the member with the given name.
    /// </param>
    /// <param name="memberCount">The number of members.</param>
    /// <param name="instance">The created instance.</param>
    /// <returns><see langword="true"/> if creation was successful, otherwise false.</returns>
    /// <exception cref="SymbolException">is thrown when the data is invalid.</exception>
    bool TryCreateInstance(Func<string, Type, object?> memberValueGetter, int memberCount, [MaybeNullWhen(false)] out object instance);
}
