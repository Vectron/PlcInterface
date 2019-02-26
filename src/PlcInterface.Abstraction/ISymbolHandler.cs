using System.Collections.Generic;

namespace PlcInterface
{
    public interface ISymbolHandler
    {
        IReadOnlyCollection<ISymbolInfo> AllSymbols
        {
            get;
        }

        ISymbolInfo GetSymbolinfo(string ioName);
    }
}