using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
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
        [Test]
        [AutoData]
        public void GIVEN_serialized_aggregate_root_id_WHEN_instantiating_aggregate_root_THEN_initializes_aggregate_root_with_provided_id_AND_initial_version(string serializedAggregateRootId)
        {
            // Act
            var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
            var aggregateRoot = new TestAggregateRoot(aggregateRootId);
            
            // Assert
            aggregateRoot.Id
                .Should()
                .NotBeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_stream_WHEN_loading_from_events_stream_THEN_sets_aggregate_root_valid_state(string serializedAggregateRootId)
        {
            // Arrange
            const int EXPECTED_NUMBER_OF_UNCOMMITTED_EVENTS = 0;
            
            var version = Constants.INITIAL_VERSION;

            var @event = new BehaviorExecuted(serializedAggregateRootId);
            @event.IncrementVersion(ref version);

            var eventsStream = new List<Event>
            {
                @event
            };

            var aggregateRoot = new TestAggregateRoot();
            
            // Act
            aggregateRoot.LoadFromStream(eventsStream);

            // Assert
            aggregateRoot.Id.ToString()
                .Should()
                .Be(serializedAggregateRootId);

            aggregateRoot.Version
                .Should()
                .Be(version)
                .And
                .BeGreaterThan(Constants.INITIAL_VERSION);

            aggregateRoot.GetUncommittedChanges().Count
                .Should()
                .Be(EXPECTED_NUMBER_OF_UNCOMMITTED_EVENTS);
        }

        [Test]
        public void GIVEN_no_aggregate_root_stream_WHEN_loading_from_events_stream_THEN_throws_EventStreamNullException()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act
            var act = aggregateRoot.Invoking(ar => ar.LoadFromStream(null));

            // Assert
            act.Should()
               .Throw<EventStreamNullException>();

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
            var act = aggregateRoot.Invoking(ar => ar.LoadFromStream(Array.Empty<Event>()));

            // Assert
            act.Should()
               .Throw<EmptyEventStreamException>();

            aggregateRoot.Id
                .Should()
                .BeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_with_uncommitted_changes_WHEN_commiting_changes_THEN_sets_correct_aggregate_root_version(string serializedAggregateRootId)
        {
            // Arrange
            const int EXPECTED_NUMBER_OF_CHANGES_TO_SAVE = 1;
            const int EXPECTED_VERSION = 0;
            const int EXPECTED_NUMBER_OF_UNCOMMITTED_CHANGES = 0;

            var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
            var aggregateRoot = new TestAggregateRoot(aggregateRootId);

            // Act
            aggregateRoot.ExecuteBehavior();

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

            aggregateRoot.GetUncommittedChanges().Count
                .Should()
                .Be(EXPECTED_NUMBER_OF_UNCOMMITTED_CHANGES);
        }
        
        [Test]
        public void GIVEN_aggregate_root_without_id_WHEN_commiting_changes_THEN_throws_AggregateRootIdException()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act
            var act = aggregateRoot.Invoking(ar => ar.CommitChanges());

            // Assert
            act.Should()
               .Throw<AggregateRootIdException>();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        [AutoData]
        public void GIVEN_event_without_aggregate_root_event_handler_implemented_WHEN_processing_event_within_aggregate_THEN_throws_Exception(string serializedAggregateRootId)
        {
            // Arrange
            var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
            var aggregateRoot = new TestAggregateRoot(aggregateRootId);

            // Act
            var act = aggregateRoot.Invoking(ar => ar.ExecuteUnhandledBehavior());
            
            // Assert
            act.Should()
               .Throw<Exception>();
        }

        [Test]
        [AutoData]
        public void GIVEN_any_aggregate_root_id_WHEN_aggregate_root_is_already_initialized_THEN_throws_AggregateRootIdException(string serializedAggregateRootId)
        {
            // Arrange
            var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
            var aggregateRoot = new TestAggregateRoot(aggregateRootId);
            
            // Act
            var act = aggregateRoot.Invoking(ar => ar.SetId(serializedAggregateRootId));
            
            // Assert
            act.Should()
               .Throw<AggregateRootIdException>();
        }
    }
}