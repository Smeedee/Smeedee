using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace APD.Client.Framework.ViewModels
{
    public class UiInvokerMock : APD.Client.Framework.IInvokeUI
    {
        public bool WasInvoked { get; set; }

        public UiInvokerMock()
        {
            WasInvoked = false;
        }

        #region IInvokeUI Members

        public void Invoke(Action action)
        {
            WasInvoked = true;
            action.Invoke();
        }

        #endregion
    }
}
