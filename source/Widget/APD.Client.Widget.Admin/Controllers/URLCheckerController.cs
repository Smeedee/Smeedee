using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.DomainModel.Framework.Services;
using APD.Client.Framework;


namespace APD.Client.Widget.Admin.Controllers
{
    public class URLCheckerController
    {
        private ICheckIfResourceExists urlChecker;
        private IInvokeBackgroundWorker<bool> backgroundWorker;
        private ProviderConfigItemViewModel viewModel;
        private const string URL_PROPERTY = "URL";

        public URLCheckerController(ProviderConfigItemViewModel viewModel,
            ICheckIfResourceExists urlChecker,
            IInvokeBackgroundWorker<bool> backgroundWorker)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            if (urlChecker == null)
                throw new ArgumentNullException("urlChecker");

            if (backgroundWorker == null)
                throw new ArgumentNullException("backgroundWorker");

            this.viewModel = viewModel;
            this.urlChecker = urlChecker;
            this.backgroundWorker = backgroundWorker;
           

            this.viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(viewModel_PropertyChanged);
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(URL_PROPERTY))
            {
                backgroundWorker.RunAsyncVoid(() =>
                {
                    PrintWorkInProgress();
                    if (!urlChecker.Check(viewModel.URL))
                        PrintInvalidURL();
                    else
                        PrintValidURL();
                    
                });
            }
        }

        private void PrintWorkInProgress()
        {
            viewModel.Status = "Checking URL...";
        }

        private void PrintInvalidURL()
        {
            viewModel.Status = "Invalid URL";
        }

        private void PrintValidURL()
        {
            viewModel.Status = "URL is ok";
        }
    }
}
