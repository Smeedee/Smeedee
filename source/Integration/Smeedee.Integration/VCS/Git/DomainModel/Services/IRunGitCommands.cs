using System;

namespace Smeedee.Integration.VCS.Git.DomainModel.Services
{
    public interface IRunGitCommands
    {
        void Clone(string fromUrl, string toPath);
    }
}