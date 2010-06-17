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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using Smeedee.DomainModel.ProjectInfo;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.ProjectSpecs
{
    public class Shared
    {
        protected static ProjectInfoServer server;
        protected static Project project;

        protected Context project_and_projectInfoServer_has_been_created = () =>
        {
            server = new ProjectInfoServer();
            server.Url = "Fake.com";
            
            project = new Project("Bad Bush");
            project.SystemId = "1";
        };

        protected Context project_is_added_to_server = () =>
        {
            server.AddProject(project);
        };
        
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        
        [SetUp]
        public void SetUp()
        {
            server = new ProjectInfoServer();
            project = new Project("test");
            server.AddProject(project);
        }

        [Test]
        public void should_have_default_constructor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("a project is to be created");

                scenario.When("the project object is created with default constructor");
                scenario.Then("a project object should be made", ()=>
                {
                    project = new Project();
                    project.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_CurrentIteration_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created);
                scenario.When("always");
                scenario.Then("the project should have an iteration property", ()=>
                {
                    project.Iterations.ShouldNotBeNull();
                    project.Iterations.ShouldBeInstanceOfType<List<Iteration>>();
                });
            });
        }

        [Test]
        public void should_have_SystemId_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created);
                scenario.When("always");
                scenario.Then("the project should have an SystemId property", () =>
                {
                    project.SystemId.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_Name_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created);
                scenario.When("always");
                scenario.Then("the project should have an Name property", () =>
                {
                    project.Name.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void should_have_ProjectInfoServer_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created);
                scenario.When("always");
                scenario.Then("the project should have an ProjectInfoServer property", () =>
                {
                    project.Server.ShouldBeNull();
                });
            });
        }


        [Test]
        public void should_generate_unique_hashcode()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("the hashcode of a project object is requested");
                scenario.Then("an unique hashcode should be returned" , ()=>
                {
                    var otherProject = new Project("Bad Bush");
                    otherProject.Server = server;
                    otherProject.SystemId = "2";

                    otherProject.GetHashCode().ShouldNotBe(project.GetHashCode());
                });
            });
        }

        [Test]
        public void should_have_equals_method()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("the project is compared with another");
                scenario.Then("the equals method should return true only if they have the same hashCode", ()=>
                {
                    var otherProject = new Project("Bad Bush");
                    otherProject.Server = server;
                    otherProject.SystemId = project.SystemId;

                    project.Equals(otherProject).ShouldBeTrue();

                    otherProject.SystemId = "Somethingnew";

                    project.Equals(otherProject).ShouldBeFalse();
                });
            });
        }

        
    }

    [TestFixture]
    public class When_adding_iterations : Shared
    {

        [Test]
        public void should_be_able_to_add_Iterations()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("Iterations are to be added");

                scenario.Then("the iterations are stored in a list", ()=>
                {
                    var iteration1 = new Iteration();
                    project.AddIteration(iteration1);
                    var iteration2 = project.CurrentIteration;
                    project.AddIteration(iteration2);
                    iteration1.ShouldBe(iteration2);
                });
            });
        }

        [Test]
        public void should_set_Project_reference_on_Iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("an Iteration is added");

                scenario.Then(
                    "the Project property on the Iteration object should point to the current Project", () =>
                    {
                        var iteration = new Iteration();
                        project.AddIteration(iteration);
                        
                        iteration.Project.Name.ShouldBe(project.Name);
                    });
            });
        }

        [Test]
        public void should_automatically_set_current_iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("there are more then one Iteration to be added");
                scenario.Then("the Iteration which spans over todays date is set as current Iteration" , ()=>
                {
                    var iteration1 = new Iteration();
                    var iteration2 = new Iteration();

                    iteration1.StartDate = DateTime.Now.AddDays(-5);
                    iteration1.EndDate = DateTime.Now.AddDays(5);

                    iteration2.StartDate = DateTime.Now.AddDays(6);
                    iteration2.EndDate = DateTime.Now.AddDays(12);

                    project.AddIteration(iteration1);
                    project.AddIteration(iteration2);

                    project.CurrentIteration.ShouldBe(iteration1);
                });
            });
        }

        [Test]
        public void should_automatically_set_current_iteration_as_previous_iteration_if_no_current_iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("there are more then one Iteration to be added");
                scenario.Then("if none of them span over todays date the current Iteration should be set to the closest ending Iteration" ,()=>
                {
                    var iteration1 = new Iteration();
                    var iteration2 = new Iteration();
                    var iteration3 = new Iteration();

                    iteration1.StartDate = DateTime.Now.AddDays(-15);
                    iteration1.EndDate = DateTime.Now.AddDays(-10);

                    iteration2.StartDate = DateTime.Now.AddDays(-8);
                    iteration2.EndDate = DateTime.Now.AddDays(-2);

                    iteration3.StartDate = DateTime.Now.AddDays(5);
                    iteration3.EndDate = DateTime.Now.AddDays(10);

                    project.AddIteration(iteration1);
                    project.AddIteration(iteration2);
                    project.AddIteration(iteration3);

                    project.CurrentIteration.StartDate.ShouldBe(iteration2.StartDate);
                });
            });
        }

        [Test]
        public void should_automatically_set_current_iteration_as_next_iteration_if_no_current_iteration_and_no_previuos_iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(project_and_projectInfoServer_has_been_created)
                    .And(project_is_added_to_server);

                scenario.When("there are more then one Iteration to be added");
                scenario.Then("if none of them span over todays date and there are no previuos Iterations the current Iteration should be set to the closest next Iteration", () =>
                {
                    var iteration1 = new Iteration();
                    var iteration2 = new Iteration();
                    var iteration3 = new Iteration();


                    iteration2.StartDate = DateTime.Now.AddDays(11);
                    iteration2.EndDate = DateTime.Now.AddDays(16);

                    iteration1.StartDate = DateTime.Now.AddDays(5);
                    iteration1.EndDate = DateTime.Now.AddDays(10);

                    iteration3.StartDate = DateTime.Now.AddDays(17);
                    iteration3.EndDate = DateTime.Now.AddDays(20);

                    project.AddIteration(iteration2);
                    project.AddIteration(iteration1);
                    project.AddIteration(iteration3);

                    project.CurrentIteration.ShouldBe(iteration1);
                });
            });
        }
    }
}