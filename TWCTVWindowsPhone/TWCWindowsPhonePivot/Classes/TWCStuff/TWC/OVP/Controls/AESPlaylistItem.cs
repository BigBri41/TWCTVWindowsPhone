namespace TWC.OVP.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using TWC.OVP.Decryption;

    public class AESPlaylistItem : VideoPlayerPlaylistItem
    {
        public AESDecryptionInfo DecryptionInfo { get; set; }

        public bool IsEncrypted { get; set; }
    }
}

