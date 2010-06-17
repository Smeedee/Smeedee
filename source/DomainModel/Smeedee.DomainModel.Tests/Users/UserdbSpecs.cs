using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Users;

using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModelTests.Users.UserdbSpecs
{
    [TestFixture]
    public class When_created 
    {
        [Test]
        public void Assure_Users_is_not_null()
        {
            var userdb = new Userdb();
            userdb.Users.ShouldNotBeNull();
        }
    }
    
    [TestFixture]
    public class When_using_default_factory_method
    {
        [Test]
        public void Assure_object_is_created()
        {
            Userdb userdb = Userdb.Default();
            userdb.ShouldNotBeNull();            
        }

        [Test]
        public void Assure_default_name_is_used()
        {
            Userdb userdb = Userdb.Default();
            userdb.Name.ShouldBe("default");
        }
    }
}
