using System.Collections.Generic;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "Section", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class Section
    {
        [XmlElement(ElementName = "Member", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public List<Member> Member
        {
            get; set;
        }

        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get; set;
        }
    }
}