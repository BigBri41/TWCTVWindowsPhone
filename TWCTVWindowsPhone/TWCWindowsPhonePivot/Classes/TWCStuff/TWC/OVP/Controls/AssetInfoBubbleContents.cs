namespace TWC.OVP.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TWC.OVP.Framework.Controls;
    using TWC.OVP.ViewModels;

    public class AssetInfoBubbleContents : UserControl
    {
        private AssetViewModel _assetViewModel;
        private bool _contentLoaded;
        internal VisualState Actors;
        internal TextBlock Actors1;
        internal VisualStateGroup ActorsStates;
        internal TextBlock ActorsValues;
        internal VisualState Asset;
        internal TextBlock AssetInfoBodyTextBlock;
        internal VisualState AssetNoMetaData;
        internal Grid ContentGrid;
        internal Grid CornerLogoGrid;
        internal Image coverArt;
        internal VisualState CoverArt;
        internal VisualState Description;
        internal Grid DescriptionGrid;
        internal VisualStateGroup DescriptionStates;
        internal VisualState Director;
        internal TextBlock Director1;
        internal VisualStateGroup DirectorStates;
        internal TextBlock DirectorValues;
        internal VisualState EpisodeName;
        internal VisualStateGroup EpisodeNameStates;
        internal TextBlock EpisodeNameTextBlock;
        internal VisualState EpisodeProgramType;
        internal TextBlock EpisodeTextBlock;
        internal Grid EpisodeTitleGrid;
        internal TextBlock ErrorCopyTextBlock;
        internal Grid ErrorGrid;
        internal VisualState Expires;
        internal TextBlock Expires1;
        internal VisualStateGroup ExpiresStates;
        internal TextBlock ExpiresValues;
        internal Grid ImageGrid;
        internal VisualStateGroup ImageStates;
        internal Grid LoadedGrid;
        internal VisualState Loading;
        internal Grid LoadingGrid;
        internal Grid MetaDataGrid;
        internal Grid MissingImageGrid;
        internal VisualState MovieProgramType;
        internal Image NetworkLogo;
        private bool networkLogoLoaded;
        internal VisualState NoActors;
        internal VisualState NoCoverArt;
        internal VisualState NoCoverArtUseNetworkLogo;
        internal VisualState NoDescription;
        internal VisualState NoDirector;
        internal VisualState NoEpisodeName;
        internal VisualState NoExpires;
        internal VisualState NoInfoError;
        internal VisualState NoRuntime;
        internal VisualState NoSeasonAndEpisode;
        internal VisualState NoYear;
        internal VisualStateGroup PanelStates;
        internal Image PopupNetworkLogo;
        internal VisualStateGroup ProgramTypeStates;
        internal ContentRatingControl ratingControl;
        internal StackPanel RatingYearGrid;
        internal VisualState Runtime;
        internal TextBlock Runtime1;
        internal VisualStateGroup RuntimeStates;
        internal TextBlock RuntimeValues;
        internal VisualState SeasonAndEpisode;
        internal VisualStateGroup SeasonAndEpisodeNumberStates;
        internal TextBlock SeasonTextBlock;
        internal LoadSpinnerControl Spinner;
        internal TextBlock TitleTextBlock;
        internal VisualState Year;
        internal VisualStateGroup YearStates;
        internal TextBlock YearTextBlock;

        public AssetInfoBubbleContents()
        {
            this.InitializeComponent();
            base.add_DataContextChanged(new DependencyPropertyChangedEventHandler(this.AssetInfoPanelDataContextChanged));
            this.coverArt.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(this.CoverArtImageFailed);
            this.coverArt.ImageOpened += new EventHandler<RoutedEventArgs>(this.CoverArtImageOpened);
            this.PopupNetworkLogo.ImageOpened += new EventHandler<RoutedEventArgs>(this.PopupNetworkLogoImageOpened);
            this.ratingControl.MouseLeftButtonDown += new MouseButtonEventHandler(this.RatingControlMouseLeftButtonDown);
        }

        private void AssetInfoPanelDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, "NoCoverArtUseNetworkLogo", true);
            if (this._assetViewModel != null)
            {
                this._assetViewModel.PropertyChanged -= new PropertyChangedEventHandler(this.AssetViewModelPropertyChanged);
            }
            AssetViewModel dataContext = base.DataContext as AssetViewModel;
            if (dataContext != null)
            {
                this._assetViewModel = dataContext;
                dataContext.PropertyChanged += new PropertyChangedEventHandler(this.AssetViewModelPropertyChanged);
                this.InitializeVisualStates(dataContext);
            }
        }

        private void AssetViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AssetViewModel asset = sender as AssetViewModel;
            this.InitializeVisualStates(asset);
        }

        private void CoverArtImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (this.networkLogoLoaded)
            {
                VisualStateManager.GoToState(this, "NoCoverArtUseNetworkLogo", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoCoverArt", true);
            }
        }

        private void CoverArtImageOpened(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "CoverArt", true);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/AssetInfoBubbleContents.xaml", UriKind.Relative));
                this.PanelStates = (VisualStateGroup) base.FindName("PanelStates");
                this.Asset = (VisualState) base.FindName("Asset");
                this.AssetNoMetaData = (VisualState) base.FindName("AssetNoMetaData");
                this.NoInfoError = (VisualState) base.FindName("NoInfoError");
                this.Loading = (VisualState) base.FindName("Loading");
                this.ProgramTypeStates = (VisualStateGroup) base.FindName("ProgramTypeStates");
                this.MovieProgramType = (VisualState) base.FindName("MovieProgramType");
                this.EpisodeProgramType = (VisualState) base.FindName("EpisodeProgramType");
                this.ImageStates = (VisualStateGroup) base.FindName("ImageStates");
                this.CoverArt = (VisualState) base.FindName("CoverArt");
                this.NoCoverArt = (VisualState) base.FindName("NoCoverArt");
                this.NoCoverArtUseNetworkLogo = (VisualState) base.FindName("NoCoverArtUseNetworkLogo");
                this.YearStates = (VisualStateGroup) base.FindName("YearStates");
                this.Year = (VisualState) base.FindName("Year");
                this.NoYear = (VisualState) base.FindName("NoYear");
                this.EpisodeNameStates = (VisualStateGroup) base.FindName("EpisodeNameStates");
                this.NoEpisodeName = (VisualState) base.FindName("NoEpisodeName");
                this.EpisodeName = (VisualState) base.FindName("EpisodeName");
                this.SeasonAndEpisodeNumberStates = (VisualStateGroup) base.FindName("SeasonAndEpisodeNumberStates");
                this.NoSeasonAndEpisode = (VisualState) base.FindName("NoSeasonAndEpisode");
                this.SeasonAndEpisode = (VisualState) base.FindName("SeasonAndEpisode");
                this.DirectorStates = (VisualStateGroup) base.FindName("DirectorStates");
                this.NoDirector = (VisualState) base.FindName("NoDirector");
                this.Director = (VisualState) base.FindName("Director");
                this.ExpiresStates = (VisualStateGroup) base.FindName("ExpiresStates");
                this.NoExpires = (VisualState) base.FindName("NoExpires");
                this.Expires = (VisualState) base.FindName("Expires");
                this.ActorsStates = (VisualStateGroup) base.FindName("ActorsStates");
                this.NoActors = (VisualState) base.FindName("NoActors");
                this.Actors = (VisualState) base.FindName("Actors");
                this.RuntimeStates = (VisualStateGroup) base.FindName("RuntimeStates");
                this.NoRuntime = (VisualState) base.FindName("NoRuntime");
                this.Runtime = (VisualState) base.FindName("Runtime");
                this.DescriptionStates = (VisualStateGroup) base.FindName("DescriptionStates");
                this.NoDescription = (VisualState) base.FindName("NoDescription");
                this.Description = (VisualState) base.FindName("Description");
                this.LoadedGrid = (Grid) base.FindName("LoadedGrid");
                this.ContentGrid = (Grid) base.FindName("ContentGrid");
                this.TitleTextBlock = (TextBlock) base.FindName("TitleTextBlock");
                this.EpisodeTitleGrid = (Grid) base.FindName("EpisodeTitleGrid");
                this.EpisodeNameTextBlock = (TextBlock) base.FindName("EpisodeNameTextBlock");
                this.SeasonTextBlock = (TextBlock) base.FindName("SeasonTextBlock");
                this.EpisodeTextBlock = (TextBlock) base.FindName("EpisodeTextBlock");
                this.RatingYearGrid = (StackPanel) base.FindName("RatingYearGrid");
                this.ratingControl = (ContentRatingControl) base.FindName("ratingControl");
                this.YearTextBlock = (TextBlock) base.FindName("YearTextBlock");
                this.DescriptionGrid = (Grid) base.FindName("DescriptionGrid");
                this.AssetInfoBodyTextBlock = (TextBlock) base.FindName("AssetInfoBodyTextBlock");
                this.ImageGrid = (Grid) base.FindName("ImageGrid");
                this.coverArt = (Image) base.FindName("coverArt");
                this.MissingImageGrid = (Grid) base.FindName("MissingImageGrid");
                this.NetworkLogo = (Image) base.FindName("NetworkLogo");
                this.ErrorGrid = (Grid) base.FindName("ErrorGrid");
                this.ErrorCopyTextBlock = (TextBlock) base.FindName("ErrorCopyTextBlock");
                this.CornerLogoGrid = (Grid) base.FindName("CornerLogoGrid");
                this.PopupNetworkLogo = (Image) base.FindName("PopupNetworkLogo");
                this.MetaDataGrid = (Grid) base.FindName("MetaDataGrid");
                this.Actors1 = (TextBlock) base.FindName("Actors1");
                this.ActorsValues = (TextBlock) base.FindName("ActorsValues");
                this.Director1 = (TextBlock) base.FindName("Director1");
                this.DirectorValues = (TextBlock) base.FindName("DirectorValues");
                this.Runtime1 = (TextBlock) base.FindName("Runtime1");
                this.RuntimeValues = (TextBlock) base.FindName("RuntimeValues");
                this.Expires1 = (TextBlock) base.FindName("Expires1");
                this.ExpiresValues = (TextBlock) base.FindName("ExpiresValues");
                this.LoadingGrid = (Grid) base.FindName("LoadingGrid");
                this.Spinner = (LoadSpinnerControl) base.FindName("Spinner");
            }
        }

        private void InitializeVisualStates(AssetViewModel asset)
        {
            VisualStateManager.GoToState(this, asset.IsEpisodic ? "EpisodeProgramType" : "MovieProgramType", true);
            VisualStateManager.GoToState(this, (asset.Year == 0) ? "NoYear" : "Year", true);
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(asset.EpisodeName) ? "NoEpisodeName" : "EpisodeName", true);
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(asset.Season) ? "NoSeasonAndEpisode" : "SeasonAndEpisode", true);
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(asset.Description) ? "NoDescription" : "Description", true);
            if (asset.IsLive)
            {
                VisualStateManager.GoToState(this, "NoRuntime", true);
            }
            else
            {
                VisualStateManager.GoToState(this, (asset.Duration == 0) ? "NoRuntime" : "Runtime", true);
            }
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(asset.Cast) ? "NoActors" : "Actors", true);
            VisualStateManager.GoToState(this, !asset.ExpirationDate.HasValue ? "NoExpires" : "Expires", true);
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(asset.Director) ? "NoDirector" : "Director", true);
            if (string.IsNullOrEmpty(asset.Title))
            {
                VisualStateManager.GoToState(this, "NoInfoError", true);
            }
            else if (((asset.Duration == 0) && string.IsNullOrEmpty(asset.Cast)) && (!asset.ExpirationDate.HasValue && string.IsNullOrEmpty(asset.Director)))
            {
                VisualStateManager.GoToState(this, "AssetNoMetaData", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Asset", true);
            }
        }

        private void PopupNetworkLogoImageOpened(object sender, RoutedEventArgs e)
        {
            this.networkLogoLoaded = true;
        }

        private void RatingControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}

