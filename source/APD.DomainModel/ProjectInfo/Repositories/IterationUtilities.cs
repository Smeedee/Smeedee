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
// <author>Jan Fredrik Drabløs</author>
// <email>jaffe88@hotmail.com</email>
// <date>2009-08-13</date>

#endregion

using System.Collections.Generic;
using System.Linq;


namespace APD.DomainModel.ProjectInfo.Repositories
{
    public static class IterationUtilities
    {
        public static IList<Task> MergeTasks(IList<Task> oldTasks, IList<Task> newTasks)
        {
            IList<Task> newlyCreatedTasks = GetNewlyCreatedTasks(oldTasks, newTasks);
            IList<Task> updatedTasks = GetUpdatedTasks(oldTasks, newTasks);
            IList<Task> removedTasks = GetRemovedTasks(oldTasks, updatedTasks);

            var mergedTasks = new List<Task>();
            mergedTasks.AddRange(removedTasks);
            mergedTasks.AddRange(updatedTasks);
            mergedTasks.AddRange(newlyCreatedTasks);

            return mergedTasks;
        }

        public static IList<Task> GetNewlyCreatedTasks(IList<Task> oldTasks, IList<Task> newTasks)
        {
            var newlyCreatedTasks = new List<Task>();
            newlyCreatedTasks.AddRange(newTasks);

            // TODO: Rewrite to LINQ!
            for (int i = 0; i < newlyCreatedTasks.Count; i++)
            {
                Task taskFromMingle = newlyCreatedTasks.ElementAt(i);
                foreach (Task oldTask in oldTasks)
                {
                    if (taskFromMingle.Name == oldTask.Name)
                    {
                        newlyCreatedTasks.Remove(taskFromMingle);
                        i--;
                    }
                }
            }

            return newlyCreatedTasks;
        }

        public static IList<Task> GetUpdatedTasks(IList<Task> oldTasks, IList<Task> newTasks)
        {
            var updatedTasks = new List<Task>();
            foreach (Task newTask in newTasks)
            {
                foreach (Task oldTask in oldTasks)
                {
                    if (newTask.SystemId == oldTask.SystemId)
                    {
                        WorkEffortHistoryItem newHistoryItem = newTask.WorkEffortHistory.Last();
                        WorkEffortHistoryItem oldHistoryItem = oldTask.WorkEffortHistory.Last();

                        if (newHistoryItem.RemainingWorkEffort != oldHistoryItem.RemainingWorkEffort ||
                            newHistoryItem.TimeStampForUpdate.Date != oldHistoryItem.TimeStampForUpdate.Date)
                        {
                            oldTask.AddWorkEffortHistoryItem(newHistoryItem);
                            oldTask.Status = newTask.Status;
                            updatedTasks.Add(oldTask);
                        }
                    }
                }
            }
            return updatedTasks;
        }

        public static IList<Task> GetRemovedTasks(IList<Task> oldTasks, IList<Task> updatedTasks)
        {
            var removedTasks = new List<Task>();
            removedTasks.AddRange(oldTasks);

            foreach (Task oldTask in oldTasks)
            {
                foreach (Task updatedTask in updatedTasks)
                {
                    if (updatedTask.Name == oldTask.Name)
                    {
                        removedTasks.Remove(oldTask);
                    }
                }
            }

            return removedTasks;
        }
    }
}