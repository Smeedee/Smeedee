using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel
    {
        //public DelegateCommand FetchAsImage { get; set; }
        //public DelegateCommand FetchAsSnapshot { get; set; }

        partial void OnInitialize()
        {
            InputUrl = "Enter URL here";
            ValidatedUrl = string.Empty;

            FetchAsImage = new DelegateCommand { CanExecuteDelegate = ShouldFetchAsImage };
            FetchAsSnapshot = new DelegateCommand { CanExecuteDelegate = ShouldFetchAsSnapshot };

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
            //else if (e.PropertyName == "FetchImage")
            //{
            //    if (IsValidInputUrl() && )
            //}
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
            return Regex.IsMatch(InputUrl, "^https?://[a-zA-Z1-9]");
        }

        public bool IsPictureUrl()
        {
            var fileExtension = Path.GetExtension(InputUrl).ToLower();
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
