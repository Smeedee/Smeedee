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
using System.IO;
using System.Net;


namespace APD.Integration.Framework.Utils
{
    public static class NetUtilities
    {
        /// <summary>
        /// Download the content from a webpage as a string given a URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Content from the webpage as a string.</returns>
        public static string DownloadContentAsString(string url)
        {
            var response = CreateRequestAndGetResponse(url);
            var pageData = GetContentAsString(response);

            return pageData;
        }

        public static HttpWebResponse CreateRequestAndGetResponse(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            var response = (HttpWebResponse) request.GetResponse();

            return response;
        }

        public static HttpWebResponse CreateRequestWithCompressionSupport(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            var response = (HttpWebResponse)request.GetResponse();

            return response;
        }

        public static string GetContentAsString(HttpWebResponse httpWebResponse)
        {
            var contents = String.Empty;

            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                contents = streamReader.ReadToEnd();
            }

            return contents;
        }
    }
}