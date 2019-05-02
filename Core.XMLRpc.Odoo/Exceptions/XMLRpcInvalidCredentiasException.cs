using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Odoo.Exceptions
{
    /// <summary>
    ///  Represents errors that occur during Login Method.
    /// </summary>
    public class XMLRpcInvalidCredentiasException : Exception
    {
        public XMLRpcInvalidCredentiasException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
