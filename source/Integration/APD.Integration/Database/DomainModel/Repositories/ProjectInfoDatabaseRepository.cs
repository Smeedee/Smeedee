using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.ProjectInfo;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
{
    public class ProjectInfoDatabaseRepository : GenericDatabaseRepository<Project>
    {
        public ProjectInfoDatabaseRepository()
        {
            
        }

        public ProjectInfoDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }
    }
}
