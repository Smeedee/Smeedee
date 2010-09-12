using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.DomainModel.Framework;
using System.Linq.Expressions;

namespace Smeedee.DomainModel.Config
{
    public class ConfigurationByName : Specification<Configuration>
    {
        public string Name { get; set; }

        public ConfigurationByName() { }

        public ConfigurationByName(string name)
        {
            this.Name = name;
        }

        public override Expression<Func<Configuration, bool>> IsSatisfiedByExpression()
        {
            return c => c.Name == Name;
        }
    }

    public class ConfigurationById : Specification<Configuration>
    {
        public Guid Id { get; set; }

        public ConfigurationById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Configuration, bool>> IsSatisfiedByExpression()
        {
            return c => c.Id.Equals(Id);
        }
    }
}
