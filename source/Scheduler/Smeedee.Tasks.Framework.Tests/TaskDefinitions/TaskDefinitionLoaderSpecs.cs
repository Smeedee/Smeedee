using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Tasks.Framework.TaskAttributes;
using Smeedee.Tasks.Framework.TaskDefinitions;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Framework.Tests.TaskDefinitions
{
    [TestFixture]
    public class TaskDefinitionLoaderSpecs : SmeedeeScenarioTestClass
    {
        private static string folderWithManyDlls = @"C:\smeedee-merged\source\Smeedee.Task.TestProjectWithAttributes\bin\Debug\";
        private static string fileName;

        private static TaskDefinitionLoader taskDefinitionLoader;

        [SetUp]
        public void SetUp()
        {
            var loggerMock = new Mock<ILog>();
            taskDefinitionLoader = new TaskDefinitionLoader(loggerMock.Object);
        }

        [Test]
        [Ignore("Test uses a local path.")]
        public void assure_loads_from_folder_without_failing()
        {
            Given("there is a folder with tons of dlls in it, one of which has two settings");
            When("we load task definitions from the folder");
            Then(two_settings_are_loaded_from_folder);
        }
        
        [Test]
        public void assure_loads_a_taskDefinition_from_dll()
        {
            Given(there_is_a_dll_with_a_specific_task_attribute_in_it);
            When("we load task definitions from the folder");
            Then(a_task_definition_is_loaded_from_file);
        }

        [Test]
        public void assure_loads_a_taskDefinition_and_settings_from_dll()
        {
            Given(there_is_a_dll_with_a_specific_task_attribute_in_it);
            When("we load task definitions from the folder");
            Then(a_task_definition_is_loaded_from_file).
                And(two_settings_are_loaded_from_file);
        }

        private Context there_is_a_dll_with_a_specific_task_attribute_in_it = () =>
        {
            fileName = Assembly.GetAssembly(typeof(TaskDefinitionLoaderSpecs)).Location;
        };
        
        private Then a_task_definition_is_loaded_from_file = () =>
        {
            taskDefinitionLoader.LoadFromDLL(fileName).Count().ShouldBe(1);
        };

        private Then two_settings_are_loaded_from_file = () =>
        {
            taskDefinitionLoader.LoadFromDLL(fileName).First().SettingDefinitions.Count().ShouldBe(2);
        };
        
        private Then two_settings_are_loaded_from_folder = () =>
        {
            taskDefinitionLoader.LoadFromFolder(folderWithManyDlls).First().SettingDefinitions.Count().ShouldBe(2);
        };
    }

    [Task("myName",
        Author = "myAuthor",
        Description = "myDesc",
        Name = "myOtherName?",
        Version = 42,
        Webpage = "http://myWebsite.ru")]
    [TaskSetting("some_setting", typeof(int), "4")]
    [TaskSetting("some_other_setting", typeof(object), "ifdsa")]
    public class TestClassWithAttributes : TaskBase
    {
        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}