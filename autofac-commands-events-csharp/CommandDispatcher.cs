namespace Infrastructure
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceLocator _serviceLocator;

        public CommandDispatcher(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public void Dispatch<T>(T command) where T : ICommand
        {
            _serviceLocator.Resolve<ICommandHandler<T>>().Handle(command);
        }
    }
}