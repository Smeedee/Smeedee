using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.Controllers;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework;
using Moq;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.Views;

namespace APD.Client.Widget.AdminTests.Controllers.ModalDialogControllerSpecs
{
    public class CustomModalDialog : IModalDialog
    {
    
    }

    public class Shared
    {
        protected static ModalDialogController controller;
        protected static ModalDialogViewModel viewModel;
        protected static Mock<INotify<ShowModalDialogEventArgs>> showModalDialogEventNotifierMock;
        protected static Mock<INotify<HideModalDialogEventArgs>> hideModalDialogEventNotifierMock;

        protected Context ModalDialogController_is_created = () =>
        {
            viewModel = new ModalDialogViewModel(new NoUIInvocation(), null);
            showModalDialogEventNotifierMock = new Mock<INotify<ShowModalDialogEventArgs>>();
            hideModalDialogEventNotifierMock = new Mock<INotify<HideModalDialogEventArgs>>();
            controller = new ModalDialogController(viewModel, showModalDialogEventNotifierMock.Object, hideModalDialogEventNotifierMock.Object);
        };

        protected When DisplayModalDialogEvent_is_triggered = () =>
        {
            showModalDialogEventNotifierMock.Raise(n => n.NewNotification += null, new ShowModalDialogEventArgs()
            {
                UserInterface = new CustomModalDialog()
            });
        };

        protected When HideModalDialogEvent_is_triggered = () =>
        {
            hideModalDialogEventNotifierMock.Raise(n => n.NewNotification += null, new HideModalDialogEventArgs());
        };
    }

    [TestFixture]
    public class When_spawing : Shared
    {
        [Test]
        public void Assure_Arguments_are_validated()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("ModalDialogController is about to be spawned");

                scenario.When("spawning new without a ShowModalDialogEventNotifier specified");
                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new ModalDialogController(new ModalDialogViewModel(new NoUIInvocation(), null), null, null), exception =>
                            exception.Message.ShouldBe("Value can not be null\r\nParameter name: showModalDialogCommandNotifier")));

                scenario.When("spawning new without a ModalDialogViewModel specified");
                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new ModalDialogController(null, new CommandNotifierWiring<ShowModalDialogEventArgs>(), null), exception =>
                            exception.Message.ShouldBe("Value can not be null\r\nParameter name: modalDialogViewModel")));

                scenario.When("spawning new without a HideModalDialogEventNotifier specified");
                scenario.Then("assure ArgumentException is throw", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new ModalDialogController(new ModalDialogViewModel(new NoUIInvocation(), null), new CommandNotifierWiring<ShowModalDialogEventArgs>(), null), exception =>
                            exception.Message.ShouldBe("Value can not be null\r\nParameter name: hideModalDialogEventNotifier")));
            });
        }
    }

    [TestFixture]
    public class When_HideModalDialogEvent_is_triggered : Shared
    {
        [Test]
        public void Assure_Visibility_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ModalDialogController_is_created).
                    And("ModalDialog is visible", () =>
                        viewModel.Visible = true);

                scenario.When(HideModalDialogEvent_is_triggered);
                scenario.Then("assure Visible is set to false on ViewModel", () =>
                    viewModel.Visible.ShouldBeFalse());
            });
        }
    }

    [TestFixture]
    public class When_DisplayModalDialogEvent_is_triggered : Shared
    {
        [Test]
        public void Assure_Visiblity_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ModalDialogController_is_created);

                scenario.When(DisplayModalDialogEvent_is_triggered);
                
                scenario.Then("assure Visible is set to true on ViewModel", () =>
                    viewModel.Visible.ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_ModalDialog_is_set_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ModalDialogController_is_created);

                scenario.When(DisplayModalDialogEvent_is_triggered);

                scenario.Then("assure ModalDialog is set on ViewModel", () =>
                    viewModel.ModalDialog.ShouldNotBeNull());
            });
        }
    }
}
