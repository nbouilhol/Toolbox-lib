
using System.ComponentModel.Composition;

namespace Utilities.DomainCommands
{
    [InheritedExport]
    public interface ICommandHandler<in TCommand>
    {
        ICommandResult Handle(TCommand command);
    }
}