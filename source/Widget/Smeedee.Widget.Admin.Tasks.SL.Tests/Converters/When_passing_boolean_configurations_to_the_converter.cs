using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Smeedee.Widget.Admin.Tasks.SL.Tests.Converters
{
    [TestClass]
    public class When_passing_boolean_configurations_to_the_converter : ConverterTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();   
        }

        [TestMethod]
        public void Should_return_an_unchecked_CheckBox_when_config_value_is_null()
        {
            var config = CreateNewConfigEntry("UseHttps", typeof(bool), null);

            var dataTemplate = Convert(config) as DataTemplate;
            var checkBox = dataTemplate.LoadContent() as CheckBox;

            checkBox.ShouldNotBeNull();
            checkBox.IsChecked.ShouldBe(false);
        }

        [TestMethod]
        public void Should_return_an_unchecked_CheckBox_when_config_value_is_false()
        {
            var config = CreateNewConfigEntry("UseHttps", typeof(bool), false);

            var dataTemplate = Convert(config) as DataTemplate;
            var checkBox = dataTemplate.LoadContent() as CheckBox;

            checkBox.ShouldNotBeNull();
            checkBox.IsChecked.ShouldBe(false);
        }

        //[TestMethod]
        //public void Should_return_a_checked_CheckBox_when_config_value_is_true()
        //{
        //    var config = CreateNewConfigEntry("UseHttps", typeof(bool), true);

        //    var dataTemplate = Convert(config) as DataTemplate;
        //    var checkBox = dataTemplate.LoadContent() as CheckBox;

        //    checkBox.ShouldNotBeNull();
        //    checkBox.IsChecked.ShouldBe(true);
        //}
    }
}