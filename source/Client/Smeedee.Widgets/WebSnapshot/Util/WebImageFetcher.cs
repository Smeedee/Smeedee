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

        public string FindImageURLInWebpage(string pageURL, string xpath)
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
            pageURL = AddTrailingSlash(pageURL);
            pictureURL = RemoveLeadingSlash(pictureURL);
            
            return pageURL + pictureURL;
        }

        private string RemoveTrailingSlash(string url)
        {
            return url.TrimEnd(new char[] { '/' });
        }

        private string AddTrailingSlash(string url)
        {
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url;
        }

        private string RemoveLeadingSlash(string url)
        {
            if (url.StartsWith("/"))
            {
                url = url.Remove(0, 1);
            }
            return url;
        }


    }
}
