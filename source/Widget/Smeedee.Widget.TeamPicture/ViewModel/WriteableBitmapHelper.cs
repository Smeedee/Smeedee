using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
    public class WriteableBitmapHelper
    {
        public static byte[] ToByteArray(int[] data)
        {
            int len = data.Length << 2;
            byte[] result = new byte[len];
            Buffer.BlockCopy(data, 0, result, 0, len);
            return result;
        }

        public static WriteableBitmap FromByteArray(byte[] buffer, int pictureWidth, int pictureHeight)
        {
            var bmp = new WriteableBitmap(pictureWidth, pictureHeight);
            Buffer.BlockCopy(buffer, 0, bmp.Pixels, 0, buffer.Length);
            return bmp;
        }
    }
}
