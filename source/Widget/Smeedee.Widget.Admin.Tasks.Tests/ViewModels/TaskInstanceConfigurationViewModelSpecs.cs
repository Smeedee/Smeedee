using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Tasks.Tests.ViewModels
{

    [TestFixture]
    public class When_a_new_task_instance_configuration_is_created :Shared_test_class
    {
        
        [Test]
        public void assure_that_an_event_is_triggered_when_the_name_of_a_task_config_is_changed()
        {
            Given(someone_subscribes_to_an_task_config);
            When("the task config name is edited", () => taskInstance.RunningTaskName = "Nytt navn");
            Then("it should raise the edit event exactly once", () => raised.ShouldBe(1));
        }

        [Test]
        public void assure_that_an_event_is_not_triggered_when_the_change_is_done_without_the_user()
        {
            Given(someone_subscribes_to_an_task_config);
            When("the task config name is edited", () => taskInstance.SetInstanceNameWithoutFiringProperty("nytt navn"));
            Then("it should not raise any event", () => raised.ShouldBe(0));
        }

        [Test]
        public void assure_the_username_setting_is_placed_at_the_top_if_present()
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing a username", () => 
                CreateTaskInstance(configWithManyPrioritizedSettingsWithOrderIndex));
            Then("the first settings entry should be 'Username'", () =>
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Username"));
        }

        [Test]
        public void assure_settings_which_are_not_prioritized_are_placed_at_the_bottom()
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing many settings is created", () =>
                CreateTaskInstance(configWithPrioritizedAndNonPrioritizedSettingWithOrderIndex));
            Then("the first settings entry should be 'Username'", () =>
            {
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Username");
                taskInstance.ConfigurationEntries[1].Name.ShouldBe("Password");
                taskInstance.ConfigurationEntries[2].Name.ShouldBe("Excotic Setting"); 
            });
        }

        [Test]
        public void assure_the_task_settings_are_placed_in_the_correct_order()
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing many settings is created", () =>
                CreateTaskInstance(configWithManyPrioritizedSettingsWithOrderIndex));
            Then("the first settings entry should be 'Username'", () =>
            {
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Username");
                taskInstance.ConfigurationEntries[1].Name.ShouldBe("Password");
                taskInstance.ConfigurationEntries[2].Name.ShouldBe("Url");
            });
        }

        [Test]
        public void assure_that_settings_in_a_script_task_are_placed_in_the_correct_order() 
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing many settings is created", () =>
                CreateTaskInstance(configWithSettingsForAScriptTaskWithOrderIndex));
            Then("the first settings entry should be 'Script name'", () =>
            {
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Script Name");
                taskInstance.ConfigurationEntries[1].Name.ShouldBe("Args");
            });
        }

        [Test]
        public void assure_that_settings_without_orderIndex_property_are_placed_as_the_entered_in_the_confgiEntryViewModel()
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing many settings without orderIndex property is created", () =>
                CreateTaskInstance(configWithManyPrioritizedSettingsWithoutOrderIndex));

            Then("the first settings entry should be 'Username'", () =>
            {
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Password");
                taskInstance.ConfigurationEntries[1].Name.ShouldBe("Url");
                taskInstance.ConfigurationEntries[2].Name.ShouldBe("Username"); 
            });
        }

        [Test]  
        public void assure_that_settings_with_default_value_for_orderIndex_is_set_below_settings_with_orderIndex_set()
        {
            Given(a_list_of_available_tasks);
            When("a task configuration containing many settings without orderIndex property is created", () =>
                CreateTaskInstance(configWithOnePrioritizedSettingAndManyNonPrioritzedSettings));
            Then("assure the setting with priority comes before the ones without", () =>
            {
                taskInstance.ConfigurationEntries[0].Name.ShouldBe("Username");
            });
        }
    }

    [TestFixture]
    public class DispatchIntervalPropertiesTests
    {
        private TaskInstanceConfigurationViewModel vm;
        
        [Test]
        public void DispatchIntervalComponents_equal_zero_at_zero_DispatchInterval()
        {
            vm.DispatchInterval = 0;

            Assert.AreEqual(0, vm.DispatchIntervalHours);
            Assert.AreEqual(0, vm.DispatchIntervalMinutes);
            Assert.AreEqual(0, vm.DispatchIntervalSeconds);
        }

        [Test]
        public void DispatchIntervalHours_equals_1_at_1h_DispatchInterval()
        {
            var hour = new TimeSpan(0, 1, 0, 0);
            vm.DispatchInterval = Convert.ToInt32(hour.TotalMilliseconds);

            Assert.AreEqual(1, vm.DispatchIntervalHours);
            Assert.AreEqual(0, vm.DispatchIntervalMinutes);
            Assert.AreEqual(0, vm.DispatchIntervalSeconds);
        }

        [Test]
        public void DispatchIntervalComponents_are_equal_to_DispatchInterval()
        {
            var time = new TimeSpan(0, 1, 1, 1);
            vm.DispatchInterval = Convert.ToInt32(time.TotalMilliseconds);

            Assert.AreEqual(1, vm.DispatchIntervalHours);
            Assert.AreEqual(1, vm.DispatchIntervalMinutes);
            Assert.AreEqual(1, vm.DispatchIntervalSeconds);
        }

        [Test]
        public void DispatchInterval_equals_DispatchIntervalComponents()
        {
            vm.DispatchIntervalHours = 1;
            vm.DispatchIntervalMinutes = 1;
            vm.DispatchIntervalSeconds = 1;
            var time = new TimeSpan(0, 1, 1, 1);

            Assert.AreEqual(time.TotalMilliseconds, vm.DispatchInterval);
        }

        [SetUp]
        public void InstantiateVM()
        {
            vm = new TaskInstanceConfigurationViewModel();
        }
    }


    [TestFixture]
    public class Shared_test_class : SmeedeeScenarioTestClass
    {
        protected static ObservableCollection<ConfigurationEntryViewModel> configWithManyPrioritizedSettingsWithOrderIndex;
        protected static ObservableCollection<ConfigurationEntryViewModel> configWithPrioritizedAndNonPrioritizedSettingWithOrderIndex;
        protected static TaskInstanceConfigurationViewModel taskInstance;
        protected static ObservableCollection<ConfigurationEntryViewModel> configWithSettingsForAScriptTaskWithOrderIndex;
        protected static ObservableCollection<ConfigurationEntryViewModel> configWithManyPrioritizedSettingsWithoutOrderIndex;
        protected static ObservableCollection<ConfigurationEntryViewModel> configWithOnePrioritizedSettingAndManyNonPrioritzedSettings;

        protected static int raised;

        static Shared_test_class()
        {
            configWithManyPrioritizedSettingsWithOrderIndex = new ObservableCollection<ConfigurationEntryViewModel>
            {
                new ConfigurationEntryViewModel {OrderIndex = 2, Name = "Password", Value = 1234},
                new ConfigurationEntryViewModel {OrderIndex = 3,Name = "Url", Value = new Uri("http://test.com") },                
                new ConfigurationEntryViewModel {OrderIndex = 1,Name = "Username", Value = "something"},
            };
            configWithPrioritizedAndNonPrioritizedSettingWithOrderIndex = new ObservableCollection<ConfigurationEntryViewModel>
            {
                new ConfigurationEntryViewModel {OrderIndex = 2, Name = "Password", Value = 1234},
                new ConfigurationEntryViewModel {OrderIndex = 3, Name = "Excotic Setting", Value = null },                
                new ConfigurationEntryViewModel {OrderIndex = 1, Name = "Username", Value = "something"},
            };

            configWithSettingsForAScriptTaskWithOrderIndex = new ObservableCollection<ConfigurationEntryViewModel>()
            {
                new ConfigurationEntryViewModel() {OrderIndex = 2,Name = "Args", Value = "something"},
                new ConfigurationEntryViewModel() {OrderIndex = 1,Name = "Script Name", Value = "something aswell"}
            };

            configWithManyPrioritizedSettingsWithoutOrderIndex = new ObservableCollection<ConfigurationEntryViewModel>
            {
                new ConfigurationEntryViewModel {Name = "Password", Value = 1234},
                new ConfigurationEntryViewModel {Name = "Url", Value = new Uri("http://test.com") },                
                new ConfigurationEntryViewModel {Name = "Username", Value = "something"},
            };

            configWithOnePrioritizedSettingAndManyNonPrioritzedSettings = new ObservableCollection<ConfigurationEntryViewModel>()
            {
                new ConfigurationEntryViewModel {OrderIndex = int.MaxValue, Name = "Password", Value = 1234},
                new ConfigurationEntryViewModel {OrderIndex = int.MaxValue, Name = "Url", Value = new Uri("http://test.com") },                
                new ConfigurationEntryViewModel {OrderIndex = 1, Name = "Username", Value = "something"},
            };
}

        [SetUp]
        protected void SetUp()
        {
            taskInstance = new TaskInstanceConfigurationViewModel
                                {
                                    AvailableTaskName = "Test task",
                                    RunningTaskName = "New Test task",
                                    ConfigurationEntries = configWithManyPrioritizedSettingsWithOrderIndex
                                };
        }

        protected Context a_list_of_available_tasks = () =>
        {

        };

        protected Context someone_subscribes_to_an_task_config = () =>
        {
            raised = 0;
            taskInstance.PropertyChanged += EditSubscriber;
        };



        protected static void EditSubscriber(object sender, EventArgs args)
        {
            raised++;
        }

        protected void CreateTaskInstance(ObservableCollection<ConfigurationEntryViewModel> configurationEntryViewModels)
        {
            taskInstance = new TaskInstanceConfigurationViewModel
            {
                AvailableTaskName = "Test task",
                RunningTaskName = "New Test task",
                ConfigurationEntries = configurationEntryViewModels
            };
        }

        
    }
}
