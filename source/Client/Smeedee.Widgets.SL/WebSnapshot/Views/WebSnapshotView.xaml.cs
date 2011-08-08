using System.Windows.Controls;
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
            TryParseValues();

            Snapshot.Source = sourceImage.Source;
            var wb = Snapshot.Source as WriteableBitmap;

            if (wb == null) return;
            
            if (ShouldCrop())
            {
                Snapshot.Source = wb.Crop(x, y, (int)rect.Width, (int)rect.Height);
            }
            else
            {
                Snapshot.Source = wb;
            }
        }

        private bool ShouldCrop()
        {
            return 0 != x + y + (int)rect.Width + (int)rect.Height;
        }

        private int x;
        private int y;

        private void TryParseValues()
        {
            double rectHeight;
            double rectWidth;

            if (double.TryParse(Heightbox.Text, out rectHeight) 
                && double.TryParse(Heightbox.Text, out rectWidth) 
                && int.TryParse(Xbox.Text, out x) 
                && int.TryParse(Ybox.Text, out y))
            {
                rect.Height = rectHeight;
                rect.Width = rectWidth;
            }
            else
            {
                rect.Height = 0;
                rect.Width = 0;
                x = 0;
                y = 0;
            }
        }
    }
}
