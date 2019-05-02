using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    /// <summary>
    /// Define the methods that an RPC parameter should implement
    /// </summary>
    public interface IXMLRpcParameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Filter type
        /// </summary>
        FilterOption FilterOption { get; set; }
        /// <summary>
        /// specifies if a parameter is an array of parameters
        /// </summary>
        bool IsArray { get; }
        /// <summary>
        /// Obtain the value from the parameter
        /// </summary>
        /// <returns>The value</returns>
        object GetObjectValue();
        /// <summary>
        /// Obtain DataType from the parameter
        /// </summary>
        /// <returns></returns>
        string GetValueType();
        /// <summary>
        /// Obtain the value in string format
        /// </summary>
        /// <returns></returns>
        string GetValue();
    }
}
