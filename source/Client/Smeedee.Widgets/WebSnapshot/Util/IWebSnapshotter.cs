using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Smeedee.Widgets.WebSnapshot.Util
{
    public interface IWebSnapshotter
    {
        string Url { get; }
        Bitmap GetSnapshot(string url);
        bool IsBlank(Bitmap image);
    }
}
