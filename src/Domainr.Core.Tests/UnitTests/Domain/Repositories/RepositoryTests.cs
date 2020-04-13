using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domainr.Core.Domain.Repositories;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Tests.TestDoubles;
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
        private readonly Guid _aggregateRootId = Guid.NewGuid();
        private readonly ICollection<Event> _events = new List<Event>();
        private readonly Mock<IConcurrencyResolver> _mockConcurrencyResolver = new Mock<IConcurrencyResolver>();
        private readonly Mock<IEventStore> _mockEventStore = new Mock<IEventStore>();
        private readonly Mock<IEventPublisher> _mockEventsDispatcher = new Mock<IEventPublisher>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockConcurrencyResolver
                .Setup(m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>())).Returns(
                    (Type type, IReadOnlyCollection<Type> types) => type != typeof(TestAggregateRootNumberChanged));

            _mockEventStore.Setup(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<long>())).ReturnsAsync(
                (Guid aggregateRootId, long fromVersion) => _events
                    .Where(e => e.AggregateRootId == aggregateRootId && e.Version > fromVersion).OrderBy(e => e.Version)
                    .ToList());

            _mockEventStore.Setup(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>())).Returns(
                (IReadOnlyCollection<Event> events) =>
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
            _events.Add(new TestAggregateRootCreated(_aggregateRootId));
            _events.Add(new TestAggregateRootNameChanged(_aggregateRootId, AGGREGATE_ROOT_NAME));
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
            _mockEventsDispatcher.Invocations.Clear();
        }

        [Test]
        public async Task GIVEN_aggregate_root_identifier_WHEN_getting_aggregate_root_THEN_returns_aggregate_root_from_event_store_with_its_correct_state()
        {
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object, _mockEventsDispatcher.Object);

            var aggregateRoot = await repository.GetByIdAsync(new TestAggregateRootId(_aggregateRootId.ToString()));

            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once());

            var expectedVersion = _events.Max(e => e.Version);
            Assert.IsNotNull(aggregateRoot);
            Assert.AreEqual(_aggregateRootId, aggregateRoot.Id);
            Assert.AreEqual(expectedVersion, aggregateRoot.Version);
            Assert.AreEqual(AGGREGATE_ROOT_NAME, aggregateRoot.Name);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("896db6df-f698-4bdd-9e34-e05df65ebc79")]
        public void GIVEN_invalid_aggregate_root_identifier_WHEN_getting_aggregate_root_THEN_returns_null(string aggregateRootId)
        {
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object,
                _mockEventsDispatcher.Object);
            TestAggregateRoot aggregateRoot = null;
            Assert.DoesNotThrow(() => aggregateRoot = repository.GetByIdAsync(new Guid(aggregateRootId)).Result);
            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once());
            Assert.IsNull(aggregateRoot);
        }

        [Test]
        public void GIVEN_aggregate_root_concurrent_event_WHEN_saving_aggregate_root_that_has_changed_in_parallel_THEN_throws_ConcurrencyException()
        {
            const int EXPECTED_VERSION = 0;
            const string NEW_AGGREGATE_ROOT_NAME = "New_Aggregate_Root_Name";
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object,
                _mockEventsDispatcher.Object);
            var aggregateRoot = new TestAggregateRoot();
            aggregateRoot.LoadFromStream(_events.ToList());
            aggregateRoot.ChangeName(NEW_AGGREGATE_ROOT_NAME);
            Assert.ThrowsAsync<ConcurrencyException>(() => repository.SaveAsync(aggregateRoot, EXPECTED_VERSION));
            _mockConcurrencyResolver.Verify(
                m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Once());
            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once());
            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Never());
            _mockEventsDispatcher.Verify(m => m.DispatchAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Never());
        }

        [Test]
        public void GIVEN_aggregate_root_not_concurrent_event_WHEN_saving_aggregate_root_that_has_changed_THEN_saves_aggregate_root_uncommitted_event()
        {
            const int EXPECTED_VERSION = 0;
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object,
                _mockEventsDispatcher.Object);
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            Assert.DoesNotThrowAsync(() => repository.SaveAsync(aggregateRoot, aggregateRoot.Version));
            _mockConcurrencyResolver.Verify(
                m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Never());
            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Once());
            _mockEventsDispatcher.Verify(m => m.DispatchAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Once());
            Assert.AreEqual(EXPECTED_VERSION, aggregateRoot.Version);
        }

        [Test]
        public void GIVEN_aggregate_root_not_concurrent_event_WHEN_saving_aggregate_root_has_changed_in_parallel_THEN_saves_aggregate_root_uncommitted_event()
        {
            const int EXPECTED_VERSION = 1;
            const int NEW_AGGREGATE_ROOT_NUMBER = -999;
            var repository = new TestRepository(_mockConcurrencyResolver.Object, _mockEventStore.Object,
                _mockEventsDispatcher.Object);
            var aggregateRoot = new TestAggregateRoot();
            aggregateRoot.LoadFromStream(_events.ToList());
            aggregateRoot.ChangeNumber(NEW_AGGREGATE_ROOT_NUMBER);
            var tempVersion = DEFAULT_AGGREGATE_ROOT_VERSION;
            var newEvent = new TestAggregateRootNumberChanged(_aggregateRootId, AGGREGATE_ROOT_NUMBER);
            while (tempVersion != aggregateRoot.Version + 1)
            {
                newEvent.IncrementVersion(ref tempVersion);
            }

            _events.Add(newEvent);
            Assert.DoesNotThrowAsync(() => repository.SaveAsync(aggregateRoot, EXPECTED_VERSION));
            _mockConcurrencyResolver.Verify(
                m => m.ConflictsWith(It.IsAny<Type>(), It.IsAny<IReadOnlyCollection<Type>>()), Times.Once());
            _mockEventStore.Verify(m => m.GetByAggregateRootIdAsync(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once());
            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Once());
            _mockEventsDispatcher.Verify(m => m.DispatchAsync(It.IsAny<IReadOnlyCollection<Event>>()), Times.Once());
            Assert.AreEqual(EXPECTED_VERSION + 2, aggregateRoot.Version);
            Assert.AreEqual(_events.Max(e => e.Version), aggregateRoot.Version);
        }
    }
}