using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.Converters;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Framework.Tests.Converters
{
    public class DateToStringConverterSpecs
    {
        public class Shared
        {
            protected DateToStringConverter converter;

            [SetUp]
            public void Setup()
            {
                converter = new DateToStringConverter();
            }

            protected object Convert(Object val)
            {
                return converter.Convert(val, typeof(string), null, null);
            }
        }

        [TestFixture]
        public class When_converting_a_non_DateTime_value : Shared
        {
            [Test]
            public void Assure_format_error_string_is_returned()
            {
                int val = 100;

                var result = Convert(val);

                result.ShouldBe("No end-date set or wrong format!");
            }
        }

        [TestFixture]
        public class When_converting_a_NullObj_value : Shared
        {
            [Test]
            public void Assure_empty_string_is_returned()
            {
                var result = Convert(null);

                result.ShouldBe("No end-date set or wrong format!");
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

            [Test]
            public void assure_will_format_the_date_properly()
            {
                var time = new DateTime(1984, 1, 2, 20, 15, 0);
                var converter = new DateToStringConverter();

                var result = converter.Convert(time, typeof(string), null, null) as string;
                result.ShouldBe("Since 02 jan 1984");
            }
        }


    }
}
