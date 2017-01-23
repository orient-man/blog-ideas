using Autofac;
using Autofac.Wcf.Extensions

namespace Company.App.Composition
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // ...

            builder.RegisterWithProxy<IServiceContract, ServiceVolatileProxyCaller>();

            // IServiceContract is 1 big contract (sad reality...) which inherits from IServiceSubContract
            builder.RegisterWithProxy<IServiceSubContract, IServiceContract, ServiceVolatileProxyCaller>();

            // now it's possible to use both IServiceContract & IServiceSubContract
            // in your classes hiding WCF proxy usage
        }
    }
}
