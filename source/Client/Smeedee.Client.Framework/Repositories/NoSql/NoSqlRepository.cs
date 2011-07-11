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
        private IDownloadStringService downloadStringService;
        

        public NoSqlRepository(IDownloadStringService downloadStringService)
        {
            Guard.Requires<ArgumentNullException>(downloadStringService != null);
            this.downloadStringService = downloadStringService;
        }


        public void GetDatabases(Action<Collection> callback)
        {
            downloadStringService.DownloadAsync(new Uri("../Services/NoSql/Databases.ashx", UriKind.Relative), json =>
            {
                Collection databases = Collection.Parse(json);
                callback(databases);
            });
            
        }
    }
}
