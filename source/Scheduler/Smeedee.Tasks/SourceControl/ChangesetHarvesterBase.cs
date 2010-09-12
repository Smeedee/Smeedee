using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Tasks.Framework;

namespace Smeedee.Tasks.SourceControl
{
    public abstract class ChangesetHarvesterBase : TaskBase
    {
        protected const int FIRST_CHANGESET_REVISION_ID = 0;

        protected const string URL_SETTING_NAME = "Url";
        protected const string USERNAME_SETTING_NAME = "Username";
        protected const string PASSWORD_SETTING_NAME = "Password";

        protected IRepository<Changeset> changesetDbRepository;
        protected IPersistDomainModels<Changeset> databasePersister;

        protected ChangesetHarvesterBase(IRepository<Changeset> changesetDbRepository,
                                         IPersistDomainModels<Changeset> databasePersister)
        {
            this.changesetDbRepository = changesetDbRepository;
            this.databasePersister = databasePersister;
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
            var unsavedChangesets = GetUnsavedChangesets(changesetRepository);
            databasePersister.Save(unsavedChangesets);
        }
    }
}