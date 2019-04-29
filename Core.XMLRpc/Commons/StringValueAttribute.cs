using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Commons
{
    public class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }
}
