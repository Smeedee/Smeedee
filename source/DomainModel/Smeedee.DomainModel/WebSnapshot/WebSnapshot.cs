using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    public class WebSnapshot
    {
        public virtual string Name { get; set; }
        public virtual string PictureFilePath { get; set; }
        public virtual int PictureHeight { get; set; }
        public virtual int PictureWidth { get; set; }
    }
}
