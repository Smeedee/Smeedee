using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Config
{
    public class SettingsEntryExSpecs
    {
        [TestFixture]
        public class when_converting_to_int
        {
            [Test]
            public void Assure_valid_ints_are_parsed_correctly()
            {
                SettingsEntry entry = new SettingsEntry("test", "100");
                var intValue = entry.ValueAsInt();
                intValue.ShouldBe(100);
            }

            [Test]
            public void Assure_nonvalid_int_thows_exception()
            {
                try
                {
                    SettingsEntry entry = new SettingsEntry("test", "hundre");
                    var intValue = entry.ValueAsInt();
                    Assert.Fail();
                }
                catch (ArgumentException argumentException)
                {
                    argumentException.Message.ShouldBe("The value was not convertable to int: hundre");
                }
            }
        }

    }
}
