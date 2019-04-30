using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Core.XMLRpc
{

    [XmlRoot("methodResponse")]
    public class XMLRpcResponse
    {
        [XmlArray("params")]
        [XmlArrayItem("param")]
        public XMLRpcResponseDetails[] Response;
    }
}
