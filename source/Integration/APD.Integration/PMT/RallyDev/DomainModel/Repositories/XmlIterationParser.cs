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

using System.Collections.Generic;
using System.Xml;
using APD.DomainModel.ProjectInfo;


namespace APD.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class XmlIterationParser : XmlParser
    {
        public XmlIterationParser(string xmlData)
            : base(xmlData) { }

        public XmlIterationParser() { }


        public List<string> ParseTaskReferencesForIteration()
        {
            XmlNode iterationNode = xmlDocument["QueryResult"]["Results"];
            List<string> tasks = new List<string>();

            foreach (XmlNode taskNode in iterationNode)
            {
                XmlNode taskReference = taskNode.Attributes.GetNamedItem("ref");
                tasks.Add(GetStringValueFromNode(taskReference));
            }

            return tasks;
        }

        public Iteration ParseIteration()
        {
            Iteration iteration = new Iteration();
            XmlNode iterationNode = xmlDocument["Iteration"];

            if (iterationNode != null)
            {

                var iterationName = iterationNode.Attributes.GetNamedItem("refObjectName");

                var startDateNode = iterationNode["StartDate"];
                var endDateNode = iterationNode["EndDate"];
                var idNode = iterationNode["ObjectID"];

                iteration.Name = GetStringValueFromNode(iterationName);
                iteration.StartDate = GetDateFromXmlNode(startDateNode);
                iteration.EndDate = GetDateFromXmlNode(endDateNode);
                iteration.SystemId = GetStringValueFromNode(idNode);

            }
            return iteration;
        }
    }
}