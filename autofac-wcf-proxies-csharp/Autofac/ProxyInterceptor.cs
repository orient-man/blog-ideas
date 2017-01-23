using System.Reflection;
using Castle.DynamicProxy;

namespace Autofac.Wcf.Extensions
{
    public class ProxyInterceptor<TService> : IInterceptor where TService : class
    {
        private readonly IServiceProxyCaller<TService> _proxyCaller;

        public ProxyInterceptor(IServiceProxyCaller<TService> proxyCaller)
        {
            _proxyCaller = proxyCaller;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.GetConcreteMethod();
            try
            {
                if (method.ReturnType == typeof(void))
                    _proxyCaller.CallMethod(proxy => method.Invoke(proxy, invocation.Arguments));
                else
                    invocation.ReturnValue =
                        _proxyCaller.CallFunction(
                            proxy => method.Invoke(proxy, invocation.Arguments));
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }
    }
}