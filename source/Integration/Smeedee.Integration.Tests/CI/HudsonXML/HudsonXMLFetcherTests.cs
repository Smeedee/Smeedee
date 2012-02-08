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

using System.Diagnostics;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    [TestFixture]
    public class HudsonXmlFetcherTests
    {
        private const string PROJECT_NAME = "apitest";
        private HudsonXmlFetcher hudsonXmlFetcher;
        private XmlDocument doc;

        [SetUp]
        public void Setup()
        {
            hudsonXmlFetcher = new HudsonXmlFetcher("http://deadlock.netbeans.org/hudson/", null);
            doc = new XmlDocument();
        }

        [Test]
        [Category("IntegrationTest")]
        //[Ignore("Not working")]
        public void TestGetHudsonProjects()
        {
            var path = hudsonXmlFetcher.GetProjects();
            doc.Load(path);
            var result = doc.SelectNodes("/allView/job");
            result.ShouldNotBeNull();
            if (result != null)
            {
                var count = result.Cast<XmlNode>().Count();
                (count > 0).ShouldBeTrue();
            }
        }

        [Test]
        public void TestGetBuilds()
        {
            doc.LoadXml(hudsonXmlFetcher.GetBuilds(PROJECT_NAME));

            string[] nodes = {"name", "url", "build"};

            foreach (var node in nodes)
            {
                var result = doc.SelectNodes("/freeStyleProject/" + node);
                result.ShouldNotBeNull();
                if (result != null)
                {
                    var count = result.Count;
                    Debug.WriteIf(count==0, node + " was not found.");
                    Assert.Greater(count, 0);
                }                
            }
        }

        [Test]
        public void TestGetBuild()
        {
            doc.LoadXml(hudsonXmlFetcher.GetBuild(PROJECT_NAME, "562"));

            string[] nodes = { "url", "result", "number", "changeSet" };

            foreach (var node in nodes)
            {
                var result = doc.SelectNodes("/freeStyleBuild/" + node);
                result.ShouldNotBeNull();
                if (result != null)
                {
                    var count = result.Count;
                    Debug.WriteIf(count == 0, node + " was not found");
                    Assert.Greater(count, 0);
                }
            }
        }
    }

}
