using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework.Logging;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class LogEntryDatabaseRepository
        : GenericDatabaseRepository<LogEntry>
    {
        public LogEntryDatabaseRepository()
        {
            
        }

        public LogEntryDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
