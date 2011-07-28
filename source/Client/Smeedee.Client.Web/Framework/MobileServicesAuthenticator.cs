using System.Linq;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Framework
{
    public class MobileServicesAuthenticator
    {
        private const string API_KEY_SETTINGS_KEY = "ApiKey";
        private const string MOBILE_SERVICES_CONFIGURATION_KEY = "MobileServices";

        private readonly IRepository<Configuration> configRepo;

        public MobileServicesAuthenticator()
        {
            configRepo = new ConfigurationDatabaseRepository();
        }

        public bool IsApiKeyValid(string apiKey)
        {
            var config = configRepo.Get(new ConfigurationByName(MOBILE_SERVICES_CONFIGURATION_KEY));
            if (config != null && config.Count() > 0)
            {
                if (config.First().ContainsSetting(API_KEY_SETTINGS_KEY))
                {
                    return apiKey == config.First().GetSetting(API_KEY_SETTINGS_KEY).Value;
                }
            }
            return false;
        }
    }
}