using System.Collections.Generic;

namespace PlcInterface.S7
{
    internal class SymbolInfo : ISymbolInfo
    {
        public AreaType AreaType
        {
            get;
        }

        public IList<string> ChildSymbols
        {
            get;
        }

        public string Comment
        {
            get;
        }

        public int DBNumber
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
        }

        public string NameLower
        {
            get;
        }

        public string ShortName
        {
            get;
        }

        public int StartAdress
        {
            get;
            internal set;
        }

        public SymbolType SymbolType
        {
            get;
        }
    }
}