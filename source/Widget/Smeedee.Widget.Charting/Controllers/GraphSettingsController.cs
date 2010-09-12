
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Framework;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.Controllers  
{
	public class GraphSettingsController
	{
	    private IDownloadStringService downloadStringService;
	    private GraphConfig graphConfig;
	    private GraphConfig backupConfig;
	    private IPersistDomainModels<Configuration> configRepository;
	    private GraphController graphController;
	    private const string documentsUrlFormat = "../Services/NoSql/Documents.ashx?database={0}&collection={1}";

	    public GraphSettingsController(GraphSettings graphSettingsViewModel,
            GraphController graphController,
            Configuration configuration,
            IDownloadStringService downloadStringService,
            IPersistDomainModels<Configuration> configRepository)
		{
	        Guard.Requires<ArgumentNullException>(graphSettingsViewModel != null);
            Guard.Requires<ArgumentNullException>(graphController != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(downloadStringService != null);
            Guard.Requires<ArgumentNullException>(configRepository != null);

            this.graphController = graphController;
            GraphSettingsViewModel = graphSettingsViewModel;
	        GraphSettingsViewModel.Graph = GraphViewModel;
            GraphSettingsViewModel.PropertyChanged += ViewModel_PropertyChanged;
            GraphSettingsViewModel.Save.AfterExecute += ViewModel_Save_AfterExecute;
            GraphSettingsViewModel.Test.AfterExecute += ViewModel_Test_AfterExecute;
            graphSettingsViewModel.Cancel.AfterExecute += ViewModel_Cancel_AfterExecute;
            this.downloadStringService = downloadStringService;
            this.configRepository = configRepository;
            graphConfig = new GraphConfig(configuration);
            backupConfig = new GraphConfig(configuration.Clone() as Configuration);

		    DownloadAndLoadDataIntoViewModel();
		}

        public GraphSettings GraphSettingsViewModel { get; private set; }
        public Graph GraphViewModel { get { return graphController.ViewModel; } }

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCollection")
            {
                if (GraphSettingsViewModel.SelectedDatabase != null &&
                    GraphSettingsViewModel.SelectedCollection != null)
                {
                    GraphSettingsViewModel.AvailableProperties.Clear();

                    downloadStringService.DownloadAsync(GetDocumentUrl(), data =>
                    {
                        foreach (var doc in JArray.Parse(data))
                        {
                            var properties = from property in doc.Children()
                                             where property is JProperty
                                             select property as JProperty;

                            foreach (var property in properties)
                            {
                                if (!GraphSettingsViewModel.AvailableProperties.Contains(property.Name))
                                    GraphSettingsViewModel.AvailableProperties.Add(property.Name);
                            }
                        }

                        GraphSettingsViewModel.SelectedPropertyForXAxis = GraphSettingsViewModel.AvailableProperties.FirstOrDefault(p =>
                            p == graphConfig.XAxisProperty);
                        GraphSettingsViewModel.SelectedPropertyForYAxis = GraphSettingsViewModel.AvailableProperties.FirstOrDefault(p =>
                            p == graphConfig.YAxisProperty);
                    });
                }
            }
        }

        void ViewModel_Save_AfterExecute(object sender, EventArgs e)
        {
            MapViewModelToConfiguration();

            //Refresh the View
            GraphViewModel.Refresh.Execute();

            configRepository.Save(graphConfig.Configuration);
        }

	    private void MapViewModelToConfiguration()
	    {
	        graphConfig.Database = GraphSettingsViewModel.SelectedDatabase.Name;
	        graphConfig.Collection = GraphSettingsViewModel.SelectedCollection.Name;
	        graphConfig.XAxisProperty = GraphSettingsViewModel.SelectedPropertyForXAxis;
	        graphConfig.YAxisProperty = GraphSettingsViewModel.SelectedPropertyForYAxis;
	        graphConfig.MaxNumberOfDataPoints = GraphSettingsViewModel.NumberOfDataPoints;
	        graphConfig.IsConfigured = true;
	    }

	    void ViewModel_Test_AfterExecute(object sender, EventArgs e)
        {
            MapViewModelToConfiguration();

            graphController.UpdateConfiguration(graphConfig.Configuration);
        }

        void ViewModel_Cancel_AfterExecute(object sender, EventArgs e)
        {
            graphController.UpdateConfiguration(backupConfig.Configuration);
            UpdateConfiguration(backupConfig.Configuration);
        }

	    private Uri GetDocumentUrl()
	    {
	        return new Uri(string.Format(
                documentsUrlFormat, 
                    GraphSettingsViewModel.SelectedDatabase.Name, 
                    GraphSettingsViewModel.SelectedCollection.Name), 
                UriKind.Relative);
	    }

	    private void DownloadAndLoadDataIntoViewModel()
	    {
            downloadStringService.DownloadAsync(new Uri("../Services/NoSql/Databases.ashx", UriKind.Relative), data =>
            {
                var databases = Collection.Parse(data);
                foreach (var databaseDoc in databases.Documents.OrderBy(d => d["Name"].Value<Object>()))
                {
                    var databaseViewModel = new DatabaseViewModel()
                    {
                        Name = databaseDoc["Name"].Value<String>()
                    };
                    foreach (var collectionDoc in databaseDoc["Collections"])
                    {
                        databaseViewModel.Collections.Add(new CollectionViewModel()
                        {
                            Name = collectionDoc["Name"].Value<string>() 
                        });    
                    }
                    GraphSettingsViewModel.Databases.Add(databaseViewModel);

                    MapConfigurationToViewModel();
                }
            });
	    }

	    public void UpdateConfiguration(Configuration config)
	    {
            graphConfig = new GraphConfig(config);
            backupConfig = new GraphConfig(config.Clone() as Configuration);

            DownloadAndLoadDataIntoViewModel();
	    }

	    private void MapConfigurationToViewModel()
	    {
	        GraphSettingsViewModel.NumberOfDataPoints = graphConfig.MaxNumberOfDataPoints;
	        GraphSettingsViewModel.SelectedDatabase =
	            GraphSettingsViewModel.Databases.FirstOrDefault(d => 
	                d.Name == graphConfig.Database);

	        if (GraphSettingsViewModel.SelectedDatabase != null)
	        {
	            GraphSettingsViewModel.SelectedCollection =
	                GraphSettingsViewModel.SelectedDatabase.Collections.FirstOrDefault(c =>
	                    c.Name == graphConfig.Collection);
	        }
	    }
	}
}
