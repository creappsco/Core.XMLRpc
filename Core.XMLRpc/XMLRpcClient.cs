using Core.XMLRpc.Exceptions;
using Core.XMLRpc.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.XMLRpc
{
    public class XMLRpcClient
    {
        public readonly string UrlBase;
        public readonly string UserName;
        public readonly string Password;
        public readonly string Database;

        private readonly XMLRpcRequest XMLRpcLoginRequest;

        public XMLRpcClient(string url, string userName, string password, string database)
        {
            this.UrlBase = url;
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
        public async Task<T> Login<T>()
        {
            var rpcResponse = new object();

            using (var client = new HttpClient())
            {
                string bodyContent = XMLRpcSerializer.SerializeObject(XMLRpcLoginRequest);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(XMLRpcLoginRequest.Url),
                    Content = new StringContent(bodyContent, Encoding.UTF8, "application/xml")
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    string data = await result.Content.ReadAsStringAsync();
                    using (Stream response = await result.Content.ReadAsStreamAsync())
                    {
                        try
                        {
                            rpcResponse = XMLRpcSerializer.Deserialize<T>(response);
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
                }
            }
            return (T)rpcResponse;
        }

        public async Task<T> Send<T>(string methodName, string modelName, XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>> parameters)
        {
            var loginRequestResult = await this.Login<int>();

            var rpcResponse = new object();
            XMLRpcRequest rpcRequest = new XMLRpcRequest
            {
                Url = $"{UrlBase}/xmlrpc/2/object",
                MethodName = "execute",
                Encoding = Encoding.UTF8,
                Parameters = new List<IXMLRpcParameter>{
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
                        Value=loginRequestResult
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
                        Value=methodName
                    }
                }
            };

            if ((parameters?.Value?.Count ?? 0) > 0)
            {
                var filter = new XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>>
                {
                    Value = new XMLRpcParamList<IXMLRpcParameter>()
                };
                foreach (var item in parameters.Value)
                {
                    filter.Value.Add(item);
                }
                rpcRequest.Parameters.Add(filter);
            }

            using (var client = new HttpClient())
            {
                string bodyContent = XMLRpcSerializer.SerializeObject(rpcRequest);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(rpcRequest.Url),
                    Content = new StringContent(bodyContent, Encoding.UTF8, "application/xml")
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    string data = await result.Content.ReadAsStringAsync();

                    using (Stream response = await result.Content.ReadAsStreamAsync())
                    {
                        rpcResponse = XMLRpcSerializer.Deserialize<T>(response);
                    }
                }
            }
            return (T)rpcResponse;
        }


    }
}
