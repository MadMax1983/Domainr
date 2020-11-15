using System;

namespace Domainr.Messaging.Core.DependencyInjection
{
    public interface IScope
        : IDisposable
    {
        IContainer Container { get; }
    }
}