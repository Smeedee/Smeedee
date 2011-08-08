using System;
using System.Windows.Controls;
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
        private int x;
        private int y;
        private float scaleFactor;
        private float scaleFactorInverse;

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

            if (wb == null)
                return;

            if (ShouldCrop())
            {
                ResolveScaleFactors(wb.PixelWidth, wb.PixelHeight);

                if (ShouldScale(scaleFactor))
                {
                    ScaleAndCrop(wb);
                }
                else
                {
                    OnlyCrop(wb);
                }
  
                int rectangleWidth = (int) (rect.Width*scaleFactor);
                int rectangleHeight = (int) (rect.Height*scaleFactor);

                Snapshot.Source = wb.Crop(x, y, rectangleWidth, rectangleHeight);
                
            } else
            {
                Snapshot.Source = wb;
            }
        }

        private void OnlyCrop(WriteableBitmap wb)
        {
            int paddingWidth = ResolvePaddingWidth(wb.PixelWidth);
            int paddingHeight = ResolvePaddingHeight(wb.PixelHeight);

            if (paddingWidth > 0)
            {
                x -= paddingWidth;
            }
            if (paddingHeight > 0)
            {
                y -= paddingHeight;
            }
        }

        private void ScaleAndCrop(WriteableBitmap wb)
        {
            float reducedPaddingWidth = (VIEW_WIDTH - (wb.PixelWidth*scaleFactorInverse))/2;
            float reducedPaddingHeight = (VIEW_HEIGHT - (wb.PixelHeight*scaleFactorInverse))/2;

            int paddingWidth = (int) (reducedPaddingWidth*scaleFactor);
            int paddingHeight = (int) (reducedPaddingHeight*scaleFactor);

            x = (int) (x*scaleFactor);
            y = (int) (y*scaleFactor);

            if (paddingWidth > 0)
            {
                x -= paddingWidth;
            }
            if (paddingHeight > 0)
            {
                y -= paddingHeight;
            }
        }

        private bool ShouldScale(float scaleFactor)
        {
            return scaleFactor != 1.0f;
        }

        private void ResolveScaleFactors(int actualWidth, int actualHeight)
        {
            scaleFactor = 1.0f;
            scaleFactorInverse = 1.0f;

            var heightOverflow = actualHeight - VIEW_HEIGHT;
            var widthOverflow = actualWidth - VIEW_WIDTH;

            if (heightOverflow == 0 && widthOverflow == 0)
                return;
            
            if (heightOverflow > 0 && widthOverflow <= 0 ||
                heightOverflow > widthOverflow)
            {
                scaleFactor = (float) actualHeight/VIEW_HEIGHT;
                scaleFactorInverse = (float) VIEW_HEIGHT/actualHeight;
            } 
            else if (widthOverflow > 0 && heightOverflow <= 0 ||
                widthOverflow > heightOverflow)
            {
                scaleFactor = (float) actualWidth/VIEW_WIDTH;
                scaleFactorInverse = (float) VIEW_WIDTH/actualWidth;
            }

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
            return 0 != x + y + (int)rect.Width + (int)rect.Height;
        }

        private void TryParseValues()
        {
            double rectHeight;
            double rectWidth;

            if (double.TryParse(Heightbox.Text, out rectHeight) 
                && double.TryParse(Widthbox.Text, out rectWidth) 
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
