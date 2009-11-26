#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace APD.DomainModel.Framework
{
    public class StaticRepositoryCache<TDomainModel> : RepositoryDecorator<TDomainModel>
    {
        private static Dictionary<Specification<TDomainModel>, IEnumerable<TDomainModel>> cache = new Dictionary<Specification<TDomainModel>, IEnumerable<TDomainModel>>();
        private static Dictionary<Specification<TDomainModel>, DateTime> timestamp =
            new Dictionary<Specification<TDomainModel>, DateTime>();
        private static Dictionary<Specification<TDomainModel>, bool> fetching = 
            new Dictionary<Specification<TDomainModel>, bool>();
        private int cacheTime;
       
        public StaticRepositoryCache(IRepository<TDomainModel> decoratedObject, int cacheTime) :
            base(decoratedObject)
        {
            this.cacheTime = cacheTime;
        }

        static StaticRepositoryCache()
        {
        }

        public void Clear()
        {
            cache.Clear();
        }

        public bool flag = false;

        public override IEnumerable<TDomainModel> Get(Specification<TDomainModel> specification)
        {
            lock (cache)
            {
                Console.WriteLine("StaticRepositoryCache.Get() (" + Thread.CurrentThread.ManagedThreadId + ")");

                if (!fetching.ContainsKey(specification))
                    fetching.Add(specification, false);
                if (!timestamp.ContainsKey(specification))
                    timestamp.Add(specification, DateTime.MinValue);
                if (!cache.ContainsKey(specification))
                    cache.Add(specification, null);

                var timesSinceCached = DateTime.Now.Subtract(timestamp[specification]).TotalMilliseconds;

                if (cache[specification] == null || timesSinceCached > cacheTime)
                {
                    fetching[specification] = true;

                    Console.WriteLine("DecoratedRepository.Get()");
                    cache[specification] = base.Get(specification);
                    timestamp[specification] = DateTime.Now;
                    fetching[specification] = false;
                }

                Console.WriteLine("Result: " + cache[specification]);
                return cache[specification];
            }
        }
    }
}