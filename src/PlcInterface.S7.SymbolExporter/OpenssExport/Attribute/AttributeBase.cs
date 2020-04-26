using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport.Attribute
{
    public class AttributeBase : UnknownItem
    {
        [XmlAttribute(AttributeName = "Informative")]
        public string Informative
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "SystemDefined")]
        public string SystemDefined
        {
            get;
            set;
        }
    }
}