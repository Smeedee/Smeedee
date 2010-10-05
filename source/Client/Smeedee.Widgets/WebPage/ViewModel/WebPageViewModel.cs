using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smeedee.Widgets.WebPage.ViewModel
{
    public partial class WebPageViewModel
    {
        partial void OnInitialize()
        {
            Url = string.Empty;
            RefreshInterval = 30;
        }

        public bool CanGoTo()
        {
            return !string.IsNullOrEmpty(Url) && IsValidUrl();
        }

        private bool IsValidUrl()
        {
            var protocol = "^(http|https)://";
            var domains = "([a-zA-Z1-9]*\\.{0,1}){1,}";
            var topLevelDomain = "\\.[a-zA-Z]{2,4}";
            var resource = "/{0,1}[a-zA-Z]*";

            return Regex.IsMatch(Url, protocol + domains + topLevelDomain + resource);
        }
    }
}
