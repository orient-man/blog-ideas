using Autofac;

namespace Infrastructure
{
    class ServiceLocator : IServiceLocator
    {
        private readonly ILifetimeScope _scope;
        private readonly bool _ownsScope;

        public ServiceLocator(ILifetimeScope scope) : this(scope, false)
        {
        }

        private ServiceLocator(ILifetimeScope scope, bool ownsScope)
        {
            _scope = scope;
            _ownsScope = ownsScope;
        }

        public TService Resolve<TService>()
        {
            return _scope.Resolve<TService>();
        }

        public IServiceLocator CreateNew()
        {
            return new ServiceLocator(_scope.BeginLifetimeScope(), true);
        }

        public void Dispose()
        {
            if (_ownsScope)
                _scope.Dispose();
        }
    }
}