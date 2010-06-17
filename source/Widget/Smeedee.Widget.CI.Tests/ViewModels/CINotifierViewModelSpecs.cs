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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Linq;
using Smeedee.Client.Framework;
using Smeedee.Widget.CI;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.CI.Tests.ViewModels.CINotifierSpecs
{
    public class Shared
    {
        protected static CINotifierViewModel viewModel;
        protected static CIViewModel ciViewModel;


        protected Context the_object_is_created_without_buildViewModel_status = () =>
        {
            ciViewModel = new CIViewModel();
            var projectInfoViewModel = new ProjectInfoViewModel();
            var buildViewModel = new BuildViewModel();
            

            projectInfoViewModel.LatestBuild = buildViewModel;
            ciViewModel.Data.Add(projectInfoViewModel);

            viewModel = new CINotifierViewModel(ciViewModel);
        };

        protected Context the_object_is_created_without_data = () =>
        {
            ciViewModel = new CIViewModel();
            viewModel = new CINotifierViewModel(ciViewModel);
        };


        protected Context the_object_is_created_with_buildViewModel_status_successfull = () =>
        {
            ciViewModel = new CIViewModel();
            var projectInfoViewModel = new ProjectInfoViewModel();
            var latestBuild = new BuildViewModel();

            latestBuild.Status = BuildStatus.Successful;
            projectInfoViewModel.LatestBuild = latestBuild;
            ciViewModel.Data.Add(projectInfoViewModel);

            viewModel = new CINotifierViewModel(ciViewModel);
        };

        protected Context the_object_is_created_with_buildViewModel_status_building = () =>
        {
            ciViewModel = new CIViewModel();
            var projectInfoViewModel = new ProjectInfoViewModel();
            var latestBuild = new BuildViewModel();

            latestBuild.Status = BuildStatus.Building;
            projectInfoViewModel.LatestBuild = latestBuild;
            ciViewModel.Data.Add(projectInfoViewModel);

            viewModel = new CINotifierViewModel(ciViewModel);
        };

        protected Context the_object_is_created_with_buildViewModel_status_failed = () =>
        {
            ciViewModel = new CIViewModel();
            var projectInfoViewModel = new ProjectInfoViewModel();
            var latestBuild = new BuildViewModel();

            latestBuild.Status = BuildStatus.Failed;
            projectInfoViewModel.LatestBuild = latestBuild;
            ciViewModel.Data.Add(projectInfoViewModel);

            viewModel = new CINotifierViewModel(ciViewModel);
        };


    }   

    [TestFixture]
    public class When_spawned : Shared
    {

        [Test]
        public void Should_have_default_constructor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("the object is createt with default constructor");
                scenario.When("always");
                scenario.Then("object should not be null", ()=>
                {
                    var viewModel = new CINotifierViewModel();
                    viewModel.ShouldNotBeNull();
                });
            });
        }


        [Test]
        public void Should_have_status_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_without_buildViewModel_status);
                scenario.When("status is set on the viewmodel");
                scenario.Then("then Status should not be null", () =>
                {
                    viewModel.Status = BuildStatus.Successful;
                    viewModel.Status.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void Should_have_default_status_unknown()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_without_buildViewModel_status);

                scenario.When("no data status is ateinable from ProjectInfroViewModel");

                scenario.Then("status should be set to successful", () =>
                    viewModel.Status.ShouldBe(BuildStatus.Unknown));
            });
        }
    }

    [TestFixture]
    public class When_properties_change : Shared
    {

        [Test]
        public void Should_set_status_to_successful()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_buildViewModel_status_successfull);

                scenario.When("there are no failed or building builds");

                scenario.Then("status should be set to successful", () =>
                    viewModel.Status.ShouldBe(BuildStatus.Successful));
            });
        }

        [Test]
        public void Should_set_status_to_building()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_buildViewModel_status_building);

                scenario.When("there are no projects with failed builds but one with status building");

                scenario.Then("status should be set to building", () =>
                    viewModel.Status.ShouldBe(BuildStatus.Building));
            });
        }

        [Test]
        public void Should_set_status_to_failed()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_buildViewModel_status_failed);

                scenario.When("there is aleast one project with failed status");

                scenario.Then("status should be set to failed", () =>
                    viewModel.Status.ShouldBe(BuildStatus.Failed));
            });
        }


        [Test]
        public void Assure_status_is_updated_when_projects_buildstatus_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_with_buildViewModel_status_failed);

                scenario.When("there is aleast one project with failed status");

                scenario.Then("status should change when a projects buildstatus is changed", () =>
                {
                    viewModel.Status.ShouldBe(BuildStatus.Failed);

                    ciViewModel.Data[0].LatestBuild.Status = BuildStatus.Successful;

                    viewModel.Status.ShouldBe(BuildStatus.Successful);
                });
            });
        }


        [Test]
        public void Assure_status_is_updated_when_data_is_added_to_civiewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_object_is_created_without_data);

                scenario.When("data is added to the civiewmodel");

                scenario.Then("status should change from successfull to the new projects buildstatus", () =>
                {
                    viewModel.Status.ShouldBe(BuildStatus.Unknown);
                    
                    var projectInfoViewModel = new ProjectInfoViewModel();
                    var buildViewModel = new BuildViewModel();
                    buildViewModel.Status = BuildStatus.Building;
                    projectInfoViewModel.LatestBuild = buildViewModel;
                    ciViewModel.Data.Add(projectInfoViewModel);

                    viewModel.Status.ShouldBe(BuildStatus.Building);
                });
            });
        }


        [Test] 
        public void Assure_status_is_updated_when_LatestBuild_is_overridden()
        {
             Scenario.StartNew(this, scenario =>
             {
                 scenario.Given(the_object_is_created_without_buildViewModel_status);
                 scenario.When("LatestBuild is set on projectInfroViewModel");
                 scenario.Then("status should change from unknown to the new latestbuild status", () =>
                 {
                     viewModel.Status.ShouldBe(BuildStatus.Unknown);

                     var buildViewModel = new BuildViewModel();
                     buildViewModel.Status = BuildStatus.Building;
                     ciViewModel.Data.Last().LatestBuild = buildViewModel;

                     viewModel.Status.ShouldBe(BuildStatus.Building);
                 });
             });
        }
    
    }
}
