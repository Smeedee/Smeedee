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
using System.Windows.Input;

using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;
using APD.Client.Framework;
using System.Collections.Specialized;
using System.ComponentModel;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class UserdbViewModel : BindableViewModel<UserViewModel>
    {
        private bool dataIsChanged;
        public bool DataIsChanged
        {
            get
            {
                return dataIsChanged;
            }
            set
            {
                if( value != dataIsChanged )
                {
                    dataIsChanged = value;
                    TriggerPropertyChanged<UserdbViewModel>(vm => vm.DataIsChanged);
                }
            }
        }

        public string Name { get; set; }

        public ICommand SaveUserdbUICommand { get; set; }
        public ICommand ReloadUserdbUICommand { get; set; }
        public ICommand EditUICommand { get; private set; }

        public UserdbViewModel(IInvokeUI uiInvoker, ITriggerCommand saveUserDbCommandTriggerer, ITriggerCommand reloadUserdbCommandTriggerer,
            ITriggerEvent<EventArgs> editCommandTrigger) :
            base(uiInvoker)
        {
            this.Name = string.Empty;
            this.Data.CollectionChanged += new NotifyCollectionChangedEventHandler(Data_CollectionChanged);

            SaveUserdbUICommand = new UICommand(saveUserDbCommandTriggerer);

            ReloadUserdbUICommand = new UICommand(reloadUserdbCommandTriggerer);
            EditUICommand = new UICommand(editCommandTrigger);
        }

        void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DataIsChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (UserViewModel item in e.NewItems)
                {
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (UserViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DataIsChanged = true;
        }
        
    }
}
