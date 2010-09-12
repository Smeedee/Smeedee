using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Framework.DSL.Specifications
{
    public static class All
    {
        public static AllSpecification<TModel> ItemsOf<TModel>()
        {
            return new AllSpecification<TModel>();
        }
    }
}
