using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.Holidays;
using APD.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.Database.DomainModel.Repositories
{
    [TestFixture]
    public class HolidayDatabaseRepositorySpecs : Shared
    {

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void Assure_holidays_can_be_Saved()
        {
            var dbRepo = new HolidayDatabaseRepository(sessionFactory);
            var norwegianConstitutionDay = new Holiday() {Date = new DateTime(2010, 05, 17), Description = "17. Mai"};
            dbRepo.Save(norwegianConstitutionDay);

            RecreateSessionFactory();
            var newDbRepo = new HolidayDatabaseRepository(sessionFactory);
            var holidays = newDbRepo.Get(new AllSpecification<Holiday>());

            holidays.First().Date.ShouldBe(norwegianConstitutionDay.Date);
            holidays.First().Description.ShouldBe(norwegianConstitutionDay.Description);
        }

    }
}
