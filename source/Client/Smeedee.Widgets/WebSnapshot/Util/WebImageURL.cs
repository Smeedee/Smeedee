using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public static class WebImageURL
    {

        public static Bitmap GetBitmapFromURL(string url)
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
            catch (Exception)
            {

            }

            return picture;
        }

        
    }
}
