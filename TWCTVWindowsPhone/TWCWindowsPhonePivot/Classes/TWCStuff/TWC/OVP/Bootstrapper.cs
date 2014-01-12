namespace TWC.OVP
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Browser;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Framework.Utilities;
    using TWC.OVP.Services;
    using TWC.OVP.ViewModels;

    public class Bootstrapper : Caliburn.Micro.Bootstrapper
    {
        private SimpleContainer container;

        public Bootstrapper() : base(true)
        {
            LogManager.GetLog = type => new DebugLog(type, true);
        }

        public Bootstrapper(bool useApplication = true) : base(useApplication)
        {
            LogManager.GetLog = type => new DebugLog(type, true);
        }

        [CompilerGenerated]
        private IEnumerable<Assembly> <>n__FabricatedMethod8()
        {
            return base.SelectAssemblies();
        }

        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }

        protected override object GetInstance(Type service, string key)
        {
            return this.container.GetInstance(service, key);
        }

        public void Initialize(AppMode appMode, string startupStreamUrl, string startupTmsId, bool isDiagModeEnabled)
        {
            this.container = new SimpleContainer();
            this.container.RegisterSingleton(typeof(IEventAggregator), null, typeof(EventAggregator));
            this.container.RegisterSingleton(typeof(ISettingsService), null, typeof(SettingsService));
            this.container.RegisterSingleton(typeof(ScriptBridge), null, typeof(ScriptBridge));
            this.container.RegisterSingleton(typeof(LocationService), null, typeof(LocationService));
            this.container.GetInstance<ScriptBridge>();
            this.container.GetInstance<ISettingsService>().AppMode = appMode;
            this.container.RegisterSingleton(typeof(PlayerViewModel), null, typeof(PlayerViewModel));
            this.container.RegisterSingleton(typeof(InteractionViewModel), null, typeof(InteractionViewModel));
            this.container.RegisterSingleton(typeof(AssetViewerViewModel), null, typeof(AssetViewerViewModel));
            this.container.RegisterSingleton(typeof(AssetInfoViewModel), null, typeof(AssetInfoViewModel));
            this.container.RegisterSingleton(typeof(CaptionSettingsViewModel), null, typeof(CaptionSettingsViewModel));
            this.container.GetInstance<AssetViewerViewModel>().ShowErrorDetail = isDiagModeEnabled;
            switch (appMode)
            {
                case AppMode.Default:
                    this.container.RegisterSingleton(typeof(BaseShellViewModel), null, typeof(OnDemandShellViewModel));
                    break;

                case AppMode.Live:
                    this.container.RegisterSingleton(typeof(ChannelBrowserViewModel), null, typeof(ChannelBrowserViewModel));
                    this.container.RegisterSingleton(typeof(BaseShellViewModel), null, typeof(LiveTVShellViewModel));
                    if (startupTmsId.IsNotNullOrEmpty())
                    {
                        this.container.GetInstance<ChannelBrowserViewModel>().StartupTmsId = startupTmsId;
                    }
                    break;

                case AppMode.SportsNetwork:
                    this.container.RegisterSingleton(typeof(BaseShellViewModel), null, typeof(SportsNetworkShellViewModel));
                    if (startupStreamUrl.IsNotNullOrEmpty())
                    {
                        SportsNetworkShellViewModel instance = (SportsNetworkShellViewModel) this.container.GetInstance<BaseShellViewModel>();
                        instance.StartupStreamUrl = startupStreamUrl;
                    }
                    break;
            }
            HtmlPage.RegisterScriptableObject("Bridge", this.container.GetInstance<ScriptBridge>());
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            string str3;
            AppMode appMode = AppMode.Default;
            if (e.InitParams.ContainsKey("mode") && ((str3 = e.InitParams["mode"]) != null))
            {
                if (!(str3 == "live"))
                {
                    if (str3 == "ondemand")
                    {
                        appMode = AppMode.Default;
                    }
                    else if (str3 == "sportsnetwork")
                    {
                        appMode = AppMode.SportsNetwork;
                    }
                }
                else
                {
                    appMode = AppMode.Live;
                }
            }
            ((App) Application.Current).OVPApplicationMode = appMode;
            string startupStreamUrl = null;
            if (e.InitParams.ContainsKey("stream"))
            {
                startupStreamUrl = e.InitParams["stream"];
            }
            if (e.InitParams.ContainsKey("isAdEnabled"))
            {
                bool flag = false;
                bool.TryParse(e.InitParams["isAdEnabled"], out flag);
                ((App) Application.Current).IsAdEnabled = flag;
            }
            string startupTmsId = null;
            if (HtmlPage.Document.QueryString.ContainsKey("tmsid"))
            {
                startupTmsId = HtmlPage.Document.QueryString["tmsid"];
            }
            bool result = false;
            if (HtmlPage.Document.QueryString.ContainsKey("diagnostics"))
            {
                bool.TryParse(HtmlPage.Document.QueryString["diagnostics"], out result);
            }
            this.Initialize(appMode, startupStreamUrl, startupTmsId, result);
            ((App) Application.Current).EventAggregator = this.container.GetInstance<IEventAggregator>();
            ((App) Application.Current).PlayerViewModel = this.container.GetInstance<PlayerViewModel>();
            ((App) Application.Current).LocationService = this.container.GetInstance<LocationService>();
            ((App) Application.Current).InteractionViewModel = this.container.GetInstance<InteractionViewModel>();
            Caliburn.Micro.Bootstrapper.DisplayRootViewFor(Application.Current, typeof(BaseShellViewModel));
            ((App) Application.Current).ShellViewModel = this.container.GetInstance<BaseShellViewModel>();
            this.container.GetInstance<ScriptBridge>().OnAppLoaded();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            foreach (Assembly iteratorVariable0 in this.<>n__FabricatedMethod8())
            {
                yield return iteratorVariable0;
            }
            yield return typeof(AssetViewerViewModel).Assembly;
            yield return typeof(LiveTVShellViewModel).Assembly;
        }

    }
}

