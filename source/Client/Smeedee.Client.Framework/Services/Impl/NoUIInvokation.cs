using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class NoUIInvokation : IUIInvoker
    {
        public void Invoke(Action a)
        {
            a.Invoke();
        }
    }
}
