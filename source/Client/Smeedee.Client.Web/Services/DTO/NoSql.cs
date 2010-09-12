using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Smeedee.Client.Web.Services.DTO.NoSql
{
    [DataServiceKey("Name")]
    [DataContract(Name = "NoSqlDatabase", IsReference = true)]
    public class NoSqlDatabaseDTO
    {
        [DataMember]
        public string Name { get; set; }
        public IQueryable<NoSqlCollectionDTO> Collections { get; set; }
    }

    [DataServiceKey("Name")]
    [DataContract(Name = "Collection", IsReference = true)]
    public class NoSqlCollectionDTO
    {
        [DataMember]
        public string Name { get; set; }
        public NoSqlDatabaseDTO Database { get; set; }
        public IQueryable<NoSqlDocumentDTO> Documents { get; set; }
    }

    [DataServiceKey("Id")]
    [DataContract(Name = "Document")]
    public class NoSqlDocumentDTO
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string JSON { get; set; }
        [DataMember]
        public NoSqlCollectionDTO Collection { get; set; }
    }
}