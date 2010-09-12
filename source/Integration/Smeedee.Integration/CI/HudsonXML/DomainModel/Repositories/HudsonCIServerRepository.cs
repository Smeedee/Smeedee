using System;
using System.Collections.Generic;
using System.Net;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories
{
    public class HudsonCiServerRepository : IRepository<CIServer>
    {
        private readonly HudsonXmlParser parser;

        public HudsonCiServerRepository(HudsonXmlParser parser)
        {
            this.parser = parser;
        }

        public HudsonCiServerRepository(string url, NetworkCredential networkCredential)
        {
            parser = new HudsonXmlParser(new HudsonXmlFetcher(url, networkCredential));
        }

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            var hudsonServer = new CIServer("Hudson Server", parser.GetUrl());
            var projects = parser.GetProjects();
            foreach (var project in projects)
            {
                var ciproject = new CIProject(project.ProjectName);
                var builds = parser.GetBuilds(project.ProjectName);
                foreach (var build in builds)
                {
                    ciproject.AddBuild(build);
                }
                hudsonServer.AddProject(ciproject);
            }

            return new List<CIServer> { hudsonServer };
        }
    }
}
