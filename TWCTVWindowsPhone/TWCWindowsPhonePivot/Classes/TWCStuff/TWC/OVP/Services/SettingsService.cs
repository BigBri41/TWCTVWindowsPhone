namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using TWC.OVP;
    using TWC.OVP.Framework.Models;
    using TWC.OVP.Framework.Utilities;
    using TWC.OVP.Messages;

    public class SettingsService : ISettingsService
    {
        private TWC.OVP.Framework.Models.CaptionsOverrideSettings _captionsOverrideSettings;
        private IEventAggregator _eventAggregator;
        private bool _isClosedCaptioningEnabled;
        private bool _isMuted;
        private string _prevCaptionStreamLanguage;
        private string _prevCaptionStreamName;
        private List<string> _recentChannels;
        private string _selectedChannelFilter;
        private double _volumeLevel;
        private const string HistoryfolderName = "hf";
        private const string RecentChannelData = "rcd";

        public SettingsService(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            this.LoadSavedSettings();
        }

        private void LoadSavedSettings()
        {
            TWC.OVP.Framework.Models.CaptionsOverrideSettings settings = new TWC.OVP.Framework.Models.CaptionsOverrideSettings {
                CharacterColor = (CaptionsOverrideColors) IsoStorageManager.GetUserSetting("CaptionsOverrideCharacterColor", CaptionsOverrideColors.Default),
                CharacterSize = (CaptionsOverrideSizes) IsoStorageManager.GetUserSetting("CaptionsOverrideCharacterSize", CaptionsOverrideSizes.pct100),
                CharacterOpacity = (CaptionsOverrideOpacities) IsoStorageManager.GetUserSetting("CaptionsOverrideCharacterOpacity", CaptionsOverrideOpacities.Default),
                CharacterFont = (CaptionsOverrideFonts) IsoStorageManager.GetUserSetting("CaptionsOverrideCharacterFont", CaptionsOverrideFonts.Default),
                CharacterEdgeAttribute = (CaptionsOverrideCharacterEdges) IsoStorageManager.GetUserSetting("CaptionsOverrideEdgeAttribute", CaptionsOverrideCharacterEdges.Default),
                CharacterBackgroundColor = (CaptionsOverrideColors) IsoStorageManager.GetUserSetting("CaptionsOverrideBackgroundColor", CaptionsOverrideColors.Default),
                CharacterBackgroundOpacity = (CaptionsOverrideOpacities) IsoStorageManager.GetUserSetting("CaptionsOverrideBackgroundOpacity", CaptionsOverrideOpacities.Default),
                WindowColor = (CaptionsOverrideColors) IsoStorageManager.GetUserSetting("CaptionsOverrideWindowColor", CaptionsOverrideColors.Default),
                WindowOpacity = (CaptionsOverrideOpacities) IsoStorageManager.GetUserSetting("CaptionsOverrideWindowOpacity", CaptionsOverrideOpacities.Default)
            };
            this._captionsOverrideSettings = settings;
            this._isClosedCaptioningEnabled = IsoStorageManager.GetUserSetting("IsClosedCaptioningEnabled") ?? false;
            this._prevCaptionStreamName = (string) IsoStorageManager.GetUserSetting("PrevCaptionStreamName");
            this._prevCaptionStreamLanguage = (string) IsoStorageManager.GetUserSetting("PrevCaptionStreamLanguage");
            this._volumeLevel = IsoStorageManager.GetUserSetting("VolumeLevel") ?? 0.5;
            this._isMuted = IsoStorageManager.GetUserSetting("IsMuted") ?? false;
            this._recentChannels = IsoStorageManager.LoadList<string>("hf", "rcd");
            this._selectedChannelFilter = (string) IsoStorageManager.GetUserSetting("SelectedChannelFilter");
        }

        private void SaveSettings()
        {
            IsoStorageManager.SetUserSetting("CaptionsOverrideCharacterColor", (int) this.CaptionsOverrideSettings.CharacterColor);
            IsoStorageManager.SetUserSetting("CaptionsOverrideCharacterSize", (int) this.CaptionsOverrideSettings.CharacterSize);
            IsoStorageManager.SetUserSetting("CaptionsOverrideCharacterOpacity", (int) this.CaptionsOverrideSettings.CharacterOpacity);
            IsoStorageManager.SetUserSetting("CaptionsOverrideCharacterFont", (int) this.CaptionsOverrideSettings.CharacterFont);
            IsoStorageManager.SetUserSetting("CaptionsOverrideEdgeAttribute", (int) this.CaptionsOverrideSettings.CharacterEdgeAttribute);
            IsoStorageManager.SetUserSetting("CaptionsOverrideBackgroundColor", (int) this.CaptionsOverrideSettings.CharacterBackgroundColor);
            IsoStorageManager.SetUserSetting("CaptionsOverrideBackgroundOpacity", (int) this.CaptionsOverrideSettings.CharacterBackgroundOpacity);
            IsoStorageManager.SetUserSetting("CaptionsOverrideWindowColor", (int) this.CaptionsOverrideSettings.WindowColor);
            IsoStorageManager.SetUserSetting("CaptionsOverrideWindowOpacity", (int) this.CaptionsOverrideSettings.WindowOpacity);
            IsoStorageManager.SetUserSetting("IsClosedCaptioningEnabled", this.IsClosedCaptioningEnabled);
            IsoStorageManager.SetUserSetting("PrevCaptionStreamName", this.PrevCaptionStreamName);
            IsoStorageManager.SetUserSetting("PrevCaptionStreamLanguage", this.PrevCaptionStreamLanguage);
            IsoStorageManager.SaveList<string>("hf", "rcd", this.RecentChannels);
            IsoStorageManager.SetUserSetting("VolumeLevel", this.VolumeLevel);
            IsoStorageManager.SetUserSetting("IsMuted", this.IsMuted);
            IsoStorageManager.SetUserSetting("SelectedChannelFilter", this.SelectedChannelFilter);
            this._eventAggregator.Publish(new SettingsChangedMessage());
        }

        public TWC.OVP.AppMode AppMode { get; set; }

        public string CachedGenreData
        {
            get
            {
                return (string) IsoStorageManager.GetUserSetting("CachedGenreData");
            }
            set
            {
                IsoStorageManager.SetUserSetting("CachedGenreData", value);
            }
        }

        public string CachedGenreServiceData
        {
            get
            {
                return (string) IsoStorageManager.GetUserSetting("CachedGenreServiceData");
            }
            set
            {
                IsoStorageManager.SetUserSetting("CachedGenreServiceData", value);
            }
        }

        public TWC.OVP.Framework.Models.CaptionsOverrideSettings CaptionsOverrideSettings
        {
            get
            {
                return this._captionsOverrideSettings;
            }
            set
            {
                this._captionsOverrideSettings = value;
                this.SaveSettings();
            }
        }

        public bool IsClosedCaptioningEnabled
        {
            get
            {
                return this._isClosedCaptioningEnabled;
            }
            set
            {
                if (value != this._isClosedCaptioningEnabled)
                {
                    this._isClosedCaptioningEnabled = value;
                    this.SaveSettings();
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                return this._isMuted;
            }
            set
            {
                if (this._isMuted != value)
                {
                    this._isMuted = value;
                    this.SaveSettings();
                }
            }
        }

        public string PrevCaptionStreamLanguage
        {
            get
            {
                return this._prevCaptionStreamLanguage;
            }
            set
            {
                this._prevCaptionStreamLanguage = value;
                this.SaveSettings();
            }
        }

        public string PrevCaptionStreamName
        {
            get
            {
                return this._prevCaptionStreamName;
            }
            set
            {
                this._prevCaptionStreamName = value;
                this.SaveSettings();
            }
        }

        public List<string> RecentChannels
        {
            get
            {
                return this._recentChannels;
            }
            set
            {
                if (this._recentChannels != value)
                {
                    this._recentChannels = value;
                    this.SaveSettings();
                }
            }
        }

        public string SelectedChannelFilter
        {
            get
            {
                return this._selectedChannelFilter;
            }
            set
            {
                if (this._selectedChannelFilter != value)
                {
                    this._selectedChannelFilter = value;
                    this.SaveSettings();
                }
            }
        }

        public double VolumeLevel
        {
            get
            {
                return this._volumeLevel;
            }
            set
            {
                if (this._volumeLevel != value)
                {
                    this._volumeLevel = value;
                    this.SaveSettings();
                }
            }
        }
    }
}

