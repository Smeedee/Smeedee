using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class AsyncClient<TReturnType> : IInvokeBackgroundWorker<TReturnType>
    {
        public event EventHandler<AsyncResponseEventArgs<TReturnType>> RunAsyncCompleted;

        public void RunAsync(Func<TReturnType> action)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                var retValue = action.Invoke();
                if (RunAsyncCompleted != null)
                    RunAsyncCompleted(this, new AsyncResponseEventArgs<TReturnType>(retValue));
            });
        }

        public void RunAsyncVoid(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action.Invoke());
        }
    }

    public class AsyncVoidClient : IInvokeBackgroundWorker
    {
        public void RunAsyncVoid(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }
    }

    public class NoBackgroundWorkerInvocation : IInvokeBackgroundWorker
    {
        public void RunAsyncVoid(Action action)
        {
            action();
        }
    }


    public class NoBackgroundWorkerInvocation<TReturnType> : IInvokeBackgroundWorker<TReturnType>
    {
        #region IInvokeBackgroundWorker<TReturnType> Members

        public void RunAsync(Func<TReturnType> action)
        {
            var retValue = action.Invoke();
            if (RunAsyncCompleted != null)
                RunAsyncCompleted(this, new AsyncResponseEventArgs<TReturnType>(retValue));
        }

        public event EventHandler<AsyncResponseEventArgs<TReturnType>> RunAsyncCompleted;

        public void RunAsyncVoid(Action action)
        {
            action.Invoke();
        }

        #endregion
    }
}
