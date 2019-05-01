using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Commons
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string theString)
        {
            if (theString == null || theString.Length < 2)
                return theString;

            string[] words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            string result = "";
            for (int i = 0; i < words.Length; i++)
            {
                result += words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
            }

            return result;
        }
    }
}
