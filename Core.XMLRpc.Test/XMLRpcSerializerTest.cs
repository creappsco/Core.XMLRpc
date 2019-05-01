using Core.XMLRpc.App;
using Core.XMLRpc.Commons;
using Core.XMLRpc.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace Core.XMLRpc.Test
{
    public class XMLRpcSerializerTest
    {       
        [Fact]
        public void Can_DeSerialize_LoginResponse()
        {
            //Arrange
            string data = "<methodResponse><params><param><value><string>1</string></value></param></params></methodResponse>";

            //Act
            XmlSerializer serializer = new XmlSerializer(typeof(XMLRpcResponse));

            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            var returned = (XMLRpcResponse)serializer.Deserialize(stream);
            //Assert
            Assert.IsType<XMLRpcResponse>(returned);
            Assert.True(returned.Response.Length == 1);
            Assert.Equal("1", returned.Response[0].Value);
            Assert.Equal("string", StringEnum.GetStringValue(returned.Response[0].DataType));
        }

        [Fact]
        public void Can_DeSerialize_SimpleResponse()
        {
            //Arrange
            string data = "<methodResponse><params><param><value><string>South Dakota</string></value></param></params></methodResponse>";

            //Act
            XmlSerializer serializer = new XmlSerializer(typeof(XMLRpcResponse));

            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            var returned = (XMLRpcResponse)serializer.Deserialize(stream);
            //Assert
            Assert.IsType<XMLRpcResponse>(returned);
            Assert.True(returned.Response.Length == 1);
            Assert.Equal("South Dakota", returned.Response[0].Value);
            Assert.Equal("string", StringEnum.GetStringValue(returned.Response[0].DataType));
        }

        [Fact]
        public void Can_DeSerialize_ArraySimple()
        {
            //Arrange
            string data = "<methodResponse><params><param><value><array><data><value><int>12</int></value><value><string>Egypt</string></value><value><boolean>0</boolean></value><value><int>-31</int></value></data></array></value></param></params></methodResponse>";

            //Act
            XmlSerializer serializer = new XmlSerializer(typeof(XMLRpcResponse));

            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            var returned = (XMLRpcResponse)serializer.Deserialize(stream);
            Assert.IsType<XMLRpcResponse>(returned);
            var returnedDetauls = Assert.IsType<List<XMLRpcResponseDetails>>(returned.Response[0].Value);
            //Assert

            Assert.True(returnedDetauls.Count == 4);
            Assert.Equal("12", returnedDetauls[0].Value);
            Assert.Equal("int", StringEnum.GetStringValue(returnedDetauls[0].DataType));

            Assert.Equal("Egypt", returnedDetauls[1].Value);
            Assert.Equal("string", StringEnum.GetStringValue(returnedDetauls[1].DataType));

            Assert.Equal("0", returnedDetauls[2].Value);
            Assert.Equal("boolean", StringEnum.GetStringValue(returnedDetauls[2].DataType));

            Assert.Equal("-31", returnedDetauls[3].Value);
            Assert.Equal("int", StringEnum.GetStringValue(returnedDetauls[3].DataType));
        }
        [Fact]
        public void Can_DeSerialize_ArraySimple2()
        {
            //Arrange
            string data = @"<methodResponse><params><param><value><array><data><value><int>7569483</int></value><value><int>7569484</int></value><value><int>7569485</int></value><value><int>7569486</int></value><value><int>7569487</int></value><value><int>7569488</int></value><value><int>7569489</int></value><value><int>7569490</int></value><value><int>7569491</int></value><value><int>7569492</int></value><value><int>7569493</int></value><value><int>7569494</int></value></data></array></value></param></params></methodResponse>";

            //Act
            XmlSerializer serializer = new XmlSerializer(typeof(XMLRpcResponse));

            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            var returned = (XMLRpcResponse)serializer.Deserialize(stream);
            Assert.IsType<XMLRpcResponse>(returned);
            var returnedDetauls = Assert.IsType<List<XMLRpcResponseDetails>>(returned.Response[0].Value);
            //Assert
            Assert.True(returnedDetauls.Count == 12);
        }

        [Fact]
        public void Can_DeSerialize_StructSimple()
        {
            //Arrange
            string xmlData = "<struct><member><name>lowerBound</name><value><int>18</int></value></member><member><name>upperBound</name><value><int>139</int></value></member></struct>";
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };
            byte[] byteArray = Encoding.ASCII.GetBytes(xmlData);
            MemoryStream stream = new MemoryStream(byteArray)
            {
                Position = 0
            };
            XmlReader reader = XmlReader.Create(stream, settings);
            //Act
            var data = XMLRpcSerializer.DeserializeStruct<DataXML>(reader);
            //Assert
            Assert.IsType<DataXML>(data);
            Assert.Equal(18, data.LowerBound);
            Assert.Equal(139, data.UpperBound);
        }

        [Fact]
        public void Can_DeSerialize_StructComplex()
        {
            //Arrange
            string xmlData = @"<struct><member><name>display_name</name><value><string>100</string></value></member><member><name>__last_update</name><value><string>2019-04-30 21:12:23</string></value></member><member><name>codigo_requerimiento</name><value><int>100</int></value></member><member><name>active</name><value><boolean>1</boolean></value></member><member><name>id</name><value><int>1545</int></value></member><member><name>tipo_solicitud</name><value><int>1</int></value></member></struct>";
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };
            byte[] byteArray = Encoding.ASCII.GetBytes(xmlData);
            MemoryStream stream = new MemoryStream(byteArray)
            {
                Position = 0
            };
            XmlReader reader = XmlReader.Create(stream, settings);
            //Act
            var data = XMLRpcSerializer.DeserializeStruct<DataXML2>(reader);
            //Assert
            Assert.IsType<DataXML2>(data);
            Assert.Equal(100, data.CodigoRequerimiento);
        }
    }

    public class DataXML
    {
        public int UpperBound { get; set; }
        public int LowerBound { get; set; }
    }

    public class DataXML2
    {
       public bool Active { get; set; }
        public int CodigoRequerimiento { get; set; }
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }
        public int TipoSolicitud { get; set; }
    }
}
