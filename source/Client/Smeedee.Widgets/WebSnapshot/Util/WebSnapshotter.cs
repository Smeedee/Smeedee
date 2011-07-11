using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class WebSnapshotter
    {
        public string Url { get; private set; }
        public bool IsFinished { get; private set; }
        public Bitmap Snapshot { get; private set; }


        public Bitmap GetSnapshot(string url)
        {
            Url = url;
            IsFinished = false;

            var thread = new Thread(WorkerMethod);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (!IsFinished) { /*stupid way of halting code execution untill thread is finished*/}
            //while (thread.IsAlive){}

            return Snapshot;
        }


        private void WorkerMethod(object state)
        {
            var wb = SetupBrowser();
            Snapshot = RenderBitmap(wb);
            //pic.Save("websnapshot.bmp", ImageFormat.Bmp);
            IsFinished = true;
        }

        private WebBrowser SetupBrowser()
        {
            var wb = new WebBrowser();
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;
            wb.Navigate(Url);
            while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }
            wb.Width = wb.Document.Body.ScrollRectangle.Width;
            wb.Height = wb.Document.Body.ScrollRectangle.Height;

            return wb;
        }

        private Bitmap RenderBitmap(WebBrowser wb)
        {
            var bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
            wb.Dispose();
            
            return bitmap;
        }

        public bool IsBlank(Bitmap picture)
        {
            var colors = new List<Color>();          
            for (int i = 0 ; i < picture.Width ; i++)
            {
                for (int j = 0 ; j < picture.Height ; j++ )
                {
                    colors.Add(picture.GetPixel(i,j));
                }
            }

            var firstColor = colors.First().ToArgb();
            var filteredColors = colors.FindAll(pixel => pixel.ToArgb() == firstColor);
            return colors.Count == filteredColors.Count;
        }


    }
}
