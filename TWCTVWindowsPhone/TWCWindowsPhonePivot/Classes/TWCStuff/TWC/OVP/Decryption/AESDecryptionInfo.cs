namespace TWC.OVP.Decryption
{
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;

    public class AESDecryptionInfo
    {
        private static AesManaged _aes = new AesManaged();

        public AESDecryptionInfo(string iv, byte[] key)
        {
            this.Decryptor = _aes.CreateDecryptor(key, this.HexStringToBytes(iv));
        }

        private byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }
            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException(hexString, "hexString must contain an even number of characters.");
            }
            if (hexString.Length > 0x20)
            {
                hexString = hexString.Substring(2, 0x20);
            }
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }

        public ICryptoTransform Decryptor { get; set; }

        public SmoothStreamingMediaElement MediaElement { get; set; }

        public string SessionID { get; set; }
    }
}

