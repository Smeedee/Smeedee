using System;
using System.Drawing;

namespace Smeedee.Widgets.WebSnapshot.Util
{


    public class WebImageFetcher
    {
        private IWebImageProvider imageProvider;

        public WebImageFetcher(IWebImageProvider imageProvider)
        {
            this.imageProvider = imageProvider;
        }

        public Bitmap GetBitmapFromURL(string url)
        {
            return imageProvider.GetBitmapFromURL(url);
        }

        public Bitmap GetBitmapFromURL(string url, string xpath)
        {
            var imageUrl = FindImageURLInWebpage(url, xpath);

            if(imageUrl == null)
            {
                return null;
            }

            return imageProvider.GetBitmapFromURL(imageUrl);
        }

        private string FindImageURLInWebpage(string pageURL, string xpath)
        {
            if (URLValidator.IsPictureURL(pageURL))
            {
                return pageURL;
            }
            var pictureURL = imageProvider.GetPictureNodeURLFromXpath(pageURL, xpath);
            
            if(pictureURL == null)
            {
                return null;
            }

            if (!URLValidator.IsValidUrl(pictureURL))
            {
                pictureURL = AppendBaseURLWithPictureURL(pageURL, pictureURL);
            }

            pictureURL = RemoveTrailingSlash(pictureURL);
            return pictureURL;
        }

        private string AppendBaseURLWithPictureURL(string pageURL, string pictureURL)
        {
            if (!pageURL.EndsWith("/"))
            {
                pageURL += "/";
            }

            pageURL += pictureURL;
            return pageURL;
        }

        private string RemoveTrailingSlash(string pictureURL)
        {
            return pictureURL.TrimEnd(new char[] { '/' });
        }
    }
}
