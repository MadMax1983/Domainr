using System.Collections.Generic;
using System.Linq;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
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
        public void GIVEN_serialized_aggregate_root_id_WHEN_instantiating_aggregate_root_THEN_initializes_aggregate_root_with_provided_id_AND_initial_version()
        {
            // Act
            var aggregateRoot = TestAggregateRoot.Create();
            
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

            var aggregateRoot = TestAggregateRoot.Create();

            // Act
            aggregateRoot.ExecuteSomeAction();
            
            // Assert
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

            var aggregateRoot = TestAggregateRoot.Create();
            
            // Act
            aggregateRoot.LoadFromStream(eventsStream);

            // Assert
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

            var aggregateRoot = TestAggregateRoot.Create();

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
            var aggregateRoot = TestAggregateRoot.Create();

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

            var aggregateRoot = TestAggregateRoot.Create();

            aggregateRoot.ExecuteSomeAction();

            // Act
            var action = aggregateRoot.Invoking(ar => ar.CommitChanges());

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

            var aggregateRoot = TestAggregateRoot.Create();

            // Act
            aggregateRoot.ExecuteSomeAction();

            // Assert
            var changesToSave = aggregateRoot.GetUncommittedChanges().ToArray();
            aggregateRoot.CommitChanges();

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

            // aggregateRoot.GetUncommittedChanges().Count
            //     .Should()
            //     .Be(EXPECTED_NUMBER_OF_UNCOMMITTED_CHANGES);
        }

        [Test]
        public void GIVEN_aggregate_root_with_uncommitted_changes_WHEN_commiting_changes_after_concurrency_check_THEN_sets_correct_aggregate_root_version()
        {
            // Arrange
            const int AGGREGATE_ROOT_VERSION_AFTER_CONCURRENCY_CHECK = 1;
            const int EXPECTED_VERSION = 2;

            var aggregateRoot = TestAggregateRoot.Create();

            // Act
            aggregateRoot.CommitChanges();

            aggregateRoot.ExecuteSomeAction();

            // Assert
            var changesToSave = aggregateRoot.GetUncommittedChanges().ToArray();
            aggregateRoot.CommitChanges();

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

            var aggregateRoot = TestAggregateRoot.Create();

            // Act
            var action = aggregateRoot.Invoking(ar => ar.DoSomethingUnhandled());
            
            // Assert
            action
                .Should()
                .NotThrow();

            // aggregateRoot.GetUncommittedChanges().Count
            //     .Should()
            //     .Be(EXPECTED_NUMBER_OF_UNCOMMITED_EVENTS);
        }
    }
}