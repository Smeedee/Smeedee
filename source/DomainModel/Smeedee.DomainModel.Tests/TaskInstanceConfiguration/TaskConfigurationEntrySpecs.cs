using System;
using NUnit.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tests;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.TaskInstanceConfiguration
{
    [TestFixture]
    public class When_initializing_a_TaskConfigurationEntry : SmeedeeScenarioTestClass
    {
        protected TaskConfigurationEntry _configuration;
        protected object _value;

        [Test]
        public void Assure_null_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a null value", () =>
                  _configuration = new TaskConfigurationEntry { Value = null, Type = typeof(string) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be  null", () =>
                 _value.ShouldBe(null));
        }

        [Test]
        public void Assure_Integer_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with an integer value", () =>
                  _configuration = new TaskConfigurationEntry { Value = 123, Type = typeof(int) });

            When("we fetch the value", () =>
                 _value = _configuration.Value);

            Then("the value should be an integer", () =>
                 _value.ShouldBe(123));
        }

        [Test]
        public void Assure_string_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a string value", () =>
                  _configuration = new TaskConfigurationEntry { Value = "foobar", Type = typeof(string) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a string", () =>
                 _value.ShouldBe("foobar"));
        }

        [Test]
        public void Assure_float_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a float value", () =>
                  _configuration = new TaskConfigurationEntry { Value = 0.1234567, Type = typeof(float) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a float", () =>
                 _value.ShouldBe(0.1234567));
        }

        [Test]
        public void Assure_double_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a double value", () =>
                  _configuration = new TaskConfigurationEntry { Value = 0.12345678912345, Type = typeof(double) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a double", () =>
                 _value.ShouldBe(0.12345678912345));
        }

        [Test]
        public void Assure_DateTime_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a double value", () =>
                  _configuration = new TaskConfigurationEntry { Value = new DateTime(2012, 6, 7), Type = typeof(DateTime) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a DateTime", () =>
                 _value.ShouldBe(new DateTime(2012, 6, 7)));
        }

        [Test]
        public void Assure_TimeSpan_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a double value", () =>
                  _configuration = new TaskConfigurationEntry { Value = new TimeSpan(6, 6, 6), Type = typeof(TimeSpan) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a TimeSpan", () =>
                 _value.ShouldBe(new TimeSpan(6, 6, 6)));
        }

        [Test]
        public void Assure_bool_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a double value", () =>
                  _configuration = new TaskConfigurationEntry { Value = true, Type = typeof(bool) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a bool", () =>
                 _value.ShouldBe(true));
        }

        [Test]
        public void Assure_Uri_configurations_can_be_handled()
        {
            Given("a new TaskConfiguration is created with a Uri value", () =>
                  _configuration = new TaskConfigurationEntry { Value = new Uri("http://smeedee.org/"), Type = typeof(Uri) });

            When("we fetch the value", () =>
                _value = _configuration.Value);

            Then("the value should be a Uri", () =>
                 (_value as Uri).OriginalString.ShouldBe("http://smeedee.org/"));
        }
    }
}