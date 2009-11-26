using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using System.Collections.ObjectModel;
using Config = APD.DomainModel.Config;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public class ProviderConfigItemViewModel : ConfigurationItemViewModel
    {
        private const string TFS_PROVIDER_NAME = "tfs";
        private Config.Configuration configuration;

        public string Username
        {
            get { return configuration.GetSetting("username").Value; }
            set
            {
                configuration.NewSetting("username", value);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.Username);
            }
        }

        public string Password
        {
            get { return configuration.GetSetting("password").Value; }
            set
            {
                configuration.NewSetting("password", value);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.Password);
            }
        }

        public string URL
        {
            get { return configuration.GetSetting("url").Value; }
            set
            {
                configuration.NewSetting("url", value);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.URL);
            }
        }

        public string SelectedProvider
        {
            get { return configuration.GetSetting("provider").Value; }
            set
            {
                configuration.NewSetting("provider", value);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.SelectedProvider);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.IsProjectEditable);

                if ( value == "tfs" && this.Project.StartsWith("$/") == false)
                    this.Project = "$/" + this.Project;
            }
        }

        public bool IsProjectEditable
        {
            get
            {
                return ( SelectedProvider == TFS_PROVIDER_NAME );
            }
        }

        public string Project
        {
            set
            {
                configuration.NewSetting("project", value);
                TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.Project);
            }
            get { return configuration.GetSetting("project").Value; }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                if( value != status )
                {
                    status = value;
                    TriggerPropertyChanged<ProviderConfigItemViewModel>(vm => vm.Status);
                }
            }
        }

        public ObservableCollection<string> SupportedProviders { get; private set; }

        public ProviderConfigItemViewModel(IInvokeUI uiInvoker, Config.Configuration configuration) :
            base(uiInvoker)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            SupportedProviders = new ObservableCollection<string>();
            foreach (var value in configuration.GetSetting("supported-providers").Vals)
                SupportedProviders.Add(value);
    
            this.configuration = configuration;
        }
    }
}
