using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Client.Web.Tests.TaskConfigurationRepositoryService;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.WebTests.Services.Integration
{
    [TestFixture][Category("IntegrationTest")]
    public class TaskConfigurationRepositoryServiceSpecs : SmeedeeScenarioTestClass
    {
        protected static TaskConfigurationRepositoryWebserviceClient service;
        protected static AllSpecification<TaskConfiguration> all_configs = new AllSpecification<TaskConfiguration>();

        protected const string SMEEDEE_URL = "http://smeedee.org/";

        [Test]
        public void Assure_database_is_empty_after_delete()
        {
            Given(we_have_a_database_with_data);

            When("we delete the data", () =>
                service.Delete(all_configs));

            Then("the database should be empty", () =>
                service.Get(all_configs).Count.ShouldBe(0));
        }

        [Test]
        public void Assure_saved_item_can_be_retrieved()
        {
            Given(we_have_an_empty_database);

            When(we_add_a_configuration);

            Then("there should be one configuration item", () =>
                 service.Get(all_configs).Count.ShouldBe(1));
        }

        [Test]
        public void Assure_configuration_can_be_persisted_with_a_Uri_entry_and_then_reserialized()
        {
            Given(we_have_an_empty_database);

            When(we_add_a_configuration_with_a_Uri_value);

            Then("the Uri we fetched should have the correct value", () =>
                 service.Get(all_configs).FirstConfigurationEntryValue().ShouldBe(SMEEDEE_URL));
        }

        [Test]
        public void Assure_configuration_can_be_persisted_with_a_orderIndex_property_and_a_URI_entry_and_then_serialized()
        {
            Given(we_have_an_empty_database);

            When(we_add_a_configuration_with_a_Uri_value);

            Then("the Uri we fetched should have the correct value", () =>
                 service.Get(all_configs).FirstConfigurationEntryValue().ShouldBe(SMEEDEE_URL));
        }

        [SetUp]
        public void SetUp()
        {
            service = new TaskConfigurationRepositoryWebserviceClient();
        }

        protected Context we_have_a_database_with_data = () => SaveConfiguration();

        protected Context we_have_an_empty_database = () => service.Delete(all_configs);

        protected When we_add_a_configuration = () => SaveConfiguration();

        protected When we_add_a_configuration_with_a_Uri_value = () => SaveConfiguration(SmeedeeUriEntry());

        protected When we_add_a_configuration_with_orderIndex_property_and_a_Uri_value = () => SaveConfiguration(SmeedeeUriEntryWithOrderIndexProperty());
        
        private static void SaveConfiguration()
        {
            SaveConfiguration(new List<TaskConfigurationEntry>());
        }

        private static void SaveConfiguration(IList<TaskConfigurationEntry> configEntries)
        {
            service.Save(new List<TaskConfiguration>
                             {
                                 new TaskConfiguration
                                     {
                                         Name = "Foo",
                                         TaskName = "Bar",
                                         DispatchInterval = 1,
                                         Entries = configEntries
                                     }
                             });
        }

        private static List<TaskConfigurationEntry> SmeedeeUriEntry()
        {
            return new List<TaskConfigurationEntry>
                       {
                           new TaskConfigurationEntry
                               {
                                   Name = "Foo",
                                   Type = typeof(Uri),
                                   Value = new Uri(SMEEDEE_URL)
                               }
                       };
        }

        private static List<TaskConfigurationEntry> SmeedeeUriEntryWithOrderIndexProperty()
        {
            return new List<TaskConfigurationEntry>
                       {
                           new TaskConfigurationEntry()
                               {
                                   OrderIndex = 1,
                                   Name = "Foo",
                                   Type = typeof (Uri),
                                   Value = new Uri(SMEEDEE_URL)
                               }
                       };
        }
    }

    internal static class Extensions
    {
        public static string FirstConfigurationEntryValue(this List<TaskConfiguration> configurations)
        {
            return configurations.First().Entries.First().Value.ToString();
        }
    }
}