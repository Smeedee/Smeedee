using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Users;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModelTests.Users.DefaultUserdbSpecificationSpecs
{
    [TestFixture]
    public class When_IsSatisfiedBy
    {
        [Test]
        public void Assure_only_Userdb_with_default_name_is_satisfied()
        {
            var spec = new DefaultUserdbSpecification();
            spec.IsSatisfiedBy(new Userdb("default")).ShouldBeTrue();
            spec.IsSatisfiedBy(new Userdb()).ShouldBeFalse();
        }

    }

    [TestFixture]
    public class When_AssembleObject
    {
        [Test]
        public void Assure_default_name_is_set()
        {
            var spec = new DefaultUserdbSpecification();
            var obj = spec.AssembleObject();

            obj.Name.ShouldBe("default");
        }
    }
}
