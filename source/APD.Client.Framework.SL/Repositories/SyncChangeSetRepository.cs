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

using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;


namespace APD.Client.Framework.SL.Repositories
{
    public class SyncChangeSetRepository : AsyncRepositoryWrapperBase<Changeset>
    {
        public SyncChangeSetRepository(IAsyncRepository<Changeset> asyncRepository)
            : base(asyncRepository) { }
    }
}
