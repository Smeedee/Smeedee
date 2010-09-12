using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.Client.Tests.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests.ViewModel.Dialogs
{
    public class DialogTests
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Context()
            {
                When_Dialog_is_spawned();
            }

            [Test]
            public void Then_assure_DisplayOkButton_is_true()
            {
                viewModel.DisplayOkButton.ShouldBeTrue();
            }

            [Test]
            public void Then_assure_DisplayCancelButton_is_true()
            {
                viewModel.DisplayCancelButton.ShouldBeTrue();
            }

            [Test]
            public void Then_assure_OkButtonText_is_set()
            {
                viewModel.Ok.Text.ShouldBe("OK");
            }

            [Test]
            public void Then_assure_CancelButtonText_is_set()
            {
                viewModel.Cancel.Text.ShouldBe("Cancel");
            }

            [Test]
            public void Then_assure_it_has_a_Progressbar()
            {
                viewModel.Progressbar.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_OK : Shared
        {
            private CloseDialogEventArgs closeDialogEventArgs = null;

            public override void Context()
            {
                base.Context();

                Given_Dialog_is_created();
                And("Observer has subscribed to CloseDialog event", () =>
                    viewModel.CloseDialog += (o, e) => closeDialogEventArgs = e);

                When_execute_Ok_Command();
            }

            [Test]
            public void Then_assure_CloseDialog_event_is_triggered()
            {
                closeDialogEventArgs.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_DialogResult_is_true()
            {
                closeDialogEventArgs.DialogResult.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_Cancel : Shared
        {
            private CloseDialogEventArgs closeDialogEventArgs = null;

            public override void Context()
            {
                base.Context();

                Given_Dialog_is_created();
                And("Observer has subscribed to CloseDialog event", () =>
                    viewModel.CloseDialog += (o, e) => closeDialogEventArgs = e);

                When_execute_Cancel_Command();
            }

            [Test]
            public void Then_assure_CloseDialog_event_is_triggered()
            {
                closeDialogEventArgs.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_DialogResult_is_false()
            {
                closeDialogEventArgs.DialogResult.ShouldBeFalse();
            }
        }

        public class Shared : DialogTestContext
        {
            public override void Context()
            {
                
            }
        }
    }
}
