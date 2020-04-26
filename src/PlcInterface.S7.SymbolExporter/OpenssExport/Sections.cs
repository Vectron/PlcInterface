using System.Collections.Generic;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "Sections", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class Sections
    {
        [XmlElement(ElementName = "Section", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
        public List<Section> Section
        {
            get; set;
        }
    }
}