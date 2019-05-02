using Core.XMLRpc.Odoo.Exceptions;
using Core.XMLRpc.Odoo;
using Core.XMLRpc.Odoo.Test;
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
            XMLRpcOdooClient caller = new XMLRpcOdooClient("https://corexmlrpcdemo.odoo.com", "demoodoo@demoodoo.co", "odoo2019", "corexmlrpcdemo");
            //Act
            var returned = await caller.Login();
            //Assert
            Assert.IsType<int>(returned);
        }

        [Fact]
        public async void Login_Failed()
        {
            //Arrange            
            XMLRpcOdooClient caller = new XMLRpcOdooClient("https://corexmlrpcdemo.odoo.com", "demoodoo@demoodoo.co", "odoo2018", "corexmlrpcdemo");
            //Act && Assert
            await Assert.ThrowsAsync<XMLRpcInvalidCredentiasException>(() => caller.Login());
        }

        [Fact]
        public async void Can_Call_Search()
        {
            //Arrange         
            var filters = new XMLRpcParamList<IXMLRpcParameter>();            
            XMLRpcOdooClient caller = new XMLRpcOdooClient("https://corexmlrpcdemo.odoo.com", "demoodoo@demoodoo.co", "odoo2019", "corexmlrpcdemo");
            //Act
            var returned = await caller.Search("res.partner", filters);
            Assert.IsType<List<int>>(returned);
            //Assert
            Assert.True(returned.Count > 0);
        }

        [Fact]
        public async void Can_Call_SearchRead()
        {
            //Arrange         
            var filters = new XMLRpcParamList<IXMLRpcParameter>();            
            XMLRpcOdooClient caller = new XMLRpcOdooClient("https://corexmlrpcdemo.odoo.com", "demoodoo@demoodoo.co", "odoo2019", "corexmlrpcdemo");
            //Act
            var returned = await caller.SearchAndRead<List<Contacto>>("res.partner", filters);
            Assert.IsType<List<Contacto>>(returned);
            //Assert
            Assert.True(returned.Count > 0);
        }
    }
}
