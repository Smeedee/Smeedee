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
                float scaleFactor = ResolveScaleFactor(wb.PixelWidth, wb.PixelHeight);
                int paddingWidth = 0;
                int paddingHeight = 0;
                int x = 0;
                int y = 0;

                if (ShouldScale(scaleFactor))
                {
                    paddingWidth = (int)Math.Round((((scaleFactor * VIEW_HEIGHT) - VIEW_HEIGHT) / 2) * scaleFactor);
                    paddingHeight = (int)Math.Round((((scaleFactor * VIEW_WIDTH) - VIEW_WIDTH) / 2) * scaleFactor);

                    //x = (int)Math.Round(int.Parse(Xbox.Text) * scaleFactor) - paddingWidth;
                    //y = (int)Math.Round(int.Parse(Ybox.Text) * scaleFactor) - paddingHeight;

                    x = (int) Math.Round(int.Parse(Xbox.Text)*scaleFactor);
                    y = (int) Math.Round(int.Parse(Ybox.Text)*scaleFactor);
                    if (paddingHeight > 0) { y -= paddingHeight; }
                    if (paddingWidth > 0) { x -= paddingWidth; }

                }
                else
                {
                    paddingWidth = ResolvePaddingWidth(wb.PixelWidth);
                    paddingHeight = ResolvePaddingHeight(wb.PixelHeight);
                    x = int.Parse(Xbox.Text) - paddingWidth;
                    y = int.Parse(Ybox.Text) - paddingHeight;
                }
  
                int rectangleWidth = (int) (rect.Width*scaleFactor);
                int rectangleHeight = (int) (rect.Height*scaleFactor);

                Snapshot.Source = wb.Crop(x, y, rectangleWidth, rectangleHeight);
                
            } else
            {
                Snapshot.Source = wb;
            }
        }

        private bool ShouldScale(float scaleFactor)
        {
            return scaleFactor != 1.0f;
        }

        private float ResolveScaleFactor(int actualWidth, int actualHeight)
        {
            float scaleFactor = 1.0f;

            var heightOverflow = actualHeight - VIEW_HEIGHT;
            var widthOverflow = actualWidth - VIEW_WIDTH;

            if (heightOverflow == 0 && widthOverflow == 0)
                return scaleFactor;
            
            if (heightOverflow > 0 && widthOverflow <= 0 ||
                heightOverflow > widthOverflow)
            {
                scaleFactor = (float)actualHeight/VIEW_HEIGHT;
            } 
            else if (widthOverflow > 0 && heightOverflow <= 0 ||
                widthOverflow > heightOverflow)
            {
                scaleFactor = (float)actualWidth/VIEW_WIDTH;
            }

            return scaleFactor;
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
