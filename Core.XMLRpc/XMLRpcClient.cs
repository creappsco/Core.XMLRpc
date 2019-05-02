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
    /// <summary>
    /// Allow to Send XML RPC Request
    /// </summary>
    public class XMLRpcClient
    {
        /// <summary>
        /// Url base (https://x.y.z)
        /// </summary>
        protected readonly string UrlBase;

        /// <summary>
        /// Create a Client with the URL base
        /// </summary>
        /// <param name="url"></param>
        public XMLRpcClient(string url)
        {
            this.UrlBase = url;
        }
        /// <summary>
        /// Send a XML RPC Request
        /// </summary>
        /// <typeparam name="T">Expected Element Type</typeparam>
        /// <param name="requestedService">URL complement</param>
        /// <param name="methodName">XML RPC Method Name</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns>Deserialized Object</returns>
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
