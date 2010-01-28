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
using System.Threading;

using APD.Client.Framework;
using APD.Client.Widget.SourceControl.Controllers;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Users;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.SourceControlTests.Controllers.TopCommitersControllerSpecs
{
    public class Shared
    {
        protected static int changesetRepositoryGetThreadId;
        protected static Mock<IRepository<Changeset>> changesetRepositoryMock;
        protected static TopCommitersController controller;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static Mock<IRepository<User>> userRepositoryMock;
        protected static Mock<ILog> logger = new Mock<ILog>();

        protected Context a_new_changeset_is_committed = () =>
        {
            List<Changeset> changesets = GenerateChangesetData();

            changesets.Add(new Changeset
            {
                Revision = 4,
                Time = new DateTime(1981, 11, 1),
                Comment = "Added support for superfeature",
                Author = new Author
                {
                    Username = "goeran"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(changesets.Where(c => c.Revision == 4)).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context a_new_changeset_with_a_new_user_is_committed = () =>
        {
            List<Changeset> changesets = GenerateChangesetData();

            changesets.Add(new Changeset
            {
                Revision = 5,
                Time = new DateTime(1981, 11, 1),
                Comment = "Added support for superfeature",
                Author = new Author
                {
                    Username = "heine"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<ChangesetsAfterRevisionSpecification>())).
                Returns(changesets.Where(c => c.Revision == 5)).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected When the_Controller_is_created = CreateController;
        protected Context the_Controller_is_spawned = CreateController;

        protected Context there_are_changesets_in_SourceControl_system = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(GenerateChangesetData()).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context there_are_changesets_with_null_Author = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(new List<Changeset>
                {
                    new Changeset
                    {
                        Revision = 1,
                        Time = new DateTime(1981, 9, 1),
                        Comment = "Repository created",
                        Author = null
                    },
                    new Changeset
                    {
                        Revision = 2,
                        Time = new DateTime(1981, 9, 2),
                        Comment = "Repository updated",
                        Author = new Author(null)
                    },
                    new Changeset
                    {
                        Revision = 3,
                        Time = new DateTime(1981, 9, 3),
                        Comment = "Repository updated some more",
                        Author = new Author("tuxbear")
                    }
                });
        };

        protected Context there_are_no_users_in_userdb = () =>
        {
            userRepositoryMock = new Mock<IRepository<User>>();
            userRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<User>>())).Returns(new List<User>());
        };

        protected Context there_are_users_in_userdb = () =>
        {
            userRepositoryMock = new Mock<IRepository<User>>();

            var users = new List<User>
            {
                new User
                {
                    Username = "goeran",
                    Email = "mail@goeran.no",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    ImageUrl = "http://goeran.no/avatar.jpg"
                },
                new User
                {
                    Username = "dagolap",
                    Email = "mail@goeran.no",
                    Firstname = "Dag",
                    ImageUrl = "http://goeran.no/avatar.jpg"
                },
                new User
                {
                    Username =  "heine",
                    Email = "heine@heine.no",
                    Firstname = "Heine",
                    ImageUrl =  "http://goeran.no/avatar.jpg"
                }
            };

            userRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<User>>())).Returns(users);
        };

        private static List<Changeset> GenerateChangesetData()
        {
            var changesets = new List<Changeset>();
            changesets.Add(new Changeset
            {
                Revision = 1,
                Time = new DateTime(1981, 9, 1),
                Comment = "Repository created",
                Author = new Author
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset
            {
                Revision = 2,
                Time = new DateTime(1981, 9, 1),
                Comment = "Added build script",
                Author = new Author
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset
            {
                Revision = 3,
                Time = new DateTime(1981, 10, 1),
                Comment = "Added unit testing framework",
                Author = new Author
                {
                    Username = "dagolap"
                }
            });
            return changesets;
        }

        private static void CreateController()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new TopCommitersController(iNotifyWhenToRefreshMock.Object,
                                                    changesetRepositoryMock.Object,
                                                    userRepositoryMock.Object,
                                                    new NoUIInvocation(),
                                                    new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                    logger.Object);
        }
    }

    public class LogEntryMockPersister : IPersistDomainModels<LogEntry>
    {
        public void Save(LogEntry domainModel) { }
        public void Save(IEnumerable<LogEntry> domainModels) { }
    }

    [TestFixture]
    public class Controller_is_spawned : Shared
    {
        [Test]
        public void Assure_it_query_ChangesetRepository_for_all_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system)
                    .And(there_are_users_in_userdb);

                scenario.When(the_Controller_is_created);

                scenario.Then("assure it query ChangesetRepository for all changesets", () =>
                                                                                        changesetRepositoryMock
                                                                                            .Verify(
                                                                                            r =>
                                                                                            r.Get(
                                                                                                It.IsAny
                                                                                                    <
                                                                                                    Specification<Changeset>
                                                                                                    >()),
                                                                                            Times.Once()));
            });
        }
    }

    [TestFixture]
    public class Loading_data : Shared
    {
        [Test]
        public void Assure_data_loading_is_performed_on_the_specified_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_users_in_userdb).
                    And(there_are_changesets_in_SourceControl_system)
                    .And(the_Controller_is_spawned);

                scenario.When("data is loading");

                scenario.Then(
                    "assure data loading is performed on the supplied thread (this case: current thread)",
                    () =>
                    changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
            });
        }

        [Test]
        public void Assure_controller_can_handle_null_author()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_users_in_userdb).
                    And(there_are_changesets_with_null_Author)
                    .And(the_Controller_is_spawned);

                scenario.When("Loading data");

                scenario.Then("No Exception should be thrown",() =>
                {
                    logger.Verify(l=>l.WriteEntry(It.IsAny<LogEntry>()), Times.Never());
                });
            });
        }
    }


    [TestFixture]
    public class Data_is_loaded : Shared
    {
        [Test]
        public void Assure_data_is_loaded_into_the_viewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb);

                scenario.When(the_Controller_is_created);

                scenario.Then("assure data is loaded into the viewModel", () =>
                {
                    Thread.Sleep(200);
                    controller.ViewModel.Data.Count.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().
                        NumberOfCommits.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().
                        NumberOfCommits.ShouldBe(1);
                });
            });
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void changed_NumberOfCommits_should_not_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned).
                    And(a_new_changeset_is_committed).
                    And("subscribe to ViewModel CollectionChanged",
                        () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });


                scenario.When("controller is notified to refresh",
                    () => { iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs()); });

                scenario.Then("assure new NumberOfCommits is loaded into the viewModel",
                    () =>
                    {
                        controller.ViewModel.Data.Count.ShouldBe(2);
                        controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(3);
                        controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
                    }).
                    And("the CollectionChanged event in ViewModel should not have been fired",
                        () => { viewModelChanged.ShouldBeFalse(); });
            });
        }

        [Test]
        public void new_user_in_changesets_should_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned).
                    And(a_new_changeset_with_a_new_user_is_committed).
                    And("subscribe to ViewModel CollectionChanged",
                        () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });

                scenario.When("controller is notified to refresh", () =>
                                                                   iNotifyWhenToRefreshMock.Raise(
                                                                       n => n.Refresh += null,
                                                                       new RefreshEventArgs()));

                scenario.Then("the viewmodel should be updated", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(3);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().
                        NumberOfCommits.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().
                        NumberOfCommits.ShouldBe(1);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("heine")).First().
                        NumberOfCommits.ShouldBe(1);
                }).
                    And("the CollectionChanged event in ViewModel should have been fired",
                        () => { viewModelChanged.ShouldBeTrue(); });
            });
        }

        [Test]
        public void should_query_repository_for_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned);

                scenario.When("controller is notified to refresh", () =>
                                                                   iNotifyWhenToRefreshMock.Raise(
                                                                       n => n.Refresh += null,
                                                                       new RefreshEventArgs()));

                scenario.Then("it should query repository for changesets", () =>
                                                                           changesetRepositoryMock.Verify(
                                                                               r =>
                                                                               r.Get(
                                                                                   It.IsAny
                                                                                       <
                                                                                       Specification<Changeset>
                                                                                       >()), Times.Exactly(2)));
            });
        }

        [Test]
        public void unchanged_changeset_should_not_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned).
                    And("subscribe_to_ViewModel_CollectionChanged",
                        () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });


                scenario.When("controller is notified to refresh", () =>
                                                                   iNotifyWhenToRefreshMock.Raise(
                                                                       n => n.Refresh += null,
                                                                       new RefreshEventArgs()));

                scenario.Then("the viewmodel should not be updated",
                              () => { viewModelChanged.ShouldBeFalse(); });
            });
        }
    }
}