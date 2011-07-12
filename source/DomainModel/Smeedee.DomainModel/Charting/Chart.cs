using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Charting
{
    public class Chart
    {

        public virtual string Database { get; set; }
        public virtual string Collection { get; set; }
        public virtual IList<DataSet> DataSets { get; set; }

    }

    public class DataSet
    {
        public virtual string Name { get; set; }
        public virtual IList<DataPoint> DataPoints { get; set; }
    }

    public class DataPoint
    {
        public virtual Object X { get; set; }
        public virtual Object Y { get; set; }

    }
}
