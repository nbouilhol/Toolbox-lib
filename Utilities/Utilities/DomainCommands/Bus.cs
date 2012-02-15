using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.DomainCommands
{
    public partial class Bus : IBus
    {
        private readonly Lazy<TaskFactory<ICommandResult>> handleFactory;
        private readonly Lazy<TaskFactory<IEnumerable<ValidationResult>>> validateFactory;
        private readonly IRegistrationService registrationService;

        public Bus(IRegistrationService registrationService)
        {
            this.registrationService = registrationService;
            this.handleFactory = new Lazy<TaskFactory<ICommandResult>>(() => Task<ICommandResult>.Factory, true);
            this.validateFactory = new Lazy<TaskFactory<IEnumerable<ValidationResult>>>(() => Task<IEnumerable<ValidationResult>>.Factory, true);
        }

        public IEnumerable<ICommandResult> Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.registrationService.GetCommandRegistrations<TCommand>().AsParallel().Select(handler => ExecuteCommand(handler, command)).ToList();
        }

        public IEnumerable<IEnumerable<ValidationResult>> Validate<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.registrationService.GetValidationRegistrations<TCommand>().AsParallel().Select(handler => ExecuteValidation(handler, command)).ToList();
        }

        public IEnumerable<Task<ICommandResult>> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.registrationService.GetCommandRegistrations<TCommand>().AsParallel()
                .Select(handler => handleFactory.Value.StartNew(() => ExecuteCommand(handler, command)));
        }

        public IEnumerable<Task<IEnumerable<ValidationResult>>> ValidateAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.registrationService.GetValidationRegistrations<TCommand>().AsParallel()
                .Select(handler => validateFactory.Value.StartNew(() => ExecuteValidation(handler, command)));
        }

        private static ICommandResult ExecuteCommand<TCommand>(ICommandHandler<TCommand> handler, TCommand command) where TCommand : ICommand
        {
            ICommandResult result = handler.Handle(command);
            if (handler is IDisposable)
                ((IDisposable)handler).Dispose();
            return result;
        }

        private static IEnumerable<ValidationResult> ExecuteValidation<TCommand>(IValidationHandler<TCommand> handler, TCommand command) where TCommand : ICommand
        {
            IEnumerable<ValidationResult> result = handler.Validate(command);
            if (handler is IDisposable)
                ((IDisposable)handler).Dispose();
            return result;
        }
    }
}
