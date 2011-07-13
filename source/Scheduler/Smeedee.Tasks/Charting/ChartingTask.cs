using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.Database.DomainModel.Charting;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.Charting
{
    [Task("Charting Task",
         Author = "Smeedee Team",
         Description = "Retrieves information from some file to show in some chart",
         Version = 1,
         Webpage = "http://smeedee.org")]
    [TaskSetting(1, FILEPATH, typeof(Uri), "")]
    [TaskSetting(2, VALUE_SEPARATOR, typeof(string), ",","At what value should the data be separated. Ex. CSV uses ','")]
    [TaskSetting(3, DATABASE_NAME, typeof(string), "", "Give your database a name")]
    [TaskSetting(4, COLLECTIONS_NAME, typeof(string),"default collection","" )]
    public class ChartingTask : TaskBase
    {
        protected const string FILEPATH = "File path";
        protected const string VALUE_SEPARATOR = "Value separator";
        protected const string DATABASE_NAME = "Name of database";
        protected const string COLLECTIONS_NAME = "Name of collection";

        private IChartStorage chartStorage;
        private TaskConfiguration _configuration;
        private IDownloadStringService downloadStringService;

        public ChartingTask(IChartStorage chartStorage, TaskConfiguration configuration, IDownloadStringService downloadStringService)
        {
            //Guard.Requires<ArgumentNullException>(databasePersister != null);
            //Guard.Requires<ArgumentNullException>(configuration != null);
            //Guard.Requires<TaskConfigurationException>(configuration.Entries.Count() >= 3);
            this.chartStorage = chartStorage;
            _configuration = configuration;
            this.downloadStringService = downloadStringService;
        }

        public override string Name 
        { 
            get { return "Charting Task"; } 
        }

        public override void Execute()
        {
            //var separator = (string)_configuration.ReadEntryValue(VALUE_SEPARATOR);
            //var filepath = (string) _configuration.ReadEntryValue(FILEPATH);

            //var dataset = GetDataSetFromFile(filepath, separator);

            //var chart = new Chart();
            //chart.Database = (string) _configuration.ReadEntryValue(DATABASE_NAME);
            //chart.Collection = (string) _configuration.ReadEntryValue(COLLECTIONS_NAME);
            //chart.DataSets.Add(dataset);

            //chartStorage.Save(chart);
            
        }
        string someString = "";
        public DataSet GetDataSetFromFile(string somefilepath, string separator)
        {
            var dataset = new DataSet();
            //downloadStringService.DownloadAsync(new Uri(somefilepath), () => someMethod(ref someString) );
            //var splittedString = someString.Split(char.Parse(separator));
            //for (int i = 0; i < splittedString.Length ; i++)
            //{
            //    var dp = new DataPoint();
            //    dp.X = i;
            //    dp.Y = splittedString[i];
            //    dataset.DataPoints.Add(dp);
            //}

            return dataset;
        }

        private void someMethod(ref string someStringparameter)
        {
            someString = someStringparameter;
        }
    }
}
