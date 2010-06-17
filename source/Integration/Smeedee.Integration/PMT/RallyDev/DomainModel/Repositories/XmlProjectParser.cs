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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections.Generic;
using System.Xml;
using Smeedee.DomainModel.ProjectInfo;


namespace Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class XmlProjectParser : XmlParser
    {
        public XmlProjectParser(string xmlData)
            : base(xmlData) {}

        public XmlProjectParser() {}


        public Project ParseProject()
        {
            XmlNode projectNode = xmlDocument["QueryResult"]["Results"]["Object"];
            XmlNode projectNameNode = projectNode.Attributes.GetNamedItem("refObjectName");

            return new Project(GetStringValueFromNode(projectNameNode));
        }
        
       public List<string> ParseIterationReferencesForProject()
        {
            XmlNode projectNode = xmlDocument["QueryResult"]["Results"];
            List<string> iterationReferences = new List<string>();

            foreach (XmlNode iterationNode in projectNode)
            {
                XmlNode iterationReference = iterationNode.Attributes.GetNamedItem("ref");
                iterationReferences.Add(GetStringValueFromNode(iterationReference));
            }

            return iterationReferences;
        }

        public List<string> ParseProjectNames()
        {
            XmlNode projects = xmlDocument["QueryResult"]["Results"];
            List<string> projectNames = new List<string>();

            foreach (XmlNode projectNode in projects)
            {
                XmlNode iterationReference = projectNode.Attributes.GetNamedItem("refObjectName");
                projectNames.Add(GetStringValueFromNode(iterationReference));
            }

            return projectNames;
        }

        public string ParseProjectReference()
        {
            XmlNode projectNode = xmlDocument["QueryResult"]["Results"]["Object"];
            if (projectNode != null)
            {
                return GetStringValueFromNode(projectNode.Attributes.GetNamedItem("ref"));
            }
            return null;
        }

        public string ParseIdFromReference()
        {
            var idNode = xmlDocument["Project"]["ObjectID"];
            return GetStringValueFromNode(idNode);
        }
    }
}