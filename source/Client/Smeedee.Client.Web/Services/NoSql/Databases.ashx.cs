using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.NoSql;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services.NoSql
{
    /// <summary>
    /// Summary description for Databases
    /// </summary>
    public class Databases : IHttpHandler
    {
        private IRepository<NoSqlDatabase> repository = new SqliteNoSqlDatabaseRepository();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var sb = new StringBuilder();
            sb.Append("[");

            var databases = GetDatabases();

            for (int i = 0; i < databases.Count(); i++ )
            {
                var database = databases.ElementAt(i);
                sb.Append("{\r\n");
                sb.AppendFormat("\tName: \"{0}\",\r\n", database.Name);
                sb.Append("\tCollections: [\r\n");
                for (int x = 0; x < database.Collections.Count; x++)
                {
                    var collection = database.Collections.ElementAt(x);
                    sb.Append("\t\t{ ");
                    sb.AppendFormat("Name: \"{0}\"", collection.Key);
                    sb.Append("}");
                    if (x != database.Collections.Count - 1)
                        sb.Append(",\r\n");
                }
                sb.Append("\t]\r\n");
                sb.Append("}");

                if (i != databases.Count() - 1)
                    sb.Append(",\r\n");
            }

            sb.Append("]");

            context.Response.Write(sb.ToString());
        }

        private IEnumerable<NoSqlDatabase> GetDatabases()
        {
            return repository.Get(All.ItemsOf<NoSqlDatabase>());
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