using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;

namespace Smeedee.Tasks.Framework.Factories
{
    /// <summary>
    /// Factory for assembling a Changeset Repository based on a Configuration
    /// </summary>
    public interface IAssembleRepository<TDomainModel>
    {
        IRepository<TDomainModel> Assemble(Configuration configuration);
    }

    public interface IAssembleRepositoryForTasks<TDomainModel>
    {
        IRepository<TDomainModel> Assemble(TaskConfiguration configuration);
    }
}
