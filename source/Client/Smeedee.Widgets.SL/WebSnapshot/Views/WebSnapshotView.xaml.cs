using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smeedee.Widgets.SL.WebSnapshot.Util;


namespace Smeedee.Widgets.SL.WebSnapshot.Views
{
    public partial class WebSnapshotView : UserControl
    {
        public WebSnapshotView()
        {
            InitializeComponent();
        }
        public void CropImage()
        {

            var sv = new WebSnapshotSettingsView();

            var rect = new Rectangle();
            rect.Height = double.Parse(Heightbox.Text);
            rect.Width = double.Parse(Widthbox.Text);
            Snapshot.Source = new BitmapImage(new Uri(ImageBox.Text));
            Snapshot.Source = sv.CropPicture(new Point(double.Parse(Xbox.Text), double.Parse(Ybox.Text)), rect, Snapshot);
        }
    }
}
