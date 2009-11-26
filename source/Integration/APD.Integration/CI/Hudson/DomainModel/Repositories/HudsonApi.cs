using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Integration.CI.Hudson.DomainModel.Repositories.Data;


namespace APD.Integration.CI.Hudson.DomainModel.Repositories
{
    public class HudsonApi
    {
        #region Private Members

        private IRequestProxy requestProxy = null;

        #endregion

        #region Constructors

		public HudsonApi()
		{
			requestProxy = new RequestProxy();
		}

        public HudsonApi(string serverUrl)
		{
			requestProxy = new RequestProxy(serverUrl);
		}

        public HudsonApi(IRequestProxy requestProxy)
		{
			this.requestProxy = requestProxy;
		}

		#endregion

        #region Private Methods

        private Root FetchRootData()
        {
            Root root = requestProxy.Execute<Root>("api/json");

            if (root == null)
            {
                Console.WriteLine("Root is null");
            }

            return root;
        }

        #endregion

        #region Public Methods

        public List<Project> FetchProjects()
        {
            Root root = FetchRootData();

            if (root == null)
            {
                return null;
            }

            return root.Jobs;
        }

        public List<Job> FetchJobs()
        {
            Root root = FetchRootData();

            if (root == null)
            {
                return null;
            }

            if (root.Jobs.Count == 0)
            {
                return new List<Job>();
            }

            List<Job> jobs = new List<Job>();
            foreach (Project project in root.Jobs)
            {
                Job job = FetchJob(project.Name);
                if (job == null)
                    continue;

                jobs.Add(job);
            }
            return jobs;
        }

        public Job FetchJob(string jobName)
        {
            if (String.IsNullOrEmpty(jobName))
            {
                return null;
            }
            string endpoint = String.Format("job/{0}/api/json", jobName);
            return requestProxy.Execute<Job>(endpoint);
        }

        #endregion
    }
}
