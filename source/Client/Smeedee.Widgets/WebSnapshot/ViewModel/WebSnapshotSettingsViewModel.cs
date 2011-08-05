using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel
    {
        partial void OnInitialize()
        {
            AvailableImages = new ObservableCollection<string>();
            AvailableImagesUri = new ObservableCollection<string>();
        }

        public ObservableCollection<string> AvailableImagesUri;

        public string UriOfSelectedImage
        {
            get
            {
                var selected = SelectedImage;
                if (AvailableImagesUri.Count > 0 && selected != null)
                {
                    return AvailableImagesUri.ElementAt(AvailableImages.IndexOf(selected));
                }
                return string.Empty;
            }
            set {  }
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

        public virtual bool IsTimeToUpdate
        {
            get
            {
                OnGetIsTimeToUpdate(ref _IsTimeToUpdate);

                return _IsTimeToUpdate;
            }
            set
            {
                if (value != _IsTimeToUpdate)
                {
                    OnSetIsTimeToUpdate(ref value);
                    _IsTimeToUpdate = value;
                    TriggerPropertyChanged("IsTimeToUpdate");
                }
            }
        }
        private bool _IsTimeToUpdate;


        public string ScalingFactor
        {
            get { return scalingFactor; }
            set
            {
                if (value != scalingFactor)
                {
                    scalingFactor = value;
                    TriggerPropertyChanged("ScalingFactor");
                }
            }
        }
        private string scalingFactor;


        partial void OnGetIsTimeToUpdate(ref bool value);
        partial void OnSetIsTimeToUpdate(ref bool value);

    }
}
