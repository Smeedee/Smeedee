using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Tasks.Tests.ViewModels
{

    [TestFixture]
    public class When_configuration_entry_is_changed : Shared_ConfigEntries_test_class
    {
        [Test]
        public void assure_that_it_raises_an_event()
        {
            Given(someone_subscribes_to_an_task_config);
            When("The config entry has been changed", () => configurationEntries.Name = "awd");
            Then("An event should have been raised", () => raised.ShouldBe(1));
        }

    }

    [TestFixture]
    public class Shared_ConfigEntries_test_class : SmeedeeScenarioTestClass
    {
        [SetUp]
        public void SetUp()
        {
            configurationEntries = new ConfigurationEntryViewModel {Name = "Test", Value = new object()};
        }

        protected Context someone_subscribes_to_an_task_config = () =>
        {
            raised = 0;
            configurationEntries.ConfigChanged += ConfigHasBeenChanged;
        };

        protected static void ConfigHasBeenChanged(object sender, EventArgs args)
        {
            raised++;
        }

        protected static int raised;
        protected static ConfigurationEntryViewModel configurationEntries;
    }
}
