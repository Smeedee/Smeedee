using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Holidays;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.Tests.Holidays.HolidaySpecificationSpecs
{
    [TestFixture]
    public class HolidaySpecificationSpecs
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Setting_startDate_later_than_endDate_throws_argumentException()
        {
            var spec = new HolidaySpecification();
            spec.EndDate = new DateTime(2010, 10,10);
            spec.StartDate = new DateTime(2010, 11, 11);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Setting_endDate_to_a_date_earlier_than_startDate_throws_argumentException()
        {
            var spec = new HolidaySpecification();
            spec.StartDate = new DateTime(2010, 11, 11);
            spec.EndDate = new DateTime(2010, 10, 10);
        }

        [Test]
        public void Assure_invalid_endDate_does_not_change_the_endDate()
        {
            var spec = new HolidaySpecification();
            try
            {
                spec.StartDate = new DateTime(2010, 11, 11);
                spec.EndDate = new DateTime(2010, 10, 10);
            }
            catch (ArgumentException expected) { }

            spec.EndDate.ShouldNotBe(new DateTime(2010,10,10));
        }

        [Test]
        public void Assure_invalid_startDate_does_not_change_the_startDate()
        {
            var spec = new HolidaySpecification();
            try
            {
                spec.EndDate = new DateTime(2010, 10, 10);
                spec.StartDate = new DateTime(2010, 11, 11);
            }
            catch (ArgumentException expected) { }

            spec.StartDate.ShouldNotBe(new DateTime(2010, 11, 11));
        }

        [Test]
        public void Assure_start_and_end_date_can_be_set()
        {
            var spec = new HolidaySpecification();
            spec.StartDate = new DateTime(2010,10,10);
            spec.EndDate = new DateTime(2010,11,11);

            spec.StartDate.ShouldBe(new DateTime(2010, 10, 10));
            spec.EndDate.ShouldBe(new DateTime(2010,11,11));
        }
    }
}
