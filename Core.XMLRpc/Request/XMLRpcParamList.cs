using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    /// <summary>
    /// Define a class that allow set a List of IXMLRpcParameter objects
    /// </summary>
    /// <typeparam name="T">Class that implements IXMLRpcParameter</typeparam>
    public class XMLRpcParamList<T> : List<T>, IEnumerable<T> where T : IXMLRpcParameter
    {
        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
