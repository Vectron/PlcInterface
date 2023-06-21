﻿using System.Diagnostics.CodeAnalysis;

namespace PlcInterface.OpcUa;

/// <summary>
/// The Opc implementation of a <see cref="ISymbolHandler"/>.
/// </summary>
public interface IOpcSymbolHandler : ISymbolHandler
{
    /// <summary>
    /// Gets the <see cref="ISymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <returns>The found <see cref="ISymbolInfo"/>.</returns>
    new IOpcSymbolInfo GetSymbolinfo(string ioName);

    /// <summary>
    /// Try to get the <see cref="IOpcSymbolInfo"/>.
    /// </summary>
    /// <param name="ioName">The tag name.</param>
    /// <param name="symbolInfo">The found <see cref="IOpcSymbolInfo"/>.</param>
    /// <returns><see langword="true"/> when the symbol was found else <see langword="false"/>.</returns>
    bool TryGetSymbolinfo(string ioName, [MaybeNullWhen(false)] out IOpcSymbolInfo symbolInfo);
}