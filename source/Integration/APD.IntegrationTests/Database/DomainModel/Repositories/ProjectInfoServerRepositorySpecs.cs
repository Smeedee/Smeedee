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
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.IntegrationTests.Database.DomainModel.Repositories;


namespace APD.Plugin.DatabaseTests.ProjectInfoServerRepositorySpecs
{

    public class ProjectInfoServerRepositorySpecsShared : Shared
    {
        protected static GenericDatabaseRepository<ProjectInfoServer> repository;
        protected static GenericDatabaseRepository<ProjectInfoServer> persister;

        protected static List<Project> projects = new List<Project>();
       
        protected static ProjectInfoServer firstPIServer = new ProjectInfoServer()
        {
            Name = "ProjectInfoServer 1", 
            Url = "www.firstPIServer.com",
            Projects = projects
        };

        protected static ProjectInfoServer secondPIServer = new ProjectInfoServer()
        {
            Name = "ProjectInfoServer 2",
            Url = "www.secondPIServer.com",
            Projects = projects
        };

        

        protected Context a_ProjectInfoServerRepository_is_created = () =>
        {
            repository = new GenericDatabaseRepository<ProjectInfoServer>(sessionFactory);
            persister = new GenericDatabaseRepository<ProjectInfoServer>(sessionFactory);
        };


        protected Context there_is_ProjectInfoServers_in_the_database = () =>
        {
            persister.Save(firstPIServer);
            persister.Save(secondPIServer);
        };

    }
   
    [TestFixture]
    public class When_spawned : ProjectInfoServerRepositorySpecsShared
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
                scenario.Given(a_ProjectInfoServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it is not null", () => repository.ShouldNotBeNull());
            });
            
        }


        [Test]
        public void should_be_instance_of_type_IRepository_of_type_ProjectInfoServer()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoServerRepository_is_created);
                scenario.When("instance is created");
                scenario.Then("it implements IRepository<ProjectInfoServer", () =>
                    repository.ShouldBeInstanceOfType<IRepository<ProjectInfoServer>>());
            });
        }
    }


    [TestFixture]
    public class When_accessed : ProjectInfoServerRepositorySpecsShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_retrieve_all_ProjectInfoServers_in_database()
        {
            IEnumerable<ProjectInfoServer> retrievedPIServers = new List<ProjectInfoServer>();
            
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoServerRepository_is_created)
                    .And(there_is_ProjectInfoServers_in_the_database);
                scenario.When("ProjectInfoServers are requested", () =>
                    retrievedPIServers = repository.Get(new AllSpecification<ProjectInfoServer>()));                
                scenario.Then("all ProjectInfoServers are be returned", () =>
                    retrievedPIServers.Count().ShouldBe(2));
                    retrievedPIServers.Contains(firstPIServer);
                    retrievedPIServers.Contains(secondPIServer);
            });
        }

    }

}
