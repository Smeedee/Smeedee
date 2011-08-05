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
        private static int VIEW_WIDTH = 600;
        private static int VIEW_HEIGHT = 350;

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

            if (wb == null)
                return;

            if (ShouldCrop())
            {
                var scaling = ResolveScaling(wb.PixelWidth, wb.PixelHeight);
                int paddingWidth = 0;
                int paddingHeight = 0;
                int x = 0;
                int y = 0;

                if (scaling == 1.0)
                {
                    paddingWidth = ResolvePaddingWidth(wb.PixelWidth);
                    paddingHeight = ResolvePaddingHeight(wb.PixelHeight);
                    x = int.Parse(Xbox.Text) - paddingWidth;
                    y = int.Parse(Ybox.Text) - paddingHeight;
                }
                else
                {
                    paddingWidth = (int) Math.Round((((scaling*VIEW_HEIGHT) - VIEW_HEIGHT)/2)*scaling);
                    paddingHeight = (int) Math.Round((((scaling*VIEW_WIDTH) - VIEW_WIDTH)/2)*scaling);

                    x = (int)Math.Round(int.Parse(Xbox.Text) * scaling) - paddingWidth;
                    y = (int)Math.Round(int.Parse(Ybox.Text) * scaling) - paddingHeight;
                }
                
                int rectangleWidth = (int) (Math.Round(rect.Width)*scaling);
                int rectangleHeight = (int) (Math.Round(rect.Height)*scaling);

                Snapshot.Source = wb.Crop(x, y, rectangleWidth, rectangleHeight);
                
            } else
            {
                Snapshot.Source = wb;
            }
        }

        private double ResolveScaling(int actualWidth, int actualHeight)
        {
            double scaling = 1.0;

            var heightOverflow = actualHeight - VIEW_HEIGHT;
            var widthOverflow = actualWidth - VIEW_WIDTH;

            if (heightOverflow + widthOverflow == 0)
                return scaling;
            
            if (heightOverflow > 0 && widthOverflow <= 0)
            {
                scaling = (double)actualHeight/VIEW_HEIGHT;
            } 
            else if (widthOverflow > 0 && heightOverflow <= 0)
            {
                scaling = (double)actualWidth/VIEW_WIDTH;
            }

            return scaling;
        }

        private int ResolvePaddingHeight(int actualHeight)
        {
            var padding = (int)Math.Round((double) ((VIEW_HEIGHT - actualHeight)/2));
            return padding > 0 ? padding : 0;
        }

        private int ResolvePaddingWidth(int actualWidth)
        {
            var padding = (int)Math.Round((double) ((VIEW_WIDTH - actualWidth)/2));
            return padding > 0 ? padding : 0;
        }   

        private bool ShouldCrop()
        {
            return 0 != int.Parse(Xbox.Text) + int.Parse(Ybox.Text) + (int) rect.Width + (int) rect.Height;
        }
    }
}
