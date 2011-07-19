using System.Drawing;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public interface IWebImageProvider
    {
        Bitmap GetBitmapFromURL(string url);
        string GetPictureNodeURLFromXpath(string pageURL, string xpath);
    }
}
