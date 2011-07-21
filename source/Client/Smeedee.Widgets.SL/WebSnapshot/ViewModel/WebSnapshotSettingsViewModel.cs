using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Smeedee.Widgets.WebSnapshot.Util;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel
    {
        public DelegateCommand FetchMethod { get; set; }

        partial void OnInitialize()
        {
            InputUrl = "Enter image or page URL here";
            Xpath = "Get image URL from Xpath expression";
            RefreshInterval = 15;
            ValidatedUrl = string.Empty;

            FetchAsImage = new DelegateCommand { CanExecuteDelegate = ShouldFetchAsImage };
            FetchAsSnapshot = new DelegateCommand { CanExecuteDelegate = ShouldFetchAsSnapshot };

            PropertyChanged += WebSnapshotViewModel_PropertyChanged;
        }

        public int RefreshIntervalInSeconds
        {
            get { return RefreshInterval*1000*60; }
        }

        void WebSnapshotViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputUrl")
            {
                ErrorMessage = IsValidInputUrl() ? "" : "Invalid URL!";
                ValidatedUrl = IsValidInputUrl() ? InputUrl : string.Empty;

                SetFetchMethod();
                Save.TriggerCanExecuteChanged();
            }

        }

        private void SetFetchMethod()
        {
            if ( IsValidInputUrl() && IsPictureUrl())
            {
                FetchMethod = FetchAsImage;
            } 
            else if ( IsValidInputUrl() && !IsPictureUrl())
            {
                FetchMethod = FetchAsSnapshot;
            }
        }
        

        private bool ShouldFetchAsImage()
        {
            return IsValidInputUrl() && IsPictureUrl();
        }

        private bool ShouldFetchAsSnapshot()
        {
            return IsValidInputUrl() && !IsPictureUrl();
        }

        public bool CanSave()
        {
            return !string.IsNullOrEmpty(InputUrl) && IsValidInputUrl();
        }

        private bool IsValidInputUrl()
        {
            return URLValidator.IsValidUrl(InputUrl);
        }

        public bool IsPictureUrl()
        {
            return URLValidator.IsPictureURL(InputUrl);
        }

        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged("IsSaving");
                }
            }
        }
        private bool isSaving;
    }
}
