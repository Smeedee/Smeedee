using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.TaskInstanceConfiguration
{
    public class TaskConfigurationSpecs
    {
        [TestFixture]
        public class when_reading_entry_value
        {
            [Test]
            public void Assure_null_entries_are_ignored()
            {
                var config = new TaskConfiguration();
                config.Entries.Add(null);
                config.Entries.Add(new TaskConfigurationEntry(){Name = "testName", Value = "myValue",Type = typeof(string)});

                config.ReadEntryValue("testName").ShouldBe("myValue");
            }
        }
    }
}
