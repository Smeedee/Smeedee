using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.TeamPicture
{
    public class TeamPicture
    {
        public virtual int PictureHeight { get; set; }
        public virtual int PictureWidth { get; set; }
        public virtual string Message { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual string PictureScaling { get; set; }
    }
}
