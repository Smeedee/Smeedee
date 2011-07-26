using System.Windows;
using System.Windows.Shapes;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class CropUtil
    {
        public static Point GetUpperLeftCornerInRectangel(Point press, Point release, Rectangle rectangle)
        {
            var upperLeft  = new Point(200,200);
            
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
    }
}
