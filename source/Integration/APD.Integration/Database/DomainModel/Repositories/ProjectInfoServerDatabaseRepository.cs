using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.ProjectInfo;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
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
