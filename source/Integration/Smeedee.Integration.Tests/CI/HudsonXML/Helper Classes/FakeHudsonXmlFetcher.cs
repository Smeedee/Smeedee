using System.IO;
using Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    public class FakeHudsonXmlFetcher : IFetchHudsonXml
    {
        public string GetProjects()
        {
            return
                @"<allView>
                      <job>
                        <name>analytics-server</name>
                        <url>http://deadlock.netbeans.org/hudson/job/analytics-server/</url>
                        <color>disabled</color>
                      </job>
                      <job>
                        <name>
                          apitest</name>
                        <url>http://deadlock.netbeans.org/hudson/job/apitest/</url>
                        <color>
                          blue</color>
                      </job>
                    </allView>";
        }

        public string GetBuilds(string projectName)
        {
            return @"<freeStyleProject>
  <name>
    apitest</name>
  <url>
    http://deadlock.netbeans.org/hudson/job/apitest/</url>
  <buildable>
    true</buildable>
  <build>
    <number>
      562</number>
    <url>
      http://deadlock.netbeans.org/hudson/job/apitest/562/</url>
  </build>
  <build>
    <number>
      561</number>
    <url>
      http://deadlock.netbeans.org/hudson/job/apitest/561/</url>
  </build>
  <build>
    <number>
      560</number>
    <url>
      http://deadlock.netbeans.org/hudson/job/apitest/560/</url>
  </build>
      </freeStyleProject>";
        }

        public string GetBuild(string projectName, string buildId)
        {
            return File.OpenText("../../CI/HudsonXML/562.xml").ReadToEnd();
        }

        public string GetUrl()
        {
            return "http://fake";
        }

    }
}
