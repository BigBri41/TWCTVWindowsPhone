using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Microsoft.Web.Media.SmoothStreaming;
using System.Globalization;

namespace TWC.OVP.Decription
{
public class AESDecryptionInfo
{
    // Fields
    private static AesManaged _aes = new AesManaged();

    // Methods
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

    // Properties
    public ICryptoTransform Decryptor { get; set; }

    public SmoothStreamingMediaElement MediaElement { get; set; }

    public string SessionID { get; set; }
}

 

 

}
