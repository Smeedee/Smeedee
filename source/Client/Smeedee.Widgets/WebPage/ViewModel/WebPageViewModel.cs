using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace Smeedee.Widgets.WebPage.ViewModel
{
    public partial class WebPageViewModel
    {
    	private Timer timer;

    	partial void OnInitialize()
        {
            InputUrl = "Enter URL here";
            ValidatedUrl = string.Empty;
            RefreshInterval = 30;

            PropertyChanged += WebPageViewModel_PropertyChanged;
        }

    	public int RefreshIntervalInSeconds
    	{
    		get
    		{
    			return RefreshInterval * 1000;		
    		}
    	}

        void WebPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputUrl")
            {
                ErrorMessage = IsValidInputUrl() ? "" : "Invalid URL!";
                ValidatedUrl = IsValidInputUrl() ? InputUrl : string.Empty;

				Save.TriggerCanExecuteChanged();
            }
        }

		public void OnRefresh()
		{
			if (IsValidInputUrl())
				TriggerPropertyChanged("ValidatedUrl");
		}

        public bool CanGoTo()
        {
            return !string.IsNullOrEmpty(InputUrl) && IsValidInputUrl();
        }

		public bool CanSave()
		{
			return !string.IsNullOrEmpty(InputUrl) && IsValidInputUrl();
		}

        private bool IsValidInputUrl()
        {
            return Regex.IsMatch(InputUrl, "^https?://[a-zA-Z1-9]");
        }
    }
}
