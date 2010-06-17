using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

using NHibernate;


namespace Smeedee.Integration.Database.DomainModel.Repositories
{
    public class CIServerDatabaseRepository : GenericDatabaseRepository<CIServer>, IDeleteDomainModels<CIServer>
    {
        public CIServerDatabaseRepository()
        {
            
        }

        public CIServerDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }

        #region IDeleteDomainModels<CIServer> Members

        public void Delete(Specification<CIServer> specification)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
