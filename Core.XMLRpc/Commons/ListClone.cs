using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.XMLRpc.Commons
{
    /// <summary>
    /// Class that contains extensions methods
    /// </summary>
    public class ListClone
    {
        /// <summary>
        /// Clone a List from object items to T items
        /// </summary>
        /// <typeparam name="T">The type to cast the elements of source to</typeparam>
        /// <param name="source">The System.Collections.IEnumerable that contains the elements to be cast to type T</param>
        /// <returns>An List that contains each element of the source sequence cast to the specified type</returns>
        public static List<T> CloneListAs<T>(IList<object> source)
        {
            // Here we can do anything we want with T
            // T == source[0].GetType()
            return source.Cast<T>().ToList();
        }
    }
}
