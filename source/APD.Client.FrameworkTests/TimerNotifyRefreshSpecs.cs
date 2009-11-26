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
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

#region Usings

using System.Threading;

using APD.Client.Framework;

using NUnit.Framework;


#endregion

namespace APD.Client.FrameworkTests
{
    [TestFixture]
    public class TimerNotifyRefreshSpecs
    {
        private const int TEST_INTERVAL = 500;
        private const double ERROR_MARGIN = 0.20;
        
        private bool testTimerHasFinished;


        private void testTimerFinishedCallback(object state, RefreshEventArgs e)
        {
            testTimerHasFinished = true;
        }

        [Test]
        public void Should_Not_Fire_Too_Early()
        {
            testTimerHasFinished = false;

            var testTimer = new TimerNotifyRefresh(TEST_INTERVAL, false);
            testTimer.Refresh += testTimerFinishedCallback;

            Thread.Sleep((int) ( TEST_INTERVAL*( 1-ERROR_MARGIN ) ));

            Assert.IsFalse(testTimerHasFinished);
        }

        [Test]
        public void Should_Not_Fire_Too_Late()
        {
            testTimerHasFinished = false;

            var testTimer = new TimerNotifyRefresh(TEST_INTERVAL,false);
            testTimer.Refresh += testTimerFinishedCallback;

            Thread.Sleep((int) ( TEST_INTERVAL*( 1+ERROR_MARGIN ) ));

            Assert.IsTrue(testTimerHasFinished);
        }
    }
}