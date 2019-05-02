using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    /// <summary>
    /// Class that encapsulate a XML RPC Request
    /// </summary>
    public class XMLRpcRequest
    {
        /// <summary>
        /// XML RPC Service URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Method name to call
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// Encoding Type
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// A List of IXMLRpcParameter objects
        /// </summary>
        public IList<IXMLRpcParameter> Parameters { get; set; }
    }
}
