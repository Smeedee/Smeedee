using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.IoC;

namespace Smeedee.Widget.Admin.Tasks.Tests.ViewModels
{
    [TestFixture]
    public class When_an_available_task_is_selected : TasksViewModelTest
    {
        [Test]
        public void Assure_the_selected_task_can_be_activated()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);

            When("we select a task",
                 () => _viewModel.SelectedAvailableTask = _AvailableTask);

            Then("we should be able to activate the selected task",
                 () => _viewModel.CanActivateSelectedTask().ShouldBe(true));
        }

        [Test]
        public void Assure_that_no_running_task_is_selected()
        {
            Given(a_new_ViewModel_is_instantiated)
                .And(the_ViewModel_has_one_available_task)
                .And(the_ViewModel_has_one_running_task)
                .And("the running task is selected", 
                () =>_viewModel.SelectedRunningTask = _RunningTask);

            When("we select an available task",
                () => _viewModel.SelectedAvailableTask = _AvailableTask);

            Then("the selected running task should loose focus",
                 () => _viewModel.RunningTaskIsSelected.ShouldBeFalse());
        }
    }

    [TestFixture]
    public class When_an_available_task_is_not_selected : TasksViewModelTest
    {
        [Test]
        public void Assure_no_available_tasks_can_be_activated()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);

            When("no available tasks are selected",
                 () => _viewModel.SelectedAvailableTask = null);

            Then("we should not be able to activate the selected available task",
                 () => _viewModel.CanActivateSelectedTask().ShouldBe(false));
        }
    }

    [TestFixture]
    public class When_a_running_task_is_selected : TasksViewModelTest
    {
        [Test]
        public void Assure_the_selected_running_task_can_be_deactivated()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And(the_ViewModel_has_one_running_task);

            When("we select a running task",
                 () => _viewModel.SelectedRunningTask = _RunningTask);

            Then("we should be able to deactivate the selected running task",
                 () => _viewModel.CanDeactivateSelectedTask().ShouldBe(true));
        }

        [Test]
        public void Assure_that_no_available_task_is_selected()
        {
            Given(a_new_ViewModel_is_instantiated)
                .And(the_ViewModel_has_one_available_task)
                .And(the_ViewModel_has_one_running_task)
                .And("we select an available task",
                () => _viewModel.SelectedAvailableTask = _AvailableTask);

            When("we select a running task",
                () => _viewModel.SelectedRunningTask = _RunningTask);

            Then("the selected available task should loose focus",
                 () => _viewModel.AvailableTaskIsSelected.ShouldBeFalse());
        }
    }

    [TestFixture]
    public class When_a_running_task_is_not_selected : TasksViewModelTest
    {
        [Test]
        public void Assure_no_running_tasks_can_be_deactivated()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And(the_ViewModel_has_one_running_task);

            When("we do not select a running task",
                 () => _viewModel.SelectedRunningTask = null);

            Then("we should not be able to deactivate the selected running task",
                 () => _viewModel.CanDeactivateSelectedTask().ShouldBe(false));
        }
    }

    [TestFixture]
    public class When_an_available_task_is_activated : TasksViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And(an_available_task_is_selected);

            When("we activate the selected task",
                 () => _viewModel.ActivateSelectedTask.Execute(null));
        }

        [Test]
        public void Assure_it_is_added_as_a_new_running_task()
        {
            Then("it should be added as a running task",
                 () => _viewModel.RunningTasks.Count.ShouldBe(1));
        }

        [Test]
        public void Assure_the_new_running_task_has_the_correct_task_name()
        {
            Then("the running task name should be the same as the available task name",
                 () => _viewModel.RunningTasks.First().AvailableTaskName.ShouldBe(_AvailableTask.Name));
        }

        [Test]
        public void Assure_the_new_running_task_has_the_correct_instance_name()
        {
            Then("the instance name should contain the task name",
                 () => _viewModel.RunningTasks.First().RunningTaskName.ShouldBe("New " + _AvailableTask.Name));
        }

        [Test]
        public void Assure_the_available_task_is_deselected()
        {
            Then("the available task should be deselected",
                 () => _viewModel.SelectedAvailableTask.ShouldBe(null));
        }

        [Test]
        public void Assure_the_running_task_is_selected()
        {
            Then("the running task should be selected",
                 () => _viewModel.SelectedRunningTask.ShouldBe(_viewModel.RunningTasks.Last()));
        }
    }

    [TestFixture]
    public class When_an_available_task_has_been_added : TasksViewModelTest
    {
        [Test]
        public void assure_the_added_task_in_running_tasks_is_deselected_if_a_new_available_task_is_selected()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And("the task is selected", () => _viewModel.SelectedAvailableTask = _viewModel.AvailableTasks.First());
            And("the task is added", () => _viewModel.ActivateSelectedTask.Execute(null));
           
            When("a new available task is selected", () =>
                 _viewModel.SelectedAvailableTask = _AvailableTask);

            Then("the running task that just was added should be deselected", () =>
                _viewModel.SelectedRunningTask.ShouldBeNull());
        }
    }

    [TestFixture]
    public class When_a_running_task_is_deactivated : TasksViewModelTest
    {

        [Test]
        public void assure_nothing_is_removed_if_cancel_is_pushed()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And(the_ViewModel_has_one_running_task);
            And(a_running_task_is_selected);
            And(the_viewModel_has_a_fake_messagebox_service_which_will_return_cancel);

            When("we deactivate the selected running task",
                 () => _viewModel.DeactivateSelectedTask.Execute(null));

            Then("no tasks are deleted from Running Tasks",
                 () => _viewModel.RunningTasks.Count().ShouldBe(1));
        }

        [Test]
        public void assure_the_selected_running_task_is_removed_if_ok_is_pushed()
        {
            Given(a_new_ViewModel_is_instantiated);
            And(the_ViewModel_has_one_available_task);
            And(the_ViewModel_has_one_running_task);
            And(a_running_task_is_selected);
            And(the_viewModel_has_a_fake_messageboxservice_which_will_return_ok);

            When("we deactivate the selected task instance",
                 () => _viewModel.DeactivateSelectedTask.Execute(null));

            Then("the task is deleted from Running Tasks",
                 () => _viewModel.RunningTasks.Count().ShouldBe(0));
        }

        [Test]
        public void assure_the_task_below_it_is_selected_if_present()
        {
            string taskInstanceNameOfFirstEntryInList = "";
            string taskInstanceNameOfSecondEntryInList = "";

            Given(a_new_ViewModel_is_instantiated)
                .And(the_ViewModel_has_2_task_instances)
                .And(the_viewModel_has_a_fake_messageboxservice_which_will_return_ok)
                .And("the first task is selected", () =>
                {
                    taskInstanceNameOfFirstEntryInList = _viewModel.RunningTasks.First().RunningTaskName;
                    taskInstanceNameOfSecondEntryInList = _viewModel.RunningTasks.Last().RunningTaskName;
                    _viewModel.SelectedRunningTask = _viewModel.RunningTasks.First();
                });

            When("this task is removed", 
                () => _viewModel.DeactivateSelectedTask.Execute(null));

            Then("the task instance, which was below should have been selected", 
                () => _viewModel.SelectedRunningTask.RunningTaskName.ShouldBe(taskInstanceNameOfSecondEntryInList));
        }

        [Test]
        public void assure_the_task_above_it_is_selected_if_no_task_below_it_is_present()
        {
            string taskInstanceNameOfFirstEntryInList = "";
            string taskInstanceNameOfSecondEntryInList = "";

            Given(a_new_ViewModel_is_instantiated)
                .And(the_ViewModel_has_2_task_instances)
                .And(the_viewModel_has_a_fake_messageboxservice_which_will_return_ok)
                .And("the last task is selected", () =>
                {
                    taskInstanceNameOfFirstEntryInList = _viewModel.RunningTasks.First().RunningTaskName;
                    taskInstanceNameOfSecondEntryInList = _viewModel.RunningTasks.Last().RunningTaskName;
                    _viewModel.SelectedRunningTask = _viewModel.RunningTasks.Last();
                });

            When("this task is removed",
                () => _viewModel.DeactivateSelectedTask.Execute(null));

            Then("the task instance, which was above should have been selected",
                () => _viewModel.SelectedRunningTask.RunningTaskName.ShouldBe(taskInstanceNameOfFirstEntryInList));
        }

        [Test]
        public void assure_no_task_is_selected_when_the_last_task_instance_is_removed()
        {
            Given(a_new_ViewModel_is_instantiated)
                .And(the_ViewModel_has_one_running_task)
                .And(the_viewModel_has_a_fake_messageboxservice_which_will_return_ok)
                .And(a_running_task_is_selected);

            When("this task is removed",
                () => _viewModel.DeactivateSelectedTask.Execute(null));

            Then("no task instance should be selected",
                () => _viewModel.SelectedRunningTask.ShouldBeNull());
        }
    }

    [TestFixture]
    public class When_a_task_is_added : TasksViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            Given(a_new_ViewModel_is_instantiated).
            And(the_ViewModel_has_one_available_task);
        }
        [Test]
        public void Assure_no_duplicates_can_exist_when_same_task_is_added_once()
        {
            When(the_viewModels_1_task_is_added_once_more);

            Then("assure that the task only exists once in the task list", () =>
                _viewModel.AvailableTasks.Count.ShouldBe(1));
        }

        [Test]
        public void assure_no_duplicates_can_exist_when_same_task_is_added_several_times()
        {
            When(the_viewModels_1_task_is_added_3_times_more);

            Then("assure that the task only exists once in the task list", () =>
                _viewModel.AvailableTasks.Count.ShouldBe(1));
        }
    }

    [TestFixture]
    public class When_several_tasks_are_added : TasksViewModelTest
    {
        [Test]
        public void assure_no_duplicates_can_exist()
        {
            Given(a_new_ViewModel_is_instantiated)
                .And(the_viewModel_has_3_unique_tasks);

            When(the_same_tasks_are_added_once_more);

            Then("assure that the task only exists once in the task list", () =>
                _viewModel.AvailableTasks.Count.ShouldBe(3)); 
        }
    }

    [TestFixture]
    public class When_a_task_instance_is_added_from_UI : TasksViewModelTest
    {

        [SetUp]
        public void SetUp()
        {
            Given(a_new_ViewModel_is_instantiated).
            And(the_ViewModel_has_one_available_task).
            And(the_ViewModel_has_one_running_task);
        }
        [Test]
        public void assure_a_task_instance_get_a_unique_name_automatically()
        {
            When(a_new_task_instance_with_the_same_name_is_trying_to_get_added_from_the_UI);

            Then("assure that the new task gets a prefix to its name that makes the name unique", () =>
            {
                _viewModel.RunningTasks.Count.ShouldBe(2);
                _viewModel.RunningTasks[0].RunningTaskName.ShouldBe("New "+ _AvailableTask.Name);
                _viewModel.RunningTasks[1].RunningTaskName.ShouldBe("New " + _AvailableTask.Name +"(1)");
            });
        }

        [Test]
        public void assure_several_task_instances_of_same_type_get_a_unique_name_automatically()
        {
            When(several_task_instances_of_the_same_type_are_added_from_the_UI);

            Then(assure_each_task_instance_gets_a_name_with_a_unique_postfix);
        }
    }

    [TestFixture]
    public class When_a_task_instance_name_is_edited : TasksViewModelTest
    {
        [Test]
        public void assure_that_it_cant_be_edited_to_the_same_name_as_another_task()
        {
            Given(a_new_ViewModel_is_instantiated).
                And(two_2_task_instances_are_added_to_the_viewModel).
                And(a_running_task_is_selected);

            When(a_task_instance_is_edited_to_an_already_existing_name);

            Then("The name should get a unique postfix", () =>
            {
                _viewModel.RunningTasks[0].RunningTaskName.ShouldBe(_taskInstance2.RunningTaskName + "(1)");
                _viewModel.RunningTasks[1].RunningTaskName.ShouldBe(_taskInstance2.RunningTaskName);
            });
        }

        [Test]
        public void assure_that_each_postfix_is_1_more_than_the_last()
        {
            Given(a_new_ViewModel_is_instantiated).
                And(three_task_instances_with_postfixes_are_added_to_the_viewModel).
                And(a_running_task_is_selected);

            When("a task name after edit equals an already existing task's name", () =>
            {
                _viewModel.SelectedRunningTask = _taskInstance3;
                _viewModel.SelectedRunningTask.RunningTaskName = "New Test task";
            });

            Then("The names should get a unique postfix", () =>
            {
                _viewModel.RunningTasks[0].RunningTaskName.ShouldBe("New Test task"); 
                _viewModel.RunningTasks[1].RunningTaskName.ShouldBe("New Test task(1)");
                _viewModel.RunningTasks[2].RunningTaskName.ShouldBe("New Test task(2)");
            });
        }
    }

    [TestFixture]
    public class when_no_data_has_been_changed_in_running_tasks : TasksViewModelTest
    {
        [Test]
        public void assure_that_the_save_button_is_disabled()
        {
            Given(a_new_ViewModel_is_instantiated);

            When("");

            Then("the save button should be disabled", () => _viewModel.CanSaveChanges().ShouldBe(false));
        }
    }

    [TestFixture]
    public class when_data_has_been_changed_in_running_tasks : TasksViewModelTest
    {
        
        [Test]
        public void assure_that_the_save_button_is_enabled_when_the_name_has_been_edited()
        {
            Given(a_new_ViewModel_is_instantiated).And(the_ViewModel_has_one_running_task);

            When(a_task_instance_is_edited_to_an_already_existing_name);

            Then("the save button should be enabled", () => _viewModel.CanSaveChanges().ShouldBe(true));
        }

        [Test]
        public void assure_that_the_save_button_is_enabled_when_the_password_has_been_edited()
        {
            Given(a_new_ViewModel_is_instantiated).And(the_ViewModel_has_one_running_task);

            When("Something is changed", () => _RunningTask.ConfigurationEntries.First().Name = "tull");

            Then("the save button should be enabled", () => _viewModel.CanSaveChanges().ShouldBe(true));
        }

        [Test]
        public void assure_the_save_button_is_disabled_when_data_has_been_saved()
        {
            Given(a_new_ViewModel_is_instantiated).
                And(the_ViewModel_has_one_running_task).
                And("Something is changed", () => _RunningTask.ConfigurationEntries.First().Name = "tull");

            When("Save is clicked", () => _viewModel.SaveChanges.Execute(null));

            Then("the save button should be disabled", () => _viewModel.CanSaveChanges().ShouldBeFalse());
        }
    }

    public class TasksViewModelTest : SmeedeeScenarioTestClass
    {
        protected Context a_new_ViewModel_is_instantiated = () =>
        {
            _viewModel = new TasksViewModel
                             {
                                 AvailableTasks = new ObservableCollection<TaskViewModel>(),
                                 RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>()
                             };
        };

        protected Context the_ViewModel_has_one_available_task = () =>
        {
            _AvailableTask = new TaskViewModel { Name = "Test task" };
            _viewModel.AvailableTasks = new ObservableCollection<TaskViewModel> {_AvailableTask};
        };

        protected Context the_viewModel_has_3_unique_tasks = () =>
        {
            _task1 = new TaskViewModel { Name = "Test task1", Author = "1", Version = 1, Description = "Jeg er 1"};
            _task2 = new TaskViewModel { Name = "Test task2", Author = "2", Version = 2, Description = "Jeg er 2" };
            _task3 = new TaskViewModel { Name = "Test task3", Author = "3", Version = 3, Description = "Jeg er 3" };

            _viewModel.AvailableTasks.Add(_task1);
            _viewModel.AvailableTasks.Add(_task2);
            _viewModel.AvailableTasks.Add(_task3);
        };

        protected When the_viewModels_1_task_is_added_once_more = () =>
        {
            _AvailableTask = new TaskViewModel { Name = "Test task" };
            _viewModel.AddTask(_AvailableTask);
        };


        protected When the_viewModels_1_task_is_added_3_times_more = () =>
        {
            _AvailableTask = new TaskViewModel { Name = "Test task" };
            _viewModel.AddTask(_AvailableTask);
            _viewModel.AddTask(_AvailableTask);
            _viewModel.AddTask(_AvailableTask);
        };

        protected When the_same_tasks_are_added_once_more = () =>
        {
            _task1 = new TaskViewModel { Name = "Test task1", Author = "1", Version = 1, Description = "Jeg er 1" };
            _task2 = new TaskViewModel { Name = "Test task2", Author = "2", Version = 2, Description = "Jeg er 2" };
            _task3 = new TaskViewModel { Name = "Test task3", Author = "3", Version = 3, Description = "Jeg er 3" };

            _viewModel.AddTask(_task1);
            _viewModel.AddTask(_task2);
            _viewModel.AddTask(_task3);
        };

        protected Context the_ViewModel_has_one_running_task = () =>
        {
            _RunningTask = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task",
                RunningTaskName = "New Test task",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }

            };
            _viewModel.RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            _viewModel.AddTaskInstance(_RunningTask);
        };

        protected When a_new_task_instance_with_the_same_name_is_trying_to_get_added = () =>
        {
             _taskInstance2 = new TaskInstanceConfigurationViewModel
                                        {
                                            AvailableTaskName = "Test task", 
                                            RunningTaskName = "New Test task",
                                            ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
                                        };

            _viewModel.AddTaskInstance(_taskInstance2);
        };

        protected When a_new_task_instance_with_the_same_name_is_trying_to_get_added_from_the_UI = () =>
        {
            _viewModel.SelectedAvailableTask = _AvailableTask;
            _viewModel.ActivateSelectedTask.Execute(null);
        };

        protected When several_task_instances_of_the_same_type_are_added_from_the_UI = () =>
        {

            _task1 = new TaskViewModel { Name = "Test task", Author = "1", Version = 1, Description = "Jeg er 1" };
           
            _viewModel.SelectedAvailableTask = _task1;
            _viewModel.ActivateSelectedTask.Execute(null);
            _viewModel.SelectedAvailableTask = _task1;
            _viewModel.ActivateSelectedTask.Execute(null);
            _viewModel.SelectedAvailableTask = _task1;
            _viewModel.ActivateSelectedTask.Execute(null);
        };

        protected When a_new_task_instance_with_the_same_name_is_trying_to_get_added_from_the_user_interface = () =>
        {

            _AvailableTask = new TaskViewModel { Name = "Test task" };

            _viewModel.SelectedAvailableTask = _AvailableTask;

            _viewModel.ActivateSelectedTask.Execute(null);
        };

        protected When a_task_instance_is_edited_to_an_already_existing_name = () =>
        {
            _viewModel.SelectedRunningTask = _RunningTask;
            _viewModel.SelectedRunningTask.RunningTaskName = "New Test task 2";
        };

        protected When a_task_instance_is_edited_to_an_already_existing_name_with_postfix = () =>
        {
            _taskInstance3.RunningTaskName = "New Test task";
        };

        protected Context the_ViewModel_has_2_task_instances = () =>
        {
            _RunningTask = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task",
                RunningTaskName = "New Test task",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _taskInstance2 = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task 2",
                RunningTaskName = "New Test task 2",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _viewModel.RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            _viewModel.RunningTasks.Add(_RunningTask);
            _viewModel.RunningTasks.Add(_taskInstance2);
        };

        protected Context two_2_task_instances_are_added_to_the_viewModel = () =>
        {
            _RunningTask = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task",
                RunningTaskName = "New Test task",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _taskInstance2 = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task 2",
                RunningTaskName = "New Test task 2",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _viewModel.RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            _viewModel.AddTaskInstance(_RunningTask);
            _viewModel.AddTaskInstance(_taskInstance2);
        };

        protected Context three_task_instances_with_postfixes_are_added_to_the_viewModel = () =>
        {
            _RunningTask = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task",
                RunningTaskName = "New Test task",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _taskInstance2 = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task 2",
                RunningTaskName = "New Test task(1)",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _taskInstance3 = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task 3",
                RunningTaskName = "New Test task(2)",
                ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                           {
                                               new ConfigurationEntryViewModel {}
                                           }
            };

            _viewModel.RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            _viewModel.AddTaskInstance(_RunningTask);
            _viewModel.AddTaskInstance(_taskInstance2);
            _viewModel.AddTaskInstance(_taskInstance3);
        };

        protected Context an_available_task_is_selected = () =>
        {
            _viewModel.SelectedAvailableTask = _AvailableTask;
        };

        protected Context a_running_task_is_selected = () =>
        {
            _viewModel.SelectedRunningTask = _RunningTask;
        };

        protected Context a_task_is_not_selected = () =>
        {
            _viewModel.SelectedAvailableTask = null;
        };

        protected Then assure_each_task_instance_gets_a_name_with_a_unique_postfix = () =>
        {
            _viewModel.RunningTasks[1].RunningTaskName.ShouldBe("New " + _task1.Name + "(1)");
            _viewModel.RunningTasks[2].RunningTaskName.ShouldBe("New " + _task1.Name + "(2)");
            _viewModel.RunningTasks[3].RunningTaskName.ShouldBe("New " + _task1.Name + "(3)");
        };

        protected Context the_viewModel_has_a_fake_messagebox_service_which_will_return_cancel = () =>
        {
            _fakeMessageBoxService = new FakeMessageBoxService();
            _fakeMessageBoxService.MessageBoxResultToReturn = MessageBoxResult.Cancel;
            _viewModel.ConfigureDependencies(config => config.Bind<IMessageBoxService>().ToInstance(_fakeMessageBoxService));
        };

        protected Context the_viewModel_has_a_fake_messageboxservice_which_will_return_ok = () =>
        {
            _fakeMessageBoxService = new FakeMessageBoxService();
            _fakeMessageBoxService.MessageBoxResultToReturn = MessageBoxResult.OK;
            _viewModel.ConfigureDependencies(config => config.Bind<IMessageBoxService>().ToInstance(_fakeMessageBoxService));
        };
           

        protected static TasksViewModel _viewModel;
        protected static TaskViewModel _AvailableTask;
        protected static TaskInstanceConfigurationViewModel _RunningTask;
        protected static TaskInstanceConfigurationViewModel _taskInstance2;
        protected static TaskViewModel _task1;
        protected static TaskViewModel _task2;
        protected static TaskViewModel _task3;
        protected static TaskInstanceConfigurationViewModel _taskInstance3;
        protected static TaskInstanceConfigurationViewModel _taskInstance4;
        protected static FakeMessageBoxService _fakeMessageBoxService;
    }

    public class FakeMessageBoxService : IMessageBoxService
    {
        public MessageBoxResult MessageBoxResultToReturn { get; set; }

        public MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            return MessageBoxResultToReturn;
        }
    }
}