using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.NoSql;
using System.Collections.Generic;


namespace Smeedee.Client.Framework.Repositories.Charting
{
    public interface IChartStorageReader
    {
        event EventHandler DatasourcesRefreshed;
        event EventHandler<ChartLoadedEventArgs> ChartLoaded;
        IList<string> GetDatabases();
        IList<string> GetCollectionsInDatabase(string database);
        void LoadChart(string database, string collection);
        void LoadChart(string database, string collection, Action<Chart> callback);
        void RefreshDatasources();
    }

    public class ChartStorageReader : IChartStorageReader
    {
        private INoSqlRepository repository;

        private Collection databases = new Collection();

        public ChartStorageReader(INoSqlRepository noSqlRepository)
        {
            repository = noSqlRepository;
        }

        public event EventHandler DatasourcesRefreshed;
        public event EventHandler<ChartLoadedEventArgs> ChartLoaded;

        public IList<string> GetDatabases()
        {
            return NoSqlRepository.GetDatabasesAsList(databases);
        }
        
        public IList<string> GetCollectionsInDatabase(string database)
        {
            return NoSqlRepository.GetCollectionsInDatabase(database, databases);
        }

        public void LoadChart(string database, string collection)
        {
            LoadChart(database, collection, null);
        }
        
        public void LoadChart(string database, string collection, Action<Chart> callback)
        {
            repository.GetDocuments(database, collection, c =>
            {
                var chart = new Chart(database, collection);
                foreach (var document in c.Documents)
                {
                    var dataset = new DataSet {Name = document["Name"].Value<string>()};

                    foreach (var point in document["DataPoints"].Values<object>().ToList<object>())
                        dataset.DataPoints.Add(point);

                    chart.DataSets.Add(dataset);
                }
                if (callback != null)
                    callback(chart);
                if (ChartLoaded != null)
                    ChartLoaded(this, new ChartLoadedEventArgs(chart));
            });

        }

        public void RefreshDatasources()
        {
            repository.GetDatabases(d =>
            {
                databases = d;
                DatasourcesRefreshed(this, EventArgs.Empty);
            });
        }
    }

    public class ChartLoadedEventArgs : EventArgs
    {
        public Chart Chart { get; private set; }

        public ChartLoadedEventArgs(Chart chart)
        {
            this.Chart = chart;
        }
    }
}
