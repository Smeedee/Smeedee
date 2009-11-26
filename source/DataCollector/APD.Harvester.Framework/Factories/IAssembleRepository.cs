using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;


namespace APD.Harvester.SourceControl.Factories
{
    /// <summary>
    /// Factory for assembling a Changeset Repository based on a Configuration
    /// </summary>
    public interface IAssembleRepository<TDomainModel>
    {
        IRepository<TDomainModel> Assemble(Configuration configuration);
    }
}
