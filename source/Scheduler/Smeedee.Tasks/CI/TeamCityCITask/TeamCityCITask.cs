using System;
using System.Linq;
using System.Net;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.CI.TeamCity.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.CI.TeamCityCITask
{
    [Task("Team City Continuous Integration Task",
          Author = "Smeedee Team",
          Description = "Retrieves information from Jetbrains' TeamCity continuous integration server. Used to populate Smeedee's database with build status information.",
          Version = 1,
          Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(Uri), "")]
    public class TeamCityCiTask : CiTaskBase
    {
        private TaskConfiguration _configuration;

        public TeamCityCiTask(IPersistDomainModels<CIServer> databasePersister, TaskConfiguration configuration) : base(databasePersister, configuration)
        {
            Guard.ThrowIfNull<ArgumentNullException>(databasePersister, configuration);
            Guard.Requires<TaskConfigurationException>(configuration.Entries.Count() >= 3);

            _configuration = configuration;
            Interval = TimeSpan.FromMilliseconds(_configuration.DispatchInterval);
        }

        public override string Name
        {
            get { return "Team City Continuous Integration Harvester"; }
        }

        public override void Execute()
        {  
            PersistCiServers(GetAllTeamCityServers());
        }

        private IRepository<CIServer> GetAllTeamCityServers()
        {
            var serverUrl = (string)_configuration.ReadEntryValue(URL_SETTING_NAME);

            var credentials = new NetworkCredential()
                                    {
                                        UserName = (string)_configuration.ReadEntryValue(USERNAME_SETTING_NAME),
                                        Password = (string)_configuration.ReadEntryValue(PASSWORD_SETTING_NAME)
                                    };

            var xmlFetcher = new TeamCityXmlFetcher(serverUrl, credentials);
            var xmlParser = new TeamCityXmlParser(xmlFetcher);

            return new TeamCityCIServerRepository(xmlParser);
        }
    }
}