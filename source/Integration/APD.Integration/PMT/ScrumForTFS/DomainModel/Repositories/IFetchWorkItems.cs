using System;
using System.Collections.Generic;

using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public interface IFetchWorkItems {
        List<Task> GetAllWorkEffort();
        IEnumerable<String> GetAllIterations();
        List<Task> GetAllWorkEffortInSprint(string iterationPath);
    }
}