using System;
using System.Collections.Generic;
using System.IO;
using Smeedee.Tasks.FileImport.Factories;
using Smeedee.Tasks.FileImport.Services;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.Services;

namespace Smeedee.Tasks.FileImport
{
    public class FileImportTask : TaskBase
    {
        private IManageFileSystems fileSystemService;
        private IKnowTheFileImportQueue fileImportQueueService;
        private IAssembleFileProcessors fileProcessorFactory;
        private string fileImportQueuePath;
        private string fileImportUnrecognizedPath;
        private string fileImportCompletedPath;
        private IEnumerable<string> filesInQueue;
        private static Dictionary<string, IProcessFiles> recognizedFileEndings = new Dictionary<string, IProcessFiles>();

        static FileImportTask()
        {
            recognizedFileEndings.Add(".userdb", null);
        }

        public FileImportTask(
            IManageFileSystems fileSystemService, 
            IKnowTheFileImportQueue fileImportQueueService,
            IAssembleFileProcessors fileProcessorFactory)
        {
            if (fileSystemService == null)
                throw new ArgumentNullException("fileSystemService");

            if (fileImportQueueService == null)
                throw new ArgumentNullException("fileImportQueueService");

            if (fileProcessorFactory == null)
                throw new ArgumentNullException("fileProcessorFactory");

            this.fileSystemService = fileSystemService;
            this.fileImportQueueService = fileImportQueueService;
        }

        public override void Execute()
        {
            fileImportQueuePath = fileImportQueueService.GetDirectoryPath();
            fileImportUnrecognizedPath = fileImportQueueService.GetUnrecognizedDirPath();
            fileImportCompletedPath = fileImportQueueService.GetCompletedDirPath();
            filesInQueue = fileSystemService.GetFiles(fileImportQueuePath);

            foreach (var file in filesInQueue)
            {
                if (!IsFileEndingRecognized(file))
                {
                    fileSystemService.Move(file,
                                           string.Format(@"{0}\{1}", fileImportUnrecognizedPath, FileName(file)));
                }
                else
                {
                    fileSystemService.Move(file,
                        string.Format(@"{0}\{1}", fileImportCompletedPath, FileName(file)));
                }
            }
        }

        private bool IsFileEndingRecognized(string file)
        {
            var fileInfo = new FileInfo(file);
            return recognizedFileEndings.ContainsKey(fileInfo.Extension);
        }

        private string FileName(string file)
        {
            var fileInfo = new FileInfo(file);
            return string.Format("{0}.{1}", fileInfo.Name, fileInfo.Extension.ToLower());
        }
    }
}
