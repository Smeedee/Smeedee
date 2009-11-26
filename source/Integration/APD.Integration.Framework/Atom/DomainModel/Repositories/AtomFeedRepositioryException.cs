using System;

namespace APD.Integration.Framework.Atom.DomainModel.Repositories
{
    public class AtomFeedRepositoryException : Exception
    {
        public AtomFeedRepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}