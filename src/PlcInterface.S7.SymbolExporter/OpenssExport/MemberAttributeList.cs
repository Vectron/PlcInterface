using PlcInterface.S7.SymbolExporter.OpenssExport.Attribute;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "AttributeList", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class MemberAttributeList
    {
        [XmlElement(typeof(BooleanAttribute), ElementName = "BooleanAttribute", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        [XmlElement(typeof(IntegerAttribute), ElementName = "IntegerAttribute", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public List<AttributeBase> Attributes
        {
            get;
            set;
        }
    }
}