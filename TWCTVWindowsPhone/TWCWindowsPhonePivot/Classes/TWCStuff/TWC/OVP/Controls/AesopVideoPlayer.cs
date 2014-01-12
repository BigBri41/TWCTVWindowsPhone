namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core;
    using System;
    using TWC.OVP.Decryption;

    public class AesopVideoPlayer : VideoPlayer
    {
        private AESOVPRequestDecryptingCache _cache;

        public AesopVideoPlayer()
        {
            base.DefaultStyleKey = typeof(SMFPlayer);
        }

        protected override object SelectAdaptiveCacheProvider()
        {
            if ((base.CurrentPlaylistItem != null) && ((AESPlaylistItem) base.CurrentPlaylistItem).IsEncrypted)
            {
                this._cache = new AESOVPRequestDecryptingCache(((AESPlaylistItem) base.CurrentPlaylistItem).DecryptionInfo);
            }
            else
            {
                this._cache = null;
            }
            return this._cache;
        }

        public override void Stop()
        {
            if (this._cache != null)
            {
                this._cache.IsStopping = true;
            }
            base.Stop();
            if (this._cache != null)
            {
                this._cache.IsStopped = true;
            }
        }
    }
}

