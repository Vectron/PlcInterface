using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "Member", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class Member
    {
        [XmlElement(ElementName = "AttributeList", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public MemberAttributeList AttributeList
        {
            get; set;
        }

        [XmlAttribute(AttributeName = "Datatype")]
        public string Datatype
        {
            get; set;
        }

        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get; set;
        }

        [XmlAttribute(AttributeName = "Remanence")]
        public string Remanence
        {
            get; set;
        }

        [XmlElement(ElementName = "Sections", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public Sections Sections
        {
            get; set;
        }

        [XmlAttribute(AttributeName = "Version")]
        public string Version
        {
            get; set;
        }
    }
}