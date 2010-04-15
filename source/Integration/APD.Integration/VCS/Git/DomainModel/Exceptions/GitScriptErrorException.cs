using System;
using System.Diagnostics;

namespace APD.Integration.VCS.Git.DomainModel.Exceptions
{
    public class GitScriptErrorException : Exception
    {
        public GitScriptErrorException(string s)
        {
            Debug.WriteLine(s);
        }
    }
}
