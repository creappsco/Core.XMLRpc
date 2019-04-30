using Core.XMLRpc.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc
{
    public enum XMLRpcType
    {
        [StringValue("int")]
        Int,
        [StringValue("boolean")]
        Boolean,
        [StringValue("double")]
        Double,
        [StringValue("dateTime.iso8601")]
        Datetime,
        [StringValue("string")]
        String,
        [StringValue("struct")]
        Struct,
        [StringValue("array")]
        Array,

    }
}
