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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Tests;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.SourceControlTests.Controllers.CheckInNotificationControllerSpecs
{
    public class Shared
    {
        protected static PropertyChangedRecorder changeRecorder;
        protected static CheckInNotificationController controller;
        protected static Mock<ITimer> ITimerMock;
        protected static Mock<IRepository<Changeset>> repositoryMock;
        protected static Mock<IRepository<User>> userRepositoryMock;
        protected static CheckInNotificationViewModel viewModel;

        protected Then assure_it_queried_Changeset_Repository_for_the_newest_changeset =
            () => { repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>())); };

        protected Context controller_is_created = () => { CreateController(); };

        protected When the_controller_is_created = () => { CreateController(); };

        protected Context there_are_changesets_in_sourcecontrol = () =>
        {
            repositoryMock = new Mock<IRepository<Changeset>>();
            var changesets = new List<Changeset>
            {
                new Changeset()
                {
                    Revision = 201222,
                    Comment = "Added hello world method",
                    Time = new DateTime(1986, 5, 20),
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                }
            };
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
        };

        protected Context user_info_exist_in_userdb = () =>
        {
            var users = new List<User>();
            users.Add(new User()
            {
                Firstname = "Gøran",
                Surname = "Hansen",
                Username = "goeran",
                ImageUrl = "http://goeran.no/avatar.jpg",
                Email = "mail@goeran.no"
            });
            users.Add(new User()
            {
                Firstname = "Jonas",
                Surname = "Ask",
                Username = "jask",
                ImageUrl = "http://goeran.no/haldis.jpg",
                Email = "jask@trondheim.com"
            });

            userRepositoryMock = new Mock<IRepository<User>>();
            userRepositoryMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("goeran")))).Returns(
                new List<User>() {users.First()});

            userRepositoryMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("jask")))).Returns(
                new List<User>() {users.Last()});

            userRepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<User>>())).Returns(users);
        };

        private static void CreateController()
        {
            ITimerMock = new Mock<ITimer>();
            controller = new CheckInNotificationController(new CheckInNotificationViewModel(),
                                                           new NoUIInvokation(),
                                                           ITimerMock.Object,
                                                           repositoryMock.Object,
                                                           userRepositoryMock.Object,
                                                           new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                           new Logger(new LogEntryMockPersister()));
            viewModel = controller.ViewModel;
            changeRecorder = new PropertyChangedRecorder(viewModel);
            Thread.Sleep(1500);
        }
    }

    internal class LogEntryMockPersister : IPersistDomainModels<LogEntry> {
        public void Save(LogEntry domainModel) {}
        public void Save(IEnumerable<LogEntry> domainModels) {}
    }

    [TestFixture]
    public class When_controller_is_spawned : Shared
    {
        [Test]
        public void Should_load_changeset_data_into_the_ViewModel()
        {
            Scenario.New(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);

                scenario.Then("changeset data should be loaded into the ViewModel", () =>
                {
                    //viewModel.Message.ShouldBe("Added hello world method");
                    //viewModel.Date.ShouldBe(new DateTime(1986, 5, 20));
                    viewModel.Changesets.Count.ShouldNotBe(0);
                    viewModel.Changesets.First().Message.ShouldBe("Added hello world method");
                    viewModel.Changesets.First().Date.ShouldBe(new DateTime(1986, 5, 20).ToLocalTime());
                });
            }).Execute();
        }

        [Test]
        public void Should_load_userinfo_data_into_the_ViewModel()
        {
            Scenario.New(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);

                scenario.Then("user info data should be loaded into the ViewModel", () =>
                {
                    Person developer = viewModel.Changesets.First().Developer;
                    developer.Name.ShouldBe("Gøran Hansen");
                    developer.Email.ShouldBe("mail@goeran.no");
                    developer.ImageUrl.ShouldBe("http://goeran.no/avatar.jpg");
                });
            }).Execute();
        }

        [Test]
        public void Should_query_Changeset_repository_for_newest_changeset()
        {
            Scenario.New(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol)
                    .And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);

                scenario.Then(assure_it_queried_Changeset_Repository_for_the_newest_changeset);
            }).Execute();
        }

        [Test]
        public void Should_query_userdb_repository_for_userinfo()
        {
            Scenario.New(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);
    
                scenario.Then("it should fetch user info",() => 
                    userRepositoryMock.Verify(r => r.Get(It.IsAny<AllSpecification<User>>()), Times.Once()));
            }).Execute();
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void Should_query_Changeset_repository_for_newest_changeset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(controller_is_created);

                scenario.When("notified to refresh", () =>
                                                     ITimerMock.Raise(n => n.Elapsed += null,
                                                                                    new EventArgs()));

                scenario.Then("assure it queried Changeset Repository for the newest changeset", 
                    () => repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()),
                        Times.Exactly(2)));
            });
        }
    }

    [TestFixture]
    public class When_controller_is_spawned_and_Changeset_repository_impl_is_invalid : Shared
    {
        private Context Changeset_repository_return_a_null_reference_value = () =>
        {
            repositoryMock = new Mock<IRepository<Changeset>>();
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(
                new Object() as IEnumerable<Changeset>);
        };

        [Test]
        [Ignore]
        public void
            Assure_ImplementationViolationException_is_thrown_if_Changeset_repository_return_a_null_reference()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("there are no changesets in sourcecontrol").
                    And(Changeset_repository_return_a_null_reference_value).
                    And(user_info_exist_in_userdb);

                scenario.When("the controller is created");

                scenario.Then("assure ImplementationViolationException is thrown",
                              () => this.ShouldThrowException<Exception>(
                                        () => the_controller_is_created(),
                                        exception =>
                                        exception.Message.ShouldBe(
                                            "Violation of IChangesetRepository. Does not accept a null reference as a return value.")));
            });
        }
    }

    [TestFixture]
    public class When_controller_is_spawned_and_there_are_no_changesets_in_sourceControl : Shared
    {
        private Context There_are_no_changesets_in_sourceControl = () =>
        {
            repositoryMock = new Mock<IRepository<Changeset>>();
            var changesets = new List<Changeset>();
            repositoryMock.Setup(r => r.Get(new AllChangesetsSpecification())).Returns(changesets);
        };

        [Test]
        public void Assure_userinfo_is_not_loaded_into_the_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(There_are_no_changesets_in_sourceControl).
                    And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);

                scenario.Then("assure userinfo is not loaded into the viewModel",
                              () => { viewModel.Changesets.Count.ShouldBe(0); });
            });
        }

        [Test]
        public void Assure_viewModel_is_set_to_NoChangesets_state()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(There_are_no_changesets_in_sourceControl).
                    And(user_info_exist_in_userdb);

                scenario.When(the_controller_is_created);

                scenario.Then("assure viewModel is set to NoChangesets state", 
                    () => viewModel.NoChangesets.ShouldBeTrue());
            });
        }
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        // TODO: Move to new tests in the Framework
        [Test]
        public void Assure_state_in_ViewModel_is_set_to_Loading() {}

        //[Test]
        //public void Assure_DisplayLoadingAnimationCmdPublisher_is_notified()
        //{
        //    Scenario.StartNew(this, scenario =>
        //    {
        //        scenario.Given(there_are_changesets_in_sourcecontrol).
        //            And(user_info_exist_in_userdb).
        //            And(controller_is_created);

        //        scenario.When("loading data");

        //        scenario.Then("assure DisplayLoadingAnimationCmdPublisher is notified", () =>
        //            displayLoadingAnimationCmdPublisherMock.Verify(cp => cp.Notify(), Times.Once()));
        //    });
        //}

        //[Test]
        //public void Assure_HideLoadingAnimatioCmdPublisher_is_notified()
        //{
        //    Scenario.StartNew(this, scenario =>
        //    {
        //        scenario.Given(there_are_changesets_in_sourcecontrol).
        //            And(user_info_exist_in_userdb).
        //            And(controller_is_created);

        //        scenario.When("loading data");

        //        scenario.Then("assure HideLoadingAnimationCmdPublisher is notified", () =>
        //            hideLoadingAnimationCmdPublisherMock.Verify(cp => cp.Notify(), Times.Once()));
        //    });
        //}
    }

    [TestFixture]
    public class When_data_is_loaded : Shared
    {
        [Test]
        public void Assure_only_new_changesets_are_added_to_viewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(controller_is_created).
                    And("and data is loaded");

                scenario.When("data is reloaded from repository");


                scenario.Then("assure only new changesets are added to the viewmodel", () =>
                {
                    ObservableCollection<ChangesetViewModel> oldChangesets = viewModel.Changesets;

                    var changesets = new List<Changeset>();
                    changesets.Add(new Changeset()
                    {
                        Revision = 201222,
                        Comment = "Added hello world method",
                        Time = new DateTime(1986, 5, 20),
                        Author = new Author()
                        {
                            Username = "goeran"
                        }
                    });
                    changesets.Add(new Changeset()
                    {
                        Revision = 201223,
                        Comment = "New thing with comments",
                        Time = new DateTime(1986, 5, 23),
                        Author = new Author()
                        {
                            Username = "jask"
                        }
                    });
                    repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);

                    ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());

                    //viewModel.Changesets.Count.ShouldBe(2);

                    //Console.WriteLine( viewModel.Changesets);
                    //TODO: check that only the second item was added
                });
            });
        }

        [Test]
        public void Assure_state_in_ViewModel_is_set_to_Ready()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then("assure state in ViewModel is changed to Ready",
                              () => { viewModel.IsLoading.ShouldBeFalse(); });
            });
        }
    }
}