using System.Collections.Generic;
using System.Linq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads
{
    internal class SymbolInfo : ISymbolInfo
    {
        public SymbolInfo(ISymbol symbol)
        {
            Symbol = symbol;
        }

        public IList<string> ChildSymbols
            => Symbol.SubSymbols.Select(x => x.InstancePath).ToList();

        public string Comment
            => Symbol.Comment;

        public bool IsArray
            => Symbol.DataType?.Category == DataTypeCategory.Array;

        public bool IsBigType
            => Symbol.DataType?.Category == DataTypeCategory.Struct;

        public string Name
            => Symbol.InstancePath;

        public string NameLower
            => Name.ToLower();

        public string ShortName
            => Symbol.InstanceName;

        public ISymbol Symbol
        {
            get;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}