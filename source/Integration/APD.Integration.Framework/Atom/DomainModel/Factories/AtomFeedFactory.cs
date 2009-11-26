using System.Xml.Linq;


namespace APD.Integration.Framework.Atom.DomainModel.Factories
{
    public interface AtomFeedFactory<TFeed, TEntry>
    {
        AtomEntryFactory<TEntry> EntryFactory { get; set; }
        
        TFeed Assemble(XElement xmlFeed);
    }
}