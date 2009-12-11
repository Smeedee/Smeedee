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
using System.Threading;
using APD.Client.Framework;
using APD.Client.Widget.SourceControl.Controllers;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.SourceControlTests.Controllers.CommitStatisticsControllerSpecs
{
    public class Shared
    {
        protected static Mock<IRepository<Changeset>> changesetRepositoryMock;
        protected static CommitStatisticsController controller;
        protected static int changesetRepositoryGetThreadId;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>> backgroundWorkerInvokerMock;

        protected Context there_are_changesets_in_SourceControl_system = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            var changesets = new List<Changeset>();
            changesets.Add(new Changeset()
                           {
                               Revision = 1,
                               Time = DateTime.Today,
                               Comment = "Repository created",
                               Author = new Author()
                                        {
                                            Username = "goeran"
                                        }
                           });
            changesets.Add(new Changeset()
                           {
                               Revision = 2,
                               Time = DateTime.Today,
                               Comment = "Added build script",
                               Author = new Author()
                                        {
                                            Username = "goeran"
                                        }
                           });
            changesets.Add(new Changeset()
                           {
                               Revision = 3,
                               Time = DateTime.Today,
                               Comment = "Added unit testing framework",
                               Author = new Author()
                                        {
                                            Username = "dagolap"
                                        }
                           });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(() =>
                    changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };


        protected Context there_are_only_three_changesets_this_week = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            var changesets = new List<Changeset>();
            changesets.Add(new Changeset()
            {
                Revision = 1,
                Time = DateTime.Today.AddDays(-6),
                Comment = "Repository created",
                Author = new Author()
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset()
            {
                Revision = 2,
                Time = DateTime.Today.AddDays(-3),
                Comment = "Added build script",
                Author = new Author()
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset()
            {
                Revision = 3,
                Time = DateTime.Today,
                Comment = "Added unit testing framework",
                Author = new Author()
                {
                    Username = "dagolap"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(() =>
                                             changesetRepositoryGetThreadId =
                                             Thread.CurrentThread.ManagedThreadId);
        };


        protected When the_controller_is_created = CreateController;

        protected static void CreateController()
        {
            backgroundWorkerInvokerMock = new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new CommitStatisticsController(iNotifyWhenToRefreshMock.Object,
                                                        changesetRepositoryMock.Object,
                                                        new NoUIInvocation(),
                                                        new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                        new DatabaseLogger(new LogEntryMockPersister()));
        }
    }

    public class LogEntryMockPersister : IPersistDomainModels<LogEntry>
    {
        public void Save(LogEntry domainModel) { }
        public void Save(IEnumerable<LogEntry> domainModels) { }
    }


    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assert_groups_all_changesets_on_the_same_date()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system);
                scenario.When(the_controller_is_created);
                scenario.Then("all the changesets on the same date sould be grouped", () => { controller.ViewModel.Data.Count.ShouldBe(14); });
            });
        }


        [Test]
        public void Assure_data_loading_is_performed_on_the_specified_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system);

                scenario.When(the_controller_is_created);

                scenario.Then("assure data loading is performed on the supplied thread (this case: current thread)", () =>
                    changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
            });
        }

        [Test]
        public void Assure_data_loading_is_delegated_to_backgroundWorkerInvoker()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And("the controller is spawned using a BackgroundWorkerInvoker mock", () =>
                    {
                        backgroundWorkerInvokerMock =
                            new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
                        controller = new CommitStatisticsController(
                            iNotifyWhenToRefreshMock.Object,
                            changesetRepositoryMock.Object,
                            new NoUIInvocation(),
                            backgroundWorkerInvokerMock.Object,
                            new DatabaseLogger(new LogEntryMockPersister()));
                    });

                scenario.When("data is loading");

                scenario.Then("assure data loading is performed delegated to the BackgroundWorkerInvoker object", () =>
                    backgroundWorkerInvokerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Once()));
            });
        }

        [Test]
        public void Assure_we_have_data_points_for_all_days_in_the_week()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_only_three_changesets_this_week);
                scenario.When(the_controller_is_created);
                scenario.Then("we should have data points for dates with no commits", () =>
                          {

                              var controllerData = controller.ViewModel.Data;

                              controllerData.Count.ShouldBe(14);

                              int[] expectedCommits = new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1 };
                              DateTime[] testDates = new DateTime[]
                              {
                                  DateTime.Today.AddDays(-13),
                                  DateTime.Today.AddDays(-12),
                                  DateTime.Today.AddDays(-11),
                                  DateTime.Today.AddDays(-10),
                                  DateTime.Today.AddDays(-9),
                                  DateTime.Today.AddDays(-8),
                                  DateTime.Today.AddDays(-7),
                                  DateTime.Today.AddDays(-6),
                                  DateTime.Today.AddDays(-5),
                                  DateTime.Today.AddDays(-4),
                                  DateTime.Today.AddDays(-3),
                                  DateTime.Today.AddDays(-2),
                                  DateTime.Today.AddDays(-1),
                                  DateTime.Today
                              };


                              for (int day = 0; day < testDates.Length; day++)
                              {
                                  VerifyThatTheDateAndNumberOfCommitsAreCorrect(
                                      controllerData[day], testDates[day], expectedCommits[day]).ShouldBeTrue();
                              }

                          }
                    );
            });

        }

        private bool VerifyThatTheDateAndNumberOfCommitsAreCorrect
            (APD.Client.Widget.SourceControl.ViewModels.CommitStatisticsForDate commit, DateTime expectedDate, int expectedNumberOfCommits)
        {
            if (commit == null)
                return false;

            if (commit.Date.Date != expectedDate.Date)
                return false;

            if (commit.NumberOfCommits != expectedNumberOfCommits)
                return false;

            return true;

        }
    }



    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void should_query_repository_for_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system);

                scenario.When("the controller is created and asked to refresh", () =>
                {
                    CreateController();
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs());
                });

                scenario.Then("it should query repository for changesets", () =>
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<AllChangesetsSpecification>()), Times.Exactly(2)));
            });
        }
    }
}