using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;

namespace Smeedee.DomainModel.Corkboard
{
    public class RetrospectiveNoteByDescriptionSpecification : Specification<RetrospectiveNote>
    {
        public RetrospectiveNoteByDescriptionSpecification() {}

        public RetrospectiveNoteByDescriptionSpecification(String description)
        {
            Description = description;
        }

        public string Description { get; set; }

        public override Expression<Func<RetrospectiveNote, bool>> IsSatisfiedByExpression()
        {
            return (r => r.Description == Description );
        }
    }

    public class RetrospectiveNoteByIdSpecification : Specification<RetrospectiveNote>
    {
        public RetrospectiveNoteByIdSpecification() { }

        public RetrospectiveNoteByIdSpecification(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public override Expression<Func<RetrospectiveNote, bool>> IsSatisfiedByExpression()
        {
            return (r => r.Id == Id);
        }
    }

    public class RetrospectivePositiveNoteSpecification : Specification<RetrospectiveNote>
    {
        public override Expression<Func<RetrospectiveNote, bool>> IsSatisfiedByExpression()
        {
            return (r => r.Type == NoteType.Positive);
        }
    }

    public class RetrospectiveNegativeNoteSpecification : Specification<RetrospectiveNote>
    {
        public override Expression<Func<RetrospectiveNote, bool>> IsSatisfiedByExpression()
        {
            return (r => r.Type == NoteType.Negative);
        }
    }
}
