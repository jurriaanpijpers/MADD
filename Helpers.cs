using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADD
{
  public static  class Helpers
    {
        public static string RemoveInvalidChars(string originalString)
        {
            string finalString = string.Empty;
            if (!string.IsNullOrEmpty(originalString))
            {
                finalString = string.Concat(originalString.Split(Path.GetInvalidFileNameChars()));
            }
            finalString = ReturnCleanASCII(finalString);
            return finalString;
        }
        public static string ReturnCleanASCII(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if ((int)c > 127) // you probably don't want 127 either
                    continue;
                if ((int)c < 33)  // I bet you don't want control characters 
                    continue;
                if (c == '%')
                    continue;
                if (c == '?')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
