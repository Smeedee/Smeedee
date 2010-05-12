using System;
using System.Collections.Generic;

using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public interface IFetchWorkItems {
        List<Task> GetAllWorkEffort();
        Dictionary<int, String> GetAllSprints();
        List<Task> GetAllWorkEffortInSprint(string iterationPath);
        DateTime GetStartDateForIteration(int iterationId);
        DateTime GetEndDateForIteration(int iterationId);
    }
}