using System;
using System.Linq;
using System.Net;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.CI.TFSBuild.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.CI.TeamFoundationServerCITask
{
    [Task("TFS Continuous Integration Task",
       Author = "Smeedee Team",
       Description = "Retrieves information from Team Foundation's build functionality. Used to populate Smeedee's database with build status information.",
       Version = 1,
       Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(Uri), "")]
    [TaskSetting(4, PROJECT_SETTING_NAME, typeof(string), "")]
    public class TeamFoundationServerCITask : CiTaskBase
    {
        private readonly TaskConfiguration _configuration;

        public TeamFoundationServerCITask(IPersistDomainModels<CIServer> databasePersister, TaskConfiguration configuration)
            : base(databasePersister, configuration)
        {
            Guard.Requires<ArgumentNullException>(databasePersister != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<TaskConfigurationException>(configuration.Entries.Count() >= 4);

            _configuration = configuration;
            Interval = TimeSpan.FromMilliseconds(_configuration.DispatchInterval);
        }

        public override string Name
        {
            get { return "TFS Continuous Integration Harvester"; }
        }

        public override void Execute()
        {
            PersistCiServers(GetTfsciServerRepository());
        }

        private IRepository<CIServer> GetTfsciServerRepository()
        {
            var serverUrl = (string)_configuration.ReadEntryValue(URL_SETTING_NAME);

            return new TFSCIServerRepository(
                serverUrl,
                (string)_configuration.ReadEntryValue(PROJECT_SETTING_NAME),
                new NetworkCredential()
                {
                    UserName = (string)_configuration.ReadEntryValue(USERNAME_SETTING_NAME),
                    Password = (string) _configuration.ReadEntryValue(PASSWORD_SETTING_NAME)
                });
        }
    }
}
