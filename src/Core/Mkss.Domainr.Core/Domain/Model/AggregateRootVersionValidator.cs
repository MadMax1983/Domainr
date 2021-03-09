using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;

namespace Domainr.Core.Domain.Model
{
    internal static class AggregateRootVersionValidator
    {
        public static void Validate(long aggregateRootVersion)
        {
            if (aggregateRootVersion < Constants.INITIAL_VERSION)
            {
                throw new AggregateRootVersionException($"Aggregate root version must be greater or equal to {Constants.INITIAL_VERSION} but was {aggregateRootVersion}.");
            }
        }
    }
}