using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public interface IWebImageFetcher
    {
        Bitmap GetBitmapFromURL(string url);
        Bitmap GetBitmapFromURL(string url, string xpath);
    }
}
