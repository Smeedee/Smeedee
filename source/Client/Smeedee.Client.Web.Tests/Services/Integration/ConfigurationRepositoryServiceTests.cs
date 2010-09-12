using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Web.Tests.ConfigurationRepositoryService;
using NHibernate;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;
using System.IO;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.WebTests.Services.Integration.ConfigurationRepositoryServiceTests
{
    public class Shared : ScenarioClass
    {
        private static ISessionFactory sessionfactory;
        protected static ISession databaseSession;
        protected static ConfigurationRepositoryServiceClient webServiceClient;

        protected Context Database_is_created = () =>
        {
            if (File.Exists(GenericDatabaseRepository<User>.DatabaseFilePath))
                File.Delete(GenericDatabaseRepository<User>.DatabaseFilePath);

            sessionfactory = NHibernateFactory.AssembleSessionFactory(GenericDatabaseRepository<User>.DatabaseFilePath);

            databaseSession = sessionfactory.OpenSession();
        };

        protected Context database_contains_Configurations = () =>
        {
            configuration = new Configuration("source control");
            configuration.NewSetting("provider", "svn");
            configuration.NewSetting("username", "goeran");
            configuration.NewSetting("password", "hansen");


            databaseSession.Save(configuration);
            databaseSession.Flush();
        };

        protected Context webServiceClient_is_created = () =>
        {
            webServiceClient = new ConfigurationRepositoryServiceClient();
        };

        protected static Configuration configuration;

        [TearDown]
        public void Teardown()
        {
            StartScenario();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_get_Configurations_via_Webservice : Shared
    {
        [Test]
        public void Assure_all_data_is_serialized()
        {
            IEnumerable<Configuration> result = null;

            Given(Database_is_created).
                And(database_contains_Configurations).
                And(webServiceClient_is_created);

            When("get all", () =>
                result = webServiceClient.Get(new AllSpecification<Configuration>()));

            Then("assure all data is serialized", () =>
            {
                result.Count().ShouldNotBe(0);

                var configDb = databaseSession.Get<Configuration>(configuration.Id);

                var config = result.Where(c => c.Name == "source control").FirstOrDefault();
                config.ShouldNotBeNull();

                config.Name.ShouldBe(configDb.Name);
                config.Settings.Count().ShouldBe(configDb.Settings.Count());

                foreach (var settingDb in configDb.Settings)
                {
                    var setting = config.GetSetting(settingDb.Name);
                    setting.Name.ShouldBe(settingDb.Name);
                    setting.HasMultipleValues.ShouldBe(settingDb.HasMultipleValues);
                    setting.Configuration.Name.ShouldBe(settingDb.Configuration.Name);
                    setting.Vals.Count().ShouldBe(settingDb.Vals.Count());
                }
            });
        }
    }
}
