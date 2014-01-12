namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using TWC.OVP.Framework.Models;
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;
    using TWC.OVP.Services;

    public class CaptionSettingsViewModel : BaseViewModel, IHandle<SettingsChangedMessage>, IHandle
    {
        private int _backgroundColorOptionSelectedIndex;
        private List<string> _backgroundOpacityOptions;
        private int _backgroundOpacityOptionSelectedIndex;
        private List<string> _edgeOptions;
        private int _edgeOptionSelectedIndex;
        private IEventAggregator _eventAggregator;
        private List<CaptionsOverrideFonts> _fontOptions;
        private int _fontOptionSelectedIndex;
        private bool _initComplete;
        private int _languageOpotionSelectedIndex;
        private List<string> _languageOptions;
        private List<string> _opacityOptions;
        private ISettingsService _settingsService;
        private List<int> _sizeOptions;
        private int _sizeOptionSelectedIndex;
        private int _textColorOptionSelectedIndex;
        private int _textOpacityOptionSelectedIndex;
        private int _windowColorOptionSelectedIndex;
        private List<string> _windowOpacityOptions;
        private int _windowOpacityOptionSelectedIndex;

        public CaptionSettingsViewModel(IEventAggregator eventAggregator, ISettingsService settingsService)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this._settingsService = settingsService;
                this._eventAggregator = eventAggregator;
                eventAggregator.Subscribe(this);
            }
        }

        public void Handle(SettingsChangedMessage message)
        {
        }

        private void Init()
        {
            this.WindowOpacityOptions = new List<string> { "Default Opacity", "Opaque", "Semitransparent", "Transparent" };
            this.OpacityOptions = new List<string> { "Default Opacity", "Opaque", "Semitransparent" };
            this.BackgroundOpacityOptions = new List<string> { "Default Opacity", "Opaque", "Semitransparent", "Transparent" };
            this.FontOptions = new List<CaptionsOverrideFonts> { 0, 1, 2, 3, 4, 5, 6, 7 };
            this.EdgeOptions = new List<string> { "Default", "No Attribute", "Raised", "Depressed", "Uniform", "Drop shadow" };
            this.SizeOptions = new List<int> { 0.ToPercentage(), 1.ToPercentage(), 2.ToPercentage(), 3.ToPercentage(), 4.ToPercentage(), 5.ToPercentage(), 6.ToPercentage() };
            this.LanguageOptions = new List<string> { "English", "French" };
            this.TextColorOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterColor;
            this.SizeOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterSize;
            this.TextOpacityOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterOpacity;
            this.FontOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterFont;
            this.EdgeOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterEdgeAttribute;
            this.BackgroundColorOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterBackgroundColor;
            this.BackgroundOpacityOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.CharacterBackgroundOpacity;
            this.WindowColorOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.WindowColor;
            this.WindowOpacityOptionSelectedIndex = (int) this._settingsService.CaptionsOverrideSettings.WindowOpacity;
            this._initComplete = true;
        }

        public IEnumerable<IResult> Loaded()
        {
            return new <Loaded>d__7(-2) { <>4__this = this };
        }

        public void RestoreDefaultValues()
        {
            this.TextColorOptionSelectedIndex = 0;
            this.SizeOptionSelectedIndex = 2;
            this.TextOpacityOptionSelectedIndex = 0;
            this.FontOptionSelectedIndex = 0;
            this.EdgeOptionSelectedIndex = 0;
            this.BackgroundColorOptionSelectedIndex = 0;
            this.BackgroundOpacityOptionSelectedIndex = 0;
            this.WindowColorOptionSelectedIndex = 0;
            this.WindowOpacityOptionSelectedIndex = 0;
        }

        private void SaveSettings()
        {
            if (this._initComplete)
            {
                CaptionsOverrideSettings settings = new CaptionsOverrideSettings {
                    WindowColor = (CaptionsOverrideColors) this.WindowColorOptionSelectedIndex,
                    WindowOpacity = (CaptionsOverrideOpacities) this.WindowOpacityOptionSelectedIndex,
                    CharacterColor = (CaptionsOverrideColors) this._textColorOptionSelectedIndex,
                    CharacterOpacity = (CaptionsOverrideOpacities) this._textOpacityOptionSelectedIndex,
                    CharacterBackgroundColor = (CaptionsOverrideColors) this._backgroundColorOptionSelectedIndex,
                    CharacterBackgroundOpacity = (CaptionsOverrideOpacities) this._backgroundOpacityOptionSelectedIndex,
                    CharacterFont = (CaptionsOverrideFonts) this._fontOptionSelectedIndex,
                    CharacterEdgeAttribute = (CaptionsOverrideCharacterEdges) this._edgeOptionSelectedIndex,
                    CharacterSize = (CaptionsOverrideSizes) this._sizeOptionSelectedIndex
                };
                this._settingsService.CaptionsOverrideSettings = settings;
            }
        }

        public int BackgroundColorOptionSelectedIndex
        {
            get
            {
                return this._backgroundColorOptionSelectedIndex;
            }
            set
            {
                this._backgroundColorOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_BackgroundColorOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public List<string> BackgroundOpacityOptions
        {
            get
            {
                return this._backgroundOpacityOptions;
            }
            set
            {
                this._backgroundOpacityOptions = value;
                this.NotifyOfPropertyChange<List<string>>(Expression.Lambda<System.Func<List<string>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_BackgroundOpacityOptions)), new ParameterExpression[0]));
            }
        }

        public int BackgroundOpacityOptionSelectedIndex
        {
            get
            {
                return this._backgroundOpacityOptionSelectedIndex;
            }
            set
            {
                this._backgroundOpacityOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_BackgroundOpacityOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public List<string> EdgeOptions
        {
            get
            {
                return this._edgeOptions;
            }
            set
            {
                this._edgeOptions = value;
                this.NotifyOfPropertyChange<List<string>>(Expression.Lambda<System.Func<List<string>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_EdgeOptions)), new ParameterExpression[0]));
            }
        }

        public int EdgeOptionSelectedIndex
        {
            get
            {
                return this._edgeOptionSelectedIndex;
            }
            set
            {
                this._edgeOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_EdgeOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public List<CaptionsOverrideFonts> FontOptions
        {
            get
            {
                return this._fontOptions;
            }
            set
            {
                this._fontOptions = value;
                this.NotifyOfPropertyChange<List<CaptionsOverrideFonts>>(Expression.Lambda<System.Func<List<CaptionsOverrideFonts>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_FontOptions)), new ParameterExpression[0]));
            }
        }

        public int FontOptionSelectedIndex
        {
            get
            {
                return this._fontOptionSelectedIndex;
            }
            set
            {
                this._fontOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_FontOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public int LanaguageOptionSelectedIndex
        {
            get
            {
                return this._languageOpotionSelectedIndex;
            }
            set
            {
                this._languageOpotionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Field(Expression.Constant(this, typeof(CaptionSettingsViewModel)), fieldof(CaptionSettingsViewModel._languageOpotionSelectedIndex)), new ParameterExpression[0]));
            }
        }

        public List<string> LanguageOptions
        {
            get
            {
                return this._languageOptions;
            }
            set
            {
                this._languageOptions = value;
                this.NotifyOfPropertyChange<List<string>>(Expression.Lambda<System.Func<List<string>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_LanguageOptions)), new ParameterExpression[0]));
            }
        }

        public List<string> OpacityOptions
        {
            get
            {
                return this._opacityOptions;
            }
            set
            {
                this._opacityOptions = value;
                this.NotifyOfPropertyChange<List<string>>(Expression.Lambda<System.Func<List<string>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_OpacityOptions)), new ParameterExpression[0]));
            }
        }

        public List<int> SizeOptions
        {
            get
            {
                return this._sizeOptions;
            }
            set
            {
                this._sizeOptions = value;
                this.NotifyOfPropertyChange<List<int>>(Expression.Lambda<System.Func<List<int>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_SizeOptions)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public int SizeOptionSelectedIndex
        {
            get
            {
                return this._sizeOptionSelectedIndex;
            }
            set
            {
                this._sizeOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_SizeOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public int TextColorOptionSelectedIndex
        {
            get
            {
                return this._textColorOptionSelectedIndex;
            }
            set
            {
                this._textColorOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_TextColorOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public int TextOpacityOptionSelectedIndex
        {
            get
            {
                return this._textOpacityOptionSelectedIndex;
            }
            set
            {
                this._textOpacityOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_TextOpacityOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public int WindowColorOptionSelectedIndex
        {
            get
            {
                return this._windowColorOptionSelectedIndex;
            }
            set
            {
                this._windowColorOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_WindowColorOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        public List<string> WindowOpacityOptions
        {
            get
            {
                return this._windowOpacityOptions;
            }
            set
            {
                this._windowOpacityOptions = value;
                this.NotifyOfPropertyChange<List<string>>(Expression.Lambda<System.Func<List<string>>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_WindowOpacityOptions)), new ParameterExpression[0]));
            }
        }

        public int WindowOpacityOptionSelectedIndex
        {
            get
            {
                return this._windowOpacityOptionSelectedIndex;
            }
            set
            {
                this._windowOpacityOptionSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(CaptionSettingsViewModel)), (MethodInfo) methodof(CaptionSettingsViewModel.get_WindowOpacityOptionSelectedIndex)), new ParameterExpression[0]));
                this.SaveSettings();
            }
        }

        [CompilerGenerated]
        private sealed class <Loaded>d__7 : IEnumerable<IResult>, IEnumerable, IEnumerator<IResult>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private IResult <>2__current;
            public CaptionSettingsViewModel <>4__this;
            private int <>l__initialThreadId;

            [DebuggerHidden]
            public <Loaded>d__7(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                if (this.<>1__state == 0)
                {
                    this.<>1__state = -1;
                    Task.Factory.StartNew(new System.Action(this.<>4__this.Init));
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<IResult> IEnumerable<IResult>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return this;
                }
                return new CaptionSettingsViewModel.<Loaded>d__7(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Caliburn.Micro.IResult>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            IResult IEnumerator<IResult>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }
    }
}

