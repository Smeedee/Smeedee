using System;
using System.Linq;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.VCS.SVN.DomainModel.Repositories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.SourceControl
{
    [Task("SVN Changeset Harvester",
        Author = "Smeedee Team",
        Description = "Retrieves information from a Subversion version control repository. Used to populate Smeedee's database with information about the latest commits and other commit statistics.",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(string), "")]
    public class SVNChangesetHarvesterTask : ChangesetHarvesterBase
    {
        private readonly TaskConfiguration config;
        public override string Name { get { return "SVN Changeset Harvester"; } }

        public SVNChangesetHarvesterTask(IRepository<Changeset> changesetDbRepository,
                                         IPersistDomainModels<Changeset> databasePersister,
                                         TaskConfiguration config)
            : base(changesetDbRepository, databasePersister)
        {
            Guard.ThrowIfNull<ArgumentNullException>(changesetDbRepository, databasePersister, config);
            Guard.Requires<TaskConfigurationException>(config.Entries.Count() >= 3);
            this.config = config;
            Interval = TimeSpan.FromMilliseconds(config.DispatchInterval);
        }

        public override void Execute()
        {
            SaveUnsavedChangesets(GetChangesetRepository());
        }

        private SVNChangesetRepository GetChangesetRepository()
        {
            return new SVNChangesetRepository(
                (string)config.ReadEntryValue(URL_SETTING_NAME),
                (string)config.ReadEntryValue(USERNAME_SETTING_NAME),
                (string)config.ReadEntryValue(PASSWORD_SETTING_NAME)
                );
        }
    }
}
