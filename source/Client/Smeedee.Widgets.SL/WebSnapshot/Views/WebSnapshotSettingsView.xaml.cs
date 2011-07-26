using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smeedee.Widgets.SL.WebSnapshot.Util;


namespace Smeedee.Widgets.SL.WebSnapshot.Views
{
    public partial class WebSnapshotSettingsView : UserControl
    {
        public WebSnapshotSettingsView()
        {
            InitializeComponent();
            canvas.MouseLeftButtonDown += canvas_MouseLeftButtonDown;
        }

        private Point MousePress;
        private Point MouseRelease;
        private  Point origPoint;
        private Rectangle rect;

        void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point point = e.GetPosition(image);

            MousePress.X = point.X;
            MousePress.Y = point.Y;

            if (CropUtil.OutsidePicture(point, image)) return;
            
            rect = new Rectangle();
            origPoint = e.GetPosition(canvas);
            Canvas.SetLeft(rect, origPoint.X);
            Canvas.SetTop(rect, origPoint.Y);
            rect.Stroke = new SolidColorBrush(Colors.Cyan);
            rect.StrokeThickness = 2;

            canvas.Children.Add(rect);

            canvas.MouseMove += canvas_MouseMove;
            canvas.MouseLeftButtonUp += canvas_MouseLeftButtonUp;
        }

        void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (rect != null)
            {
                Point curPoint = e.GetPosition(canvas);
                if (curPoint.X > origPoint.X)
                {
                    rect.Width = curPoint.X - origPoint.X;
                }
                else if (curPoint.X < origPoint.X)
                {
                    Canvas.SetLeft(rect, curPoint.X);
                    rect.Width = origPoint.X - curPoint.X;
                }

                if (curPoint.Y > origPoint.Y)
                {
                    rect.Height = curPoint.Y - origPoint.Y;
                }
                else if (curPoint.Y < origPoint.Y)
                {
                    Canvas.SetTop(rect, curPoint.Y);
                    rect.Height = origPoint.Y - curPoint.Y;
                }
            }
        }

        void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(image);

            MouseRelease.X = point.X;
            MouseRelease.Y = point.Y;

            if (rect != null)
            {
                canvas.MouseMove -= canvas_MouseMove;
                canvas.MouseLeftButtonUp -= canvas_MouseLeftButtonUp;
            }
        }

        private void crop_click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap wb = new WriteableBitmap(Convert.ToInt32(rect.Width), Convert.ToInt32(rect.Height));

            Point upperleftpoint = CropUtil.GetUpperLeftCornerInRectangel(MousePress, MouseRelease, rect);

            TranslateTransform t = new TranslateTransform();
            t.X = NegativeNumber(upperleftpoint.X);
            t.Y = NegativeNumber(upperleftpoint.Y);

            //Draw to Writable Bitmap
            wb.Render(image, t);
            wb.Invalidate();

            canvas.Children.Clear();
            canvas.Children.Add(image);

            //Finally set the Image back on to the canvas
            image.Source = wb;
        }

        private double NegativeNumber(double number)
        {
            return -Math.Abs(number);
        }
    }
}
