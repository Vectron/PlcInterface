using System.Collections.Generic;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    [XmlRoot(ElementName = "AttributeList")]
    public class AttributeList
    {
        [XmlElement(ElementName = "Interface")]
        public Interface Interface
        {
            get;
            set;
        }

        public List<UnknownItem> Items
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Number")]
        public int Number
        {
            get;
            set;
        }
    }
}