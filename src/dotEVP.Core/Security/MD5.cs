namespace dotEVP.Core.Security {
    internal static class MD5 {
        internal static byte[] ComputeHash(byte[] input) {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                return md5.ComputeHash(input);
            }
        }
    }
}