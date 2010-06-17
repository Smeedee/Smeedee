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
        private string name;

        public ConfigurationByName(string name)
        {
            this.name = name;
        }

        public override Expression<Func<Configuration, bool>> IsSatisfiedByExpression()
        {
            return c => c.Name == name;
        }
    }
}
