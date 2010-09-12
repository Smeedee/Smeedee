using System;
using System.Globalization;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widget.Admin.Tasks.SL.Converters;
using Smeedee.Widget.Admin.Tasks.ViewModels;

namespace Smeedee.Widget.Admin.Tasks.SL.Tests
{
    public class TestBase
    {
    }

    public class ConverterTestBase : TestBase
    {
        protected ConfigEntryControlConverter Converter;

        protected object Convert(object value)
        {
            return Converter.Convert(value, typeof(DataTemplate), null, new CultureInfo("en-US"));
        }

        protected ConfigurationEntryViewModel CreateNewConfigEntry(string name, Type type, object value)
        {
            return new ConfigurationEntryViewModel
                       {
                           Name = name,
                           Type = type,
                           Value = value
                       };
        }

        protected void SetUp()
        {
            Converter = new ConfigEntryControlConverter(); 
        }
    }

    public static class TestExtensions
    {
        public static void ShouldBe(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }
    }
}