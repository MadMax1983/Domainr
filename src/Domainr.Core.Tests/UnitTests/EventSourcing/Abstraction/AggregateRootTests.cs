using Domainr.Core.Exceptions;
using Domainr.Core.Infrastructure;
using Domainr.Core.Resources;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.EventSourcing.Abstraction
{
    [TestFixture]
    public sealed class AggregateRootTests
    {
        private const string AGGREGATE_ROOT_ID = "aggregateRootId";

        [Test]
        public void GIVEN_no_data_WHEN_instantiating_new_aggregate_root_THEN_initializes_aggregate_root_with_null_id_AND_initial_version()
        {
            // Arrange & Act
            var aggregateRoot = new TestAggregateRoot();

            // Assert
            aggregateRoot.Id
                .Should()
                .BeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_aggregate_root_id_WHEN_instantiating_aggregate_root_THEN_initializes_aggregate_root_with_id_AND_initial_version()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act
            aggregateRoot.InitializeId(AGGREGATE_ROOT_ID, true);

            // Assert
            aggregateRoot.Id
                .Should()
                .NotBeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_aggregate_root_id_WHEN_calling_aggregate_root_method_that_generates_event_that_initializes_aggregate_root_id_multiple_times_THEN_throws_exception()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act

            var action = aggregateRoot.Id.Invoking(aggregateRootId =>
            {
                aggregateRoot.InitializeId(AGGREGATE_ROOT_ID, true);
                aggregateRoot.InitializeId(AGGREGATE_ROOT_ID, true);
            });

            // Assert
            action
                .Should()
                .Throw<AggregateRootIdException>()
                .WithMessage(ExceptionResources.CannotChangeAggregateRootId);

            aggregateRoot.Id
                .Should()
                .NotBeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }

        [Test]
        public void GIVEN_null_aggregate_root_id_WHEN_calling_aggregate_root_method_that_generates_event_that_initializes_aggregate_root_id_THEN_throws_exception()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot();

            // Act

            var action = aggregateRoot.Id.Invoking(aggregateRootId =>
            {
                aggregateRoot.InitializeId(AGGREGATE_ROOT_ID, false);
            });

            // Assert
            action
                .Should()
                .Throw<AggregateRootIdException>()
                .WithMessage(ExceptionResources.NullAggregateRootId);

            aggregateRoot.Id
                .Should()
                .BeNull();

            aggregateRoot.Version
                .Should()
                .Be(Constants.INITIAL_VERSION);
        }
    }
}