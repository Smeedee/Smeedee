using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Config;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class ConfigurationDatabaseRepository 
        : GenericDatabaseRepository<Configuration>
    {
        public ConfigurationDatabaseRepository()
            : base()
        {
            
        }

        public ConfigurationDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
