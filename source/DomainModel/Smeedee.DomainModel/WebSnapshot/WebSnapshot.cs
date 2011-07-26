using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Smeedee.DomainModel.WebSnapshot
{
    [DataContract]
    public class WebSnapshot
    {
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string PictureFilePath { get; set; }
        [DataMember]
        public virtual int PictureHeight { get; set; }
        [DataMember]
        public virtual int PictureWidth { get; set; }
    }
}
