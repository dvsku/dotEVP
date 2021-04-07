namespace dotEVP.Core {
    internal struct EVP {
        public string path;
        public byte[] data;
        public byte[] dataHash;
        public int pathLen;
        public int dataLen;
        public int dataStartOffset;
    }
}