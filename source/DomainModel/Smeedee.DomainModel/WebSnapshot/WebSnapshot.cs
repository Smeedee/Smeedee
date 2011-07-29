using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    [DataContract(IsReference = true, Name = "WebSnapshot", Namespace = "Smeedee.DomainModel.WebSnapshot")]
    public class WebSnapshot
    {
        [DataMember(Order = 1)]
        public virtual string Name { get; set; }
        [DataMember(Order = 2)]
        public virtual string PictureFilePath { get; set; }
        [DataMember(Order = 3)]
        public virtual int PictureHeight { get; set; }
        [DataMember(Order = 4)]
        public virtual int PictureWidth { get; set; }
        [DataMember(Order = 5)]
        public virtual string Timestamp { get; set; }
    }
}
