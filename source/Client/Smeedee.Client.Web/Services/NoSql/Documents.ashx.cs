using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Smeedee.Client.Web.Services.DTO.NoSql;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.NoSql;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services.NoSql
{
    /// <summary>
    /// Summary description for Documents
    /// </summary>
    public class Documents : IHttpHandler
    {
        private IRepository<NoSqlDatabase> repository = new SqliteNoSqlDatabaseRepository();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.ContentType = "application/json";

            var str = new StringBuilder();
            str.Append("[");

            var docs = GetDocuments(context.Request["database"], context.Request["collection"]);
            for (int i = 0; i < docs.Count(); i++)
            {
                var doc = docs.ElementAt(i);
                str.Append("{\r\n");
                str.AppendFormat("Id: \"{0}\",", doc.Id);
                str.Append(doc.JSON.Substring(0, doc.JSON.Length - 1).Substring(1, doc.JSON.Length - 2));
                str.Append("}\r\n");

                if (i != docs.Count() - 1)
                    str.Append(",\r\n");
            }
            str.Append("]");

            context.Response.Write(str.ToString());
        }

        private IEnumerable<NoSqlDocumentDTO> GetDocuments(string database, string collection)
        {
            var allDocuments = repository.Get(All.ItemsOf<NoSqlDatabase>()).SelectMany(d => d.Collections).SelectMany(g => g.Value.Documents);

            return from document in allDocuments.Where(d => d.Collection.Name == collection && d.Collection.NoSqlDatabase.Name == database)
                   select new NoSqlDocumentDTO
                   {
                       Id = document.Id,
                       JSON = document.JSON,
                       Collection = new NoSqlCollectionDTO { Name = document.Collection.Name }
                   };
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}