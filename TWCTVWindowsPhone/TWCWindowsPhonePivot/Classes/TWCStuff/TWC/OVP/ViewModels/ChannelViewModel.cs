namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using TWC.OVP;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Models;
    using TWC.OVP.Services;

    public class ChannelViewModel : PropertyChangedBase
    {
        private Uri _firstLogo;
        private bool _highlightLogo;
        private AssetViewModel _onNext;
        private DateTime? _onNextAirDate;
        private string _onNextEpisodeTitle;
        private string _onNextShowTitle;
        private AssetViewModel _onNow;
        private DateTime? _onNowAirDate;
        private string _onNowEpisodeTitle;
        private string _onNowShowTitle;
        private string _searchText;
        private bool _showWhatsOnNext;

        public ChannelViewModel(Channel model, int idx)
        {
            this.TmsId = model.tmsId;
            this.CallSign = model.callSign;
            this.NetworkName = model.networkName;
            this.Idx = idx;
            string uriString = model.logoUrl.Replace("https", "http").Replace("style=mono", "height=60&width=60&sourceType=colorHybrid");
            this.FirstLogo = new Uri(uriString);
            this.IsAvailableOutOfHome = model.availableOutOfHome;
            Stream stream = null;
            if (((App) Application.Current).IsAdEnabled)
            {
                stream = (from c in model.streams
                    where c.type == "smooth-dai"
                    select c).FirstOrDefault<Stream>();
                if (stream != null)
                {
                    this.SmoothStreamUrl = LiveTVService.OVPDomain + stream.uri;
                }
                else
                {
                    stream = (from c in model.streams
                        where c.type == "smooth"
                        select c).FirstOrDefault<Stream>();
                    if (stream != null)
                    {
                        this.SmoothStreamUrl = LiveTVService.OVPDomain + stream.uri;
                    }
                    else
                    {
                        this.SmoothStreamUrl = null;
                    }
                }
            }
            else
            {
                stream = (from c in model.streams
                    where c.type == "smooth"
                    select c).FirstOrDefault<Stream>();
                if (stream != null)
                {
                    this.SmoothStreamUrl = LiveTVService.OVPDomain + stream.uri;
                }
                else
                {
                    this.SmoothStreamUrl = null;
                }
            }
        }

        public void Apply(WhatsOnItems whatsOnItems)
        {
            WhatsOn whatsOn = whatsOnItems.Items.FirstOrDefault<WhatsOn>();
            if (whatsOn != null)
            {
                WhatsOn on2 = whatsOnItems.Items.Skip<WhatsOn>(1).FirstOrDefault<WhatsOn>();
                if ((this.OnNow != null) && (this.OnNow.EpisodeID == whatsOn.EpisodeId))
                {
                    this.ApplyEpisodeInfo(this.OnNow, whatsOn);
                }
                else
                {
                    AssetViewModel assetVM = new AssetViewModel {
                        IsLive = true,
                        IsEncrypted = true
                    };
                    this.ApplyEpisodeInfo(assetVM, whatsOn);
                    this.OnNow = assetVM;
                }
                this.OnNext = null;
                if (on2 != null)
                {
                    if ((this.OnNext != null) && (this.OnNext.EpisodeID == on2.EpisodeId))
                    {
                        this.ApplyEpisodeInfo(this.OnNext, on2);
                    }
                    else
                    {
                        AssetViewModel model3 = new AssetViewModel {
                            IsLive = true,
                            IsEncrypted = true
                        };
                        this.ApplyEpisodeInfo(model3, on2);
                        this.OnNext = model3;
                    }
                }
            }
        }

        private void ApplyEpisodeInfo(AssetViewModel assetVM, WhatsOn whatsOn)
        {
            assetVM.EpisodeID = whatsOn.EpisodeId;
            assetVM.ServiceID = int.Parse(whatsOn.TmsId);
            assetVM.Title = whatsOn.Title.Replace("&amp;", "&");
            assetVM.EpisodeName = whatsOn.EpisodeTitle.Replace("&amp;", "&");
            if (!string.IsNullOrWhiteSpace(assetVM.EpisodeName))
            {
                assetVM.IsEpisodic = true;
                assetVM.SeriesTitle = assetVM.Title;
            }
            assetVM.AirDate = whatsOn.AirDateTime.Parse("yyyyMMddHHmm");
            assetVM.NetworkID = whatsOn.CallSign;
            if (string.IsNullOrWhiteSpace(assetVM.Description))
            {
                assetVM.Description = whatsOn.ShortDesc;
            }
        }

        public string CallSign { get; private set; }

        public Uri FirstLogo
        {
            get
            {
                return this._firstLogo;
            }
            set
            {
                this._firstLogo = value;
                this.NotifyOfPropertyChange<Uri>(System.Linq.Expressions.Expression.Lambda<System.Func<Uri>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_FirstLogo)), new ParameterExpression[0]));
            }
        }

        public bool HighlightLogo
        {
            get
            {
                return this._highlightLogo;
            }
            set
            {
                if (this._highlightLogo != value)
                {
                    this._highlightLogo = value;
                    this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_HighlightLogo)), new ParameterExpression[0]));
                }
            }
        }

        public int Idx { get; private set; }

        public bool IsAvailableOutOfHome { get; set; }

        public string NetworkName { get; private set; }

        public AssetViewModel OnNext
        {
            get
            {
                return this._onNext;
            }
            set
            {
                if (this._onNext != value)
                {
                    this._onNext = value;
                    this.NotifyOfPropertyChange<AssetViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<AssetViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNext)), new ParameterExpression[0]));
                }
            }
        }

        public DateTime? OnNextAirDate
        {
            get
            {
                return this._onNextAirDate;
            }
            set
            {
                if (this._onNextAirDate != value)
                {
                    this._onNextAirDate = value;
                    this.NotifyOfPropertyChange<DateTime?>(System.Linq.Expressions.Expression.Lambda<System.Func<DateTime?>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNextAirDate)), new ParameterExpression[0]));
                }
            }
        }

        public string OnNextEpisodeTitle
        {
            get
            {
                return this._onNextEpisodeTitle;
            }
            set
            {
                if (this._onNextEpisodeTitle != value)
                {
                    this._onNextEpisodeTitle = value;
                    this.NotifyOfPropertyChange<string>(System.Linq.Expressions.Expression.Lambda<System.Func<string>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNextEpisodeTitle)), new ParameterExpression[0]));
                }
            }
        }

        public string OnNextShowTitle
        {
            get
            {
                return this._onNextShowTitle;
            }
            set
            {
                if (this._onNextShowTitle != value)
                {
                    this._onNextShowTitle = value;
                    this.NotifyOfPropertyChange<string>(System.Linq.Expressions.Expression.Lambda<System.Func<string>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNextShowTitle)), new ParameterExpression[0]));
                }
            }
        }

        public AssetViewModel OnNow
        {
            get
            {
                return this._onNow;
            }
            set
            {
                if (this._onNow != value)
                {
                    this._onNow = value;
                    this.NotifyOfPropertyChange<AssetViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<AssetViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNow)), new ParameterExpression[0]));
                }
            }
        }

        public DateTime? OnNowAirDate
        {
            get
            {
                return this._onNowAirDate;
            }
            set
            {
                if (this._onNowAirDate != value)
                {
                    this._onNowAirDate = value;
                    this.NotifyOfPropertyChange<DateTime?>(System.Linq.Expressions.Expression.Lambda<System.Func<DateTime?>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNowAirDate)), new ParameterExpression[0]));
                }
            }
        }

        public string OnNowEpisodeTitle
        {
            get
            {
                return this._onNowEpisodeTitle;
            }
            set
            {
                if (this._onNowEpisodeTitle != value)
                {
                    this._onNowEpisodeTitle = value;
                    this.NotifyOfPropertyChange<string>(System.Linq.Expressions.Expression.Lambda<System.Func<string>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNowEpisodeTitle)), new ParameterExpression[0]));
                }
            }
        }

        public string OnNowShowTitle
        {
            get
            {
                return this._onNowShowTitle;
            }
            set
            {
                if (this._onNowShowTitle != value)
                {
                    this._onNowShowTitle = value;
                    this.NotifyOfPropertyChange<string>(System.Linq.Expressions.Expression.Lambda<System.Func<string>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_OnNowShowTitle)), new ParameterExpression[0]));
                }
            }
        }

        public string SearchText
        {
            get
            {
                return this._searchText;
            }
            set
            {
                if (this._searchText != value)
                {
                    this._searchText = value;
                    this.NotifyOfPropertyChange<string>(System.Linq.Expressions.Expression.Lambda<System.Func<string>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_SearchText)), new ParameterExpression[0]));
                }
            }
        }

        public bool ShowWhatsOnNext
        {
            get
            {
                return this._showWhatsOnNext;
            }
            set
            {
                this._showWhatsOnNext = value;
                if (this.OnNext != null)
                {
                    this.OnNext.RefreshTimeUntilAirDateText();
                }
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(ChannelViewModel)), (MethodInfo) methodof(ChannelViewModel.get_ShowWhatsOnNext)), new ParameterExpression[0]));
            }
        }

        public string SmoothStreamUrl { get; private set; }

        public string TmsId { get; private set; }
    }
}

