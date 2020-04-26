using PlcInterface.S7.SymbolExporter.OpenssExport;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PlcInterface.S7.SymbolExporter
{
    internal static class XmlNodeExtensions
    {
        public static T Deserialize<T>(this XmlNode data) where T : class, new()
        {
            if (data == null)
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(T));
            serializer.UnknownElement += (o, e) =>
            {
                switch (e.ObjectBeingDeserialized)
                {
                    case AttributeList attributeList:
                        AddToList(attributeList.Items, e.Element);
                        break;

                    case MemberAttributeList attributeList:
                        AddToList(attributeList.Attributes, e.Element);
                        break;

                    default:
                        break;
                }
            };
            using (var xmlNodeReader = new XmlNodeReader(data))
            {
                return (T)serializer.Deserialize(xmlNodeReader);
            }
        }

        private static void AddToList<T>(IList<T> items, XmlElement element)
            where T : UnknownItem
        {
            using (var stringReader = new StringReader(element.OuterXml))
            {
                var thingSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(element.Name));
                var item = (T)thingSerializer.Deserialize(stringReader);

                //Name can't be mapped for us, assign this manually
                item.ElementName = element.Name;
                items.Add(item);
            }
        }
    }
}