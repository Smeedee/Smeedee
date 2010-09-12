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
                    var deleteUsersQuery = string.Format("DELETE User u where Userdb_fid='{0}'", domainModel.Name);

                    session.CreateQuery(deleteUsersQuery).ExecuteUpdate();
                    session.SaveOrUpdate(domainModel);
                    session.Transaction.Commit();
                    session.Flush();
                }
            }
        }
        
        public override void Save(IEnumerable<Userdb> domainModels)
        {
            foreach (var domainModel in domainModels)
            {
                Save(domainModel);
            }
        }
    }
}
