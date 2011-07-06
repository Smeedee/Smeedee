using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotViewModel
    {
        public DelegateCommand FetchImage { get; set; }

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

        public bool IsPictureUrl()
        {
            var fileExtension = Path.GetExtension(InputUrl).ToLower();
            switch (fileExtension)
            {
                case ".png"  :
                case ".gif"  :
                case ".jpg"  :
                case ".jpeg" :
                case ".bmp"  :
                case ".tiff" :
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
