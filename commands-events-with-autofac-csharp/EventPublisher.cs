using System.Collections.Generic;

namespace Infrastructure
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceLocator _serviceLocator;

        public EventPublisher(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            var handlers = _serviceLocator.Resolve<IEnumerable<IEventHandler<T>>>();
            foreach (var handler in handlers)
                handler.Handle(@event);
        }
    }
}