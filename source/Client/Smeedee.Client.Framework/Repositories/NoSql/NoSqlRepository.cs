using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Framework;
using Newtonsoft.Json.Linq;


namespace Smeedee.Client.Framework.Repositories.NoSql
{
    public interface INoSqlRepository
    {
        [Obsolete]
        void GetDatabases(Action<Collection> callback);

        void GetDatabases(Action<Collection> callback, Action<Exception> onError);

        [Obsolete]
        void GetDocuments(string database, string collection, Action<Collection> callback);

        void GetDocuments(string database, string collection, Action<Collection> callback, Action<Exception> onError);
    }

    public class NoSqlRepository : INoSqlRepository
    {
        public static readonly string documentsUrl = "../Services/NoSql/Documents.ashx?database={0}&collection={1}";
        public static readonly string databasesUrl = "../Services/NoSql/Databases.ashx";

        private IDownloadStringService downloadStringService;
        

        public NoSqlRepository(IDownloadStringService downloadStringService)
        {
            Guard.Requires<ArgumentNullException>(downloadStringService != null);
            this.downloadStringService = downloadStringService;
        }


        [Obsolete]
        public void GetDatabases(Action<Collection> callback)
        {
            GetDatabases(callback, null);
        }

        public void GetDatabases(Action<Collection> callback, Action<Exception> onError)
        {
            downloadStringService.DownloadAsync(new Uri(databasesUrl, UriKind.Relative), json =>
            {
                Collection databases = Collection.Parse(json);
                callback(databases);
            },
            exception => { if (onError != null) onError(exception); });
        }

        [Obsolete]
        public void GetDocuments(string database, string collection, Action<Collection> callback)
        {
            GetDocuments(database, collection, callback, null);
        }

        public void GetDocuments(string database, string collection, Action<Collection> callback, Action<Exception> onError)
        {
            downloadStringService.DownloadAsync(Url(database, collection), json =>
            {
                Collection documents = Collection.Parse(json);
                callback(documents);
            },
            exception => { if (onError != null) onError(exception); });
        }

        private Uri Url(string database, string collection)
        {
            return new Uri(
                string.Format(documentsUrl,
                    database,
                    collection),
                UriKind.Relative);
        }

        public static IList<string> GetDatabasesAsList(Collection databases)
        {
            return databases.Documents.Select(doc => doc["Name"].Value<string>()).ToList();
        }

        public static IList<string> GetCollectionsInDatabase(string currentDatabase, Collection databases)
        {
            var list = new List<string>();
            foreach (var database in databases.Documents)
            {
                if (database["Name"].Value<string>().Equals(currentDatabase))
                {
                    if (database["Collections"].Count() == 0) return list;
                    var json = database["Collections"].ToList();
                    list.AddRange(json.Select(doc => doc["Name"].Value<string>()));
                }
            }
            return list;
        }
    }
}
