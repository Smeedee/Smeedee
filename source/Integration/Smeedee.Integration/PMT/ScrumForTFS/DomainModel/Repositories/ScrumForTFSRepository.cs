using System;
using System.Collections.Generic;
using System.Net;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;


namespace Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories
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

		public ScrumForTFSRepository(string server, string project, Dictionary<string, string> config, NetworkCredential credentials)
    	{
			serverAddress = server;
			projectName = project;
			fetcher = new WorkItemFetcher(server, project, credentials, config);
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
            foreach (var iterationName in fetcher.GetAllIterations())
            {
                var iteration = new Iteration
                                    {
                                        Name = iterationName,
                                        SystemId = iterationName
                                    };

                foreach (var task in fetcher.GetAllWorkEffortInSprint(iterationName))
                {
                    iteration.AddTask(task);
                }

                project.AddIteration(iteration);
            }
        }
    }
}
