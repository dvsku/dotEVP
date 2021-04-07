using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dotEVP.Core.Helpers {
    internal static class PackerHelpers {
        internal static byte[] GenerateFileDetails(EVP evp) {
            byte[] output;
            byte[] paddingBytes = new byte[sizeof(long)];
            Array.Clear(paddingBytes, 0, paddingBytes.Length);

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter binaryWriter = new BinaryWriter(stream)) {
                    binaryWriter.Write(evp.path.Length);
                    binaryWriter.Write(Configuration.Encoding.GetBytes(evp.path));
                    binaryWriter.Write(evp.dataStartOffset);
                    binaryWriter.Write(evp.dataLen);
                    binaryWriter.Write(evp.dataLen);
                    binaryWriter.Write(1);
                    binaryWriter.Write(paddingBytes);
                    binaryWriter.Write(evp.dataHash);
                }
                output = stream.ToArray();
            }
            return output;
        }
    }
}
