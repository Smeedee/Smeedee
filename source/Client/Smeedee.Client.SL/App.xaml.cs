using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Windows;
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.SL.MEF;
using TinyMVVM.Framework;

namespace Smeedee.Client.SL
{
    public partial class App : Application
    {   

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServiceLocator.SetLocator(new ServiceLocatorForSLClient());
            
            var aggregateCatalog = new AggregateCatalog();
            var deploymentFolderCatalog = new DeploymentFolderCatalog();
            deploymentFolderCatalog.DownloadAsync();

            aggregateCatalog.Catalogs.Add(deploymentFolderCatalog);

            foreach (var catalog in aggregateCatalog.Catalogs)
            {
                if (catalog is DeploymentCatalog)
                {
                    var deploymentCatalog = catalog as DeploymentCatalog;
                    deploymentCatalog.DownloadCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(catalog_DownloadCompleted);
                    deploymentCatalog.DownloadAsync();
                }
            }

            CompositionHost.Initialize(aggregateCatalog);
            Thread.Sleep(10000);

            this.RootVisual = new MainPage();
        }

        void catalog_DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
