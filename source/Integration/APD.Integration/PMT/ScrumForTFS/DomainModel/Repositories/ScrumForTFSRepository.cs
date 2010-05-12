using System;
using System.Collections.Generic;
using System.Net;

using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public class ScrumForTFSRepository : IRepository<ProjectInfoServer>
    {
        private readonly IFetchWorkItems fetcher;
        private readonly string serverAddress;
        private readonly string projectName;
        

        public ScrumForTFSRepository(string server, string project, string username, string password, Dictionary<String, String> config)
        {
            serverAddress = server;
            projectName = project;
            var credentials = new NetworkCredential(username, password);
            fetcher = new WorkItemFetcher(server, project, credentials, config);
        }


        public ScrumForTFSRepository(IFetchWorkItems workItemFetcher)
        {
            fetcher = workItemFetcher;
        }


        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            var server = new ProjectInfoServer(serverAddress + " - " + projectName, serverAddress);
            var project = new Project
                              {
                                  Name = projectName,
                                  SystemId = projectName
                              };

            AddIterations(project);
            server.AddProject(project);

            var results = new List<ProjectInfoServer> { server };

            return results;
        }


        private void AddIterations(Project project)
        {
            foreach (var sprint in fetcher.GetAllSprints())
            {
                var iteration = new Iteration
                {
                    Name = sprint.Value,
                    SystemId = sprint.Key.ToString(),
                    StartDate = fetcher.GetStartDateForIteration(sprint.Key),
                    EndDate = fetcher.GetEndDateForIteration(sprint.Key)
                };

                foreach (var task in fetcher.GetAllWorkEffortInSprint(sprint.Value))
                {
                    iteration.AddTask(task);
                }

                project.AddIteration(iteration);
            }
        }
    }
}
