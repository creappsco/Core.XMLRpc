using Core.XMLRpc.Commons;
using Core.XMLRpc.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Core.XMLRpc
{
    public class XMLRpcResponseDetails : IXmlSerializable
    {
        [XmlIgnore]
        public XMLRpcType DataType { get; set; }
        [XmlElement(ElementName = "value")]
        public object Value { get; set; }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "value")
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            var value = el.FirstNode as XElement;
                            this.DataType = (XMLRpcType)StringEnum.Parse(typeof(XMLRpcType),
                                     value.Name.LocalName, true);
                            switch (this.DataType)
                            {
                                case XMLRpcType.Int:
                                case XMLRpcType.Boolean:
                                case XMLRpcType.Double:
                                case XMLRpcType.String:
                                case XMLRpcType.Datetime:
                                    Value = value.Value;
                                    break;
                                case XMLRpcType.Array:
                                    List<XMLRpcResponseDetails> data = new List<XMLRpcResponseDetails>();
                                    var arrayTag = (value as XElement).FirstNode as XElement;
                                    var internalElements = arrayTag.Elements();

                                    foreach (var internalElement in internalElements)
                                    {
                                        var content = internalElement.FirstNode as XElement;
                                        data.Add(new XMLRpcResponseDetails
                                        {
                                            DataType = (XMLRpcType)StringEnum.Parse(typeof(XMLRpcType),
                                                        content.Name.LocalName, true),
                                            Value = content.Value
                                        });
                                    }
                                    this.Value = data;
                                    break;
                                case XMLRpcType.Struct:
                                    throw new NotImplementedException("");
                                //this.Value = XMLRpcSerializer.DeserializeStruct<T>("");
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
