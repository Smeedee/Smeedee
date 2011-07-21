using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.DomainModel.Charting;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartUpdater
    {

        private ChartViewModel viewModel;
        private IUIInvoker uiInvoker;
        private ChartConfig chartConfig;
        private IChartStorageReader storageReader;
        private CollectionDownloadManager downloadManager = null;

        private List<SeriesConfigViewModel> series;
        private Collection<Chart> downloadedCharts;

        public ChartUpdater(ChartViewModel viewModel, ChartConfig config, IChartStorageReader storageReader, IUIInvoker uiInvoker)
        {
            this.viewModel = viewModel;
            this.uiInvoker = uiInvoker;
            chartConfig = config;
            this.storageReader = storageReader;
        }

        public void Update()
        {
            if (chartConfig.IsValid)
            {
                DownloadData();
                HideErrorMessage();
            }
            else
                ShowErrorMessage("No chart configured.");
        }

        private void DownloadData()
        {
            downloadManager = new CollectionDownloadManager(storageReader, DownloadCompleted);
            series = chartConfig.GetSeries();
            foreach (var s in series)
            {
                if (s.Action == ChartConfig.SHOW || s.Action == ChartConfig.REFERENCE)
                    downloadManager.AddDownload(new CollectionDownload(s.Database, s.Collection));
            }
            downloadManager.BeginDownload();
        }

        private void DownloadCompleted(Collection<Chart> downloadedCharts)
        {
            this.downloadedCharts = downloadedCharts;
            if (downloadedCharts.Count > 0)
            {
                uiInvoker.Invoke(() =>
                                     {
                                         AddSeries();
                                         downloadedCharts.Clear();
                                     });
            }
        }

        private void AddSeries()
        {
            viewModel.Lines.Clear();
            viewModel.Areas.Clear();
            viewModel.Columns.Clear();
            series.ForEach(AddOneSeries);
        }

        private void AddOneSeries(SeriesConfigViewModel series)
        {
            var chart = downloadedCharts.Where(c => c.Database == series.Database && c.Collection == series.Collection).FirstOrDefault();
            if (chart == null) return; // TODO: error handling
            var dataset = chart.DataSets.Where(s => s.Name == series.Name).FirstOrDefault();
            if (dataset == null) return; // TODO: error handling;

            var vm = ConvertDataSetToViewModel(dataset);
            switch (series.ChartType)
            {
                case ChartConfig.LINE:
                    viewModel.Lines.Add(vm);
                    break;
                case ChartConfig.COLUMNS:
                    viewModel.Columns.Add(vm);
                    break;
                case ChartConfig.AREA:
                    viewModel.Areas.Add(vm);
                    break;
            }
           
        }

        private DataSetViewModel ConvertDataSetToViewModel(DataSet dataset)
        {
            var vm = new DataSetViewModel {Name = dataset.Name};
            for (int i = 0; i<dataset.DataPoints.Count; i++)
            {
                vm.Data.Add(new DataPointViewModel { X = i, Y = dataset.DataPoints[i]});
            }
            return vm;
        }

        private void ShowErrorMessage(string message)
        {
            uiInvoker.Invoke(() =>
                                 {
                                     viewModel.ErrorMessage = message;
                                     viewModel.ShowErrorMessageInsteadOfChart = true;
                                 });
        }

        private void HideErrorMessage()
        {
            uiInvoker.Invoke(() =>
                                 {
                                     viewModel.ShowErrorMessageInsteadOfChart = false;
                                     viewModel.ErrorMessage = null;
                                 });
        }
    }

    public class CollectionDownloadManager
    {
        private Collection<CollectionDownload> downloads;
        private Action<Collection<Chart>> callbackWhenDownloaded;

        private Collection<Chart> downloadedCharts;

        private IChartStorageReader reader;

        private object LockObject = new object();

        public CollectionDownloadManager(IChartStorageReader reader, Action<Collection<Chart>> callbackWhenDownloaded)
        {
            downloads = new Collection<CollectionDownload>();
            this.callbackWhenDownloaded = callbackWhenDownloaded;
            this.reader = reader;
        }

        public void AddDownload(CollectionDownload download)
        {
            if (downloads.Any(download.IsTheSameAs))
                return;

            downloads.Add(download);
        }

        public void BeginDownload()
        {
            lock (LockObject)
            {
                downloadedCharts = new Collection<Chart>();
                foreach (var d in downloads)
                {
                    reader.LoadChart(d.Database, d.Collection, OnChartLoaded);
                }
            }
        }

        private CollectionDownload Get(Chart chart)
        {
            return downloads.FirstOrDefault(d => d.Database == chart.Database && d.Collection == chart.Collection);
        }

        private bool IsAllDownloaded()
        {
            return downloads.All(d => d.Done);
        }

        private void OnChartLoaded(Chart chart)
        {
            lock (LockObject)
            {
                var dl = Get(chart);
                if (dl == null) return; //TODO: error handling
                dl.Done = true;
                downloadedCharts.Add(chart);
                if (IsAllDownloaded() && callbackWhenDownloaded != null)
                    callbackWhenDownloaded(downloadedCharts);
            }
        }
    }

    public class CollectionDownload
    {

        public string Database { get; private set; }
        public string Collection { get; private set; }

        public CollectionDownload(string database, string collection)
        {
            Database = database;
            Collection = collection;
            Done = false;
        }

        public bool IsTheSameAs(CollectionDownload other)
        {
            return (Database == other.Database && Collection == other.Collection);
        }

        public bool Done { get; set; }
    }
}
