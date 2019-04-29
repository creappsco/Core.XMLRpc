using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
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
    }
}
