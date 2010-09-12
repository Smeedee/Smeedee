using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Smeedee.DomainModel.TeamPicture;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class TeamPictureDatabaseRepository : GenericDatabaseRepository<TeamPicture>
    {
        public TeamPictureDatabaseRepository()
        {
            
        }

        public TeamPictureDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }


        public override void Save(TeamPicture domainModel)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE TeamPicture t").ExecuteUpdate();
                    session.SaveOrUpdate(domainModel);
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }

        public override void Save(IEnumerable<TeamPicture> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE TeamPicture t").ExecuteUpdate();
                    foreach (var domainModel in domainModels)
                    {
                        session.SaveOrUpdate(domainModel);
                    }
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }

    }
}
