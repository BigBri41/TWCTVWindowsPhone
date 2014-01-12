namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Browser;
    using System.Windows.Threading;
    using TWC.OVP.Framework;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;
    using TWC.OVP.Models;
    using TWC.OVP.Services;
    using TWC.OVP.Utilities;
    using TWC.OVP.Views;

    public class ChannelBrowserViewModel : BaseViewModel, IHandle<SelectChannelMessage>, IHandle<OutOfHomeStatusChangedMessage>, IHandle
    {
        private IList<ChannelViewModel> _allChannels = new List<ChannelViewModel>();
        private Dictionary<string, List<ChannelViewModel>> _channelGroups;
        private ChannelViewModel _currentChannel;
        private string _currentListState = VisualStates.AllChannelsState;
        private string _currentSearchText = string.Empty;
        private IEventAggregator _eventAggregator;
        private IList<ChannelViewModel> _filteredChannels;
        private ObservableCollection<TWC.OVP.ViewModels.Filter> _filterOptions;
        private IList<GenreInfo> _genres;
        private IList<GenreServiceInfo> _genreServices;
        private RetryIntervalManager _IntervalManager;
        private bool _isOnNextEnabled;
        private bool _isOnNowShown;
        private bool _isSearchEnabled;
        private string _lastListState;
        private string _lastState;
        private LocationService _LocationService;
        private string _menuButtonText = "All Channels";
        private int _onNextSearchMatchCount;
        private string _onNowOrNextState;
        private int _onNowSearchMatchCount;
        private IList<ChannelViewModel> _outOfHomeChannels;
        private ObservableCollection<ChannelViewModel> _recentChannels = new ObservableCollection<ChannelViewModel>();
        private DispatcherTimer _RetryTimer;
        private bool _searchHasFocus;
        private ChannelViewModel _selectedChannelAll;
        private ChannelViewModel _selectedChannelOOH;
        private ChannelViewModel _selectedChannelRecent;
        private int _selectedFilterIndex;
        private ISettingsService _settingService;
        private const int RecentHistoryChannels = 10;
        private bool TurnOnNextAndSearchOn;

        public ChannelBrowserViewModel(ISettingsService settingsService, IEventAggregator eventAggregator, LocationService locationService)
        {
            ObservableCollection<TWC.OVP.ViewModels.Filter> observables = new ObservableCollection<TWC.OVP.ViewModels.Filter>();
            TWC.OVP.ViewModels.Filter item = new TWC.OVP.ViewModels.Filter {
                FilterCriteria = "All",
                FilterDisplayText = "All Channels"
            };
            observables.Add(item);
            TWC.OVP.ViewModels.Filter filter2 = new TWC.OVP.ViewModels.Filter {
                FilterCriteria = "Recent",
                FilterDisplayText = "Recent History"
            };
            observables.Add(filter2);
            this._filterOptions = observables;
            this._settingService = settingsService;
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
            this._LocationService = locationService;
            if (this.TurnOnNextAndSearchOn)
            {
                this.IsOnNextEnabled = true;
                this.IsSearchEnabled = true;
            }
            this._IntervalManager = new RetryIntervalManager();
            this._RetryTimer = new DispatcherTimer();
            this._RetryTimer.Tick += new EventHandler(this._RetryTimer_Tick);
        }

        private void _RetryTimer_Tick(object sender, EventArgs e)
        {
            this._RetryTimer.Stop();
            Task.Factory.StartNew(new System.Action(this.InitializeProc));
        }

        private void channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OnNow")
            {
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_CurrentChannel)), new ParameterExpression[0]));
            }
        }

        public void ClearSearch()
        {
            ChannelBrowserView view = (ChannelBrowserView) this.GetView(null);
            if (view != null)
            {
                view.MainButton.Focus();
                this.CurrentSearchText = string.Empty;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_CurrentSearchText)), new ParameterExpression[0]));
            }
        }

        public void FilterSelectionChanged()
        {
            if (this.SelectedFilterIndex < this.FilterOptions.Count)
            {
                TWC.OVP.ViewModels.Filter filter = this.FilterOptions[this.SelectedFilterIndex];
                this._eventAggregator.Publish(new EGFilterListFilterModeChangedEventMessage(filter.FilterDisplayText));
                this._settingService.SelectedChannelFilter = filter.FilterCriteria;
                if (filter.FilterCriteria == "Recent")
                {
                    if (this.CurrentListState != VisualStates.OutOfHome)
                    {
                        this.CurrentListState = VisualStates.RecentHistoryState;
                    }
                    else
                    {
                        this._lastListState = VisualStates.RecentHistoryState;
                    }
                }
                else
                {
                    this.RefreshFilteredChannels(filter, this.CurrentSearchText);
                    if (this.FilteredChannels.Any<ChannelViewModel>())
                    {
                        if (this.CurrentListState != VisualStates.OutOfHome)
                        {
                            this.CurrentListState = VisualStates.AllChannelsState;
                        }
                        else
                        {
                            this._lastListState = VisualStates.AllChannelsState;
                        }
                    }
                    else if (this.CurrentListState != VisualStates.OutOfHome)
                    {
                        this.CurrentListState = VisualStates.NoResultsState;
                    }
                    else
                    {
                        this._lastListState = VisualStates.NoResultsState;
                    }
                }
                this.UpdateListBoxSelection(this.CurrentChannel);
            }
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentListState;
            yield return this.OnNowOrNextState;
        }

        public void Handle(OutOfHomeStatusChangedMessage message)
        {
            bool hasAvailableChannel = true;
            System.Action dismissedEvent = () => this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(message.IsOutOfHome, message.IsOutOfCountry, this.CurrentChannel.IsAvailableOutOfHome, hasAvailableChannel));
            bool isOutOfCountry = message.IsOutOfCountry;
            if (message.IsOutOfHome)
            {
                this._lastListState = this.CurrentListState;
                this.CurrentListState = VisualStates.OutOfHome;
                if (!this.CurrentChannel.IsAvailableOutOfHome)
                {
                    if (this.OutOfHomeChannels[0].IsAvailableOutOfHome)
                    {
                        this.CurrentChannel = this.OutOfHomeChannels[0];
                        this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.IHtoOOHLive, "", ErrorMessageType.Message, 0x2710, dismissedEvent, true));
                    }
                    else
                    {
                        hasAvailableChannel = false;
                        this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleNotAvailable, MessageText.IHtoOOHNoAvailable, "", ErrorMessageType.Message, 0x2710, dismissedEvent, true));
                    }
                }
                else
                {
                    this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.IHtoOOHLive, "", ErrorMessageType.Message, 0x2710, dismissedEvent, true));
                }
            }
            else
            {
                this.CurrentListState = this._lastListState;
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleIH, MessageText.OOHtoIHLive, "", ErrorMessageType.Message, 0x2710, dismissedEvent, true));
            }
        }

        public void Handle(SelectChannelMessage message)
        {
            ChannelViewModel model = Enumerable.FirstOrDefault<ChannelViewModel>(this._allChannels, c => c.TmsId == message.TmsId);
            if (model != null)
            {
                this.CurrentChannel = model;
            }
        }

        private void InitializeChannelHistory()
        {
            System.Func<ChannelViewModel, bool> func2 = null;
            System.Func<ChannelViewModel, bool> func3 = null;
            List<string> recentChannels = this._settingService.RecentChannels;
            this.RecentChannels.Clear();
            using (List<string>.Enumerator enumerator = recentChannels.GetEnumerator())
            {
                System.Func<ChannelViewModel, bool> func = null;
                string id;
                while (enumerator.MoveNext())
                {
                    id = enumerator.Current;
                    if (func == null)
                    {
                        func = c => c.TmsId == id;
                    }
                    ChannelViewModel item = Enumerable.FirstOrDefault<ChannelViewModel>(this._allChannels, func);
                    if (item != null)
                    {
                        this.RecentChannels.Add(item);
                    }
                }
            }
            bool local1 = this._LocationService.IsOutOfCountry.Value;
            if (this._LocationService.IsOutOfHome.Value)
            {
                this.PlayDefaultOOHChannel();
            }
            else
            {
                ChannelViewModel channelToRestore = null;
                if (this.StartupTmsId.IsNotNullOrEmpty())
                {
                    if (func2 == null)
                    {
                        func2 = c => c.TmsId == this.StartupTmsId;
                    }
                    if (Enumerable.Any<ChannelViewModel>(this._allChannels, func2))
                    {
                        if (func3 == null)
                        {
                            func3 = c => c.TmsId == this.StartupTmsId;
                        }
                        channelToRestore = Enumerable.First<ChannelViewModel>(this._allChannels, func3);
                    }
                }
                if (channelToRestore == null)
                {
                    string channelStreamId = recentChannels.FirstOrDefault<string>();
                    if (channelStreamId != null)
                    {
                        channelToRestore = Enumerable.FirstOrDefault<ChannelViewModel>(this._allChannels, c => c.TmsId == channelStreamId);
                    }
                }
                if (channelToRestore == null)
                {
                    channelToRestore = this._allChannels.FirstOrDefault<ChannelViewModel>();
                }
                if ((channelToRestore != null) && Enumerable.Any<ChannelViewModel>(this.FilteredChannels, c => c.TmsId == channelToRestore.TmsId))
                {
                    this.CurrentChannel = channelToRestore;
                    this._eventAggregator.Publish(new EGChannelListChannelSelectedEventMessage("TMS", this.CurrentChannel.TmsId, "tabLoaded"));
                }
            }
        }

        private void InitializeProc()
        {
            System.Action<bool, bool, bool> callback = null;
            Action<ChannelList> action2 = null;
            System.Action action = null;
            System.Action action4 = null;
            Action<GenreData> action5 = null;
            System.Func<string, string> processJson = null;
            Action<GenreServiceData> action6 = null;
            System.Func<string, string> func2 = null;
            System.Action action7 = null;
            try
            {
                if (!this._LocationService.IsOutOfHome.HasValue)
                {
                    if (callback == null)
                    {
                        callback = delegate (bool isOOH, bool isOutOfCountry, bool isLocationChanged) {
                            if (isOutOfCountry)
                            {
                                this.CurrentListState = VisualStates.OutOfHome;
                                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleNotAvailable, MessageText.OOCNotAvailable, "", ErrorMessageType.Message, 0x3e8, null, true));
                            }
                            else if (isOOH)
                            {
                                this.CurrentListState = VisualStates.OutOfHome;
                                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.LaunchOOH, "", ErrorMessageType.Message, 0x2710, null, true));
                            }
                        };
                    }
                    this._LocationService.CheckLocation(callback);
                }
                else if (this._LocationService.IsOutOfCountry.Value)
                {
                    this.CurrentListState = VisualStates.OutOfHome;
                    this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleNotAvailable, MessageText.OOCNotAvailable, "", ErrorMessageType.Message, 0x3e8, null, true));
                }
                else if (this._LocationService.IsOutOfHome.Value)
                {
                    this.CurrentListState = VisualStates.OutOfHome;
                }
                if (action2 == null)
                {
                    action2 = channelList => delegate {
                        this.FilteredChannels = channelList.ToObservable();
                        this.OutOfHomeChannels = ToOutOfHomeList(this.FilteredChannels);
                        this._allChannels = this.FilteredChannels.ToList<ChannelViewModel>();
                    }.OnUIThread();
                }
                LiveTVService.LoadChannels(action2, delegate (Exception ex) {
                    throw new Exception("LoadChannels failed", ex);
                });
                if (action == null)
                {
                    action = delegate {
                        this.LoadCachedGenreData();
                        this.RefreshGenres();
                        this.RestoreSelectedGenre();
                    };
                }
                action.OnUIThread();
                this.RefreshWhatsOn();
                if (action4 == null)
                {
                    action4 = () => this.InitializeChannelHistory();
                }
                action4.OnUIThread();
                if (action5 == null)
                {
                    action5 = genreData => ((System.Action) (() => (this.Genres = (from g in genreData.Genres.GenreInfos
                        where g.IsEnabled
                        select g).ToList<GenreInfo>()))).OnUIThread();
                }
                if (processJson == null)
                {
                    processJson = delegate (string genreJson) {
                        this._settingService.CachedGenreData = genreJson;
                        return genreJson;
                    };
                }
                LiveTVService.LoadGenreData(action5, delegate (Exception ex) {
                    throw new Exception("LoadGenreData failed", ex);
                }, processJson);
                if (action6 == null)
                {
                    action6 = genreServiceData => delegate {
                        this.GenreServices = genreServiceData.Services.ServiceInfos;
                        int selectedFilterIndex = this.SelectedFilterIndex;
                        this.RefreshGenres();
                        if (selectedFilterIndex > -1)
                        {
                            this.SelectedFilterIndex = selectedFilterIndex;
                        }
                        else
                        {
                            this.SelectedFilterIndex = 0;
                        }
                    }.OnUIThread();
                }
                if (func2 == null)
                {
                    func2 = delegate (string genreServiceJson) {
                        this._settingService.CachedGenreServiceData = genreServiceJson;
                        return genreServiceJson;
                    };
                }
                LiveTVService.LoadGenreServices(action6, delegate (Exception ex) {
                    throw new Exception("LoadGenreServices failed", ex);
                }, func2);
                this._IntervalManager.Reset();
            }
            catch (Exception exception)
            {
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleDefault, MessageText.PerformanceErrorMessage, exception.ToString(), ErrorMessageType.Message, 0x3e8, null, true));
                if (action7 == null)
                {
                    action7 = delegate {
                        this._RetryTimer.Interval = this._IntervalManager.GetNextInterval();
                        this._RetryTimer.Start();
                    };
                }
                action7.OnUIThread();
            }
        }

        private void LoadCachedGenreData()
        {
            try
            {
                if (this._settingService.CachedGenreData.IsNotNullOrEmpty() && this._settingService.CachedGenreServiceData.IsNotNullOrEmpty())
                {
                    this.Genres = (from g in this._settingService.CachedGenreData.DeserializeFromJson<GenreData>().Genres.GenreInfos
                        where g.IsEnabled
                        select g).ToList<GenreInfo>();
                    this.GenreServices = this._settingService.CachedGenreServiceData.DeserializeFromJson<GenreServiceData>().Services.ServiceInfos;
                }
                else
                {
                    this.Genres = new List<GenreInfo>();
                    this.GenreServices = new List<GenreServiceInfo>();
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.ToString());
            }
        }

        public bool NextChannel()
        {
            if (this.CurrentListState == VisualStates.OutOfHome)
            {
                int index = this.OutOfHomeChannels.IndexOf(this.CurrentChannel);
                if (((index != -1) && (index < this.OutOfHomeChannels.Count)) && this.OutOfHomeChannels[index + 1].IsAvailableOutOfHome)
                {
                    this.CurrentChannel = this.OutOfHomeChannels[index + 1];
                    return true;
                }
            }
            else
            {
                int num2 = this.FilteredChannels.IndexOf(this.CurrentChannel);
                if ((num2 != -1) && (num2 < this.FilteredChannels.Count))
                {
                    this.CurrentChannel = this.FilteredChannels[num2 + 1];
                    return true;
                }
            }
            return false;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (!HtmlPage.Document.DocumentUri.Host.Contains("localhost"))
            {
                this._LocationService.UpdateInitialLocationFromCookie();
                Task.Factory.StartNew(new System.Action(this.InitializeProc));
            }
        }

        private void PlayDefaultOOHChannel()
        {
            System.Action dismissedEvent = null;
            if (this.OutOfHomeChannels[0].IsAvailableOutOfHome)
            {
                this.CurrentChannel = this.OutOfHomeChannels[0];
            }
            else
            {
                if (dismissedEvent == null)
                {
                    dismissedEvent = () => this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(this._LocationService.IsOutOfHome.Value, this._LocationService.IsOutOfCountry.Value, false, false));
                }
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.IHtoOOHNoAvailable, "", ErrorMessageType.Message, 0x2710, dismissedEvent, true));
            }
        }

        public bool PreviousChannel()
        {
            if (this.CurrentListState == VisualStates.OutOfHome)
            {
                int index = this.OutOfHomeChannels.IndexOf(this.CurrentChannel);
                if ((index != -1) && (index > 0))
                {
                    this.CurrentChannel = this.OutOfHomeChannels[index - 1];
                    return true;
                }
            }
            else
            {
                int num2 = this.FilteredChannels.IndexOf(this.CurrentChannel);
                if ((num2 != -1) && (num2 > 0))
                {
                    this.CurrentChannel = this.FilteredChannels[num2 - 1];
                    return true;
                }
            }
            return false;
        }

        private void RefreshFilteredChannels(TWC.OVP.ViewModels.Filter filter, string searchText)
        {
            IEnumerable<ChannelViewModel> enumerable;
            var func = null;
            string str;
            ObservableCollection<ChannelViewModel> observables = new ObservableCollection<ChannelViewModel>();
            if (((str = filter.FilterCriteria) != null) && (str == "All"))
            {
                enumerable = this._allChannels;
            }
            else
            {
                enumerable = this._channelGroups[filter.FilterCriteria];
            }
            if (enumerable != null)
            {
                if (!searchText.IsNullOrWhiteSpace())
                {
                    if (func == null)
                    {
                        func = c => new { Channel = c, MatchesNow = (c.OnNow == null) ? false : (c.OnNow.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ? true : c.OnNow.EpisodeName.Contains(searchText, StringComparison.OrdinalIgnoreCase)), MatchesNext = (c.OnNext == null) ? false : (c.OnNext.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ? true : c.OnNext.EpisodeName.Contains(searchText, StringComparison.OrdinalIgnoreCase)), MatchesNetwork = c.NetworkName.Contains(searchText, StringComparison.OrdinalIgnoreCase) };
                    }
                    var list = Enumerable.Where(Enumerable.Select(enumerable, func), delegate (<>f__AnonymousType0<ChannelViewModel, bool, bool, bool> m) {
                        if (!m.MatchesNetwork && !m.MatchesNow)
                        {
                            return m.MatchesNext;
                        }
                        return true;
                    }).ToList();
                    this.OnNowSearchMatchCount = Enumerable.Where(list, delegate (<>f__AnonymousType0<ChannelViewModel, bool, bool, bool> m) {
                        if (!m.MatchesNow)
                        {
                            return m.MatchesNetwork;
                        }
                        return true;
                    }).Count();
                    this.OnNextSearchMatchCount = (from m in list
                        where m.MatchesNext
                        select m).Count();
                    foreach (var type in list)
                    {
                        type.Channel.SearchText = searchText;
                        type.Channel.HighlightLogo = type.MatchesNetwork;
                        observables.Add(type.Channel);
                    }
                }
                else
                {
                    this.OnNowSearchMatchCount = 0;
                    this.OnNextSearchMatchCount = 0;
                    foreach (ChannelViewModel model in enumerable)
                    {
                        model.SearchText = "";
                        model.HighlightLogo = false;
                        observables.Add(model);
                    }
                }
            }
            this.FilteredChannels = observables;
        }

        private void RefreshGenres()
        {
            System.Func<GenreInfo, bool> func2 = null;
            this._channelGroups = new Dictionary<string, List<ChannelViewModel>>();
            using (IEnumerator<GenreInfo> enumerator = (from g in this.Genres
                where g.IsEnabled
                select g).GetEnumerator())
            {
                System.Func<GenreServiceInfo, bool> func = null;
                GenreInfo genre;
                while (enumerator.MoveNext())
                {
                    genre = enumerator.Current;
                    List<ChannelViewModel> list = new List<ChannelViewModel>();
                    if (func == null)
                    {
                        func = s => (genre != null) && (s.Genre == genre.Name);
                    }
                    IEnumerable<string> genreProviders = from s in Enumerable.Where<GenreServiceInfo>(this.GenreServices, func) select s.Provider;
                    IEnumerable<ChannelViewModel> collection = from c in this._allChannels
                        where genreProviders.Contains<string>(c.CallSign)
                        select c;
                    list.AddRange(collection);
                    this._channelGroups.Add(genre.Name, list);
                }
            }
            ObservableCollection<TWC.OVP.ViewModels.Filter> observables = new ObservableCollection<TWC.OVP.ViewModels.Filter>();
            TWC.OVP.ViewModels.Filter item = new TWC.OVP.ViewModels.Filter {
                FilterCriteria = "All",
                FilterDisplayText = "All Channels"
            };
            observables.Add(item);
            TWC.OVP.ViewModels.Filter filter3 = new TWC.OVP.ViewModels.Filter {
                FilterCriteria = "Recent",
                FilterDisplayText = "Recent History"
            };
            observables.Add(filter3);
            this.FilterOptions = observables;
            if (this.Genres != null)
            {
                if (func2 == null)
                {
                    func2 = g => this._channelGroups.ContainsKey(g.Name) && this._channelGroups[g.Name].Any<ChannelViewModel>();
                }
                foreach (GenreInfo info in Enumerable.Where<GenreInfo>(this.Genres, func2))
                {
                    TWC.OVP.ViewModels.Filter filter = new TWC.OVP.ViewModels.Filter {
                        FilterCriteria = info.Name,
                        FilterDisplayText = info.Name
                    };
                    this.FilterOptions.Add(filter);
                }
            }
        }

        public IResult RefreshWhatsOn()
        {
            return LiveTVService.LoadWhatsOn(delegate (WhatsOnList whatsOnList) {
                delegate {
                    foreach (WhatsOnItems items in whatsOnList.Items)
                    {
                        WhatsOn whatsOnNow = items.Items.FirstOrDefault<WhatsOn>();
                        if ((whatsOnNow != null) && (this.FilteredChannels != null))
                        {
                            ChannelViewModel model = Enumerable.FirstOrDefault<ChannelViewModel>(this._allChannels, c => c.TmsId == whatsOnNow.TmsId);
                            if (model != null)
                            {
                                model.Apply(items);
                            }
                        }
                    }
                }.OnUIThread();
            }, delegate (Exception ex) {
                throw new Exception("LoadWhatsOn failed", ex);
            });
        }

        private void RestoreSelectedGenre()
        {
            TWC.OVP.ViewModels.Filter item = Enumerable.FirstOrDefault<TWC.OVP.ViewModels.Filter>(this.FilterOptions, f => f.FilterCriteria == this._settingService.SelectedChannelFilter);
            if (item != null)
            {
                this.SelectedFilterIndex = this.FilterOptions.IndexOf(item);
            }
            else
            {
                this.SelectedFilterIndex = 0;
            }
            this.FilterSelectionChanged();
        }

        public void ShowOnNext()
        {
            this.OnNowOrNextState = VisualStates.OnNextState;
            if (this._allChannels != null)
            {
                foreach (ChannelViewModel model in this._allChannels)
                {
                    model.ShowWhatsOnNext = true;
                }
            }
        }

        public void ShowOnNow()
        {
            this.OnNowOrNextState = VisualStates.OnNowState;
            if (this._allChannels != null)
            {
                foreach (ChannelViewModel model in this._allChannels)
                {
                    model.ShowWhatsOnNext = false;
                }
            }
        }

        public void ToggleMenuState()
        {
            if (this.CurrentListState == VisualStates.OptionsState)
            {
                this.CurrentListState = this._lastState;
            }
            else
            {
                this._lastState = this.CurrentListState;
                this.CurrentListState = VisualStates.OptionsState;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedFilterIndex)), new ParameterExpression[0]));
            }
        }

        public static IList<ChannelViewModel> ToOutOfHomeList(IList<ChannelViewModel> channelList)
        {
            BindableCollection<ChannelViewModel> bindables = new BindableCollection<ChannelViewModel>();
            IOrderedEnumerable<ChannelViewModel> first = from c in channelList
                where c.IsAvailableOutOfHome
                orderby c.NetworkName
                select c;
            IOrderedEnumerable<ChannelViewModel> enumerable2 = from c in channelList
                where !c.NetworkName.StartsWith("~") && !c.IsAvailableOutOfHome
                orderby c.NetworkName
                select c;
            IOrderedEnumerable<ChannelViewModel> second = from c in channelList
                where c.NetworkName.StartsWith("~") && !c.IsAvailableOutOfHome
                orderby c.NetworkName
                select c;
            foreach (ChannelViewModel model in first.Union<ChannelViewModel>(enumerable2.Union<ChannelViewModel>(second)))
            {
                bindables.Add(model);
            }
            return bindables;
        }

        private void UpdateListBoxSelection(ChannelViewModel channelVM)
        {
            if (channelVM != null)
            {
                this._selectedChannelAll = channelVM;
                this._selectedChannelRecent = channelVM;
                if (channelVM.IsAvailableOutOfHome)
                {
                    this._selectedChannelOOH = channelVM;
                }
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelAll)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelRecent)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelOOH)), new ParameterExpression[0]));
            }
        }

        private void UpdateMenuButtonText()
        {
            this.MenuButtonText = this.FilterOptions[this.SelectedFilterIndex].FilterDisplayText;
        }

        public ChannelViewModel CurrentChannel
        {
            get
            {
                return this._currentChannel;
            }
            set
            {
                if (this._currentChannel != value)
                {
                    this._currentChannel = value;
                    if (value != null)
                    {
                        if (this._recentChannels.Contains(value))
                        {
                            this._recentChannels.Remove(value);
                        }
                        this._recentChannels.Insert(0, value);
                        this._settingService.RecentChannels = (from channelViewModel in this.RecentChannels select channelViewModel.TmsId).Take<string>(10).ToList<string>();
                        this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_CurrentChannel)), new ParameterExpression[0]));
                        this.UpdateListBoxSelection(value);
                        value.PropertyChanged += new PropertyChangedEventHandler(this.channel_PropertyChanged);
                    }
                }
            }
        }

        public string CurrentListState
        {
            get
            {
                return this._currentListState;
            }
            set
            {
                this._currentListState = value;
                this.IsOnNowShown = true;
                this.UpdateMenuButtonText();
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public string CurrentSearchText
        {
            get
            {
                return this._currentSearchText;
            }
            set
            {
                string str = (value ?? "").Trim();
                if (str != this._currentSearchText)
                {
                    this._currentSearchText = str;
                    this.FilterSelectionChanged();
                }
            }
        }

        public IList<ChannelViewModel> FilteredChannels
        {
            get
            {
                return this._filteredChannels;
            }
            set
            {
                if (this._filteredChannels != value)
                {
                    this._filteredChannels = value;
                    this.NotifyOfPropertyChange<IList<ChannelViewModel>>(Expression.Lambda<System.Func<IList<ChannelViewModel>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_FilteredChannels)), new ParameterExpression[0]));
                }
            }
        }

        public ObservableCollection<TWC.OVP.ViewModels.Filter> FilterOptions
        {
            get
            {
                return this._filterOptions;
            }
            set
            {
                this._filterOptions = value;
                this.NotifyOfPropertyChange<ObservableCollection<TWC.OVP.ViewModels.Filter>>(Expression.Lambda<System.Func<ObservableCollection<TWC.OVP.ViewModels.Filter>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_FilterOptions)), new ParameterExpression[0]));
            }
        }

        public IList<GenreInfo> Genres
        {
            get
            {
                return this._genres;
            }
            set
            {
                if (this._genres != value)
                {
                    this._genres = value;
                    this.NotifyOfPropertyChange<IList<GenreInfo>>(Expression.Lambda<System.Func<IList<GenreInfo>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_Genres)), new ParameterExpression[0]));
                }
            }
        }

        public IList<GenreServiceInfo> GenreServices
        {
            get
            {
                return this._genreServices;
            }
            set
            {
                if (this._genreServices != value)
                {
                    this._genreServices = value;
                    this.NotifyOfPropertyChange<IList<GenreServiceInfo>>(Expression.Lambda<System.Func<IList<GenreServiceInfo>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_GenreServices)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsOnNextEnabled
        {
            get
            {
                return this._isOnNextEnabled;
            }
            set
            {
                this._isOnNextEnabled = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_IsOnNextEnabled)), new ParameterExpression[0]));
            }
        }

        public bool IsOnNowShown
        {
            get
            {
                return this._isOnNowShown;
            }
            set
            {
                this._isOnNowShown = value;
                if (this._isOnNowShown)
                {
                    this.ShowOnNow();
                }
                else
                {
                    this.ShowOnNext();
                }
                this.NotifyOfPropertyChange("IsOnNowShown");
            }
        }

        public bool IsSearchEnabled
        {
            get
            {
                return this._isSearchEnabled;
            }
            set
            {
                this._isSearchEnabled = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_IsSearchEnabled)), new ParameterExpression[0]));
            }
        }

        public string MenuButtonText
        {
            get
            {
                return this._menuButtonText;
            }
            set
            {
                this._menuButtonText = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_MenuButtonText)), new ParameterExpression[0]));
            }
        }

        public int OnNextSearchMatchCount
        {
            get
            {
                return this._onNextSearchMatchCount;
            }
            set
            {
                this._onNextSearchMatchCount = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_OnNextSearchMatchText)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_OnNextSearchMatchCount)), new ParameterExpression[0]));
            }
        }

        public string OnNextSearchMatchText
        {
            get
            {
                if (this.OnNextSearchMatchCount != 0)
                {
                    return string.Format("({0})", this.OnNextSearchMatchCount);
                }
                return string.Empty;
            }
        }

        public string OnNowOrNextState
        {
            get
            {
                return this._onNowOrNextState;
            }
            set
            {
                this._onNowOrNextState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public int OnNowSearchMatchCount
        {
            get
            {
                return this._onNowSearchMatchCount;
            }
            set
            {
                this._onNowSearchMatchCount = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_OnNowSearchMatchText)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_OnNowSearchMatchCount)), new ParameterExpression[0]));
            }
        }

        public string OnNowSearchMatchText
        {
            get
            {
                if (this.OnNowSearchMatchCount != 0)
                {
                    return string.Format("({0})", this.OnNowSearchMatchCount);
                }
                return string.Empty;
            }
        }

        public IList<ChannelViewModel> OutOfHomeChannels
        {
            get
            {
                return this._outOfHomeChannels;
            }
            set
            {
                this._outOfHomeChannels = value;
                this.NotifyOfPropertyChange<IList<ChannelViewModel>>(Expression.Lambda<System.Func<IList<ChannelViewModel>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_OutOfHomeChannels)), new ParameterExpression[0]));
            }
        }

        public ObservableCollection<ChannelViewModel> RecentChannels
        {
            get
            {
                return this._recentChannels;
            }
            set
            {
                this._recentChannels = value;
                this.NotifyOfPropertyChange<ObservableCollection<ChannelViewModel>>(Expression.Lambda<System.Func<ObservableCollection<ChannelViewModel>>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_RecentChannels)), new ParameterExpression[0]));
            }
        }

        public bool SearchHasFocus
        {
            get
            {
                return this._searchHasFocus;
            }
            set
            {
                this._searchHasFocus = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SearchHasFocus)), new ParameterExpression[0]));
            }
        }

        public ChannelViewModel SelectedChannelAll
        {
            get
            {
                return this._selectedChannelAll;
            }
            set
            {
                this._selectedChannelAll = value;
                if (this._selectedChannelAll != null)
                {
                    this._eventAggregator.Publish(new EGChannelListChannelSelectedEventMessage("TMS", this._selectedChannelAll.TmsId, "userSelected"));
                    this.CurrentChannel = this._selectedChannelAll;
                }
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelAll)), new ParameterExpression[0]));
            }
        }

        public ChannelViewModel SelectedChannelOOH
        {
            get
            {
                return this._selectedChannelOOH;
            }
            set
            {
                this._selectedChannelOOH = value;
                if (this._selectedChannelOOH != null)
                {
                    this._eventAggregator.Publish(new EGChannelListChannelSelectedEventMessage("TMS", this._selectedChannelOOH.TmsId, "userSelected"));
                    this.CurrentChannel = this._selectedChannelOOH;
                }
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelOOH)), new ParameterExpression[0]));
            }
        }

        public ChannelViewModel SelectedChannelRecent
        {
            get
            {
                return this._selectedChannelRecent;
            }
            set
            {
                this._selectedChannelRecent = value;
                if (this._selectedChannelRecent != null)
                {
                    this._eventAggregator.Publish(new EGChannelListChannelSelectedEventMessage("TMS", this._selectedChannelRecent.TmsId, "userSelected"));
                    this.CurrentChannel = this._selectedChannelRecent;
                }
                this.NotifyOfPropertyChange<ChannelViewModel>(Expression.Lambda<System.Func<ChannelViewModel>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedChannelRecent)), new ParameterExpression[0]));
            }
        }

        public int SelectedFilterIndex
        {
            get
            {
                return this._selectedFilterIndex;
            }
            set
            {
                if ((value != -1) && (this._selectedFilterIndex != value))
                {
                    this._selectedFilterIndex = value;
                    this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(ChannelBrowserViewModel)), (MethodInfo) methodof(ChannelBrowserViewModel.get_SelectedFilterIndex)), new ParameterExpression[0]));
                }
            }
        }

        public string StartupTmsId { get; set; }


        public static class VisualStates
        {
            public static readonly string AllChannelsState = "AllChannels";
            public static readonly string NoResultsState = "NoResults";
            public static readonly string OnNextState = "OnNext";
            public static readonly string OnNowState = "OnNow";
            public static readonly string OptionsState = "Options";
            public static readonly string OutOfHome = "OutOfHome";
            public static readonly string RecentHistoryState = "RecentHistory";
        }
    }
}

