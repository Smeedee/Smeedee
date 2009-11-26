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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using Moq;
using APD.DomainModel.Users;
using APD.Client.Widget.Admin.Controllers;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework;
using APD.DomainModel.Framework;
using APD.Client.Framework.Controllers;
using TinyBDD.Specification.NUnit;
using System.Threading;
using APD.Tests;

// ReSharper disable InconsistentNaming
namespace APD.Client.Widget.AdminTests.Controllers.UserdbControllerSpecs
{
    public class Shared : ScenarioClass
    {
        protected static UserdbController controller;
        protected static UserdbViewModel viewModel;
        protected static Mock<IRepository<Userdb>> userdbRepositoryMock;
        protected static Mock<IPersistDomainModels<Userdb>> userdbPersistanceRepositoryMock;
        protected static Mock<INotifyWhenToRefresh> refreshNotifierMock;
        protected static Mock<INotify<EventArgs>> saveNotifier;
        protected static Mock<INotify<EventArgs>> editNotifierMock;
        protected static Mock<ITriggerEvent<ShowModalDialogEventArgs>> showModalDialogEventTriggerMock;
        protected static Mock<IInvokeBackgroundWorker<IEnumerable<Userdb>>> backgroundWorkerMock;
        protected static Userdb savedData;
        protected static PropertyChangedRecorder propertyChangeRecorder;

        protected Context users_exist_in_Userdb = () =>
        {
            userdbRepositoryMock = new Mock<IRepository<Userdb>>();

            var userdbs = new List<Userdb>();

            var userdb = new Userdb { Name = "userdb" };
            userdbs.Add(userdb);
            userdb.AddUser(new User
            {
                Email = "mail@goeran.no",
                ImageUrl = "http://goeran.no/haldis.jpg",
                Firstname = "Gøran",
                Surname = "Hansen",
                Username = "goeran"
            });

            userdbRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Userdb>>())).Returns(userdbs);
        };

        protected Context no_users_exist_in_UserDb = () =>
        {
            userdbRepositoryMock = new Mock<IRepository<Userdb>>();

            userdbRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Userdb>>())).Returns(new List<Userdb>());
        };

        protected Context controller_is_created = CreateController;

        private static void CreateController()
        {
            viewModel = new UserdbViewModel(new NoUIInvocation(), null, null, null);
            propertyChangeRecorder = new PropertyChangedRecorder(viewModel);
            refreshNotifierMock = new Mock<INotifyWhenToRefresh>();
            backgroundWorkerMock = new Mock<IInvokeBackgroundWorker<IEnumerable<Userdb>>>();
            backgroundWorkerMock.Setup(w => w.RunAsyncVoid(It.IsAny<Action>())).Callback<Action>( a => a.Invoke());
            saveNotifier = new Mock<INotify<EventArgs>>();
            editNotifierMock = new Mock<INotify<EventArgs>>();
            showModalDialogEventTriggerMock = new Mock<ITriggerEvent<ShowModalDialogEventArgs>>();

            userdbPersistanceRepositoryMock = new Mock<IPersistDomainModels<Userdb>>();
            userdbPersistanceRepositoryMock.Setup(r => r.Save(It.IsAny<Userdb>())).Callback<Userdb>((d) =>
            {
                savedData = d;
            });

            if (userdbRepositoryMock == null)
                userdbRepositoryMock = new Mock<IRepository<Userdb>>();

            controller = new UserdbController(viewModel, userdbRepositoryMock.Object, backgroundWorkerMock.Object,
                refreshNotifierMock.Object, userdbPersistanceRepositoryMock.Object, 
                saveNotifier.Object, editNotifierMock.Object, showModalDialogEventTriggerMock.Object, new NoUIInvocation());
        }

        protected Context data_is_loaded = () =>
        {
            refreshNotifierMock.Raise(i => i.Refresh += null, new RefreshEventArgs());
        };

        protected When controller_is_spawned = CreateController;

        protected When notified_to_refresh = () =>
        {
            refreshNotifierMock.Raise(i => i.Refresh += null, new RefreshEventArgs());
        };

        protected When notified_to_save = () =>
        {
            saveNotifier.Raise(i => i.NewNotification += null, new EventArgs());
        };

        protected static void AssertUsers()
        {
            var user = savedData.Users.First();
            user.Username.ShouldBe("goeran");
            user.Firstname.ShouldBe("Gøran");
            user.Email.ShouldBe("mail@goeran.no");
            user.ImageUrl.ShouldBe("http://goeran.no/haldis.jpg");
            user.Name.ShouldBe("Gøran Hansen");
        }

        [TearDown]
        public void Teardown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("UserDbController is spanwed");
        }

        [Test]
        public void Assure_users_are_loaded_from_Repository()
        {
            Given(users_exist_in_Userdb);

            When(controller_is_spawned);

            Then("assure users are loaded from repository", () =>
                userdbRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Userdb>>()), Times.Once()));
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
	{
        [SetUp]
        public void Setup()
        {
            Scenario("Controller is notified to refresh");
        }

        [Test]
        public void Assure_users_are_loaded_from_UserdbRepository()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When(notified_to_refresh);

            Then("assure users are loaded from UserdbRepository", () =>
                userdbRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Userdb>>()), Times.Exactly(2)));
        }

        [Test]
        public void Assure_data_is_extracted_from_data_to_ViewModel()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When(notified_to_refresh);

            Then("assure data is extracted from data to ViewModel", () =>
            {
                viewModel.Name.ShouldBe("userdb");
                viewModel.Data.Count.ShouldBe(1);

                var user = viewModel.Data.First();
                user.Firstname.ShouldBe("Gøran");
                user.Name.ShouldBe("Gøran Hansen");
                user.Email.ShouldBe("mail@goeran.no");
                user.ImageUrl.ShouldBe("http://goeran.no/haldis.jpg");
                user.Username.ShouldBe("goeran");
            });
        }

        [Test]
        public void Assure_empty_user_database_causes_default_ViewModel()
        {

            Given(no_users_exist_in_UserDb)
            .And(controller_is_created);

            When(notified_to_refresh);

            Then("assure ViewModel is empty", () =>
            {
                viewModel.Data.Count.ShouldBe(0);
                viewModel.Name.ShouldBe("default");
            });
        }
    }

    [TestFixture]
    public class When_notified_to_edit : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Controller is notified to edit");
        }

        [Test]
        public void Assure_ShowModalDialogEvent_is_triggered()
        {
            Given(controller_is_created);

            When("notified to edit", () =>
                editNotifierMock.Raise(e => e.NewNotification += null, EventArgs.Empty));
            
            Then("assure ShowModalDialogEvent is triggered", () =>
                showModalDialogEventTriggerMock.Verify(t => t.NewEvent(It.IsAny<ShowModalDialogEventArgs>())));
        }
    }

    [TestFixture]
    public class When_saving : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Controller is saving");
        }

        [Test]
        public void Assure_PersistanceRepository_is_called()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When(notified_to_save);

            Then("assure PersistanceRepository is called", () =>
                userdbPersistanceRepositoryMock.Verify(r => r.Save(It.IsAny<Userdb>()), Times.Once()));
        }

        [Test]
        public void Assure_work_is_done_in_background()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When(notified_to_save);

            Then("assure work is done in background", () =>
                backgroundWorkerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Exactly(2)));
        }

        [Test]
        public void Assure_data_is_extracted_from_ViewModel_and_sent_to_the_PersistanceRepository()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created).
                And(data_is_loaded);

            When(notified_to_save);

            Then("Assure data is extracted from the ViewModel and sent to the PersistanceRepository", () =>
            {
                savedData.ShouldNotBeNull();
                savedData.Name.ShouldBe("userdb");
                savedData.Users.Count().ShouldBe(1);

                AssertUsers();
            });
        }

        [Test]
        public void Assure_ViewModel_is_notified_about_work_in_progress()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created).
                And("PropertyChange-vmRecorder is turned on", () =>
                    propertyChangeRecorder.ChangedProperties.Clear());

            When(notified_to_save);

            Then("assure viewModel is notified about work in progress", () =>
                propertyChangeRecorder.ChangedProperties.ShouldContain("IsLoading"));
        }
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Controller is loading");
        }

        [Test]
        public void Assure_data_is_loaded_on_a_background_thread()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When(controller_is_spawned);

            Then("assure data is loaded on a background thread", () =>
                backgroundWorkerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Once()));
        }

        [Test]
        public void Assure_ViewModel_is_notified_about_work_in_progress()
        {
            Given(users_exist_in_Userdb).
                And(controller_is_created);

            When("loading data");

            Then("assure IsLoading is set on ViewModel when loading starts", () =>
                propertyChangeRecorder.ChangedProperties.ShouldContain("IsLoading")).
                And("assure IsLoading is set on ViewModel when loading is complete", () =>
                    propertyChangeRecorder.ChangedProperties.Where(c => c.Equals("IsLoading")).Count().ShouldBe(2));
        }
    }
}
// ReSharper restore InconsistentNaming