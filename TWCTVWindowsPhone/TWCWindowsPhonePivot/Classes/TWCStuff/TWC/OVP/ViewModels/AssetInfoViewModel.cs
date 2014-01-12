namespace TWC.OVP.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using TWC.OVP.Framework.ViewModels;

    public class AssetInfoViewModel : BaseViewModel
    {
        private AssetViewModel _assetViewModel;
        private string _currentLoadState = VisualStates.LoadingState;

        public void ClearAsset()
        {
            this.CurrentLoadState = VisualStates.LoadingState;
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentLoadState;
        }

        public void LoadAsset(AssetViewModel asset)
        {
            this.Asset = asset;
            this.CurrentLoadState = VisualStates.LoadedState;
        }

        public AssetViewModel Asset
        {
            get
            {
                return this._assetViewModel;
            }
            set
            {
                this._assetViewModel = value;
                this.NotifyOfPropertyChange<AssetViewModel>(Expression.Lambda<System.Func<AssetViewModel>>(Expression.Property(Expression.Constant(this, typeof(AssetInfoViewModel)), (MethodInfo) methodof(AssetInfoViewModel.get_Asset)), new ParameterExpression[0]));
            }
        }

        public string CurrentLoadState
        {
            get
            {
                return this._currentLoadState;
            }
            set
            {
                this._currentLoadState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(Expression.Lambda<System.Func<IEnumerable<string>>>(Expression.Property(Expression.Constant(this, typeof(AssetInfoViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }


        public static class VisualStates
        {
            public static readonly string LoadedState = "LoadedState";
            public static readonly string LoadingState = "Loading";
        }
    }
}

