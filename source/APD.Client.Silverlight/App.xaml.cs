#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Resources;
using System.IO;

using APD.Client.Framework.Settings;
using APD.Client.Framework.Settings.Repository;
using APD.DomainModel.Framework.Logging;
using APD.Framework.SL.Logging;


namespace APD.Client.Silverlight
{
    public partial class App : Application
    {
        private ClientSettingsReader clientSettingsReader;

        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeSettingsReader();
            //clientSettingsReader.SettingsLoaded += InitializeBootstrapperAfterSettingsLoaded;
        }

        private void InitializeSettingsReader() {
            var localSettingsRepository = new IsolatedStorageSettingsRepository();
            var serverSettingsRepository = new ServerSettingsRepository();

            this.clientSettingsReader = new ClientSettingsReader(localSettingsRepository, serverSettingsRepository, InitializeBootstrapperAfterSettingsLoaded);
        }

        private void InitializeBootstrapperAfterSettingsLoaded(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var bootstrapper = new BootStrapper(clientSettingsReader);
                bootstrapper.Run();
            });
        }

        private void Application_Exit(object sender, EventArgs eventArgs) {}

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                ILog logger = new DatabaseLogger(new LogEntryWebservicePersister());
                ErrorLogEntry error = new ErrorLogEntry()
                {
                    Message = e.ExceptionObject.ToString(),
                    Source = sender.GetType().ToString(),
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

                System.Windows.Browser.HtmlPage.Window.Eval(
                    "throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception) {}
        }
    }
}