using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.MobileServices.ViewModels
{
    public class MobileServicesAuthenticationViewModel : ViewModelBase
    {
        private const string MOBILE_SERVICES_CONFIGURATION_KEY = "MobileServices";
        private const string GENERATING_NEW_KEY_MESSAGE = "Generating new key. Please Wait...";
        private IAsyncRepository<Configuration> repo;
        private IPersistDomainModelsAsync<Configuration> persister;
        private IProgressbar loadingNotifier;
        private IUIInvoker uiInvoker;
        

        public MobileServicesAuthenticationViewModel(
            IAsyncRepository<Configuration> settingsRepository, 
            IPersistDomainModelsAsync<Configuration> settingsPersister,
            IProgressbar loadingNotifier,
            IUIInvoker uiInvoker)
        {
            this.uiInvoker = uiInvoker;
            repo = settingsRepository;
            persister = settingsPersister;


            GenerateNewKey = new DelegateCommand(CreateNewKey);

            
            settingsRepository.GetCompleted += settingsRepository_GetCompleted;
            persister.SaveCompleted += persister_SaveCompleted;

            uiInvoker.Invoke(() => ApiKey = "Please wait..."); // TODO
           
            settingsRepository.BeginGet(new ConfigurationByName(MOBILE_SERVICES_CONFIGURATION_KEY));
        }

        private void CreateNewKey()
        {
            uiInvoker.Invoke(() => ApiKey = GENERATING_NEW_KEY_MESSAGE);
            SaveNewConfigWithRandomKey();
        }

        void persister_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            uiInvoker.Invoke(() => ApiKey = "New key generated. Loading...");
            repo.BeginGet(new ConfigurationByName(MOBILE_SERVICES_CONFIGURATION_KEY));
        }

        void settingsRepository_GetCompleted(object sender, GetCompletedEventArgs<Configuration> e)
        {
            
            if (e.Result.Count() > 0)
            {
                var apiKeySetting = e.Result.First().GetSetting("ApiKey");
                if (apiKeySetting != null)
                {
                    uiInvoker.Invoke(() => ApiKey  = apiKeySetting.Value);
                }
                else
                {
                    uiInvoker.Invoke(() => ApiKey = GENERATING_NEW_KEY_MESSAGE);
                    e.Result.First().NewSetting("ApiKey", GenerateRandomKey());
                    persister.Save(e.Result.First());
                }
            }
            else
            {
                uiInvoker.Invoke(() => ApiKey = GENERATING_NEW_KEY_MESSAGE);
                SaveNewConfigWithRandomKey();
            }
        }

        private void SaveNewConfigWithRandomKey()
        {
            var passwordConfig = new Configuration(MOBILE_SERVICES_CONFIGURATION_KEY);
            passwordConfig.NewSetting("ApiKey", GenerateRandomKey());
            persister.Save(passwordConfig);
        }

        private string GenerateRandomKey()
        {
            //TODO: Proper keygen
            return "TODOGenerateNiceKey";
        }

        private string _apiKey;
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (value == _apiKey) return;
                _apiKey = value;
                TriggerPropertyChanged("ApiKey");
            }
        }

        public DelegateCommand GenerateNewKey { get; private set; }
    }
}
