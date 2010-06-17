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

using System;
using System.Globalization;
using System.Xml;


namespace Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class XmlParser
    {
        protected XmlDocument xmlDocument;

        public XmlParser(string inputXmlData)
        {
            LoadXml(inputXmlData);
        }

        public XmlParser() {}


        public void LoadXml(string inputXmlData)
        {
            xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(inputXmlData);
            }
            catch (XmlException xe)
            {
                throw new  XmlException("XML-data is malformed.", xe);
            }
        }

        public string GetStringValueFromNode(XmlNode node)
        {
            if (node != null)
            {
                int childnodes = node.ChildNodes.Count;
                if (childnodes == 1)
                    return node.ChildNodes[0].Value;
            }
                
            return "";
        }

        public int GetIntValueFromNode(XmlNode node)
        {
            if (node == null) return 0;

            string value = GetStringValueFromNode(node);
            value = value.Replace(',', '.');

            return Convert.ToInt32(double.Parse(value.Trim(), new CultureInfo("en-US")));
        }

        public DateTime GetDateFromXmlNode(XmlNode dateNode)
        {
            if(dateNode != null)
            {
                string date = GetStringValueFromNode(dateNode);
                return DateTime.Parse(date);
            }
            return new DateTime();
        }

        public int GetTotalResultCount()
        {
            return GetIntValueFromNode(xmlDocument["QueryResult"]["TotalResultCount"]);
        }
    }
}