using System;
using System.Collections.Generic;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.Repositories
{
    public class WidgetConfigurationRepository : IAsyncRepository<Configuration>, IRepository<Configuration>, IPersistDomainModelsAsync<Configuration>
    {
        private readonly Widget widgetToReturnConfigurationFrom;
        private readonly IPersistDomainModelsAsync<Configuration> configPersister;

        public WidgetConfigurationRepository(Widget widgetToReturnConfigurationFrom, IPersistDomainModelsAsync<Configuration> configPersister)
        {
            this.widgetToReturnConfigurationFrom = widgetToReturnConfigurationFrom;
            this.configPersister = configPersister;
            this.configPersister.SaveCompleted += (o, e) => RaiseSaveCompleted();

        }

        public void BeginGet(Specification<Configuration> specification)
        {
            if( GetCompleted != null )
                GetCompleted(this, new GetCompletedEventArgs<Configuration>(new List<Configuration>{widgetToReturnConfigurationFrom.Configuration}, specification));
        }

        public event EventHandler<GetCompletedEventArgs<Configuration>> GetCompleted;
        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            return new List<Configuration> {widgetToReturnConfigurationFrom.Configuration};
        }

        public void Save(Configuration domainModel)
        {
            domainModel.Id = widgetToReturnConfigurationFrom.Configuration.Id;
            widgetToReturnConfigurationFrom.Configuration = domainModel;
            configPersister.Save(widgetToReturnConfigurationFrom.Configuration);
        }

        private void RaiseSaveCompleted()
        {
            if( SaveCompleted != null )
                SaveCompleted(this, new SaveCompletedEventArgs());
        }

        public void Save(IEnumerable<Configuration> domainModels)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;
    }
}