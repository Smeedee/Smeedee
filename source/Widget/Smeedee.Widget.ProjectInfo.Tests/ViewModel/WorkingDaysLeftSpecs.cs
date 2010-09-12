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

#endregion

using NUnit.Framework;
using Smeedee.Tests;
using Smeedee.Widget.ProjectInfo.ViewModels;

namespace Smeedee.Widget.ProjectInfo.Tests.ViewModel
{
    public class Shared
    {
        protected WorkingDaysLeftViewModel viewModel;
    }
    

    [TestFixture]
    public class When_viewmodel_is_spawned : Shared
    {
        [SetUp]
        public  void Setup()
        {
            viewModel = new WorkingDaysLeftViewModel();
        }

        [Test]
        public void should_have_DaysRemaining_property()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;

            PropertyTester.TestForExistence<WorkingDaysLeftViewModel>(v => v.DaysRemaining);
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
        public void should_notify_user_when_there_are_no_projects_in_repository()
        {
            Assert.IsFalse(viewModel.ProjectsInRepository);

            var largeControllerText = WorkingDaysLeftViewModel.NO_PROJECTS_STRING_LARGE;
            var smallControllerText = WorkingDaysLeftViewModel.NO_PROJECTS_STRING;
            var largeViewModelText = viewModel.DaysRemainingTextLarge;
            var smallViewModelText = viewModel.DaysRemainingTextSmall;

            Assert.IsTrue(largeControllerText.CompareTo(largeViewModelText) == 0);
            Assert.IsTrue(smallControllerText.CompareTo(smallViewModelText) == 0);
        }

        [Test]
        public void should_notify_user_when_there_are_no_iterations_for_given_project()
        {
            viewModel.ProjectsInRepository = true;
            Assert.IsFalse(viewModel.IterationInProject);

            var largeControllerText = WorkingDaysLeftViewModel.NO_ITERATIONS_STRING_LARGE;
            var largeViewModelText = viewModel.DaysRemainingTextLarge;
            var smallControllerText = WorkingDaysLeftViewModel.NO_ITERATIONS_STRING;
            var smallViewModelText = viewModel.DaysRemainingTextSmall;

            Assert.IsTrue(largeControllerText.CompareTo(largeViewModelText) == 0);
            Assert.IsTrue(smallControllerText.CompareTo(smallViewModelText) == 0);
        }

        [Test]
        public void should_notify_user_when_iteration_is_on_overtime()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;
            viewModel.IsOnOvertime = true;
            Assert.AreSame(WorkingDaysLeftViewModel.DAYS_ON_OVERTIME_STRING, viewModel.DaysRemainingTextLarge);
            Assert.AreSame(WorkingDaysLeftViewModel.DAYS_ON_OVERTIME_STRING, viewModel.DaysRemainingTextSmall);
        }

        [Test]
        public void should_notify_user_when_iteration_is_on_overtime_using_singular_form_when_only_one_day()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;
            viewModel.IsOnOvertime = true;
            viewModel.DaysRemaining = 1;
            Assert.AreSame(WorkingDaysLeftViewModel.DAYS_ON_OVERTIME_SINGULAR_STRING, viewModel.DaysRemainingTextLarge);
        }

        [Test]
        public void should_notify_user_about_remaining_days_left()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;
            Assert.AreSame(WorkingDaysLeftViewModel.WORKING_DAYS_LEFT_STRING, viewModel.DaysRemainingTextLarge);
            Assert.AreSame(WorkingDaysLeftViewModel.DAYS_LEFT_STRING, viewModel.DaysRemainingTextSmall);
        }

        [Test]
        public void should_notify_user_about_remaining_days_in_singular_form_when_one_day_left()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;
            viewModel.DaysRemaining = 1;
            Assert.AreSame(WorkingDaysLeftViewModel.WORKING_DAYS_LEFT_SINGULAR_STRING, viewModel.DaysRemainingTextLarge);
        }

        [Test]
        public void should_notify_user_about_connection_problems()
        {
            viewModel.HasConnectionProblems = true;
        }

        [Test]
        public void should_not_display_days_when_having_problems()
        {
            viewModel.HasConnectionProblems = true;

            Assert.IsNull(viewModel.DaysRemaining);
        }

        [Test]
        public void should_display_days_when_no_problems()
        {
            viewModel.ProjectsInRepository = true;
            viewModel.IterationInProject = true;
            viewModel.HasConnectionProblems = false;
            viewModel.DaysRemaining = 42;

            Assert.IsNotNull(viewModel.DaysRemaining);
        }
    }
}