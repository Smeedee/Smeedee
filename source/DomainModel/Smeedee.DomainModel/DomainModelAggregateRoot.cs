using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel
{
    public abstract class DomainModelAggregateRoot
    {
        public Guid Id { get; set; }
    }
}
