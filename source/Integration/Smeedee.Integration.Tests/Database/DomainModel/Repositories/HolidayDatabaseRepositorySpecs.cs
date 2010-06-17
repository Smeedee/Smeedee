using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using Smeedee.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories
{
    [TestFixture]
    public class HolidayDatabaseRepositorySpecs : Shared
    {
        private HolidayDatabaseRepository dbRepo;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            this.dbRepo = new HolidayDatabaseRepository(sessionFactory);
        }

        [Test]
        public void Assure_holidays_can_be_Saved()
        {
            var norwegianConstitutionDay = new Holiday() {Date = new DateTime(2010, 05, 17), Description = "17. Mai"};
            dbRepo.Save(norwegianConstitutionDay);

            RecreateSessionFactory();
            var newDbRepo = new HolidayDatabaseRepository(sessionFactory);
            var holidays = newDbRepo.Get(new AllSpecification<Holiday>());

            holidays.First().Date.ShouldBe(norwegianConstitutionDay.Date);
            holidays.First().Description.ShouldBe(norwegianConstitutionDay.Description);
        }

        [Test]
        public void Assure_repository_returns_given_weekdays_as_holidays()
        {
            var holidaysFromRepo = dbRepo.Get(new HolidaySpecification()
            {
                StartDate = new DateTime(2010, 3, 10), // a thursday
                EndDate = new DateTime(2010, 3, 20), // a saturday
                NonWorkingDaysOfWeek = new List<DayOfWeek>() {DayOfWeek.Saturday, DayOfWeek.Sunday}
            });

            holidaysFromRepo.Where(h => h.Date.DayOfWeek == DayOfWeek.Saturday).Count().ShouldBe(2);
            holidaysFromRepo.Where(h => h.Date.DayOfWeek == DayOfWeek.Sunday).Count().ShouldBe(1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Not_setting_startDate_and_endDate_throws_argument_exception()
        {
            var spec = new HolidaySpecification();
            dbRepo.Get(spec);
        }

    }
}
