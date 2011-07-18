using System;
using System.Linq;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace Smeedee.Widgets.WebSnapshot.Util
{


    public class WebImageFetcher
    {
        private IWebImageProvider imageProvider;

        public WebImageFetcher(IWebImageProvider imageProvider)
        {
            this.imageProvider = imageProvider;
        }

        public WriteableBitmap GetBitmapFromURL(string url)
        {
            return imageProvider.GetBitmapFromURL(url);
        }

        //public WriteableBitmap GetBitmapFromURL(string url, string xpath)
        //{
        //    return imageProvider.GetBitmapFromURL(FindImageURLInWebpage(url, xpath));
        //}

        //private string FindImageURLInWebpage(string pageURL, string xpath)
        //{
        //    if (URLValidator.IsPictureURL(pageURL))
        //    {
        //        return pageURL;
        //    }
        //    var pictureURL = imageProvider.GetPictureNodeURLFromXpath(pageURL, xpath);

        //    if (!URLValidator.IsValidUrl(pictureURL))
        //    {
        //        pictureURL = AppendBaseURLWithPictureURL(pageURL, pictureURL);
        //    }

        //    pictureURL = RemoveTrailingSlash(pictureURL);
        //    return pictureURL;
        //}

        //private string AppendBaseURLWithPictureURL(string pageURL, string pictureURL)
        //{
        //    if (!pageURL.EndsWith("/"))
        //    {
        //        pageURL += "/";
        //    }

        //    pageURL += pictureURL;
        //    return pageURL;
        //}

        //private string RemoveTrailingSlash(string pictureURL)
        //{
        //    return pictureURL.TrimEnd(new char[] {'/'});
        //}
    }
}
