using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "Interface")]
    public class Interface
    {
        [XmlElement(ElementName = "Sections", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public Sections Sections
        {
            get; set;
        }
    }
}