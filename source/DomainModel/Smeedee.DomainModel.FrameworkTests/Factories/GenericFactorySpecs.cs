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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Factories;
using Smeedee.DomainModel.Users;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModel.FrameworkTests.Factories.GenericFactorySpecs
{
    public class Shared
    {
        protected static GenericFactory<Userdb> factory;

        protected Context Factory_is_created = () =>
        {
            factory = new GenericFactory<Userdb>();
        };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_isa_IAssembleObjectBasedOnSpec()
        {
            Factory_is_created();
            (factory is IAssembleObjectBasedOnSpec<Userdb>).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_assemble_based_on_Specification : Shared
    {
        [Test]
        public void Assure_obj_is_assembled()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Factory_is_created).
                    And("AllSpecification is used as specification");

                scenario.When("assembling");

                scenario.Then("assure object is assembled", () =>
                {
                    var obj = factory.Assemble(new AllSpecification<Userdb>());
                    obj.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void Assure_null_object_is_not_accepted()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Factory_is_created).
                    And("Specification is a NULL object");

                scenario.When("assembling");

                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        factory.Assemble(null), exception =>
                        exception.Message.ShouldBe("Specification can not be null")));
            });
        }
    }
}
