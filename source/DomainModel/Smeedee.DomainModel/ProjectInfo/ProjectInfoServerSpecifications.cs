using System;
using System.Linq.Expressions;
using Smeedee.DomainModel.Framework;

namespace Smeedee.DomainModel.ProjectInfo
{
    public class ProjectInfoServerByName : Specification<ProjectInfoServer>
    {
        public string Name { get; set; }

        public ProjectInfoServerByName() { }

        public ProjectInfoServerByName(string name)
        {
            Name = name;
        }

        public override Expression<Func<ProjectInfoServer, bool>> IsSatisfiedByExpression()
        {
            return p => p.Name == Name;
        }

        public override bool Equals(object obj)
        {
            var other = (obj as ProjectInfoServerByName);
            return  other != null && other.Name == Name;
        }
    }

    public class ProjectInfoServerByUrl : Specification<ProjectInfoServer>
    {
        public string Url { get; set; }

        public ProjectInfoServerByUrl() { }

        public ProjectInfoServerByUrl(string url)
        {
            Url = url;
        }

        public override Expression<Func<ProjectInfoServer, bool>> IsSatisfiedByExpression()
        {
            return p => p.Url == Url;
        }

        public override bool Equals(object obj)
        {
            var other = (obj as ProjectInfoServerByUrl);
            return other != null && other.Url == Url;
        }
    }
}
