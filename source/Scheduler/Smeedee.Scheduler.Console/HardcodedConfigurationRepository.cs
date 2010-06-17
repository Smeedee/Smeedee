using System.Collections.Generic;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Scheduler.Console
{
    internal class HardcodedConfigurationRepository : IRepository<Configuration>
    {
        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            var configuration = new Configuration("vcs");
            configuration.NewSetting("provider", "svn");
            configuration.NewSetting("username", "guest");
            configuration.NewSetting("password", "");
            configuration.NewSetting("url", "http://smeedee.googlecode.com/svn");

            return new List<Configuration>(){ configuration };
        }
    }
}
