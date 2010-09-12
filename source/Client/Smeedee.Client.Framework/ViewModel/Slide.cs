using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Slide
    {
        public const int DEFAULT_SECONDSONSCREEN = 15;


        partial void OnInitialize()
        {
            SecondsOnScreen = DEFAULT_SECONDSONSCREEN;
        }


        public WriteableBitmap Thumbnail
        {
            get
            {
				if (Widget != null && Widget.View != null  )
				{
#if SILVERLIGHT
                    
				    if (WidgetIsDisplayed())
				    {
				        WriteableBitmap bitmap = new WriteableBitmap((int)Widget.View.ActualWidth, (int)Widget.View.ActualHeight);
				        bitmap.Render(Widget.View, Widget.View.RenderTransform);
				        bitmap.Invalidate();
				        return bitmap;
				    }
                    else
                    { 
				        var bitmap = new WriteableBitmap(800, 600);
                        return bitmap;
				    }
                    
#else
                    return new WriteableBitmap(124, 124, 1, 1, PixelFormats.Bgr101010, new BitmapPalette(new List<Color>(){ Colors.Black }));
#endif
				}
				else
					return null;
            }
        }

        private bool WidgetIsDisplayed()
        {
            return Widget.View.ActualHeight != 0;
        }
    }
}
