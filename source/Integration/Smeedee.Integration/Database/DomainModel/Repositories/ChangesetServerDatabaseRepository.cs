using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Smeedee.DomainModel.SourceControl;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class ChangesetServerDatabaseRepository : GenericDatabaseRepository<ChangesetServer>
    {
        public ChangesetServerDatabaseRepository()
        {
            
        }

        public ChangesetServerDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
