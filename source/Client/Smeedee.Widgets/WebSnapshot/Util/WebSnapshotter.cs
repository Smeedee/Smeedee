using System;
using System.Linq;
using System.Text;


namespace Smeedee.Widgets.WebSnapshot.Util
{
    public class WebSnapshotter
    {
        private string Url { get; set; }
        public bool IsFinished { get; private set; }


        private Bitmap RenderBitmap(WebBrowser wb)
        {
            var bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
            wb.Dispose();

            return bitmap;
        }



    }
}
