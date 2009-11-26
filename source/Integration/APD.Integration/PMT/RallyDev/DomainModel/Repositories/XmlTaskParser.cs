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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class XmlTaskParser : XmlParser
    {
        public XmlTaskParser(string xmlData)
            : base(xmlData) {}

        public XmlTaskParser() {}


        public Task ParseTaskWithData()
        {
            XmlNode taskNode = xmlDocument["Task"];

            var task = new Task();
            XmlNode taskName = taskNode.Attributes.GetNamedItem("refObjectName");
            XmlElement taskEstimateNode = xmlDocument["Task"]["Estimate"];
            XmlElement taskStateNode = xmlDocument["Task"]["State"];
            XmlElement creationDateNode = xmlDocument["Task"]["CreationDate"];
            XmlElement idNode = xmlDocument["Task"]["ObjectID"];

            task.Name = GetStringValueFromNode(taskName);
            task.WorkEffortEstimate = GetIntValueFromNode(taskEstimateNode);
            task.Status = GetStringValueFromNode(taskStateNode);
            task.SystemId = GetStringValueFromNode(idNode);
            
            var workEffortHistoryItem = new WorkEffortHistoryItem(
                GetIntValueFromNode(taskEstimateNode), GetDateFromXmlNode(creationDateNode));

            task.AddWorkEffortHistoryItem(workEffortHistoryItem);

            return task;
        }

        public string ParseRevisionHistoryReference()
        {
            XmlNode revisionHistoryNode = xmlDocument["Task"]["RevisionHistory"];
            XmlNode iterationName = revisionHistoryNode.Attributes.GetNamedItem("ref");

            return GetStringValueFromNode(iterationName);
        }

        public WorkEffortHistoryItem ParseWorkEffortHistoryItem ()
        {
            XmlNode node = xmlDocument["Task"]["ToDo"];
            return new WorkEffortHistoryItem(GetIntValueFromNode(node), DateTime.Now);
        }

        public List<WorkEffortHistoryItem> ParseWorkEffortHistoryFromRevisionHistory()
        {
            XmlNode revisions = xmlDocument["RevisionHistory"]["Revisions"];
            List<WorkEffortHistoryItem> workEffortHistoryItems = new List<WorkEffortHistoryItem>();

            foreach (XmlNode revistion in revisions)
            {
                string description = GetStringValueFromNode(revistion["Description"]);
                if(description.Contains("TO DO changed from"))
                {
                    WorkEffortHistoryItem workEffortHistoryItem = new WorkEffortHistoryItem();
                    workEffortHistoryItem.TimeStampForUpdate = GetDateFromXmlNode(revistion["CreationDate"]);
                    workEffortHistoryItem.RemainingWorkEffort = ParseRemainingWorkEffortFromDecription(description);
                    
                    if(workEffortHistoryItem.RemainingWorkEffort != -1)
                        workEffortHistoryItems.Add(workEffortHistoryItem);
                }
            }
            return workEffortHistoryItems;
        }

        private static int ParseRemainingWorkEffortFromDecription(string description)
        {
            MatchCollection matches = Regex.Matches(description, @"TO DO changed from \[[^\]]+?\] to \[(?<remaining>[^\.]+?.[\d]+?) Hours\]", RegexOptions.Singleline);

            if (matches.Count > 0)
            {
                string matchValue = matches[0].Groups["remaining"].Value.Trim();
                return (int)Convert.ToDecimal(matchValue, System.Globalization.CultureInfo.InvariantCulture);  //Todo: Remove when domain is changed to float
            }
            else
                return -1;
        }
    }
}