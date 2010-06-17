using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;


namespace APD.DomainModel.Framework
{
    public class GenericRepositoryBase<TDomainModel> : IRepository<TDomainModel>
    {
        private IAsyncRepository<TDomainModel> asyncRepository;
        private int updateInterval;
        private System.Timers.Timer updateTimer = new Timer();
        private bool isGetInProgress = false;
        private IEnumerable<TDomainModel> cache = new List<TDomainModel>();

        public double UpdateInterval
        {
            get { return updateTimer.Interval;}
            set
            {
                updateTimer.Interval = value;
            }
        }

        public GenericRepositoryBase(IAsyncRepository<TDomainModel> asyncRepository)
        {
            if (asyncRepository == null)
                throw new ArgumentException("The given asyncRepository cannot be null");

            this.asyncRepository = asyncRepository;
            this.asyncRepository.GetCompleted += new EventHandler<GetCompletedEventArgs<TDomainModel>>(asyncRepository_GetCompleted);

            updateTimer.Interval = 15000;
            updateTimer.Elapsed += new ElapsedEventHandler(updateTimer_Elapsed);
        }

        void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if( ! isGetInProgress )
            {
                isGetInProgress = true;
                asyncRepository.BeginGet(new AllSpecification<TDomainModel>());
            }
        }

        void asyncRepository_GetCompleted(object sender, GetCompletedEventArgs<TDomainModel> e)
        {
            isGetInProgress = false;
            cache = e.Result;
        }
        

        #region IRepository<TDomainModel> Members

        public IEnumerable<TDomainModel> Get(Specification<TDomainModel> specification)
        {
            if( updateTimer.Enabled == false )
                updateTimer.Start();

            if (specification is AllSpecification<TDomainModel>)
                return cache;
            else
                return cache.Where(item => specification.IsSatisfiedBy(item));
        }

        #endregion
    }
}
