using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.DomainModel.Framework.Services;
using APD.Client.Framework;

namespace APD.Client.Widget.Admin.Controllers
{
    public class CredentialsCheckerController
    {
        private ProviderConfigItemViewModel viewModel;
        private ICheckIfCredentialsIsValid authChecker;
        private IInvokeBackgroundWorker<bool> backgroundWorker;
        private const string USERNAME_PROPERTY = "Username";
        private const string PASSWORD_PROPERTY = "Password";
        private const string URL_PROPERTY = "URL";

        public CredentialsCheckerController(ProviderConfigItemViewModel viewModel, 
            ICheckIfCredentialsIsValid authChecker,
            IInvokeBackgroundWorker<bool> backgroundWorker)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            if (authChecker == null)
                throw new ArgumentNullException("authChecker");

            if (backgroundWorker == null)
                throw new ArgumentNullException("backgroundWorker");

            this.viewModel = viewModel;
            this.authChecker = authChecker;
            this.backgroundWorker = backgroundWorker;

            this.viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(viewModel_PropertyChanged);
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(USERNAME_PROPERTY) ||
                e.PropertyName.Equals(PASSWORD_PROPERTY) ||
                e.PropertyName.Equals(URL_PROPERTY))
            {
                if (viewModel.URL != null && viewModel.URL != string.Empty &&
                    viewModel.Username != null && viewModel.Username != string.Empty &&
                    viewModel.Password != null)
                {
                    backgroundWorker.RunAsyncVoid(() =>
                    {
                        PrintWorkInProgress();
                        if (!authChecker.Check(viewModel.SelectedProvider, viewModel.URL, viewModel.Username, viewModel.Password))
                            PrintAuthenticationFailed();
                        else
                            PrintAuthenticationSuccessful();
                        
                    });
                }
            }
        }

        private void PrintWorkInProgress()
        {
            viewModel.Status = "Authenticating...";
        }

        private void PrintAuthenticationFailed()
        {
            viewModel.Status = "Authentication failed";
        }

        private void PrintAuthenticationSuccessful()
        {
            viewModel.Status = "Authentication successful";
   
        }
    }
}
