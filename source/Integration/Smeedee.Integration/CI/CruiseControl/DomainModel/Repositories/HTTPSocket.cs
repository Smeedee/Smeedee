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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class HTTPSocket:IHTTPSocket
    {
        private TcpClient tcpClient;
        private string hostName;
        private int hostPort;

        /// <summary>
        /// Workaround: Because TcpClient IsConnected property threw nullexcp
        /// after Close method was called. 
        /// </summary>
        private bool isConnected;

        public HTTPSocket()
        {
            tcpClient = new TcpClient();
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        public void Open(string URL, int port)
        {
            tcpClient = new TcpClient();
            Uri uri = new Uri(URL);
            hostPort = port;
            hostName = uri.Host;
            try
            {
                tcpClient.Connect(hostName, hostPort);
                isConnected = tcpClient.Connected;
            } catch(Exception ex)
            {
                isConnected = false;
                throw new HTTPSockectCouldNotConnectException();
            }
            
        }

        public void Send(string data)
        {
            if (!isConnected)
                throw new WebException("Can not Send when not connected");

            NetworkStream stream = tcpClient.GetStream();

            byte[] outgoing = System.Text.Encoding.ASCII.GetBytes(data);

            stream.Write(outgoing,0,outgoing.Length);
        }

        public byte[] Read(int bytesToRead)
        {
            return ReadBytesFromHost(bytesToRead);
        }



        private byte[] ReadBytesFromHost(int requestedMaxLength)
        {
            if (!isConnected) throw new WebException("Can not Read when not connected");
            if (requestedMaxLength < 1) throw new HTTPSockectInvalidReadException();

            NetworkStream stream = tcpClient.GetStream();
            stream.ReadTimeout = 3000; // in ms
            
            var tinyBuffer = new byte[1024];
            int readBytes = 0;
            int tinyRead = 0;
            bool atEndOfFile = false;
            var myCompleteMessage = new StringBuilder();
            
            do
            {
                try
                {
                    tinyRead = stream.Read(tinyBuffer, 0, tinyBuffer.Length);
                }
                catch(Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                    {
                        atEndOfFile = true;
                    } 
                    else
                    {
                        this.Close();
                        throw new HTTPSockectRejectedByHostException();
                    }
                }
                if (!atEndOfFile)
                {
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(tinyBuffer, 0, tinyRead));

                    readBytes += tinyRead;
                }
            }while (!atEndOfFile && (readBytes < requestedMaxLength));

            if(myCompleteMessage.Length > requestedMaxLength)
            {
                return Encoding.ASCII.GetBytes(myCompleteMessage.ToString().Substring(0, requestedMaxLength));
            }

            return Encoding.ASCII.GetBytes(myCompleteMessage.ToString());

        }

        public void Close()
        {
            tcpClient.Close();
            isConnected = false;
        }
    }
}
