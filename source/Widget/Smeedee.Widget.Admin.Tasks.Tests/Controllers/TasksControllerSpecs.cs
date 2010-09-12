using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Tasks.Controllers;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Tasks.Tests.Controllers
{
    [TestFixture]
    public class When_initialized : Shared
    {
        [Test]
        public void Assure_that_there_are_no_Tasks()
        {
            Given("a controller");
            And(the_tasks_repository_contains_a_task);

            When("We count the number of tasks");

            Then("the tasks should be empty",
                 () => _viewModel.AvailableTasks.Count().ShouldBe(0));
        }

        [Test]
        public void Assure_that_there_are_no_TaskInstances()
        {
            Given("a controller");
            And(the_task_instances_repository_contains_a_task_instance);

            When("We count the number of task instances");

            Then("the count should be empty",
                 () => _viewModel.RunningTasks.Count().ShouldBe(0));
        }
    }

    [TestFixture]
    public class When_started : Shared
    {
        [Test]
        public void Assure_that_the_Tasks_were_loaded()
        {
            Given(the_tasks_repository_contains_a_task);
            And(the_controller_was_started);

            When("We count the number of tasks");

            Then("the tasks should contain a task",
                 () => _viewModel.AvailableTasks.Count().ShouldBe(1));
        }

        [Test]
        public void Assure_that_the_TaskInstances_were_loaded()
        {
            Given(the_task_instances_repository_contains_a_task_instance);
            And(the_controller_was_started);

            When("We count the number of task instances");

            Then("the task instances should contain a task instance",
                 () => _viewModel.RunningTasks.Count().ShouldBe(1));
        }

        [Test]
        public void assure_that_the_refreshnotifier_has_started()
        {
            Given("A controller");

            When("Start is called on the controller", () => _controller.Start());

            Then("refreshnotifier.start should have been called", () =>
                _refreshNotifierMock.Verify(n => n.Start(It.IsAny<int>()), Times.Exactly(1)));
        }
    }

    [TestFixture]
    public class When_saving : Shared
    {
        [Test]
        public void Assure_the_repository_was_asked_to_save()
        {
            Given(the_view_model_contains_a_task_instance);

            When("Save is called",
                 () => _controller.SaveChanges(null, null));

            Then("The domain model persister should have been asked to save",
                 () => _domainModelPersisterMock.Verify(p => p.Save(It.IsAny<IEnumerable<TaskConfiguration>>()), Times.Once()));
        }
    }

    [TestFixture]
    public class Notify_when_loading : Shared
    {
        [Test]
        public void assure_loading_notifyer_is_shown_while_loading_data()
        {
            Given("A controller");
            When("Start is called", () => _controller.Start());
            Then("assure loading notifier showInView has been called", () =>
                _loadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()), Times.AtLeastOnce()));
        }

        [Test]
        public void assure_loading_notifyer_is_removed_after_data_has_finished_loading()
        {
            Given("A controller");
            When("Start is called", () => _controller.Start());
            Then("assure loading loading notifier hide has been called",
                 () => _loadingNotifierMock.Verify(l => l.HideInBothViews(), Times.AtLeastOnce()));
        }
    }

    public class Shared : SmeedeeScenarioTestClass
    {
        protected static TasksController _controller;
        protected static TasksViewModel _viewModel;

        protected static Mock<IRepository<TaskDefinition>> _taskDefinitionRepositoryMock;
        protected static Mock<IRepository<TaskConfiguration>> _taskConfigurationRepositoryMock;
        protected static Mock<IPersistDomainModels<TaskConfiguration>> _domainModelPersisterMock;
        protected static Mock<IProgressbar> _loadingNotifierMock;
        protected static Mock<ITimer> _refreshNotifierMock;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new TasksViewModel();

            _taskDefinitionRepositoryMock = new Mock<IRepository<TaskDefinition>>();
            _taskConfigurationRepositoryMock = new Mock<IRepository<TaskConfiguration>>();
            _domainModelPersisterMock = new Mock<IPersistDomainModels<TaskConfiguration>>();
            _loadingNotifierMock = new Mock<IProgressbar>();
            _refreshNotifierMock = new Mock<ITimer>();

            _controller = new TasksController(_viewModel,
                                              _taskDefinitionRepositoryMock.Object,
                                              _taskConfigurationRepositoryMock.Object,
                                              _domainModelPersisterMock.Object,
                                              new NoUIInvokation(),
                                              _loadingNotifierMock.Object,
                                              _refreshNotifierMock.Object,
                                              new NoBackgroundWorkerInvocation<IEnumerable<TaskConfiguration>>());
        }

        protected Context the_tasks_repository_contains_a_task = () =>
        {
            _taskDefinitionRepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<TaskDefinition>>()))
                                         .Returns(1.FakeTaskDefinitions());
        };

        protected Context the_task_instances_repository_contains_a_task_instance = () =>
        {
            _taskConfigurationRepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<TaskConfiguration>>()))
                                            .Returns(1.FakeTaskConfigurations());
        };

        protected Context the_view_model_contains_a_task_instance = () =>
        {
            _viewModel.RunningTasks.Add(
                new TaskInstanceConfigurationViewModel
                {
                    AvailableTaskName = "Fake task",
                    RunningTaskName = "Fake task instance",
                    ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>()
                });
        };

        protected Context the_controller_was_started = () => _controller.Start();
    }

    public static class Extensions
    {
        public static IEnumerable<TaskDefinition> FakeTaskDefinitions(this int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return new TaskDefinition
                {
                    Author = "Author " + i,
                    Description = "Task description " + i,
                    Name = "Fake task " + i,
                    SettingDefinitions = new List<TaskSettingDefinition>(),
                    Version = i,
                    Webpage = "http://www.smeedee.org/"
                };
            }
        }

        public static IEnumerable<TaskConfiguration> FakeTaskConfigurations(this int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return new TaskConfiguration
                {
                    Name = "Fake task config " + i,
                    DispatchInterval = i,
                    TaskName = "Fake task " + i,
                    Entries = new List<TaskConfigurationEntry>()
                };
            }
        }
    }
}