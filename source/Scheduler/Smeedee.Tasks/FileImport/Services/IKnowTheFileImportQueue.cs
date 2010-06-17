namespace Smeedee.Tasks.FileImport.Services
{
    public interface IKnowTheFileImportQueue
    {
        string GetDirectoryPath();
        string GetCompletedDirPath();
        string GetErrorDirPath();
        string GetUnrecognizedDirPath();
    }
}
