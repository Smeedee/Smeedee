#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Collections.Generic;
using System.Linq;

using APD.DomainModel.Config;
using APD.DomainModel.Framework;

using NHibernate;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.ConfigurationSpecs
{
    public class ConfigurationRepositoryShared : Shared
    {
        protected static GenericDatabaseRepository<Configuration> configurationRepository;

        protected Context Database_exists = () =>
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        };

        protected Context Database_contains_configuration_data = () =>
        {
            using (var session = sessionFactory.OpenSession())
            {
                var configuration1 = new Configuration()
                {
                    Name = "SourceControl",
                };
                configuration1.NewSetting("url", new string[] {"http://sourcecontrol"});

                var configuration2 = new Configuration()
                {
                    Name = "ContinuousIntegration",
                };
                configuration2.NewSetting("url", new string[] { "http://continuousintegration" });

                session.Save(configuration1);
                session.Save(configuration2);
                session.Flush();
            }
        };

        protected Context Repository_is_created = () =>
        {
            configurationRepository = new GenericDatabaseRepository<Configuration>(sessionFactory);
        };
    }

    [TestFixture]
    public class When_get : ConfigurationRepositoryShared
    {
        [Test]
        public void Assure_rows_in_resultset_contains_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<Configuration> configurations = null;

                scenario.Given(Database_exists).
                    And(Database_contains_configuration_data).
                    And(Repository_is_created);

                scenario.When("Get is called", () =>
                    configurations = configurationRepository.Get(new AllSpecification<Configuration>()));

                scenario.Then("assure rows in resultset contains data", () =>
                {
                    (configurations.Count() > 0).ShouldBeTrue();
                    var configuration = configurations.Where(c => c.Name == "SourceControl").SingleOrDefault();
                    configuration.Name.ShouldBe("SourceControl");
                    (configuration.Settings.Count() > 0).ShouldBeTrue();
                    var setting = configuration.Settings.Where(s => s.Name == "url").SingleOrDefault();
                    setting.Name.ShouldBe("url");
                    (setting.Vals != null).ShouldBeTrue();
                    (setting.Vals.Count() > 0).ShouldBeTrue();
                    setting.Vals.AsEnumerable().FirstOrDefault().ShouldBe("http://sourcecontrol");
                });
            });
        }
    }

    [TestFixture]
    public class When_save : ConfigurationRepositoryShared
    {
        [Test]
        public void Assure_new_record_is_created()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists).
                    And(Repository_is_created);

                scenario.When("a new Configuration is saved", () =>
                {
                    var configuration = new Configuration()
                    {
                        Name = "ProcessManagement",
                    };
                    configuration.NewSetting("username", new string[] { "goeran" });
                    configurationRepository.Save(configuration);
                });

                scenario.Then("assure a new record is created in the Configuration database table", () =>
                {
                    var configuration = sessionFactory.OpenSession().Get(typeof(Configuration), "ProcessManagement") as Configuration;
                    configuration.ShouldNotBeNull();
                    configuration.Name.ShouldBe("ProcessManagement");
                    (configuration.Settings.Count() > 0).ShouldBeTrue();
                    var setting = configuration.Settings.Where(s => s.Name == "username").SingleOrDefault();
                    setting.Name.ShouldBe("username");
                    (setting.Vals != null).ShouldBeTrue();
                    (setting.Vals.Count() > 0).ShouldBeTrue();
                    setting.Vals.AsEnumerable().FirstOrDefault().ShouldBe("goeran");
                });
            });
        }
        
        [Test]
        public void Assure_not_possible_to_save_two_Configurations_with_same_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                var session = sessionFactory.OpenSession();

                scenario.Given(Database_exists);

                scenario.When("a new Configuration is created", () =>
                {
                    var configuration = new Configuration()
                    {
                        Name = "ProcessManagement"
                    };
                    session.Save(configuration);
                });

                scenario.Then("assure an exception is thrown when creating a new Configuration with the same name", () =>
                {
                    var configuration = new Configuration()
                    {
                        Name = "ProcessManagement"
                    };
                    this.ShouldThrowException<NonUniqueObjectException>(
                        () =>
                        session.Save(configuration),
                        exception => { exception.ShouldNotBeNull(); }
                        );
                });
            });
        }

    }
}
