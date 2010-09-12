using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Client.Framework.Converters;

namespace Smeedee.Client.Framework.SL.Tests.Converters
{
    [TestClass]
    public class InvertedBoolToVisibilityConverterSpecs
    {
        private InvertedBoolToVisibilityConverter _converter;

        [TestInitialize]
        public void Setup()
        {
            _converter = new InvertedBoolToVisibilityConverter();
        }
        [TestMethod]
        public void Assert_true_is_converted_to_Collapsed()
        {
            Assert.AreEqual(Visibility.Collapsed,
                            _converter.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void Assert_false_is_converted_to_Visible()
        {
            Assert.AreEqual(Visibility.Visible, _converter.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void Assert_Visible_is_converted_back_to_false()
        {
            Assert.IsFalse((bool)_converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void Assert_Collapsed_is_converted_back_to_true()
        {
            Assert.IsTrue(
                (bool)_converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.CurrentCulture));
        }
    }
}
