using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel

    {
        partial void OnInitialize()
        {
            SelectedImage = "http://www.newfreeware.com/img/scr/7-1013.jpg";
            AvailableImages = new ObservableCollection<string> { "http://www.freeclipartpictures.com/clipart/thumbnails/food007.jpg", "http://www.freeclipartpictures.com/clipart/thumbnails/food017.jpg", "http://www.freeclipartpictures.com/clipart/thumbnails/food027.jpg" };
            Image = new BitmapImage(new Uri(SelectedImage));
        }

        public bool CanSave()
        {
            return true;
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
