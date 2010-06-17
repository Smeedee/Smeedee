using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Users;

using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.DomainModelTests.Users.UserSpecs
{
    [TestFixture]
    public class When_created
    {
        [Test]
        public void Assure_User_obj_set_default_Database()
        {
            var user = new User("goeran")
            {
                Email = "mail@goeran.no",
                ImageUrl = "http://goeran.no/avatar.jpg"
            };

            user.Database.ShouldNotBeNull();
            user.Database.Name.ShouldBe("default");
        }
    }
}
