using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.DomainCommands
{
    public class Bus : IBus
    {
        private readonly Lazy<TaskFactory<ICommandResult>> _handleFactory;
        private readonly IRegistrationService _registrationService;
        private readonly Lazy<TaskFactory<IEnumerable<ValidationResult>>> _validateFactory;

        public Bus(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
            _handleFactory = new Lazy<TaskFactory<ICommandResult>>(() => Task<ICommandResult>.Factory, true);
            _validateFactory =
                new Lazy<TaskFactory<IEnumerable<ValidationResult>>>(() => Task<IEnumerable<ValidationResult>>.Factory,
                    true);
        }

        ICommandResult IBus.Execute<TCommand>(TCommand command)
        {
            return Execute(command).FirstOrDefault();
        }

        IEnumerable<ValidationResult> IBus.Validate<TCommand>(TCommand command)
        {
            return Validate(command).FirstOrDefault();
        }

        Task<ICommandResult> IBus.ExecuteAsync<TCommand>(TCommand command)
        {
            return ExecuteAsync(command).FirstOrDefault();
        }

        Task<IEnumerable<ValidationResult>> IBus.ValidateAsync<TCommand>(TCommand command)
        {
            return ValidateAsync(command).FirstOrDefault();
        }

        public IEnumerable<ICommandResult> Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            return
                _registrationService.GetCommandRegistrations<TCommand>()
                    .AsParallel()
                    .Select(handler => ExecuteCommand(handler, command))
                    .ToList();
        }

        public IEnumerable<IEnumerable<ValidationResult>> Validate<TCommand>(TCommand command) where TCommand : ICommand
        {
            return
                _registrationService.GetValidationRegistrations<TCommand>()
                    .AsParallel()
                    .Select(handler => ExecuteValidation(handler, command))
                    .ToList();
        }

        public IEnumerable<Task<ICommandResult>> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            return _registrationService.GetCommandRegistrations<TCommand>().AsParallel()
                .Select(handler => _handleFactory.Value.StartNew(() => ExecuteCommand(handler, command)));
        }

        public IEnumerable<Task<IEnumerable<ValidationResult>>> ValidateAsync<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            return _registrationService.GetValidationRegistrations<TCommand>().AsParallel()
                .Select(handler => _validateFactory.Value.StartNew(() => ExecuteValidation(handler, command)));
        }

        private static ICommandResult ExecuteCommand<TCommand>(ICommandHandler<TCommand> handler, TCommand command)
            where TCommand : ICommand
        {
            ICommandResult result = handler.Handle(command);
            if (handler is IDisposable)
                ((IDisposable) handler).Dispose();
            return result;
        }

        private static IEnumerable<ValidationResult> ExecuteValidation<TCommand>(IValidationHandler<TCommand> handler,
            TCommand command) where TCommand : ICommand
        {
            IEnumerable<ValidationResult> result = handler.Validate(command);
            if (handler is IDisposable)
                ((IDisposable) handler).Dispose();
            return result;
        }
    }
}