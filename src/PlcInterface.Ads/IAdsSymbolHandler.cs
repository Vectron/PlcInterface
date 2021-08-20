namespace PlcInterface.Ads
{
    /// <summary>
    /// The Ads implementation of a <see cref="ISymbolHandler"/>.
    /// </summary>
    public interface IAdsSymbolHandler : ISymbolHandler
    {
        /// <summary>
        /// Gets the <see cref="ISymbolInfo"/>.
        /// </summary>
        /// <param name="ioName">The tag name.</param>
        /// <returns>The found <see cref="ISymbolInfo"/>.</returns>
        new IAdsSymbolInfo GetSymbolinfo(string ioName);
    }
}