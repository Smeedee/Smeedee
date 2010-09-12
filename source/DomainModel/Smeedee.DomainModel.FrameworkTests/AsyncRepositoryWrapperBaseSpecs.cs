using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.SemanticModel;
using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModel.FrameworkTests.AsyncRepositoryWrapperBaseSpecs
{
    public class Shared : ScenarioClass
    {
        static protected Context the_asyncRepo_is_valid = () =>
        {
            asyncRepoMock.Setup(asr => asr.BeginGet(new AllChangesetsSpecification())).Callback(() =>
            {
                ThreadPool.QueueUserWorkItem(o =>
                    {
                        Thread.Sleep(500);
                        asyncRepoMock.Raise(asr => asr.GetCompleted += null,
                            new GetCompletedEventArgs<Changeset>(new List<Changeset> { new Changeset() }, new AllChangesetsSpecification()));

                    });
            });

            asyncRepository = asyncRepoMock.Object;
        };

        static protected Context the_asyncRepo_is_null = () =>
        {
            asyncRepository = null;
        };

        static protected When the_genericRepository_is_spawned = () =>
        {
            AsyncRepositoryWrapperBase = new AsyncRepositoryWrapperBase<Changeset>(asyncRepository);
        };

        static protected IAsyncRepository<Changeset> asyncRepository = null;
        static protected AsyncRepositoryWrapperBase<Changeset> AsyncRepositoryWrapperBase = null;
        static protected Mock<IAsyncRepository<Changeset>> asyncRepoMock = new Mock<IAsyncRepository<Changeset>>();
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
            Then("It should throw an argumentexception", () =>
                new AsyncRepositoryWrapperBase<Changeset>(null));

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
                Then(() => (AsyncRepositoryWrapperBase as IRepository<Changeset>).ShouldNotBeNull());
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
                var result = AsyncRepositoryWrapperBase.Get(new AllChangesetsSpecification());
                result.Count().ShouldBe(0);
            });
            StartScenario();
        }

        [Test]
        public void the_second_call_should_not_return_empty()
        {
            Then(() =>
            {
                AsyncRepositoryWrapperBase.Get(new AllChangesetsSpecification());
                Thread.Sleep(700);
                var result = AsyncRepositoryWrapperBase.Get(new AllChangesetsSpecification());
                result.Count().ShouldNotBe(0);
            });
            StartScenario();
        }
    }
}
