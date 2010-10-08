using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Framework;

namespace Smeedee.Tasks.SourceControl
{
    public abstract class ChangesetHarvesterBase : TaskBase
    {
        protected const int FIRST_CHANGESET_REVISION_ID = 0;

        public const string SOURCECONTROL_SERVER_NAME = "ServerName";
        public const string URL_SETTING_NAME = "Url";
        public const string USERNAME_SETTING_NAME = "Username";
        public const string PASSWORD_SETTING_NAME = "Password";

        protected IRepository<Changeset> changesetDbRepository;
        protected IPersistDomainModels<Changeset> databasePersister;
        protected TaskConfiguration config;

        protected ChangesetHarvesterBase(IRepository<Changeset> changesetDbRepository,
                                         IPersistDomainModels<Changeset> databasePersister,
                                        TaskConfiguration config)
        {
            this.changesetDbRepository = changesetDbRepository;
            this.databasePersister = databasePersister;
            this.config = config;
        }

        protected long GetLatestSavedRevisionId()
        {
            IEnumerable<Changeset> allSavedChangesets = changesetDbRepository.Get(new AllChangesetsSpecification());

            long latestSavedRevision = FIRST_CHANGESET_REVISION_ID;
            if (allSavedChangesets.Count() > 0)
            {
                latestSavedRevision = allSavedChangesets.First().Revision;
            }
            return latestSavedRevision;
        }

        protected IEnumerable<Changeset> GetUnsavedChangesets(IRepository<Changeset> changesetRepository)
        {
            var latestSavedRevisionId = GetLatestSavedRevisionId();
            return changesetRepository.Get(
                new ChangesetsAfterRevisionSpecification(latestSavedRevisionId));
        }

        protected void SaveUnsavedChangesets(IRepository<Changeset> changesetRepository)
        {
            var sourceChangesetServer = BuildChangesetServerFromConfig();

            var unsavedChangesets = GetUnsavedChangesets(changesetRepository);
            foreach (var unsavedChangeset in unsavedChangesets)
            {
                unsavedChangeset.Server = sourceChangesetServer;
            }

            databasePersister.Save(unsavedChangesets);
        }

        private ChangesetServer BuildChangesetServerFromConfig()
        {
            var serverUrl = config.ReadEntryValue(URL_SETTING_NAME) as string;
            var serverName = config.ReadEntryValue(SOURCECONTROL_SERVER_NAME) as string;
            return new ChangesetServer(serverUrl, serverName);
        }
    }
}