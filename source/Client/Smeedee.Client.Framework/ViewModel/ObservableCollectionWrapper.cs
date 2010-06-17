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
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;

namespace Smeedee.Client.Framework.ViewModel
{
    /// <summary>
    /// This is work in progress.
    /// 
    /// Todo: refactor to remove the AbstractViewModel and ItemViewModelWrapper dependencies
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    /// <typeparam name="TWrapperItemType"></typeparam>
    public class ObservableCollectionWrapper<TItemType, TWrapperItemType> : ObservableCollection<TWrapperItemType> where TItemType : AbstractViewModel where TWrapperItemType : ItemViewModelWrapper<TItemType>
    {
        private ObservableCollection<TItemType> dataSource;
        private bool addingItemToMediatorFlag;

        public ObservableCollectionWrapper(ObservableCollection<TItemType> dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            this.dataSource = dataSource;

            this.CollectionChanged += new NotifyCollectionChangedEventHandler(Data_CollectionChanged);
        
            this.dataSource.CollectionChanged += new NotifyCollectionChangedEventHandler(ViewModelData_CollectionChanged);

            LoadDataFromViewModel();
        }

        private void LoadDataFromViewModel()
        {
            StopObservingData();
            foreach (var item in dataSource)
                Add(
                    (TWrapperItemType)
                    Activator.CreateInstance(typeof (TWrapperItemType), (TItemType) item));
            StartObservingData();
        }

        private void StopObservingData()
        {
            addingItemToMediatorFlag = true;
        }

        private void StartObservingData()
        {
            addingItemToMediatorFlag = false;
        }

        void ViewModelData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!ObservingData())
            {
                StopObservingData();
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var newItem in e.NewItems)
                        Add((TWrapperItemType) Activator.CreateInstance(typeof(TWrapperItemType), (TItemType)newItem));
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var removedItem in e.OldItems)
                    {
                        var qMediator = this.Where(i => i.ViewModel.Equals(removedItem));
                        if (qMediator.Count() > 0)
                            Remove(qMediator.Single());
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.Clear();
                }
                StartObservingData();
                
            }
        }

        private bool ObservingData()
        {
            return addingItemToMediatorFlag;
        }

        void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!ObservingData())
            {
                StopObservingData();
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var newItem in e.NewItems)
                        dataSource.Add(((TWrapperItemType)newItem).ViewModel);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var removedItem in e.OldItems)
                        dataSource.Remove(((TWrapperItemType)removedItem).ViewModel);
                }
                StartObservingData();
            }
        }

    }
}
