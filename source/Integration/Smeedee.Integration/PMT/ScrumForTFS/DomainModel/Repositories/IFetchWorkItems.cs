using System;
using System.Collections.Generic;

using Smeedee.DomainModel.ProjectInfo;


namespace Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public interface IFetchWorkItems {
        List<Task> GetAllWorkEffort();
        IEnumerable<String> GetAllIterations();
        List<Task> GetAllWorkEffortInSprint(string iterationPath);
    }
}