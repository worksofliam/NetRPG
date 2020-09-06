using System;
using System.Text;
using System.Collections.Generic;

namespace NetRPG.Language
{
    class EBCDIC
    {
        public static string ConvertHex(string from, string hex) {
            hex = hex.ToUpper();

            byte[] raw = new byte[hex.Length / 2];
            for (int x = 0; x < raw.Length; x++)
                raw[x] = Convert.ToByte(hex.Substring(x * 2, 2), 16);

            return Encoding.UTF32.GetString(Encoding.Convert(Encoding.GetEncoding(from), Encoding.UTF32, raw));
        }

        private static readonly Dictionary<int, string> EncodingMap = new Dictionary<int, string>() {
            {37, "IBM037"},
            {273, "IBM273"},
            {277, "IBM277"},
            {278, "IBM278"},
            {280, "IBM280"},
            {284, "IBM284"},
            {285, "IBM285"},
            {297, "IBM297"}
        };

        public static string GetEncoding(int ccsid) {
            if (EncodingMap.ContainsKey(ccsid))
              return EncodingMap[ccsid];
            else
              Error.ThrowCompileError("CCSID " + ccsid.ToString() + " not mapped to .NET Core encoding.");

            return "";
        }
    }
}