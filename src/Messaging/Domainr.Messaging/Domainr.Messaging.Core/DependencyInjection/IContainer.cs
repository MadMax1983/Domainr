using System;
using System.Collections.Generic;

namespace Domainr.Messaging.Core.DependencyInjection
{
    public interface IContainer
    {
        TService GetService<TService>();

        object GetService(Type serviceType);

        IReadOnlyCollection<TService> GetServices<TService>();

        IReadOnlyCollection<object> GetServices(Type serviceType);
    }
}