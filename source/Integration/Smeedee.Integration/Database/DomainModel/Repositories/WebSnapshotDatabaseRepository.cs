using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class WebSnapshotDatabaseRepository : GenericDatabaseRepository<WebSnapshot>
    {
        public WebSnapshotDatabaseRepository() : base() {}

        public WebSnapshotDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {}
    }
}
