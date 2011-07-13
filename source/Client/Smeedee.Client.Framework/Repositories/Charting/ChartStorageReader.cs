using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.NoSql;

namespace Smeedee.Client.Framework.Repositories.Charting
{
    public class ChartStorageReader
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
            repository.GetDocuments(database, collection, c =>
            {
                var chart = new Chart(database, collection);
                foreach (var document in c.Documents)
                {
                    var dataset = new DataSet {Name = document["Name"].Value<string>()};

                    foreach (var point in document["DataPoints"].ToList<object>())
                        dataset.DataPoints.Add(point);

                    chart.DataSets.Add(dataset);
                }
                ChartLoaded.Invoke(this, new ChartLoadedEventArgs(chart));
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
