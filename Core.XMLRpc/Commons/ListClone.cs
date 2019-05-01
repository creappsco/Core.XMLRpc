using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.XMLRpc.Commons
{
    public class ListClone
    {
        public static List<T> CloneListAs<T>(IList<object> source)
        {
            // Here we can do anything we want with T
            // T == source[0].GetType()
            return source.Cast<T>().ToList();
        }
    }
}
