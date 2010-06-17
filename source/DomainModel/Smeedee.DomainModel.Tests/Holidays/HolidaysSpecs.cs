using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Holidays;
using Smeedee.Tests;

using NUnit.Framework;


namespace Smeedee.DomainModel.Tests
{
    [TestFixture]
    public class HolidaySpecs
    {
        [Test]
        public void Holiday_needs_a_date()
        {
            var holiday = new Holiday();
            PropertyTester.TestForExistence<Holiday>( h=> h.Date );
        }

        [Test]
        public void Holiday_needs_a_description()
        {
            var holiday = new Holiday();
            PropertyTester.TestForExistence<Holiday>( h=> h.Description );
        }

    }
}
