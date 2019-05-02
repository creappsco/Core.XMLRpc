using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Request
{
    /// <summary>
    /// Base class to XML RPC Parameter
    /// </summary>
    public abstract class XMLRpcParameterBase : IXMLRpcParameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public abstract string Name { get; set; }
        /// <summary>
        /// specifies if a parameter is an array of parameters
        /// </summary>
        public abstract bool IsArray { get; }
        /// <summary>
        /// Obtain the value from the parameter
        /// </summary>
        /// <returns>The value</returns>
        public abstract FilterOption FilterOption { get; set; }
        /// <summary>
        /// Obtain the value in string format
        /// </summary>
        /// <returns></returns>
        public abstract string GetValue();
        /// <summary>
        /// Obtain the value from the parameter
        /// </summary>
        /// <returns>The value</returns>
        public abstract object GetObjectValue();
        /// <summary>
        /// Obtain DataType from the parameter
        /// </summary>
        /// <returns></returns>
        public abstract string GetValueType();
    }
}
