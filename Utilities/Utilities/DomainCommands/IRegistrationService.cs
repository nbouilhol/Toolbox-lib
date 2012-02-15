using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Utilities.DomainCommands
{
    [InheritedExport]
    public interface IRegistrationService
    {
        IEnumerable<ICommandHandler<TCommand>> GetCommandRegistrations<TCommand>() where TCommand : ICommand;
        IEnumerable<IValidationHandler<TCommand>> GetValidationRegistrations<TCommand>() where TCommand : ICommand;
    }
}
