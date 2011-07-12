using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public static class URLValidator
    {

        public static bool IsValidUrl(string url)
        {
            return Regex.IsMatch(url, "^https?://[a-zA-Z1-9]");
        }

        public static bool IsPictureURL(string url)
        {
            var fileExtension = Path.GetExtension(url).ToLower();
            switch (fileExtension)
            {
                case ".png":
                case ".gif":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".tiff":
                    return true;
            }
            return false;            
        }
    }
}
