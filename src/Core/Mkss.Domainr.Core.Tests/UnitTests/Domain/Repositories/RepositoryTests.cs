using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Domainr.Core.EventSourcing.Abstraction;
using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.Domain.Repositories
{
    [TestFixture]
    public sealed class RepositoryTests
    {
        private readonly string _aggregateRootIdValue = Guid.NewGuid().ToString();

        private readonly ICollection<Event> _events = new List<Event>();

        private readonly Mock<IEventStore> _mockEventStore = new Mock<IEventStore>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockEventStore.Setup(m => m.GetByAggregateRootIdAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (string aggregateRootId, long fromVersion, CancellationToken cancellationToken) => _events
                        .Where(e => e.AggregateRootId == aggregateRootId && e.Version > fromVersion)
                        .OrderBy(e => e.Version)
                        .ToList());

            _mockEventStore
                .Setup(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((IReadOnlyCollection<Event> events, string  metadata, CancellationToken cancellationToken) =>
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
            var version = Constants.INITIAL_VERSION;

            _events.Add(new BehaviorExecuted(_aggregateRootIdValue));

            foreach (var @event in _events)
            {
                @event.IncrementVersion(ref version);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _events.Clear();

            _mockEventStore.Invocations.Clear();
        }

        [Test]
        public async Task GIVEN_aggregate_root_identifier_WHEN_getting_aggregate_root_THEN_returns_aggregate_root_in_its_correct_state()
        {
            // Arrange
            var repository = new TestRepository(_mockEventStore.Object);

            // Act
            var aggregateRoot = await repository.GetByIdAsync(_aggregateRootIdValue);

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
            var repository = new TestRepository(_mockEventStore.Object);

            var aggregateRootId = new TestAggregateRootId(Guid.Empty.ToString());

            // Act
            var aggregateRoot = await repository.GetByIdAsync(aggregateRootId.ToString());

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
            var repository = new TestRepository(_mockEventStore.Object);

            // Act
            Func<Task> act = async () => await repository.SaveAsync(null);

            // Assert
            act.Should()
               .Throw<AggregateRootNullException>();

            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void GIVEN_unchanged_aggregate_root_WHEN_attempting_to_save_THEN_does_not_throw_any_exception()
        {
            // Arrange
            var repository = new TestRepository(_mockEventStore.Object);

            var aggregateRoot = new TestAggregateRoot();

            aggregateRoot.LoadFromStream(_events.ToList());

            // Act
            Func<Task> act = async () => await repository.SaveAsync(aggregateRoot);

            // Assert
            act.Should()
                .NotThrow<Exception>();

            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        }
        
        [Test]
        [AutoData]
        public async Task GIVEN_changed_aggregate_root_WHEN_attempting_to_save_THEN_saves_aggregate_root(string serializedAggregateRootId)
        {
            // Arrange
            var repository = new TestRepository(_mockEventStore.Object);

            var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
            var aggregateRoot = new TestAggregateRoot(aggregateRootId);

            aggregateRoot.ExecuteBehavior();

            // Act
            await repository.SaveAsync(aggregateRoot);

            // Assert
            _mockEventStore.Verify(m => m.SaveAsync(It.IsAny<IReadOnlyCollection<Event>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}