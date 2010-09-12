using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Scheduler.Console
{
    internal class HardcodedConfigurationRepository : IRepository<Configuration>
    {
        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            var svn = new Configuration("vcs");
            svn.NewSetting("provider", "svn");
            svn.NewSetting("username", "guest");
            svn.NewSetting("password", "");
            svn.NewSetting("url", "http://smeedee.googlecode.com/svn/");
            svn.IsConfigured = true;

            var ci = new Configuration("ci");
            ci.NewSetting("provider", "teamcity");
            ci.NewSetting("username", "guest");
            ci.NewSetting("password", "");
            ci.NewSetting("url", "http://smeedee.com:8111");
            ci.IsConfigured = true;

            var pi = new Configuration("project-info");
            pi.NewSetting("use-config-repo", "true");
            pi.NewSetting("end-date", "2010-07-23");
            pi.IsConfigured = true;

            var pit = new Configuration("pi");
            pit.NewSetting("provider", "conchango-tfs");
            pit.NewSetting("username", "smeedee");
            pit.NewSetting("password", "dlog4321.");
            pit.NewSetting("project", "Smeedee");
            pit.NewSetting("url", "http://80.203.160.221:8080/tfs");

            var configs = new List<Configuration> { ci, svn, pi, pit }; 

            return from Configuration c in configs  where specification.IsSatisfiedBy(c) select c;

        }
    }
}
