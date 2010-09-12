using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.VCS.TFSVC.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.SourceControl
{
    [Task("TFS Changeset Harvester",
       Author = "Smeedee Team",
       Description = "Retrieves information from a Team Foundation version control repository. Used to populate Smeedee's database with information about the latest commits and other commit statistics.",
       Version = 1,
       Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(string), "")]
    [TaskSetting(4, PROJECT_SETTING_NAME, typeof(string), "")]
    public class TFSChangesetHarvesterTask : ChangesetHarvesterBase
    {
        private readonly TaskConfiguration config;
        protected const string PROJECT_SETTING_NAME = "Project";
        public override string Name { get { return "TFS Changeset Harvester"; } }

        public TFSChangesetHarvesterTask(IRepository<Changeset> changesetDbRepository, IPersistDomainModels<Changeset> databasePersister, TaskConfiguration config) 
            : base(changesetDbRepository, databasePersister)
        {
            Guard.ThrowIfNull<ArgumentNullException>(changesetDbRepository, databasePersister, config);
            Guard.Requires<TaskConfigurationException>(config.Entries.Count() >= 4);
            this.config = config;
            Interval = TimeSpan.FromMilliseconds(config.DispatchInterval);
        }

        public override void Execute()
        {
            SaveUnsavedChangesets(GetChangesetRepository());
        }

        private TFSChangesetRepository GetChangesetRepository()
        {
            return new TFSChangesetRepository(
                   (string)config.ReadEntryValue(URL_SETTING_NAME),
                   (string)config.ReadEntryValue(PROJECT_SETTING_NAME),
                   new NetworkCredential()
                   {
                       UserName = (string)config.ReadEntryValue(USERNAME_SETTING_NAME),
                       Password = (string)config.ReadEntryValue(PASSWORD_SETTING_NAME)
                   });
        }
    }
}
