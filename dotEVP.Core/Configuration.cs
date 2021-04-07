using System;
using System.Text;
using dotEVP.Core.Converters;

namespace dotEVP.Core {
    internal static class Configuration {
        internal static Encoding Encoding { get; } = Encoding.ASCII;
        internal static int DataStartOffset { get; } = 76;
        internal static int HeaderEndOffset { get; } = 60;
        internal static int OffsetBetweenNames { get; } = 36;
        internal static byte[] UnknownHeaderBytes { get; } 
            = Encoding.GetBytes(HexStringToStringConverter.ConvertToString(
                "353235633137613661376366626364373534313265636430363964346237326333383900100000004E4F524D414C5F5041434B5F5459504564000000"));
        internal static byte[] Padding {
            get {
                byte[] bytes = new byte[16];
                Array.Clear(bytes, 0, bytes.Length);
                return bytes;
            }
        }
        internal static string TempFileName { get; } = "\\ptmp.evp";
    }
}