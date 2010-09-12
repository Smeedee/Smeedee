using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.UserdbRepositorySpecs
{
    public class UserdbRepositoryShared : Shared 
    {
        protected static GenericDatabaseRepository<Userdb> repository;

        protected Context Repository_is_created = () =>
        {
            repository = new UserdbDatabaseRepository(sessionFactory);
        };
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_get_Userdbs : UserdbRepositoryShared
    {
        [Test]
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

    public class when_saving : UserdbRepositoryShared
    {
        [Test]
        public void Assure_only_given_userDB_is_replaced_with_saved_DB()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            repository = new UserdbDatabaseRepository(sessionFactory);
            var firstDB = new Userdb("default");
            firstDB.AddUser(new User("haldis"));

            var secondDB = new Userdb("not_default");
            secondDB.AddUser(new User("kari"));

            repository.Save(firstDB);
            repository.Save(secondDB);

            var firstDBUpdate = new Userdb("default");
            firstDBUpdate.AddUser(new User("tuxbear"));

            repository.Save(firstDBUpdate);

            var result = repository.Get(new AllSpecification<Userdb>());
            result.Count().ShouldBe(2);
            result.ElementAt(0).Name.ShouldBe("default");
            result.ElementAt(0).Users.First().Username.ShouldBe("tuxbear");
            result.ElementAt(1).Name.ShouldBe("not_default");
            result.ElementAt(1).Users.First().Username.ShouldBe("kari");

        }
    }
}
