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
using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.CIServerRepositorySpecs
{


    public class CIServerRepositoryShared : Shared
    {
        protected static GenericDatabaseRepository<CIServer> _databaseRepository;
        protected static GenericDatabaseRepository<CIServer> persister;

        protected static List<CIProject> projects = new List<CIProject> { };
        protected static CIServer ciServer = new CIServer
        {
            Name = "Mock CI Server",
            Projects = projects,
            Url = "hjsdlkfjds"
        };
        protected static CIServer ciServer2 = new CIServer
        {
            Name = "Another CI Server",
            Projects = projects,
            Url = "afdsafd"
        };


        protected Context a_CIServerRepository_is_created = () =>
        {
            var factory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
            _databaseRepository = new GenericDatabaseRepository<CIServer>(factory);
            persister = new GenericDatabaseRepository<CIServer>(factory);
        };

        protected Context there_are_CIServer_objects_in_the_database = () =>
        {

            persister.Save(ciServer);
            persister.Save(ciServer2);
        };

    }

    [TestFixture]
    public class when_created : CIServerRepositoryShared
    {

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_be_created()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it is not null", () => _databaseRepository.ShouldNotBeNull());
            });
        }

        [Test]
        public void should_implement_IRepository_of_type_CIServer()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it implements IRepository<CIServer>", () =>
                    _databaseRepository.ShouldBeInstanceOfType<IRepository<CIServer>>());
            });
        }
    }

    [TestFixture]
    public class when_accessed : CIServerRepositoryShared
    {

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }        
        
        [Test]
        public void should_fetch_all_CI_servers_in_database()
        {
            IEnumerable<CIServer> result = null;
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created).
                         And(there_are_CIServer_objects_in_the_database);
                scenario.When("all CI Servers are requested", () =>
                    result = _databaseRepository.Get(new AllSpecification<CIServer>()));
                scenario.Then("all CI Servers are returned", () =>
                {
                    result.Count().ShouldBe(2);
                    result.Contains(ciServer);
                    result.Contains(ciServer2);
                });
            });
        }
    }
}
