using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Commons
{
    /// <summary>
    /// Custom Attribute that allow set string as Attribute to enum
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Enum item value
        /// </summary>
        public string Value { get; private set; } 

        /// <summary>
        /// Create a string value to enum member
        /// </summary>
        /// <param name="value">the string value</param>
        public StringValueAttribute(string value )
        {
            Value = value;
        }
    }
}
