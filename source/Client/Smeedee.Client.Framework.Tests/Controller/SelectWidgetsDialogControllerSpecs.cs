using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests.Controller.SelectWidgetsDialogControllerTests
{
    public class SelectWidgetsDialogControllerSpecs
    {
        [TestFixture]
        public class When_spawning : Shared
        {
            [Test]
            public void Assure_viewmodel_is_validated()
            {
                Given(ViewModelRepository_is_created).
                    And(SlideConfigPersister_is_created);
                When("spawning");
                Then(()=>this.ShouldThrowException<ArgumentNullException>(() =>
                                                                 new SelectWidgetsDialogController(null,
                                                                                                   viewModelRepositoryMock
                                                                                                       .Object, slideConfigPersiterMock.Object)));
            }

            [Test]
            public void Assure_persister_is_validated()
            {
                Given(ViewModelRepository_is_created).And(ViewModel_is_created);
                When("spawning");
                Then(()=> this.ShouldThrowException<ArgumentNullException>( () => new SelectWidgetsDialogController(viewModel, viewModelRepositoryMock.Object, null)));
            }

            [Test]
            public void Assure_wigdetMetadataRepository_is_validated()
            {
                Given(ViewModel_is_created).
                    And(SlideConfigPersister_is_created);
                When("spawning");
                Then(() =>
                     this.ShouldThrowException<ArgumentNullException>(() =>
                                                                      new SelectWidgetsDialogController(viewModel, null, slideConfigPersiterMock.Object)));
            }
        }

        [TestFixture]
        public class When_spawned : Shared
        {
            [Test]
            public void Assure_AvailableWidgets_in_viewmodel_is_populated()
            {
                Given(ViewModel_is_created);
                And(ViewModelRepository_is_created);
                And(ViewModelRepository_contains_data);
                And(SlideConfigPersister_is_created);

                When(Controller_is_spawned);

                Then(() => { viewModel.AvailableWidgets.Count().ShouldNotBe(0); });
            }
        }

        [TestFixture]
        public class When_loading_data : Shared
        {
            [SetUp]
            public void Setup()
            {
                Given(ViewModel_is_created);
                And(ViewModelRepository_is_created);
                And(SlideConfigPersister_is_created);
                And(Controller_is_created);

                When("Loading data");
            }

            [Test]
            public void Assure_progressBar_is_visible()
            {
                Then(() => viewModel.Progressbar.IsVisible.ShouldBeTrue());
            }
        }

        [TestFixture]
        public class When_data_is_loaded : Shared
        {
            [SetUp]
            public void Setup()
            {
                Given(ViewModel_is_created);
                And(ViewModelRepository_is_created);
                And(SlideConfigPersister_is_created);
                And(Controller_is_created);

                When("data is loaded", () =>
                    viewModelRepositoryMock.Raise(r => r.GetCompleted += null, 
                    new GetCompletedEventArgs<WidgetMetadata>(new List<WidgetMetadata>(), All.ItemsOf<WidgetMetadata>())));
            }

            [Test]
            public void Assure_progressBar_is_set_to_invisible_when_completed()
            {
                Then(() => viewModel.Progressbar.IsVisible.ShouldBeFalse());
            }
        }

        [TestFixture]
        public class when_ok_command_is_executed : Shared
        {
            private bool dialogWasClosed;

            [SetUp]
            public void Setup()
            {
                Given(ViewModel_is_created).
                And(ViewModelRepository_is_created).
                And(SlideConfigPersister_is_created).
                And(ViewModelRepository_contains_data).
                And(Controller_is_created).And("test is setup", () =>
                {
                    dialogWasClosed = false;
                    viewModel.CloseDialog += (o, e) => dialogWasClosed = true;
                });

                When("Ok Command is executed", () => viewModel.Ok.Execute(null));
            }


            [Test]
            public void Assure_selected_widgets_are_persisted()
            {

                Then("Two new slideconfigurations should be saved", () =>
                     {
                         slideConfigPersiterMock.Verify(p => p.Save(It.Is<SlideConfiguration>(s => s.Title == firstSelectedWidgetMetadata.UserSelectedTitle)), Times.AtLeastOnce());
                         slideConfigPersiterMock.Verify(p => p.Save(It.Is<SlideConfiguration>(s => s.Title == secondSelectedWidgetMetadata.UserSelectedTitle)), Times.AtLeastOnce());
                     });
            }

            [Test]
            public void Assure_slides_are_created()
            {
                Then(() =>
                {
                    viewModel.NewSlides.Count.ShouldBe(2);
                    viewModel.NewSlides.ElementAt(0).Title.ShouldBe(firstSelectedWidgetMetadata.Name);
                    viewModel.NewSlides.ElementAt(0).SecondsOnScreen.ShouldBe(firstSelectedWidgetMetadata.SecondsOnScreen);
                    viewModel.NewSlides.ElementAt(0).Widget.GetType().ShouldBe(firstSelectedWidgetMetadata.Type);
                });
            }

            [Test]
            public void Assure_dialog_is_closed_when_the_configurations_are_all_saved()
            {
                Then(() =>
                {
                    dialogWasClosed.ShouldBeFalse();
                    slideConfigPersiterMock.Raise(p => p.SaveCompleted += null, new SaveCompletedEventArgs());
                    dialogWasClosed.ShouldBeFalse();
                    slideConfigPersiterMock.Raise(p => p.SaveCompleted += null, new SaveCompletedEventArgs());
                    dialogWasClosed.ShouldBeTrue();

                });
            }
        }

        [TestFixture]
        public class when_ok_command_is_executed_and_no_slides_are_selected : Shared
        {
            private bool dialogWasClosed;
            private bool progressbarWasShown;

            [SetUp]
            public void Setup()
            {
                Given(ViewModel_is_created).
                And(ViewModelRepository_is_created).
                And(SlideConfigPersister_is_created).
                And(No_slides_are_selected).
                And(Controller_is_created).And("test is setup", () =>
                {
                    dialogWasClosed = false;
                    viewModel.CloseDialog += (o, e) => dialogWasClosed = true;

                    progressbarWasShown = false;
                    viewModel.Progressbar.PropertyChanged += (o,e) =>
                    {
                        if (viewModel.Progressbar.IsVisible)
                            progressbarWasShown = true;
                    } ;
                });

                When("Ok Command is executed", () => viewModel.Ok.Execute(null));
            }

            [Test]
            public void Assure_dialog_is_closed_when_no_slides_are_selected()
            {
                Then(() => dialogWasClosed.ShouldBe(true));
            }

            [Test]
            public void Assure_progresbar_is_not_shown()
            {
                Then(() => progressbarWasShown.ShouldBeFalse());
            }
        }

        public class Shared : ScenarioClass
        {
            public Shared()
            {
                ViewModelBootstrapperForTests.Initialize();
            }

            protected static SelectWidgetsDialog viewModel;
            protected static SelectWidgetsDialogController controller;
            protected static Mock<IAsyncRepository<WidgetMetadata>> viewModelRepositoryMock;
            protected static Mock<IPersistDomainModelsAsync<SlideConfiguration>> slideConfigPersiterMock;
            protected static WidgetMetadata firstSelectedWidgetMetadata;
            protected static List<WidgetMetadata> widgetMetadatas = new List<WidgetMetadata>();
            protected static WidgetMetadata secondSelectedWidgetMetadata;
            protected static WidgetMetadata thirdSelectedWidgetMetadata;

            protected Context ViewModel_is_created = () =>
            {
            	viewModel = new SelectWidgetsDialog();
            };

            protected Context ViewModelRepository_is_created =
                () => { viewModelRepositoryMock = new Mock<IAsyncRepository<WidgetMetadata>>(); };

            protected Context SlideConfigPersister_is_created =
                () => { slideConfigPersiterMock = new Mock<IPersistDomainModelsAsync<SlideConfiguration>>(); };

            protected Context ViewModelRepository_contains_data = () =>
            {
                firstSelectedWidgetMetadata = new WidgetMetadata {Author = "goeran", Name = "Twitter", IsSelected = true, Type = typeof(Widget), XAPName = "Smeedee.Widgets.xap"};
                secondSelectedWidgetMetadata = new WidgetMetadata {Author = "forfatter", Name = "navn", IsSelected = true, Type = typeof(Widget), XAPName = "Smeedee.Framework.Widgets.xap"};
                widgetMetadatas = new List<WidgetMetadata>
                {
                    firstSelectedWidgetMetadata,
                    secondSelectedWidgetMetadata,
                    new WidgetMetadata {Author = "tuxbear", Name = "Ci Builds", IsSelected = false},
                    new WidgetMetadata {Author = "mikelsen", Name = "Holidays", IsSelected = false}
                };

                viewModelRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<WidgetMetadata>>()))
                    .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<WidgetMetadata>(widgetMetadatas, null));
            };

            protected Context No_slides_are_selected = () =>
            {
                new WidgetMetadata{ Author = "tuxbear", Name = "Ci Builds", IsSelected = false};
            };

            protected Context Controller_is_created = () => { CreateController(); };

            protected When Controller_is_spawned = () => { CreateController(); };

            private static void CreateController()
            {
                controller = new SelectWidgetsDialogController(viewModel, viewModelRepositoryMock.Object, slideConfigPersiterMock.Object);
                if (viewModel != null)
                    viewModel.controller = controller;
            }

            [TearDown]
            public void Teardown()
            {
                StartScenario();
            }
        }
    }
}
