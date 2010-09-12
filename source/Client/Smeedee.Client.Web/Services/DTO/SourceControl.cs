using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Web;

namespace Smeedee.Client.Web.Services.DTO.SourceControl
{
    [DataServiceKey("Revision")]
    public class ChangesetDTO
    {
        public string Revision { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime Time { get; set; }
        public string Comment { get; set; }
    }

    [DataServiceKey("Username")]
    public class AuthorDTO
    {
        public string Username { get; set; }
    }
}