using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Core.XMLRpc.Serializer
{
    public static class XMLRpcSerializer
    {
        public static string SerializeObject(XMLRpcRequest request)
        {
            using (var stream = new MemoryStream())
            {
                XmlWriter xmlWriter = new XmlTextWriter(stream, request.Encoding);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("", "methodCall", "");

                xmlWriter.WriteStartElement("", "methodName", "");
                xmlWriter.WriteValue(request.MethodName);
                xmlWriter.WriteEndElement(); //Close methodName
                xmlWriter.WriteStartElement("", "params", "");
                foreach (var parameter in request.Parameters)
                {
                    SerializeParameter(xmlWriter, parameter);
                }

                xmlWriter.WriteEndElement(); //Close params
                xmlWriter.WriteEndDocument();//Close methodCall
                xmlWriter.Flush();

                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                string bodyContent = reader.ReadToEnd();
                xmlWriter.Close();
                return bodyContent;
            }
        }

        private static void SerializeParameter(XmlWriter xmlWriter, IXMLRpcParameter parameter)
        {
            if (parameter.IsArray)
            {
                xmlWriter.WriteStartElement("", "param", "");
                xmlWriter.WriteStartElement("", "value", "");

                xmlWriter.WriteStartElement("", "array", "");
                xmlWriter.WriteStartElement("", "data", "");
                var list = parameter.GetObjectValue() as XMLRpcParamList<IXMLRpcParameter>;
                foreach (var item in list)
                {
                    xmlWriter.WriteStartElement("", "value", "");
                    SerializeParameter(xmlWriter, item);
                    xmlWriter.WriteEndElement(); //Close value interanl 
                }
                xmlWriter.WriteEndElement(); //Close data
                xmlWriter.WriteEndElement(); //Close array

                xmlWriter.WriteEndElement(); //Close Value
                xmlWriter.WriteEndElement(); //Close param
            }
            else
            {
                if (parameter.FilterOption == FilterOption.None)
                {
                    xmlWriter.WriteStartElement("", "param", "");
                    xmlWriter.WriteStartElement("", "value", "");

                    xmlWriter.WriteStartElement("", parameter.GetValueType(), "");
                    xmlWriter.WriteValue(parameter.GetValue());
                    xmlWriter.WriteEndElement(); //Close ValueType

                    xmlWriter.WriteEndElement(); //Close Value
                    xmlWriter.WriteEndElement(); //Close param
                }
                else
                {

                    xmlWriter.WriteStartElement("", "array", "");
                    xmlWriter.WriteStartElement("", "data", "");

                    xmlWriter.WriteStartElement("", "value", "");
                    xmlWriter.WriteStartElement("", "string", "");
                    xmlWriter.WriteValue(parameter.Name);
                    xmlWriter.WriteEndElement(); //Close ValueType
                    xmlWriter.WriteEndElement(); //Close value interanl

                    xmlWriter.WriteStartElement("", "value", "");
                    xmlWriter.WriteStartElement("", "string", "");
                    xmlWriter.WriteValue(StringEnum.GetStringValue(parameter.FilterOption));
                    xmlWriter.WriteEndElement(); //Close ValueType
                    xmlWriter.WriteEndElement(); //Close value interanl

                    xmlWriter.WriteStartElement("", "value", "");
                    xmlWriter.WriteStartElement("", parameter.GetValueType(), "");
                    xmlWriter.WriteValue(parameter.GetValue());
                    xmlWriter.WriteEndElement(); //Close ValueType
                    xmlWriter.WriteEndElement(); //Close value interanl

                    xmlWriter.WriteEndElement(); //Close data
                    xmlWriter.WriteEndElement(); //Close array
                }
            }
        }

        public static T DeserializeStruct<T>(string xmlContent)
        {
            Dictionary<object, object> structInfo = new Dictionary<object, object>();
            Dictionary<object, XMLRpcType> structDataTypeInfo = new Dictionary<object, XMLRpcType>();
            byte[] byteArray = Encoding.ASCII.GetBytes(xmlContent);
            MemoryStream stream = new MemoryStream(byteArray)
            {
                Position = 0
            };
            XmlReader reader = XmlReader.Create(stream);
            if (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //<struct>
                    if (reader.Name == "struct")
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            //get array of <member>
                            var arrayOfProperties = el.Elements();
                            foreach (var property in arrayOfProperties)
                            {
                                //<name>
                                var nameNode = property.FirstNode as XElement;
                                string propertyName = NormalizePropertyName(nameNode.Value);
                                //<value>
                                var valueNode = property.LastNode as XElement;
                                //Get DataType From Tag
                                var dataTypeNode = valueNode.FirstNode as XElement;
                                var dataType = (XMLRpcType)StringEnum.Parse(typeof(XMLRpcType), dataTypeNode.Name.LocalName, true);
                                var readedValue = new object();
                                switch (dataType)
                                {
                                    case XMLRpcType.Int:
                                    case XMLRpcType.Boolean:
                                    case XMLRpcType.Double:
                                    case XMLRpcType.String:
                                    case XMLRpcType.Datetime:
                                        readedValue = dataTypeNode.Value;
                                        break;
                                    case XMLRpcType.Array:
                                        List<XMLRpcResponseDetails> data = new List<XMLRpcResponseDetails>();
                                        var arrayTag = (dataTypeNode as XElement).FirstNode as XElement;
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
                                        readedValue = data;
                                        break;
                                    case XMLRpcType.Struct:
                                        throw new NotImplementedException("");
                                    default:
                                        break;
                                }

                                structInfo.Add(propertyName, readedValue);
                                structDataTypeInfo.Add(propertyName, dataType);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidCastException("Can not find the struct specification");
                    }
                }
            }

            Type tModelType = typeof(T);
            var newObject = Activator.CreateInstance<T>();
            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                if (structInfo.ContainsKey(property.Name))
                {
                    property.SetValue(newObject, ConvertToType(property.PropertyType, structInfo[property.Name]));
                }
            }

            return (T)newObject;
        }



        private static object ConvertToType(Type clrType, object value)
        {
            switch (clrType.Name    )
            {
                case "Int32":
                    return Int32.Parse(value.ToString());
                case "Boolean":
                    if (value.Equals("1"))
                    {
                        return true;
                    }
                    else
                    {
                        if (value.Equals("0"))
                        {
                            return false;
                        }
                    }
                    return Boolean.Parse(value.ToString());
                case "Double":
                    return Double.Parse(value.ToString());
                case "DateTime":
                    return DateTime.Parse(value.ToString());
                case "String":
                    return value.ToString();
                default:
                    return value;
            }
        }

        private static string NormalizePropertyName(string value)
        {
            return value.Replace("_", " ").ToCamelCase();
        }
    }
}
