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
using APD.Client.Services;
using System.Windows.Browser;

namespace APD.Client.Silverlight.Services.Impl
{
    public class CheckIfAdminUIShouldBeDisplayedFromUrl : ICheckIfAdminUIShouldBeDisplayed
    {

        #region ICheckIfAdminUIShouldBeDisplayed Members

        public bool DisplayAdminUI()
        {
            return HtmlPage.Document.QueryString.ContainsKey("view") &&
                HtmlPage.Document.QueryString["view"] == "admin";
        }

        #endregion
    }
}
