using System.Collections.Generic;
using System.Dynamic;

namespace PlcInterface.OpcUa.Dynamic
{
    internal class DynamicCollection : DynamicObject
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public void Add(string name, object value)
            => dictionary[name] = value;

        public override IEnumerable<string> GetDynamicMemberNames()
                    => dictionary.Keys;

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var propName = CreateIndexedName(indexes);
            return dictionary.TryGetValue(propName, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => dictionary.TryGetValue(binder.Name, out result);

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var propName = CreateIndexedName(indexes);
            Add(propName, value);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Add(binder.Name, value);
            return true;
        }

        private string CreateIndexedName(object[] indexes)
        {
            int index = (int)indexes[0];
            return $"[{index}]";
        }
    }
}