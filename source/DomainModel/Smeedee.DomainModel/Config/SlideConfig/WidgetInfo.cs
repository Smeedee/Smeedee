using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Config.SlideConfig
{
    public class WidgetInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public byte[] ScreenshotJpg { get; set; }
        public string XAPName { get; set; }
        public string Type { get; set; }
    }

    public static class CommonTags
    {
        public const string SourceControl = "Source control";
        public const string VCS = "VCS";
        public const string ProjectManagement = "Project management";
        public const string ContinuousIntegration = "Continuous integration";

        public const string Fun = "Fun";
        public const string TeamBuilding = "Team building";
        public const string TeamCommunication = "Team communication";

        public const string Agile = "Agile";
        public const string Scrum = "Scrum";

        public const string Charting = "Charting";
    }
}
