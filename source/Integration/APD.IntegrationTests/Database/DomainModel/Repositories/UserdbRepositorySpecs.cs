using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.Users;
using APD.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.UserdbRepositorySpecs
{
    public class UserdbRepositoryShared : Shared 
    {
        protected static GenericDatabaseRepository<Userdb> repository;

        protected Context Repository_is_created = () =>
        {
            repository = new UserdbDatabaseRepository(sessionFactory);
        };
    }

    [TestFixture]
    public class When_get_Userdbs : UserdbRepositoryShared
    {
        [Test]
        [Ignore]
        public void Assure_rows_in_resultset_contains_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<Userdb> resultset = null;

                scenario.Given(Database_exists).
                    And(Database_contains_user_data).
                    And(Repository_is_created);

                scenario.When("Get is called", () =>
                    resultset = repository.Get(new AllSpecification<Userdb>()));
                scenario.Then("assure rows in resultsset contains data", () =>
                {
                    ( resultset.Count() > 0 ).ShouldBeTrue();
                    foreach (var userdb in resultset)
                        (userdb.Users.Count() > 0).ShouldBeTrue();
                });
            });
        }
    }
}
