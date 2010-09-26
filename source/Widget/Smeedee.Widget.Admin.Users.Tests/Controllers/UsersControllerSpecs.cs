using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.Admin.Users.Controllers;
using Smeedee.Widget.Admin.Users.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Users.Tests.Controllers
{
    

    [TestFixture]
    class When_controller_is_spawned : Shared
    {
        [Test]
        public void Assure_the_controller_works_when_no_Userdbs_exists()
        {
            Given(there_are_no_dbs);

            When(controller_is_created);

            Then(() => viewModel.Users.Count.ShouldBe(0));
        }

        [Test]
        public void Assure_that_HasDatabases_is_true_when_there_are_dbs()
        {
            Given(db_with_0_users_and_controller_has_been_created);

            When("always");

            Then(() => viewModel.HasDatabase.ShouldBeTrue());
        }

        [Test]
        public void Assure_loadData_is_not_called_when_there_are_no_dbs()
        {
            Given(there_are_no_dbs);

            When(controller_is_created);

            Then("loadData should not be called, no exceptions should be thrown after GetAllUserDBs()",
                () => viewModel.HasConnectionProblems.ShouldBe(false));
        }

        [Test]
        public void Assure_controller_updates_ViewModel_on_creation()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then("team members should be added to the ViewModel",() => 
                viewModel.Users.Count.ShouldBe(3));
        }

        [Test]
        public void Assure_viewModel_stores_correct_db_name()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then(() => viewModel.CurrentDBName.ShouldBe("Default0"));
        }

        [Test]
        public void Assure_all_userdbs_are_fetched()
        {
            Given(there_are_three_dbs_with_0_users_in_repository);

            When(controller_is_created);

            Then(() => viewModel.Userdbs.Count.ShouldBe(3));
        }
    }

    [TestFixture]
    public class When_ViewModel_commands : Shared
    {
        [Test]
        public void Assure_save_command_results_in_persistance_of_userdb()
        {
            Given(db_with_3_users_and_controller_has_been_created);

            When(viewModel_saves);

            Then("save should be performed on the persistDomainModels repository exactly once",
                () => mockPersistDomainModels.Verify(r => r.Save(It.IsAny<Userdb>()), Times.Once()));
        }

        [Test]
        public void Assure_the_correct_db_is_saved_from_viewModel_to_repository()
        {
            Given(there_are_three_dbs_with_0_users_in_repository).
                And(controller_has_been_created).
                And(currentDBName_is_changed_to_Default2);

            When(viewModel_saves);

            Then("the db saved should be the current one (in ViewModel.CurrentDBName)",
                () => mockPersistDomainModels.Verify(r => r.Save(It.Is<Userdb>(u => u.Name.Equals(viewModel.CurrentDBName)))));
        }
        
        [Test]
        public void Assure_the_correct_userdb_is_updated_on_the_ViewModel()
        {
            Given(there_are_three_dbs_with_0_users_in_repository).
                And(controller_has_been_created).
                And(currentDBName_is_changed_to_Default2);

            When(refresh_delegate_is_executed);

            Then(() => viewModel.CurrentDBName.ShouldBe("Default2"));
        }

        [Test]
        public void Assure_ViewModel_is_updated_when_refresh_delegate_is_executed()
        {
            Given(db_with_0_users_and_controller_has_been_created).
                And(db_has_been_updated_with_3_users);

            When(refresh_delegate_is_executed);

            Then(() => viewModel.Users.Count.ShouldBe(3));
        }

        [Test]
        public void Should_be_able_to_edit_users()
        {
            Given(db_with_3_users_and_controller_has_been_created).
                And(asyncUserdbPersisterMock_saves_users).
                And("user is edited", () =>
                {
                    viewModel.SelectedUser = new UserViewModel{Username = "0", Firstname = "simSalabim"};
                    viewModel.EditUser.Execute();
                    viewModel.EditedUser.Username = "Frank";
                    viewModel.EditedUser.Surname = "the tank";
                    viewModel.AddUser.Execute();

                }).
                And(repository_returnes_peristed_data);

            When(save_command_is_executed);

            Then("controller should save the edited collection", () =>
            {
                persistedData.Users.First().Username.ShouldBe("Frank");
                persistedData.Users.Count().ShouldBe(3);
            });
        }
    }

    [TestFixture]
    public class When_data_is_changed : Shared
    {
        [Test]
        public void Assure_repository_is_asked_for_all_userdbs_when_selected_userdb_is_changed()
        {
            Given(there_are_three_dbs_with_0_users_in_repository).
                And(controller_has_been_created);

            When(CurrentDBName_is_changed);

            Then(() => mockRepository.Verify(r => r.Get(It.IsAny<Specification<Userdb>>()), Times.Exactly(2)));
        }

        [Test]
        public void Assure_that_UpdateViewModel_exeptions_are_logged()
        {
            Given(there_are_three_dbs_with_0_users_in_repository).
                And(there_is_something_wrong_with_the_userdbRepository).
                And(controller_has_been_created);

            When("getting userdbs from repository throws an exception");
            
            Then(() =>
                    {
                        viewModel.HasConnectionProblems.ShouldBeTrue();
                        mockLog.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()));
                    });
        }

        [Test]
        public void Assure_dbList_is_updated_with_all_userDbs_names()
        {
            Given(there_are_three_dbs_with_0_users_in_repository);

            When(controller_is_created);

            Then(() => viewModel.Userdbs.Count.ShouldBe(3));
        }

        [Test]
        public void Assure_that_a_user_with_existing_username_will_not_be_created()
        {
            Given(db_with_3_users_and_controller_has_been_created);

            When(a_new_user_is_created_with_the_same_name);

            Then("the new user should not be added to the database", () => 
                viewModel.Users.Count.ShouldBe(3));
        }
       
        [Test]
        public void Assure_that_a_user_with_nonexisting_username_will_be_created()
        {
            Given(db_with_3_users_and_controller_has_been_created);

            When(a_new_user_is_created_with_a_new_name);

            Then("the new user should be added to the database", () => 
                viewModel.Users.Count.ShouldBe(4));
        }
    }

    [TestFixture]
    public class When_notifying : Shared
    {
        [Test]
        public void Assure_loadingNotifyer_is_shown_while_loading_data()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then("the loadingNotifyer should be shown", () =>
            {
                loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(1));
            });
        }

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_loading_data_is_complete()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then(() =>
            {
                loadingNotifierMock.Verify(l => l.HideInView(), Times.Exactly(2));
                viewModel.IsLoading.ShouldBeFalse();
            });
        }
        
        [Test]
        public void Assure_loadingNotifier_is_shown_while_saving_data()
        {
            Given(db_with_3_users_and_controller_has_been_created);

            When(save_command_is_executed);

            Then("the loadingNotifier should be shown", () =>
                {
                    loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(2));
                    viewModel.IsSaving.ShouldBeTrue();
                });
        }
        
        [Test]
        public void Assure_loadingNotifier_is_hidden_after_saving_data_is_complete()
        {
            Given(asyncUserdbPersisterMock_setup_to_return_savecomplete).
                And(db_with_3_users_and_controller_has_been_created);

            When(save_command_is_executed);

            Then(() =>
            {
                loadingNotifierMock.Verify(l => l.HideInView(), Times.Exactly(3));
                viewModel.IsSaving.ShouldBeFalse();
            });
        }
    } 

    public class Shared : ScenarioClass
    {
        protected static UsersController controller;
        protected static UsersViewModel viewModel;
        protected static IInvokeBackgroundWorker<IEnumerable<Userdb>> backgroundWorkerInvoker
            = new NoBackgroundWorkerInvocation<IEnumerable<Userdb>>();
        
        protected static Mock<IRepository<Userdb>> mockRepository = new Mock<IRepository<Userdb>>();
        protected static Mock<IPersistDomainModelsAsync<Userdb>> mockPersistDomainModels = new Mock<IPersistDomainModelsAsync<Userdb>>();
        protected static Mock<ILog> mockLog = new Mock<ILog>();
        protected static Mock<IProgressbar> loadingNotifierMock = new Mock<IProgressbar>();
        protected static Mock<IMetadataService> metadataServiceMock = new Mock<IMetadataService>();
        protected static Userdb persistedData = new Userdb();

        protected const String SETTINGS_ENTRY_NAME = "TeamMembers";
        protected const String FIRSTNAME_ENTRY_NAME = "firstnameIsChecked";
        protected const String SURNAME_ENTRY_NAME = "surnameIsChecked";
        protected const String MIDDLENAME_ENTRY_NAME = "middlenameIsChecked";
        protected const String USERNAME_ENTRY_NAME = "usernameIsChecked";

        protected Context controller_has_been_created = CreateController;
        protected Context db_with_0_users_has_been_created = () => InitializeUserdbs(0, 1);
        protected Context db_with_3_users_has_been_created = () => InitializeUserdbs(3, 1);
        protected Context there_are_three_dbs_with_0_users_in_repository = () => InitializeUserdbs(0, 3);
        protected Context db_has_been_updated_with_3_users = () => InitializeUserdbs(3, 1);
        protected Context db_with_0_users_and_controller_has_been_created = () => InitializeControllerAndDBs(0, 1);
        protected Context db_with_3_users_and_controller_has_been_created = () => InitializeControllerAndDBs(3, 1);
        protected Context currentDBName_is_changed_to_Default2 = () => viewModel.CurrentDBName = "Default2";
        protected Context there_are_no_dbs = () => InitializeUserdbs(0, 0);
        protected Context there_are_users_in_userDB_with_names = () =>
        {
            var userdbs = new List<Userdb>();
            var userdb = new Userdb("Test");
            userdb.AddUser(new User{ Firstname = "cnorris" });
            userdbs.Add(userdb);

            mockRepository.Setup(r => r.Get(It.IsAny<AllSpecification<Userdb>>())).Returns(userdbs);
        };
        protected Context there_is_something_wrong_with_the_userdbRepository = () => 
            mockRepository.Setup(r => r.Get(It.IsAny<AllSpecification<Userdb>>())).Throws(new Exception());

        protected Context asyncUserdbPersisterMock_setup_to_return_savecomplete = () =>
            mockPersistDomainModels.Setup(r => r.Save(It.IsAny<Userdb>())).
            Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

        protected Context repository_returnes_peristed_data = () => {
            mockRepository.Setup(
                r =>
                r.Get(It.IsAny<AllSpecification<Userdb>>()))
                .Returns(new List<Userdb>{persistedData}); 
        };

        protected Context asyncUserdbPersisterMock_saves_users = () => 
            mockPersistDomainModels.Setup(p => p.Save(It.IsAny<Userdb>())).Callback((Userdb userdb) =>
                    {
                        persistedData = userdb; 
                    });

        protected When controller_is_created = CreateController;
        protected When viewModel_saves = () => viewModel.Save.Execute(null);
        protected When CurrentDBName_is_changed = () => viewModel.CurrentDBName = "Default2";
        protected When a_new_user_is_created_with_the_same_name = () => AddNewUser("1");
        protected When a_new_user_is_created_with_a_new_name = () => AddNewUser("4");
        protected When loading_Data = () => viewModel.Refresh.ExecuteDelegate();
        protected When refresh_delegate_is_executed = () => viewModel.Refresh.Execute(null);
        protected When save_command_is_executed = () => viewModel.Save.Execute(null);
        protected When userRepository_throws_an_exception_under_update = () => 
            mockRepository.Setup(r => r.Get(It.IsAny<AllSpecification<Userdb>>())).Throws(new Exception());

        private static void CreateController()
        {
            viewModel = new UsersViewModel();

            controller = new UsersController(
                viewModel,
                mockRepository.Object,
                mockPersistDomainModels.Object,
                backgroundWorkerInvoker,
                mockLog.Object,
                new NoUIInvokation(),
                loadingNotifierMock.Object,
                metadataServiceMock.Object);
        }

        private static void InitializeControllerAndDBs(int numberOfUsers, int numberOfUserdbs)
        {
            InitializeUserdbs(numberOfUsers, numberOfUserdbs);
            CreateController();
        }

        protected static void InitializeUserdbs(int numberOfUsers, int numberOfUserdbs)
        {
            var listOfUserDbs = new List<Userdb>();

            for (var dbNum = 0; dbNum < numberOfUserdbs; dbNum++)
            {
                listOfUserDbs.Add(new Userdb("Default" + dbNum));

                for (var userNum = 0; userNum < numberOfUsers; userNum++)
                {
                    listOfUserDbs[dbNum].AddUser(new User(userNum.ToString()));
                }
            }
            mockRepository.Setup(r => r.Get(new AllSpecification<Userdb>())).Returns(listOfUserDbs);
        }

        private static void AddNewUser(string username)
        {
            viewModel.EditedUser = new UserViewModel
            {
                Username = username,
                Firstname = "test",
                ImageUrl = "test"
            };
            viewModel.AddUser.Execute(null);
        }

        [SetUp]
        public void SetUp()
        {
            Scenario("Controller is spawned");
            mockRepository = new Mock<IRepository<Userdb>>();
            mockPersistDomainModels = new Mock<IPersistDomainModelsAsync<Userdb>>();
            loadingNotifierMock = new Mock<IProgressbar>();

            metadataServiceMock.Setup(r => r.GetDeploymentUrl()).Returns(new Uri("http://localhost:1155/Smeedee/"));
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
