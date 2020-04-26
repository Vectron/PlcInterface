using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport.Attribute
{
    [XmlRoot(ElementName = "BooleanAttribute", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class BooleanAttribute : AttributeBase
    {
    }
}