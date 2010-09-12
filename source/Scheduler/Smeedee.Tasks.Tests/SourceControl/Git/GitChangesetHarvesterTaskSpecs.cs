using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.VCS.Git.DomainModel.Services;
using Smeedee.Tasks.Framework.TaskAttributes;
using Smeedee.Tasks.SourceControl.Git;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.SourceControl.Git
{
    [TestFixture]
    public class Ensuring_the_configuration_attributes_are_correct
    {
        private const bool do_not_inherit = false;

        [Test]
        public void should_have_5_configuration_attributes()
        {
            var settingAttributes = GetGitChangesetHarvesterTaskAttributes<TaskSettingAttribute>();

            settingAttributes.Count().ShouldBe(5);
        }

        [Test]
        public void default_git_pull_command_should_be_correct()
        {
            var settingAttributes = GetGitChangesetHarvesterTaskAttributes<TaskSettingAttribute>();
            var gitPullAttribute = settingAttributes.Where(a => a.SettingName == "GitPullCommand").SingleOrDefault();

            gitPullAttribute.ShouldNotBeNull();
            gitPullAttribute.DefaultValue.ShouldBe("pull origin master");
        }

        [Test]
        public void default_git_installation_location_should_be_empty()
        {
            var settingAttributes = GetGitChangesetHarvesterTaskAttributes<TaskSettingAttribute>();
            var gitPullAttribute = settingAttributes.Where(a => a.SettingName == "GitExecutableLocation").SingleOrDefault();

            gitPullAttribute.ShouldNotBeNull();
            gitPullAttribute.DefaultValue.ShouldBe("");
        }

        private IEnumerable<T> GetGitChangesetHarvesterTaskAttributes<T>()
        {
            var query = from attribute in typeof (GitChangesetHarvesterTask).GetCustomAttributes(do_not_inherit)
                        where attribute is T
                        select (T)attribute;

            return query.ToArray();
        }
    }

    [TestFixture]
    public class When_initializing_a_GitChangesetHarvesterTask : GitChangesetHarvesterTaskTests
    {
        [Test]
        public void Assure_that_the_ProcessRunner_has_a_default_impementation()
        {
            gitTask = CreateGitTask(taskConfig);

            var processRunnerImplementation = gitTask.ProcessRunner as ProcessRunner;

            processRunnerImplementation.ShouldNotBeNull();
        }

        [Test]
        public void Assure_that_the_FileSystemAccessor_has_a_default_impementation()
        {
            gitTask = CreateGitTask(taskConfig);

            var fileSystemWriterImplementation = gitTask.FileSystemAccessor as FileSystemAccessor;

            fileSystemWriterImplementation.ShouldNotBeNull();
        }

        [Test]
        public void Assure_that_the_GitCommandRunner_has_a_default_impementation()
        {
            gitTask = CreateGitTask(taskConfig);

            var gitCommandServiceImplementation = gitTask.GitCommandRunner as GitCommandService;

            gitCommandServiceImplementation.ShouldNotBeNull();
        }

        [Test]
        public void Assure_that_the_GitChangesetRepository_has_a_default_impementation()
        {
            gitTask = CreateGitTask(taskConfig);

            var gitChangesetRepositoryImplementation = gitTask.ManageGitChangesetRepositories as GitChangesetRepositoryManager;

            gitChangesetRepositoryImplementation.ShouldNotBeNull();
        }

        [Test]
        [ExpectedException(typeof(TaskConfigurationException))]
        public void Assure_an_exception_is_thrown_if_a_config_entry_is_missing()
        {
            taskConfig.Entries.RemoveAt(0);
            gitTask = CreateGitTask(taskConfig);
        }

        [Test]
        public void Assure_the_dispatch_interval_is_set_on_the_task_from_the_config()
        {
            gitTask = CreateGitTask(taskConfig);
            gitTask.Interval.ShouldBe(TimeSpan.FromMilliseconds(DISPATCH_INTERVAL));
        }
    }

    [TestFixture]
    public class When_executing_the_GitChangesetHarvesterTask : GitChangesetHarvesterTaskTests
    {
        [Test]
        public void Assure_the_git_clone_command_is_fired_if_no_repository_exists_on_disk()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();
            fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(false);

            gitTask.Execute();

            gitCommandRunnerMock.Verify(g => g.Clone(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Assure_the_git_clone_command_is_not_fired_if_the_repository_exists_on_disk()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();
            fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(true);

            gitTask.Execute();

            gitCommandRunnerMock.Verify(g => g.Clone(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Assure_the_git_clone_command_is_passed_the_correct_source_parameter()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();
            fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(false);

            gitTask.Execute();

            gitCommandRunnerMock.Verify(g => g.Clone(It.IsAny<string>(), localRepositoryPath), Times.Once());
        }

        [Test]
        public void Assure_the_git_clone_command_is_passed_the_correct_target_parameter()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();
            fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(false);
            SetConfigValue("Url", REMOTE_GIT_SOURCE_REPO);

            gitTask.Execute();

            gitCommandRunnerMock.Verify(g => g.Clone(REMOTE_GIT_SOURCE_REPO, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Assure_the_git_pull_command_is_written_to_disk()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();

            gitTask.Execute();

            fileSystemAccessorMock.Verify(f => f.WriteFile(localRepositoryPath, "pull.bat", It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Assure_the_git_pull_command_has_correct_default_git_executable_location()
        {
            string defaultGitExe = PROGRAM_FILES_FOLDER + "Git\\bin\\git.exe";
            SetConfigValue("GitExecutableLocation", "");

            gitTask.Execute();

            fileSystemAccessorMock.Verify(f => f.WriteFile(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.Is<string>(c => c.Contains(defaultGitExe))),
                                          Times.Once());
        }

        [Test]
        public void Assure_the_git_pull_command_has_correct_user_defined_git_executable_location()
        {
            string userDefinedGitExe = "c:\\Foo\\Bar\\git.exe";
            SetConfigValue("GitExecutableLocation", userDefinedGitExe);

            gitTask.Execute();

            fileSystemAccessorMock.Verify(f => f.WriteFile(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.Is<string>(c => c.Contains(userDefinedGitExe))),
                                          Times.Once());
        }

        [Test]
        public void Assure_that_the_process_runner_is_told_to_run_the_pull_script()
        {
            string pullScriptLocation = gitTask.GetLocalRepositoryPath() + "\\pull.bat";

            gitTask.Execute();

            processRunnerMock.Verify(r => r.RunProcess(It.Is<Process>(p => p.StartInfo.FileName == pullScriptLocation)), Times.Once());
        }

        [Test]
        public void Assure_that_the_git_task_asks_for_a_GitChangesetRepository()
        {
            string localRepositoryPath = gitTask.GetLocalRepositoryPath();

            gitTask.Execute();

            gitChangesetRepositoryManagerMock.Verify(p => p.GetRepository(localRepositoryPath), Times.Once());
        }

        [Test]
        public void Assure_the_data_persister_was_asked_to_save_changes()
        {
            gitTask.Execute();

            databasePersisterMock.Verify(c => c.Save(It.IsAny<IEnumerable<Changeset>>()), Times.Once());
        }

        //[Test]
        //public void Assure_that_the_user_specified_location_is_being_used_by_git()
        //{
        //    var fakeExeLocation = "c:\\Some\\Other\\Git\\Folder\\sh.exe";

        //    var exeLocationConfig = taskConfig.Entries.SingleOrDefault(e => e.Name == "GitExecutableLocation");
        //    taskConfig.Entries.Remove(exeLocationConfig);
        //    taskConfig.Entries.Add(new TaskConfigurationEntry
        //                               {
        //                                   Name = "GitExecutableLocation",
        //                                   Type = typeof(string),
        //                                   Value = fakeExeLocation
        //                               });

        //    gitTask.Execute();

        //    processRunnerMock.Verify(p => p.RunProcess(It.Is<Process>(x => x.StartInfo.FileName == fakeExeLocation)));
        //}

        //[Test]
        //public void Assure_that_the_program_files_folder_is_being_used_by_git_by_default()
        //{
        //    string defaultGitShellPath = gitTask.GetGitExecutable();

        //    gitTask.Execute();

        //    processRunnerMock.Verify(p => p.RunProcess(It.Is<Process>(x => x.StartInfo.FileName == defaultGitShellPath)));
        //}

        //[Test]
        //public void Assure_current_directory_is_set_to_local_repository_path()
        //{
        //    var localRepositoryPath = gitTask.GetLocalRepositoryPath();

        //    gitTask.Execute();

        //    fileSystemAccessorMock.Verify(f => f.SetCurrentDirectory(localRepositoryPath), Times.Once());
        //}

        //[Test]
        //public void Assure_a_folder_is_created_for_the_repository_if_it_doesnt_already_exist()
        //{
        //    fileSystemAccessorMock.Setup(f => f.GetAppDataDirectory()).Returns("c:\\AppData");

        //    var localRepositoryPath = gitTask.GetLocalRepositoryPath();
        //    fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(false);

        //    gitTask.Execute();

        //    fileSystemAccessorMock.Verify(f => f.CreateDirectory(localRepositoryPath), Times.Once());
        //}

        //[Test]
        //public void Assure_a_folder_is_not_created_for_the_repository_if_it_already_exists()
        //{
        //    var localRepositoryPath = gitTask.GetLocalRepositoryPath();
        //    fileSystemAccessorMock.Setup(f => f.DirectoryExists(localRepositoryPath)).Returns(true);

        //    gitTask.Execute();

        //    fileSystemAccessorMock.Verify(f => f.CreateDirectory(localRepositoryPath), Times.Never());
        //}
    }

    public class GitChangesetHarvesterTaskTests
    {
        protected Mock<IRepository<Changeset>> changesetDbRepositoryMock;
        protected Mock<IPersistDomainModels<Changeset>> databasePersisterMock;
        protected TaskConfiguration taskConfig;

        protected Mock<IAccessFileSystems> fileSystemAccessorMock;
        protected Mock<IRunProcesses> processRunnerMock;
        protected Mock<IManageGitChangesetRepositories> gitChangesetRepositoryManagerMock;
        protected Mock<IRunGitCommands> gitCommandRunnerMock;

        protected Mock<IRepository<Changeset>> gitChangesetRepository;

        protected GitChangesetHarvesterTask gitTask;

        protected const string LOCAL_GIT_SOURCE_REPO = "C:\\Some\\Git\\Repo";
        protected const string REMOTE_GIT_SOURCE_REPO = "git://github.com/JoeBloggs/FooBar.git";

        protected const string PROGRAM_FILES_FOLDER = "c:\\ProgramFiles\\";
        protected const string APP_DATA_FOLDER = "c:\\AppData\\";

        protected const int DISPATCH_INTERVAL = 123;

        protected GitChangesetHarvesterTask CreateGitTask(TaskConfiguration config)
        {
            return new GitChangesetHarvesterTask(changesetDbRepositoryMock.Object,
                                                 databasePersisterMock.Object,
                                                 config);
        }

        [SetUp]
        public void SetUp()
        {
            changesetDbRepositoryMock = new Mock<IRepository<Changeset>>();
            databasePersisterMock = new Mock<IPersistDomainModels<Changeset>>();

            fileSystemAccessorMock = new Mock<IAccessFileSystems>();
            processRunnerMock = new Mock<IRunProcesses>();
            gitChangesetRepositoryManagerMock = new Mock<IManageGitChangesetRepositories>();
            gitCommandRunnerMock = new Mock<IRunGitCommands>();

            gitChangesetRepository = new Mock<IRepository<Changeset>>();

            changesetDbRepositoryMock.Setup(c => c.Get(It.IsAny<Specification<Changeset>>()))
                                     .Returns(new List<Changeset>());

            gitChangesetRepositoryManagerMock.Setup(c => c.GetRepository(It.IsAny<string>()))
                                             .Returns(gitChangesetRepository.Object);

            fileSystemAccessorMock.Setup(f => f.AppDataDirectory)
                                  .Returns(APP_DATA_FOLDER);
            fileSystemAccessorMock.Setup(f => f.ProgramFilesDirectory)
                                  .Returns(PROGRAM_FILES_FOLDER);

            taskConfig = new TaskConfiguration();
            taskConfig.Name = "My Git Task";
            taskConfig.TaskName = "Get Changeset Harvester";
            taskConfig.DispatchInterval = DISPATCH_INTERVAL;
            taskConfig.Entries = new List<TaskConfigurationEntry>
                                     {
                                         new TaskConfigurationEntry { Name = "Url", Type = typeof(string), Value = LOCAL_GIT_SOURCE_REPO },
                                         new TaskConfigurationEntry { Name = "Username", Type = typeof(string), Value = "" },
                                         new TaskConfigurationEntry { Name = "Password", Type = typeof(string), Value = "" },
                                         new TaskConfigurationEntry { Name = "GitPullCommand", Type = typeof(string), Value = "pull origin master" },
                                         new TaskConfigurationEntry { Name = "GitExecutableLocation", Type = typeof(string), Value = "" }
                                     };

            gitTask = CreateGitTask(taskConfig);

            gitTask.ManageGitChangesetRepositories = gitChangesetRepositoryManagerMock.Object;
            gitTask.FileSystemAccessor = fileSystemAccessorMock.Object;
            gitTask.ProcessRunner = processRunnerMock.Object;
            gitTask.GitCommandRunner = gitCommandRunnerMock.Object;
        }

        protected void SetConfigValue(string name, string value)
        {
            var config = taskConfig.Entries.Single(c => c.Name == name);
            config.Value = value;
        }
    }
}