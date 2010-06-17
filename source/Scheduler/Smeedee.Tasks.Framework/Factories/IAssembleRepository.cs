using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Tasks.Framework.Factories
{
    /// <summary>
    /// Factory for assembling a Changeset Repository based on a Configuration
    /// </summary>
    public interface IAssembleRepository<TDomainModel>
    {
        IRepository<TDomainModel> Assemble(Configuration configuration);
    }
}
