using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Smeedee.Widget.Admin.Tasks.SL.Tests.Converters
{
    [TestClass]
    public class When_passing_string_configurations_to_the_converter : ConverterTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
        }

        [TestMethod]
        public void Should_return_a_TextBox_with_empty_text_when_config_value_is_null()
        {
            var config = CreateNewConfigEntry("Username", typeof(string), null);

            var dataTemplate = Convert(config) as DataTemplate;
            var textBox = dataTemplate.LoadContent() as TextBox;

            textBox.Text.ShouldBe("");
        }

        [TestMethod]
        public void Should_return_a_TextBox_with_empty_text_when_config_value_is_empty()
        {
            var config = CreateNewConfigEntry("Username", typeof(string), "");

            var dataTemplate = Convert(config) as DataTemplate;
            var textBox = dataTemplate.LoadContent() as TextBox;

            textBox.Text.ShouldBe("");
        }

        //[TestMethod]
        //public void Should_return_a_TextBox_with_correct_text_when_config_value_contains_data()
        //{
        //    var config = CreateNewConfigEntry("Username", typeof(string), "Foobar");

        //    var dataTemplate = Convert(config) as DataTemplate;
        //    var textBox = dataTemplate.LoadContent() as TextBox;

        //    textBox.Text.ShouldBe("Foobar");
        //} 
    }
}