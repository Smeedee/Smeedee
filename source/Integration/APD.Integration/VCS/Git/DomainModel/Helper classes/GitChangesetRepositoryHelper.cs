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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Diagnostics;
using System.IO;

using APD.DomainModel.Config;
using APD.Integration.VCS.Git.DomainModel.Exceptions;


namespace APD.Integration.VCS.Git.DomainModel.RepositoryHelpers
{
    public class GitChangesetRepositoryHelper
    {
        private readonly Configuration configuration;
        public string OurRepoPath { get; private set; }
        public string SmeedeeDataLocation { get; private set; }
        public string BatScriptDir { get; private set; }
        public string PullScriptPath { get; private set; }
        public string RemoteRepoURL { get; private set; }
        public string CloneScriptPath { get; private set; }
        public string HomeDirPath { get; private set; }

        public GitChangesetRepositoryHelper(string repoUrl) : this(repoUrl, null) { }

        public GitChangesetRepositoryHelper(string repoUrl, Configuration config)
        {
            SmeedeeDataLocation = GetSmeedeeDataLocation();
            CreateDirectoryIfNotExists(SmeedeeDataLocation);

            string personalDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (personalDir == null)
                throw new DirectoryNotFoundException("Cannot locate personal directory.");

            var homeDir = Directory.GetParent(personalDir);
            if (homeDir == null)
                throw new DirectoryNotFoundException("Cannot locate home directory.");
            HomeDirPath = homeDir.ToString();


            BatScriptDir = Path.Combine(SmeedeeDataLocation, "bat");
            CreateDirectoryIfNotExists(BatScriptDir);

            RemoteRepoURL = repoUrl;
            GenerateCloneScript(repoUrl);

            GeneratePullScript();

            configuration = config;
        }

        public void RunPullScript()
        {
            var pulling = new Process
            {
                StartInfo = { FileName = PullScriptPath }
            };

            pulling.Start();
            pulling.WaitForExit();
            if (pulling.ExitCode != 0)
                throw new GitScriptErrorException("Error while running git pull script. Please run the git-pull.bat script in " + BatScriptDir +
                                                  " manually to discover the solution to this.");
            pulling.Close();

        }

        public void RunCloneScript()
        {
            if (!Directory.Exists(OurRepoPath))
            {
                var cloning = new Process
                {
                    StartInfo = { FileName = CloneScriptPath }
                };

                cloning.Start();
                cloning.WaitForExit();
                if (cloning.ExitCode != 0)
                    throw new GitScriptErrorException("Error while running git clone script. Please run the git-clone.bat script in " +
                                                      BatScriptDir +
                                                      " manually to discover the solution to this.");
                cloning.Close();
            }
        }

        public void GenerateCloneScript(string reposUrl)
        {
            OurRepoPath = Path.Combine(SmeedeeDataLocation, "git_repos");
            CloneScriptPath = Path.Combine(BatScriptDir, "git-clone.bat");

            if (!File.Exists(CloneScriptPath))
            {
                TextWriter writer = new StreamWriter(CloneScriptPath);
                writer.WriteLine("set HOME=" + HomeDirPath);
                writer.Write(GetQuotedGitPath() + " clone " + reposUrl + " " + OurRepoPath);
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        private static string GetSmeedeeDataLocation()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "smeedee_data");
        }

        private static void CreateDirectoryIfNotExists(string location)
        {
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
            if (!Directory.Exists(location))
                throw new DirectoryNotFoundException("Unable to find directory " + location + " after creating it.");
        }

        public string GetQuotedGitPath()
        {
            if (configuration != null && configuration.ContainSetting("gitpath"))
            {
                var quotedPath = "\"" + configuration.GetSetting("gitpath") + "\"";
                return quotedPath;
            }

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "git");
            path = Path.Combine(path, "bin");
            path = Path.Combine(path, "git.exe");
            return "\"" + path + "\"";

        }

        public void GeneratePullScript()
        {
            PullScriptPath = Path.Combine(BatScriptDir, "git-pull.bat");

            if (!File.Exists(PullScriptPath))
            {
                TextWriter writer = new StreamWriter(PullScriptPath);
                writer.WriteLine("set HOME=" + HomeDirPath);
                writer.Write(GetQuotedGitPath() + " --git-dir \"" + Path.Combine(OurRepoPath, ".git") + "\" pull origin master");
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }

        }
    }
}
