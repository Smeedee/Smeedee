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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.CI.BuildInfoSpecs
{
    [TestFixture]
    public class when_spawned
    {
        private Build info;

        [SetUp]
        public void Setup()
        {
            info = new Build();
        }

        [Test]
        public void should_have_start_time()
        {
            info.StartTime.ShouldNotBeNull();
        }

        [Test]
        public void should_have_finished_time()
        {
            info.FinishedTime.ShouldNotBeNull();
        }

        [Test]
        public void should_have_duration()
        {
            info.Duration.ShouldNotBeNull();
        }

        //[Test]
        //public void should_have_build_result()
        //{
        //    info.Result.ShouldNotBeNull();
        //}

        [Test]
        public void should_have_build_status()
        {
            info.Status.ShouldNotBeNull();
        }

        [Test]
        public void should_have_unknown_build_trigger()
        {
            info.Trigger.ShouldBeInstanceOfType<UnknownTrigger>();
        }
    }


    [TestFixture]
    public class when_build_status_is_building
    {
        private Build info;

        [SetUp]
        public void Setup()
        {
            info = new Build()
                       {
                           Status = BuildStatus.Building,
                           StartTime = DateTime.Now.AddHours(-1)
                       };
        }

        [Test]
        public void duration_should_be_difference_between_now_and_start_time()
        {
            TimeSpan expectedDuration = DateTime.Now - info.StartTime;
            var acceptedErrorMargin = new TimeSpan(0, 0, 1);

            TimeSpan timeDifference = expectedDuration - info.Duration;

            Assert.IsTrue(timeDifference < acceptedErrorMargin);
        }
    }


    [TestFixture]
    public class when_build_status_is_finished_successfully
    {
        private Build info;

        [SetUp]
        public void Setup()
        {
            info = new Build()
                       {
                           StartTime = DateTime.Now.AddMinutes(-10),
                           FinishedTime = DateTime.Now,
                           Status = BuildStatus.FinishedSuccefully
                       };
        }

        [Test]
        public void duration_should_be_difference_between_finished_and_start_time()
        {
            TimeSpan shouldbe = info.FinishedTime - info.StartTime;
            Assert.AreEqual(shouldbe, info.Duration);
        }
    }


    [TestFixture]
    public class when_build_status_is_unknown
    {
        private Build info;

        [SetUp]
        public void Setup()
        {
            info = new Build()
                       {
                           StartTime = DateTime.Now.AddMinutes(-10),
                           FinishedTime = DateTime.Now,
                           Status = BuildStatus.Unknown
                       };
        }

        [Test]
        public void duration_should_be_nothing()
        {
            info.Duration.ShouldBe(new TimeSpan());
        }
    }
}