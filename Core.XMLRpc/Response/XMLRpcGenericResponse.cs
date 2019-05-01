using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Response
{
    public class XMLRpcGenericResponse<T>
    {
        public T Response { get; set; }
    }
}
