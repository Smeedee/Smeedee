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
        private Rectangle rect;
        public WebSnapshotView()
        {
            InitializeComponent();
            rect = new Rectangle();
        }
        public void UpdateImage()
        {
            rect.Height = double.Parse(Heightbox.Text);
            rect.Width = double.Parse(Widthbox.Text);
            Snapshot.Source = sourceImage.Source;
            var wb = Snapshot.Source as WriteableBitmap;

            if (ShouldCrop())
            {
                Snapshot.Source = wb.Crop(int.Parse(Xbox.Text), int.Parse(Ybox.Text), (int)rect.Width, (int)rect.Height);
                
            } else
            {
                Snapshot.Source = wb;
            }
        }

        private bool ShouldCrop()
        {
            return 0 != int.Parse(Xbox.Text) + int.Parse(Ybox.Text) + (int) rect.Width + (int) rect.Height;
        }
    }
}
