using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.ProjectInfo;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class ProjectInfoServerDatabaseRepository : GenericDatabaseRepository<ProjectInfoServer>
    {
        public ProjectInfoServerDatabaseRepository()
        {
            
        }

        public ProjectInfoServerDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
