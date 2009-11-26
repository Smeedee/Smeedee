using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework;
using TinyBDD.Specification.NUnit;
using Moq;

namespace APD.Client.Widget.AdminTests.ViewModels.ModalDialogViewModelSpecs
{
    public class Shared 
    {
        protected static ModalDialogViewModel viewModel;
        protected static Mock<ITriggerCommand> closeModalDialogCommandTriggerMock;

        protected Context ViewModel_is_created = () =>
        {
            closeModalDialogCommandTriggerMock = new Mock<ITriggerCommand>();
            viewModel = new ModalDialogViewModel(new NoUIInvocation(), closeModalDialogCommandTriggerMock.Object);
        };

        protected Context modal_dialog_is_open = () =>
        {
            viewModel.Visible = true;
        };
    }

    [TestFixture]
    public class When_close_ModalDialog : Shared
    {
        [Test]
        public void Assure_CloseModalDialogCommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ViewModel_is_created).
                    And(modal_dialog_is_open);

                scenario.When("close", () =>
                    viewModel.CloseUICommand.Execute(null));
                scenario.Then("assure CloseModalDialogCommand is triggered", () =>
                    closeModalDialogCommandTriggerMock.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }
}
