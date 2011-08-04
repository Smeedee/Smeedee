using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class CIServerDatabaseRepository : GenericDatabaseRepository<CIServer>, IDeleteDomainModels<CIServer>
    {
        public CIServerDatabaseRepository()
        {
            
        }

        public CIServerDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }

        public override IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            // this is a sort of hack that prevents the database from pushing out all builds,
            // when only latest build for each project is needed.
            var servers = base.Get(specification).ToList();

            foreach (var server in servers)
            {
                foreach (var project in server.Projects)
                {
                    var latestBuild = project.LatestBuild;
                    project.Builds = latestBuild != null ? new List<Build> {latestBuild} : new List<Build>();
                }
            }
            return servers;
        }

        #region IDeleteDomainModels<CIServer> Members

        public void Delete(Specification<CIServer> specification)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
