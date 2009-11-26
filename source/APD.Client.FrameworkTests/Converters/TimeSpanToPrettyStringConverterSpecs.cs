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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using APD.Client.Framework.Converters;
using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Framework.Tests.Converters.TimeSpanToPrettyStringConverterSpecs
{
    public class Shared
    {
        /*
         * How it _should_ be according to the other tests in the solution 
         */
//        protected static TimeSpanToPrettyStringConverter converter;
//
//        protected Context the_converter_has_been_created =
//            () => { converter = new TimeSpanToPrettyStringConverter(); };

        protected string ConvertToString(object span)
        {
            var converter = new TimeSpanToPrettyStringConverter();
            return converter.Convert(span, typeof(string), null, null) as string;
        }
    }


    [TestFixture]
    public class assure_can_parse_strings : Shared
    {
        [Test]
        public void assure_will_convert_correctly_parsed_strings()
        {
            var result = ConvertToString("01:02:03");

            result.ShouldBe("1 hour and 2 minutes");
        }
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void assure_will_NOT_convert_incorrectly_parsed_strings()
        {
            var result = ConvertToString("eh.. About sixteen minutes ago.");
        }
    }

    [TestFixture] 
    public class assure_can_handle_seconds : Shared
    {
                [Test]
        public void assure_will_convert_zero_time()
        {
            var result = ConvertToString(new TimeSpan(0, 0, 0, 0));

            result.ShouldBe(string.Empty);
        }

        [Test]
        public void assure_will_convert_1_second()
        {
            var result = ConvertToString(new TimeSpan(0, 0, 0, 1));

            result.ShouldBe("1 second");
        }

        [Test]
        public void assure_will_convert_15_seconds()
        {
            var result = ConvertToString(new TimeSpan(0, 0, 0, 15));
            result.ShouldBe("15 seconds");
        }
    }

    [TestFixture]
    public class assure_can_handle_minutes : Shared
    {
                [Test]
        public void assure_will_convert_1_minute()
        {
            var result = ConvertToString(new TimeSpan(0, 0, 1, 0));

            result.ShouldBe("1 minute");
        }

        [Test]
        public void assure_will_convert_12_minutes()
        {
            var result = ConvertToString(new TimeSpan(0, 0, 12, 0));
            result.ShouldBe("12 minutes");
        }
    }


    [TestFixture]
    public class assure_can_handle_hours : Shared
    {
                [Test]
        public void assure_will_convert_1_hour()
        {
            var result = ConvertToString(new TimeSpan(0, 1, 0, 0));

            result.ShouldBe("1 hour");
        }

        [Test]
        public void assure_will_convert_7_hours()
        {
            var result = ConvertToString(new TimeSpan(0, 7, 0, 0));
            result.ShouldBe("7 hours");
        }
    }



    [TestFixture]
    public class assure_can_handle_compsite_strings : Shared
    {
        [Test]
        public void assure_will_drop_seconds_if_time_is_over_an_hour()
        {
            ConvertToString(new TimeSpan(0, 1, 0, 12)).ShouldBe("1 hour");
        }

        [Test]
        public void assure_will_format_1_hour_53_minutes()
        {
            ConvertToString(new TimeSpan(0, 1, 53,0)).
                ShouldBe("1 hour and 53 minutes");
        }

        [Test]
        public void assure_will_format_5_minutes_and_2_seconds()
        {
            ConvertToString(new TimeSpan(0, 0, 5, 2)).
                ShouldBe("5 minutes and 2 seconds");
        }

        [Test]
        public void assure_will_format_7_hours_12_minutes()
        {
            ConvertToString(new TimeSpan(0, 7, 12, 0)).
                ShouldBe("7 hours and 12 minutes");
        }

        [Test]
        public void assure_will_treat_days_as_24_hours()
        {
            ConvertToString(new TimeSpan(3, 0, 12, 0)).
                ShouldBe("72 hours and 12 minutes");
        }

        [Test]
        public void assure_can_hande_negative_intervals_too()
        {
            ConvertToString(new TimeSpan(0, -7, -3, 0)).
                ShouldBe("minus 7 hours and 3 minutes");
        }
    }
}


