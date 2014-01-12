namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using TWC.OVP.Services;

    public class OnDemandShellViewModel : BaseShellViewModel
    {
        private LocationService _LocationService;
        private ISettingsService _settingsService;

        public OnDemandShellViewModel(AssetViewerViewModel assetViewerViewModel, InteractionViewModel interactionViewModel, CaptionSettingsViewModel captionSettingsViewModel, ISettingsService settingsService, LocationService locationService, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this._settingsService = settingsService;
            this._LocationService = locationService;
            base.AssetViewer = assetViewerViewModel;
            base.Interaction = interactionViewModel;
            base.CaptionSettings = captionSettingsViewModel;
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentAssetInfoState;
            yield return this.CurrentCaptionSettingsState;
            yield return this.CurrentCaptionBubblePopupState;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ActivateItem(base.AssetViewer);
            this.ActivateItem(base.CaptionSettings);
            this._LocationService.UpdateInitialLocationFromCookie();
        }

        public void ToggleControlBar()
        {
            base.IsControllerHide = !base.IsControllerHide;
        }

    }
}

