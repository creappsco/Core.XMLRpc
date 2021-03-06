﻿using Core.XMLRpc.Commons;
using System;

namespace Core.XMLRpc.Request
{   
    public class XMLRpcParameter<T> : XMLRpcParameterBase
    {
        public T Value { get; set; }
        public override bool IsArray => Value is XMLRpcParamList<IXMLRpcParameter>;
        public override FilterOption FilterOption { get; set; }
        public override string Name { get; set; }
        public override string GetValue()
        {
            return this.Value.ToString();
        }

        public override object GetObjectValue()
        {
            return Value;
        }

        public override string GetValueType()
        {
            if (Value is int)
            {
                return "int";
            }
            else
            {
                if (Value is bool)
                {
                    return "boolean";
                }
                else
                {
                    if (Value is double || Value is float)
                    {
                        return "double";
                    }
                    else
                    {
                        if (Value is DateTime)
                        {
                            return "dateTime.iso8601";
                        }
                    }
                }
            }
            return "string";
        }
    }
}
