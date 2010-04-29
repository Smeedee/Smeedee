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

using System.Diagnostics;
using System.IO;
using APD.Integration.VCS.Git.DomainModel.RepositoryHelpers;
using APD.IntegrationTests.VCS.Git.Context;
using GitSharp;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.Git.DomainModel.RepositoryHelpers
{
    [TestFixture]
    public class GitChangesetRepositoryHelperTests : GitPluginTestContext
    {
        private GitChangesetRepositoryHelper gitSharpHelper;

        [SetUp]
        public void Setup()
        {
            reposToBeCloned = "git://github.com/flyrev/Smeedee_dummy.git";
            gitSharpHelper = new GitChangesetRepositoryHelper(reposToBeCloned);
            gitSharpHelper = new GitChangesetRepositoryHelper(reposToBeCloned);
        }

        private void TheBatScriptsDoesNotExist()
        {
            try
            {
                if (File.Exists(gitSharpHelper.PullScriptPath))
                    File.Delete(gitSharpHelper.PullScriptPath);
                if (File.Exists(gitSharpHelper.CloneScriptPath))
                    File.Delete(gitSharpHelper.CloneScriptPath);
            }catch (IOException)
            {
                Debug.WriteLine("Please delete the bat scripts manually");
                throw;
            }
        }

        [Test]
        public void AssureCloneScriptIsGenerated()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("The clone scripts do not exist", TheBatScriptsDoesNotExist);
                scenario.When("I call the generate function", () => gitSharpHelper.GenerateCloneScript(reposToBeCloned));
                scenario.Then("the clone script is generated", CheckThatTheCloneScriptWasGenerated);
            });
            
        }

        public void CheckThatTheCloneScriptWasGenerated()
        {
            File.Exists(gitSharpHelper.CloneScriptPath).ShouldBeTrue();
            var fileReader = File.OpenText(gitSharpHelper.CloneScriptPath);
            string fileContents = fileReader.ReadToEnd();
            fileContents.IndexOf("git").ShouldNotBe(-1);
            fileContents.IndexOf("clone").ShouldNotBe(-1);
        }

        [Test]
        [Ignore] // Integration specific
        public void AssureRunningThePullScriptResultsInAClonedRepository()
        {
            Scenario.StartNew(this, scenario => {
                scenario.Given("The clone script is in place", () => 
                gitSharpHelper.GenerateCloneScript(gitSharpHelper.RemoteRepoURL));
                scenario.When("I run the clone script", gitSharpHelper.RunCloneScript);
                scenario.Then("A clone should exist at the destination", AssureACloneWasMade);
            });

        }

        public void AssureACloneWasMade()
        {
            gitSharpHelper.OurRepoPath.ShouldNotBeNull();
            Directory.Exists(gitSharpHelper.OurRepoPath).ShouldBeTrue();
            var rep = new Repository(gitSharpHelper.OurRepoPath);
            rep.Get<Commit>("HEAD").ShouldNotBeNull();
        }

        [Test]
        public void AssurePullScriptIsGenerated()
        {
            gitSharpHelper.GeneratePullScript();
            gitSharpHelper.PullScriptPath.ShouldNotBeNull();
            File.Exists(gitSharpHelper.PullScriptPath).ShouldBeTrue();
        }

        [Test]
        [Ignore] // Integration specific
        public void AssureRunningThePullScriptResultsInAPull()
        {
            var rep = new Repository(gitSharpHelper.OurRepoPath);
            rep.Get<Commit>("HEAD").ShouldNotBeNull();
            gitSharpHelper.RunPullScript();
        }
    }
}
