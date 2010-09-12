using System;
using System.Collections.Generic;
using NHibernate;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class RetrospectiveNoteDatabaseRepository : GenericDatabaseRepository<RetrospectiveNote>
    {

        public RetrospectiveNoteDatabaseRepository()
        {
        }

        public RetrospectiveNoteDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public override void Save(IEnumerable<RetrospectiveNote> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var id = new List<RetrospectiveNote>(domainModels)[0].Id;
                    var deleteNotesQuery = string.Format("DELETE FROM RetrospectiveNote WHERE Id = '{0}'", id);

                    session.CreateQuery(deleteNotesQuery).ExecuteUpdate();
                    foreach (var note in domainModels)
                    {
                        session.SaveOrUpdate(note);
                    }

                    session.Transaction.Commit();
                }
            }
        }
    }
}