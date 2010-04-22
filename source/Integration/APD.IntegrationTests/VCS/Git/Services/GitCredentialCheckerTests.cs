using System.IO;

using APD.Integration.VCS.Git.DomainModel.Services;
using APD.IntegrationTests.VCS.Git.Context;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.Git.Services
{
    [TestFixture]
    public class SshCredentialCheckerTests : GitCredentialCheckerContext
    {
        protected GitCredentialChecker credentialChecker;

        #region Setup/Teardown
        [SetUp]
        public void SetupContext()
        {
            SetupSshDir();
            credentialChecker = new GitCredentialChecker();
        }
        #endregion

        [Test]
        public void AssureGeneratedRsaKeyIsWrittenToFileAndIsValid()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("no SSH key exists.", RemoveSmeedeeKeysFromSSHDirectory);
                scenario.When("we try to generate SSH key to file", () => sshKey = credentialChecker.GetSshKey("rsa"));
                scenario.Then("a generated rsa ssh file should exist in public and private version.", () =>
                {
                    AssureFileExists(sshRsaKeyPath);
                    ReadFile(sshRsaKeyPath + ".pub").StartsWith("ssh-rsa").ShouldBeTrue();
                    AssureKeyIsValid(sshRsaKeyPath, "ssh-rsa", rsaKeyLength);
                });
            });
        }


        [Test]
        public void AssureRsaIsGeneratedWhenDsaExists()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("a dsa ssh key exists", DsaSshKeyExists);
                scenario.When("we request ssh key", () =>
                {
                    sshKey = credentialChecker.GetSshKey("rsa");
                });
                scenario.Then("a valid rsa ssh key should be retrieved", () => AssureKeyIsValid(sshRsaKeyPath,"ssh-rsa",rsaKeyLength));
            });
        }

        [Test]
        public void AssureGeneratedDsaKeyIsWrittenToFile()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("no SSH key exists.", RemoveSmeedeeKeysFromSSHDirectory);
                scenario.When("We try to generate dsa SSH key to file", () =>
                {
                    sshKey = credentialChecker.GetSshKey("dsa");
                });
                scenario.Then("File should exist in public and private version.", () =>
                {
                    AssureFileExists(sshDsaKeyPath);
                    ReadFile(sshDsaKeyPath+".pub").StartsWith("ssh-dss").ShouldBeTrue();
                    AssureKeyIsValid(sshDsaKeyPath, "ssh-dss", dsaKeyLength);
                });
            });
        }

        protected void AssureKeyIsValid(string filepath, string startsWith, int length)
        {
            string rawKey = ReadFile(filepath + ".pub");
            sshKey.Equals(rawKey).ShouldBeTrue();
            sshKey.Length.ShouldBe(length);
            sshKey.StartsWith(startsWith).ShouldBeTrue();
        }


        protected static void AssureFileExists(string filepath)
        {
            File.Exists(filepath).ShouldBeTrue();
            File.Exists(filepath + ".pub").ShouldBeTrue();
        }

       
    }
}
