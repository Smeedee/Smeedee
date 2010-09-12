using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Scheduler.Services;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Scheduler.Tests
{
    [TestFixture]
    public class When_Scheduler_creates_tasks : Shared
    {
        [Test]
        public void assure_it_is_able_to_find_types_with_a_specific_task_name_based_on_its_attributes()
        {
            Given(a_list_of_two_task_instance_configurations_of_different_types);

            When(the_scheduler_creates_tasks);

            Then("the list of ready tasks should contain tasks of all matching (TaskConfiguration, Type) - pairs", () => 
                scheduler.InstantiatedTasks.Count().ShouldBe(2));
        }

        [Test]
        public void assure_it_instantiate_the_correct_tasks()
        {
            Given(a_list_of_two_task_instance_configurations_of_different_types);

            When(the_scheduler_creates_tasks);

            Then("the list of ready tasks should contain one instance of Task1 and one instance of Task2", () =>
            {
                scheduler.InstantiatedTasks.Where(t => t.GetType() == typeof(Task1)).Count().ShouldBe(1);
                scheduler.InstantiatedTasks.Where(t => t.GetType() == typeof(Task2)).Count().ShouldBe(1);
            });
        }

        [Test]
        public void assure_it_is_able_to_instantiate_up_several_different_instances_of_the_same_type()
        {
            Given(two_task_instance_configurations_of_the_same_type);

            When(the_scheduler_creates_tasks);

            Then("the list of ready tasks should contain two different instances of Task1", () =>
            {
                scheduler.InstantiatedTasks.Count().ShouldBe(2);
            });
        }

        [Test]
        public void assure_it_gives_the_tasks_correct_config()
        {
            Given(one_task_configuration);

            When(the_scheduler_creates_tasks);

            Then("the tasks should have the correct configuration", () =>
            {
                var task = ((Task1) scheduler.InstantiatedTasks.First());
                task.Username.ShouldBe("Håkon");
                task.Password.ShouldBe("1234");
            });
        }

        [Test]
        public void assure_instances_of_the_same_type_are_differentiated_on_their_configuration_parameters()
        {
            Given(two_task_instance_configurations_of_the_same_type);

            When(the_scheduler_creates_tasks);

            Then("the list of ready tasks should contain two different instances of Task1", () =>
            {
                scheduler.InstantiatedTasks.Where(t => t.GetType() == typeof (Task1)).Count().ShouldBe(2);
                scheduler.InstantiatedTasks.Cast<Task1>().Select(t => t.Username).ShouldContain("Håkon");
                scheduler.InstantiatedTasks.Cast<Task1>().Select(t => t.Username).ShouldContain("Zorro");
            });
        }

        [Test]
        public void assure_no_task_of_a_specific_type_are_initiated_when_it_does_not_find_the_desired_task_type()
        {
            Given(a_task_instance_configuration_of_a_none_existing_type);

            When(the_scheduler_creates_tasks);

            Then("the list of tasks ready for excecution should be empty", () =>
                scheduler.InstantiatedTasks.Count().ShouldBe(0)); 

        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void assure_it_fetches_new_tasks_from_the_repository()
        {
            Given(a_list_of_two_task_instance_configurations_of_different_types);

            When(the_scheduler_is_notified_to_refresh);

            Then("the new tasks should have been added to the list of ready tasks", () =>
            {
                scheduler.InstantiatedTasks.Count().ShouldBe(2);
            });
        }
    }

    [TestFixture]
    public class When_scheduler_fetches_tasks : Shared
    {
        [Test]
        public void assure_already_existing_tasks_are_not_added_as_new_tasks()
        {
            Given(a_list_of_two_task_instance_configurations_of_different_types);

            When(the_scheduler_creates_tasks);

            Then("no new tasks should have been added", () =>
            {
                scheduler.InstantiatedTasks.Count().ShouldBe(2);
            });
        }

        [Test]
        public void assure_it_updates_already_existing_tasks()
        {

            Given("a repository which returns an updated task configuration the second time it is called Get() on", () =>
            {
                var repositoryForTesting = new TaskConfigurationRepositoryForTestingUpdate();
                iocContainer.BindToConstant<IRepository<TaskConfiguration>>(repositoryForTesting);
                scheduler = new Scheduler(iocContainer, folderToSearchForTasks, fileUtilityMock.Object, taskConfigTimerMock.Object, dispatchTimerMock.Object, loggerMock.Object);
                scheduler.Start();
            });

            When("the scheduler fetches task configruations the second time", () => scheduler.Start());

            Then("the list of ready tasks should contain the updated settings", () =>
            {
                scheduler.InstantiatedTasks.Count().ShouldBe(2);
                var task1 = (Task1) scheduler.InstantiatedTasks.Single(t => t.GetType() == typeof (Task1));
                task1.Username.ShouldBe(UPDATED_VALUE_IN_TASK_ENTRY);
            });
        }

        [Test]
        public void assure_deleted_task_configurations_are_not_instantiated()
        {
            Given("a repository which returns one task configuration less the second time it is called Get() on", () =>
            {
                var repositoryForTesting = new TaskConfigurationRepositoryForTestingDelete();
                iocContainer.BindToConstant<IRepository<TaskConfiguration>>(repositoryForTesting);
                scheduler = new Scheduler(iocContainer, folderToSearchForTasks, fileUtilityMock.Object, taskConfigTimerMock.Object, dispatchTimerMock.Object, loggerMock.Object);
                scheduler.Start();
            });

            When(the_scheduler_is_notified_to_refresh);

            Then("the list of instantiated tasks should no longer contain the task that was deleted", () =>
            {
                scheduler.InstantiatedTasks.Count().ShouldBe(1);
            });
        }

        [Test]
        public void assure_last_dispatch_time_is_not_reset_when_updating_configuration()
        {
           
            var repositoryForTesting = new TaskConfigurationRepositoryForTestingUpdate();

            iocContainer.BindToConstant<IRepository<TaskConfiguration>>(repositoryForTesting);
            scheduler = new Scheduler(iocContainer, folderToSearchForTasks, fileUtilityMock.Object, taskConfigTimerMock.Object, dispatchTimerMock.Object, loggerMock.Object);
            scheduler.Start();
            Thread.Sleep(50);
            the_scheduler_is_told_to_dispatch();
            Thread.Sleep(50);
            the_scheduler_is_notified_to_refresh();
            Thread.Sleep(50);

            foreach (var task in scheduler.TaskMetaDatas)
            {
                Console.WriteLine(task.InstanceName + ": " + task.LastDispatch);
                task.LastDispatch.ShouldNotBe(DateTime.MinValue);
            }
        }
    }

    [TestFixture]
    public class When_dispatching : Shared
    {
        [Test]
        public void assure_tasks_are_dispatched_on_separate_threads_from_scheduler()
        {
            Given(one_task_configuration)
                .And(the_scheduler_has_started);

            When(the_scheduler_is_told_to_dispatch);

            Then("assure data loading is performed on a different thread from the scheduler", () =>
            {
                var task1 = (Task1) scheduler.InstantiatedTasks.Single(t => t.Name == FIRST_TASK1_NAME);
                task1.ThreadId.ShouldNotBe(Thread.CurrentThread.ManagedThreadId);

            });
        }

        [Test]
        public void assure_tasks_are_called_at_specified_intervals()
        {

            Given(one_task_configuration)
                .And(the_scheduler_has_started);

            When("the scheduler has dispatched several times", () =>
            {
                var time = DateTime.Now;

                for (int i = 0; i < 5; i++)
                {
                    time = time.AddHours(1);
                    dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(time));
                    Thread.Sleep(50);
                }
            });
            
            Then("after some time has gone by, the task has been dispatched the correct number of times", () =>
            {
                var task1 = (Task1) scheduler.InstantiatedTasks.Single(t => t.Name == FIRST_TASK1_NAME);
                task1.NumberOfTimesExecuted.ShouldBe(5);
            });
        }

        [Test]
        public void assure_multiple_tasks_are_dispatched_correctly()
        {

            Scenario("One task which is able to run every 5 minute and one task which is able to run every 10 minute");

            Given(a_list_of_two_task_instance_configurations_of_different_types)
                .And(the_scheduler_has_started);
            
            When("the scheduler has dispatched several times", () =>
            {
                var time = DateTime.Now;

                for (int i = 0; i < 20; i++)
                {
                    time = time.AddMinutes(5);
                    dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(time));
                    Thread.Sleep(5);
                }
            });

            Then("one task should have excecuted about twice as many times as the other", () =>
            {
                var task1 = (Task1) scheduler.InstantiatedTasks.Single(t => t.Name == FIRST_TASK1_NAME);
                var task2 = (Task2) scheduler.InstantiatedTasks.Single(t => t.Name == TASK2_NAME);

                task1.NumberOfTimesExecuted.ShouldBeGreaterThan(17);
                task1.NumberOfTimesExecuted.ShouldBeLessThan(23);
                task2.NumberOfTimesExecuted.ShouldBeGreaterThan(8);
                task2.NumberOfTimesExecuted.ShouldBeLessThan(12);
            });

        }

        [Test]
        public void assure_tasks_are_not_dispatched_when_their_interval_time_has_not_elapsed()
        {
            Scenario("A task which has a dispatch interval on 5 minutes");

            Given(one_task_configuration)
                .And(the_scheduler_has_started);

            When("the scheduler has dispatched 12 times with 1 minute between the dispatches", () =>
            {
                var time = DateTime.Now;

                for (int i = 0; i < 12; i++)
                {
                    time = time.AddMinutes(1);
                    dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(time));
                    Thread.Sleep(5);
                }
            });

            Then("the task should be executed 3 times", () =>
            {
                var task1 = (Task1)scheduler.InstantiatedTasks.Single(t => t.Name == FIRST_TASK1_NAME);
                task1.NumberOfTimesExecuted.ShouldBe(3);
            });
        }


        [Test]
        public void assure_it_can_handle_exceptions_from_dispatched_task()
        {
            Given(one_failing_and_one_non_failing_task)
                .And(the_scheduler_has_started);

            When("the failing task has executed and therefore thrown an exception", () =>
            {
                dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
                var taskThatWillFail = (FailingTask) scheduler.InstantiatedTasks.Single(t => t.Name == FAILING_TASK_NAME);

                WaitUntil(() => taskThatWillFail.NumberOfTimesExecuted >= 1);

                var task2 = (Task2) scheduler.InstantiatedTasks.Single(t => t.Name == TASK2_NAME);
                int timesExcecutedBeforeDispatch = task2.NumberOfTimesExecuted;
                dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now.AddHours(1)));
                WaitUntil(() => task2.NumberOfTimesExecuted > timesExcecutedBeforeDispatch);
            });

            Then("the scheduler should continue to dispatch tasks", () =>
            {
                var task2 = (Task2) scheduler.InstantiatedTasks.Single(t => t.Name == TASK2_NAME);
                task2.NumberOfTimesExecuted.ShouldBeGreaterThan(1);
            });
        }




        [Test]
        public void assure_will_put_failing_tasks_in_cooldown()
        {
            Given(a_task_configuration_with_a_failing_task)
                .And(the_scheduler_has_started);

            When("a failing task has been dispatched, and thus throw an exception", () =>
            {
                var time = DateTime.Now;

                for (int i = 0; i < 10; i++)
                {
                    time = time.AddSeconds(20);
                    dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(time));
                    Thread.Sleep(50);
                }
            });
                   
            
            Then("the scheduler should have dispatched this task only THREE times, due to its cooldown period", () =>
            {
                var failingTask = (FailingTask) scheduler.InstantiatedTasks.Single(t => t.Name == FAILING_TASK_NAME);
                (failingTask.NumberOfTimesExecuted).ShouldBe(3);
            });
        }
    }

    public class Shared : ScenarioClass
    {
        protected const string FIRST_TASK1_NAME = "Some task of type: Task1";
        protected const string SECOND_TASK1_NAME = "A second task of type: Task1";
        protected const string TASK2_NAME = "Some task of type: Task2";
        protected const string FAILING_TASK_NAME = "A failing task";
        protected const string UPDATED_VALUE_IN_TASK_ENTRY = "updated value";
        protected const int TIME_OUT_LIMIT_MS = 500;
        
        protected static string folderToSearchForTasks = Directory.GetCurrentDirectory();
        protected static IEnumerable<TaskConfiguration> taskConfigs;
        protected static List<TaskConfiguration> taskConfigurations;
        protected static IocContainerForScheduler iocContainer;
        protected static Scheduler scheduler;

        protected static Mock<ILog> loggerMock;
        protected static Mock<IRepository<TaskConfiguration>> configMock;
        protected static Mock<ITimerWithTimestamp> taskConfigTimerMock;
        protected static Mock<ITimerWithTimestamp> dispatchTimerMock;
        protected static Mock<IFileUtilities> fileUtilityMock; 
        
        protected static TaskConfiguration firstTask1Config;
        protected static TaskConfiguration secondTask1Config;
        protected static TaskConfiguration task2Config;
        protected static TaskConfiguration failingTask;


        [SetUp]
        public void SetUp()
        {
            Scenario("");
            taskConfigurations = new List<TaskConfiguration>();

            SetupMocks();
            CreateTaskConfigurations();

            iocContainer = new IocContainerForScheduler();
            iocContainer.BindToConstant(configMock.Object);
            scheduler = new Scheduler(iocContainer, folderToSearchForTasks, fileUtilityMock.Object, taskConfigTimerMock.Object, dispatchTimerMock.Object, loggerMock.Object);
        }

        private void CreateTaskConfigurations()
        {
            firstTask1Config = new TaskConfiguration
                                   {
                                       Name = FIRST_TASK1_NAME,
                                       TaskName = "Task 1",
                                       Entries = new List<TaskConfigurationEntry> { Entry("Username", "Håkon", typeof(string)), Entry("Password", "1234", typeof(string)) }
                                   };
            secondTask1Config = new TaskConfiguration
                                    {
                                        Name = SECOND_TASK1_NAME,
                                        TaskName = "Task 1",
                                        Entries = new List<TaskConfigurationEntry> { Entry("Username", "Zorro", typeof(string)), Entry("Password", "666", typeof(string)) }
                                    };
            task2Config = new TaskConfiguration
                                   {
                                       Name = TASK2_NAME,
                                       TaskName = "Task 2",
                                       Entries = new List<TaskConfigurationEntry> { Entry("PrimesToGenerate", 100, typeof(int)) }
                                   };
            failingTask = new TaskConfiguration
                              {
                                  Name = FAILING_TASK_NAME,
                                  TaskName = "Failing Task"
                              };
        }

        private void SetupMocks()
        {
            loggerMock = new Mock<ILog>();
            loggerMock.Setup(l => l.WriteEntry(It.IsAny<LogEntry>()))
                .Callback((LogEntry log) =>
                          Console.WriteLine("{0}:({1}){2}: {3}", DateTime.Now, log.Severity, log.Source, log.Message));

            configMock = new Mock<IRepository<TaskConfiguration>>();
            configMock.Setup(r => r.Get(It.IsAny<AllSpecification<TaskConfiguration>>())).Returns(taskConfigurations);
            dispatchTimerMock = new Mock<ITimerWithTimestamp>();
            taskConfigTimerMock = new Mock<ITimerWithTimestamp>();
            fileUtilityMock = new Mock<IFileUtilities>();
            var availableTypes = new List<Type> {typeof (Task1), typeof (Task2), typeof (FailingTask)};
            fileUtilityMock.Setup(t => t.FindImplementationsOrSubclassesOf<TaskBase>(It.IsAny<string>())).Returns(availableTypes);
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }



        protected Context the_scheduler_has_started = () =>
        {
            scheduler.Start();
        };


        protected Context one_task_configuration = () =>
        {
            taskConfigurations.Add(firstTask1Config);
        };

        protected Context a_list_of_two_task_instance_configurations_of_different_types = () =>
        {
            taskConfigurations.Add(firstTask1Config);
            taskConfigurations.Add(task2Config);
        };

        protected Context two_task_instance_configurations_of_the_same_type = () =>
        {
            taskConfigurations.Add(firstTask1Config);
            taskConfigurations.Add(secondTask1Config);
        };

        protected Context a_task_instance_configuration_of_a_none_existing_type = () =>
        {
            taskConfigurations.Add(new TaskConfiguration
                                    {
                                        Name = "My none exisiting task",
                                        TaskName = "Not existing"
                                    });
        };

        protected Context a_task_configuration_with_a_failing_task = () =>
        {
            taskConfigurations.Add(failingTask);
        };

        protected Context one_failing_and_one_non_failing_task = () =>
        {
            taskConfigurations.Add(task2Config);
            taskConfigurations.Add(failingTask);
        };

        protected When the_scheduler_is_notified_to_refresh = () =>
        {
            taskConfigTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
        };

        protected When the_scheduler_creates_tasks = () =>
        {
            scheduler.Start();
        };

        protected When the_scheduler_is_told_to_dispatch = () => dispatchTimerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));

        private static TaskConfigurationEntry Entry(string name, object value, Type type)
        {
            return new TaskConfigurationEntry
                       {
                           Name = name,
                           Value = value,
                           Type = type
                       };
        }

        protected void WaitUntil(Func<bool> isTrue)
        {
            int counter = 0;
            while (!isTrue() && counter < TIME_OUT_LIMIT_MS)
            {
                counter++;
                Thread.Sleep(1);
            }
        }

        #region Hardcoded TaskConfigurationRepositories used for testing

        protected class TaskConfigurationRepositoryForTestingUpdate : IRepository<TaskConfiguration>
        {
            public int counter;

            public IEnumerable<TaskConfiguration> Get(Specification<TaskConfiguration> specification)
            {
                if (counter < 1)
                {
                    counter++;
                    taskConfigurations.Clear();
                    taskConfigurations.Add(firstTask1Config);
                    taskConfigurations.Add(task2Config);
                }
                else if (counter == 1)
                {
                    taskConfigurations.First().Entries.First().Value = UPDATED_VALUE_IN_TASK_ENTRY;
                    counter++;
                }
                return taskConfigurations;
            }
        }

        protected class TaskConfigurationRepositoryForTestingDelete : IRepository<TaskConfiguration>
        {
            public int counter;

            public IEnumerable<TaskConfiguration> Get(Specification<TaskConfiguration> specification)
            {
                if (counter < 1)
                {
                    counter++;

                    taskConfigurations.Add(firstTask1Config);
                    taskConfigurations.Add(task2Config);
                }
               
                else if (counter == 1)
                {
                    taskConfigurations.Remove(taskConfigurations.Last());
                    counter++;
                    
                }

                return taskConfigurations.ToList();
            }
        }

        #endregion
    }

    #region Tasks used for testing

    public class TaskForTesting : TaskBase
    {
        public int NumberOfTimesExecuted;
        public int ThreadId;

        public override string Name { get; set; }
        public override TimeSpan Interval { get; set; }

        public TaskForTesting(TaskConfiguration configuration)
        {
            Name = configuration.Name;
        }

        public override void Execute()
        {
            NumberOfTimesExecuted++;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }

    [Task("Task 1")]
    [TaskSetting("Username", typeof(string), "guest")]
    [TaskSetting("Password", typeof(string), "")]
    public class Task1 : TaskForTesting
    {
        public string Username;
        public string Password;

        public Task1(TaskConfiguration configuration) : base(configuration)
        {
            Interval = TimeSpan.FromMinutes(5);
            Username = (string) configuration.Entries.Single(t => t.Name == "Username").Value;
            Password = (string) configuration.Entries.Single(t => t.Name == "Password").Value;
        }
    }

    [Task("Task 2")]
    [TaskSetting("PrimesToGenerate", typeof(int), "0")]
    public class Task2 : TaskForTesting
    {
        public int PrimesToGenerate;

        public Task2(TaskConfiguration configuration) : base(configuration)
        {
            Interval = TimeSpan.FromMinutes(10);
            PrimesToGenerate = (int) configuration.Entries.Single(t => t.Name == "PrimesToGenerate").Value;
        }
    }

    [Task("Failing Task")]
    public class FailingTask : TaskForTesting
    {

        public FailingTask(TaskConfiguration configuration) : base(configuration)
        {
            Interval = TimeSpan.FromMilliseconds(2000);
        }

        public override void Execute()
        {
            base.Execute();
            throw new Exception("this is a failing task");
        }
    }

    public static class TestExtensions
    {
        public static void ShouldBeGreaterThan(this int actual, int expected)
        {
            Assert.Greater(actual, expected);
        }
    }

    #endregion
}