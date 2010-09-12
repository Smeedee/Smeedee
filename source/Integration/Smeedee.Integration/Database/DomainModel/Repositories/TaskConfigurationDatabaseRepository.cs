using System;
using System.Collections.Generic;
using NHibernate;
using Smeedee.DomainModel.TaskInstanceConfiguration;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class TaskConfigurationDatabaseRepository : GenericDatabaseRepository<TaskConfiguration>
    {
        public TaskConfigurationDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public TaskConfigurationDatabaseRepository()
        {
        }

        public override void Save(IEnumerable<TaskConfiguration> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE TaskConfiguration t").ExecuteUpdate();
                    session.CreateQuery("DELETE TaskConfigurationEntry e").ExecuteUpdate();
                    
                    foreach (var config in domainModels)
                    {
                        session.SaveOrUpdate(config);
                    }

                    session.Transaction.Commit();
                }
            }
        }
    }
}