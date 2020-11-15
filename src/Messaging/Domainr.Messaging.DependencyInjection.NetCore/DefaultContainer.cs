using System;
using System.Collections.Generic;
using System.Linq;
using Domainr.Messaging.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Domainr.Messaging.DependencyInjection.NetCore
{
    public sealed class DefaultContainer
        : IContainer
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IScope CreateScope()
        {
            return new DefaultScope(_serviceProvider.CreateScope(), this);
        }

        public TService GetService<TService>()
        {
            return _serviceProvider.GetService<TService>();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IReadOnlyCollection<TService> GetServices<TService>()
        {
            return _serviceProvider.GetServices<TService>().ToList();
        }

        public IReadOnlyCollection<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType).ToList();
        }
    }
}