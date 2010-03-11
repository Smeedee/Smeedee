using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public class ScrumForTFSRepository : IRepository<ProjectInfoServer>
    {
        private IFetchWorkItems fetcher;
        
        public ScrumForTFSRepository(string server, string project, ICredentials credentials)
        {
            fetcher = new WorkItemFetcher(server, project, "\\", credentials);
        }

        public ScrumForTFSRepository(IFetchWorkItems workItemFetcher)
        {
            fetcher = workItemFetcher;
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            //TODO: Config and whatnot
            var server = new ProjectInfoServer("Team Foundation Work Items", "");
            var project = new Project();

            foreach (var iterationName in fetcher.GetAllIterations())
            {
                var iteration = new Iteration();
                iteration.Name = iterationName;

                foreach (var task in fetcher.GetAllWorkEffortInSprint(iterationName))
                {
                    iteration.AddTask(task);
                }

                project.AddIteration(iteration);
            }

            server.AddProject(project);

            var results = new List<ProjectInfoServer> {server};

            return results;
        }
    }
}
