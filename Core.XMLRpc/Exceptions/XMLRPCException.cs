using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Exceptions
{
    public class XMLRPCException : Exception
    {
        public int FaultCode { get; set; }
        public string FaultString { get; set; }

        public XMLRPCException()
        {

        }

        public XMLRPCException(int faultCode,string faultString) : base(faultString)
        {
            this.FaultCode = faultCode;
            this.FaultString = faultString;
        }
    }
}
