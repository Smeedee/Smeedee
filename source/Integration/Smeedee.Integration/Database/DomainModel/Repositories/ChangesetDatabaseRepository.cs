using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
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

        public override void Save(Changeset domainModel)
        {
            if (domainModel.Server == null)
                domainModel.Server = ChangesetServer.Default;

            AssureServerIsPresentInDb(domainModel.Server);
            base.Save(domainModel);
        }

        private void AssureServerIsPresentInDb(ChangesetServer server)
        {
            using(var session = sessionFactory.OpenSession())
            {
                using(session.BeginTransaction())
                {
                    var hit = session.CreateQuery(string.Format("from ChangesetServer c WHERE c.Url ='{0}'", server.Url)).UniqueResult();
                    if (hit == null)
                    {
                        session.Save(server);
                        session.Transaction.Commit();
                        session.Flush();
                    }
                }
            }
        }

        public override void Save(IEnumerable<Changeset> domainModels)
        {
            AssureAllServersArePresentInDb(domainModels);

            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (var domainModel in domainModels)
                    {
                        if (domainModel.Server == null)
                            domainModel.Server = ChangesetServer.Default;

                        session.SaveOrUpdate(domainModel);
                    }
                    
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }

        private void AssureAllServersArePresentInDb(IEnumerable<Changeset> domainModels)
        {
            AssureServerIsPresentInDb(ChangesetServer.Default);
            var uniqeServers = domainModels.Where(cs => cs.Server != null).Select(cs => cs.Server).Distinct();
            foreach (var changesetServer in uniqeServers)
            {
                AssureServerIsPresentInDb(changesetServer);
            }
        }

        public override IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            var result = base.Get(specification);
            return result.OrderByDescending(c => c.Revision);
        }
    }
}
