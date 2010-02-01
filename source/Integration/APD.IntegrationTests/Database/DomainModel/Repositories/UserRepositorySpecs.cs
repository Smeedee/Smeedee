// Note: I have commented out these tests as the UserDatabaseRepository was switched with the UserDbDatabaseRepository
// This file may be deleted - Ole (01.02.10)


//#region File header

//// <copyright>
//// This library is free software; you can redistribute it and/or
//// modify it under the terms of the GNU Lesser General Public
//// License as published by the Free Software Foundation; either
//// version 2.1 of the License, or (at your option) any later version.
//// 
//// This library is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//// Lesser General Public License for more details.
//// 
//// You should have received a copy of the GNU Lesser General Public
//// License along with this library; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//// </copyright> 
//// 
//// <contactinfo>
//// The project webpage is located at http://agileprojectdashboard.org/
//// which contains all the neccessary information.
//// </contactinfo>
//// 
//// <author>Your Name</author>
//// <email>your@email.com</email>
//// <date>2009-MM-DD</date>

//#endregion

//using System.Collections.Generic;
//using System.Linq;
//using APD.DomainModel.Users;
//using APD.Integration.Database.DomainModel.Repositories;
//using NUnit.Framework;
//using TinyBDD.Dsl.GivenWhenThen;
//using APD.DomainModel.Framework;
//using TinyBDD.Specification.NUnit;


//namespace APD.IntegrationTests.Database.DomainModel.Repositories.UserRepositorySpecs
//{
//    public class UserRepositoryShared : Shared
//    {
//        protected static GenericDatabaseRepository<User> userRepository;


//        protected Context Repository_is_created = () =>
//        {
//            userRepository = new GenericDatabaseRepository<User>(sessionFactory);
//        };
//    }

//    [TestFixture]
//    public class When_get_Users : UserRepositoryShared
//    {
//        [Test]
//        public void Assure_rows_in_resultset_contains_data()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                IEnumerable<User> users = null;

//                scenario.Given(Database_exists).
//                    And(Database_contains_user_data).
//                    And(Repository_is_created);

//                scenario.When("Get is called", () =>
//                    users = userRepository.Get(new AllSpecification<User>()));

//                scenario.Then("assure rows in resultset contains data", () =>
//                {
//                    (users.Count() > 0).ShouldBeTrue();
//                    var user = users.Where(u => u.Username == "goeran").SingleOrDefault();
//                    user.Email.ShouldBe("mail@goeran.no");
//                    user.ImageUrl.ShouldBe("http://goeran.no/avatar.jpg");
//                    user.Name.ShouldBe("Gøran Hansen");
//                    user.Database.ShouldNotBeNull();
//                    user.Database.Name.ShouldBe("default");
//                });
//            });
//        }

//        [Test]
//        public void Assure_resultset_contains_data()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                IEnumerable<User> users = null;

//                scenario.Given(Database_exists).
//                    And(Database_contains_user_data).
//                    And(Repository_is_created);

//                scenario.When("Get is called", () =>
//                    users = userRepository.Get(new AllSpecification<User>()));

//                scenario.Then("assure resultset contains data", () =>
//                {
//                    users.ShouldNotBeNull();
//                    (users.Count() > 0).ShouldBeTrue();
//                    users.First().Database.ShouldNotBeNull();
//                });
//            });
//        }
//    }

//    [TestFixture]
//    public class When_save_User : UserRepositoryShared
//    {   
//        [Test]
//        [Ignore]
//        public void Assure_new_record_is_created()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                scenario.Given(Database_exists).
//                    And(Repository_is_created);

//                scenario.When("a new User is saved", () =>
//                    userRepository.Save(new User()
//                    {
//                        Username = "goeran",
//                        Email = "mail@goeran.no",
//                        ImageUrl = "http://goeran.no/Avatar.jpg",
//                        Firstname = "Gøran",
//                        Surname = "Hansen"
//                    }));

//                scenario.Then("assure a new record is created in the Users database table", () =>
//                {
//                    var users = sessionFactory.OpenSession().CreateCriteria(typeof(User)).List<User>();
//                    users.Count.ShouldNotBe(0);
//                    var user = users.Where(u => u.Username == "goeran").SingleOrDefault();
//                    user.ShouldNotBeNull();
//                    user.Username.ShouldBe("goeran");
//                    user.Email.ShouldBe("mail@goeran.no");
//                    user.ImageUrl.ShouldBe("http://goeran.no/Avatar.jpg");
//                    user.Firstname.ShouldBe("Gøran");
//                    user.Middlename.ShouldBe(null);
//                    user.Surname.ShouldBe("Hansen");
//                    user.Name.ShouldBe("Gøran Hansen");
//                    user.Database.ShouldNotBeNull();
//                    user.Database.Name.ShouldBe("default");
//                });
//            });
//        }
//    }
//}
