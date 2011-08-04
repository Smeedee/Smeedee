using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smeedee.Client.SL;
using Smeedee.Widgets.SL.WebSnapshot.Util;


namespace Smeedee.Widgets.SL.WebSnapshot.Views
{
    public partial class WebSnapshotSettingsView : UserControl
    {

        private Point origPoint;
        private Rectangle rect;
        private Point MousePress;
        private Point MouseRelease;
        private Queue<Point> previousPoints;
        private Stack<Rectangle> previousRect;
        private bool selectionDisabled;

        public WebSnapshotSettingsView()
        {
            InitializeComponent();

            previousPoints = new Queue<Point>();
            previousRect = new Stack<Rectangle>();
            LayoutRoot.MouseLeftButtonDown += canvas_MouseLeftButtonDown;
            
        }

        void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectionDisabled)
                return;

            Point point = e.GetPosition(image);
            
            if (CropUtil.OutsidePicture(point, image)) return;

            MousePress.X = point.X;
            MousePress.Y = point.Y;

            Xbox.Text = MousePress.X.ToString();
            Ybox.Text = MousePress.Y.ToString();

            rect = new Rectangle();
            origPoint = e.GetPosition(canvas);
            Canvas.SetLeft(rect, origPoint.X);
            Canvas.SetTop(rect, origPoint.Y);
            rect.Stroke = new SolidColorBrush(Colors.Red);
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

                if (CropUtil.OutsidePicture(curPoint, image)) return;

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

            if (CropUtil.OutsidePicture(point, image)) return;

            MouseRelease.X = point.X;
            MouseRelease.Y = point.Y;

            Heightbox.Text = rect.Height.ToString();
            Widthbox.Text = rect.Width.ToString();

            if (rect != null)
            {
                canvas.MouseMove -= canvas_MouseMove;
                canvas.MouseLeftButtonUp -= canvas_MouseLeftButtonUp;
            }
        }

        private void crop_click(object sender, RoutedEventArgs e)
        {
            Point upperleftpoint = CropUtil.GetUpperLeftCornerInRectangle(MousePress, MouseRelease, rect);

            previousPoints.Enqueue(upperleftpoint);
            previousRect.Push(rect);

            var img = CropPicture(upperleftpoint, rect, image);

            if (img == null)
            {
                ResetImage();
                ResetCoordinateBoxes();
                return;
            }

            SetImage(img);

            CropButton.IsEnabled = false;
            selectionDisabled = true;
            rect = null;
        }

        private void SetImage(WriteableBitmap img)
        {
            ResetImage();
            image.Source = img;
            img.Invalidate();
        }

        public WriteableBitmap CropPicture(Point upperleftpoint, Rectangle rectangle, Image img)
        {
            WriteableBitmap wb = null;
            try
            {
                wb = new WriteableBitmap(Convert.ToInt32(rectangle.Width), Convert.ToInt32(rectangle.Height));
                var t = new TranslateTransform
                {
                    X = CropUtil.NegativeNumber(upperleftpoint.X),
                    Y = CropUtil.NegativeNumber(upperleftpoint.Y)
                };

                //Draw to Writable Bitmap
                wb.Render(img, t);
                wb.Invalidate();

                //image.Source = wb;
            }
            catch (Exception)
            {
                // TODO insert log entry?
            }

            return wb;
        }

        private void ResetImage()
        {
            image.Clip = null;
            canvas.Children.Clear();
            var WebSnapshotURI = new Uri(App.Current.Host.Source, "../" + TaskNames.SelectedItem);
            image.Source = new BitmapImage(WebSnapshotURI);
            canvas.Children.Add(image);
            rect = null;
        }

        private void ResetCoordinateBoxes()
        {
            Xbox.Text = "0";
            Ybox.Text = "0";
            Heightbox.Text = string.Empty;
            Widthbox.Text = string.Empty;
        }


        private void reset_click(object sender, RoutedEventArgs e)
        {
            ResetImage(); 
            ResetCoordinateBoxes();
            CropButton.IsEnabled = true;
            selectionDisabled = false;
        }
    }
}
