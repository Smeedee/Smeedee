#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;

using Moq;
using TinyBDD.Specification.NUnit;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.IntegrationTests.PMT.RallyDev.RallyDevReaderSpecs
{

    [TestFixture][Category("IntegrationTest")]
    public class When_reading_from_RallyDev
    {

        private static string PROJECT_URL;
        private static string USERNAME;
        private static string PASSWORD;

        //private static Mock<IDownloadXml> xmlDownloaderMock;

        protected static RallyDevReader rallyDevReader;


        protected Context credentials_are_correct = () =>
        {
            PROJECT_URL = "https://community.rallydev.com/slm/webservice/1.13/";
            USERNAME = "d.nyvik@gmail.com";
            PASSWORD = "haldis07";
        };


        protected Context the_rallyDevReader_is_created = () =>
        {
            rallyDevReader_is_created();
        };


        protected static void rallyDevReader_is_created()
        {
            //xmlDownloaderMock = new Mock<IDownloadXml>();

            //xmlDownloaderMock.Setup(dl => dl.GetXmlDocumentString(It.IsAny<string>())).
            //    Returns((string xmlurl) => GetMockXmlDocument(xmlurl));

            //rallyDevReader = new RallyDevReader(PROJECT_URL, xmlDownloaderMock.Object);
            rallyDevReader = new RallyDevReader(PROJECT_URL, new XmlDownloader(USERNAME,PASSWORD));
        }
      
        private static string GetMockXmlDocument(string xmlUrl)
        {
            return "To be implemented with unit tests";
        }


        [Ignore]
        [Test]
        public void should_return_all_projects()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(credentials_are_correct)
                    .And(the_rallyDevReader_is_created);
                scenario.When("all projects asked for");
                scenario.Then("all projects are returned", ()=>
                {
                    var projects = new List<Project>();
                    projects.AddRange(
                        rallyDevReader.Get(new AllSpecification<ProjectInfoServer>()).First().Projects);

                    (projects.Count >= 1).ShouldBeTrue();
                    projects[0].Name.ShouldBe("Continuous Project Monitor");
                });
            });

        }

        [Ignore]
        [Test]
        public void should_return_project_with_the_given_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(credentials_are_correct)
                    .And(the_rallyDevReader_is_created);

                scenario.When("a given project is requested");

                scenario.Then("the project with the given name is returned", () =>
                {
                    var project = rallyDevReader.GetProjectWithAllInfo("Continuous Project Monitor");
                    project.Name.ShouldBe("Continuous Project Monitor");
                    project.SystemId.ShouldBe("290873262");

                    ( project.GetIterationCount() >= 5 ).ShouldBeTrue();

                    foreach (var iteration in  project.Iterations)
                    {
                        iteration.Project.Name.ShouldBe(project.Name);

                        foreach (var task in iteration.Tasks)
                        {
                            task.Iteration.Name.ShouldBe(iteration.Name);
                        }
                    }
                    project.CurrentIteration.Name.ShouldBe("Sprint 3");
                });
            });
        }

        [Ignore]
        [Test]
        public void should_return_the_iterations_for_a_given_project()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(credentials_are_correct)
                    .And(the_rallyDevReader_is_created);
                scenario.When("the iterations for a given project is requested");
                scenario.Then("the iterations for the given project is returned", () =>
                {
                    List<Iteration> iterations = rallyDevReader.GetIterationsForProject("Continuous Project Monitor");

                    iterations[4].Name.ShouldBe("Sprint 3");

                    iterations[4].StartDate.ShouldBe(new DateTime(2009, 07, 28, 2, 0, 0));

                    iterations[4].EndDate.ShouldBe(new DateTime(2009, 08, 17, 1, 59, 59));

                    (iterations.Count >= 4).ShouldBeTrue();
                });
            });
        }

        [Ignore]   
        [Test]
        public void should_return_the_tasks_for_a_given_iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(credentials_are_correct)
                    .And(the_rallyDevReader_is_created);

                scenario.When("the tasks for a given iteration is requested");

                scenario.Then("the tasks for the given iteration is returned", () =>
                {
                    List<Task> tasks = rallyDevReader.GetTasksForIteration("Startup");

                    (tasks.Count>=9).ShouldBeTrue();
                    tasks[0].Name.ShouldBe("Contact Microsoft Student Community");
                    tasks[0].WorkEffortEstimate.ShouldBe(1);
                    tasks[3].Name.ShouldBe("Create Agenda for the Presentation");
                    (tasks[3].WorkEffortHistory.Count()>=3).ShouldBeTrue();
                    
                });
            });
        }

        [Ignore]
        [Test]
        public void should_return_references_over_pagesize()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(credentials_are_correct)
                    .And(the_rallyDevReader_is_created);

                scenario.When("tasks or for a given iteration is requested");
                scenario.Then("all the task for the given iteration is returned even if query-result is bigger then pagesize", ()=>
                {
                    var iterations = rallyDevReader.GetIterationsForProject("Continuous Project Monitor");

                    (iterations.Count >= 5).ShouldBeTrue();

                    var tasks = rallyDevReader.GetTasksForIteration("Sprint 3");

                    (tasks.Count >= 31).ShouldBeTrue();
                });
            });
        }
    }
}
