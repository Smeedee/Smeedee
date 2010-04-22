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

using GitSharp.Commands;

using NUnit.Framework;

using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;


namespace APD.IntegrationTests.VCS.Git.Learning.SharpSSH
{
    [TestFixture]
    public class SharpSSHLearningTests : SharpSSHContext
    {
        [Test]
        public void how_to_connect_to_a_remote_location()
        {
            var shell = new SshShell(theHostIWantToConnectTo, myUsername);
            shell.Password = "roflsaurus";
            //This statement must be prior to connecting
            shell.RedirectToConsole();

            Console.Write("Connecting...");
            shell.Connect();
            Console.WriteLine("OK");

            System.Threading.Thread.Sleep(500);
            
            Console.Write("Disconnecting...");
            shell.Close();
            Console.WriteLine("OK");
        }
    }
}
