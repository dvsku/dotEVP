using System;
using System.Collections.Generic;
using System.Text;

namespace dotEVP.Core.Converters {
    internal static class HexStringToStringConverter {
        internal static string ConvertToString(string hexString) {
            hexString = hexString.Replace("-", "");
            byte[] raw = new byte[hexString.Length / 2];
            for (int i = 0; i < raw.Length; i++) {
                raw[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Configuration.Encoding.GetString(raw);
        }
    }
}
