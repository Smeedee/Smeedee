using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WidgetMetadataRepositoryService
    {
        [OperationContract]
        public IEnumerable<WidgetInfo> Get(Specification<WidgetInfo> specification )
        {
            return new List<WidgetInfo>();
        }
    }
}
