using System;
using System.ComponentModel;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Widgets.SourceControl.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests.Controller
{
    class ControllerBaseSpecs
    {
        [TestFixture]
        public class When_creating_controllerbase : Shared
        {

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_timer_is_null()
            {
                Given("");
                When("creating new controller with null as timer", () => refreshNotifierMock = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateControllerBase));
            }

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_uiInvoker_is_null()
            {
                Given("");
                When("creating new controller with null as uiInvoker", () => uiInvokerMock = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateControllerBase));
            }

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_viewmodel_is_null()
            {
                Given("");
                When("creating new controller with null as viewmodel", () => viewModel = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateControllerBase));
            }

            [Test]
            public void Assure_exception_is_thrown_when_constructor_is_called_and_loadingNotifier_is_null()
            {
                Given("");
                When("creating new controller with null as loadingNotifier", () => loadingNotifierMock = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateControllerBase));
            }
        }

        [TestFixture]
        public class When_saving_config : Shared
        {

            [Test]
            public void Assure_saving_config_sets_isSaving_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When(save_is_called);
                Then(() =>
                {
                    myControllerBase.ViewModel.IsSaving.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInSettingsView("Saving configuration..."));
                });
            }

            [Test]
            public void Assure_config_save_completed_sets_isSaving_and_hides_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created).
                    And("IsSaving is true", () => myControllerBase.ViewModel.IsSaving = true).
                    And(configPersisterMock_setup_to_return_savecomplete_without_error);
                When(save_is_called);
                Then(() =>
                {
                    myControllerBase.ViewModel.IsSaving.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.AtLeastOnce());
                });
            }

            [Test]
            public void Assure_config_save_completed_shows_errormessage_if_error()
            {
                Given(the_controllerbase_has_been_created).
                    And(configPersisterMock_setup_to_return_savecomplete_with_error);
                When(save_is_called);
                Then(() => widgetMock.Verify(w => w.ShowErrorMessage("Failed to save settings :(")));
            }

            [Test]
            public void Assure_config_save_completed_sets_noErrors_if_no_error()
            {
                Given(the_controllerbase_has_been_created).
                    And(configPersisterMock_setup_to_return_savecomplete_without_error);
                When(save_is_called);
                Then(() => widgetMock.Verify(w => w.NoErrors()));
            }


            private Context configPersisterMock_setup_to_return_savecomplete_without_error =
                () => configPersisterMock.Setup(r => r.Save(It.IsAny<Configuration>())).Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

            private Context configPersisterMock_setup_to_return_savecomplete_with_error =
               () => configPersisterMock.Setup(r => r.Save(It.IsAny<Configuration>())).Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs(new Exception { HelpLink = "failhelp" }));

            private When save_is_called = () => myControllerBase.MySaveConfiguration();
            
        }

        [TestFixture]
        public class When_starting_and_stopping_refreshNotifier : Shared
        {
            private int REFRESH_INTERVAL = 1 * 60 * 1000;

            [Test]
            public void Assure_start_starts_the_refreshNotifier_with_set_refresh_interval()
            {
                Given(the_controllerbase_has_been_created);
                When("start is called", () => myControllerBase.Start());
                Then("start should start", () => refreshNotifierMock.Verify(r => r.Start(REFRESH_INTERVAL)));
            }

            [Test]
            public void Assure_stop_stops_the_refreshNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("stop is called", () => myControllerBase.Stop());
                Then("start should start", () => refreshNotifierMock.Verify(r => r.Stop()));
            }

            [Test]
            public void Assure_subcribes_to_timer_elapsed()
            {
                Given(the_controllerbase_has_been_created);
                When("refresh is called", () => refreshNotifierMock.Raise(t => t.Elapsed += null, EventArgs.Empty));
                Then("OnNotifyToRefresh should be called", () => myControllerBase.WasNotified.ShouldBeTrue());
            }
        }

        [TestFixture]
        public class When_ToggleRefreshInSettingsMode_is_called : Shared
        {
            [Test]
            public void Assure_nothing_happens_if_conditions_are_not_met()
            {
                Given(the_controllerbase_has_been_created).
                    And(sender_is_not_widget_and_propertyName_not_IsInSettingsMode);
                When(ToggleRefreshInSettingsMode_is_called);
                Then(() =>
                 {
                     refreshNotifierMock.Verify(r => r.Start(It.IsAny<int>()), Times.Never());
                     refreshNotifierMock.Verify(r => r.Stop(), Times.Never());
                 });
            }

            [Test]
            public void Assure_refreshNotifier_is_stopped_if_conditions_are_met_and_widget_is_in_settingsMode()
            {
                Given(the_controllerbase_has_been_created).
                    And(sender_widget_and_propertyName_is_IsInSettingsMode_and_widget_is_in_settingsmode);
                When(ToggleRefreshInSettingsMode_is_called);
                Then(() =>
                {
                    refreshNotifierMock.Verify(r => r.Start(It.IsAny<int>()), Times.Never());
                    refreshNotifierMock.Verify(r => r.Stop(), Times.Once());
                });
            }

            [Test]
            public void Assure_refreshNotifier_is_started_if_conditions_are_met_and_widget_is_not_in_settingsMode()
            {
                Given(the_controllerbase_has_been_created).
                    And(sender_widget_and_propertyName_is_IsInSettingsMode_and_widget_is_not_in_settingsmode);
                When(ToggleRefreshInSettingsMode_is_called);
                Then(() =>
                {
                    refreshNotifierMock.Verify(r => r.Start(It.IsAny<int>()), Times.Once());
                    refreshNotifierMock.Verify(r => r.Stop(), Times.Never());
                });
            }


            private static object sender;
            private static PropertyChangedEventArgs e;

            private Context sender_is_not_widget_and_propertyName_not_IsInSettingsMode = () =>
            {
                sender = new AbstractViewModel();
                e = new PropertyChangedEventArgs("aName");
            };

            private Context sender_widget_and_propertyName_is_IsInSettingsMode_and_widget_is_in_settingsmode = () =>
            {
                sender = new Widget();
                e = new PropertyChangedEventArgs("IsInSettingsMode");
                ((Widget)sender).IsInSettingsMode = true;
            };

            private Context sender_widget_and_propertyName_is_IsInSettingsMode_and_widget_is_not_in_settingsmode = () =>
            {
                sender = new Widget();
                e = new PropertyChangedEventArgs("IsInSettingsMode");
                ((Widget)sender).IsInSettingsMode = false;
            };

            private When ToggleRefreshInSettingsMode_is_called = () => myControllerBase.ToggleRefreshInSettingsMode(sender, e);
        }

        [TestFixture]
        public class When_calling_loading_and_saving_methods : Shared
        {
            [Test]
            public void Assure_SetIsSavingData_sets_isSavingData_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsSavingData is called", () => myControllerBase.MySetIsSavingData());
                Then(() =>
                         {
                             myControllerBase.ViewModel.IsSaving.ShouldBeTrue();
                             loadingNotifierMock.Verify(l => l.ShowInView("Saving data..."));
                         });
            }

            [Test]
            public void Assure_SetIsNotSavingData_sets_isSavingData_and_hides_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created).
                    And("IsSaving is set to true", () => myControllerBase.ViewModel.IsSaving = true);
                When("SetIsNotSavingData is called", () => myControllerBase.MySetIsNotSavingData());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsSaving.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInView());
                });
            }

            [Test]
            public void Assure_SetIsLoadingRealData_sets_isLoading_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsLoadingRealData is called", () => myControllerBase.MySetIsLoadingRealData());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsLoading.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<String>()));
                    loadingNotifierMock.Verify(l => l.ShowInBothViews("The default configuration is now loaded. We are still trying to load the real configuration"));
                });
            }

            [Test]
            public void Assure_SetIsLoadingData_sets_isLoading_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsLoadingData is called", () => myControllerBase.MySetIsLoadingData());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsLoading.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()));
                    loadingNotifierMock.Verify(l => l.ShowInView("Loading data from server..."));
                });
            }

            [Test]
            public void Assure_SetIsNotLoadingData_sets_isLoading_and_hides_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsNotLoadingData is called", () => myControllerBase.MySetIsNotLoadingData());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsLoading.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInView());
                });
            }

            [Test]
            public void Assure_SetIsSavingConfig_sets_isSaving_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsSavingConfig is called", () => myControllerBase.MySetIsSavingConfig());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsSaving.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()));
                    loadingNotifierMock.Verify(l => l.ShowInSettingsView("Saving configuration..."));
                });
            }

            [Test]
            public void Assure_SetIsNotSavingConfig_sets_isSaving_and_hides_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsNotSavingConfig is called", () => myControllerBase.MySetIsNotSavingConfig());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsSaving.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInSettingsView());
                });
            }

            [Test]
            public void Assure_SetIsLoadingConfig_sets_isLoading_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsLoadingConfig is called", () => myControllerBase.MySetIsLoadingConfig());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsLoadingConfig.ShouldBeTrue();
                    loadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()));
                    loadingNotifierMock.Verify(l => l.ShowInBothViews("Loading configuration from server..."));
                });
            }

            [Test]
            public void Assure_SetIsNotLoadingConfig_sets_isLoading_and_shows_loadingNotifier()
            {
                Given(the_controllerbase_has_been_created);
                When("SetIsNotLoadingConfig is called", () => myControllerBase.MySetIsNotLoadingConfig());
                Then(() =>
                {
                    myControllerBase.ViewModel.IsLoadingConfig.ShouldBeFalse();
                    loadingNotifierMock.Verify(l => l.HideInBothViews());
                });
            }
        }


        public class Shared : ScenarioClass
        {
            protected static AbstractViewModel viewModel;

            protected static Mock<IWidget> widgetMock;
            protected static Mock<ITimer> refreshNotifierMock;
            protected static Mock<IUIInvoker> uiInvokerMock;
            protected static Mock<IProgressbar> loadingNotifierMock;
            protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;

            protected static MyControllerBase myControllerBase;

            protected Context the_controllerbase_has_been_created = () => CreateControllerBase();

            protected static ITimer GetTimerObject() { return refreshNotifierMock != null ? refreshNotifierMock.Object : null; }

            protected static IUIInvoker GetUiInvokerObject() { return uiInvokerMock != null ? uiInvokerMock.Object : null; }

            protected static IProgressbar GetLoadingNotifierObject() { return loadingNotifierMock != null ? loadingNotifierMock.Object : null; }

            protected static void CreateControllerBase()
            {
                myControllerBase = new MyControllerBase(viewModel,
                    GetTimerObject(),
                    GetUiInvokerObject(),
                    GetLoadingNotifierObject(),
                    widgetMock.Object,
                    configPersisterMock.Object);
            }



            [SetUp]
            public void Setup()
            {
                configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
                uiInvokerMock = new Mock<IUIInvoker>();

                RemoveAllGlobalDependencies.ForAllViewModels();
                ConfigureGlobalDependencies.ForAllViewModels(
                    config =>
                        {
                            config.Bind<IPersistDomainModelsAsync<Configuration>>().
                                ToInstance(configPersisterMock.Object);
                            config.Bind<IUIInvoker>().ToInstance(uiInvokerMock.Object);
                        });

                widgetMock = new Mock<IWidget>();
               
                Scenario("");

                viewModel = new AbstractViewModel();
                refreshNotifierMock = new Mock<ITimer>();
                
                loadingNotifierMock = new Mock<IProgressbar>();

                uiInvokerMock.Setup(s => s.Invoke(It.IsAny<Action>())).Callback((Action a) => a.Invoke());

               

            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }

        public class MyControllerBase : ControllerBase<AbstractViewModel>
        {
            public bool WasNotified;

            public MyControllerBase(AbstractViewModel viewModel, ITimer timer, IUIInvoker uiInvoker, IProgressbar loadingNotifier, IWidget widget, IPersistDomainModelsAsync<Configuration> configPersister)
                : base(viewModel, timer, uiInvoker, loadingNotifier, widget, configPersister)
            {
                WasNotified = false;
            }

            public void MySaveConfiguration() { SaveConfiguration(); }

            public void MySetIsSavingData() { SetIsSavingData(); }

            public void MySetIsNotSavingData() { SetIsNotSavingData(); }

            public void MySetIsLoadingRealData() { SetIsLoadingRealData(); }

            public void MySetIsLoadingData() { SetIsLoadingData(); }

            public void MySetIsNotLoadingData() { SetIsNotLoadingData(); }

            public void MySetIsSavingConfig() { SetIsSavingConfig(); }

            public void MySetIsNotSavingConfig() { SetIsNotSavingConfig(); }

            public void MySetIsLoadingConfig() { SetIsLoadingConfig(); }

            public void MySetIsNotLoadingConfig() { SetIsNotLoadingConfig(); }

            public void MyConfigurationChanged(object sender, EventArgs e)
        {
            OnConfigurationChanged(Widget.Configuration);
        }


            protected override void OnNotifiedToRefresh(object sender, EventArgs e)
            {
                WasNotified = true;
            }
        }
    }
}
