using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using Smeedee.Tasks.Script.Services;
using Smeedee.Tasks.Script.Services.Impl;

namespace Smeedee.Tasks.Script
{
    [Task("Script Task",
        Author = "Smeedee Team",
        Description = "Executes a script on the server",
        Version = 1,
        Webpage = "http://smeedee.org")]
    [TaskSetting(1,ScriptName, typeof(string), "")]
    [TaskSetting(2, Args, typeof(string), "")]
    public class ScriptTask : TaskBase
    {
        public static string DefaultSafeDirectoryPathForScripts = Path.Combine(Environment.CurrentDirectory, "..\\", "scripts");
        private const string ScriptName = "Script Name";
        private const string Args = "Args";
        private IProcessService processService;
        private string safeDirectoryPathForScripts;
        private TaskConfiguration taskConfiguration;
        private string filePath;

        static ScriptTask()
        {
            if (!Directory.Exists(DefaultSafeDirectoryPathForScripts))
                Directory.CreateDirectory(DefaultSafeDirectoryPathForScripts);
        }

        public ScriptTask(
            TaskConfiguration taskConfiguration) :
            this(DefaultSafeDirectoryPathForScripts, taskConfiguration, new ProcessService())
        {
            Interval = new TimeSpan(0,5,0);   
        }

        public ScriptTask(
            string safeDirectoryPathForScripts,
            TaskConfiguration taskConfiguration,
            IProcessService processService)
        {
            Guard.Requires<ArgumentNullException>(safeDirectoryPathForScripts != null);
            Guard.Requires<ArgumentException>(
                Directory.Exists(safeDirectoryPathForScripts) == true,
                "Path specified for SafeDirectoryPathForScripts does not exist");

            Guard.Requires<ArgumentNullException>(taskConfiguration != null);
            Guard.Requires<TaskConfigurationException>(taskConfiguration.EntryExists(ScriptName) == true && taskConfiguration.Entries.Single(c => c.Name == ScriptName).Value != "");
            Guard.Requires<TaskConfigurationException>(
                taskConfiguration.ReadEntryValue(ScriptName) != null && 
                taskConfiguration.ReadEntryValue(ScriptName) != string.Empty);

            filePath = Path.Combine(safeDirectoryPathForScripts, taskConfiguration.ReadEntryValue(ScriptName).ToString());
            Guard.Requires<FileNotFoundException>(File.Exists(filePath) == true);

            Guard.Requires<ArgumentNullException>(processService != null);

            this.taskConfiguration = taskConfiguration;
            this.safeDirectoryPathForScripts = safeDirectoryPathForScripts;
            this.processService = processService;
        }

        public override void Execute()
        {
            Console.WriteLine(Path.Combine(safeDirectoryPathForScripts, taskConfiguration.ReadEntryValue(ScriptName).ToString()));
            processService.Start(
                safeDirectoryPathForScripts,
                taskConfiguration.ReadEntryValue(ScriptName).ToString(),
                string.Format("\"{0}\" {1}", 
                    safeDirectoryPathForScripts,
                    taskConfiguration.ReadEntryValue(Args).ToString()));
        }
    }
}
