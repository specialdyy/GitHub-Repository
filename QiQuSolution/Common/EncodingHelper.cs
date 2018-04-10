using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class EncodingHelper
    {
        public static string ConvertToHanZiString(this string unicodeNumberString)
        {
            StringBuilder hanZiString = new StringBuilder(unicodeNumberString);
            MatchCollection matches = Regex.Matches(unicodeNumberString, "\\\\u([a-fA-F0-9]{4})");
            string result = string.Empty;
            byte[] hanZiChar = new byte[2];
            foreach (Match m in matches)
            {
                string t = m.Groups[1].Value;
                hanZiChar[0] = (byte)Convert.ToInt32(t.Substring(2), 16);
                hanZiChar[1] = (byte)Convert.ToInt32(t.Substring(0, 2), 16);
                hanZiString = hanZiString.Replace(m.Value, Encoding.Unicode.GetString(hanZiChar));
            }
            return hanZiString.ToString();
        }

        public static bool IsContainUnicodeNumber(this string content)
        {
            return Regex.Matches(content, "\\\\u[a-fA-F0-9]{4}").Count > 0;
        }
    }
}
