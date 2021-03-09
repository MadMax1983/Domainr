using Domainr.EventStore.Sql.Data;
using FluentAssertions;
using NUnit.Framework;

namespace Domainr.EventStore.Sql.Tests.IntegrationTests.Data
{
    [TestFixture]
    public sealed class SqlStatementsLoaderTests
    {
        [Test]
        public void WHEN__THEN_()
        {
            // Arrange
            var loader = new SqlStatementsLoader();

            // Act
            loader.LoadScripts();

            // Assert
            loader["Initialize"].Should().NotBeNullOrWhiteSpace();
            loader["GetByAggregateRootId"].Should().NotBeNullOrWhiteSpace();
            loader["Save"].Should().NotBeNullOrWhiteSpace();
        }
    }
}