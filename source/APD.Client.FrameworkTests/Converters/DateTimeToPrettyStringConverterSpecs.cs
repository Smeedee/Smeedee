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
using TinyBDD.Specification.NUnit;


namespace APD.Client.Framework.Tests.Converters.DateTimeToPrettyStringConverterSpecs
{
    public class Shared
    {
        protected DateTimeToPrettyStringConverter converter;

        [SetUp]
        public void Setup()
        {
            converter = new DateTimeToPrettyStringConverter();
        }

        protected object Convert(Object val)
        {
            return converter.Convert(val, typeof (string), null, null);
        }
    }

    [TestFixture]
    public class When_converting_a_non_DateTime_value : Shared
    {
        [Test]
        public void Assure_ToString_of_value_is_returned()
        {
            int val = 100;

            var result = Convert(val);

            result.ShouldBe("100");
        }
    }

    [TestFixture]
    public class When_converting_a_NullObj_value : Shared
    {
        [Test]
        public void Assure_empty_string_is_returned()
        {
            var result = Convert(null);

            result.ShouldBe(string.Empty);
        }
    }

    [TestFixture]
    public class When_converting_DateTime_value : Shared
    {
        [Test]
        public void assure_will_convert_DateTime_to_string()
        {
            var time = new DateTime(1984, 1, 1, 0, 0, 0);

            var result = Convert(time);
            result.ShouldBeInstanceOfType<string>();
        }

        //[Test]
        //public void assure_will_format_time_the_way_we_want()
        //{
        //    int ms = 948;
        //    var time = new DateTime(1984, 1, 2, 20, 15, 0, ms);
        //    var converter = new DateTimeToPrettyStringConverter();

        //    var result = converter.Convert(time, typeof(string), null, null) as string;
        //    result.ShouldBe("Jan 1. 1984, 20:15:00");
        //}

        [Test]
        public void assure_will_remove_milliseconds()
        {
            int ms = 948;
            var time = new DateTime(1984, 1, 1, 0, 0, 0, ms);

            var result = Convert(time) as string;
            result.Contains(ms.ToString()).ShouldBeFalse();
        }
    }
}
