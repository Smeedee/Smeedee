using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using APD.DomainModel.CI;
using APD.Integration.Framework.Atom.DomainModel.Factories;


namespace APD.Integration.CI.TeamCity.DomainModel.Factories
{
    public class TeamCityBuildAtomEntryFactory : AtomEntryFactory<Build>
    {
        #region AtomEntryFactory<Build> Members

        public Build Assemble(XElement entry)
        {
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            BuildContext context = ParseBuildTitle(entry.Element(atomNs + "title").Value);

            var build = new Build
            {
                FinishedTime = Convert.ToDateTime(entry.Element(atomNs + "updated").Value).ToLocalTime(),
                StartTime = Convert.ToDateTime(entry.Element(atomNs + "updated").Value).ToLocalTime(),
                Status = context.BuildStatus,
                Project =
                    new CIProject {ProjectName = context.ProjectName, SystemId = context.ProjectSystemId}
            };
            build.SystemId = GenerateSystemId(build);
            return build;
        }

        #endregion

        private static BuildContext ParseBuildTitle(string titleElement)
        {
            const string PROJECTNAME_ID = "projectname";
            const string BUILDNO_ID = "buildno";
            const string BUILDSTATUS_ID = "buildstatus";

            string TITLE_COMPOSITION_REGEX = 
                string.Format(@"Build (?<{0}>.*?) #(?<{1}>\d+) (?<{2}>(has failed|was successful))", 
                PROJECTNAME_ID, BUILDNO_ID, BUILDSTATUS_ID);

            var regex = new Regex(TITLE_COMPOSITION_REGEX);
            Match match = regex.Match(titleElement);

            var context = new BuildContext();
            context.ProjectName = match.Result(string.Format("${{{0}}}", PROJECTNAME_ID));
            context.ProjectSystemId = context.ProjectName;
            context.BuildNumber = Convert.ToInt32(match.Result(string.Format("${{{0}}}", BUILDNO_ID)));

            switch (match.Result(string.Format("${{{0}}}", BUILDSTATUS_ID)))
            {
                case "was successful":
                    context.BuildStatus = BuildStatus.FinishedSuccefully;
                    break;
                case "has failed":
                    context.BuildStatus = BuildStatus.FinishedWithFailure;
                    break;
                default:
                    context.BuildStatus = BuildStatus.Unknown;
                    break;
            }

            return context;
        }

        private static string GenerateSystemId(Build build)
        {
            return build.FinishedTime.Ticks.ToString();
        }
    }

    internal struct BuildContext
    {
        internal string ProjectName { get; set; }
        internal string ProjectSystemId { get; set; }
        internal int BuildNumber { get; set; }
        internal BuildStatus BuildStatus { get; set; }
    }
}