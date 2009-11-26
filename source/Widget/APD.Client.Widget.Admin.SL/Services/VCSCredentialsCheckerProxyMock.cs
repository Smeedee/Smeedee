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
    public class VCSCredentialsCheckerProxyMock : ICheckIfCredentialsIsValid
    {

        #region ICheckIfCredentialsIsValid Members

        public bool Check(string provider, string url, string username, string password)
        {
            Thread.Sleep(1500);
            if (username == "goeran" && password == "hansen")
                return true;
            else
                return false;
        }

        #endregion
    }
}
