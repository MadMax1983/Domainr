using System;
using Domainr.Core.Domain.Model;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.Domain.Model
{
    [TestFixture]
    public sealed class AggregateRootVersionValidatorTests
    {
        [Test]
        public void GIVEN_valid_aggregate_root_version_WHEN_validating_aggregate_root_version_THEN_does_not_throw_exception()
        {
            const long AGGREGATE_ROOT_VERSION = Constants.INITIAL_VERSION - 1;

            // Arrange

            // Act
            Action action = () => AggregateRootVersionValidator.Validate(AGGREGATE_ROOT_VERSION);

            // Assert
            action
                .Should()
                .Throw<AggregateRootVersionException>();
        }

        [TestCase(Constants.INITIAL_VERSION)]
        [TestCase(Constants.INITIAL_VERSION + 1)]
        [TestCase(Constants.INITIAL_VERSION + 2)]
        public void GIVEN_invalid_aggregate_root_version_WHEN_validating_aggregate_root_version_THEN_throws_exception(long aggregateRootVersion)
        {
            // Arrange

            // Act
            Action action = () => AggregateRootVersionValidator.Validate(aggregateRootVersion);

            // Assert
            action
                .Should()
                .NotThrow<AggregateRootVersionException>();
        }
    }
}