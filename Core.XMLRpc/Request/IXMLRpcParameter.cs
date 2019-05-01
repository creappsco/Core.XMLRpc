using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    public interface IXMLRpcParameter
    {
        string Name { get; set; }
        FilterOption FilterOption { get; set; }
        bool IsArray { get; }
        object GetObjectValue();
        string GetValueType();
        string GetValue();
    }
}
