using System;
using System.Collections.Generic;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.TeamMembers.Controllers;
using Smeedee.Widget.TeamMembers.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Moq;

namespace Smeedee.Widget.TeamMembers.Test.Controllers
{
    [TestFixture]
    class When_controller_is_spawned : Shared
    {

        [Test]
        public void Assure_the_controller_works_with_no_UserDBs_exists()
        {
            Given(there_are_no_dbs);
            When(controller_is_created);

            Then("", () => viewModel.TeamMembers.Count.ShouldBe(0));
        }

        [Test]
        public void Assure_that_HasDatabases_is_true_when_there_are_dbs()
        {
            Given(db_with_0_users_and_controller_has_been_created);
            When("always");
            Then("", () => viewModel.HasDatabase.ShouldBeTrue());
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

            Then("team members should be added to the ViewModel",
                () => viewModel.TeamMembers.Count.ShouldBe(3));
        }

        [Test]
        public void Assure_correct_Userdb_name_is_set_on_settingsViewModel()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then("the CurrentDBName on viewModel should be set",
                () => settingsViewModel.CurrentDBName.ShouldBe("Default0"));
        }

        [Test]
        public void Assure_all_userdbs_names_are_fetched_and_set_on_settingsViewModel()
        {
            Given(there_are_three_dbs_with_0_users_in_repository);

            When(controller_is_created);

            Then(() => settingsViewModel.Userdbs.Count.ShouldBe(3));
        }

        [Test]
        public void Should_show_first_and_last_name_by_default_if_there_is_no_other_config()
        {
            Given(there_are_users_in_userDB_with_names).
                And(there_are_no_settings_in_the_repository);

            When(controller_is_created);

            Then("should display first and last name by default", () =>
            {
                viewModel.FirstnameIsChecked.ShouldBeTrue();
                viewModel.MiddlenameIsChecked.ShouldBeFalse();
                viewModel.SurnameIsChecked.ShouldBeTrue();
                viewModel.UsernameIsChecked.ShouldBeFalse();
                settingsViewModel.FirstnameIsChecked.ShouldBeTrue();
                settingsViewModel.MiddlenameIsChecked.ShouldBeFalse();
                settingsViewModel.SurnameIsChecked.ShouldBeTrue();
                settingsViewModel.UsernameIsChecked.ShouldBeFalse();
            });
        }

        [Test]
        public void Should_show_only_username_if_all_users_only_have_username()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then("only username should be visible", () =>
            {
                viewModel.FirstnameIsChecked.ShouldBeFalse();
                viewModel.MiddlenameIsChecked.ShouldBeFalse();
                viewModel.SurnameIsChecked.ShouldBeFalse();
                viewModel.UsernameIsChecked.ShouldBeTrue();
                settingsViewModel.FirstnameIsChecked.ShouldBeFalse();
                settingsViewModel.MiddlenameIsChecked.ShouldBeFalse();
                settingsViewModel.SurnameIsChecked.ShouldBeFalse();
                settingsViewModel.UsernameIsChecked.ShouldBeTrue();
            });
        }
    }

    [TestFixture]
    public class When_ViewModel_commands : Shared
    {
        [Test]
        public void Assure_save_command_results_in_persistance_of_settings()
        {
            Given(there_are_users_in_userDB_with_names).
                And(settings_in_repo_are_true_true_false_false).
                And(controller_has_been_created);

            When(viewModel_saves);

            Then("save should be performed on the persistConfiguration repository exactly once",
                () => mockPersistConfiguration.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
        }

        [Test]
        public void Assure_refresh_command_results_in_updating_the_team_members_in_the_viewModel()
        {
            Given(db_with_0_users_and_controller_has_been_created).
                And(db_has_been_updated_with_3_users).
                And(controller_is_finished_loading_data);

            When(viewModel_refreshes);

            Then("viewModel should reflect the changes",
                () => viewModel.TeamMembers.Count.ShouldBe(3));
        }

        [Test]
        public void Assure_that_refresh_command_results_in_updating_the_settings()
        {
            Given(there_are_users_in_userDB_with_names).
                And(controller_has_been_created).
                And(settings_in_repo_are_true_true_false_false).
                And(settings_is_changed_in_viewModel_to_false_false_true_true);

            When(refresh_delegate_is_executed);

            Then(() =>
                     {
                         viewModel.FirstnameIsChecked.ShouldBeTrue();
                         viewModel.SurnameIsChecked.ShouldBeTrue();
                         viewModel.MiddlenameIsChecked.ShouldBeFalse();
                         viewModel.UsernameIsChecked.ShouldBeFalse();
                         settingsViewModel.FirstnameIsChecked.ShouldBeTrue();
                         settingsViewModel.SurnameIsChecked.ShouldBeTrue();
                         settingsViewModel.MiddlenameIsChecked.ShouldBeFalse();
                         settingsViewModel.UsernameIsChecked.ShouldBeFalse();
                     });
        }

        [Test]
        public void Assure_the_correct_db_is_updated_on_the_ViewModel()
        {
            Given(three_dbs_with_0_users_and_controller_has_been_created).And(currentDBName_is_Default2);

            When(viewModel_refreshes);

            Then( () => settingsViewModel.CurrentDBName.ShouldBe("Default2"));
        }

        [Test]
        public void Assure_the_correct_amount_of_UsersDbs_is_updated_on_refresh()
        {
            Given(three_dbs_with_0_users_and_controller_has_been_created);

            When(viewModel_refreshes);

            Then(() => settingsViewModel.Userdbs.Count.ShouldBe(3));
        }
    }

    [TestFixture]
    public class Notify_when_loading : Shared
    {
       
        [Test]
        public void Assure_loadingNotifier_is_shown_while_loading()
        {
            Given(controller_has_been_created);
            When(loading_Data);
            Then("the loadingNotifier should be shown", 
                () => loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()),Times.AtLeastOnce()));
        }

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_loading()
        {
            Given("No controller spawned");
            When(controller_is_created);
            Then("the loadingNotifier should be hidden again", () =>
                {
                    loadingNotifierMock.Verify(l => l.HideInView(), Times.AtLeastOnce());
                    viewModel.IsLoading.ShouldBeFalse();
                });
        }

        
        [Test]
        public void Assure_saveNotifier_is_shown_while_saving_settings()
        {
            Given(controller_has_been_created);
            When(save_command_is_executed);
            Then(() => loadingNotifierMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.AtLeastOnce()));
        }

    }

    [TestFixture]
    public class When_notyfied_to_refresh : Shared
    {
        [Test]
        public void Assure_ViewModel_is_updated_on_notifiedToRefresh()
        {
            Given(db_with_0_users_and_controller_has_been_created).
                And(db_has_been_updated_with_3_users).
                And(controller_is_finished_loading_data);

            When(controller_is_notified_to_refresh);

            Then(() => viewModel.TeamMembers.Count.ShouldBe(3));
        }
        
    }

    [TestFixture]
    public class When_data_is_changed : Shared
    {
        [Test]
        public void Assure_list_of_userdbs_on_viewModel_is_updated_on_event_CurrentDBList()
        {
            Given(three_dbs_with_0_users_and_controller_has_been_created).
                And(controller_is_finished_loading_data);

            When(viewModelCurrentDBName_is_changed);

            Then("the repository should be asked for the all userdbs exactly twice",
                () => mockRepository.Verify(r => r.Get(It.IsAny<Specification<Userdb>>()), Times.Exactly(2)));
        }

        [Test]
        public void Assure_that_exceptions_from_repository_are_logged()
        {
            Given(userdbRepository_is_faulty);

            When(controller_is_created);

            Then("the exception should be logged",
                () => mockLog.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>())));
        }
        
    }

    [TestFixture]
    public class When_loading_and_reloading : Shared
    {
        [Test]
        public void Assure_selected_userdb_is_set_to_config_value()
        {
            Given(there_are_three_dbs_with_0_users_in_repository).
                And(configured_userdb_is_Default1);
            When(controller_is_created);
            Then("CurrentDBname should be set to configured value", () =>
            {
                settingsViewModel.CurrentDBName.ShouldBe("Default1");
            });
        }

        [Test]
        public void Assure_first_userdb_is_selected_if_configured_userdb_does_not_exist()
        {
           Given(there_are_three_dbs_with_0_users_in_repository).
               And(configured_userdb_is_Default5);
           When(controller_is_created);
           Then("CurrentDBname should be set to configured value", () =>
            {
                settingsViewModel.CurrentDBName.ShouldBe("Default0");
            });
        }

        [Test]
        [Ignore("Not implemented")]
        public void Should_inform_user_if_there_are_no_userdbs_or_users ()
        {
            Given(db_with_0_users_and_controller_has_been_created);
            When(controller_is_created);
            Then("user should be informed that there are no userdbs", ()=>
            {
                settingsViewModel.InformationField.ShouldBe("There are no users! Go to User Administration and add some!");                                                                              
            });
        }
        
        
        [Test]
        public void Assure_loadingNotifyer_is_shown_while_loading()
        {
            Given(db_with_3_users_has_been_created);
            When(controller_is_created);
            Then("the loadingNotifyer should be shown",
            () =>
                {
                    loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(1)); 
                });
        }


        [Test]
        public void Assure_loadingNotifier_is_hidden_after_loading_is_complete()
        {
            Given(db_with_3_users_has_been_created);

            When(controller_is_created);

            Then("", () =>
            {
                loadingNotifierMock.Verify(l => l.HideInView(), Times.Exactly(1));
                viewModel.IsLoading.ShouldBe(false);
            });
        }
       
        [Test]
        public void Assure_loadingNotifier_is_shown_while_saving()
        {
            Given(db_with_3_users_and_controller_has_been_created);
            When(save_command_is_executed);
            Then("the loadingNotifier should be shown",
                () =>
                    {
                        loadingNotifierMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.Exactly(2));
                        settingsViewModel.IsSaving.ShouldBeTrue();
                    });
        }
         

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_save_is_complete()
        {
            Given(db_with_3_users_and_controller_has_been_created).
                And(configPersisterMock_setup_to_return_savecomplete);

            When(save_command_is_executed);

            Then("", () =>
            {
                loadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.Exactly(2));
                settingsViewModel.IsSaving.ShouldBeFalse();
            });
        }

        [Test]
        public void Assure_loadingNotifier_is_shown_while_reloading_config()
        {
            Given(db_with_3_users_and_controller_has_been_created);
            When(refresh_delegate_is_executed);
            Then("the loadingNotifier should be shown",
                () =>
                {
                    loadingNotifierMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.Exactly(2));
                });
        }

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_reloade_config_is_complete()
        {
            Given(db_with_3_users_and_controller_has_been_created);

            When(refresh_delegate_is_executed);

            Then("", () =>
            {
                loadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.Exactly(2));
                settingsViewModel.IsSaving.ShouldBeFalse();
            });
        }
    }

    public class Shared : ScenarioClass
    {
        protected static TeamMembersController controller;
        protected static IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> BackgroundWorkerInvoker
            = new NoBackgroundWorkerInvocation<IEnumerable<ProjectInfoServer>>();
        protected static TeamMembersViewModel viewModel;
        protected static TeamMembersSettingsViewModel settingsViewModel;

        protected static Mock<ITimer> mockRefershTimer = new Mock<ITimer>();
        protected static Mock<IRepository<Userdb>> mockRepository = new Mock<IRepository<Userdb>>();
        protected static Mock<IPersistDomainModelsAsync<Configuration>> mockPersistConfiguration;
        protected static Mock<IRepository<Configuration>> configRepositoryMock;
        protected static Mock<ILog> mockLog = new Mock<ILog>();
        protected static Mock<IProgressbar> loadingNotifierMock = new Mock<IProgressbar>();
        protected static Mock<IMetadataService> metadataServiceMock = new Mock<IMetadataService>();

        protected const String SETTINGS_ENTRY_NAME = "TeamMembers";
        protected const String FIRSTNAME_ENTRY_NAME = "firstnameIsChecked";
        protected const String SURNAME_ENTRY_NAME = "surnameIsChecked";
        protected const String MIDDLENAME_ENTRY_NAME ="middlenameIsChecked";
        protected const String USERNAME_ENTRY_NAME = "usernameIsChecked";
        protected const String SELECTED_USERDB = "selectedUserdb";

        protected Context controller_has_been_created = CreateController;
        protected Context db_with_0_users_has_been_created = () => InitializeUserdbs(0, 1);
        protected Context db_with_3_users_has_been_created = () => InitializeUserdbs(3, 1);
        protected Context there_are_three_dbs_with_0_users_in_repository = () => InitializeUserdbs(0, 3);
        protected Context db_has_been_updated_with_3_users = () => InitializeUserdbs(3, 1);
        protected Context three_dbs_with_0_users_and_controller_has_been_created = () => InitializeControllerAndDBs(0, 3);
        protected Context db_with_0_users_and_controller_has_been_created = () => InitializeControllerAndDBs(0, 1);
        protected Context db_with_3_users_and_controller_has_been_created = () => InitializeControllerAndDBs(3, 1);
        protected Context currentDBName_is_Default2 = () => settingsViewModel.CurrentDBName = "Default2";
        protected Context there_are_no_dbs = () => InitializeUserdbs(0, 0);
        protected Context controller_is_finished_loading_data = () => { viewModel.IsLoading = false; };
        protected Context there_are_users_in_userDB_with_names = () =>
        {
            var Userdbs = new List<Userdb>();
            var Userdb = new Userdb("Test");
            Userdb.AddUser(new User(){Firstname = "cnorris"});
            Userdbs.Add(Userdb);

            mockRepository.Setup(r => r.Get(new AllSpecification<Userdb>())).Returns(Userdbs);
        };

        protected Context userdbRepository_is_faulty = () =>
        {
            mockRepository.Setup(r => r.Get(new AllSpecification<Userdb>())).Throws(new Exception());
        };

        protected Context settings_in_repo_are_true_true_false_false = () =>
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            configuration.NewSetting(FIRSTNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(SURNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(MIDDLENAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(USERNAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(SELECTED_USERDB, "default");

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context configured_userdb_is_Default1 = () =>
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            configuration.NewSetting(FIRSTNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(SURNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(MIDDLENAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(USERNAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(SELECTED_USERDB, "Default1");

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };


        protected Context configured_userdb_is_Default5 = () =>
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            configuration.NewSetting(FIRSTNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(SURNAME_ENTRY_NAME, true.ToString());
            configuration.NewSetting(MIDDLENAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(USERNAME_ENTRY_NAME, false.ToString());
            configuration.NewSetting(SELECTED_USERDB, "Default5");

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context there_are_no_settings_in_the_repository = () =>
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context settings_is_changed_in_viewModel_to_false_false_true_true = () =>
        {
            settingsViewModel.FirstnameIsChecked = false;
            settingsViewModel.SurnameIsChecked = false;
            settingsViewModel.MiddlenameIsChecked = true;
            settingsViewModel.UsernameIsChecked = true;
        };

        protected Context configPersisterMock_setup_to_return_savecomplete = () =>
        {
            mockPersistConfiguration.Setup(r => r.Save(It.IsAny<Configuration>())).Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());
        };

        protected When controller_is_created = CreateController;
        protected When controller_is_notified_to_refresh = NotifyControllerToRefresh;
        protected When viewModel_saves = () => settingsViewModel.Save.Execute(null);
        protected When viewModel_refreshes = NotifyControllerToRefresh;
        protected When viewModelCurrentDBName_is_changed = () => settingsViewModel.CurrentDBName = "Default2";
        protected When loading_Data = () => settingsViewModel.Refresh.ExecuteDelegate();
        protected When refresh_delegate_is_executed = () => settingsViewModel.Refresh.Execute(null);
        protected When save_command_is_executed = () => settingsViewModel.Save.Execute(null);

        private static void NotifyControllerToRefresh()
        {
            mockRefershTimer.Raise(n => n.Elapsed += null, new EventArgs());
        }

        private static void CreateController()
        {
            viewModel = new TeamMembersViewModel();
            settingsViewModel = new TeamMembersSettingsViewModel();
            
            controller = new TeamMembersController(
                viewModel,
                settingsViewModel,
                mockRepository.Object,
                configRepositoryMock.Object,
                mockPersistConfiguration.Object,
                BackgroundWorkerInvoker,
                mockLog.Object,
                mockRefershTimer.Object,
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

        [SetUp]
        public void SetUp()
        {
            Scenario("Controller is spawned");
            mockRepository = new Mock<IRepository<Userdb>>();
            mockPersistConfiguration = new Mock<IPersistDomainModelsAsync<Configuration>>();
            configRepositoryMock = new Mock<IRepository<Configuration>>();
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