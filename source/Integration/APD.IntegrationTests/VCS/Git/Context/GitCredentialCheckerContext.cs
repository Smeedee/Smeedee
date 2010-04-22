using System;
using System.Collections.Generic;
using System.IO;

using Tamir.SharpSsh.jsch;

using FileMode=System.IO.FileMode;


namespace APD.IntegrationTests.VCS.Git.Context
{
    public class GitCredentialCheckerContext
    {
        protected static string sshKeyDirPath = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).ToString(), @".ssh\");
        protected static string sshRsaKeyPath = Path.Combine(sshKeyDirPath, "Smeedee_RSA");
        protected static string sshDsaKeyPath = Path.Combine(sshKeyDirPath, "Smeedee_DSA");

        protected static List<string> keyFilePaths = new List<string>(); 
        //Not entirely sure about these
        protected static int rsaKeyLength = 210;
        protected static int dsaKeyLength = 586;

        protected static string sshKey = "";

        protected static void SetupSshDir()
        {
            var personalDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var personalDirParent = Directory.GetParent(personalDir);
            if (personalDirParent == null)
                throw new DirectoryNotFoundException("Unable to find personal directory");

            sshKeyDirPath = Path.Combine(personalDirParent.ToString(), ".ssh");
            AddKeyToFilePaths(sshRsaKeyPath);
            AddKeyToFilePaths(sshDsaKeyPath);
        }

        protected static void AddKeyToFilePaths(string path)
        {
            keyFilePaths.Add(path);
            keyFilePaths.Add(path +".pub");
        }
        

        protected void RsaSshKeyExists()
        {
            if (File.Exists(sshRsaKeyPath) && !ReadFile(sshRsaKeyPath).StartsWith("ssh-rsa"))
            {
                RemoveSmeedeeKeysFromSSHDirectory();
                var jsch = new JSch();

                KeyPair kpair = KeyPair.genKeyPair(jsch, KeyPair.RSA);
                kpair.setPassphrase("");
                kpair.writePrivateKey(sshRsaKeyPath);
                kpair.writePublicKey(sshRsaKeyPath + ".pub", "");
                kpair.dispose();
            }
        }

        protected void DsaSshKeyExists()
        {
            if (File.Exists(sshDsaKeyPath) && !ReadFile(sshDsaKeyPath).StartsWith("ssh-dss"))
            {
                RemoveSmeedeeKeysFromSSHDirectory();
                var jsch = new JSch();

                KeyPair kpair = KeyPair.genKeyPair(jsch, KeyPair.DSA);
                kpair.setPassphrase("");
                kpair.writePrivateKey(sshDsaKeyPath);
                kpair.writePublicKey(sshDsaKeyPath + ".pub", "");
                kpair.dispose();
            }
        }


        protected static void RemoveSmeedeeKeysFromSSHDirectory()
        {
            if (sshKeyDirPath == null)
            {
                var personalDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var personalDirParent = Directory.GetParent(personalDir);
                if (personalDirParent==null)
                    throw new DirectoryNotFoundException("Unable to find personal directory");
                sshKeyDirPath = Path.Combine(personalDirParent.ToString(), ".ssh");
            }
            
            foreach (var file in keyFilePaths)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        protected static string ReadFile(string filepath)
        {
            var f = new FileStream(filepath, FileMode.Open);
            var sr = new StreamReader(f);
            string key = sr.ReadToEnd();

            sr.Close();

            return key;
        }
    }
}
