using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Smeedee.Widget.Admin.Tasks.SL.Tests.Converters
{
    [TestClass]
    public class When_passing_invalid_parameters_to_the_converter : ConverterTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
        }

        [TestMethod]
        public void Should_return_null_when_null_is_passed_in_as_the_value()
        {
            var result = Convert("A string shouldn't be passed in as the value");

            result.ShouldBeNull();
        }

        [TestMethod]
        public void Should_return_null_when_string_is_passed_in_as_the_value()
        {
            var result = Convert(null);

            result.ShouldBeNull();
        }
    }
}