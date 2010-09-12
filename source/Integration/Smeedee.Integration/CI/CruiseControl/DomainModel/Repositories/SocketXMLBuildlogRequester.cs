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
using System.Text.RegularExpressions;


namespace Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class SocketXMLBuildlogRequester : IHTTPRequest
    {
        private const int DEFAULT_BYTES_TO_FETCH = 5000;

        public string Request(string fileURL, int port)
        {
            return ReadDataFromSocket(fileURL, port, DEFAULT_BYTES_TO_FETCH);
        }

        private string ReadDataFromSocket(string buildLogXmlUrl, int port, int fetchLogSize)
        {
            if (fetchLogSize > 160000)
                throw new Exception("Read data request was too large");

            var webResult = "";
            var request = new OptimizedHTTPRequest(new HTTPSocket())
                              {
                                  CooldownMS = 5000,
                                  NumberOfRetries = 10
                              };
            webResult = request.RequestURL(buildLogXmlUrl, port, fetchLogSize);

            if (string.IsNullOrEmpty(webResult))
                throw new Exception("HTTP request did not return any data");


            if (Regex.IsMatch(webResult, @"\<build .*\>") == false)
            {
                return ReadDataFromSocket(buildLogXmlUrl, port, fetchLogSize * 2);
            }

            var xmlData = Regex.Match(webResult, ".+(<cruisecontrol.*<build .+?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;

            xmlData += " </build>\n</cruisecontrol>\n";

            return xmlData;
        }
    }
}