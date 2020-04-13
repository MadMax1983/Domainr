﻿using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;

namespace Domainr.Core.Domain.Model
{
    internal static class AggregateRootVersionValidator
    {
        public static void Validate(long aggregateRootVersion)
        {
            if (aggregateRootVersion < Constants.INITIAL_VERSION)
            {
                throw new AggregateRootVersionException(string.Format(ExceptionResources.AggregateRootVersionIsInvalid, Constants.INITIAL_VERSION, aggregateRootVersion));
            }
        }
    }
}