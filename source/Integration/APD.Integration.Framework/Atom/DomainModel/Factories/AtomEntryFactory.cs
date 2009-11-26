using System.Xml.Linq;

namespace APD.Integration.Framework.Atom.DomainModel.Factories
{
    public interface AtomEntryFactory<TEntry>
    {
        TEntry Assemble(XElement entry);
    }
}