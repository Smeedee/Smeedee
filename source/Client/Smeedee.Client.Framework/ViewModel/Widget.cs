using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Widget
    {
        public virtual void OnInitialize()
        {   
            ErrorInfo = new ErrorInfo();
        }

        protected void ReportFailure(string message)
        {
            ErrorInfo.HasError = true;
            ErrorInfo.ErrorMessage = message;
        }

        protected void NoFailure()
        {
            ErrorInfo.HasError = false;
            ErrorInfo.ErrorMessage = string.Empty;
        }
    }
}
