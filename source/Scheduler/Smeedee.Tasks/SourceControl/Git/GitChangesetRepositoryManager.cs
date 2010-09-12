using System;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.VCS.Git.DomainModel.Repositories;

namespace Smeedee.Tasks.SourceControl.Git
{
    public class GitChangesetRepositoryManager : IManageGitChangesetRepositories
    {
        public IRepository<Changeset> GetRepository(string path)
        {
            return new GitChangesetRepository(path);
        }
    }
}