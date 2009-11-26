#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;


namespace APD.Client.Framework
{
    public interface IInvokeBackgroundWorker<TReturnType>
    {
        void RunAsync(Func<TReturnType> action);
        event EventHandler<AsyncResponseEventArgs<TReturnType>> RunAsyncCompleted;
        void RunAsyncVoid(Action action);
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