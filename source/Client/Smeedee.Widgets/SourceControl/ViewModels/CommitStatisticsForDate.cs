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
using System.Diagnostics;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widgets.SourceControl.ViewModels
{

    [DebuggerDisplay("CommitStatisticsForDate ({Date}: {NumberOfCommits}")]
    public class CommitStatisticsForDate : AbstractViewModel, IComparable<CommitStatisticsForDate>
    {
        public DateTime Date { get; set; }
        
        private int _num;
        public int NumberOfCommits
        {
            get { return _num; }
            set
            {
                if (value != _num)
                {
                    _num = value;
                    TriggerPropertyChanged("NumberOfCommits");
                }
            }
        }

        #region IComparable<CommitStatisticsForDate> Members

        public int CompareTo(CommitStatisticsForDate other)
        {
            return Date.CompareTo(other.Date);
        }

        #endregion
    }
}
