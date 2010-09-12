using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Windows;
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.SL.MEF;
using Smeedee.Client.Framework.SL.ViewModel.Repositories;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.Framework.Logging;
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
            FrameworkBootstrapper.Initialize();
       
            WidgetMetadataRepository.Instance.BeginGet(All.ItemsOf<WidgetMetadata>());

            RootVisual = new MainPage();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                ILog logger = new Logger(new Framework.SL.Repositories.LogEntryWebserviceRepository());
                ErrorLogEntry error = new ErrorLogEntry()
                {
                    Message = e.ExceptionObject.ToString(),
                    Source = "UNHANDLED IN: " + sender.GetType().ToString(),
                    TimeStamp = DateTime.Now
                };

                logger.WriteEntry(error);
                e.Handled = true;
            }
            catch (Exception logException)
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
