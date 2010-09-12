
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.NoSql;
using Smeedee.Framework;
using Tskjortebutikken.Widgets.ViewModel;
using Newtonsoft.Json.Linq;

namespace Tskjortebutikken.Widgets.Controllers  
{
	public class GraphController
	{
        public static readonly string relativeUrl = "../Services/NoSql/Documents.ashx?database={0}&collection={1}";

	    private IDownloadStringService downloadStringService;
	    private ITimer timer;
	    private IProgressbar progressbarService;
	    private GraphConfig graphConfig;

	    public GraphController(Graph viewModel, 
            IDownloadStringService downloadStringService,
            ITimer timer,
            Configuration configuration,
            IProgressbar progressbarService)
		{
	        Guard.Requires<ArgumentNullException>(viewModel != null);
            Guard.Requires<ArgumentNullException>(downloadStringService != null);
            Guard.Requires<ArgumentNullException>(timer != null);
            Guard.Requires<ArgumentNullException>(progressbarService != null);
            Guard.Requires<ArgumentNullException>(configuration != null);

            graphConfig = new GraphConfig(configuration);

            Guard.Requires<ArgumentException>(graphConfig.IsValid, graphConfig.ErrorMsg);

			this.ViewModel = viewModel;
            ViewModel.Refresh.AfterExecute += new EventHandler(Refresh_AfterExecute);
            this.downloadStringService = downloadStringService;
            this.timer = timer;
            this.timer.Elapsed += new EventHandler(timer_Elapsed);
            this.progressbarService = progressbarService;

	        StartFetchingDataInBackground();

	        DownloadDataAndAddToViewModel();
		}

        void Refresh_AfterExecute(object sender, EventArgs e)
        {
            DownloadDataAndAddToViewModel();
        }

        public Graph ViewModel { get; private set; }

	    private void DownloadDataAndAddToViewModel()
	    {
            if (graphConfig.IsConfigured)
            {
                Download(collection => LoadDataIntoViewModel(collection));
            }
        }

	    void timer_Elapsed(object sender, EventArgs e)
        {
            DownloadDataAndAddToViewModel();
        }

        private void Download(Action<Collection> callback)
        {
            progressbarService.ShowInView("Downloading data...");
            downloadStringService.DownloadAsync(Url, (data) =>
            {
                progressbarService.HideInView();
                callback.Invoke(Collection.Parse(data));
            });
        }

	    private Uri Url
	    {
	        get
	        {
	            return new Uri(
                    string.Format(relativeUrl,
                        graphConfig.Database,
                        graphConfig.Collection),
                    UriKind.Relative);
	        }
	    }

	    private void LoadDataIntoViewModel(Collection collection)
	    {
            ViewModel.Data.Clear();
            int numberOfDataPointsAdded = 0;
	        var dataPoints = new List<DataPoint>();

	        try
	        {
                foreach (var doc in collection.Documents.OrderByDescending(d => d[graphConfig.XAxisProperty].Value<Object>()))
                {
                    dataPoints.Add(new DataPoint() { X = doc[graphConfig.XAxisProperty].Value<Object>(), Y = doc[graphConfig.YAxisProperty].Value<Object>() });
                    numberOfDataPointsAdded++;
                    if (numberOfDataPointsAdded >= graphConfig.MaxNumberOfDataPoints)
                        break;
                }
	        }
	        catch (Exception)
	        {
	        }

            if (numberOfDataPointsAdded > 0)
            {
                dataPoints.Reverse();
                foreach (var dataPoint in dataPoints)
                {
                    ViewModel.Data.Add(dataPoint);
                }
            }
	    }

        public void StopFetchingDataInBackground()
        {
            timer.Stop();            
        }

        public void StartFetchingDataInBackground()
        {
            this.timer.Start(graphConfig.RefreshInteval);
        }

	    public void UpdateConfiguration(Configuration config)
	    {
	        graphConfig = new GraphConfig(config);

            DownloadDataAndAddToViewModel();
	    }
	}
}
