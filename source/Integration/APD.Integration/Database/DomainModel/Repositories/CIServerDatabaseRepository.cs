using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
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
