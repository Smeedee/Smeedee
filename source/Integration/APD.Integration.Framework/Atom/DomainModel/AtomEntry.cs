using System;
using System.Xml.Linq;


namespace APD.Integration.Framework.Atom.DomainModel
{
    public class AtomEntry
    {
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string Author { get; set; }

        public virtual void Initialize(XElement entry)
        {
            Title = entry.Element("title").Value;
            Updated = DateTime.Parse(entry.Element("updated").Value);
            Author = entry.Element("author").Value;

            InitializeSpecialFields(entry);
        }

        protected virtual void InitializeSpecialFields(XElement entry)
        {
            // Implementation reserved for inheriting classes
            // Temporary solution - all initialization will be extracted to factories
        }
    }

    
}