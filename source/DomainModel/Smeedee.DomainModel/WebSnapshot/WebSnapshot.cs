using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    public class WebSnapshot
    {
        public virtual byte[] Picture { get; set; }
        public virtual int PictureHeight { get; set; }
        public virtual int PictureWidth { get; set; }
        public virtual int[] CropCoordinates { get; set; }
    }
}
