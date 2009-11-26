using System;
using System.Collections.Generic;
using System.Linq;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.CI.TeamCity.DomainModel.Factories;
using APD.Integration.Framework.Atom.DomainModel.Repositories;


namespace APD.Integration.CI.TeamCity.DomainModel.Repositories
{
    public class TeamCityServerRepository : IRepository<CIServer>
    {
        public TeamCityServerRepository(Uri address)
        {
            Address = address;
        }

        public Uri Address { get; set; }

        #region IRepository<CIServer> Members

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            var buildRepository = new AtomEntryRepository<Build>(Address)
            {
                EntryFactory = new TeamCityBuildAtomEntryFactory()
            };

            var builds = buildRepository.Get(new AllSpecification<Build>());
            var server = new CIServer("TeamCity Build Server", Address.OriginalString);

            var projects = from build in builds
                           group build by build.Project;

            foreach (var project in projects)
            {
                CIProject ciProject = project.Key;
                ciProject.Builds = project.ToList();
                server.AddProject(ciProject);
            }
            return new List<CIServer> {server};
        }

        #endregion
    }
}