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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using APD.DataCollector;
using APD.DomainModel.Framework.Logging;

using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Harvester.Framework;


namespace APD.DataCollectorTests.SchedulerSpecs
{
    public class Shared
    {
        protected static Scheduler scheduler;
        protected static int harvesterDispatchThreadID;
        protected static int schedulerThreadID;
        protected static Mock<ILog> loggerMock;
        protected static Mock<AbstractHarvester> harvesterMock;
        protected static Mock<AbstractHarvester> slowHarvesterMock;
        protected static Mock<AbstractHarvester> failingHarvesterMock;
        protected static int failingHarvesterMockCallCounter;

        protected const int SIMPLE_HARVESTER_INTRVAL_SEC = 1;
        protected const int SLOW_HARVESTER_INTRVAL_SEC = 2;
        private static void createHarvesterMocks()
        {
            harvesterDispatchThreadID = 0;
            harvesterMock = new Mock<AbstractHarvester>();
            harvesterMock.Setup(h => h.DispatchDataHarvesting()).Callback( () =>
            {
                harvesterDispatchThreadID = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("..mock harvester fetching data ");
            });
            harvesterMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SIMPLE_HARVESTER_INTRVAL_SEC));
            harvesterMock.SetupGet(h => h.Name).Returns("Simple Mock Harvester");


            slowHarvesterMock = new Mock<AbstractHarvester>();
            slowHarvesterMock.Setup(h => h.DispatchDataHarvesting()).Callback(() =>
            {
                Console.WriteLine("..SLOW mock harvester fetching data ");
            });
            slowHarvesterMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SLOW_HARVESTER_INTRVAL_SEC));
            slowHarvesterMock.SetupGet(h => h.Name).Returns("SLOW Mock Harvester");

            
            failingHarvesterMockCallCounter = 0;
            failingHarvesterMock = new Mock<AbstractHarvester>();
            failingHarvesterMock.Setup(h => h.DispatchDataHarvesting()).Callback(() =>
            {
                failingHarvesterMockCallCounter++;
                throw new Exception("I am a bad harvester, I crash");
            });
            failingHarvesterMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SIMPLE_HARVESTER_INTRVAL_SEC));
            failingHarvesterMock.SetupGet(h => h.Name).Returns("Failing Mock Harvester");
        }

#region Test contexts
        protected Context the_scheduler_is_instanced = () =>
        {
            loggerMock = new Mock<ILog>();
            loggerMock.Setup( l => l.WriteEntry(It.IsAny<LogEntry>())).Callback((LogEntry log) => 
                    Console.WriteLine("{0}:({1}){2}: {3}",DateTime.Now, log.Severity, log.Source,log.Message)
                    );
            scheduler = new Scheduler(loggerMock.Object);
            schedulerThreadID = Thread.CurrentThread.ManagedThreadId;
        };

        protected Context a_harvester_registered = () =>
        {
            createHarvesterMocks();
            var harvesters = new List<AbstractHarvester>();
            harvesters.Add(harvesterMock.Object);
            scheduler.RegisterHarvesters(harvesters);
        };
        protected Context several_harvesters_register = () =>
        {
            createHarvesterMocks();
            var harvesters = new List<AbstractHarvester>();
            harvesters.Add(harvesterMock.Object);
            harvesters.Add(slowHarvesterMock.Object);
            scheduler.RegisterHarvesters(harvesters);
        };
        protected Context a_bad_harvester_registeres = () =>
        {
            createHarvesterMocks();
            var harvesters = new List<AbstractHarvester>();
            harvesters.Add(failingHarvesterMock.Object);
            scheduler.RegisterHarvesters(harvesters);
        };
#endregion

        protected static void WaitForMockDispatch(Mock<AbstractHarvester> mock)
        {
            Thread.Sleep(5000);
            mock.Verify(h => h.DispatchDataHarvesting(), Times.AtLeastOnce());
        }

        protected When harvester_has_been_dispatched = () => WaitForMockDispatch(harvesterMock);
    }

    #region When Spawned
    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_be_instanced()
        {

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced);
                scenario.When("the scheduler is accessed");
                scenario.Then("it should be instanced", () =>
                    scheduler.ShouldNotBeNull());
            });
        }

        [Test]
        public void should_write_to_log_on_startup()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced);
                scenario.When("the log is checked");
                scenario.Then("it should have increased in size", () => 
                    loggerMock.Verify(l => l.WriteEntry(It.IsAny<InfoLogEntry>()), Times.Once())
                    );
            });
        }
    }
    #endregion

    [TestFixture]
    public class during_operation : Shared
    {
        [TearDown]
        public void Teardown()
        {
            scheduler.Dispose();
        }

        [Test]
        public void assure_harvesters_are_dispatched_on_separate_threads_from_scheduler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                    And(a_harvester_registered);

                scenario.When(harvester_has_been_dispatched);

                scenario.Then("assure data loading is performed on a different thread from the scheduler", () =>
                    harvesterDispatchThreadID.ShouldNotBe(schedulerThreadID));
            });
        }

        [Test]
        public void assure_harvesters_are_called_at_specified_intervals()
        {
            Scenario.StartNew(this, scenario =>
            {
                int dispatchesToExpect = 5;
                int waitTime = (SIMPLE_HARVESTER_INTRVAL_SEC * dispatchesToExpect) * 1000;
                waitTime += 2000; //wait a bit before asserting

                scenario.Given(the_scheduler_is_instanced).
                         And(a_harvester_registered);
                scenario.When("some time has gone by", () => Thread.Sleep(waitTime));
                scenario.Then("the harvester has been dispatched the correct number of times", () =>
                {
                    harvesterMock.Verify(h => h.DispatchDataHarvesting(), Times.AtLeast(dispatchesToExpect));
                });
            }); 
        }


        [Test]
        public void assure_multiple_harvesters_are_dispatched_correctly()
        {
            Scenario.StartNew(this, scenario =>
            {
                int dispatchesToExpect = 5;
                int slowDispatchesToExpect = 3;
                int waitTime = (SLOW_HARVESTER_INTRVAL_SEC * dispatchesToExpect) * 1000;
                waitTime += 1000;

                scenario.Given(the_scheduler_is_instanced).
                         And(several_harvesters_register);
                scenario.When("some time has gone by", () => Thread.Sleep(waitTime));
                scenario.Then("the harvesters have been dispatched the correct number of times", () =>
                {
                    harvesterMock.Verify    (h => h.DispatchDataHarvesting(), Times.AtLeast(dispatchesToExpect));
                    slowHarvesterMock.Verify(h => h.DispatchDataHarvesting(), Times.AtLeast(slowDispatchesToExpect));
                });
            });
        }

        [Test]
        public void assure_can_handle_exceptions_from_dispatched_harvester()
        {
            Scenario.StartNew(this, scenario =>
            {
                int attempts = 4;
                int waitTime = attempts * SIMPLE_HARVESTER_INTRVAL_SEC;

                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_harvester_registeres);

                scenario.When("harvester has been dispatched that, throws an exception", () =>
                    WaitForMockDispatch(failingHarvesterMock));

                scenario.Then("the scheduler continues to dispatch harvester", () =>
                {
                    (failingHarvesterMockCallCounter >= 2).ShouldBeTrue();
                });
            });
        }

        [Test]
        public void assure_event_is_fired_when_harvester_throws_exception()
        {
            bool beenNotified = false;
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_harvester_registeres).
                         And("something listens to the event", () =>
                         {
                             scheduler.OnFailedDispatch += () =>
                             {
                                 beenNotified = true;
                             };
                         });

                scenario.When("harvester has been dispatched that, throws an exception", () =>
                    WaitForMockDispatch(failingHarvesterMock));

                scenario.Then("an event should fire", () =>
                {
                    beenNotified.ShouldBeTrue();
                });
            });
        }

        [Test]
        public void assure_will_put_failing_harvesters_in_cooldown()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_harvester_registeres);

                scenario.When("harvester has been dispatched, that throws an exception", () =>
                    WaitForMockDispatch(failingHarvesterMock));

                scenario.Then("the scheduler dispatches this harvester only THREE times", () =>
                {
                    failingHarvesterMockCallCounter.ShouldBe(3);
                });
            });
        }
    }
}
