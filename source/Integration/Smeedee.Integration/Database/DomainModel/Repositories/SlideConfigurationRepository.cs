using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Integration.Database.DomainModel.Repositories;
using NHibernate;
using Smeedee.DomainModel.Config.SlideConfig;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class SlideConfigurationRepository : GenericDatabaseRepository<SlideConfiguration>
    {
        public SlideConfigurationRepository(ISessionFactory sessionFactory)
            : base(sessionFactory )
        {}
        
        public override void Save(IEnumerable<SlideConfiguration> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    string deleteQuery = string.Format("DELETE SlideConfiguration s");
                    session.CreateQuery(deleteQuery).ExecuteUpdate();

                    foreach (var slideConfiguration in domainModels)
                    {
                        session.Save(slideConfiguration);
                    }

                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }
    }
}
