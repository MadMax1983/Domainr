using System;
using AutoFixture.NUnit3;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.EventSourcing.Abstraction
{
    [TestFixture]
    public sealed class EventTests
    {
        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_id_WHEN_creating_an_event_THEN_initializes_event_with_given_aggregate_root_id_AND_initial_version(string aggregateRootId)
        {
            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            // Assert
            @event.AggregateRootId
                .Should()
                .BeEquivalentTo(aggregateRootId);

            @event.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void GIVEN_empty_aggregate_root_id_WHEN_creating_event_THEN_throws_AggregateRootException(string aggregateRootId)
        {
            // Act
            Action act = () => new BehaviorExecuted(aggregateRootId);

            // Assert
            act.Should()
               .Throw<AggregateRootIdException>();
        }

        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_version_WHEN_incrementing_event_version_THEN_increments_aggregate_root_version(string aggregateRootId)
        {
            // Arrange
            var aggregateRootVersion = Constants.INITIAL_VERSION;

            const long EXPECTED_AGGREGATE_ROOT_VERSION = Constants.INITIAL_VERSION + 1;

            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            @event.IncrementVersion(ref aggregateRootVersion);

            // Assert
            aggregateRootVersion
                .Should()
                .Be(EXPECTED_AGGREGATE_ROOT_VERSION);

            @event.Version
                .Should()
                .Be(EXPECTED_AGGREGATE_ROOT_VERSION);
        }

        [Test]
        [AutoData]
        public void GIVEN_invalid_aggregate_root_version_WHEN_incrementing_event_version_THEN_throws_AggregateRootException(string aggregateRootId)
        {
            // Arrange
            long aggregateRootVersion = -2;

            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            var action = @event.Invoking(_ => @event.IncrementVersion(ref aggregateRootVersion));

            // Assert
            action
                .Should()
                .Throw<AggregateRootVersionException>();
        }
    }
}