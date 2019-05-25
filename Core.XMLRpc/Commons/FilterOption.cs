using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Commons
{
    public enum FilterOption
    {

        [StringValue("")]
        None,
        [StringValue("=")]
        Equals,
        [StringValue(">=")]
        GreaterOrEquals,
    }
}
