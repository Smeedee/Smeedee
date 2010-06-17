using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.Views;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Widgets.SL.CI.Views;

namespace Smeedee.Client.Widgets.SL.CI
{
    public class BuildStatusTraybarWidget : TraybarWidget
    {
        public BuildStatusTraybarWidget()
        {
            View = new WidgetView(new BuildStatusView()
            {
                DataContext = this
            });

            var t = new Thread(() =>
            {
                Thread.Sleep(2000);
                //ReportFailure("Somethign went terrible wrong:(\r\ncouldn't get data from server");
            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
