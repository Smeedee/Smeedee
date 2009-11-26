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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using APD.DomainModel.SourceControl;

using Microsoft.TeamFoundation.VersionControl.Client;


namespace APD.Integration.VCS.TFSVC.DomainModel.Repositories
{
    public class QueryModel
    {
        public ChangesetVersionSpec RevisionFrom { get; set; }
        public VersionSpec RevisionTo { get; set; }
        public int MaxRevisions { get; set; }
        public Author Author { get; set; }
        public bool IncludeChanges { get; set; }
        public int DeletionId { get; set; }
        public bool SlotMode { get; set; }
        public RecursionType RecursionType { get; set; }

        public QueryModel()
        {
            RevisionFrom = new ChangesetVersionSpec(1);
            RevisionTo = VersionSpec.Latest;
            MaxRevisions = int.MaxValue;
            Author = new Author();
            IncludeChanges = true;
            DeletionId = 0;
            SlotMode = false;
            RecursionType = RecursionType.Full;
        }
    }
}