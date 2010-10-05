using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Smeedee.Widgets.WebPage.ViewModel
{
    public partial class WebPageViewModel
    {
        partial void OnInitialize()
        {
            InputUrl = "Enter URL here";
            ValidatedUrl = string.Empty;
            RefreshInterval = 30;

            PropertyChanged += WebPageViewModel_PropertyChanged;
        }

        void WebPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputUrl")
            {
                ErrorMessage = IsValidInputUrl() ? "" : "Invalid URL!";
                ValidatedUrl = IsValidInputUrl() ? InputUrl : string.Empty;
            }
        }

        public bool CanGoTo()
        {
            return !string.IsNullOrEmpty(InputUrl) && IsValidInputUrl();
        }

        private bool IsValidInputUrl()
        {
            return Regex.IsMatch(InputUrl, "^https?://[a-zA-Z1-9]");
        }
    }
}
