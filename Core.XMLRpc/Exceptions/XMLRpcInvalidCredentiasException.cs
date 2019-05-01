using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Exceptions
{
    public class XMLRpcInvalidCredentiasException : Exception
    {
        public XMLRpcInvalidCredentiasException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
