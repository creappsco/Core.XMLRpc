using Core.XMLRpc.Commons;
using Core.XMLRpc.Exceptions;
using Core.XMLRpc.Test.Models;
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
            var returned = await caller.Login<int>();
            //Assert
            Assert.IsType<int>(returned);
            Assert.Equal(1, returned);
        }

        [Fact]
        public async void Login_Failed()
        {
            //Arrange            
            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin2", "enerca");
            //Act
            //Assert 
            await Assert.ThrowsAsync<XMLRpcInvalidCredentiasException>(() => caller.Login<int>());
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
            var returned = await caller.Send<List<int>>("search", "siec.factura", parameters);
            Assert.IsType<List<int>>(returned);
            //Assert
            Assert.True(returned.Count == 12);
        }

        [Fact]
        public async void Can_Call_SearchRead()
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
            var returned = await caller.Send<List<Factura>>("search_read", "siec.factura", parameters);
            Assert.IsType<List<Factura>>(returned);
            //Assert
            Assert.True(returned.Count == 12);
            Assert.Equal("504130", returned[0].ValorOriginal);
        }
    }
}
