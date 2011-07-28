using System;
using System.Linq;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.MobileServices.ViewModels
{
    public class MobileServicesAuthenticationViewModel : ViewModelBase
    {
        private const string MOBILE_SERVICES_CONFIGURATION_KEY = "MobileServices";
        private const string MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY = "ApiKey";

        private const string LEGAL_KEY_ITEMS = "abcdefgijkmnopqrstwxyzABCDEFGHJKLMNPQRSTWXYZ123456789";

        private const string GENERATING_NEW_KEY_MESSAGE = "Generating new key. Please Wait...";
        private const string NEW_KEY_GENERATED_MESSAGE = "New key generated. Loading...";
        private const string PLEASE_WAIT_MESSAGE = "Please wait...";

        private readonly IAsyncRepository<Configuration> repo;
        private readonly IPersistDomainModelsAsync<Configuration> persister;
        private readonly IUIInvoker uiInvoker;
        private Configuration configuration;

        private bool isSaving;
        private bool isLoading;


        public DelegateCommand GenerateNewKey { get; private set; }


        public MobileServicesAuthenticationViewModel(
            IAsyncRepository<Configuration> settingsRepository,
            IPersistDomainModelsAsync<Configuration> settingsPersister,
            IUIInvoker uiInvoker)
        {
            this.uiInvoker = uiInvoker;
            repo = settingsRepository;
            persister = settingsPersister;


            AddDelegateCommands();
            AddEventHandlers();
            RetrieveCurrentKey();
        }

        private void AddDelegateCommands()
        {
            GenerateNewKey = new DelegateCommand(CreateNewKey);
        }

        private void AddEventHandlers()
        {
            repo.GetCompleted += settingsRepository_GetCompleted;
            persister.SaveCompleted += persister_SaveCompleted;
        }

        private void RetrieveCurrentKey()
        {
            uiInvoker.Invoke(() => ApiKey = PLEASE_WAIT_MESSAGE);
            if (!isLoading)
            {
                repo.BeginGet(new ConfigurationByName(MOBILE_SERVICES_CONFIGURATION_KEY));
                isLoading = true;
            }
            
        }

        void persister_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            isSaving = false;
            
            uiInvoker.Invoke(() => ApiKey = NEW_KEY_GENERATED_MESSAGE);
            if (!isLoading)
            {
                repo.BeginGet(new ConfigurationByName(MOBILE_SERVICES_CONFIGURATION_KEY));
                isLoading = true;
            }
        }

        void settingsRepository_GetCompleted(object sender, GetCompletedEventArgs<Configuration> e)
        {
            isLoading = false;

            if (e.Result.Count() > 0)
            {
                configuration = e.Result.First();
                if (configuration.ContainsSetting(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY))
                    uiInvoker.Invoke(() => ApiKey = configuration.GetSetting(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY).Value);
                else
                    CreateNewKey();
            }
            else
            {
                CreateNewKey();
            }
        }

        private void CreateNewKey()
        {
            uiInvoker.Invoke(() => ApiKey = GENERATING_NEW_KEY_MESSAGE);
            SaveNewConfigWithRandomKey();
        }

        private void SaveNewConfigWithRandomKey()
        {
            if (configuration == null)
                configuration = new Configuration(MOBILE_SERVICES_CONFIGURATION_KEY);
            configuration.IsConfigured = true;

            if (configuration.ContainsSetting(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY))
                configuration.ChangeSetting(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY, GenerateRandomKeyString());
            else
                configuration.NewSetting(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY, GenerateRandomKeyString());

            if (!isSaving)
            {
                persister.Save(configuration);
                isSaving = true;
            }
            
        }

        private static string GenerateRandomKeyString()
        {
            var randomString = "";
            var randomGenerator = new Random();
            for (int i = 0; i < 8; i++)
            {
                var randomPosition = randomGenerator.Next(0, LEGAL_KEY_ITEMS.Count());
                randomString += LEGAL_KEY_ITEMS[randomPosition];
            }
            return randomString;
        }

        private string _apiKey;
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (value == _apiKey) return;
                _apiKey = value;
                TriggerPropertyChanged(MOBILE_SERVICES_API_KEY_CONFIGURATION_KEY);
            }
        }
    }
}
