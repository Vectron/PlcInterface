namespace PlcInterface.Ads.Extensions
{
    internal static class ISymbolInfoExtension
    {
        public static SymbolInfo ConvertAndValidate(this ISymbolInfo symbolInfo)
        {
            if (!(symbolInfo is SymbolInfo symbol))
            {
                throw new SymbolException($"symbol is not a {typeof(SymbolInfo)}");
            }

            return symbol;
        }
    }
}