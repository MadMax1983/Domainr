using System;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.EventSourcing.Abstraction
{
    [TestFixture]
    public sealed class EventTests
    {
        private const string AGGREGATE_ROOT_ID = "aggregateRootId";

        [Test]
        public void GIVEN_aggregate_root_identifier_WHEN_creating_an_event_THEN_initializes_event_with_given_aggregate_root_identifier_AND_initial_version()
        {
            // Act
            var @event = new TestEvent1(AGGREGATE_ROOT_ID, true);

            // Assert
            @event.AggregateRootId
                .Should()
                .BeEquivalentTo(AGGREGATE_ROOT_ID);

            @event.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_no_aggregate_root_data_WHEN_creating_event_instance_for_deserialization_THEN_initializes_event_with_null_aggregate_root_identifier_AND_initial_event_version()
        {
            // Act
            var @event = new TestEvent1();

            // Assert
            @event.AggregateRootId
                .Should()
                .BeNullOrWhiteSpace();

            @event.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void GIVEN_empty_aggregate_root_identifier_WHEN_creating_event_THEN_throws_AggregateRootException(string aggregateRootId)
        {
            // Act
            Action action = () => new TestEvent1(aggregateRootId, true);

            // Assert
            action
                .Should()
                .Throw<AggregateRootIdException>()
                .WithMessage(ExceptionResources.EmptyAggregateRootId);
        }

        [Test]
        public void GIVEN_aggregate_root_version_WHEN_incrementing_event_version_THEN_increments_aggregate_root_version()
        {
            // Arrange
            var aggregateRootVersion = Constants.INITIAL_VERSION;

            const long EXPECTED_AGGREGATE_ROOT_VERSION = Constants.INITIAL_VERSION + 1;

            // Act
            var @event = new TestEvent1(AGGREGATE_ROOT_ID, true);

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
        public void GIVEN_invalid_aggregate_root_version_WHEN_incrementing_event_version_THEN_throws_AggregateRootException()
        {
            // Arrange
            long aggregateRootVersion = -2;

            var expectedExceptionMessage = string.Format(ExceptionResources.AggregateRootVersionIsInvalid, Constants.INITIAL_VERSION, aggregateRootVersion);

            // Act
            var @event = new TestEvent1(AGGREGATE_ROOT_ID, true);

            var action = @event.Invoking(_ => @event.IncrementVersion(ref aggregateRootVersion));

            // Assert
            action
                .Should()
                .Throw<AggregateRootVersionException>()
                .WithMessage(expectedExceptionMessage);
        }
    }
}