using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    public class XMLRpcRequest
    {
        public string Url { get; set; }
        public string MethodName { get; set; }
        public Encoding Encoding { get; set; }
        public IList<IXMLRpcParameter> Parameters { get; set; }
    }
}
