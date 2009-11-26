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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;
using APD.DomainModel.ProjectInfo;


namespace APD.DomainModel.ProjectInfoTest
{
    public class Shared
    {
        protected static ProjectInfoServer server;

        public Context server_has_been_created = () =>
        {
            server = new ProjectInfoServer("FakeName", "FakeUrl");
        };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("a ProjectInfoServer is to be created");
                scenario.When("the ProjectInfoServer object is created with the default constructor");
                scenario.Then("a ProjectInfoServer object should be made", ()=>
                {
                    var projectInfoserver = new ProjectInfoServer();

                    projectInfoserver.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_name_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(server_has_been_created);
                scenario.When("always");
                scenario.Then("the ProjectInfoServer should have an Name property", () =>
                {
                    server.Name.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_url_propery()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(server_has_been_created);
                scenario.When("always");
                scenario.Then("the ProjectInfoServer should have an Url property", () =>
                {
                    server.Url.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_Projects_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(server_has_been_created);
                scenario.When("always");
                scenario.Then("the ProjectInfoServer should have a Projects property", ()=>
                {
                    server.Projects.ShouldBeInstanceOfType<List<Project>>();
                });
            });
        }
    }

    [TestFixture]
    public class When_adding_projects : Shared
    {
        
        [Test]
        public void should_be_able_to_add_projects()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(server_has_been_created);

                scenario.When("Projects are to be added");
                scenario.Then("the Projects are stored in a list", () =>
                {
                    var project1 = new Project();

                    server.AddProject(project1);

                    var projects = (List<Project>) server.Projects;

                    projects.First().ShouldBe(project1);
                });
            });
        }

        [Test]
        public void should_set_ProjectInfoServer_reference_on_Project()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(server_has_been_created);

                scenario.When("a Projet is added");

                scenario.Then("the server property on the Project object should point to the current ProjectInfoServer", () =>
                {
                    Project project = new Project();
                    server.AddProject(project);

                    project.Server.Name.ShouldBe(server.Name);
                });
            });
        }
    }
}