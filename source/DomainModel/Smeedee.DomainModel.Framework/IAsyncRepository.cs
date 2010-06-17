using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Framework
{
    public interface IAsyncRepository<TDomainModel>
    {
        void BeginGet(Specification<TDomainModel> specification);
        event EventHandler<GetCompletedEventArgs<TDomainModel>> GetCompleted;
    }

    public class GetCompletedEventArgs<TDomainModel> : EventArgs
    {
        private IEnumerable<TDomainModel> result;
        public IEnumerable<TDomainModel> Result
        {
            get { return result; }
        }
        
        private Specification<TDomainModel> specification;
        public Specification<TDomainModel> Specification
        {
            get { return specification; }
        }

        public GetCompletedEventArgs(IEnumerable<TDomainModel> result, Specification<TDomainModel> specification)
        {
            this.result = result;
            this.specification = specification;
        }

    }
}
