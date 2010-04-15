using System.IO;
using System.Linq;

using GitSharp;


namespace APD.IntegrationTests.VCS.Git.Context
{
    public class GitPluginTestContext
    {
        protected string reposToBeCloned;
        protected string pathToLocalRepoClone;
        protected Repository repo;

        protected void SetupSharedContext()
        {
            reposToBeCloned = "git://github.com/flyrev/Smeedee_dummy.git";
            pathToLocalRepoClone = "C:\\Smeedee_test\\";
            repo = new Repository(pathToLocalRepoClone);
        }

        protected long GetLastRevision()
        {
            var latestChangeset = GetLatestChangeset();
            return latestChangeset.Ancestors.Count() + 1;
        }

        protected Commit GetLatestChangeset()
        {
            var latest = repo.Get<Commit>("HEAD");
            return latest;
        }

        protected string FilepathInLocalDirectory(string filename)
        {
            var path = Path.Combine(pathToLocalRepoClone, filename);
            return path;
        }

        protected void TheRepositoryExistsAndContainsChangesets()
        {
            if (!Directory.Exists(pathToLocalRepoClone))
                throw new DirectoryNotFoundException("The repository does not exist. Please clone it to " + pathToLocalRepoClone + " and try again.");
        }
   }
}
