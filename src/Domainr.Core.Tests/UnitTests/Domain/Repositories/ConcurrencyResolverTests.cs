using System;
using Domainr.Core.Domain.Repositories;
using Domainr.Core.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.Core.Tests.UnitTests.Domain.Repositories
{
    [TestFixture]
    public sealed class ConcurrencyResolverTests
    {
        [Test]
        public void GIVEN_not_registered_event_type_WHEN_checking_for_concurrent_events_THEN_returns_true()
        {
            // Arrange
            var concurrencyResolver = new ConcurrencyResolver();

            // Act
            var hasConflict = concurrencyResolver.ConflictsWith(typeof(TestEvent1), new Type[0]);

            // Assert
            hasConflict
                .Should()
                .BeTrue();
        }

        [Test]
        public void GIVEN_conflicted_event_type_WHEN_checking_for_concurrent_events_THEN_returns_true()
        {
            // Arrange
            var concurrencyResolver = new ConcurrencyResolver();

            concurrencyResolver.RegisterConflictList(typeof(TestEvent1), new []
            {
                typeof(TestEvent1)
            });

            // Act
            var hasConflict = concurrencyResolver.ConflictsWith(typeof(TestEvent1), new []
            {
                typeof(TestEvent2),
                typeof(TestEvent1),
                typeof(TestEvent3),
                typeof(TestEvent4)
            });

            // Assert
            hasConflict
                .Should()
                .BeTrue();
        }

        [Test]
        public void GIVEN_not_conflicted_event_type_WHEN_checking_for_concurrent_events_THEN_returns_false()
        {
            // Arrange
            var concurrencyResolver = new ConcurrencyResolver();

            concurrencyResolver.RegisterConflictList(typeof(TestEvent1), new[]
            {
                typeof(TestEvent1)
            });

            // Act
            var hasConflict = concurrencyResolver.ConflictsWith(typeof(TestEvent1), new[]
            {
                typeof(TestEvent2),
                typeof(TestEvent3),
                typeof(TestEvent4)
            });

            // Assert
            hasConflict
                .Should()
                .BeFalse();
        }
    }
}