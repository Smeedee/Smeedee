using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class WebImageProvider : IWebImageProvider
    {
        public Bitmap GetBitmapFromURL(string url)
        {
            if (!URLValidator.IsPictureURL(url))
            {
                return new WebSnapshotter().GetSnapshot(url);
            }

            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();

                return new Bitmap(stream);
            }
            catch
            {
                return null;
            }
        }

        public string GetPictureNodeURLFromXpath(string pageURL, string xpath)
        {
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument document = htmlWeb.Load(pageURL);
                var xpathNode = document.DocumentNode.SelectSingleNode(xpath);
                var attributes = xpathNode.Attributes.ToList();
                var src = attributes.FindAll(a => a.Name == "src");

                return src.First().Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
