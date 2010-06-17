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
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Tasks.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.SchedulerTests.SchedulerSpecs
{
    public class Shared
    {
        protected static Scheduler.Scheduler scheduler;
        protected static int taskDispatchThreadID;
        protected static int schedulerThreadID;
        protected static Mock<ILog> loggerMock;
        protected static Mock<TaskBase> taskMock;
        protected static Mock<TaskBase> slowTaskMock;
        protected static Mock<TaskBase> failingTaskMock;
        protected static int failingTaskMockCallCounter;

        protected const int SIMPLE_TASK_INTRVAL_SEC = 1;
        protected const int SLOW_TASK_INTRVAL_SEC = 2;
        private static void createTaskMocks()
        {
            taskDispatchThreadID = 0;
            taskMock = new Mock<TaskBase>();
            taskMock.Setup(h => h.Execute()).Callback( () =>
            {
                taskDispatchThreadID = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("..mock task fetching data ");
            });
            taskMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SIMPLE_TASK_INTRVAL_SEC));
            taskMock.SetupGet(h => h.Name).Returns("Simple Mock task");


            slowTaskMock = new Mock<TaskBase>();
            slowTaskMock.Setup(h => h.Execute()).Callback(() =>
            {
                Console.WriteLine("..SLOW mock task fetching data ");
            });
            slowTaskMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SLOW_TASK_INTRVAL_SEC));
            slowTaskMock.SetupGet(h => h.Name).Returns("SLOW Mock task");

            
            failingTaskMockCallCounter = 0;
            failingTaskMock = new Mock<TaskBase>();
            failingTaskMock.Setup(h => h.Execute()).Callback(() =>
            {
                failingTaskMockCallCounter++;
                throw new Exception("I am a bad task, I crash");
            });
            failingTaskMock.SetupGet(h => h.Interval).Returns(new TimeSpan(0, 0, SIMPLE_TASK_INTRVAL_SEC));
            failingTaskMock.SetupGet(h => h.Name).Returns("Failing Mock task");
        }

        #region Test contexts
        protected Context the_scheduler_is_instanced = () =>
        {
            loggerMock = new Mock<ILog>();
            loggerMock.Setup( l => l.WriteEntry(It.IsAny<LogEntry>())).Callback((LogEntry log) => 
                    Console.WriteLine("{0}:({1}){2}: {3}",DateTime.Now, log.Severity, log.Source,log.Message)
                    );
            scheduler = new Scheduler.Scheduler(loggerMock.Object);
            schedulerThreadID = Thread.CurrentThread.ManagedThreadId;
        };

        protected Context a_task_registered = () =>
        {
            createTaskMocks();
            var tasks = new List<TaskBase>();
            tasks.Add(taskMock.Object);
            scheduler.RegisterTasks(tasks);
        };
        protected Context several_tasks_register = () =>
        {
            createTaskMocks();
            var tasks = new List<TaskBase>();
            tasks.Add(taskMock.Object);
            tasks.Add(slowTaskMock.Object);
            scheduler.RegisterTasks(tasks);
        };
        protected Context a_bad_task_registeres = () =>
        {
            createTaskMocks();
            var tasks = new List<TaskBase>();
            tasks.Add(failingTaskMock.Object);
            scheduler.RegisterTasks(tasks);
        };
#endregion

        protected static void WaitForMockDispatch(Mock<TaskBase> mock)
        {
            Thread.Sleep(5000);
            mock.Verify(h => h.Execute(), Times.AtLeastOnce());
        }

        protected When task_has_been_dispatched = () => WaitForMockDispatch(taskMock);
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
        public void assure_taskss_are_dispatched_on_separate_threads_from_scheduler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                    And(a_task_registered);

                scenario.When(task_has_been_dispatched);

                scenario.Then("assure data loading is performed on a different thread from the scheduler", () =>
                    taskDispatchThreadID.ShouldNotBe(schedulerThreadID));
            });
        }

        [Test]
        public void assure_tasks_are_called_at_specified_intervals()
        {
            Scenario.StartNew(this, scenario =>
            {
                int dispatchesToExpect = 5;
                int waitTime = (SIMPLE_TASK_INTRVAL_SEC * dispatchesToExpect) * 1000;
                waitTime += 2000; //wait a bit before asserting

                scenario.Given(the_scheduler_is_instanced).
                         And(a_task_registered);
                scenario.When("some time has gone by", () => Thread.Sleep(waitTime));
                scenario.Then("the task has been dispatched the correct number of times", () =>
                {
                    taskMock.Verify(h => h.Execute(), Times.AtLeast(dispatchesToExpect));
                });
            }); 
        }


        [Test]
        public void assure_multiple_tasks_are_dispatched_correctly()
        {
            Scenario.StartNew(this, scenario =>
            {
                int dispatchesToExpect = 5;
                int slowDispatchesToExpect = 3;
                int waitTime = (SLOW_TASK_INTRVAL_SEC * dispatchesToExpect) * 1000;
                waitTime += 1000;

                scenario.Given(the_scheduler_is_instanced).
                         And(several_tasks_register);
                scenario.When("some time has gone by", () => Thread.Sleep(waitTime));
                scenario.Then("the tasks have been dispatched the correct number of times", () =>
                {
                    taskMock.Verify    (h => h.Execute(), Times.AtLeast(dispatchesToExpect));
                    slowTaskMock.Verify(h => h.Execute(), Times.AtLeast(slowDispatchesToExpect));
                });
            });
        }

        [Test]
        public void assure_can_handle_exceptions_from_dispatched_task()
        {
            Scenario.StartNew(this, scenario =>
            {
                int attempts = 4;
                int waitTime = attempts * SIMPLE_TASK_INTRVAL_SEC;

                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_task_registeres);

                scenario.When("task has been dispatched that, throws an exception", () =>
                    WaitForMockDispatch(failingTaskMock));

                scenario.Then("the scheduler continues to dispatch task", () =>
                {
                    (failingTaskMockCallCounter >= 2).ShouldBeTrue();
                });
            });
        }

        [Test]
        public void assure_event_is_fired_when_task_throws_exception()
        {
            bool beenNotified = false;
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_task_registeres).
                         And("something listens to the event", () =>
                         {
                             scheduler.OnFailedDispatch += () =>
                             {
                                 beenNotified = true;
                             };
                         });

                scenario.When("task has been dispatched that, throws an exception", () =>
                    WaitForMockDispatch(failingTaskMock));

                scenario.Then("an event should fire", () =>
                {
                    beenNotified.ShouldBeTrue();
                });
            });
        }

        [Test]
        public void assure_will_put_failing_tasks_in_cooldown()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_scheduler_is_instanced).
                         And(a_bad_task_registeres);

                scenario.When("task has been dispatched, that throws an exception", () =>
                    WaitForMockDispatch(failingTaskMock));

                scenario.Then("the scheduler dispatches this task only THREE times", () =>
                {
                    failingTaskMockCallCounter.ShouldBe(3);
                });
            });
        }
    }
}
