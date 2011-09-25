#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.ComponentModel;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.Controller
{
    public abstract class ControllerBase<T> where T : AbstractViewModel
    {
        public T ViewModel { get; private set; }
        protected ITimer refreshNotifier;
        protected IUIInvoker uiInvoker;
        protected int REFRESH_INTERVAL = 2 * 60 * 1000;
        protected readonly IProgressbar loadingNotifier;

        protected const string SAVING_DATA_MESSAGE = "Saving data...";
        protected const string LOADING_DATA_MESSAGE = "Loading data from server...";
        protected const string SAVING_CONFIG_MESSAGE = "Saving configuration...";
        protected const string LOADING_CONFIG_MESSAGE = "Loading configuration from server...";

        protected const string LOADING_REAL_CONFIG_MESSAGE =
            "The default configuration is now loaded. We are still trying to load the real configuration";


        public ControllerBase(T viewModel, 
            ITimer timer, 
            IUIInvoker uiInvoker, 
            IProgressbar loadingNotifier)
        {
            Guard.Requires<ArgumentException>(timer != null, "timer");
            Guard.Requires<ArgumentException>(uiInvoker != null, "uiInvoker");
            Guard.Requires<ArgumentException>(viewModel != null, "viewModel");
            Guard.Requires<ArgumentException>(loadingNotifier != null, "loadingNotifyer");
            
            this.refreshNotifier = timer;
            this.uiInvoker = uiInvoker;
            this.loadingNotifier = loadingNotifier;

            ViewModel = viewModel;
            refreshNotifier.Elapsed += OnNotifiedToRefresh;
        }

        protected void ThrowIfNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentException("The" + parameterName + " argument cannot be null");
            }
        }

        protected abstract void OnNotifiedToRefresh(object sender, EventArgs e);


        public void Start()
        {
            refreshNotifier.Start(REFRESH_INTERVAL);
        }

        public void Stop()
        {
            refreshNotifier.Stop();
        }

        public virtual void ToggleRefreshInSettingsMode(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInSettingsMode" && sender is Widget)
            {
                if (((Widget)sender).IsInSettingsMode)
                {
                    Stop();
                } else
                {
                    Start();
                }
            }
        }

        protected void SetIsSavingData()
        {
            uiInvoker.Invoke(() => ViewModel.IsSaving = true);
            loadingNotifier.ShowInView(SAVING_DATA_MESSAGE);
        }

        protected void SetIsNotSavingData()
        {
            uiInvoker.Invoke(() => ViewModel.IsSaving = false);
            loadingNotifier.HideInView();
        }

        protected void SetIsLoadingRealData()
        {
            uiInvoker.Invoke(() => ViewModel.IsLoading = true);
            loadingNotifier.ShowInBothViews(LOADING_REAL_CONFIG_MESSAGE);
        }

        protected void SetIsLoadingData()
        {
            uiInvoker.Invoke(() => ViewModel.IsLoading = true);
            loadingNotifier.ShowInView(LOADING_DATA_MESSAGE);
        }

        protected void SetIsNotLoadingData()
        {
            uiInvoker.Invoke(() => ViewModel.IsLoading = false);
            loadingNotifier.HideInView();
        }

        protected void SetIsSavingConfig()
        {
            uiInvoker.Invoke(() => ViewModel.IsSaving = true);
            loadingNotifier.ShowInSettingsView(SAVING_CONFIG_MESSAGE);
        }

        protected void SetIsNotSavingConfig()
        {
            uiInvoker.Invoke(() => ViewModel.IsSaving = false);
            loadingNotifier.HideInSettingsView();
        }

        protected void SetIsLoadingConfig()
        {
            uiInvoker.Invoke(() => ViewModel.IsLoadingConfig = true);
            loadingNotifier.ShowInBothViews(LOADING_CONFIG_MESSAGE);
        }

        protected void SetIsNotLoadingConfig()
        {
            uiInvoker.Invoke(() => ViewModel.IsLoadingConfig = false);
            loadingNotifier.HideInBothViews();
        }
    }
}