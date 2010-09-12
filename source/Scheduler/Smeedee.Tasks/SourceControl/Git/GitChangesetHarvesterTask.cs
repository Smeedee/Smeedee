using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.VCS.Git.DomainModel.Services;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.SourceControl.Git
{
    [Task("Git Changeset Harvester",
          Author = "Smeedee Team",
          Description = "Retrieves information from a Git version control repository. Used to populate Smeedee's database with information about the latest commits and other commit statistics.",
          Version = 1,
          Webpage = "http://smeedee.org")]
    [TaskSetting(1, USERNAME_SETTING_NAME, typeof(string), "guest")]
    [TaskSetting(2, PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, URL_SETTING_NAME, typeof(string), "")]
    [TaskSetting(4, GIT_PULL_COMMAND, typeof(string), "pull origin master")]
    [TaskSetting(5, GIT_EXECUTABLE_LOCATION, typeof(string), "")]
    public class GitChangesetHarvesterTask : ChangesetHarvesterBase
    {
        public const string GIT_PULL_COMMAND = "GitPullCommand";
        public const string GIT_EXECUTABLE_LOCATION = "GitExecutableLocation";

        private readonly TaskConfiguration config;

        public override string Name
        {
            get { return "Git Changeset Harvester"; }
        }

        private IRunProcesses _processRunner;
        public IRunProcesses ProcessRunner
        {
            get
            {
                if (_processRunner == null)
                    _processRunner = new ProcessRunner();
                return _processRunner;
            }
            set { _processRunner = value; }
        }

        private IAccessFileSystems _fileSystemAccessor;
        public IAccessFileSystems FileSystemAccessor
        {
            get
            {
                if (_fileSystemAccessor == null)
                    _fileSystemAccessor = new FileSystemAccessor();
                return _fileSystemAccessor;
            }
            set { _fileSystemAccessor = value; }
        }

        private IManageGitChangesetRepositories _manageGitChangesetRepositories;
        public IManageGitChangesetRepositories ManageGitChangesetRepositories
        {
            get
            {
                if (_manageGitChangesetRepositories == null)
                    _manageGitChangesetRepositories = new GitChangesetRepositoryManager();
                return _manageGitChangesetRepositories;
            }
            set { _manageGitChangesetRepositories = value; }
        }
        
        private IRunGitCommands _gitCommandRunner;
        public IRunGitCommands GitCommandRunner
        {
            get
            {
                if (_gitCommandRunner == null)
                    _gitCommandRunner = new GitCommandService();
                return _gitCommandRunner;
            }
            set { _gitCommandRunner = value; }
        }

        public GitChangesetHarvesterTask(IRepository<Changeset> changesetDbRepository,
                                         IPersistDomainModels<Changeset> databasePersister,
                                         TaskConfiguration config) 
            : base(changesetDbRepository, databasePersister)
        {
            Guard.ThrowIfNull<ArgumentNullException>(changesetDbRepository, databasePersister, config);
            Guard.Requires<TaskConfigurationException>(config.Entries.Count() == 5);
            this.config = config;
            Interval = TimeSpan.FromMilliseconds(config.DispatchInterval);
        }

        public override void Execute()
        {
            string sourcePath = (string)config.Entries.Single(c => c.Name == URL_SETTING_NAME).Value;
            string targetPath = GetLocalRepositoryPath();

            EnsureGitClone(targetPath, sourcePath);
            EnsureGitPull(targetPath);
            SaveChangesets(targetPath);
        }

        private void EnsureGitClone(string targetPath, string sourcePath)
        {
            if (!FileSystemAccessor.DirectoryExists(targetPath))
            {
                GitCommandRunner.Clone(sourcePath, targetPath);
            }
        }

        private void EnsureGitPull(string targetPath)
        {
            /*
             set HOME="C:\Code\GitSharpTestApp\sandbox\CodeKataExercises"
             "C:\Program Files (x86)\Git\bin\git.exe" --git-dir "C:\Code\GitSharpTestApp\sandbox\CodeKataExercises\.git" pull origin master
            */
            string contents = string.Format("set HOME=\"{0}\"{1}",
                                            targetPath,
                                            Environment.NewLine);
            contents += string.Format("\"{0}\" --git-dir \"{1}\\.git\" {2}",
                                      GetGitExecutable(),
                                      targetPath,
                                      config.Entries.Single(c => c.Name == GIT_PULL_COMMAND).Value);

            FileSystemAccessor.WriteFile(targetPath, "pull.bat", contents);

            var pullProcess = new Process { StartInfo = { FileName = targetPath + "\\pull.bat" } };
            ProcessRunner.RunProcess(pullProcess);
        }

        private void SaveChangesets(string targetPath)
        {
            var gitChangesetRepository = ManageGitChangesetRepositories.GetRepository(targetPath);
            SaveUnsavedChangesets(gitChangesetRepository);
        }

        private string GetGitExecutable()
        {
            string gitExecutable = (string)config.ReadEntryValue(GIT_EXECUTABLE_LOCATION);
            if (string.IsNullOrEmpty(gitExecutable))
            {
                gitExecutable = Path.Combine(FileSystemAccessor.ProgramFilesDirectory, "Git\\bin\\git.exe");
            }
            return gitExecutable;
        }

        public string GetLocalRepositoryPath()
        {
            return Path.Combine(FileSystemAccessor.AppDataDirectory, "Smeedee", config.Name);
        }
    }
}
