using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
    public partial class TeamPictureViewModel
    {
        public void OnInitialize()
        {
            Snapshots = new ObservableCollection<WriteableBitmap>();

            PropertyChanged += TeamPictureViewModel_PropertyChanged;
        }

        void TeamPictureViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSnapshot")
            {
                Select.TriggerCanExecuteChanged();
                Delete.TriggerCanExecuteChanged();
            }
        }

        public void OnDelete()
        {
            Snapshots.Remove(SelectedSnapshot);
        }

        public bool CanDelete()
        {
            return SelectedSnapshot != null;
        }

        public void OnSelect()
        {
            Snapshot = SelectedSnapshot;
        }

        public bool CanSelect()
        {
            return SelectedSnapshot != null;
        }
    }
}
