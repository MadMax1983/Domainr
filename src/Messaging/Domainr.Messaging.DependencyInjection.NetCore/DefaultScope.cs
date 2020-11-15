using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Domainr.Messaging.DependencyInjection.NetCore
{
    internal sealed class DefaultScope
        : IScope
    {
        private readonly IServiceScope _serviceScope;

        public DefaultScope(IServiceScope serviceScope, IContainer container)
        {
            _serviceScope = serviceScope;

            Container = container;
        }

        public IContainer Container { get; }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}