using Smeedee.Tasks.FileImport.Services;

namespace Smeedee.Tasks.FileImport.Factories
{
    public interface IAssembleFileProcessors
    {
        IProcessFiles Assemble(string fileExtension);
    }
}
