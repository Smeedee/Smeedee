using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Script;
using Smeedee.Tasks.Script.Services;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.Script
{
    public class ScriptTaskSpecs
    {
        [TestFixture]
        public class When_spawning : Shared
        {
            public override void Before()
            {
                Given(TaskConfiguration_is_created).And(ScriptName_is_set);

                When("spawning");                
            }

            [Test]
            public void assure_SafeDirectoryPathForScripts_arg_is_validated()
            {
                Then(() =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new ScriptTask(null, taskConfiguration, processServiceFake.Object)));
            }

            [Test]
            public void assure_TaskConfiguration_args_is_validated()
            {
                Then(() =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new ScriptTask(safeDirectoryPathForScripts, null, processServiceFake.Object)));
            }

            [Test]
            public void assure_ProcessService_args_is_validated()
            {
                Then(() =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new ScriptTask(safeDirectoryPathForScripts, taskConfiguration, null)));
            }
        }

        [TestFixture]
        public class When_spawning_and_ScriptName_is_not_set : Shared
        {
            public override void Before()
            {
                Given(SafeDirectoryPathForScripts_exists);
                And(TaskConfiguration_is_created);
                And("Script Name is not set", () =>
                {
                    taskConfiguration.Entries.Add(new TaskConfigurationEntry()
                    {
                        Name = "Script Name"
                    });
                });
                And(ProcessServiceFake_is_created);

                When("spawning");
            }

            [Test]
            public void assure_TaskConfigurationException_is_thrown()
            {
                //Should not be possible to spawn the task in an invalid state

                Then(() =>
                    this.ShouldThrowException<TaskConfigurationException>(() =>
                        NewScriptTask()));
            }
        }

        [TestFixture]
        public class When_spawning_and_SafeDirectoryPathForScripts_does_not_exists : Shared
        {
            public override void Before()
            {
                Given(SafeDirectoryPathForScripts_exists);
                And(TaskConfiguration_is_created);
                And("SafeDirectoryPathForScripts does not exists", () =>
                    safeDirectoryPathForScripts = @"c:\temp\aFolderThatDoesNotExist");
                And(ProcessServiceFake_is_created);

                When("spawning");
            }

            [Test]
            public void assure_ArgumentException_is_thrown()
            {
                Then(() =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        NewScriptTask(), ex =>
                            ex.Message.ShouldBe("Path specified for SafeDirectoryPathForScripts does not exist")));
            }
        }

        [TestFixture]
        public class When_spawning_and_Script_does_not_exists : Shared
        {
            public override void Before()
            {
                Given(SafeDirectoryPathForScripts_exists);
                And(TaskConfiguration_is_created);
                And("Script does not exists", () =>
                {
                    taskConfiguration.Entries.Add(new TaskConfigurationEntry()
                    {
                        Name = "Script Name",
                        Type = typeof(string),
                        Value = "AFileThatAbsolutelyDoesNotExists.bat"
                    });
                });

                When("spawning");
            }

            [Test]
            public void assure_FileNotFoundException_is_thrown()
            {
                Then(() =>
                    this.ShouldThrowException<FileNotFoundException>(() =>
                        NewScriptTask()));
                }
        }

        [TestFixture]
        public class When_Execute : Shared
        {
            public override void Before()
            {
                Given(SafeDirectoryPathForScripts_exists);
                And(TaskConfiguration_is_created);
                And(ScriptName_is_set);
                And(ScriptTask_is_created);

                When(execute);
            }

            [Test]
            public void assure_a_new_Process_is_started()
            {
                Then(() =>
                    processServiceFake.Verify(s => s.Start(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()), Times.Once()));
            }

            [Test]
            public void assure_executable_FileName_is_correct()
            {
                Then(() =>
                    processServiceFake.Verify(s => s.Start(
                        It.IsAny<string>(),
                        It.Is<string>(v => 
                            v == taskConfiguration.ReadEntryValue("Script Name").ToString()),
                        It.IsAny<string>())));
            }

            [Test]
            public void assure_args_are_passed_to_Process()
            {
                Then(() =>
                    processServiceFake.Verify(s => s.Start(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<string>(v => v.Contains(taskConfiguration.ReadEntryValue("Args").ToString())))));
            }

            [Test]
            public void assure_first_arg_passed_to_Process_is_the_path_to_the_Scripts_directory()
            {
                Then(() =>
                    processServiceFake.Verify(s => s.Start(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<string>(v => v.Contains(safeDirectoryPathForScripts)))));
            }
        }

        public class Shared : SmeedeeScenarioTestClass
        {
            protected static string safeDirectoryPathForScripts = Environment.CurrentDirectory;
            protected static ScriptTask scriptTask;
            protected static TaskConfiguration taskConfiguration = new TaskConfiguration();
            protected static Mock<IProcessService> processServiceFake = new Mock<IProcessService>();

            protected Context SafeDirectoryPathForScripts_exists = () =>
            {
                safeDirectoryPathForScripts = Environment.CurrentDirectory;
            };

            protected Context TaskConfiguration_is_created = () =>
            {
                taskConfiguration = new TaskConfiguration();
            };

            protected Context ScriptName_is_set = () =>
            {
                var scriptFileName = "HelloWorld.bat";
                using (var f = File.Create(Path.Combine(Environment.CurrentDirectory, scriptFileName))) ;
                taskConfiguration.Entries.Add(new TaskConfigurationEntry()
                {
                    Name = "Script Name",
                    Value = scriptFileName,
                    Type = typeof(string)
                });
                taskConfiguration.Entries.Add(new TaskConfigurationEntry()
                {
                    Name = "Args",
                    Type = typeof(string),
                    Value = "/argOne:value"
                });
            };

            protected Context ScriptTask_is_created = () =>
            {
                NewScriptTask();
            };

            protected static void NewScriptTask()
            {
                scriptTask = new ScriptTask(safeDirectoryPathForScripts, taskConfiguration, processServiceFake.Object);
            }

            protected Context ProcessServiceFake_is_created = () =>
            {
                processServiceFake = new Mock<IProcessService>();
            };

            protected When execute = () =>
            {
                scriptTask.Execute();
            };
        }
    }
}
