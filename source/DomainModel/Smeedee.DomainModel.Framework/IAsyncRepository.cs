using System;
using System.Collections.Generic;

namespace Smeedee.DomainModel.Framework
{
    public interface IAsyncRepository<TDomainModel>
    {
        void BeginGet(Specification<TDomainModel> specification);
        event EventHandler<GetCompletedEventArgs<TDomainModel>> GetCompleted;
    }

    public class GetCompletedEventArgs<TDomainModel> : EventArgs
    {
        public Exception Error { get; protected set; }

        public IEnumerable<TDomainModel> Result { get; protected set; }

        public Specification<TDomainModel> Specification { get; protected set; }

        public GetCompletedEventArgs(IEnumerable<TDomainModel> result, Specification<TDomainModel> specification)
        {
            Result = result;
            Specification = specification;
        }

        public GetCompletedEventArgs(Specification<TDomainModel> specification, Exception error) 
            : this(null, specification)
        {
            Error = error;
        }

    }
}
