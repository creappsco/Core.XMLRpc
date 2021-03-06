﻿using Core.XMLRpc.Commons;
using Core.XMLRpc.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Core.XMLRpc.Serializer
{
    /// <summary>
    /// Static class that Allow to Serialize/Deserialize a XML RPC Request/Response
    /// </summary>
    public static class XMLRpcSerializer
    {
        /// <summary>
        /// Serialize a XMLRpcRequest
        /// </summary>
        /// <param name="request">XML Rpc Request Object</param>
        /// <returns></returns>
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
        /// <summary>
        /// Deserialize a Stream into specified T Type
        /// </summary>
        /// <typeparam name="T">Expected Result of T Class</typeparam>
        /// <param name="stream">Stream that contains the XML Content</param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            XmlReader reader = XmlReader.Create(stream, settings);
            return DeserializeParam<T>(reader);
        }
        /// <summary>
        /// Serialize a XML RPC Paramter
        /// </summary>
        /// <param name="xmlWriter">XML Writer that contains XML Stream</param>
        /// <param name="parameter">Parameter to serialize</param>
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
        /// <summary>
        /// Deserialize a <struct></struct> element
        /// </summary>
        /// <typeparam name="T">Expected Type result</typeparam>
        /// <param name="reader">XML Writer that contains XML Stream</param>
        /// <param name="containedInArray">Indicates if a parameter is in Array</param>
        /// <returns>T element</returns>
        private static T DeserializeStruct<T>(XmlReader reader, bool containedInArray = false)
        {
            Dictionary<object, object> structInfo = new Dictionary<object, object>();
            Dictionary<object, XMLRpcType> structDataTypeInfo = new Dictionary<object, XMLRpcType>();
            if (containedInArray)
            {
                reader.MoveToContent();
            }
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
                                        throw new NotImplementedException("");
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
                    property.SetValue(newObject, ConvertToType(property.PropertyType, structInfo[property.Name], property.Name));
                }
            }
            return (T)newObject;
        }
        /// <summary>
        /// Deserialize a <array></array> element
        /// </summary>
        /// <typeparam name="T">Expected Type result</typeparam>
        /// <param name="reader">XML Writer that contains XML Stream</param>
        /// <returns>T element</returns>
        private static T DeserializeArray<T>(XmlReader reader)
        {
            List<object> data = new List<object>();

            reader.MoveToContent();
            if (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //<array>
                    if (reader.Name == "array")
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            var arrayTag = el.FirstNode as XElement;
                            var internalElements = arrayTag.Elements();

                            foreach (var internalElement in internalElements)
                            {
                                Type arrayItemType = typeof(T).GenericTypeArguments[0];
                                var item = DeserializeArrayItem<object>(internalElement.CreateReader(), arrayItemType, containedInArray: true);

                                data.Add(item);
                            }
                        }
                    }
                }
            }

            MethodInfo method = typeof(ListClone).GetMethod("CloneListAs");
            MethodInfo genericMethod = method.MakeGenericMethod(data[0].GetType());
            var reportDS = genericMethod.Invoke(null, new[] { data });

            return (T)reportDS;
        }
        /// <summary>
        /// Deserialize a array item
        /// </summary>
        /// <typeparam name="T">Expected Type result</typeparam>
        /// <param name="reader">XML Writer that contains XML Stream</param>
        /// <param name="arrayItemType">Type of Array Object</param>
        /// <param name="containedInArray">Indicates if a parameter is in Array</param>
        /// <returns>T element</returns>
        public static T DeserializeArrayItem<T>(XmlReader reader, Type arrayItemType, bool containedInArray = false)
        {
            var dataValue = new object();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "value")
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            var value = el.FirstNode as XElement;
                            var dataType = (XMLRpcType)StringEnum.Parse(typeof(XMLRpcType),
                                     value.Name.LocalName, true);
                            switch (dataType)
                            {
                                case XMLRpcType.Int:
                                    dataValue = ConvertToType<Int32>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Boolean:
                                    dataValue = ConvertToType<Boolean>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Double:
                                    dataValue = ConvertToType<Double>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.String:
                                    dataValue = ConvertToType<String>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Datetime:
                                    dataValue = ConvertToType<DateTime>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Array:
                                    throw new NotImplementedException("");
                                case XMLRpcType.Struct:
                                    dataValue = DeserializeParamStruct(el.CreateReader(), arrayItemType, containedInArray);
                                    return (T)dataValue;
                                default:
                                    return (T)dataValue;
                            }
                        }
                    }
                }
            }
            return (T)dataValue;
        }
        /// <summary>
        /// Deserialize a struct member
        /// </summary>
        /// <param name="reader">XML Reader that contains XML Stream</param>
        /// <param name="arrayItemType">Type of Array Object</param>
        /// <param name="containedInArray">Indicates if a parameter is in Array</param>
        /// <returns>Object element</returns>
        private static object DeserializeParamStruct(XmlReader reader, Type arrayItemType, bool containedInArray)
        {
            Dictionary<object, object> structInfo = new Dictionary<object, object>();
            Dictionary<object, XMLRpcType> structDataTypeInfo = new Dictionary<object, XMLRpcType>();
            if (containedInArray)
            {
                reader.MoveToContent();
            }
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

                                    case XMLRpcType.Struct:

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
            PropertyInfo[] arrayPropertyInfos = arrayItemType.GetProperties();
            Type objType = Type.GetType(arrayItemType.AssemblyQualifiedName);
            var newObject = Activator.CreateInstance(objType);
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                if (structInfo.ContainsKey(property.Name))
                {
                    property.SetValue(newObject, ConvertToType(property.PropertyType, structInfo[property.Name], property.Name));
                }
            }
            return newObject;
        }
        /// <summary>
        /// Deserialize a <param></param> element
        /// </summary>
        /// <typeparam name="T">Expected element</typeparam>
        /// <param name="reader">XML Reader that contains XML Stream</param>
        /// <param name="containedInArray">Indicates if a parameter is in Array</param>
        /// <returns></returns>
        private static T DeserializeParam<T>(XmlReader reader, bool containedInArray = false)
        {
            var dataValue = new object();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "value")
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            var value = el.FirstNode as XElement;
                            var dataType = (XMLRpcType)StringEnum.Parse(typeof(XMLRpcType),
                                     value.Name.LocalName, true);
                            switch (dataType)
                            {
                                case XMLRpcType.Int:
                                    dataValue = ConvertToType<Int32>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Boolean:
                                    dataValue = ConvertToType<Boolean>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Double:
                                    dataValue = ConvertToType<Double>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.String:
                                    dataValue = ConvertToType<String>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Datetime:
                                    dataValue = ConvertToType<DateTime>(value.Value);
                                    return (T)dataValue;
                                case XMLRpcType.Array:
                                    var array = DeserializeArray<T>(el.CreateReader());
                                    return array;
                                case XMLRpcType.Struct:
                                    dataValue = DeserializeStruct<T>(el.CreateReader(), containedInArray);
                                    return (T)dataValue;
                                default:
                                    return (T)dataValue;
                            }
                        }
                    }
                    else
                    {
                        if (reader.Name == "fault")
                        {
                            if (XNode.ReadFrom(reader) is XElement el)
                            {
                                var content = el.FirstNode as XElement;

                                var faultObj = DeserializeStruct<XMLRPCException>(content.CreateReader(), true);

                                throw new XMLRPCException(faultObj.FaultCode, faultObj.FaultString);
                            }
                        }
                    }
                }
            }
            return (T)dataValue;
        }
        /// <summary>
        /// Convert a object to T
        /// </summary>
        /// <typeparam name="T">Expected Type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>T object</returns>
        private static object ConvertToType<T>(object value)
        {
            Type clrType = typeof(T);
            switch (clrType.Name)
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
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ConvertToType(Type clrType, object value, string propertyName)
        {
            try
            {
                switch (clrType.Name)
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
                        return Double.Parse(value.ToString().Replace('.', ','));
                    case "DateTime":
                        if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                        {
                            return dateTime;
                        }
                        else
                        {
                            return new DateTime();
                        }
                    case "String":
                        return value.ToString();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"the {propertyName} value:{value} can not converted to {clrType.Name}");
            }
        }
        /// <summary>
        /// Convert a string value with '_' characters to Camel Case Notation
        /// </summary>
        /// <param name="value">Original string</param>
        /// <returns>Converted String</returns>
        private static string NormalizePropertyName(string value)
        {
            return value.Replace("_", " ").ToCamelCase();
        }
    }
}
