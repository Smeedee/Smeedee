using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Smeedee.Widgets.SL.WebSnapshot.Util
{
    public static class CropUtil
    {
        public static Point GetUpperLeftCornerInRectangle(Point press, Point release)
        {
            if (press.X < release.X && press.Y < release.Y)
                return press;
            
            if (release.X < press.X && release.Y < press.Y)
                return release;
            
            if (press.X > release.X && press.Y < release.Y)
                return new Point(release.X, press.Y);
            
            return new Point(press.X, release.Y);
        }

        public static bool OutsidePicture(Point position, Image img)
        {
            return position.Y > img.ActualHeight || position.X > img.ActualWidth || position.X < 0 || position.Y < 0;
        }

        public static double NegativeNumber(double number)
        {
            return -Math.Abs(number);
        }

        public static WriteableBitmap Crop(this WriteableBitmap bmp, int x, int y, int width, int height)
        {
            var SizeOfArgb = 4;
            var srcWidth = bmp.PixelWidth;
            var srcHeight = bmp.PixelHeight;

            if (x > srcWidth || y > srcHeight)
            {
                return null;
            }

            if (x < 0) x = 0;
            if (x + width > srcWidth) width = srcWidth - x;
            if (y < 0) y = 0;
            if (y + height > srcHeight) height = srcHeight - y;

            var result = new WriteableBitmap(width, height);
            for (var line = 0; line < height; line++)
            {
                var srcOff = ((y + line) * srcWidth + x) * SizeOfArgb;
                var dstOff = line * width * SizeOfArgb;
                Buffer.BlockCopy(bmp.Pixels, srcOff, result.Pixels, dstOff, width * SizeOfArgb);
            }
            return result;
        }
    }
}
