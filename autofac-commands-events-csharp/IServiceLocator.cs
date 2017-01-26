using System;

namespace Infrastructure
{
    public interface IServiceLocator : IDisposable
    {
        TService Resolve<TService>();

        IServiceLocator CreateNew();
    }
}