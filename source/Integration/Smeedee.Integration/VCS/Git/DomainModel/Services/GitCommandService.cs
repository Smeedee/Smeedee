using System;

namespace Smeedee.Integration.VCS.Git.DomainModel.Services
{
    public class GitCommandService : IRunGitCommands
    {
        public void Clone(string fromUrl, string toPath)
        {
            GitSharp.Git.Clone(fromUrl, toPath);
        }
    }
}