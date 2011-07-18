using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    public class WebSnapshot
    {
        public virtual string Name { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual int PictureHeight { get; set; }
        public virtual int PictureWidth { get; set; }

        public virtual int CropHeight { get; set; }
        public virtual int CropWidth { get; set; }
        public virtual int CropCoordinateX { get; set; }
        public virtual int CropCoordinateY { get; set; }
    }
}
