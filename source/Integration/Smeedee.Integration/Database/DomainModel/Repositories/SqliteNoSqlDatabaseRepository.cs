using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Smeedee.DomainModel.NoSql;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class SqliteNoSqlDatabaseRepository : GenericDatabaseRepository<NoSqlDatabase>
    {
        public SqliteNoSqlDatabaseRepository()
            : base()
        {
            
        }

        public SqliteNoSqlDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
