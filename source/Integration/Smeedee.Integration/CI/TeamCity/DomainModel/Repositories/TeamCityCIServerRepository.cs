using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Integration.CI.TeamCity.DomainModel.Repositories
{
    public class TeamCityCIServerRepository : IRepository<CIServer>
    {
        private TeamCityXmlParser _parser;

        public TeamCityCIServerRepository(TeamCityXmlParser parser)
        {
            _parser = parser;
        }
        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            var servers = _parser.GetTeamCityProjects();
            foreach (var server in servers)
            {
                var projects = _parser.GetTeamCityBuildConfigurations(server.Url);
                foreach (var project in projects)
                {
                    var builds = _parser.GetTeamCityBuilds(project.SystemId);
                    foreach (var build in builds)
                    {
                        project.AddBuild(build);
                    }
                    server.AddProject(project);
                }
            }

            return servers;
        }
    }
}
