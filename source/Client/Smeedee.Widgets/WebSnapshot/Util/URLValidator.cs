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
            if (Regex.IsMatch(url, "^https?://[a-zA-Z1-9]"))
                return true;
            if (Regex.IsMatch(url, "^file://[a-zA-Z1-9]"))
                return true;
            if (File.Exists(url))
                return true;
            
            return false;
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
