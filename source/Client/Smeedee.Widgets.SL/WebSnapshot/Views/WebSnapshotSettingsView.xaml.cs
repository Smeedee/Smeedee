using System;
using System.Collections.Generic;
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


            previousPoints = new Queue<Point>();
            previousRect = new Stack<Rectangle>();
            LayoutRoot.MouseLeftButtonDown += canvas_MouseLeftButtonDown;

        }

        private Queue<Point> previousPoints;
        private Stack<Rectangle> previousRect;

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(image);

            MousePress.X = point.X;
            MousePress.Y = point.Y;

        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(image);

            MouseRelease.X = point.X;
            MouseRelease.Y = point.Y;

        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(image);
            if (OutsidePicture(position))
                Heightbox.Text = "outside picture";
            else
                Heightbox.Text = string.Format("{0} {1}", position.X, position.Y);
        }

        private bool OutsidePicture(Point position)
        {
            return position.Y > image.Height || position.X > image.Width || position.X < 0 || position.Y < 0;
        }

        Point origPoint;
        Rectangle rect;

        void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point point = e.GetPosition(image);

            MousePress.X = point.X;
            MousePress.Y = point.Y;

            if (!OutsidePicture(point))
            {
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
                //rect = null;
            }
        }
        private Point MousePress;
        private Point MouseRelease;

        private void crop_click(object sender, RoutedEventArgs e)
        {
            // Crop
            Point upperleftpoint = CropUtil.GetUpperLeftCornerInRectangel(MousePress, MouseRelease, rect);

            previousPoints.Enqueue(upperleftpoint);
            previousRect.Push(rect);

            var img = CropPicture(upperleftpoint, rect);

            SetImage(img);

            CropButton.IsEnabled = false;
        }

        private void SetImage(WriteableBitmap img)
        {
            ResetImage();
            image.Source = img;
        }

        private WriteableBitmap CropPicture(Point upperleftpoint, Rectangle rectangle)
        {
            WriteableBitmap wb = null;
            try
            {
                wb = new WriteableBitmap(Convert.ToInt32(rectangle.Width), Convert.ToInt32(rectangle.Height));
                TranslateTransform t = new TranslateTransform();
                t.X = NegativeNumber(upperleftpoint.X);
                t.Y = NegativeNumber(upperleftpoint.Y);

                //Draw to Writable Bitmap
                wb.Render(image, t);
                wb.Invalidate();

                image.Source = wb;
            }
            catch (Exception)
            {
                // todo insert log entry?
            }

            return wb;
        }

        private void ResetImage()
        {

            image.Clip = null;
            canvas.Children.Clear();

            var uri = new Uri(@"http://i.imgur.com/tmsTg.png");
            image.Source = new BitmapImage(uri);
            canvas.Children.Add(image);
            rect = null;
        }


        private void reset_click(object sender, RoutedEventArgs e)
        {
            ResetImage();
            CropButton.IsEnabled = true;

        }
        private double NegativeNumber(double number)
        {
            return -Math.Abs(number);
        }

        private void cropception_click(object sender, RoutedEventArgs e)
        {
            if (previousPoints == null) return;
            ResetImage();
            var length = previousPoints.Count;

            Point point = new Point(0, 0);

            for (int i = 0; i < length; i++)
            {
                var prevPoint = previousPoints.Dequeue();

                point.X += prevPoint.X;
                point.Y += prevPoint.Y;
            }
            var rectang = previousRect.Pop();
            WriteableBitmap wb = CropPicture(point, rectang);

            SetImage(wb);


            wb = null;
            rectang = null;
            rect = null;
            previousRect.Clear();
            previousPoints.Clear();

        }
    }
}
