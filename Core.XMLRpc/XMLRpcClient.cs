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
        protected readonly string UrlBase;

        public XMLRpcClient(string url)
        {
            this.UrlBase = url;
        }

        protected async Task<T> Send<T>(string requestedService,string methodName, IList<IXMLRpcParameter> parameters)
        {
            var rpcResponse = new object();
            XMLRpcRequest rpcRequest = new XMLRpcRequest
            {
                Url = $"{UrlBase}/{requestedService}",
                MethodName = methodName,
                Encoding = Encoding.UTF8,
                Parameters = parameters
            };

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
