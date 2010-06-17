using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Web.Tests.HolidayRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.WebTests.Services.Integration.HolidayRepositoryServiceTests
{
    [TestFixture]
    public class HolidayRepositoryServiceTests
    {
        private HolidayRepositoryServiceClient client;

        [SetUp]
        public void Setup()
        {
            this.client = new HolidayRepositoryServiceClient();
        }

        [Test]
        public void Assure_webSerivce_can_save_and_retreive_holidays()
        {
            var guid = Guid.NewGuid();
            var holiday = new Holiday() { Date = new DateTime(2010, 10, 12), Description = guid.ToString() };

            client.Save(new Holiday[]{ holiday });
            var holidays = client.Get(new AllSpecification<Holiday>());
            holidays.Single(h => h.Description == guid.ToString()).ShouldNotBeNull();

        }

        [Test]
        public void Assure_service_can_handle_HolidaySpecifications()
        {
            var holidaysFromRepo = client.Get(new HolidaySpecification()
            {
                StartDate = new DateTime(2010, 3, 10),
                EndDate = new DateTime(2010, 3, 20),
                NonWorkingDaysOfWeek = new List<DayOfWeek>() { DayOfWeek.Saturday, DayOfWeek.Sunday }
            });

            holidaysFromRepo.Where(h => h.Date.DayOfWeek == DayOfWeek.Saturday).Count().ShouldBe(2);
            holidaysFromRepo.Where(h => h.Date.DayOfWeek == DayOfWeek.Sunday).Count().ShouldBe(1);
        }
    }
}
