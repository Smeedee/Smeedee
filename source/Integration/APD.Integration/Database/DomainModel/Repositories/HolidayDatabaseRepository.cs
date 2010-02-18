using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Holidays;

using NHibernate;


namespace APD.Integration.Database.DomainModel.Repositories
{
    public class HolidayDatabaseRepository
        : GenericDatabaseRepository<Holiday>
    {
        public HolidayDatabaseRepository()
            : base()
        {
            
        }

        public HolidayDatabaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
