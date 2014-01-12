namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using TWC.OVP.Messages;
    using TWC.OVP.Services;

    public class SportsNetworkShellViewModel : BaseShellViewModel, IHandle<StartLiveStreamMessage>, IHandle
    {
        private string _currentControllerVisualState;
        private bool _isMouseOverController;
        private bool _isVolumeExpanded;
        private ISettingsService _settingsService;

        public SportsNetworkShellViewModel(AssetViewerViewModel assetViewerViewModel, InteractionViewModel interactionViewModel, CaptionSettingsViewModel captionSettingsViewModel, IEventAggregator eventAggregator, ISettingsService settingsService) : base(eventAggregator)
        {
            base.AssetViewer = assetViewerViewModel;
            base.Interaction = interactionViewModel;
            this._settingsService = settingsService;
            base.CaptionSettings = captionSettingsViewModel;
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentControllerVisualState;
            yield return this.CurrentScreenState;
            yield return this.CurrentCaptionSettingsState;
            yield return this.CurrentCaptionBubblePopupState;
        }

        public void Handle(StartLiveStreamMessage message)
        {
            this.StartStream(message.StreamUrl);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ActivateItem(base.AssetViewer);
            this.ActivateItem(base.CaptionSettings);
            if (!string.IsNullOrEmpty(this.StartupStreamUrl))
            {
                this.StartStream(this.StartupStreamUrl);
            }
        }

        protected override void RefreshThreadTasks()
        {
            base.RefreshThreadTasks();
            () => base.AssetViewer.NotifyBeacon().OnUIThread();
        }

        public void ShowController()
        {
            base.AssetViewer.CurrentControllerState = "ShowController";
        }

        private void StartStream(string streamUrl)
        {
            AssetViewModel model = new AssetViewModel {
                IsLive = true,
                IsEncrypted = true,
                StreamUrl = streamUrl
            };
            base.AssetViewer.Asset = model;
            base.StartRefreshThread();
        }

        public string CurrentControllerVisualState
        {
            get
            {
                return this._currentControllerVisualState;
            }
            set
            {
                this._currentControllerVisualState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(SportsNetworkShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverController
        {
            get
            {
                return this._isMouseOverController;
            }
            set
            {
                this._isMouseOverController = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(SportsNetworkShellViewModel)), (MethodInfo) methodof(SportsNetworkShellViewModel.get_IsMouseOverController)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(SportsNetworkShellViewModel)), (MethodInfo) methodof(SportsNetworkShellViewModel.get_RestartControllerTimeout)), new ParameterExpression[0]));
            }
        }

        public bool IsVolumeExpanded
        {
            get
            {
                return this._isVolumeExpanded;
            }
            set
            {
                this._isVolumeExpanded = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(SportsNetworkShellViewModel)), (MethodInfo) methodof(SportsNetworkShellViewModel.get_IsVolumeExpanded)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(SportsNetworkShellViewModel)), (MethodInfo) methodof(SportsNetworkShellViewModel.get_RestartControllerTimeout)), new ParameterExpression[0]));
            }
        }

        public bool RestartControllerTimeout
        {
            get
            {
                if (!this.IsVolumeExpanded)
                {
                    return this.IsMouseOverController;
                }
                return true;
            }
        }

        public string StartupStreamUrl { get; set; }

    }
}

