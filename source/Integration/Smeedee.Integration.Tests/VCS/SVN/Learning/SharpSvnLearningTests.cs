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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Linq;
using NUnit.Framework;
using SharpSvn;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;

using SharpSvn.Security;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.VCS.SVN.Learning
{
    public class SharpSVNContext
    {
        protected static SharpSvn.SvnClient svnClient;
        protected static string repositoryUrl;
        protected static string localDirectory;

        protected void SetupContext()
        {
            svnClient = new SharpSvn.SvnClient();
            svnClient.Authentication.DefaultCredentials = new NetworkCredential("guest", "");
            svnClient.Authentication.SslServerTrustHandlers += AcceptCerticate;
            repositoryUrl = "https://agileprojectdashboard.org:8443/svn/IntegrationTest/trunk";
            localDirectory = Path.Combine(Environment.CurrentDirectory, "IntegrationTest");
        }

        private void AcceptCerticate(object sender, SvnSslServerTrustEventArgs e)
        {
            e.AcceptedFailures = e.Failures;
            e.Save = true; // Save acceptance to authentication store
        }

        void Authentication_UserNameHandlers(object sender, SharpSvn.Security.SvnUserNameEventArgs e)
        {
            e.UserName = "guest";
            e.Save = true;
        }

        protected static void CleanRepository()
        {
            svnClient.CheckOut(new SvnUriTarget(repositoryUrl), localDirectory);
            foreach (var file in Directory.GetFiles(localDirectory))
            {
                svnClient.Delete(file);
            }

            svnClient.Commit(localDirectory, new SvnCommitArgs() { LogMessage = "Empty repository" });
        }

        protected long GetLastRevision()
        {
            var latestChangeset = GetLatestChangeset();
            return latestChangeset.Revision;
        }

        protected SvnLogEventArgs GetLatestChangeset()
        {
            Collection<SvnLogEventArgs> log;
            svnClient.GetLog(new Uri(repositoryUrl), new SvnLogArgs() { Limit = 1 }, out log);
            return log.First();
        }


        protected static string FilepathInLocalDirectory(string filename)
        {
            var path = Path.Combine(localDirectory, filename);
            return path;
        }

        protected static void CreateFileInLocalDirectory(string filepath)
        {
            using (var sw = new StreamWriter(filepath))
            {
            };

        }

      


    }

    [TestFixture]
    [Ignore]
    public class SharpSvnLearningTests : SharpSVNContext
    {
        [SetUp]
        public void Setup()
        {
            SetupContext();
        }

        Context the_repository_exists = () =>
        {
        };

        Context the_repository_is_empty = () =>
        {
            CleanRepository();
        };

        Context the_repository_contain_one_file = () =>
        {
            CleanRepository();

            var filepath = FilepathInLocalDirectory("goeran.txt");
            CreateFileInLocalDirectory(filepath);

            svnClient.Add(filepath);
            svnClient.Commit(localDirectory, new SvnCommitArgs() { LogMessage = "Add file programatically" });
        };

        [Test]
        public void How_to_check_if_credentials_are_valid()
        {
            TryCredentials("haldis", "haldis").ShouldBeFalse();
            TryCredentials("guest", "").ShouldBeTrue();
        }

        private bool TryCredentials(string username, string password)
        {
            var client = new SvnClient();
            client.Authentication.Clear();
            client.Authentication.DefaultCredentials = new NetworkCredential(username, password);
            client.Authentication.SslServerTrustHandlers += (o, e) =>
            {
                e.AcceptedFailures = e.Failures;
                e.Save = true;
            };

            try
            {
                Collection<SvnLogEventArgs> logs;
                client.GetLog(new Uri("https://agileprojectdashboard.org:8443/svn/CPMonitor/trunk"), new SvnLogArgs() { Limit = 1 }, out logs);
                return true;
            }
            catch (SvnAuthorizationException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        [Test]
        public void How_to_get_info_about_latest_changeset()
        {
            Scenario.StartNew(this, "Get log", scenario =>
            {
                scenario.Given(the_repository_exists);

                scenario.When("log is requested");

                scenario.Then("assure log is received", () =>
                {
                    Collection<SvnLogEventArgs> logs;
                    svnClient.GetLog(new Uri(repositoryUrl), new SvnLogArgs() { Limit = 5 }, out logs);
                    logs.ShouldHaveMoreThan(0);
                    logs.First().LogMessage.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void How_to_checkout()
        {
            Scenario.StartNew(this, "Checkout", scenario =>
            {
                scenario.Given(the_repository_exists);

                scenario.When("a checkout is performed", () =>
                    svnClient.CheckOut(new SvnUriTarget(repositoryUrl, new SvnRevision(1)), localDirectory));

                scenario.Then("assure files has been checked out", () =>
                    Directory.Exists(localDirectory).ShouldBeTrue());
            });
        }

        [Test]
        public void How_to_update()
        {
            Scenario.StartNew(this, "Update", scenario =>
            {
                scenario.Given(the_repository_exists);

                scenario.When("a Update is performed", () =>
                    svnClient.Update(localDirectory));
            });
            
        }

        [Test]
        public void How_to_add_a_file_and_commit()
        {
            Scenario.StartNew(this, "Add file and commit", scenario =>
            {
                string filename = "goeran.txt";
                string filepath = FilepathInLocalDirectory(filename);
                long lastRevision = GetLastRevision();
           
                scenario.
                    Given(the_repository_is_empty).
                    And("a file is created in local directory", () =>
                        CreateFileInLocalDirectory(filepath)).
                    And("then added to the repository", () =>
                        svnClient.Add(filepath));

                scenario.When("a Commit is performed", () =>
                    svnClient.Commit(localDirectory, new SvnCommitArgs() { LogMessage = "Checkin programatically" }));

                scenario.Then("assure changes has been commited", () =>
                {
                    var changeset = GetLatestChangeset();
                    changeset.LogMessage.ShouldBe("Checkin programatically");
                    changeset.ChangedPaths.First().Path.ShouldBe(
                        string.Format("/trunk/{0}", filename));
                    changeset.ChangedPaths.First().Action.ShouldBe(SvnChangeAction.Add);
                    (changeset.Revision > lastRevision).ShouldBeTrue();
                });
            });

        }

        [Test]
        public void How_to_delete_a_file_and_commit()
        {
            Scenario.StartNew(this, "Delete file and commit", scenario =>
            {
                long lastRevision = GetLastRevision();
     
                scenario.Given(the_repository_contain_one_file).
                    And("file is deleted from the repository", () =>
                    {
                        var filepath = Directory.GetFiles(localDirectory).First();
                        svnClient.Delete(filepath);
                    });

                scenario.When("a Commit is performed", () =>
                    svnClient.Commit(localDirectory, new SvnCommitArgs() { LogMessage = "Deleting file in repository" }));

                scenario.Then("assure the file have been deleted from the repository", () =>
                {
                    var latestChangeset = GetLatestChangeset();
                    latestChangeset.LogMessage.ShouldBe("Deleting file in repository");
                    latestChangeset.ChangedPaths.First().Action.ShouldBe(SvnChangeAction.Delete);
                    latestChangeset.ChangedPaths.First().Path.ShouldBe("/trunk/goeran.txt");
                    (latestChangeset.Revision > lastRevision).ShouldBeTrue();
                });
            });
        }
    }
}
