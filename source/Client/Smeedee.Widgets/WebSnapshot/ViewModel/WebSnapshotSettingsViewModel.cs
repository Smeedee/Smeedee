using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel
    {
        public DelegateCommand FetchImage { get; set; }
        public DelegateCommand FetchSnapshot { get; set; }

        partial void OnInitialize()
        {
            InputUrl = "Enter URL here";
            ValidatedUrl = string.Empty;

            PropertyChanged += WebSnapshotViewModel_PropertyChanged;
        }
    }
}
