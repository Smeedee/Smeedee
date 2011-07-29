using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Resources;
using Smeedee.DomainModel.Charting;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartUpdater
    {

        private ChartViewModel viewModel;
        private IUIInvoker uiInvoker;
        public ChartConfig ChartConfig { get; set; }
        private IChartStorageReader storageReader;
        private CollectionDownloadManager downloadManager = null;

        private List<SeriesConfigViewModel> series;
        private Collection<Chart> downloadedCharts;

        private SeriesConfigViewModel referenceSeries = null;
        private DataSet referenceDataSet = null;

        private List<DataSetViewModel> allSeries = null;

        public ChartUpdater(ChartViewModel viewModel, IChartStorageReader storageReader, IUIInvoker uiInvoker)
        {
            this.viewModel = viewModel;
            this.uiInvoker = uiInvoker;
            this.storageReader = storageReader;
        }

        public void Update()
        {
            if (ChartConfig.IsValid)
            {
                HideErrorMessage();
                DownloadData();
            }
            else
            {
                ShowErrorMessage("No chart configured.");
                if (UpdateFinished != null)
                    UpdateFinished(this, EventArgs.Empty);
            }
        }

        public event EventHandler UpdateFinished;

        private void DownloadData()
        {
            downloadManager = new CollectionDownloadManager(storageReader, DownloadCompleted);
            series = ChartConfig.GetSeries();
            referenceSeries = null;
            referenceDataSet = null;
            foreach (var s in series)
            {
                if (s.Action == ChartConfig.SHOW || s.Action == ChartConfig.REFERENCE)
                    downloadManager.AddDownload(new CollectionDownload(s.Database, s.Collection));

                if (s.Action == ChartConfig.REFERENCE)
                {
                    referenceSeries = s;
                }
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
                                         AddNameData();
                                         AddSeries();
                                         downloadedCharts.Clear();
                                         if (UpdateFinished != null)
                                             UpdateFinished(this, EventArgs.Empty);
                                     });
            }
        }

        private void AddNameData()
        {
            viewModel.Name = !string.IsNullOrEmpty(ChartConfig.ChartName) ? ChartConfig.ChartName : null;
            viewModel.XAxisName = !string.IsNullOrEmpty(ChartConfig.XAxisName) ? ChartConfig.XAxisName : null;
            viewModel.YAxisName = !string.IsNullOrEmpty(ChartConfig.YAxisName) ? ChartConfig.YAxisName : null;
            viewModel.XAxisType = !string.IsNullOrEmpty(ChartConfig.XAxisType) ? ChartConfig.XAxisType : ChartConfig.CATEGORY;
        }

        private void AddSeries()
        {
            if (referenceSeries != null)
            {
                referenceDataSet = GetDataset(referenceSeries);
            }
            viewModel.Lines.Clear();
            viewModel.Areas.Clear();
            viewModel.Columns.Clear();
            viewModel.Series.Clear();

            allSeries = new List<DataSetViewModel>();
            series.ForEach(AddOneSeries);

            allSeries.Where(s => s.Type == ChartConfig.AREA).ToList().ForEach(viewModel.Series.Add);
            allSeries.Where(s => s.Type == ChartConfig.COLUMNS).ToList().ForEach(viewModel.Series.Add);
            allSeries.Where(s => s.Type == ChartConfig.LINE).ToList().ForEach(viewModel.Series.Add);            

            allSeries.Clear();
            allSeries = null;
        }

        private void AddOneSeries(SeriesConfigViewModel series)
        {
            if (series.Action != ChartConfig.SHOW) return;
            DataSet dataset = GetDataset(series);



            if (dataset == null) return; // TODO: error handling;

            var vm = ConvertDataSetToViewModel(dataset, series);

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

            allSeries.Add(vm);
        }

        private DataSet GetDataset(SeriesConfigViewModel series)

        {
            var chart = downloadedCharts.Where(c => c.Database == series.Database && c.Collection == series.Collection).FirstOrDefault();
            if (chart == null) return null;
            return chart.DataSets.Where(s => s.Name == series.Name).FirstOrDefault();
        }

        private DataSetViewModel ConvertDataSetToViewModel(DataSet dataset, SeriesConfigViewModel series)
        {
            var vm = new DataSetViewModel {Name = !string.IsNullOrEmpty(series.Legend) ? series.Legend : dataset.Name, Brush = BrushProvider.GetBrushName(series.Brush), Type = series.ChartType};

            for (int i = 0; i<dataset.DataPoints.Count; i++)
            {
                var Yvalue = 0;
                var Xvalue = referenceDataSet != null ? referenceDataSet.DataPoints[i] : i;
                if (int.TryParse(dataset.DataPoints[i].ToString(), out Yvalue))
                    vm.Data.Add(new DataPointViewModel { X = Xvalue, Y = Yvalue});
                else
                    Debug.WriteLine("ERROR PARSING!"); // TODO: need some errorhandling when datapoint could not be converted to a number
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
