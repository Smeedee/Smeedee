using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
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
                Then("an ArgumentNullException should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateChangesetControllerBase));
            }

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_changesetRepo_is_null()
            {
                Given("");
                When("creating new controller with null as timer", () => changesetRepoMock = null);
                Then("an ArgumentNullException should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateChangesetControllerBase));
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
            [Ignore("not completed")]
            public void Assure_something_when_eror()
            {
                Given(the_controller_has_been_created).
                    And(changesetRepository_BeginGet_completed_calls_GetCompleted_with_error);
                When(loadData_is_called);
                Then("isLoading should be true, and loadingNotifier should be shown", () =>
                {
                    myChangesetControllerBase.ViewModel.IsLoading.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInView());
                });
            }

           
            private Context changesetRepository_BeginGet_completed_calls_GetCompleted_without_error =
                () => changesetRepoMock.Setup(c => c.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Callback((Specification<Changeset> specs) => changesetRepoMock.
                        Raise(e => e.GetCompleted += null, new GetCompletedEventArgs<Changeset>(changesets, new AllChangesetsSpecification())));
            
            private static List<Changeset> changesets = smt();
            private static List<Changeset> smt()
            {
                var list = new List<Changeset>();
                list.Add(new Changeset() { Revision = 1, Time = DateTime.Today, Comment = "Repository created", Author = new Author() });
                list.Add(new Changeset() { Revision = 2, Time = DateTime.Today, Comment = "Added build script", Author = new Author() });
                list.Add(new Changeset() { Revision = 3, Time = DateTime.Today, Comment = "Added unit testing framework", Author = new Author() });
                return list;
            }

            private Context changesetRepository_BeginGet_completed_calls_GetCompleted_with_error =
                () => changesetRepoMock.Setup(c => c.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Callback((Specification<Changeset> specs) => changesetRepoMock.
                        Raise(e => e.GetCompleted += null, new GetCompletedEventArgs<Changeset>(changesets, new AllChangesetsSpecification())));

            private When loadData_is_called = () => myChangesetControllerBase.MyLoadData();
        }


        public class Shared : ScenarioClass
        {
            protected static MyChangeSetControllerBase myChangesetControllerBase;

            protected static BindableViewModel<AbstractViewModel> viewModel;
            protected static Mock<IAsyncRepository<Changeset>> changesetRepoMock;
            protected static Mock<ITimer> timerMock;
            protected static Mock<IUIInvoker> uiInvokeMock;
            protected static Mock<ILog> loggerMock;
            protected static Mock<IProgressbar> loadingNotifierMock;
            protected static Mock<IWidget> widgetMock;
            protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;

            protected Context the_controller_has_been_created = () => CreateChangesetControllerBase();


            
            //protected static void ChangesetRepositoryContains(IEnumerable<Changeset> changesets)
            //{
            //    changesetRepoMock.Setup(r => r.BeginGet(It.IsAny<Specification<Changeset>>())).
            //        Callback((Specification<Changeset> specs) => changesetRepoMock.Raise(e => e.GetCompleted += null,
            //                                                                                   new GetCompletedEventArgs<Changeset>(changesets.Where(specs.IsSatisfiedBy),
            //                                                                                                                        specs)));

            //    changesetRepoMock.Setup(r => r.BeginGet(It.IsAny<AllChangesetsSpecification>())).
            //        Raises(e => e.GetCompleted += null,
            //               new GetCompletedEventArgs<Changeset>(changesets, new AllChangesetsSpecification()));
            //}




























            protected static ILog GetLoggerObject()
            {
                return loggerMock != null ? loggerMock.Object : null;
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
                                                                          uiInvokeMock.Object,
                                                                          GetLoggerObject(),
                                                                          loadingNotifierMock.Object,
                                                                          widgetMock.Object,
                                                                          configPersisterMock.Object);
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");


                viewModel = new BindableViewModel<AbstractViewModel>();
                changesetRepoMock = new Mock<IAsyncRepository<Changeset>>();
                timerMock = new Mock<ITimer>();
                uiInvokeMock = new Mock<IUIInvoker>();
                loggerMock = new Mock<ILog>();
                loadingNotifierMock = new Mock<IProgressbar>();
                widgetMock = new Mock<IWidget>();
                configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();


                uiInvokeMock.Setup(s => s.Invoke(It.IsAny<Action>())).Callback((Action a) => a.Invoke());

                RemoveAllGlobalDependencies.ForAllViewModels();
                ConfigureGlobalDependencies.ForAllViewModels(
                    config =>
                    {
                        config.Bind<IPersistDomainModelsAsync<Configuration>>().
                            ToInstance(configPersisterMock.Object);
                        config.Bind<IUIInvoker>().ToInstance(uiInvokeMock.Object);
                        config.Bind<IAsyncRepository<Changeset>>().ToInstance(changesetRepoMock.Object);
                        config.Bind<ITimer>().ToInstance(timerMock);
                        config.Bind<ILog>().ToInstance(loggerMock);
                        config.Bind<IProgressbar>().ToInstance(loadingNotifierMock);
                        config.Bind<IWidget>().ToInstance(widgetMock);
                    });
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }

        public class MyChangeSetControllerBase : ChangesetControllerBase<AbstractViewModel>
        {
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
            }

            protected override void LoadDataIntoViewModel(IEnumerable<Changeset> qChangesets)
            {
                throw new NotImplementedException();
            }

            protected override void OnNotifiedToRefresh(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public void MyLoadData() { LoadData(); }
        }
    }
}
