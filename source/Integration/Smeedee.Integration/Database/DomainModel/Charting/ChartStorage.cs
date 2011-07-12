using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;

namespace Smeedee.Integration.Database.DomainModel.Charting
{
    public class ChartStorage : IChartStorage
    {
        private IPersistDomainModelsAsync<NoSqlDatabase> persistRepository;

        public ChartStorage(IPersistDomainModelsAsync<NoSqlDatabase> persistRepository)
        {
            this.persistRepository = persistRepository;
        }

        public void Save(Chart chart)
        {
            if (chart.Database == null || chart.Collection == null)
                throw new NullReferenceException();

            var database = new NoSqlDatabase() { Name = chart.Database };
            var collection = database.GetCollection(chart.Collection);
            foreach (var dataset in chart.DataSets)
            {
                collection.Insert(Document.FromObject(dataset));
            }
            persistRepository.Save(database);
        }

    }
}
