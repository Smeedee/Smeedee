using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.DomainModel.Framework.Services;
using System.Threading;

namespace APD.Client.Widget.Admin.SL.Services
{
    public class URLCheckerProxy : ICheckIfResourceExists
    {

        public bool Check(string url)
        {
            Thread.Sleep(1000);
            if (url == "http://goeran.no")
                return true;
            else
                return false;
        }
    }
}
