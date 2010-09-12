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

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo.ViewModels;
using Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftViewModelSpecs;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;

// ReSharper disable InconsistentNaming
namespace Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftControllerSpecs
{
    [TestFixture]
    public class When_the_controller_is_spawned : Shared
    {
        [SetUp]
        public void SetUp()
        {
            Scenario("Controller is spawned");
        }


        [Test]
        public void Configuration_should_be_checked_to_see_if_project_info_or_configuration_settings_should_be_used()
        {
            Given(configuration_says_to_use_end_date_from_configuration)
                .And(viewModels_exists);

            When(the_controller_is_created);

            Then("the configuration service should be asked for the project-info configuration", () =>
                settingViewModel.IsManuallyConfigured.ShouldBe(true));
        }

        [Test]
        public void Should_not_query_projectinfoRepository_if_using_manual_endDate()
        {
            Given(there_is_an_iteration_in_the_given_project)
                .And(viewModels_exists)
                .And(configuration_says_to_use_end_date_from_configuration);

            When(the_controller_is_created);

            Then("the project info repository should not be queried", () =>
                projectRepositoryMock.Verify(r => r.BeginGet(It.IsAny<ProjectInfoServerByName>()),
                                      Times.Never()));
        }

        [Test]
        public void should_notify_user_if_project_does_not_exist_in_repository()
        {
            Given(there_are_no_projects_in_the_repository).
                And(viewModels_exists).
                And(project_is_not_configured_manually).
                And(controller_is_created).
                And(large_text_is_displayed);

            When("requesting days remaining");

            Then("the user is notified that there are no projects in the repository", () =>
            {
                DisplayText.ShouldBe(MessageStrings.NO_INFORMATION_STRING_LARGE);
            });
        }

        [Test]
        public void should_update_days_on_overtime_in_viewmodel()
        {
            Given(there_is_an_iteration_in_the_given_project_running_over_deadline)
                .And(viewModels_exists)
                .And(there_is_a_configured_server_to_use)
                .And(there_is_a_configured_project)
                .And(controller_is_created)
                .And(large_text_is_displayed);

            When("requesting days remaining");

            Then("the number of days on overtime in regards to the current iteration is displayed", () =>
                    {
                        viewModel.DaysRemaining.ShouldBe(TestingIteration.CalculateWorkingdaysLeft(NowTime, NoHolidays).Days.ToString());           
                    })
                .And("the text should indicate that the iteration is on overtime", () =>
                    DisplayText.ShouldBe(MessageStrings.DAYS_ON_OVERTIME_STRING));
        }

        [Test]
        public void Should_use_end_date_from_config_if_project_info_setting_is_set()
        {
            Given(there_is_an_iteration_in_the_given_project)
                   .And(viewModels_exists)
                   .And(configuration_says_to_use_end_date_from_configuration);

            When(the_controller_is_created);

            Then("the configuration setting end-date should be read and returned", () =>
                {
                    IEnumerable<DayOfWeek> nonWorkingDays = NoNonWorkingDays.Where(d => d.IsNotWorkingDay).Select(d => d.Day);
                    viewModel.DaysRemaining.ShouldBe(
                        IterationFromConfig.CalculateWorkingdaysLeft(DateTime.Today, NoHolidays, nonWorkingDays).Days.ToString());           
                }
            );
        }

        [Test]
        public void Should_use_end_date_from_project_if_config_sais_not_to_use_config_endDate()
        {
            Given(viewModels_exists)
                .And(there_is_an_iteration_in_the_given_project)
                .And(configuration_says_not_to_use_end_date_from_configuration)
                .And(there_is_a_configured_server_to_use)
                .And(there_is_a_configured_project);

            When(the_controller_is_created);

            Then("the project info repository should be queried for the end date", () =>
                {
                    viewModel.DaysRemaining.ShouldBe(TestingIteration.CalculateWorkingdaysLeft(DateTime.Today, NoHolidays).Days.ToString());           
                }
            );
        }

        [Test]
        public void configuration_should_be_loaded_into_settings_view_model()
        {
            Given(viewModels_exists)
                .And(there_is_an_iteration_in_the_given_project)
                .And(configuration_says_not_to_use_end_date_from_configuration)
                .And(there_is_a_configured_server_to_use)
                .And(there_is_a_configured_project);

            When(the_controller_is_created);

            Then("The controller should load the configuration into the settings view model", () =>
            {
                settingViewModel.SelectedProject.ShouldBe(PROJECT_NAME);
                settingViewModel.SelectedServer.ShouldBe(SERVER_NAME);
                settingViewModel.SelectedEndDate.ShouldBe(TestingIteration.EndDate);
                settingViewModel.IsManuallyConfigured.ShouldBe(false);
                settingViewModel.NonWorkWeekDays.Count.ShouldBe(7);
            });
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void should_query_repository_for_given_projectInfoServer()
        {
            Given(there_is_an_iteration_in_the_given_project).
                And(viewModels_exists).
                And(project_is_not_configured_manually).
                And(there_is_a_configured_server_to_use).
                And(there_is_a_configured_project).
                And(controller_is_created);

            When("notified to refresh", () => notifiedToRefresh());

            Then("assure it queried project repository for the given project", () =>
            {
                projectRepositoryMock.Verify( r => r.BeginGet(new ProjectInfoServerByName(SERVER_NAME)), Times.Exactly(1));
            });
        }

        [Test]
        public void should_display_loading_data_message()
        {
            Given(there_is_an_iteration_in_the_given_project).
                And(viewModels_exists).
                And(project_is_not_configured_manually).
                And(there_is_a_configured_server_to_use).
                And(there_is_a_configured_project).
                And(controller_is_created);

            When("notified to refresh", () => notifiedToRefresh());

            Then("assure it queried project repository for the given project", () =>
            {
                projectRepositoryMock.Verify(r => r.BeginGet(new ProjectInfoServerByName(SERVER_NAME)), Times.AtLeastOnce());
            });
        }


    }

    [TestFixture]
    public class When_attempting_to_calculate_workingDays : Shared
    {
        [SetUp]
        public void Setup()
        {
            viewModel = new WorkingDaysLeftViewModel();
        }

        [Test]
        public void should_notify_user_when_there_is_no_information_on_the_server()
        {
            Assert.IsFalse(viewModel.HasInformationToShow);

            var largeControllerText = MessageStrings.NO_INFORMATION_STRING_LARGE;
            var largeViewModelText = viewModel.DaysRemainingTextLarge;
            var smallControllerText = MessageStrings.NO_INFORMATION_STRING;
            var smallViewModelText = viewModel.DaysRemainingTextSmall;
            
            Assert.IsTrue(largeControllerText.CompareTo(largeViewModelText) == 0);
            Assert.IsTrue(smallControllerText.CompareTo(smallViewModelText) == 0);
        }
    }

    [TestFixture]
    public class when_saving_manual_config: Shared
    {
        [Test]
        public void Assure_viewModel_is_updated_with_new_endDate()
        {
            DateTime newEndDate = DateTime.Today.AddDays(10);

            Given(there_are_no_projects_in_the_repository)
                .And(configuration_says_to_use_end_date_from_configuration)
                .And(viewModels_exists)
                .And(controller_is_created)
                .And("End Date is changed in settings", () => settingViewModel.SelectedEndDate = newEndDate);

            When("Save command is execute", () => settingViewModel.Save.Execute(null));
            
            Then("The viewModel should have the new EndDate", () => viewModel.EndDate.ShouldBe(newEndDate));
        }
    }

    [TestFixture]
    public class When_notifying : Shared
    {
        [Test]
        public void Assure_loadingNotifyer_is_shown_while_loading()
        {
            Given(some_valid_configuration_exists)
                .And(viewModels_exists)
                .And(controller_is_created);
            When(notifiedToRefresh);
            Then("the loadingNotifyer should be shown",
                () => loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.AtLeast(1)));
        }

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_loading()
        {
            Given("No controller spawned")
                .And(configuration_says_to_use_end_date_from_configuration)
                .And(viewModels_exists);
            When(the_controller_is_created);
            Then("", () =>
            {
                loadingNotifierMock.Verify(l => l.HideInView(), Times.AtLeastOnce());
                viewModel.IsLoading.ShouldBe(false);
            });
        }
    }
}
// ReSharper restore InconsistentNaming