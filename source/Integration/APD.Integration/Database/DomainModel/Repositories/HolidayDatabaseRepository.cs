using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework.Logging;
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

        public override IEnumerable<Holiday> Get(APD.DomainModel.Framework.Specification<Holiday> specification)
        {
            if( specification is HolidaySpecification )
            {
                var holidaysSpec = specification as HolidaySpecification;

                if (holidaysSpec.EndDate.Date == DateTime.MaxValue.Date && 
                    holidaysSpec.StartDate.Date == DateTime.MinValue.Date)
                    throw new ArgumentException("The given date range is not valid. Set the start and end date in the specification");

                var holidaysToReturn = base.Get(specification).ToList();

                DateTime iterator = holidaysSpec.StartDate;
                while( iterator <= holidaysSpec.EndDate )
                {
                    if (holidaysSpec.NonWorkingDaysOfWeek.Contains(iterator.DayOfWeek))
                        holidaysToReturn.Add(new Holiday()
                        {Date = iterator, Description = iterator.DayOfWeek.ToString()});

                    iterator = iterator.AddDays(1);
                }

                return holidaysToReturn;
            }
            else
            {
                return base.Get(specification);
            }
        }

        public override void Save(Holiday domainModel)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE Holiday h").ExecuteUpdate();
                    session.SaveOrUpdate(domainModel);
                    session.Transaction.Commit();
                }
            }
        }

        public override void Save(IEnumerable<Holiday> domainModels)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    session.CreateQuery("DELETE Holiday h").ExecuteUpdate();
                    foreach (Holiday holiday in domainModels)
                    {
                        session.SaveOrUpdate(holiday);    
                    }
                    
                    session.Transaction.Commit();
                }
            }
        }
    }
}
