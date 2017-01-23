using Autofac;
using Castle.DynamicProxy;

namespace Autofac.Wcf.Extensions
{
    public static class AutofacAutomaticProxyExtensions
    {
        public static void RegisterWithProxy<TContract, TProxyCaller>(
            this ContainerBuilder builder)
            where TProxyCaller : IServiceProxyCaller<TContract>, new()
            where TContract : class
        {
            builder.RegisterWithProxy<TContract, TContract, TProxyCaller>();
        }

        public static void RegisterWithProxy<TTarget, TContract, TProxyCaller>(
            this ContainerBuilder builder)
            where TProxyCaller : IServiceProxyCaller<TContract>, new()
            where TContract : class
        {
            builder
                .Register(c => GenerateAutomaticProxy<TTarget, TContract>(new TProxyCaller()))
                .As<TTarget>()
                .SingleInstance();
        }

        private static TTarget GenerateAutomaticProxy<TTarget, TContract>(
            IServiceProxyCaller<TContract> realProxy)
            where TContract : class
        {
            var interceptor = new ProxyInterceptor<TContract>(realProxy);
            return CreateInterfaceProxyWithoutTarget<TTarget>(interceptor);
        }

        private static T CreateInterfaceProxyWithoutTarget<T>(IInterceptor interceptor)
        {
            var generator = new ProxyGenerator();
            return (T)generator.CreateInterfaceProxyWithoutTarget(typeof(T), interceptor);
        }
    }
}