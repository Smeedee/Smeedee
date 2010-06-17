using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Tests.ViewModel
{
    class SlideTests
    {
        [TestFixture]
        public class When_spawned : SlideTestContext
        {
            public override void Context()
            {
                When_Slide_is_spawned();
            }

            [Test]
            public void assure_it_has_a_IsInSettingsMode_flag()
            {
                viewModel.IsInSettingsMode.ShouldBe(false);
            }

            [Test]
            public void assure_Settings_command_can_be_executed()
            {
                viewModel.Settings.CanExecute(null).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_Settings_Command_is_executed : SlideTestContext
        {
            public override void Context()
            {
                Given_Slide_is_created();

                When_execute_Settings_Command();
            }

            [Test]
            public void assure_is_in_settings_mode()
            {
                viewModel.IsInSettingsMode.ShouldBeTrue();
            }


            [Test]
            public void assure_it_goes_back_to_view_mode_when_executed_againt()
            {
                When_execute_Settings_Command();

                viewModel.IsInSettingsMode.ShouldBeFalse();
            }

        }

    }
}
