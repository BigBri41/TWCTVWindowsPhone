namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using TWC.OVP.Decryption;

    public class AssetViewModel : PropertyChangedBase
    {
        private DateTime? _airDate;
        private string _aireDateShortTimeText;
        private string _cast;
        private string _channelTitle = string.Empty;
        private string _description;
        private string _director;
        private int _duration;
        private string _episode;
        private string _episodeID;
        private string _episodeName;
        private DateTime? _expirationDate;
        private Uri _imageUri;
        private bool _isEpisodic;
        private bool _isFastForwardEnabled;
        private bool _isLive;
        private DateTime? _licenseExpirationTime;
        private string _mediaPath;
        private Uri _mediaSource;
        private string _networkID;
        private Uri _networkUri;
        private string _rating;
        private string _season;
        private string _seriesTitle;
        private int _serviceID;
        private string _shortCast;
        private int _startTime;
        private Uri _thumbSource;
        private string _timeUntilAirDateText;
        private string _title;
        private int _year;

        private string FormatDuration(int duration)
        {
            TimeSpan span = TimeSpan.FromMinutes((double) duration);
            string str = (span.Hours > 1) ? "s" : "";
            string str2 = (span.Minutes > 1) ? "s" : "";
            if (duration == 0)
            {
                return "less than a minute";
            }
            if ((duration % 60) == 0)
            {
                return string.Format("{0} hour{1}", span.Hours, str);
            }
            if (duration > 60)
            {
                return string.Format("{0} hour{1} {2} minute{3}", new object[] { span.Hours, str, span.Minutes, str2 });
            }
            return string.Format("{0} minute{1}", span.Minutes, str2);
        }

        public void RefreshTimeUntilAirDateText()
        {
            if (this.AirDate.HasValue)
            {
                this.TimeUntilAirDateText = this.FormatDuration(this.AirDate.Value.Subtract(DateTime.Now).Minutes);
            }
        }

        public DateTime? AirDate
        {
            get
            {
                return this._airDate;
            }
            set
            {
                if (this._airDate != value)
                {
                    this._airDate = value;
                    if (this._airDate.HasValue)
                    {
                        this.AirDateShortTimeText = this._airDate.Value.ToShortTimeString();
                        this.RefreshTimeUntilAirDateText();
                    }
                    this.NotifyOfPropertyChange<DateTime?>(Expression.Lambda<System.Func<DateTime?>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_AirDate)), new ParameterExpression[0]));
                }
            }
        }

        public string AirDateShortTimeText
        {
            get
            {
                return this._aireDateShortTimeText;
            }
            set
            {
                this._aireDateShortTimeText = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_AirDateShortTimeText)), new ParameterExpression[0]));
            }
        }

        public string Cast
        {
            get
            {
                return this._cast;
            }
            set
            {
                if (this._cast != value)
                {
                    this._cast = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Cast)), new ParameterExpression[0]));
                }
            }
        }

        public string ChannelTitle
        {
            get
            {
                return this._channelTitle;
            }
            set
            {
                this._channelTitle = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ChannelTitle)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_LoadingTitle)), new ParameterExpression[0]));
            }
        }

        public AESDecryptionInfo DecryptionInfo { get; set; }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Description)), new ParameterExpression[0]));
                }
            }
        }

        public string Director
        {
            get
            {
                return this._director;
            }
            set
            {
                if (this._director != value)
                {
                    this._director = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Director)), new ParameterExpression[0]));
                }
            }
        }

        public int Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                if (this._duration != value)
                {
                    this._duration = value;
                    this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Duration)), new ParameterExpression[0]));
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_FormattedDuration)), new ParameterExpression[0]));
                }
            }
        }

        public string Episode
        {
            get
            {
                return this._episode;
            }
            set
            {
                this._episode = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Episode)), new ParameterExpression[0]));
            }
        }

        public string EpisodeID
        {
            get
            {
                return this._episodeID;
            }
            set
            {
                if (this._episodeID != value)
                {
                    this._episodeID = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_EpisodeID)), new ParameterExpression[0]));
                }
            }
        }

        public string EpisodeName
        {
            get
            {
                return this._episodeName;
            }
            set
            {
                if (this._episodeName != value)
                {
                    this._episodeName = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_EpisodeName)), new ParameterExpression[0]));
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_LoadingSubtitle)), new ParameterExpression[0]));
                }
            }
        }

        public DateTime? ExpirationDate
        {
            get
            {
                return this._expirationDate;
            }
            set
            {
                if (this._expirationDate != value)
                {
                    this._expirationDate = value;
                    this.NotifyOfPropertyChange<DateTime?>(Expression.Lambda<System.Func<DateTime?>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ExpirationDate)), new ParameterExpression[0]));
                }
            }
        }

        public string FormattedDuration
        {
            get
            {
                return this.FormatDuration(this.Duration);
            }
        }

        public Uri ImageUri
        {
            get
            {
                return this._imageUri;
            }
            set
            {
                if (this._imageUri != value)
                {
                    this._imageUri = value;
                    this.NotifyOfPropertyChange<Uri>(Expression.Lambda<System.Func<Uri>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ImageUri)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsAvailableOutOfHome { get; set; }

        public bool IsEncrypted { get; set; }

        public bool IsEpisodic
        {
            get
            {
                return this._isEpisodic;
            }
            set
            {
                if (this._isEpisodic != value)
                {
                    this._isEpisodic = value;
                    this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_IsEpisodic)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsFastForwardEnabled
        {
            get
            {
                return this._isFastForwardEnabled;
            }
            set
            {
                if (!this._isFastForwardEnabled)
                {
                    this._isFastForwardEnabled = value;
                    this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_IsFastForwardEnabled)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsLive
        {
            get
            {
                return this._isLive;
            }
            set
            {
                this._isLive = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_IsLive)), new ParameterExpression[0]));
            }
        }

        public DateTime? LicenseExpirationTime
        {
            get
            {
                return this._licenseExpirationTime;
            }
            set
            {
                if (this._licenseExpirationTime != value)
                {
                    this._licenseExpirationTime = value;
                    this.NotifyOfPropertyChange<DateTime?>(Expression.Lambda<System.Func<DateTime?>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_LicenseExpirationTime)), new ParameterExpression[0]));
                }
            }
        }

        public string LoadingSubtitle
        {
            get
            {
                if (this.IsEpisodic)
                {
                    return this.EpisodeName;
                }
                return string.Empty;
            }
        }

        public string LoadingTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ChannelTitle))
                {
                    return this.ChannelTitle;
                }
                if (this.IsEpisodic && !string.IsNullOrEmpty(this.SeriesTitle))
                {
                    return this.SeriesTitle;
                }
                return this.Title;
            }
        }

        public string MediaPath
        {
            get
            {
                return this._mediaPath;
            }
            set
            {
                if (this._mediaPath != value)
                {
                    this._mediaPath = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_MediaPath)), new ParameterExpression[0]));
                }
            }
        }

        public Uri MediaSource
        {
            get
            {
                return this._mediaSource;
            }
            set
            {
                if (this._mediaSource != value)
                {
                    this._mediaSource = value;
                    this.NotifyOfPropertyChange<Uri>(Expression.Lambda<System.Func<Uri>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_MediaSource)), new ParameterExpression[0]));
                }
            }
        }

        public string NetworkID
        {
            get
            {
                return this._networkID;
            }
            set
            {
                if (this._networkID != value)
                {
                    this._networkID = value;
                    if (this._networkID != null)
                    {
                        this.NetworkUri = new Uri(string.Format("http://services.timewarnercable.com/imageserver/guide/{0}?sourceType=colorHybrid&width=160&height=240&default=false", this._networkID));
                    }
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_NetworkID)), new ParameterExpression[0]));
                }
            }
        }

        public Uri NetworkUri
        {
            get
            {
                return this._networkUri;
            }
            set
            {
                this._networkUri = value;
                this.NotifyOfPropertyChange<Uri>(Expression.Lambda<System.Func<Uri>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_NetworkUri)), new ParameterExpression[0]));
            }
        }

        public string Rating
        {
            get
            {
                return this._rating;
            }
            set
            {
                if (this._rating != value)
                {
                    this._rating = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Rating)), new ParameterExpression[0]));
                }
            }
        }

        public string RatingIcon
        {
            get
            {
                if (string.IsNullOrEmpty(this.Rating))
                {
                    this.Rating = "NR";
                }
                switch (this.Rating.ToUpperInvariant())
                {
                    case "TVY":
                    case "TV-Y":
                        return "a";

                    case "TVY7":
                    case "TV-Y7":
                        return "b";

                    case "TVG":
                    case "TV-G":
                        return "c";

                    case "TVPG":
                    case "TV-PG":
                        return "d";

                    case "TV14":
                    case "TV-14":
                        return "e";

                    case "TVMA":
                    case "TV-MA":
                        return "f";

                    case "G":
                        return "g";

                    case "PG":
                        return "h";

                    case "PG13":
                    case "PG-13":
                        return "i";

                    case "R":
                        return "j";

                    case "NC17":
                    case "NC-17":
                        return "k";

                    case "NR":
                        return "l";
                }
                return null;
            }
        }

        public string Season
        {
            get
            {
                return this._season;
            }
            set
            {
                this._season = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Season)), new ParameterExpression[0]));
            }
        }

        public string SeriesTitle
        {
            get
            {
                return this._seriesTitle;
            }
            set
            {
                if (this._title != value)
                {
                    this._seriesTitle = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_SeriesTitle)), new ParameterExpression[0]));
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_LoadingTitle)), new ParameterExpression[0]));
                }
            }
        }

        public int ServiceID
        {
            get
            {
                return this._serviceID;
            }
            set
            {
                if (this._serviceID != value)
                {
                    this._serviceID = value;
                    this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ServiceID)), new ParameterExpression[0]));
                }
            }
        }

        public string ShortCast
        {
            get
            {
                return this._shortCast;
            }
            set
            {
                if (this._shortCast != value)
                {
                    this._shortCast = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ShortCast)), new ParameterExpression[0]));
                }
            }
        }

        public int StartTime
        {
            get
            {
                return this._startTime;
            }
            set
            {
                if (this._startTime != value)
                {
                    this._startTime = value;
                    this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_StartTime)), new ParameterExpression[0]));
                }
            }
        }

        public string StreamUrl { get; set; }

        public Uri ThumbSource
        {
            get
            {
                return this._thumbSource;
            }
            set
            {
                if (this._thumbSource != value)
                {
                    this._thumbSource = value;
                    this.NotifyOfPropertyChange<Uri>(Expression.Lambda<System.Func<Uri>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_ThumbSource)), new ParameterExpression[0]));
                }
            }
        }

        public string TimeUntilAirDateText
        {
            get
            {
                return this._timeUntilAirDateText;
            }
            set
            {
                this._timeUntilAirDateText = value;
                this.NotifyOfPropertyChange(this.TimeUntilAirDateText);
            }
        }

        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Title)), new ParameterExpression[0]));
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_LoadingTitle)), new ParameterExpression[0]));
                }
            }
        }

        public int Year
        {
            get
            {
                return this._year;
            }
            set
            {
                if (this._year != value)
                {
                    this._year = value;
                    this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(AssetViewModel)), (MethodInfo) methodof(AssetViewModel.get_Year)), new ParameterExpression[0]));
                }
            }
        }
    }
}

