using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Web.Tests.UserRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using TinyBDD.Dsl.GivenWhenThen;

using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.WebTests.Services.Integration.UserRepositoryServiceTests
{
    public class UserRepositoryServiceTestsShared : Shared
    {
        protected static UserRepositoryServiceClient userRepositoryServiceClient;

        protected Context WebServiceClient_is_created = () =>
        {
            userRepositoryServiceClient = new UserRepositoryServiceClient();
        };

        protected Context Database_contains_users = () =>
        {
            var userdb = new Userdb("default");
            var users = new List<User>();
            users.Add(new User("goeran")
            {
                Email = "mail@goeran.no",
                ImageUrl = "http://goeran.no/avatar.jpg",
                Firstname = "Gøran",
                Surname = "Hansen"
            });

            users.Add(new User("jonas")
            {
                Email = "jonas@follesoe.no",
                ImageUrl = "http://jonas.follesoe.no/avatar.jpg",
                Firstname = "Jonas",
                Surname = "Follesø"
            });

            userdb.Users = users;

            databaseSession.Save(userdb);
            databaseSession.Flush();
        };
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_get_User_data_via_WebService : UserRepositoryServiceTestsShared
    {
        [Test]
        public void Assure_all_data_is_serialized()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<Userdb> userdbs = null;

                scenario.Given(Database_is_created).
                    And(Database_contains_users).
                    And(WebServiceClient_is_created);

                scenario.When("get all", () =>
                    userdbs = userRepositoryServiceClient.Get(new AllSpecification<Userdb>()));

                scenario.Then("assure all data is serialized", () =>
                {
                    (userdbs.Count() > 0).ShouldBeTrue();
                    var userdb = userdbs.First();
                    var user = userdb.Users.Where(u => u.Username == "goeran").SingleOrDefault();
                    user.Email.ShouldBe("mail@goeran.no");
                    user.ImageUrl.ShouldBe("http://goeran.no/avatar.jpg");
                    user.Name.ShouldBe("Gøran Hansen");
                    user.Database.ShouldNotBeNull();
                    user.Database.Name.ShouldBe("default");
                });
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_save_via_WebService : UserRepositoryServiceTestsShared
    {
        [Test]
        public void Assure_data_is_persisted()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_is_created).
                    And(WebServiceClient_is_created);

                scenario.When("save", () =>
                {
                    var userdb = new Userdb("default");

                    userdb.AddUser(new User("ghansen")
                    {
                        Email = "mail@goeran.no",
                        Firstname = "Gøran",
                        Surname = "Hansen",
                        ImageUrl = "http://goeran.no/avatar.jpg"
                    });
                    userRepositoryServiceClient.Save(new [] {userdb});
                });

                scenario.Then("assure data is persisted", () =>
                {
                    var user = databaseSession.CreateCriteria(typeof(User)).List<User>().
                        Where(u => u.Username == "ghansen").SingleOrDefault();
                    user.ShouldNotBeNull();
                    user.Database.ShouldNotBeNull();
                });
            });
        }
    }
}
