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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Moq;
using APD.DomainModel.Framework;
using System.Threading;
using APD.DomainModel.SourceControl;


namespace APD.DomainModel.FrameworkTests.StaticRepositoryCacheSpecs
{
    public class SlowRepository : IRepository<string>
    {
        public int GetCallCount { get; set; }

        #region IRepository<TDomainModel> Members

        public virtual IEnumerable<string> Get(Specification<string> specification)
        {
            Console.WriteLine("SlowRepository.Get() (" + Thread.CurrentThread.ManagedThreadId + ")");
            Console.WriteLine(DateTime.Now);
            Thread.Sleep(2000);
            GetCallCount += 1;
            var retValue = new List<string>();
            retValue.Add("get" + GetCallCount);
            return retValue;
        }

        #endregion
    }

    public class Shared
    {
        protected static Mock<IRepository<string>> repositoryMock;
        protected static IRepository<string> repository;
        protected static Mock<Specification<string>> specificationMock;
        protected static readonly int CACHE_TIME = 1000;
       
        protected Context Repository_is_created = () =>
        {
            repositoryMock = new Mock<IRepository<string>>();
            specificationMock = new Mock<Specification<string>>();

            repository = repositoryMock.Object;
        };

        protected Context Repository_contains_data = () =>
        {
            repositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<string>>())).
                    Returns(new List<string>());
        };

        protected Context decorated_with_StaticRepositoryCache = () =>
        {
            specificationMock = new Mock<Specification<string>>();
            var cache = new StaticRepositoryCache<string>(repository, CACHE_TIME);
            cache.Clear();
            repository = cache;
        };

        protected Context get_is_called = () =>
        {
            CallGet();
        };

        protected When get_is_called_again = () =>
        {
            CallGet();
        };

        private static void CallGet()
        {
            repository.Get(specificationMock.Object);
        }
    }

    [TestFixture]
    public class When_get : Shared
    {
        [Test]
        public void Assure_it_return_result_from_cache_when_called_more_than_once()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Repository_is_created).
                    And(decorated_with_StaticRepositoryCache).
                    And(get_is_called);

                scenario.When(get_is_called_again);

                scenario.Then("Assure repository is only called once", () =>
                    repositoryMock.Verify(r => r.Get(It.IsAny<Specification<string>>()), Times.Once()));
            });
        }

        [Test]
        public void Assure_cached_result_is_only_cached_for_n_amount_of_time()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Repository_is_created).
                    And(decorated_with_StaticRepositoryCache).
                    And("configured to cache every second").
                    And(get_is_called);

                scenario.When("get is called again after 1.5 second", () =>
                {
                    Thread.Sleep(1100);
                    repository.Get(specificationMock.Object);
                });

                scenario.Then("assure result from Repository is returned", () =>
                    repositoryMock.Verify(r => r.Get(It.IsAny<Specification<string>>()), Times.Exactly(2)));
            });
        }

        [Test]
        public void Assure_its_param_friendly()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Repository_is_created).
                    And(decorated_with_StaticRepositoryCache).
                    And(get_is_called);

                scenario.When("Get is called again with a new specification", () =>
                    repository.Get(new Mock<Specification<string>>().Object));

                scenario.Then("assure Repository is called", () =>
                    repositoryMock.Verify(r => r.Get(It.IsAny<Specification<string>>()), Times.Exactly(2)));
            });
        }

        private class MySpec : Specification<string>
        {

            public override System.Linq.Expressions.Expression<Func<string, bool>> IsSatisfiedByExpression()
            {
                return c => true;
            }
        }

        [Test]
        public void Assure_it_can_handle_to_be_called_from_multiple_threads()
        {
            Scenario.StartNew(this, scenario =>
            {
                var slowRepository = new SlowRepository();
                const int getNoTimesToExecute = 50;
                var completedGetRequests = 0;
                var resetEvent = new ManualResetEvent(false);
                var mySpec = new MySpec();
                var anotherSpec = new MySpec();

                scenario.Given("Repository is created", () =>
                        repository = slowRepository).
                    And("decorated with StaticRepositoryCache", () =>
                    {
                        specificationMock = new Mock<Specification<string>>();
                        repository = new StaticRepositoryCache<string>(repository, 5000);
                    });

                scenario.When("Get is called multiple times on different threads", () =>
                {
                    for (int i = 0; i < getNoTimesToExecute; i++)
                    {
                        var currentCallNb = i;
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            if (currentCallNb < getNoTimesToExecute / 2)
                                repository.Get(mySpec);
                            else
                                repository.Get(anotherSpec);
                            
                            completedGetRequests++;

                            if (completedGetRequests == getNoTimesToExecute)
                                resetEvent.Set();
                        });
                    }
                });

                scenario.Then("Assure result from repository is returned only once for every specification", () =>
                {
                    resetEvent.WaitOne();
                    slowRepository.GetCallCount.ShouldBe(2);
                    Console.WriteLine("Repository.Get() called " + slowRepository.GetCallCount + " times");
                });
            });  
        }
    }

    namespace Integration
    {
        public class Shared
        {
            protected static Mock<IRepository<Changeset>> repositoryMock;
            protected static Mock<IRepository<Changeset>> repositoryMockTwo;
            protected static IRepository<Changeset> repository;
            protected static IRepository<Changeset> repositoryTwo;
            protected static int repositoryCallCount;
            protected static int repositoryTwoCallCount;

            protected Context two_Repositories_is_created = () =>
            {
                repositoryMock = new Mock<IRepository<Changeset>>();
                repositoryMockTwo = new Mock<IRepository<Changeset>>();
                repository = repositoryMock.Object;
                repositoryTwo = repositoryMockTwo.Object;
            };

            protected Context repositories_contains_data = () =>
            {
                var retValue = new List<Changeset>();
                retValue.Add(new Changeset());
                repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(retValue).
                    Callback(() => repositoryCallCount++);
                repositoryMockTwo.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(retValue).
                    Callback(() => repositoryTwoCallCount++);
            };

            protected Context thay_are_decorated_with_StaticRepositoryCache = () =>
            {
                repository = new StaticRepositoryCache<Changeset>(repository, 99000);
                repositoryTwo = new StaticRepositoryCache<Changeset>(repositoryTwo, 99000);
            };

            
        }

        [TestFixture]
        public class When_get : Shared
        {
            [Test]
            public void Assure_decorated_repositories_is_only_called_once()
            {
                Scenario.StartNew(this, scenario =>
                {
                    var resetEvent = new ManualResetEvent(false);
                    var nrOfRequests = 100;

                    scenario.Given(two_Repositories_is_created).
                        And(repositories_contains_data).
                        And(thay_are_decorated_with_StaticRepositoryCache);

                    scenario.When("get is called multiple times on multiple threads", () =>
                    {
                        for (int i = 0; i < nrOfRequests; i++ )
                        {
                            var nr = i;
                            ThreadPool.QueueUserWorkItem(o =>
                            {
                                Console.WriteLine("Request: " + nr);

                                
                                if (nr % 2 == 0)
                                    repository.Get(new AllChangesetsSpecification());
                                else
                                    repositoryTwo.Get(new AllChangesetsSpecification());

                                if (nr + 1 == nrOfRequests)
                                    resetEvent.Set();
                            });
                        }
                            
                    });

                    scenario.Then("assure Repositories is only called once", () =>
                    {
                        resetEvent.WaitOne();
                        Console.WriteLine("DecoratedRepository call count = " + repositoryCallCount);
                        Console.WriteLine("DecoratedRepositoryTwo call count = " + repositoryTwoCallCount);
                        
                        (repositoryCallCount + repositoryTwoCallCount).ShouldBe(1);
                        //repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Once());

                    });
                });
            }
        }
    }
}
