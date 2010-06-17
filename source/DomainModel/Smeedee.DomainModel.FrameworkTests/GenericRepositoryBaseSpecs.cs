using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.Users;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.SemanticModel;
using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.FrameworkTests.GenericRepositoryBaseSpecs
{
    public class Shared : ScenarioClass
    {
        static protected Context the_asyncRepo_is_valid = () =>
        {
            asyncRepoMock.Setup(asr => asr.BeginGet(It.IsAny<AllSpecification<User>>()))
                .Raises(asr => asr.GetCompleted += null,
                        new GetCompletedEventArgs<User>(new List<User> {new User()}));
            
            asyncRepository = asyncRepoMock.Object;
        };

        static protected Context the_asyncRepo_is_null = () =>
        {
            asyncRepository = null;
        };

        static protected When the_genericRepository_is_spawned = () =>
        {
            genericRepositoryBase = new GenericRepositoryBase<User>(asyncRepository);
        };
        
        static protected IAsyncRepository<User> asyncRepository = null;
        static protected GenericRepositoryBase<User> genericRepositoryBase = null;
        static protected Mock<IAsyncRepository<User>> asyncRepoMock = new Mock<IAsyncRepository<User>>();
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_accept_iasyncrepository_that_is_null()
        {
            IAsyncRepository<User> asyncRepo;

            Given(the_asyncRepo_is_null);
            When(the_genericRepository_is_spawned);
            Then("It should throw an argumentexception", ()=>
                new GenericRepositoryBase<User>(null));

            StartScenario();
        }

        [TestFixture]
        public class when_asyncRepository_is_valid : Shared
        {
            [SetUp]
            public void Setup()
            {
                Given(the_asyncRepo_is_valid);
                When(the_genericRepository_is_spawned);
            }

            [Test]
            public void should_implement_IRepository()
            {
                Then(() => (genericRepositoryBase as IRepository<User>).ShouldNotBeNull());
                StartScenario();
            }
        }
    }

    [TestFixture]
    public class when_calling_get : Shared
    {
        [SetUp]
        public void Setup()
        {
            Given(the_asyncRepo_is_valid);
            When(the_genericRepository_is_spawned);
        }

        [Test]
        public void the_first_call_should_return_empty()
        {
            Then(() =>
            {
                var result = genericRepositoryBase.Get(new AllSpecification<User>());
                result.Count().ShouldBe(0);
            });
            StartScenario();
        }

        [Test]
        public void the_second_call_should_yield_a_result()
        {
            Then(() =>
            {
                genericRepositoryBase.UpdateInterval = 500;
                genericRepositoryBase.Get(new AllSpecification<User>());
                System.Threading.Thread.Sleep(1000);
                var secondCallResults = genericRepositoryBase.Get(new AllSpecification<User>());
                secondCallResults.Count().ShouldNotBe(0);
            });

            StartScenario();
        }
    }
}
