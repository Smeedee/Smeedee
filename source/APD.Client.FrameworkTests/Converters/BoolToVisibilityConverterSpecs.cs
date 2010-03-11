using System;
using APD.Client.Framework.Converters;
using NUnit.Framework;
using System.Windows;



namespace APD.Client.Framework.Tests.Converters.BoolToVisibilityConverterSpecs
{
    [TestFixture]
    public class BoolToVisibilityConverterSpecs
    {
        [Test]
        public void Assure_collapsed_returns_false()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsFalse((bool)bc.ConvertBack(Visibility.Collapsed, true.GetType(), null, null));
        }

        [Test]
        public void Assure_false_returns_colapsed()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue(Visibility.Collapsed ==
                          (Visibility)bc.Convert(false, typeof(Visibility), null, null));
        }

        [Test]
        public void Assure_true_returns_visible()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue(Visibility.Visible == (Visibility)bc.Convert(true, typeof(Visibility), null, null));
        }

        [Test]
        public void Assure_visible_returns_true()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue((bool)bc.ConvertBack(Visibility.Visible, true.GetType(), null, null));
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_bool_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert("foo", typeof(Visibility), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_visibility_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert(true, typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_both_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert("lol", typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_visibility_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack("lol", typeof(bool), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_bool_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack(Visibility.Visible, typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_both_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack("lol", typeof(string), null, null);
        }
    }
}
