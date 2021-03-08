using System;
using System.Collections.Generic;
using System.Linq;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.Domain.Model
{
    [TestFixture]
    public sealed class AggregateRootTests
    {
        private const string AGGREGATE_ROOT_ID = "aggregateRootId";

        [Test]
        public void GIVEN_serialized_aggregate_root_id_WHEN_instantiating_aggregate_root_THEN_initializes_aggregate_root_with_id_AND_initial_version()
        {
            // Act
            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);
            
            // Assert
            aggregateRoot.Id
                .Should()
                .NotBeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_aggregate_root_WHEN_getting_uncommitted_changes_THEN_returns_list_of_uncommitted_changes()
        {
            // Arrange
            const int EXPECTED_NO_OF_CHANGES = 1;

            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);

            // Act
            aggregateRoot.ExecuteSomeAction();
            
            var changes = aggregateRoot.GetUncommittedChanges();

            // Assert
            changes
                .Should()
                .NotBeNullOrEmpty();

            changes.Count
                .Should()
                .Be(EXPECTED_NO_OF_CHANGES);
        }

        [Test]
        public void GIVEN_aggregate_root_stream_WHEN_loading_from_events_stream_THEN_sets_aggregate_root_valid_state()
        {
            // Arrange
            const int EXPECTED_NUMBER_OF_UNCOMMITTED_EVENTS = 0;

            var version = Constants.INITIAL_VERSION;

            var testEvent1 = new TestEvent1(AGGREGATE_ROOT_ID, true);
            testEvent1.IncrementVersion(ref version);

            var eventsStream = new List<Event>
            {
                testEvent1
            };

            var aggregateRoot = new TestAggregateRoot();
            
            // Act
            aggregateRoot.LoadFromStream(eventsStream);

            // Assert
            aggregateRoot.GetUncommittedChanges().Count
                .Should()
                .Be(EXPECTED_NUMBER_OF_UNCOMMITTED_EVENTS);

            aggregateRoot.Id.ToString()
                .Should()
                .Be(AGGREGATE_ROOT_ID);

            aggregateRoot.Version
                .Should()
                .Be(version)
                .And
                .BeGreaterThan(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_no_aggregate_root_stream_WHEN_loading_from_events_stream_THEN_throws_EventStreamNullException()
        {
            // Arrange
            const string EVENT_STREAM_ARGUMENT_NAME = "eventStream";

            var aggregateRoot = new TestAggregateRoot();

            // Act
            var action = aggregateRoot.Invoking(ar => ar.LoadFromStream(null));

            // Assert
            action
                .Should()
                .Throw<EventStreamNullException>()
                .WithMessage(EVENT_STREAM_ARGUMENT_NAME);

            aggregateRoot.Id
                .Should()
                .BeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_empty_aggregate_root_stream_WHEN_loading_from_events_stream_THEN_throws_EmptyEventStreamException()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act
            var action = aggregateRoot.Invoking(ar => ar.LoadFromStream(new Event[0]));

            // Assert
            action
                .Should()
                .Throw<EmptyEventStreamException>();

            aggregateRoot.Id
                .Should()
                .BeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_invalid_aggregate_root_version_WHEN_commiting_changes_THEN_throws_AggregateRootVersionException()
        {
            // Arrange
            const int INVALID_AGGREGATE_ROOT_VERSION = -2;

            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);

            aggregateRoot.ExecuteSomeAction();

            // Act
            var action = aggregateRoot.Invoking(ar => ar.CommitChanges(INVALID_AGGREGATE_ROOT_VERSION));

            // Assert
            action
                .Should()
                .Throw<AggregateRootVersionException>();
        }

        [Test]
        public void GIVEN_aggregate_root_with_uncommitted_changes_WHEN_commiting_changes_THEN_sets_correct_aggregate_root_version()
        {
            // Arrange
            const int EXPECTED_NUMBER_OF_CHANGES_TO_SAVE = 1;
            const int EXPECTED_VERSION = 0;
            const int EXPECTED_NUMBER_OF_UNCOMMITTED_CHANGES = 0;

            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);

            // Act
            aggregateRoot.ExecuteSomeAction();

            // Assert
            var changesToSave = aggregateRoot.CommitChanges(aggregateRoot.Version).ToArray();

            for (var i = 0; i < changesToSave.Length; i++)
            {
                changesToSave[i].Version
                    .Should()
                    .Be(i);
            }

            changesToSave.Length
                .Should()
                .Be(EXPECTED_NUMBER_OF_CHANGES_TO_SAVE);

            aggregateRoot.Version
                .Should()
                .Be(EXPECTED_VERSION);

            aggregateRoot.GetUncommittedChanges().Count
                .Should()
                .Be(EXPECTED_NUMBER_OF_UNCOMMITTED_CHANGES);
        }

        [Test]
        public void GIVEN_aggregate_root_with_uncommitted_changes_WHEN_commiting_changes_after_concurrency_check_THEN_sets_correct_aggregate_root_version()
        {
            // Arrange
            const int AGGREGATE_ROOT_VERSION_AFTER_CONCURRENCY_CHECK = 1;
            const int EXPECTED_VERSION = 2;

            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);

            // Act
            aggregateRoot.CommitChanges(aggregateRoot.Version);

            aggregateRoot.ExecuteSomeAction();

            // Assert
            var changesToSave = aggregateRoot.CommitChanges(AGGREGATE_ROOT_VERSION_AFTER_CONCURRENCY_CHECK).ToArray();

            for (var i = 0; i < changesToSave.Length; i++)
            {
                var expectedEventVersion = AGGREGATE_ROOT_VERSION_AFTER_CONCURRENCY_CHECK + 1 + i;

                changesToSave[i].Version
                    .Should()
                    .Be(expectedEventVersion);
            }

            aggregateRoot.Version
                .Should()
                .Be(EXPECTED_VERSION);
        }

        [Test]
        public void GIVEN_event_without_aggregate_root_event_handler_implemented_WHEN_processing_event_within_aggregate_THEN_does_not_throw_Exception()
        {
            // Arrange
            const int EXPECTED_NUMBER_OF_UNCOMMITED_EVENTS = 1;

            var aggregateRoot = new TestAggregateRoot(AGGREGATE_ROOT_ID);

            // Act
            var action = aggregateRoot.Invoking(ar => ar.DoSomethingUnhandled());
            
            // Assert
            action
                .Should()
                .NotThrow();

            aggregateRoot.GetUncommittedChanges().Count
                .Should()
                .Be(EXPECTED_NUMBER_OF_UNCOMMITED_EVENTS);
        }
    }
}