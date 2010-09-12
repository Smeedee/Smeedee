using System;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.Corkboard
{
    [DataContract]
    public class RetrospectiveNote
    {
        private string _description;

        [DataMember]
        public virtual string Description 
        {
            get { return _description ?? ""; } 
            set { _description = value; } 
        }

        [DataMember]
        public virtual NoteType Type { get; set; }

        [DataMember]
        public virtual string Id { get; set; }

        public RetrospectiveNote()
        {
            _description = "";
            Id = Guid.NewGuid().ToString();
        }

        public override bool Equals(object obj)
        {
            RetrospectiveNote otherNote = obj as RetrospectiveNote;
            return otherNote != null && this.Id == otherNote.Id && this.Description == otherNote.Description;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Id,Description).GetHashCode();
        }

    }
}