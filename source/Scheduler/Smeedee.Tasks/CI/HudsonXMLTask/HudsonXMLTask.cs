using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.CI.HudsonXMLTask
{
    [Task("Hudson Continuous Integration Task",
          Author = "Christian Jonassen and Nicolai Meltveit",
          Description = "Retrieves information from Hudson continuous integration server. Used to populate Smeedee's database with build status information.",
          Version = 1,
          Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(Uri), "")]
    public class HudsonXMLTask : CiTaskBase
    {
        private TaskConfiguration _configuration;

        public HudsonXMLTask(IPersistDomainModels<CIServer> databasePersister, TaskConfiguration configuration) : base(databasePersister, configuration)
        {
            Guard.ThrowIfNull<ArgumentNullException>(databasePersister, configuration);
            Guard.Requires<TaskConfigurationException>(configuration.Entries.Count() >= 3);

            _configuration = configuration;
            Interval = TimeSpan.FromMilliseconds(_configuration.DispatchInterval);
        }

        public override string Name
        {
            get { return "Hudson Continuous Integration Harvester"; }
        }

        public override void Execute()
        {  
            PersistCiServers(GetAllHudsonServers());
        }

        private IRepository<CIServer> GetAllHudsonServers()
        {
            var serverUrl = (string)_configuration.ReadEntryValue(URL_SETTING_NAME);
            var credentials = new NetworkCredential()
                                    {
                                        UserName = (string)_configuration.ReadEntryValue(USERNAME_SETTING_NAME),
                                        Password = (string)_configuration.ReadEntryValue(PASSWORD_SETTING_NAME)
                                    };

            return new HudsonCiServerRepository(serverUrl, credentials);
        }
    }
}
