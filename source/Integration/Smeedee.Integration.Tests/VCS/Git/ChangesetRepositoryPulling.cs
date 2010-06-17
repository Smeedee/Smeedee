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

using Smeedee.Integration.VCS.Git.DomainModel;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.IntegrationTests.VCS.Git
{
    [Ignore]
    [TestFixture]
    public class when_pulling : ChangesetRepositorySpecs
    {
        #region Setup/Teardown
        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }
        #endregion

        [Test]
        public void assure_the_pull_is_done_gracefully()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("the repository exists");
                scenario.When("I try to pull", try_to_pull);
                scenario.Then("no exceptions are thrown");
            });

        }

        private void try_to_pull()
        {
            var oldCwd = Directory.GetCurrentDirectory();
            var pullScriptPath = Directory.GetCurrentDirectory();
            // cwd = C:\Smeedee_rev118_with_Git\source\Integration\Smeedee.IntegrationTests\bin\Debug ...
            try
            {
                while (!Path.GetFileName(pullScriptPath).Equals("source"))
                {
                    pullScriptPath = Directory.GetParent(pullScriptPath).ToString();
                    Console.Write    (pullScriptPath + " : ");
                    Console.WriteLine(Path.GetFileName(pullScriptPath));
                }

                pullScriptPath = Directory.GetParent(pullScriptPath).ToString();
                pullScriptPath = Path.Combine(pullScriptPath, @"tools\GitSharp_binaries\pull.bat");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("I couldn't locate pull.bat");
                throw new NullReferenceException();
            }

            Directory.SetCurrentDirectory(GitChangesetRepository.ReposDir);
            string git_shell = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                   git_shell = Path.Combine(git_shell, @"Git\bin\sh.exe");
            //string cmdArgs = "/c \"" +  git_shell + "\" " + pullScriptPath;
            var p = new Process
            {
                StartInfo =
                    {
                        CreateNoWindow = true,
                        FileName = git_shell,
                        Arguments = pullScriptPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
            };
            p.Start();
            Console.WriteLine("Git repository: ");
            p.BeginOutputReadLine();
            Console.WriteLine(p.StandardError.ReadToEnd());

            while (!p.WaitForExit(100)) {}
            if (p.ExitCode != 0)
                throw new InvalidProgramException("Git exited with exit code " + p.ExitCode);
            Directory.SetCurrentDirectory(oldCwd);
        }
    }
}
