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

        public virtual string Name { get; set; };
        public virtual string ChartType { get; set; };
        public virtual string XAxisName { get; set; };
        public virtual string YAxisName { get; set; };
        public virtual IList<DataSet> DataSets { get; set; };

    }

    public class DataSet
    {
        public virtual IList<DataPoint> DataPoints { get; set; };
    }

    public class DataPoint
    {
        public virtual Object X { get; set; };
        public virtual Object Y { get; set; };

    }
}
