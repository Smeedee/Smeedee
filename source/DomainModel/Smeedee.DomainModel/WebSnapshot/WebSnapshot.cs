using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    public class WebSnapshot
    {
        public virtual string InputUrl { get; set; }
        public virtual string XpathExpression { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual int[] CropCoordinates { get; set; }
    }
}
