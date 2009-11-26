//#region File header

//// <copyright>
//// This library is free software; you can redistribute it and/or
//// modify it under the terms of the GNU Lesser General Public
//// License as published by the Free Software Foundation; either
//// version 2.1 of the License, or (at your option) any later version.
//// 
//// This library is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//// Lesser General Public License for more details.
//// 
//// You should have received a copy of the GNU Lesser General Public
//// License along with this library; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//// /copyright> 
//// 
//// <contactinfo>
//// The project webpage is located at http://agileprojectdashboard.org/
//// which contains all the neccessary information.
//// </contactinfo>

//#endregion

//using APD.DomainModel.ProjectInfo;
//using FluentNHibernate.Mapping;


//namespace APD.DomainModel.NHMapping.Entities
//{
//    public class ProjectInfoServerMap : ClassMap<ProjectInfoServer>
//    {
//        public ProjectInfoServerMap()
//        {
//            Id(x => x.Url)
//                .GeneratedBy.Assigned();

//            Map(x => x.Name);

//            HasMany<Project>(x => x.Projects)
//                .Cascade.All()
//                .Not.LazyLoad()
//                .AsList();
//        }
//    }

//    public class ProjectMap : ClassMap<Project>
//    {
//        public ProjectMap()
//        {
//            Id(x => x.Id).GeneratedBy.Guid();

//            Map(x => x.Name);

//            References(x => x.Server)
//                .Not.LazyLoad()
//                .Cascade.All();

//            HasMany<Iteration>(x => x.Iterations)
//                .Cascade.All()
//                .Not.LazyLoad()
//                .AsList();
//        }
//    }

//    public class IterationMap : ClassMap<Iteration>
//    {
//        public IterationMap()
//        {
//            Id(x => x.Id).GeneratedBy.Guid();

//            References(x => x.Project)
//                .Not.LazyLoad()
//                .Cascade.All();

//            Map(x => x.Name);
//            Map(x => x.StartDate);
//            Map(x => x.EndDate);

//            HasMany<Task>(x => x.Tasks)
//                .Cascade.All()
//                .Not.LazyLoad();
//        }
//    }


//    public class TaskMap : ClassMap<Task>
//    {
//        public TaskMap()
//        {
//            Id(x => x.Id).GeneratedBy.Guid();

//            References(x => x.Iteration)
//                .Not.LazyLoad()
//                .Cascade.All();

//            Map(x => x.Status);
//            Map(x => x.Name);
//            Map(x => x.WorkEffortEstimate);

//            HasMany<WorkEffortHistoryItem>(x => x.WorkEffortHistory)
//                .Cascade.All()
//                .Not.LazyLoad();
//        }
//    }

//    public class WorkEffortHistoryItemMap : ClassMap<WorkEffortHistoryItem>
//    {
//        public WorkEffortHistoryItemMap()
//        {
//            Id(x => x.TimeStampForUpdate).GeneratedBy.Assigned();

//            //CompositeId()
//            //    .KeyProperty(x => x.TimeStampForUpdate)
//            //    .KeyReference(x => x.Task, "key_task_id")
//            //    .KeyReference(x => x.Task, "key_task_iteration_id")
//            //    .KeyReference(x => x.Task, "key_task_iteration_server_id")
//            //    .KeyReference(x => x.Task, "key_task_iteration_project_id");

//            References(x => x.Task)
//                .Not.LazyLoad()
//                .Cascade.All();

//            Map(x => x.RemainingWorkEffort);
//        }
//    }
//}
        