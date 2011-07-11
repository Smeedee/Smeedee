using System;
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


namespace Smeedee.Client.Framework.Repositories.NoSql
{
    public class NoSqlRepository
    {
        public static readonly string relativeUrl = "../Services/NoSql/Documents.ashx?database={0}&collection={1}";

        private IDownloadStringService downloadStringService;
        

        public NoSqlRepository(IDownloadStringService downloadStringService)
        {
            Guard.Requires<ArgumentNullException>(downloadStringService != null);
            this.downloadStringService = downloadStringService;
        }


        public void GetDatabases(Action<Collection> callback)
        {
            downloadStringService.DownloadAsync(new Uri("../Services/NoSql/Database.ashx", UriKind.Relative), json =>
            {
                Collection databases = Collection.Parse(json);
                callback(databases);
            });
        }

        public void GetDocuments(string database, string collection, Action<Collection> callback)
        {
            downloadStringService.DownloadAsync(Url(database, collection), json =>
            {
                Collection documents = Collection.Parse(json);
                callback(documents);
            });
        }

        private Uri Url(string database, string collection)
        {
            return new Uri(
                string.Format(relativeUrl,
                    database,
                    collection),
                UriKind.Relative);
        }
    }
}
