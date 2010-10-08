using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Smeedee.DomainModel.SourceControl
{
    [DataContract(IsReference = true, Name = "ChangesetServer", Namespace = "Smeedee.DomainModel.SourceControl")]
    public class ChangesetServer
    {
        [DataMember(Order=1)]
        public virtual string Url { get; set; }
        [DataMember(Order = 2)]
        public virtual string Name { get; set; }

        [DataMember(Order = 3)]
        public virtual IList<Changeset> Changesets { get; set; }
        public static ChangesetServer Default = new ChangesetServer("http://www.smeedee.org/", "Default Changeset server");

        public ChangesetServer()
        {
            Changesets = new List<Changeset>();
        }

        public ChangesetServer(string url, string name) 
            : this()
        {
            Url = url;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ChangesetServer;
            return other != null && other.Url == this.Url;
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Url);
        }
    }
}
