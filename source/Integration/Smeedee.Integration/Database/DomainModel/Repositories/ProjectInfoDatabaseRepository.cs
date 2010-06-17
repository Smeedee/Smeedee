using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.ProjectInfo;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
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
