using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.Converters;

using NUnit.Framework;
using System.Threading;
using System.Windows;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Framework.Tests.Converters.BoolToVisibilityConverterSpecs
{
    [TestFixture]
    public class When_converting
    {
        private BoolToVisibilityConverter converter;
        private Visibility co;

        [SetUp]
        public void Setup()
        {
            converter = new BoolToVisibilityConverter();
        }

        [Test]
        public void Assure_False_is_converted_to_Collapsed()
        {
            Convert(false).ShouldBe(Visibility.Collapsed);
        }

        [Test]
        public void Assure_True_is_converted_To_Visible()
        {
            Convert(true).ShouldBe(Visibility.Visible);
        }

        [Test]
        public void Assure_crap_values_are_converted_to_Collapsed()
        {
            Convert("asdadsds").ShouldBe(Visibility.Collapsed);
            Convert(123124).ShouldBe(Visibility.Collapsed);
        }

        private Object Convert(object value)
        {
            return converter.Convert(value, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
