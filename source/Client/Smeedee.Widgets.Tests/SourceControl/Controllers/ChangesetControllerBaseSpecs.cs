using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Widgets.SourceControl.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widgets.Tests.SourceControl.Controllers
{
    public class ChangesetControllerBaseSpecs
    {

        [TestFixture]
        public class When_creating_controllerbase : Shared
        {
            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_logger_is_null()
            {
                Given("");
                When("creating new controller with null as timer", () => loggerMock = null);
                Then("an ArgumentNullException should be thrown", 
                    () => this.ShouldThrowException<ArgumentNullException>(CreateChangesetControllerBase_with_null_logger));
            }

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_changesetRepo_is_null()
            {
                Given("");
                When("creating new controller with null as timer", () => changesetRepoMock = null);
                Then("an ArgumentNullException should be thrown", 
                    () => this.ShouldThrowException<ArgumentNullException>(CreateChangesetControllerBase));
            }
        }

        [TestFixture]
        public class When_loading_data : Shared
        {
            [Test]
            public void Assure_that_if_the_viewmodel_is_loading_nothing_happens()
            {
                Given(the_controller_has_been_created).
                    And("the viewmodel is already loading", () => myChangesetControllerBase.ViewModel.IsLoading = true);
                When(loadData_is_called);
                Then("nothing should happen", () =>
                {
                    loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Never());
                    changesetRepoMock.Verify(c => c.BeginGet(It.IsAny<Specification<Changeset>>()), Times.Never());
                });
            }

            [Test]
            public void Assure_loading_data_sets_isLoading_and_shows_loadingNotifier()
            {
                Given(the_controller_has_been_created);
                When(loadData_is_called);
                Then("isLoading should be true, and loadingNotifier should be shown", () =>
                {
                    myChangesetControllerBase.ViewModel.IsLoading.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()));
                });
            }

            [Test]
            public void Assure_loading_data_without_parameter_loads_new_specification()
            {
                Given(the_controller_has_been_created);
                When(loadData_is_called);
                Then("LoadData should load a new AllChangesetsSpecification", 
                    () => changesetRepoMock.Verify(c => c.BeginGet(new AllChangesetsSpecification())));
            }

            [Test]
            public void Assure_finished_loading_data_sets_isLoading_and_hides_loadingNotifier()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_without_error);
                When(loadData_is_called);
                Then("isLoading should be true, and loadingNotifier should be shown", () =>
                {
                    myChangesetControllerBase.ViewModel.IsLoading.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInView());
                });
            }

            [Test]
            public void Assure_hasConnectionProblems_set_and_error_is_logged_when_error()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_with_error);
                When(loadData_is_called);
                Then("hasConnectionProblems should be true, and the error should be logged", () =>
                {
                    myChangesetControllerBase.ViewModel.HasConnectionProblems.ShouldBeTrue();
                    LogEntryMockPersister.entries.Count.ShouldBe(1);
                    LogEntryMockPersister.entries[0].ShouldBeInstanceOfType<LogEntry>();
                    
                });
            }

           
            [Test]
            public void Assure_correct_errorMessage_when_onGetCompleted_gets_empty_result()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_with_empty_parameter);
                When(loadData_is_called);
                Then("hasConnectionProblems should be true, and the error should be logged", () =>
                {
                    exceptionMessage = LogEntryMockPersister.entries[0].Message.Remove(104);
                    wantedException = new Exception("Violation of IChangesetRepository. Does not accept a null reference as a return value.");

                    myChangesetControllerBase.ViewModel.HasConnectionProblems.ShouldBeTrue();
                    LogEntryMockPersister.entries.Count.ShouldBe(1);

                    exceptionMessage.ShouldBe(wantedException.ToString());
                });
            }
            
            [Test]
            public void Assure_no_errorMessage_if_no_errors_or_empty_parameters()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_without_error);
                When(loadData_is_called);
                Then("hasConnectionProblems should be false, and no error should be logged", () =>
                {
                    myChangesetControllerBase.ViewModel.HasConnectionProblems.ShouldBeFalse();
                    LogEntryMockPersister.entries.Count.ShouldBe(0);
                });
            }

            [Test]
            public void Assure_error_is_caught_if_loading_data_into_viewmodel_gets_exception()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_without_error).
                    And(LoadDataIntoViewModel_throws_exception);
                When(loadData_is_called);
                Then("we should still get hasConnectionProblems true, and error", () =>
                {
                    exceptionMessage = LogEntryMockPersister.entries[0].Message.Remove(46);
                    wantedException = new Exception("LoadDataIntoViewModel failed");
                    
                    myChangesetControllerBase.ViewModel.HasConnectionProblems.ShouldBeTrue();
                    LogEntryMockPersister.entries.Count.ShouldBe(1);
                    exceptionMessage.ShouldBe(wantedException.ToString());
                });
            }

           
            private Context changesetRepository_BeginGet_completed_calls_GetCompleted_without_error =
                () => changesetRepoMock.Setup(c => c.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Raises(e => e.GetCompleted += null, new GetCompletedEventArgs<Changeset>(changesets, new AllChangesetsSpecification()));
            
            private static List<Changeset> changesets = CreateChangesets();

            private Context changesetRepository_BeginGet_completed_calls_GetCompleted_with_error =
                () => changesetRepoMock.Setup(c => c.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Callback((Specification<Changeset> specs) => changesetRepoMock.
                    Raise(e => e.GetCompleted += null, new GetCompletedEventArgs<Changeset>(new AllChangesetsSpecification(), new Exception("Error!"))));

            private Context changesetRepository_BeginGet_completed_calls_GetCompleted_with_empty_parameter =
               () => changesetRepoMock.Setup(c => c.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Raises(e => e.GetCompleted += null, getCompletedEventArgs);
            private static GetCompletedEventArgs<Changeset> getCompletedEventArgs = new GetCompletedEventArgs<Changeset>(null, new AllChangesetsSpecification());

            private When loadData_is_called = () => myChangesetControllerBase.MyLoadData();

            private Context LoadDataIntoViewModel_throws_exception =
                () => myChangesetControllerBase.LoadDataIntoViewModelOk = false;
        }

        [TestFixture]
        public class When_updating_revision : Shared
        {
            [Test]
            public void Assure_returns_if_changesets_are_null()
            {
                Given(the_controller_has_been_created).
                    And("changesets is null", () => changesets = null);
                When(updating_revision);
                Then("return without changing the revision", 
                    () => myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(0));
            }

            [Test]
            public void Assure_returns_if_changesets_count_is_null()
            {
                Given(the_controller_has_been_created).
                   And("changesets is empty", () => changesets = new List<Changeset>());
                When(updating_revision);
                Then("return without changing the revision", () =>
                {
                    changesets.ShouldNotBeNull();
                    myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(0);
                });
            }

            [Test]
            public void Assure_sets_currentRevision_when_there_are_changesets()
            {
                Given(the_controller_has_been_created).
                    And(there_are_changesets);
                When(updating_revision);
                Then("revision should be changed", () => myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(3));
            }

            [Test]
            public void Assure_sets_currentRevision_when_there_are_changesets_and_currentRevision_set_to_low_number()
            {
                Given(the_controller_has_been_created).
                    And(there_are_changesets).
                    And("currentRevision is set to low number", 
                    () => myChangesetControllerBase.ViewModel.CurrentRevision=1);
                When(updating_revision);
                Then("revision should be changed", 
                    () => myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(3));
            }

            [Test]
            public void Assure_doesnt_change_currentRevision_if_currentRevision_is_larger_than_new()
            {
                Given(the_controller_has_been_created).
                   And(there_are_changesets).
                   And("currentRevision is set to large number", 
                   () => myChangesetControllerBase.ViewModel.CurrentRevision = 5);
                When(updating_revision);
                Then("revision should be changed", 
                    () => myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(5));
            }

            [Test]
            public void Assure_changes_currentRevision_when_config_is_changed_and_currentRevision_is_larger_than_new()
            {
                Given(the_controller_has_been_created).
                   And(there_are_changesets).
                   And("currentRevision is set to large number",
                   () => myChangesetControllerBase.ViewModel.CurrentRevision = 5).
                   And("config is changed", () => myChangesetControllerBase.MyConfigIsChanged());
                When(updating_revision);
                Then("revision should be changed",
                    () => myChangesetControllerBase.ViewModel.CurrentRevision.ShouldBe(3));
            }

            private static IEnumerable<Changeset> changesets;

            private Context there_are_changesets = () => changesets = CreateChangesets();
            private When updating_revision = () => myChangesetControllerBase.MyUpdateRevision(changesets);
        }

        [TestFixture]
        public class When_writing_log_entry : Shared
        {
            [Test]
            public void Assure_logging_error_message_works_correctly()
            {
                Given(the_controller_has_been_created);
                When("we want to log an error message", () => myChangesetControllerBase.MyLogErrorMsg(new Exception("aException")));
                Then("the error message should be logged correctly", () =>
                {
                    wantedException = new Exception("aException");

                    LogEntryMockPersister.entries.Count.ShouldBe(1);
                    LogEntryMockPersister.entries[0].Message.ShouldBe(wantedException.ToString());
                });
            }

            [Test]
            [Ignore ("this doesnt work because the VerbosityLevel is set to 0, and therefore only logs ErrorMessages")]
            public void Assure_logging_warning_message_works_correctly()
            {
                Given(the_controller_has_been_created);
                When("we want to log a warning message", () => myChangesetControllerBase.MyLogWarningMsg(new Exception("a warning exception")));
                Then("the warning message should be logged correctly", () =>
                {
                    wantedException = new Exception("a warning exception");

                    LogEntryMockPersister.entries.Count.ShouldBe(1);
                    LogEntryMockPersister.entries[0].Message.ShouldBe(wantedException.ToString());
                });
            }
        }


        public class Shared : ScenarioClass
        {
            protected static MyChangeSetControllerBase myChangesetControllerBase;

            protected static BindableViewModel<AbstractViewModel> viewModel;
            protected static Mock<IAsyncRepository<Changeset>> changesetRepoMock;
            protected static Mock<ITimer> timerMock;
            protected static IUIInvoker uiInvoker;

            protected static LogEntryMockPersister loggerMock;

            protected static Mock<IProgressbar> loadingNotifierMock;
            protected static Mock<IWidget> widgetMock;
            protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;

            protected string exceptionMessage;
            protected static Exception wantedException;

            protected Context the_controller_has_been_created = () => CreateChangesetControllerBase();


            protected static ILog GetLoggerObject()
            {
                return new Logger(loggerMock) ?? null;
            }

            protected static IAsyncRepository<Changeset> GetChangesetRepoObject()
            {
                return changesetRepoMock != null ? changesetRepoMock.Object : null;
            }

            public static void CreateChangesetControllerBase()
            {
                myChangesetControllerBase = new MyChangeSetControllerBase(viewModel,
                                                                          GetChangesetRepoObject(),
                                                                          timerMock.Object,
                                                                          uiInvoker,
                                                                          GetLoggerObject(),
                                                                          loadingNotifierMock.Object,
                                                                          widgetMock.Object,
                                                                          configPersisterMock.Object);
            }

            public static void CreateChangesetControllerBase_with_null_logger()
            {
                myChangesetControllerBase = new MyChangeSetControllerBase(viewModel,
                                                                          GetChangesetRepoObject(),
                                                                          timerMock.Object,
                                                                          uiInvoker,
                                                                          null,
                                                                          loadingNotifierMock.Object,
                                                                          widgetMock.Object,
                                                                          configPersisterMock.Object);
            }

            public static List<Changeset> CreateChangesets()
            {
                var list = new List<Changeset>();
                list.Add(new Changeset() { Revision = 1, Time = DateTime.Today, Comment = "Repository created", Author = new Author() });
                list.Add(new Changeset() { Revision = 2, Time = DateTime.Today, Comment = "Added build script", Author = new Author() });
                list.Add(new Changeset() { Revision = 3, Time = DateTime.Today, Comment = "Added unit testing framework", Author = new Author() });
                return list;
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");

                viewModel = new BindableViewModel<AbstractViewModel>();
                changesetRepoMock = new Mock<IAsyncRepository<Changeset>>();
                timerMock = new Mock<ITimer>();
                uiInvoker = new NoUIInvokation();

                loggerMock = new LogEntryMockPersister();

                loadingNotifierMock = new Mock<IProgressbar>();
                widgetMock = new Mock<IWidget>();
                configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }

        public class MyChangeSetControllerBase : ChangesetControllerBase<AbstractViewModel>
        {
            public bool LoadDataIntoViewModelOk;

            public MyChangeSetControllerBase(BindableViewModel<AbstractViewModel> viewModel,
                                             IAsyncRepository<Changeset> changesetRepo,
                                             ITimer timer,
                                             IUIInvoker uiInvoke,
                                             ILog logger,
                                             IProgressbar loadingNotifier,
                                             IWidget widget,
                                             IPersistDomainModelsAsync<Configuration> configPersister)
                : base(viewModel, changesetRepo, timer, uiInvoke, logger, loadingNotifier, widget, configPersister)
            {
                LoadDataIntoViewModelOk = true;
            }

            protected override void LoadDataIntoViewModel(IEnumerable<Changeset> qChangesets)
            {
                if (!LoadDataIntoViewModelOk)
                    throw new Exception("LoadDataIntoViewModel failed");
            }


            protected override void OnNotifiedToRefresh(object sender, EventArgs e) {}

            public void MyLoadData() { LoadData(); }

            public void MyUpdateRevision(IEnumerable<Changeset> changesets) { UpdateRevision(changesets); }

            public void MyConfigIsChanged() { configIsChanged = true; }

            public void MyLogErrorMsg(Exception exception) { LogErrorMsg(exception); }

            public void MyLogWarningMsg(Exception exception) { LogWarningMsg(exception); }
        }

        public class LogEntryMockPersister : IPersistDomainModelsAsync<LogEntry>
        {
            public static List<LogEntry> entries;

            public LogEntryMockPersister()
            {
                entries = new List<LogEntry>();
            }

            public void Save(LogEntry domainModel)
            {
                entries.Add(domainModel);
            }

            public void Save(IEnumerable<LogEntry> domainModels)
            {
                entries.AddRange(domainModels);
            }

            public event EventHandler<SaveCompletedEventArgs> SaveCompleted;
        }
    }
}
