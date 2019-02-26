using System.Collections.Generic;
using System.Dynamic;

namespace PlcInterface.OpcUa.Dynamic
{
    internal class DynamicValue : DynamicObject
    {
        private readonly string name;
        private readonly object value;

        public DynamicValue(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
            => new[] { name };

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (name != binder.Name)
            {
                result = null;
                return false;
            }

            result = value;
            return true;
        }
    }
}