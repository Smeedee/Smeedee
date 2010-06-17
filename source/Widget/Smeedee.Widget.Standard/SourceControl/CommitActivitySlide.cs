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
using Smeedee.Client.Widgets.SL.SourceControl.Views;

namespace Smeedee.Client.Widgets.SL.SourceControl
{
    //[Export(typeof(Slide))]
    public class CommitActivitySlide : Slide
    {
        public CommitActivitySlide()
        {
            Title = "Commit activity";
            View = new Grid()
            {
                DataContext = this
            };
            SettingsView = new ConfigureVCS()
            {
                DataContext = this
            };

            var t = new Thread(() =>
            {
                Thread.Sleep(5000);
                //ReportFailure("Version Control System is not configured...");
            });
            t.Start();
        }
    }
}
