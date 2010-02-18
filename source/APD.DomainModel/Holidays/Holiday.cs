using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.DomainModel.Holidays
{
    public class Holiday
    {
        public virtual DateTime Date { get; set; }
        public virtual string Description { get; set; }
    }
}
