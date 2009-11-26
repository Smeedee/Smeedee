using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.Client.Framework.Factories
{
    public class UIInvokerFactory
    {
        public static IInvokeUI UIInvoker = new NoUIInvocation();

        public static IInvokeUI Assemlble()
        {
            return UIInvoker;  
        }
    }
}
