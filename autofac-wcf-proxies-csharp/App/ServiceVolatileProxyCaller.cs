using System;
using System.Linq.Expressions;
using Autofac.Wcf.Extensions

namespace Company.App.Composition
{
    // wraps WCF proxy by company lib
    public class ServiceVolatileProxyCaller : IServiceProxyCaller<IServiceContract>
    {
        public void CallMethod(Expression<Action<IServiceContract>> action)
        {
            using (var proxy = new ServiceVolatileProxy<IServiceContract>())
                proxy.CallUnsafe(action);
        }

        public TResult CallFunction<TResult>(Expression<Func<IServiceContract, TResult>> func)
        {
            using (var proxy = new ServiceVolatileProxy<IServiceContract>())
                return proxy.CallUnsafe(func);
        }
    }
}