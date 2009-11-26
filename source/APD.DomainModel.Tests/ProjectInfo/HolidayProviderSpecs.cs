#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using APD.DomainModel.ProjectInfo;
using NUnit.Framework;


namespace APD.DomainModel.HolidayProviderSpecs
{
    [TestFixture]
    public class When_HolidayProvider_is_spawned
    {
        private DateTime workingDay;
        private HolidayProvider holidayProvider;

        [SetUp]
        public void SetUp()
        {
            workingDay = new DateTime(2009, 2, 10);
            holidayProvider = new HolidayProvider();
        }

        [Test]
        public void should_be_able_to_check_if_a_workinday_is_a_holiday()
        {
            Assert.IsInstanceOfType(typeof(bool), holidayProvider.IsHoliday(workingDay));
        }

        [Test]
        public void should_be_able_to_add_custom_holidays()
        {
            holidayProvider.MakeWorkingdayHoliday(workingDay);
            Assert.IsTrue(holidayProvider.IsHoliday(workingDay));
        }

        [Test]
        public void should_be_able_to_remove_holidays()
        {
            holidayProvider.MakeHolidayWorkingDay(workingDay);
            Assert.IsFalse(holidayProvider.IsHoliday(workingDay));
        }
    }
}