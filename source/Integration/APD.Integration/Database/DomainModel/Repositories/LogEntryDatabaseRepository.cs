using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework.Logging;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
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
