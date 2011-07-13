using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class WebImageFetcher : IWebImageFetcher
    {

        public Bitmap GetBitmapFromURL(string url)
        {
            if (!URLValidator.IsPictureURL(url))
            {
                return null;
            }
            
            Bitmap picture = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                picture = new Bitmap(stream);
            }
            catch {}

            return picture;
        }

        public Bitmap GetBitmapFromURL(string url, string xpath)
        {
            return GetBitmapFromURL(FindImageURLInWebpage(url, xpath));
        }

        private static string FindImageURLInWebpage(string pageURL, string xpath)
        {
            if (URLValidator.IsPictureURL(pageURL))
            {
                return pageURL;
            }

            var xpathNode = GetXpathNode(pageURL, xpath);
            var pictureURL = GetPictureURLFromNode(xpathNode);

            if (!URLValidator.IsValidUrl(pictureURL))
            {
                pictureURL = AppendBaseURLWithPictureURL(pageURL, pictureURL);
            }

            pictureURL = RemoveTrailingSlash(pictureURL);
            return pictureURL;

        }

        private static HtmlNode GetXpathNode(string pageURL, string xpath)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(pageURL);
            return document.DocumentNode.SelectSingleNode(xpath);
        }

        private static string GetPictureURLFromNode(HtmlNode xpathNode)
        {
            var attributes = xpathNode.Attributes.ToList();
            var src = attributes.FindAll(a => a.Name == "src");
            return src.First().Value;
        }

        private static string AppendBaseURLWithPictureURL(string pageURL, string pictureURL)
        {
            if (pageURL.Substring(pageURL.Length-1).Equals("/"))
            {
                pageURL = pageURL.Insert(pageURL.Length - 1, "/");
            }

            pictureURL = pageURL.Insert(pageURL.Length - 1, pictureURL);
            return pictureURL;
        }

        private static string RemoveTrailingSlash(string pictureURL)
        {
            return pictureURL.TrimEnd(new char[] {'/'});
        }
    }
}
