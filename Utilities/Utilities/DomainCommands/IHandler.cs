namespace Utilities.DomainCommands
{
    public interface IHandler<in TCommand> where TCommand : ICommand
    {
    }
}