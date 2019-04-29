using Core.XMLRpc.App;
using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Core.XMLRpc.Test
{
    public class XMLRpcCallerTest
    {
        [Fact]
        public async void Can_Call_Login()
        {
            //Arrange            
            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin", "enerca");
            //Act
            var returned = await caller.Login();
            //Assert
            Assert.IsType<XMLRpcResponse>(returned);
            Assert.True(returned.Response.Length == 1);
            Assert.Equal("1", returned.Response[0].Value);
            Assert.Equal("int", StringEnum.GetStringValue(returned.Response[0].DataType));
        }

        [Fact]
        public async void Login_Failed()
        {
            //Arrange            
            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin2", "enerca");
            //Act
            var returned = await caller.Login();
            //Assert
            Assert.IsType<XMLRpcResponse>(returned);
            Assert.True(returned.Response.Length == 1);
            Assert.Equal("0", returned.Response[0].Value);
            Assert.Equal("boolean", StringEnum.GetStringValue(returned.Response[0].DataType));
        }

        [Fact]
        public async void Can_Call_Search()
        {
            //Arrange            
            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin", "enerca");
            var parameters = new XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>>
            {
                Name = "Params",
                FilterOption = FilterOption.None,
                Value = new XMLRpcParamList<IXMLRpcParameter>
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
            };
            //Act
            var returned = await caller.Send("search", "siec.factura", parameters);
            Assert.IsType<XMLRpcResponse>(returned);
            var returnedDetauls = Assert.IsType<List<XMLRpcResponseDetails>>(returned.Response[0].Value);
            //Assert
            Assert.True(returnedDetauls.Count == 12);
        }
    }
}
