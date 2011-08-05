using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Smeedee.Widgets.SL.WebSnapshot.Util
{
    public static class CropUtil
    {
        public static Point GetUpperLeftCornerInRectangle(Point press, Point release, Rectangle rectangle)
        {
            var upperLeft  = new Point(0,0);
            
            if(press.X.CompareTo(release.X) < 0)
            {
                if(press.Y.CompareTo(release.Y) < 0)
                {
                    upperLeft = press;
                }
                else
                {
                    upperLeft.X = press.X;
                    upperLeft.Y = press.Y - rectangle.Height;
                }
            }
            else
            {
                if(press.X.CompareTo(release.X) < 0)
                {
                    upperLeft.X = release.X - rectangle.Width;
                    upperLeft.Y = release.Y - rectangle.Height;
                }
                else
                {
                    if (press.Y.CompareTo(release.Y) < 0)
                    {
                        upperLeft.X = release.X;
                        upperLeft.Y = release.Y - rectangle.Height; 
                    }
                    else
                    {
                        upperLeft = release;
                    }
                    
                }
            }
            return upperLeft;
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

            // Clamp to boundaries
            if (x < 0) x = 0;
            if (x + width > srcWidth) width = srcWidth - x;
            if (y < 0) y = 0;
            if (y + height > srcHeight) height = srcHeight - y;

            // Copy the pixels line by line using fast BlockCopy
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
