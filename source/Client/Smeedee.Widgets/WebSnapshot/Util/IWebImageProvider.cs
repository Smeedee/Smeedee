using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public interface IWebImageProvider
    {
        WriteableBitmap GetBitmapFromURL(string url);
    }
}
