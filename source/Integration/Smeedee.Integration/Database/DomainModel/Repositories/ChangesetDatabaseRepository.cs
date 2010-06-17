using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using NHibernate;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class ChangesetDatabaseRepository : GenericDatabaseRepository<Changeset>, IDeleteDomainModels<Changeset>
    {
        public ChangesetDatabaseRepository()
        {
            
        }

        public ChangesetDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }

        public override IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            var result = base.Get(specification);
            return result.OrderByDescending(c => c.Revision);
        }

        public void Delete(Specification<Changeset> specification)
        {
            if (specification is AllChangesetsSpecification)
            {
                using (var session = sessionFactory.OpenSession())
                {
                    session.CreateQuery("DELETE Changeset c").ExecuteUpdate();
                }
            }
        }
    }
}
