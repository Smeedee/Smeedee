using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Smeedee.DomainModel.Framework;

namespace Smeedee.DomainModel.Config.SlideConfig
{
    public class SlideConfigurationByTitleSpecification : Specification<SlideConfiguration>
    {
        public string Title { get; protected set; }

        public SlideConfigurationByTitleSpecification() { }

        public SlideConfigurationByTitleSpecification(string title)
        {
            Title = title;
        }

        public override Expression<Func<SlideConfiguration, bool>> IsSatisfiedByExpression()
        {
            return s => s.Title.Equals(Title);
        }
    }

    public class SlideConfigurationByIdSpecification : Specification<SlideConfiguration>
    {
        public Guid Id { get; set; }

        public SlideConfigurationByIdSpecification() { }

        public SlideConfigurationByIdSpecification(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<SlideConfiguration, bool>> IsSatisfiedByExpression()
        {
            return s => s.Id.Equals(Id);
        }
    }
}
