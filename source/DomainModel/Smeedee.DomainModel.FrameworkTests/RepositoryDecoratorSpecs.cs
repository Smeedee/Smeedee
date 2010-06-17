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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Moq;
using Smeedee.DomainModel.Framework;


namespace Smeedee.DomainModel.FrameworkTests.RepositoryDecoratorSpecs
{
    public class RepositoryDecoratorMock<TDomainModel> : RepositoryDecorator<TDomainModel>
    {
        public bool GetCalled{ get; set; }
        public bool CallDecoratedRepository { get; set; }
        public IEnumerable<TDomainModel> SetGetRetValue { get; set; }

        public RepositoryDecoratorMock(IRepository<TDomainModel> decoratedObject) :
            base(decoratedObject)
        {
            CallDecoratedRepository = true;
        }

        public override IEnumerable<TDomainModel> Get(Specification<TDomainModel> specification)
        {
            GetCalled = true;
            
            IEnumerable<TDomainModel> retValue = null;

            if (CallDecoratedRepository)
                retValue = base.Get(specification);

            if (SetGetRetValue != null)
                retValue = SetGetRetValue;

            return retValue;
        }
    }


    public class Shared
    {
        protected static Mock<IRepository<string>> repositoryMock;
        protected static IRepository<string> repository;
        protected static RepositoryDecoratorMock<string> decorator;
        protected static Mock<Specification<string>> specificationMock;

        protected Context repository_is_created = () =>
        {
            specificationMock = new Mock<Specification<string>>();
            repositoryMock = new Mock<IRepository<string>>();
            repository = repositoryMock.Object;
        };

        protected Context its_decorated = () =>
        {
            decorator = new RepositoryDecoratorMock<string>(repository);
            repository = decorator;
        };

        protected Context repository_have_data = () =>
        {
            var retValue = new List<string>();
            retValue.Add("hello world");
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<string>>())).Returns(retValue);
        };

        protected When Get_is_called = () =>
        {
            repository.Get(specificationMock.Object);   
        };
    }

    [TestFixture]
    public class When_created : Shared
    {
        [Test]
        public void Assure_it_does_not_accept_NullObj_as_param()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(repository_is_created);

                scenario.When("decorator is created");

                scenario.Then("assure it does not accept NullObj as parameter", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new RepositoryDecoratorMock<string>(null), exception =>
                            exception.Message.ShouldBe("Decorated Repository must be specified")));
            });
        
        }           
    }

    [TestFixture]
    public class When_Get : Shared
    {
        [Test]
        public void Assure_decorated_Repository_is_called()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(repository_is_created).
                    And(its_decorated);

                scenario.When(Get_is_called);

                scenario.Then("Assure decorated Repository is called", () =>
                    repositoryMock.Verify(
                        r => r.Get(It.IsAny<Specification<string>>()), Times.Once()));
            });
        }

        [Test]
        public void Assure_decorator_can_prevent_calling_Repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                RepositoryDecoratorMock<string> newDecorator = null;

                scenario.Given(repository_is_created).
                    And("its decorated", () =>
                    {
                        newDecorator = new RepositoryDecoratorMock<string>(repository);
                        repository = newDecorator;
                    }).
                    And("decorator is configured to not call repository", () =>
                    {
                        newDecorator.CallDecoratedRepository = false;
                    });

                scenario.When(Get_is_called);

                scenario.Then("assure decorated Repository is not called", () =>
                    repositoryMock.Verify(
                        r => r.Get(It.IsAny<Specification<string>>()), Times.Never()));
            });
        }
        

        [Test]
        public void Assure_return_value_from_decorated_Repository_can_be_received()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(repository_is_created).
                    And(its_decorated).
                    And(repository_have_data);

                scenario.When("Get is called");

                scenario.Then("Assure return value from decorated Repository can be received", () =>
                {
                    var retValue = repository.Get(specificationMock.Object);
                    retValue.ShouldNotBeNull();
                    retValue.First().ShouldBe("hello world");
                });
            });
        }

        [Test]
        public void Assure_return_value_can_be_manipulated_by_decorator()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(repository_is_created).
                    And(repository_have_data).
                    And(its_decorated).
                    And("decorator manipulate return value from Repository", () =>
                    {
                        var newRetValue = new List<string>();
                        newRetValue.Add("hello world 2");
                        decorator.SetGetRetValue = newRetValue;
                    });

                scenario.When("Get is called");

                scenario.Then("Assure return value can be manipulated by decorator", () =>
                {
                    var retValue = repository.Get(specificationMock.Object);
                    retValue.Count().ShouldBe(1);
                    retValue.Last().ShouldBe("hello world 2");
                });
            });
        }
    }
}