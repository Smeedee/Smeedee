using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.WebSnapshot
{
    [Task("WebSnapshot Task",
        Author = "Smeedee Team",
        Description = "Takes snapshots of web pages, recommended update interval is no less than 10 minutes as it is an expensive operation",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1, WEBPAGE, typeof(Uri), "http://", "Webpage or image URL")]
    [TaskSetting(2, XPATH, typeof(string), "", "XPath expression to an image tag")]

    public class WebSnapshotTask : TaskBase
    {
        private TaskConfiguration config;

        public WebSnapshotTask(TaskConfiguration config)
        {
            this.config = config;
        }

        public const string WEBPAGE = "Webpage URL";
        public const string XPATH = "Optional XPath expression";

        public override string Name
        {
            get { return "WebSnapshot Task"; }
            set { }
        }

        public override void Execute()
        {

        }
    }
}
