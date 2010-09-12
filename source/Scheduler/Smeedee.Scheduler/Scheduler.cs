
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Scheduler.Services;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Scheduler
{
    public class Scheduler
    {
        private const int TASK_FAILURE_LIMIT = 3;
        private const int TASK_FAILURE_COOLDOWN_INTERVAL_S = 60 * 10;

        private readonly IDictionary<string, Type> taskNameToTypeMappings;
        private readonly string taskDirectory;
        private readonly IFileUtilities fileUtility;
        private readonly IocContainerForScheduler iocContainer;
        private readonly ILog logger;
        private readonly ITimerWithTimestamp taskConfigurationTimer;
        private readonly ITimerWithTimestamp dispatchTimer;
        private IList<TaskConfiguration> taskConfigurations;

        public IEnumerable<TaskBase> InstantiatedTasks
        {
            get
            {
                return TaskMetaDatas.Select(t => t.Task);
            }
        }

        public IList<TaskMetaData> TaskMetaDatas { get; private set; }

        private DateTime startDateTime;
        public DateTime StartDateTime
        {
            get { return (startDateTime == DateTime.MinValue) ? DateTime.Now : startDateTime; }
            set { startDateTime = value; }
        }

        public Scheduler(
            IocContainerForScheduler iocContainer, 
            string taskDirectory, 
            IFileUtilities fileUtility, 
            ITimerWithTimestamp taskConfigurationTimer, 
            ITimerWithTimestamp dispatchTimer, 
            ILog logger)
        {
            taskNameToTypeMappings = new Dictionary<string, Type>();
            TaskMetaDatas = new List<TaskMetaData>();
            this.logger = logger;
            this.iocContainer = iocContainer;
            this.taskDirectory = taskDirectory;
            this.fileUtility = fileUtility;
            this.taskConfigurationTimer = taskConfigurationTimer;
            this.taskConfigurationTimer.Elapsed += (o, e) => GatherTaskConfigurations();
            this.dispatchTimer = dispatchTimer;
            this.dispatchTimer.Elapsed += (o, timerElapsedEventArgs) => DispatchTasks(timerElapsedEventArgs.Time);
        }

        public void Start()
        {
            WriteInfoToLog("Scheduler started at " + DateTime.Now);
            GatherTaskConfigurations();
            StartTimers();
        }

        private void GatherTaskConfigurations()
        {
            RegisterTaskConfigurationsFromRepository();
            UpdateListOfInstantiatedTasks();
        }

        private void RegisterTaskConfigurationsFromRepository()
        {
            GetTaskConfigurations(); 
            MapTaskNamesToTypes();
        }

        private void GetTaskConfigurations()
        {
            var configRepo = iocContainer.Get<IRepository<TaskConfiguration>>();
            taskConfigurations = configRepo.Get(new AllSpecification<TaskConfiguration>()).ToList(); 
        }

        private void MapTaskNamesToTypes()
        {
            var availableTaskTypes = FetchAvailableTaskTypes();

            foreach (var taskType in availableTaskTypes)
            {
                var taskAttribute = taskType.GetCustomAttributes(typeof (TaskAttribute), false).FirstOrDefault() as TaskAttribute;
                if( taskAttribute != null )
                    taskNameToTypeMappings[taskAttribute.Name] = taskType;
            }
        }

        private IEnumerable<Type> FetchAvailableTaskTypes()
        {
            var availableTaskTypes = new List<Type>();

            try
            {
                availableTaskTypes.AddRange(fileUtility.FindImplementationsOrSubclassesOf<TaskBase>(taskDirectory));
            }
            catch (Exception e)
            {
                WriteErrorToLog(e);
            }

            return availableTaskTypes;
        }

        private void UpdateListOfInstantiatedTasks()
        {
            RemoveUnreferencedTasks();

            foreach (var config in taskConfigurations.Where(TaskCanBeCreated))
            {
                if (TaskAlreadyExists(config))
                    UpdateExistingTaskMetaData(config);
                else
                    AddNewTask(config);
            }
        }

        private void RemoveUnreferencedTasks()
        {
            var configNames = taskConfigurations.Select(t => t.Name);
            TaskMetaDatas = TaskMetaDatas.TakeWhile(t => configNames.Contains(t.InstanceName)).ToList();
        }

        private bool TaskCanBeCreated(TaskConfiguration configuration)
        {
            return taskNameToTypeMappings.ContainsKey(configuration.TaskName);
        }

        private bool TaskAlreadyExists(TaskConfiguration config)
        {
            return TaskMetaDatas.Select(t => t.InstanceName).Contains(config.Name);
        }

        private void UpdateExistingTaskMetaData(TaskConfiguration config)
        {
            try
            {
                var taskMetaData = TaskMetaDatas.Single(t => t.InstanceName == config.Name);
                taskMetaData.Task = CreateTask(taskNameToTypeMappings[config.TaskName], config);
            }
            catch (Exception e)
            {
                WriteErrorToLog(e);
            }
        }

        private TaskBase CreateTask(Type type, TaskConfiguration configuration)
        {
            iocContainer.BindTo<TaskBase>(type);
            iocContainer.BindToConstant(configuration);
            
            return iocContainer.Get<TaskBase>();
        }

        private void AddNewTask(TaskConfiguration config)
        {
            try
            {
                TaskMetaDatas.Add(new TaskMetaData
                {
                    Task = CreateTask(taskNameToTypeMappings[config.TaskName], config),
                    InstanceName = config.Name,
                    LastDispatch = DateTime.MinValue,
                    IsRunning = false
                });
            }
            catch (Exception e)
            {
                WriteErrorToLog(e);
            }  
        }

        private void DispatchTasks(DateTime currentTime)
        {
            foreach (var taskInfo in TaskMetaDatas.Where(t => TaskIsDueToDispatch(t, currentTime)))
            {
                WriteInfoToLog("Dispatching '" + taskInfo.Task.Name + ": " + taskInfo.InstanceName + "'");
                taskInfo.IsRunning = true;

                var threadedTaskInfo = taskInfo; //Note: Do NOT remove this reference.
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    threadedTaskInfo.LastDispatch = currentTime;
                    try
                    {
                        threadedTaskInfo.Task.Execute();
                        threadedTaskInfo.FailureCounter = 0;
                    }
                    catch (TaskConfigurationException configException)
                    {
                        logger.WriteEntry(new WarningLogEntry {Message = configException.Message, Source = "Scheduler"});
                    }
                    catch (Exception ex)
                    {
                        HandleTaskException(threadedTaskInfo, ex);
                    }
                    finally
                    {
                        threadedTaskInfo.IsRunning = false;
                    }
                });
            }
        }

        private bool TaskIsDueToDispatch(TaskMetaData taskMetaData, DateTime currentTime)
        {
            bool isInCooldownMode = taskMetaData.CooldownPoint > currentTime;
            DateTime nextDispatchTime = taskMetaData.LastDispatch + taskMetaData.Task.Interval;
            bool hasNotPassedDueTime = nextDispatchTime > currentTime;

            if (taskMetaData.Task == null || taskMetaData.IsRunning  || isInCooldownMode || hasNotPassedDueTime)
            {
                return false;
            }
            
            return true;
        }

        private void HandleTaskException(TaskMetaData taskMetaData, Exception ex)
        {
            taskMetaData.FailureCounter++;

            string logMessage = "The '" + taskMetaData.InstanceName + "' instance of '" +
                                taskMetaData.Task.Name +
                                "' threw an exception. Attempt number " + taskMetaData.FailureCounter +
                                ". Exception details: \r\n" + ex;

            if (taskMetaData.FailureCounter >= TASK_FAILURE_LIMIT)
            {
                PutTaskIntoCooldown(taskMetaData);
                taskMetaData.FailureCounter = 0;
                logMessage = "Going into Cooldown, will resume at " +
                         taskMetaData.CooldownPoint + ". " + logMessage;
            }

            WriteErrorToLog(logMessage);
        }

        private void PutTaskIntoCooldown(TaskMetaData taskMetaData)
        {
            taskMetaData.CooldownPoint = DateTime.Now + new TimeSpan(0, 0, TASK_FAILURE_COOLDOWN_INTERVAL_S);
        }

        private void StartTimers()
        {
            taskConfigurationTimer.Start();
            dispatchTimer.Start();
        }

        private void WriteInfoToLog(object message)
        {
            if (logger != null)
                logger.WriteEntry(new InfoLogEntry("Scheduler", message.ToString()));
        }

        private void WriteErrorToLog(object message)
        {
            if (logger != null)
                logger.WriteEntry(new ErrorLogEntry("Scheduler", message.ToString()));
        }
    }
}
