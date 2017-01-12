namespace Infrastructure
{
    public interface ICommandDispatcher
    {
        void Dispatch<T>(T command) where T : ICommand;
    }
}