using System.Collections.ObjectModel;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widgets.WebSnapshot.Util;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel : AbstractViewModel

    {

        partial void OnInitialize()
        {
            Image = "http://www.freeclipartpictures.com/clipart/thumbnails/food007.jpg";
            AvailableImages=new ObservableCollection<string>{"Image1", "Image2", "Image3"};
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
