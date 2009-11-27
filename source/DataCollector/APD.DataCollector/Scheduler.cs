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
using System.Threading;

using APD.DomainModel.Framework.Logging;
using APD.Harvester.Framework;


namespace APD.DataCollector
{
    public class Scheduler : IScheduler, IDisposable
    {
        private class HarversterMetaData
        {
            public AbstractHarvester Harvester { get; set; }
            public DateTime LastDispatch { get; set; }
            public bool IsRunning { get; set; }
            public int FailureCounter { get; set; }
            public DateTime CooldownPoint { get; set; }
        }

        public event Action OnFailedDispatch;

        private const int HARVESTER_FAILURE_LIMIT = 3;
        private const int HARVESTER_FAILURE_COOLDOWN_INTERVAL_S = 60 * 10;
        private const int CHECK_DISPATCH_INTERVAL_MS = 500;

        private ILog logger;
        private List<HarversterMetaData> harvesterInformation;
        private Timer dispatchWorkerTimer;


        public Scheduler(ILog logger)
        {
            this.logger = logger;

            harvesterInformation =  new List<HarversterMetaData>();

            dispatchWorkerTimer = new Timer(new TimerCallback(DispatchHarvesters), null, CHECK_DISPATCH_INTERVAL_MS, CHECK_DISPATCH_INTERVAL_MS);

            WriteInfoToLog("Scheduler loaded at " + DateTime.Now);
        }

        private void DispatchHarvesters(object state)
        {

            foreach (var harvesterInfo in harvesterInformation)
            {
                if(!IsHarvesterDueToDispatch(harvesterInfo))
                    continue;
                
                WriteInfoToLog("Dispatching '" + harvesterInfo.Harvester.Name + "'");
                harvesterInfo.IsRunning = true;

                var infoHolder = harvesterInfo;//Note: Do NOT remove this referance.
                ThreadPool.QueueUserWorkItem((o) => {
                    infoHolder.LastDispatch = DateTime.Now;
                    try
                    {
                        infoHolder.Harvester.DispatchDataHarvesting();
                        infoHolder.FailureCounter = 0;
                    }
                    catch(Exception ex)
                    {
                        InvokeOnFailedDispatch();
                        HandleHarvesterException(infoHolder, ex);
                    }
                    finally
                    {
                        infoHolder.IsRunning = false;                            
                    }
                });
            }
        }


        private bool IsHarvesterDueToDispatch(HarversterMetaData harversterMetaData)
        {
            bool isInCooldownMode = harversterMetaData.CooldownPoint > DateTime.Now;
            DateTime nextDispatchTime = harversterMetaData.LastDispatch + harversterMetaData.Harvester.Interval;
            bool hasNotPassedDueTime = nextDispatchTime > DateTime.Now;

            if (harversterMetaData.Harvester == null
                || harversterMetaData.IsRunning
                || isInCooldownMode
                || hasNotPassedDueTime)
            {
                return false;
            }
            else
                return true;
        }


        /// <summary>
        /// This will be done in a more dynamic way. Bellongs to another Task.
        /// I believe tuxbear will look at registering harvesters very soon.
        /// This is a temporary workaround to allow tests to be run.
        /// </summary>
        public void RegisterHarvesters(IEnumerable<AbstractHarvester> harvesters)
        {
            foreach (AbstractHarvester harvester in harvesters)
            {
                var harvesterMeta = new HarversterMetaData()
                                    {
                                        Harvester = harvester,
                                        LastDispatch = DateTime.MinValue,
                                        IsRunning = false
                                    };
                harvesterInformation.Add(harvesterMeta);
            }
        }



        private void HandleHarvesterException(HarversterMetaData harversterMetaData, Exception ex)
        {
            harversterMetaData.FailureCounter++;

            string logMessage = "Harvester '" + harversterMetaData.Harvester.Name + 
                                "' threw an exception. Attempt number " + harversterMetaData.FailureCounter +
                                ". Exception details: \r\n" + ex.ToString();
            
            
            if (harversterMetaData.FailureCounter >= HARVESTER_FAILURE_LIMIT)
            {
                harversterMetaData.CooldownPoint = DateTime.Now + new TimeSpan(0, 0, HARVESTER_FAILURE_COOLDOWN_INTERVAL_S);
                harversterMetaData.FailureCounter = 0;
                logMessage = "Going into Cooldown, will resume at " +
                    harversterMetaData.CooldownPoint + ". " + logMessage;
            }
            
            WriteWarningToLog(logMessage);
        }


        private void WriteInfoToLog(object message)
        {
            if (logger != null)
                logger.WriteEntry(new InfoLogEntry("Scheduler", message.ToString()));
        }

        private void WriteWarningToLog(object message)
        {
            if (logger != null)
                logger.WriteEntry(new WarningLogEntry("Scheduler", message.ToString()));
        }

        private void WriteErrorToLog(object message)
        {
            if (logger != null)
                logger.WriteEntry(new ErrorLogEntry("Scheduler", message.ToString()));
        }

        private void InvokeOnFailedDispatch()
        {
            Action dispatch = OnFailedDispatch;
            if (dispatch != null)
            {
                dispatch();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            dispatchWorkerTimer.Dispose();
        }

        #endregion
    }
}
