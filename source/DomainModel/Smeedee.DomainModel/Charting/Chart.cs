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
        public Chart()
        {
            DataSets = new List<DataSet>();
        }

        public Chart(string database, string collection) : this()
        {
            Database = database;
            Collection = collection;
        }

        public string Database { get; set; }
        public string Collection { get; set; }
        public IList<DataSet> DataSets { get; private set; }

    }

    public class DataSet
    {
        public DataSet()
        {
            DataPoints = new List<object>();
        }

        public DataSet(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public IList<object> DataPoints { get; private set; }
    }
}
