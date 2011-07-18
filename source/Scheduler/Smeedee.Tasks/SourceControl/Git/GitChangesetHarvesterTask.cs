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
    [TaskSetting(3, SOURCECONTROL_SERVER_NAME, typeof(string), "Main Sourcecontrol Server")]
    [TaskSetting(4, URL_SETTING_NAME, typeof(string), "")]
    [TaskSetting(5, GIT_PULL_COMMAND, typeof(string), "pull origin master")]
    [TaskSetting(6, GIT_EXECUTABLE_LOCATION, typeof(string), "C:\\msysgit\\msysgit\\bin\\git.exe", "Full path to git.exe")]
    [TaskSetting(7, MINGW_BINARY_DIRECTORY, typeof(string), "C:\\msysgit\\msysgit\\mingw\\bin", "Typically found in the mingw folder in \nbase msysgit install folder")]
    public class GitChangesetHarvesterTask : ChangesetHarvesterBase
    {
        public const string GIT_PULL_COMMAND = "GitPullCommand";
        public const string GIT_EXECUTABLE_LOCATION = "GitExecutableLocation";
        public const string MINGW_BINARY_DIRECTORY = "MingwBinaryDir";

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
            : base(changesetDbRepository, databasePersister, config)
        {
            Guard.Requires<ArgumentNullException>(changesetDbRepository != null);
            Guard.Requires<ArgumentNullException>(databasePersister != null);
            Guard.Requires<ArgumentNullException>(config != null);
            Guard.Requires<TaskConfigurationException>(config.Entries.Count() == 7);
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
            string contents = string.Format("set HOME=\"{0}\"{1}",
                                        targetPath,
                                        Environment.NewLine);
            contents += string.Format("set PATH=$PATH;{0}{1}", 
                                        config.Entries.Single(c => c.Name == MINGW_BINARY_DIRECTORY).Value, 
                                        Environment.NewLine);
            contents += string.Format("\"{0}\" --git-dir \"{1}\\.git\" {2} {3}",
                                      GetGitExecutable(),
                                      targetPath, 
                                      "reset --hard HEAD", 
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
