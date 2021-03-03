using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.Domain.Repositories
{
    [TestFixture]
    public sealed class RepositoryTests
    {
        private const long DEFAULT_AGGREGATE_ROOT_VERSION = -1;
        private const string AGGREGATE_ROOT_NAME = "Aggregate_Root_Name";
        private const int AGGREGATE_ROOT_NUMBER = 999;

        private readonly string _aggregateRootIdValue = Guid.NewGuid().ToString();

        private readonly ICollection<Event> _events = new List<Event>();

        private readonly Mock<IConcurrencyResolver> _mockConcurrencyResolver = new Mock<IConcurrencyResolver>();
        private readonly Mock<IEventStore> _mockEventStore = new Mock<IEventStore>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockConcurrencyResolver
                .Setup(m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()))
                .Returns(
                    (Type type, IReadOnlyCollection<Type> types) => types.Contains(type));

            _mockEventStore.Setup(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (string aggregateRootId, long fromVersion, CancellationToken cancellationToken) => _events
                        .Where(e => e.AggregateRootId == aggregateRootId && e.Version > fromVersion)
                        .OrderBy(e => e.Version)
                        .ToList());

            _mockEventStore
                .Setup(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()))
                .Returns((IReadOnlyCollection<Event> events, CancellationToken cancellationToken) =>
                {
                    foreach (var @event in events)
                    {
                        _events.Add(@event);
                    }

                    return Task.FromResult(0);
                });
        }

        [SetUp]
        public void SetUp()
        {
            var version = DEFAULT_AGGREGATE_ROOT_VERSION;

            _events.Add(new TestEvent1(_aggregateRootIdValue, true));

            foreach (var @event in _events)
            {
                @event.IncrementVersion(ref version);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _events.Clear();

            _mockConcurrencyResolver.Invocations.Clear();
            _mockEventStore.Invocations.Clear();
        }

        [Test]
        public async Task GIVEN_aggregate_root_identifier_WHEN_getting_aggregate_root_THEN_returns_aggregate_root_in_its_correct_state()
        {
            // Arrange
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object);

            var aggregateRootId = new TestAggregateRootId(_aggregateRootIdValue);

            // Act
            var aggregateRoot = await repository.GetByIdAsync(aggregateRootId);

            // Assert
            aggregateRoot.Id.ToString()
                .Should()
                .Be(_aggregateRootIdValue);

            aggregateRoot.Version
                .Should()
                .Be(_events.Max(e => e.Version));

            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());

        }

        [Test]
        public async Task GIVEN_invalid_aggregate_root_identifier_WHEN_getting_aggregate_root_THEN_returns_null()
        {
            // Arrange
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object);

            var aggregateRootId = new TestAggregateRootId(Guid.Empty.ToString());

            // Act
            var aggregateRoot = await repository.GetByIdAsync(aggregateRootId);

            // Assert
            aggregateRoot
                .Should()
                .BeNull();

            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void GIVEN_no_aggregate_root_WHEN_attempting_to_save_THEN_throws_AggregateRootNullException()
        {
            // Arrange
            const string EXPECTED_EXCEPTION_MESSAGE = "aggregateRoot";

            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object);

            // Act
            Func<Task> act = async () => await repository.SaveAsync(null);

            // Assert
            act.Should()
                .Throw<AggregateRootNullException>()
                .WithMessage(EXPECTED_EXCEPTION_MESSAGE);

            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void GIVEN_unchanged_aggregate_root_WHEN_attempting_to_save_THEN_throws_AggregateRootException()
        {
            // Arrange
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object);

            var aggregateRoot = new TestAggregateRoot();

            aggregateRoot.LoadFromStream(_events.ToList());

            var expectedExceptionMessage = string.Format(ExceptionResources.NoEventsToCommit, aggregateRoot.Id, aggregateRoot.Version);

            // Act
            Func<Task> act = async () => await repository.SaveAsync(aggregateRoot);

            // Assert
            act.Should()
                .Throw<AggregateRootException>()
                .WithMessage(expectedExceptionMessage);

            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void GIVEN_aggregate_root_concurrent_event_WHEN_saving_aggregate_root_that_has_changed_in_parallel_THEN_throws_ConcurrencyException()
        {
            const long EXPECTED_VERSION = 1;

            PerformParallelTest(
                act =>
                {
                    act.Should()
                        .Throw<ConcurrencyException>();

                    _events.Max(e => e.Version)
                        .Should()
                        .Be(EXPECTED_VERSION);

                    _mockConcurrencyResolver.Verify(m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Once());
                    _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
                    _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()), Times.Never());
                },
                AddTestEvent2);
        }

        [Test]
        public void GIVEN_aggregate_root_not_concurrent_event_WHEN_saving_aggregate_root_that_has_changed_in_parallel_THEN_saves_aggregate_root_uncommitted_event()
        {
            const long EXPECTED_VERSION = 2;

            PerformParallelTest(
                act =>
                {
                    act.Should()
                        .NotThrow<ConcurrencyException>();

                    _events.Max(e => e.Version)
                        .Should()
                        .Be(EXPECTED_VERSION);

                    _mockConcurrencyResolver.Verify(m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Once());
                    _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
                    _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()), Times.Once());
                },
                AddTestEvent1);
        }

        [Test]
        public void GIVEN_changed_aggregate_root_event_WHEN_saving_aggregate_root_THEN_saves_aggregate_root_uncommitted_event()
        {
            const long EXPECTED_VERSION = 1;

            PerformParallelTest(
                act =>
                {
                    act.Should()
                        .NotThrow<ConcurrencyException>();

                    _events.Max(e => e.Version)
                        .Should()
                        .Be(EXPECTED_VERSION);

                    _mockConcurrencyResolver.Verify(m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Never());
                    _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
                    _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<CancellationToken>()), Times.Once());
                });
        }

        private void PerformParallelTest(Action<Func<Task>> assert, Action<string, long> addEvent = null)
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            aggregateRoot.LoadFromStream(_events.ToList());

            var aggregateRootVersion = aggregateRoot.Version;

            addEvent?.Invoke(aggregateRoot.Id.ToString(), aggregateRootVersion);

            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object);

            // Act
            aggregateRoot.ExecuteSomeAction();

            async Task Act() => await repository.SaveAsync(aggregateRoot);

            // Assert
            assert(Act);
        }

        private void AddTestEvent1(string aggregateRootId, long aggregateRootVersion)
        {
            var concurrentEvent = new TestEvent1(aggregateRootId, false);

            AddParallelEvent(concurrentEvent, aggregateRootVersion);
        }

        private void AddTestEvent2(string aggregateRootId, long aggregateRootVersion)
        {
            var concurrentEvent = new TestEvent2(aggregateRootId);

            AddParallelEvent(concurrentEvent, aggregateRootVersion);
        }

        private void AddParallelEvent<TEvent>(TEvent concurrentEvent, long aggregateRootVersion)
            where TEvent : Event
        {
            // Simulates situation when an aggregate root is already loaded into memory
            // and has uncommitted changes but before saving changes,
            // a concurrent event is raised in an event store for this aggregate

            concurrentEvent.IncrementVersion(ref aggregateRootVersion);

            _events.Add(concurrentEvent);
        }
    }
}