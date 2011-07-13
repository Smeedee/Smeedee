using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
/*        public event EventHandler<ChartLoadedEventArgs> ChartLoaded;

        public void RefreshDatasources()
        {
            DatasourcesRefreshed.Invoke(this, EventArgs.Empty);
        }
        */
        public IList<string> GetDatabases()
        {
            return NoSqlRepository.GetDatabasesAsList(databases);
        }
        
        public IList<string> GetCollectionsInDatabase(string database)
        {
            return NoSqlRepository.GetCollectionsInDatabase(database, databases);
        }
        /*
        public void BeginGetChart(string database, string collection)
        {
            ChartLoaded.Invoke(this, new ChartLoadedEventArgs(new Chart()));
        }*/

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
