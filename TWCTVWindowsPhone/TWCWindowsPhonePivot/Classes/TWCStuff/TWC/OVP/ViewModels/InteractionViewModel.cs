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
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;

    public class InteractionViewModel : BaseViewModel, IHandle<ErrorMessage>, IHandle
    {
        private AssetViewerViewModel _AssetViewer;
        private string _currentErrorDetailState = VisualStates.NoErrorDetailState;
        private string _currentErrorState = VisualStates.ErrorMessageNotShown;
        private string _errorMessageDetailText;
        private string _errorMessageText;
        private string _errorMessageTitle;
        private IEventAggregator _eventAggregator;

        public event EventHandler ErrorStateChanged;

        public InteractionViewModel(AssetViewerViewModel assetViewerViewModel, IEventAggregator eventAggregator)
        {
            this._AssetViewer = assetViewerViewModel;
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        public void DismissErrorMessage()
        {
            this.CurrentErrorState = VisualStates.ErrorMessageNotShown;
            if (this.CurrentErrorMessage != null)
            {
                this.CurrentErrorMessage.Dismissed();
            }
            this.ErrorMessageText = null;
            this.ErrorMessageDetailText = null;
            this.CurrentErrorMessage = null;
        }

        public void DismissPlaybackErrorMessage()
        {
            if ((this.CurrentErrorMessage != null) && (this.CurrentErrorMessage.MessageType != ErrorMessageType.Message))
            {
                this.DismissErrorMessage();
            }
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentErrorState;
            yield return this.CurrentErrorDetailState;
        }

        public void Handle(ErrorMessage message)
        {
            if ((this.CurrentErrorMessage == null) || ((this.CurrentErrorMessage != null) && (message.Priority >= this.CurrentErrorMessage.Priority)))
            {
                bool flag = false;
                if (((this.ErrorMessageTitle != message.Title) || (this.ErrorMessageDetailText != message.Detail)) || (this.ErrorMessageText != message.Message))
                {
                    flag = true;
                }
                if (this._AssetViewer.IsRetrying || (message.MessageType != ErrorMessageType.PlayerbackError))
                {
                    this.CurrentErrorMessage = message;
                    this.ErrorMessageTitle = message.Title;
                    this.ErrorMessageDetailText = message.Detail;
                    this.ErrorMessageText = message.Message;
                    this.CurrentErrorState = VisualStates.ErrorMessageShown;
                    if (flag)
                    {
                        this._eventAggregator.Publish(new EGUserMessageDisplayed(this.ErrorMessageTitle + " " + this.ErrorMessageText, "modalPopup"));
                    }
                }
            }
        }

        protected virtual void OnErrorStateChanged()
        {
            if (this.ErrorStateChanged != null)
            {
                this.ErrorStateChanged(this, null);
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        public string CurrentErrorDetailState
        {
            get
            {
                return this._currentErrorDetailState;
            }
            set
            {
                this._currentErrorDetailState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(InteractionViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public ErrorMessage CurrentErrorMessage { get; set; }

        public string CurrentErrorState
        {
            get
            {
                return this._currentErrorState;
            }
            set
            {
                this._currentErrorState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(InteractionViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
                this.OnErrorStateChanged();
            }
        }

        public string ErrorMessageDetailText
        {
            get
            {
                return this._errorMessageDetailText;
            }
            set
            {
                this._errorMessageDetailText = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(InteractionViewModel)), (MethodInfo) methodof(InteractionViewModel.get_ErrorMessageDetailText)), new ParameterExpression[0]));
            }
        }

        public string ErrorMessageText
        {
            get
            {
                return this._errorMessageText;
            }
            set
            {
                this._errorMessageText = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(InteractionViewModel)), (MethodInfo) methodof(InteractionViewModel.get_ErrorMessageText)), new ParameterExpression[0]));
            }
        }

        public string ErrorMessageTitle
        {
            get
            {
                return this._errorMessageTitle;
            }
            set
            {
                this._errorMessageTitle = value;
                this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(InteractionViewModel)), (MethodInfo) methodof(InteractionViewModel.get_ErrorMessageTitle)), new ParameterExpression[0]));
            }
        }

        public bool IsShowingErrorMessage
        {
            get
            {
                return (this.CurrentErrorState == VisualStates.ErrorMessageShown);
            }
        }


        public static class VisualStates
        {
            public static readonly string ErrorDetailState = "ErrorMessageDetailEnabled";
            public static readonly string ErrorMessageNotShown = "ErrorMessageNotShown";
            public static readonly string ErrorMessageShown = "ErrorMessageShown";
            public static readonly string NoErrorDetailState = "ErrorMessageDetailDisabled";
        }
    }
}

