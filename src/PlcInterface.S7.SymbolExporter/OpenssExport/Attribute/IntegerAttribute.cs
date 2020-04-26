using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport.Attribute
{
    [XmlRoot(ElementName = "IntegerAttribute", Namespace = "http://www.siemens.com/automation/Openness/SW/Interface/v3")]
    public class IntegerAttribute : AttributeBase
    {
    }
}