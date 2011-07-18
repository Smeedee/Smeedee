using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class WebImageProvider : IWebImageProvider
    {
        public WriteableBitmap GetBitmapFromURL(string url)
        {
            if (!URLValidator.IsPictureURL(url))
            {
                return null;
            }

            WriteableBitmap picture = null;

            try
            {
                picture = new WriteableBitmap(new BitmapImage(new Uri(url, UriKind.Absolute)));
            }
            catch { }

            return picture;
        }

        //public string GetPictureNodeURLFromXpath(string pageURL, string xpath)
        //{
        //    HtmlWeb htmlWeb = new HtmlWeb();
        //    HtmlDocument document = htmlWeb.Load(pageURL);
        //    var xpathNode = document.DocumentNode.SelectSingleNode(xpath);

        //    var attributes = xpathNode.Attributes.ToList();
        //    var src = attributes.FindAll(a => a.Name == "src");
        //    return src.First().Value;
        //}
    }
}
