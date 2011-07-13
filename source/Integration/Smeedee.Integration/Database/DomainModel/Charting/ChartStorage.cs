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
        private IPersistDomainModels<NoSqlDatabase> persistRepository;
        private IRepository<NoSqlDatabase> repository;

        public ChartStorage(IRepository<NoSqlDatabase> repository, IPersistDomainModels<NoSqlDatabase> persistRepository)
        {
            this.repository = repository;
            this.persistRepository = persistRepository;
        }

        public void Save(Chart chart)
        {
            if (chart.Database == null || chart.Collection == null)
                throw new NullReferenceException();

            var e = repository.Get(new LinqSpecification<NoSqlDatabase>(d => d.Name == chart.Database));

            var list = e.ToList();
            var database = list.Count > 0 ? list[0] : new NoSqlDatabase { Name = chart.Database };

            var collection = database.GetCollection(chart.Collection);
            collection.Documents.Clear();
            foreach (var dataset in chart.DataSets)
            {
                collection.Insert(Document.FromObject(dataset));
            }
            
            persistRepository.Save(database);
        }

        
    }
}
