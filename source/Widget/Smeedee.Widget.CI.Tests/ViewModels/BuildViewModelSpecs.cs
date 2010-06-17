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

using System;
using System.Threading;


using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Tests;
using Smeedee.Widget.CI;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.CI.Tests.BuildViewModelSpecs
{
    public class Shared
    {
        protected static BuildViewModel viewModel;

        protected Context the_viewmodel_finishes_building_successful =
            () => { viewModel.Status = BuildStatus.Successful; };

        protected Context the_viewmodel_has_been_created =
            () => { viewModel = new BuildViewModel(); };

        protected Context the_viewmodel_is_building = () => { viewModel.Status = BuildStatus.Building; };
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_have_a_build_duration_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("accessing the BuildDuration property");
                scenario.Then("it should be a TimeSpan object",
                              () => viewModel.BuildDuration.ShouldBeInstanceOfType<TimeSpan>());
            });
        }

        [Test]
        public void should_have_a_end_time_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("accessing the FinishedTime property");
                scenario.Then("it should be a DateTime object",
                              () => viewModel.FinishedTime.ShouldBeInstanceOfType<DateTime>());
            });
        }

        [Test]
        public void should_have_a_start_time_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("accessing the StartTime property");
                scenario.Then("it should be a DateTime object",
                              () => viewModel.StartTime.ShouldBeInstanceOfType<DateTime>());
            });
        }

        [Test]
        public void should_have_an_unknown_trigger()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the trigger is accessed");
                scenario.Then("Trigger Name should be 'Unknown user'",
                              () => viewModel.TriggeredBy.Name.ShouldBe("Unknown user"));
            });
        }

        [Test]
        public void should_have_unknown_build_status()
        {
            viewModel.Status.ShouldBe(BuildStatus.Unknown);
        }

        [Test]
        public void should_have_unknown_trigger_cause_status()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("accessing the TriggerCause property");
                scenario.Then("it should be set to 'Unknown'",
                              () => viewModel.TriggerCause.ShouldBe("Unknown"));
            });
        }
    }

    [TestFixture]
    public class when_building : Shared
    {
        [Test]
        public void buildDuration_should_increase()
        {
            TimeSpan initialDuration = TimeSpan.MaxValue;

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created).
                    And(the_viewmodel_is_building).
                    And("initial time is recorded", () =>
                                                    initialDuration = viewModel.BuildDuration);

                scenario.When("a little time goes by", () => Thread.Sleep(1000));

                scenario.Then("the BuildDuration should increase",
                              () => ( initialDuration < viewModel.BuildDuration ).ShouldBe(true));
            });
        }
    }

    [TestFixture]
    public class when_build_finishes : Shared
    {
        [Test]
        public void buildDuration_should_stop_increasing()
        {
            TimeSpan initialDuration = TimeSpan.MaxValue;

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created).
                    And(the_viewmodel_is_building).
                    And(the_viewmodel_finishes_building_successful).
                    And("final build time is recorded", () =>
                                                        initialDuration = viewModel.BuildDuration);

                scenario.When("a little time goes by", () => Thread.Sleep(1000));

                scenario.Then("the BuildDuration should not change",
                              () => ( initialDuration == viewModel.BuildDuration ).ShouldBe(true));
            });
        }
    }

    [TestFixture]
    public class when_build_status_is_unknown : Shared
    {
        [Test]
        public void assure_duration_is_zero()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario
                    .Given(the_viewmodel_has_been_created)
                    .And("the start time has been set", () => viewModel.StartTime = DateTime.Now - new TimeSpan(0, 0, 10))
                    .And("the finished time is zero", () => viewModel.FinishedTime = default(DateTime));

                scenario.When("the build status is unknown", () => viewModel.Status = BuildStatus.Unknown);
                
                scenario.Then("assure duration is zero",
                              () => ( viewModel.BuildDuration == TimeSpan.Zero ).ShouldBe(true));
            }
                );
        }
    }

    [TestFixture]
    public class when_properties_change : Shared
    {
        private readonly Then listeners_are_notified_about_change =
            () => PropertyTester.WasNotified.ShouldBeTrue();

        [Test]
        public void listeners_should_be_notified_when_finishedtime_change()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the FinishedTime property changes", () =>
                                                                   PropertyTester.TestChange(viewModel,
                                                                                             vm =>
                                                                                             vm.FinishedTime));
                scenario.Then(listeners_are_notified_about_change);
            });
        }

        [Test]
        public void listeners_should_be_notified_when_starttime_change()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the StartTime property changes", () =>
                                                                PropertyTester.TestChange(viewModel,
                                                                                          vm => vm.StartTime));
                scenario.Then(listeners_are_notified_about_change);
            });
        }

        [Test]
        public void listeners_should_be_notified_when_status_change()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the Status property changes", () =>
                                                             PropertyTester.TestChange(viewModel,
                                                                                       vm => vm.Status));
                scenario.Then(listeners_are_notified_about_change);
            });
        }

        [Test]
        public void listeners_should_be_notified_when_triggercause_change()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the TriggerCause property changes", () =>
                                                                   PropertyTester.TestChange(viewModel,
                                                                                             vm =>
                                                                                             vm.TriggerCause));
                scenario.Then(listeners_are_notified_about_change);
            });
        }

        [Test]
        public void listeners_should_be_notified_when_triggeredBy_change()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_viewmodel_has_been_created);
                scenario.When("the TriggeredBy property changes", () =>
                                                                  PropertyTester.TestChange(viewModel,
                                                                                            vm =>
                                                                                            vm.TriggeredBy,
                                                                                            new Person()
                                                                                            {
                                                                                                Firstname =
                                                                                                    "fake person 1234"
                                                                                            }));
                scenario.Then(listeners_are_notified_about_change);
            });
        }
    }
}