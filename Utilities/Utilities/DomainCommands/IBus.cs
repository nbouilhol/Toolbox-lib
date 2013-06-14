using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.DomainCommands
{
    public interface IBus
    {
        ICommandResult Execute<TCommand>(TCommand command) where TCommand : ICommand;

        IEnumerable<ValidationResult> Validate<TCommand>(TCommand command) where TCommand : ICommand;

        Task<ICommandResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;

        Task<IEnumerable<ValidationResult>> ValidateAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}