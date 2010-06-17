using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using NHibernate;

namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class UserdbDatabaseRepository : GenericDatabaseRepository<Userdb>
    {

        public UserdbDatabaseRepository() : base()
        {
            
        }

        public UserdbDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }

        public override void Save(Userdb domainModel)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE Userdb u").ExecuteUpdate();
                    session.CreateQuery("DELETE User u").ExecuteUpdate();
                    session.SaveOrUpdate(domainModel);
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }
    }
}
