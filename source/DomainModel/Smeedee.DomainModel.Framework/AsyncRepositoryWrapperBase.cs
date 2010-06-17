using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Smeedee.DomainModel.Framework
{
    public class AsyncRepositoryWrapperBase<TDomainModel> : IRepository<TDomainModel>
    {
        private IAsyncRepository<TDomainModel> asyncRepository;

        private static Dictionary<Specification<TDomainModel>, IEnumerable<TDomainModel>> cache = 
            new Dictionary<Specification<TDomainModel>, IEnumerable<TDomainModel>>();

        private static List<Specification<TDomainModel>> getsInProgress =
            new List<Specification<TDomainModel>>();

        public AsyncRepositoryWrapperBase(IAsyncRepository<TDomainModel> asyncRepository)
        {
            if (asyncRepository == null)
                throw new ArgumentException("The given asyncRepository cannot be null");

            this.asyncRepository = asyncRepository;
            this.asyncRepository.GetCompleted += new EventHandler<GetCompletedEventArgs<TDomainModel>>(asyncRepository_GetCompleted);
        }

        void asyncRepository_GetCompleted(object sender, GetCompletedEventArgs<TDomainModel> e)
        {
            lock (getsInProgress)
                getsInProgress.Remove(e.Specification);

            cache[e.Specification] = e.Result;
        }
        

        #region IRepository<TDomainModel> Members

        public IEnumerable<TDomainModel> Get(Specification<TDomainModel> specification)
        {
            if( ! getsInProgress.Contains(specification) )
            {
                lock( getsInProgress )
                    getsInProgress.Add(specification);

                asyncRepository.BeginGet(specification);
            }
            
            IEnumerable<TDomainModel> result = null;
            cache.TryGetValue(specification, out result);

            return result ?? new List<TDomainModel>();
        }

        #endregion
    }
}
