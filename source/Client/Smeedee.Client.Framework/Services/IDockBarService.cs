using System.Collections.Generic;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services
{
    public interface IDockBarService
    {
        /// <summary>
        /// Show the DockBar in the user interface.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the DockBar in the user interface.
        /// </summary>
        void Hide();

        /// <summary>
        /// Enumerate over all the items in the DockBar
        /// </summary>
        IEnumerable<DockBarItem> Items { get; }

        /// <summary>
        /// Add an new item to the DockBar
        /// </summary>
        /// <param name="item"></param>
        void AddItem(DockBarItem item);

        /// <summary>
        /// Remove an existing item from the DockBar
        /// </summary>
        /// <param name="item"></param>
        void RemoveItem(DockBarItem item);
    }
}
