using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widgets.SL.TeamPicture.ViewModel;

namespace Smeedee.Widget.TeamPicture.Tests
{
    [TestClass]
    public class WriteableBitmapHelperSpecs
    {
        [TestMethod]
        public void Should_be_equal_after_round_trip()
        {
            int[] data = new int[100*100];
            var rand = new Random();
            for (int i = 0; i < 100*100; i++)
            {
                data[i] = rand.Next(int.MinValue, int.MaxValue);
            }

            var byteArray = WriteableBitmapHelper.ToByteArray(data);

            var bitmap = WriteableBitmapHelper.FromByteArray(byteArray, 100,100);

            for (int i = 0; i < 100*100; i++)
            {
                Assert.AreEqual(data[i], bitmap.Pixels[i]);
            }

            
        }
    }
}
