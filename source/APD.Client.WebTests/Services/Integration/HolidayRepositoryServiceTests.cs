using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.WebTests.HolidayService;
using APD.DomainModel.Framework;
using APD.DomainModel.Holidays;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.Client.WebTests.Services.Integration.HolidayRepositoryServiceTests
{
    [TestFixture]
    public class HolidayRepositoryServiceTests
    {
        [Test]
        public void Assure_webSerivce_can_save_holidays()
        {
            var guid = Guid.NewGuid();
            var holiday = new Holiday() {Date = new DateTime(2010, 10, 12), Description = guid.ToString()};

            HolidayService.HolidayRepositoryServiceClient client = new HolidayRepositoryServiceClient();

            client.Save(new List<Holiday> { holiday });
            var holidays = client.Get(new AllSpecification<Holiday>());
            holidays.Single(h => h.Description == guid.ToString()).ShouldNotBeNull();

        }
    }
}
