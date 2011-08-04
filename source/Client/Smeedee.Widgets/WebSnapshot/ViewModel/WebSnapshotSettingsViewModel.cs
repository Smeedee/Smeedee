using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel
    {
        partial void OnInitialize()
        {
            AvailableImages = new ObservableCollection<string>();
            //AvailableImages.Add(@"http://localhost:1155/Smeedee/WebSnapshots/github-httpsgithubcomSmeedeeSmeedeecommitssprint3.png");
            //AvailableImages.Add(@"http://localhost:1155/Smeedee/WebSnapshots/smeedeeorg.png");
            //AvailableImages.Add(@"http://localhost:1155/Smeedee/WebSnapshots/NewWebSnapshotTask-httpsmeedeeorg.png");

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

        public string CropCoordinateX
        {
            get { return cropCoordinateX; }
            set
            {
                if (value != cropCoordinateX)
                {
                    cropCoordinateX = value;
                    TriggerPropertyChanged("CropCoordinateX");
                }
            }
        }
        private string cropCoordinateX;

        public string CropCoordinateY
        {
            get { return cropCoordinateY; }
            set
            {
                if (value != cropCoordinateY)
                {
                    cropCoordinateY = value;
                    TriggerPropertyChanged("CropCoordinateY");
                }
            }
        }
        private string cropCoordinateY;
        public string CropRectangleHeight
        {
            get { return cropRectangleHeight; }
            set
            {
                if (value != cropRectangleHeight)
                {
                    cropRectangleHeight = value;
                    TriggerPropertyChanged("CropRectangleHeight");
                }
            }
        }
        private string cropRectangleHeight;
        public string CropRectangleWidth
        {
            get { return cropRectangleWidth; }
            set
            {
                if (value != cropRectangleWidth)
                {
                    cropRectangleWidth = value;
                    TriggerPropertyChanged("CropRectangleWidth");
                }
            }
        }
        private string cropRectangleWidth;

        public virtual bool IsTimeToCrop
        {
            get
            {
                OnGetIsTimeToCrop(ref _IsTimeToCrop);

                return _IsTimeToCrop;
            }
            set
            {
                if (value != _IsTimeToCrop)
                {
                    OnSetIsTimeToCrop(ref value);
                    _IsTimeToCrop = value;
                    TriggerPropertyChanged("IsTimeToCrop");
                }
            }
        }
        private bool _IsTimeToCrop;

        partial void OnGetIsTimeToCrop(ref bool value);
        partial void OnSetIsTimeToCrop(ref bool value);

    }
}
