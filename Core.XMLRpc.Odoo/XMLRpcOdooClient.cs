using Core.XMLRpc.Commons;
using Core.XMLRpc.Odoo.Exceptions;
using Core.XMLRpc.Request;
using Core.XMLRpc.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.XMLRpc.Odoo
{
    public class XMLRpcOdooClient : XMLRpcClient
    {
        private const string LoginService = "xmlrpc/2/common";
        private const string ExecuteService = "xmlrpc/2/object";

        protected readonly string UserName;
        protected readonly string Password;
        protected readonly string Database;

        private readonly XMLRpcRequest XMLRpcLoginRequest;

        public XMLRpcOdooClient(string url, string userName, string password, string database) : base(url)
        {
            this.UserName = userName;
            this.Password = password;
            this.Database = database;

            this.XMLRpcLoginRequest = new XMLRpcRequest
            {
                Url = $"{UrlBase}/xmlrpc/2/common",
                MethodName = "login",
                Encoding = Encoding.UTF8,
                Parameters = new List<IXMLRpcParameter>{
                    new XMLRpcParameter<string>
                    {
                        Name="db",
                        FilterOption = FilterOption.None,
                        Value=Database
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="username",
                        FilterOption = FilterOption.None,
                        Value=UserName
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="password",
                        FilterOption = FilterOption.None,
                        Value=Password
                    }
                }
            };
        }

        public async Task<int> Login()
        {
            try
            {
                var data = await this.Send<int>(LoginService, "login", this.XMLRpcLoginRequest.Parameters);
                return data;
            }
            catch (InvalidCastException ex)
            {
                if (ex.Message.Contains("System.Boolean"))
                {
                    throw new XMLRpcInvalidCredentiasException("the username and password combination does not match the stored data", ex);
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task<List<int>> Search(string modelName, XMLRpcParamList<IXMLRpcParameter> filters)
        {
            List<IXMLRpcParameter> parameters = await PrepareCall(modelName, "search", filters);

            var data = await this.Send<List<int>>(ExecuteService, "execute", parameters);

            return data;

        }

        public async Task<T> SearchAndRead<T>(string modelName, XMLRpcParamList<IXMLRpcParameter> filters)
        {
            List<IXMLRpcParameter> parameters = await PrepareCall(modelName, "search_read", filters);

            var data = await this.Send<T>(ExecuteService, "execute", parameters);

            return data;

        }
        
        private async Task<List<IXMLRpcParameter>> PrepareCall(string modelName, string methodName, XMLRpcParamList<IXMLRpcParameter> filters)
        {
            var requestedId = await Login();

            var parameters = new List<IXMLRpcParameter>
            {
                new XMLRpcParameter<string>
                {
                    Name="db",
                    FilterOption = FilterOption.None,
                    Value=Database
                },
                new XMLRpcParameter<int>
                {
                    Name="userId",
                    FilterOption = FilterOption.None,
                    Value=requestedId
                },
                new XMLRpcParameter<string>
                {
                    Name="password",
                    FilterOption = FilterOption.None,
                    Value=Password
                },
                new XMLRpcParameter<string>
                {
                    Name="modelName",
                    FilterOption = FilterOption.None,
                    Value=modelName
                },
                new XMLRpcParameter<string>
                {
                    Name="methodName",
                    FilterOption = FilterOption.None,
                    Value= methodName
                }
            };
            if (filters != null)
            {
                var filter = new XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>>
                {
                    Value = new XMLRpcParamList<IXMLRpcParameter>()
                };
                foreach (var item in filters)
                {
                    filter.Value.Add(item);
                }
                parameters.Add(filter);
            }

            return parameters;
        }
    }
}