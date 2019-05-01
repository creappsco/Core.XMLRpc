using Core.XMLRpc.App;
using Core.XMLRpc.Commons;
using Core.XMLRpc.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Xunit;

namespace Core.XMLRpc.Test
{
    public class XMLRpcSerializerTest
    {
        [Fact]
        public void Can_Serialize_Login()
        {
            //Arrange
            XMLRpcRequest rpcRequest = new XMLRpcRequest
            {
                MethodName = "login",
                Encoding = Encoding.UTF8,
                Parameters = new List<IXMLRpcParameter>{
                    new XMLRpcParameter<string>
                    {
                        Name="db",
                        FilterOption = FilterOption.None,
                        Value="enerca"
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="username",
                        FilterOption = FilterOption.None,
                        Value="admin"
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="password",
                        FilterOption = FilterOption.None,
                        Value="admin"
                    }
                }
            };
            string expectedResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall>" +
                            "<methodName>login</methodName>" +
                            "<params>" +
                                "<param>" +
                                    "<value>" +
                                        "<string>enerca</string>" +
                                    "</value>" +
                                "</param>" +
                                "<param>" +
                                    "<value>" +
                                        "<string>admin</string>" +
                                    "</value>" +
                                "</param>" +
                                "<param>" +
                                    "<value>" +
                                        "<string>admin</string>" +
                                    "</value>" +
                                "</param>" +
                            "</params>" +
                        "</methodCall>";

            //Act
            string result = XMLRpcSerializer.SerializeObject(rpcRequest);
            string formatedResult = Regex.Replace(expectedResult, @"\t|\n|\r", string.Empty);
            //Assert
            Assert.Equal(formatedResult, result);
        }

        [Fact]
        public void Can_Serialize_ParamsArray()
        {
            //Arrange
            XMLRpcRequest rpcRequest = new XMLRpcRequest
            {
                MethodName = "execute",
                Encoding = Encoding.UTF8,
                Parameters = new List<IXMLRpcParameter>{
                    new XMLRpcParameter<string>
                    {
                        Name="db",
                        FilterOption = FilterOption.None,
                        Value="enerca"
                    },
                    new XMLRpcParameter<int>
                    {
                        Name="userId",
                        FilterOption = FilterOption.None,
                        Value=1
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="password",
                        FilterOption = FilterOption.None,
                        Value="admin"
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="modelName",
                        FilterOption = FilterOption.None,
                        Value="siec.factura"
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="method",
                        FilterOption = FilterOption.None,
                        Value="search"
                    },
                    new XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>>{
                        Name="Params",
                        FilterOption=FilterOption.None,
                        Value=new XMLRpcParamList<IXMLRpcParameter>
                        {
                            new XMLRpcParameter<string>
                            {
                                Name="cuenta",
                                FilterOption = FilterOption.Equals,
                                Value="112093"
                            },
                            new XMLRpcParameter<string>
                            {
                                Name="ano",
                                FilterOption = FilterOption.Equals,
                                Value="2010"
                            }
                        }
                    }
                }
            };
            string expectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?><methodCall><methodName>execute</methodName><params><param><value><string>enerca</string></value></param>
<param>
<value>
<int>1</int>
</value>
</param>
<param>
<value>
<string>admin</string>
</value>
</param>
<param>
<value>
<string>siec.factura</string>
</value>
</param>
<param>
<value>
<string>search</string>
</value>
</param>
<param>
<value>
<array>
<data>
<value>
<array>
<data>
<value>
<string>cuenta</string>
</value>
<value>
<string>=</string>
</value>
<value>
<string>112093</string>
</value>
</data>
</array>
</value>
<value>
<array>
<data>
<value>
<string>ano</string>
</value>
<value>
<string>=</string>
</value>
<value>
<string>2010</string>
</value>
</data>
</array>
</value>
</data>
</array>
</value>
</param>
</params>
</methodCall>";

            //Act
            string result = XMLRpcSerializer.SerializeObject(rpcRequest);
            string formatedResult = Regex.Replace(expectedResult, @"\t|\n|\r", string.Empty);
            //Assert
            Assert.Equal(formatedResult, result);
        }

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
            //Act
            var data = XMLRpcSerializer.DeserializeStruct<DataXML>(xmlData);
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
            //Act
            var data = XMLRpcSerializer.DeserializeStruct<DataXML2>(xmlData);
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
