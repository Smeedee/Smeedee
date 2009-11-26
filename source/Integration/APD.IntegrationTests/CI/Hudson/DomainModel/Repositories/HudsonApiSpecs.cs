using System.Collections.Generic;

using APD.Integration.CI.Hudson.DomainModel.Repositories;
using APD.Integration.CI.Hudson.DomainModel.Repositories.Data;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;


namespace APD.IntegrationTests.CI.Hudson.Tests.DomainModel.Repositories
{
    public class Shared
    {
        protected static HudsonApi hudsonApi = null;
        
        protected Context Server_contains_data = () =>
        {
            Dictionary<string, string> testData = new Dictionary<string, string>();
            testData.Add("api/json", "{\"assignedLabels\":[{}],\"mode\":\"NORMAL\",\"nodeDescription\":\"the master Hudson node\",\"nodeName\":\"\",\"numExecutors\":2,\"description\":null,\"jobs\":[{\"name\":\"Smeedee\",\"url\":\"http://localhost:8080/job/Smeedee/\",\"color\":\"red_anime\"}],\"overallLoad\":{},\"primaryView\":{\"name\":\"All\",\"url\":\"http://localhost:8080/\"},\"slaveAgentPort\":0,\"useCrumbs\":false,\"useSecurity\":true,\"views\":[{\"name\":\"All\",\"url\":\"http://localhost:8080/\"}]}");
            testData.Add("job/Smeedee/api/json", "{\"actions\":[],\"description\":\"\",\"displayName\":\"Smeedee\",\"name\":\"Smeedee\",\"url\":\"http://localhost:8080/job/Smeedee/\",\"buildable\":true,\"builds\":[{\"number\":14,\"url\":\"http://localhost:8080/job/Smeedee/14/\"},{\"number\":13,\"url\":\"http://localhost:8080/job/Smeedee/13/\"},{\"number\":12,\"url\":\"http://localhost:8080/job/Smeedee/12/\"},{\"number\":11,\"url\":\"http://localhost:8080/job/Smeedee/11/\"},{\"number\":10,\"url\":\"http://localhost:8080/job/Smeedee/10/\"},{\"number\":9,\"url\":\"http://localhost:8080/job/Smeedee/9/\"},{\"number\":8,\"url\":\"http://localhost:8080/job/Smeedee/8/\"},{\"number\":7,\"url\":\"http://localhost:8080/job/Smeedee/7/\"},{\"number\":6,\"url\":\"http://localhost:8080/job/Smeedee/6/\"},{\"number\":5,\"url\":\"http://localhost:8080/job/Smeedee/5/\"},{\"number\":4,\"url\":\"http://localhost:8080/job/Smeedee/4/\"},{\"number\":3,\"url\":\"http://localhost:8080/job/Smeedee/3/\"},{\"number\":2,\"url\":\"http://localhost:8080/job/Smeedee/2/\"},{\"number\":1,\"url\":\"http://localhost:8080/job/Smeedee/1/\"}],\"color\":\"red_anime\",\"firstBuild\":{\"number\":1,\"url\":\"http://localhost:8080/job/Smeedee/1/\"},\"healthReport\":[{\"description\":\"Build stability: All recent builds failed.\",\"iconUrl\":\"health-00to19.gif\",\"score\":0}],\"inQueue\":false,\"keepDependencies\":false,\"lastBuild\":{\"number\":14,\"url\":\"http://localhost:8080/job/Smeedee/14/\"},\"lastCompletedBuild\":{\"number\":13,\"url\":\"http://localhost:8080/job/Smeedee/13/\"},\"lastFailedBuild\":{\"number\":13,\"url\":\"http://localhost:8080/job/Smeedee/13/\"},\"lastStableBuild\":null,\"lastSuccessfulBuild\":null,\"nextBuildNumber\":15,\"property\":[],\"queueItem\":null,\"downstreamProjects\":[],\"upstreamProjects\":[]}");
            hudsonApi = new HudsonApi(new MockRequestProxy(testData));
        };
    }

    [TestFixture]
    public class When_retreiving_data : Shared
    {
        [Test]
        public void list_of_projects_should_be_returned()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Server_contains_data);
                scenario.When("list of projects is fetched");
                scenario.Then("a list of project objects is returned", () =>
                {
                    List<Project> projects = hudsonApi.FetchProjects();
                    Assert.IsNotNull(projects);
                    Assert.AreEqual(1, projects.Count);
                });
            });
        }

        [Test]
        public void list_of_jobs_should_be_returned()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Server_contains_data);
                scenario.When("list of jobs is fetched");
                scenario.Then("a list of job objects is returned", () =>
                {
                    List<Job> jobs = hudsonApi.FetchJobs();
                    Assert.IsNotNull(jobs);
                    Assert.AreEqual(1, jobs.Count);
                });
            });
        }

        [Test]
        public void job_should_be_returned()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Server_contains_data);
                scenario.When("a specific job is fetched");
                scenario.Then("a job object is returned", () =>
                {
                    Job job = hudsonApi.FetchJob("Smeedee");
                    Assert.IsNotNull(job);
                    Assert.AreEqual("", job.Description);
                    Assert.AreEqual("Smeedee", job.DisplayName);
                    Assert.AreEqual("Smeedee", job.Name);
                    Assert.AreEqual("http://localhost:8080/job/Smeedee/", job.Url);
                    Assert.AreEqual(true, job.Buildable);
                    Assert.IsNotNull(job.Builds);
                    Assert.AreEqual(14, job.Builds.Count);
                });
            });
        }
    }
}
