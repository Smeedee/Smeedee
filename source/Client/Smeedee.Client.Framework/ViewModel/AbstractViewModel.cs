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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Smeedee.Client.Framework.Services.Impl;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.ViewModel
{
    public class AbstractViewModel : INotifyPropertyChanged
    {
        public IUIInvoker Invoker { get; private set; }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if( value != isLoading)
                {
                    isLoading = value;
                    TriggerPropertyChanged<AbstractViewModel>(vm => vm.IsLoading);
                }
            }
        }

        private bool hasConnectionProblems;
        public bool HasConnectionProblems
        {
            get { return hasConnectionProblems; }
            set
            {
                if( value != hasConnectionProblems )
                {
                    hasConnectionProblems = value;
                    TriggerPropertyChanged<AbstractViewModel>(vm => vm.HasConnectionProblems);
                }
            }
        }

        public AbstractViewModel()
        {
            //TODO: THis is a temporary hack to work around having to bootstrap a servicelocator for every test
            if( ServiceLocator.Instance == null )
            {
                Invoker = new NoUIInvokation();
            }
            else
            {
            this.Invoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();    
            }
            
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void TriggerPropertyChanged<T>(Expression<Func<T, Object>> exp)
        {
            string propertyName;
            if (exp.Body is UnaryExpression)
                propertyName = ((MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            else
                propertyName = ((MemberExpression)exp.Body).Member.Name;

            if (PropertyChanged != null)
            {
                Invoker.Invoke(() =>
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
                );
            }
        }
    }
}
