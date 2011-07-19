using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Charting;
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
    [TaskSetting(1, FILEPATH, typeof(Uri), "file://")]
    [TaskSetting(2, VALUE_SEPARATOR, typeof(string), ",", "At what value should the data be separated. Ex. CSV uses ','")]
    [TaskSetting(3, COLLECTIONS_NAME, typeof(string), "Collection", "Give this data a unique name in the database")]
    public class ChartingTask : TaskBase
    {
        public const string FILEPATH = "File path";
        public const string VALUE_SEPARATOR = "Value separator";
        public const string COLLECTIONS_NAME = "Name of collection";

        private IChartStorage chartStorage;
        private TaskConfiguration _configuration;
        private IDownloadStringService downloadStringService;

        public ChartingTask(IChartStorage chartStorage, TaskConfiguration configuration, IDownloadStringService downloadStringService)
        {
            Guard.Requires<ArgumentNullException>(chartStorage != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<TaskConfigurationException>(configuration.Entries.Count() >= 3);
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
            var separator = (string)_configuration.ReadEntryValue(VALUE_SEPARATOR);
            var filepath = _configuration.ReadEntryValue(FILEPATH);

            GetDataSetFromFile(filepath.ToString(), separator, SaveDataToStorage);
        }

        public void GetDataSetFromFile(string filepath, string separator, Action<IList<DataSet>> callback)
        {
            if (IsValidURL(filepath))
            {
                var datasets = new List<DataSet>();
                downloadStringService.DownloadAsync(new Uri(filepath), data =>
                                                                           {
                                                                               var oneLineOneDataset = data.Split('\n');
                                                                               var valueSeparator = char.Parse(separator);
                                                                               int shortestDataset = GetShortestDataset(oneLineOneDataset, valueSeparator);

                                                                               foreach (var item in oneLineOneDataset)
                                                                               {
                                                                                   var set = new DataSet();
                                                                                   var splittedString = item.Split(valueSeparator);
                                                                                   
                                                                                   int i = 0;
                                                                                   if (!IsNumber(splittedString[0]))
                                                                                   {
                                                                                       set.Name = splittedString[0];
                                                                                       i = 1;
                                                                                   }
                                                                                   for (;i < shortestDataset; i++)
                                                                                   {
                                                                                        set.DataPoints.Add(splittedString[i].Trim());
                                                                                   }
                                                                                   datasets.Add(set);
                                                                               }
                                                                               callback(datasets);
                                                                           });
            }

        }

        private int GetShortestDataset(string[] datasets, char separator)
        {
            int shortestDataset = int.MaxValue;
            
            foreach (var set in datasets)
            {
                if (shortestDataset > set.Split(separator).Length)
                    shortestDataset = set.Split(separator).Length;
            }
            return shortestDataset;
        }

        public void SaveDataToStorage(IList<DataSet> datasets)
        {
            var collection = (string)_configuration.ReadEntryValue(COLLECTIONS_NAME);
            var chart = new Chart("charting", collection);
            foreach (var set in datasets)
                chart.DataSets.Add(set);
            chartStorage.Save(chart);
        }

        public static bool IsValidURL(string filepath)
        {
            if (Regex.IsMatch(filepath, "^file://"))
                return true;
            if (Regex.IsMatch(filepath, "^https?://[a-zA-Z1-9]"))
                return true;
            return false;
        }

        public static bool IsNumber(string word)
        {
            return Regex.IsMatch(word, @"^-?\d*[0-9]?(|.\d*[0-9]|,\d*[0-9])?$");
        }

    }
}
