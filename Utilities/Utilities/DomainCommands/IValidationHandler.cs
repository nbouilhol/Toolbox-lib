using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Utilities.DomainCommands
{
    [InheritedExport]
    public interface IValidationHandler<in TCommand>
    {
        IEnumerable<ValidationResult> Validate(TCommand command);
    }
}