using Domainr.Core.Domain.Model;

namespace Domainr.Core.Tests.TestDoubles
{
    internal sealed class TestAggregateRootId
        : IAggregateRootId
    {
        private readonly string _value;

        public TestAggregateRootId(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}