using System;
using System.Collections.Generic;


namespace APD.Integration.Framework.Atom.DomainModel
{
    public class AtomFeed<TEntry> where TEntry : AtomEntry
    {
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public IEnumerable<TEntry> Entries { get; set; }
    }
}