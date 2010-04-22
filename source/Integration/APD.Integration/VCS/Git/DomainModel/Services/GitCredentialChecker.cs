using System;
using System.IO;
using System.Linq;
using APD.DomainModel.Framework.Services;

using Tamir.SharpSsh.jsch;
using Tamir.SharpSsh.jsch.examples;


namespace APD.Integration.VCS.Git.DomainModel.Services
{
    public class GitCredentialChecker:ICheckIfCredentialsIsValid
    {
        protected static string sshKeyDirPath = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).ToString(), @".ssh\");
        protected static string sshRsaKeyPath = Path.Combine(sshKeyDirPath, "Smeedee_RSA");
        protected static string sshDsaKeyPath = Path.Combine(sshKeyDirPath, "Smeedee_DSA");

        public string GetSshKey(string type)
        {
            if (type.Equals("rsa"))
            {
                if (RsaKeyExists())
                    return ReadFile(sshRsaKeyPath);
                
                return GenerateSshKey(sshRsaKeyPath, type);
            }

            if (type.Equals("dsa"))
            {
                if (DsaKeyExists())
                    return ReadFile(sshDsaKeyPath);

                return GenerateSshKey(sshDsaKeyPath, type);
            }

            throw new ArgumentException("Type must be rsa or dsa.");
        }

        public string GenerateSshKey(string destination, string type)
        {
            if (!IsValidArguments(new[] { type, destination, "" }))
            {
                throw new ArgumentException("Invalid arguments to GenerateSshKey");
            }
            GenerateSshKeyToFile(new[] { type, destination, "" });
            return ReadFile(destination);
        }

        protected static string ReadFile(string filename)
        {
            var f = new FileStream(filename + ".pub", FileMode.Open);
            var sr = new StreamReader(f);

            string key = sr.ReadToEnd();

            sr.Close();

            return key;
        }

        public bool SshKeyExists()
        {
            return RsaKeyExists() || DsaKeyExists();
        }

        public bool RsaKeyExists()
        {
            return Directory.GetFiles(sshKeyDirPath).Contains(sshRsaKeyPath) && ReadFile(sshRsaKeyPath).StartsWith("ssh-rsa");
        }

        public bool DsaKeyExists()
        {
            return Directory.GetFiles(sshKeyDirPath).Contains(sshRsaKeyPath) && ReadFile(sshRsaKeyPath).StartsWith("ssh-dss");
        }

        protected bool IsValidArguments(params string[] arg)
        {
            return arg.Length == 3 && ( arg[0].Equals("rsa") || arg[0].Equals("dsa") );
        }

        protected int DetermineSshType(string type)
        {   
            return type.Equals("rsa") ? KeyPair.RSA : KeyPair.DSA;          
        }

        protected void GenerateSshKeyToFile(params string[] arg)
        {
            try
            {
                int type = DetermineSshType(arg[0]);
                String filename = arg[1];
                String comment = arg[2];
                var jsch = new JSch();
                String passphrase = InputForm.GetUserInput("Enter passphrase (empty for no passphrase)", true);

                KeyPair kpair = KeyPair.genKeyPair(jsch, type);
                kpair.setPassphrase(passphrase);
                kpair.writePrivateKey(filename);
                kpair.writePublicKey(filename + ".pub", comment);
                kpair.dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return;
        }

        public bool Check(string provider, string url, string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
