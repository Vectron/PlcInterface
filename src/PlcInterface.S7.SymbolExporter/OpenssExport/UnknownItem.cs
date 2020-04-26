using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter.OpenssExport
{
    public class UnknownItem
    {
        [XmlAnyAttribute]
        public List<XmlAttribute> Attributes
        {
            get; set;
        }

        [XmlIgnore]
        public string ElementName
        {
            get;
            set;
        }

        [XmlText]
        public string Text
        {
            get;
            set;
        }

        public override string ToString()
            => ElementName;
    }
}