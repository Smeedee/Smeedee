using System;
using System.Windows.Media.Imaging;

namespace Smeedee.Widgets.SL.TeamPicture.ViewModel
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