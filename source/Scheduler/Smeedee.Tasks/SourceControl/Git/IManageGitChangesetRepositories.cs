using System;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;

namespace Smeedee.Tasks.SourceControl.Git
{
    public interface IManageGitChangesetRepositories
    {
        IRepository<Changeset> GetRepository(string path);
    }
}