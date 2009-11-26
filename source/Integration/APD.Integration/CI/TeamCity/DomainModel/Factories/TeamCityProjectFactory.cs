using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using APD.DomainModel.CI;
using APD.Integration.Framework.Atom.DomainModel.Factories;


namespace APD.Integration.CI.TeamCity.DomainModel.Factories
{
    public class TeamCityProjectFactory : AtomFeedFactory<CIProject, Build>
    {
        #region AtomFeedFactory<CIProject,Build> Members

        public AtomEntryFactory<Build> EntryFactory { get; set; }
        public CIProject Assemble(XElement xmlFeed)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}