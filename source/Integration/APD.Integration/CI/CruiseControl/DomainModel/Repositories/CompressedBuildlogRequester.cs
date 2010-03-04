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
using System.IO.Compression;

using APD.Integration.Framework.Utils;


namespace APD.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class CompressedBuildlogRequester: IHTTPRequest
    {
        public string Request(string fileURL, int port)
        {
            if(port != 80)
                throw new Exception("Only port 80 is supported");

            var xmlResponse = NetUtilities.CreateRequestWithCompressionSupportAndGetResponse(fileURL);

            var xmlData = string.Empty;

            var buffer = DecompressGzip(xmlResponse.GetResponseStream());
            xmlData = System.Text.Encoding.UTF7.GetString(buffer);

            xmlResponse.Close();
            return xmlData;
        }



        /// <summary>
        /// MEthod copied from 
        /// 
        /// http://www.know24.net/blog/Decompress+GZip+Deflate+HTTP+Responses.aspx
        /// </summary>
        /// <param name="streamInput"></param>
        /// <returns></returns>
        private static byte[] DecompressGzip(Stream streamInput)
        {
            Stream streamOutput = new MemoryStream();

            int iOutputLength = 0;

            try
            {

                byte[] readBuffer = new byte[4096];
                /// read from input stream and write to gzip stream
                using (GZipStream streamGZip = new GZipStream(streamInput, CompressionMode.Decompress))
                {
                    int i;

                    while ((i = streamGZip.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        streamOutput.Write(readBuffer, 0, i);
                        iOutputLength = iOutputLength + i;
                    }
                }
            }

            catch (Exception ex)
            {

                // todo: handle exception

            }
            /// read uncompressed data from output stream into a byte array

            byte[] buffer = new byte[iOutputLength];

            streamOutput.Position = 0;

            streamOutput.Read(buffer, 0, buffer.Length);

            return buffer;
        }
    }
}
