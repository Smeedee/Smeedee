using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotViewModel
    {
        partial void OnInitialize()
        {
            InputUrl = "Enter URL here";
            ValidatedUrl = string.Empty;

            PropertyChanged += WebSnapshotViewModel_PropertyChanged;
        }

        void WebSnapshotViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputUrl")
            {
                ErrorMessage = IsValidInputUrl() ? "" : "Invalid URL!";
                ValidatedUrl = IsValidInputUrl() ? InputUrl : string.Empty;

                Save.TriggerCanExecuteChanged();
            }
        }

        public bool CanSave()
        {
            return !string.IsNullOrEmpty(InputUrl) && IsValidInputUrl();
        }

        //public void OnRefresh()
        //{
        //    if (IsValidInputUrl())
        //    {
        //        TriggerPropertyChanged("ValidatedUrl");
        //    }
        //}

        private bool IsValidInputUrl()
        {
            return Regex.IsMatch(InputUrl, "^https?://[a-zA-Z1-9]");
        }

    }
}
