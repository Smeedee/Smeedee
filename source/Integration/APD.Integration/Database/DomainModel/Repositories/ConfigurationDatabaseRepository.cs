using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Config;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
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
