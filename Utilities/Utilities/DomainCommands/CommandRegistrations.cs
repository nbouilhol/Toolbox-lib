using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.DomainCommands
{
    public class CommandRegistrations : IRegistrationService, IDisposable
    {
        private readonly ConcurrentDictionary<Type, ICollection<ICommandHandler<dynamic>>> commandHandlers;
        private readonly ConcurrentDictionary<Type, ICollection<IValidationHandler<dynamic>>> validationHandlers;
        private readonly Func<Type, bool> filterByICommandHandler;
        private readonly Func<Type, bool> filterByIValidationHandler;

        public CommandRegistrations()
        {
            commandHandlers = new ConcurrentDictionary<Type, ICollection<ICommandHandler<dynamic>>>();
            validationHandlers = new ConcurrentDictionary<Type, ICollection<IValidationHandler<dynamic>>>();
            filterByICommandHandler = i => FilterByType(i, typeof(ICommandHandler<>));
            filterByIValidationHandler = i => FilterByType(i, typeof(IValidationHandler<>));
        }

        public CommandRegistrations Add<THandler>(THandler handler)
        {
            IEnumerable<Type> interfaceTypes = GetHandlerInterfaces<THandler>(handler);

            if (interfaceTypes.Any(filterByICommandHandler))
                commandHandlers.TryAdd(interfaceTypes.FirstOrDefault(filterByICommandHandler), new List<ICommandHandler<dynamic>> { handler as ICommandHandler<dynamic> });
            else if (interfaceTypes.Any(filterByIValidationHandler))
                validationHandlers.TryAdd(interfaceTypes.FirstOrDefault(filterByIValidationHandler), new List<IValidationHandler<dynamic>> { handler as IValidationHandler<dynamic> });

            return this;
        }

        private static bool FilterByType(Type type, Type genericInterface)
        {
            return type.GetGenericTypeDefinition() == genericInterface;
        }

        private static IEnumerable<Type> GetHandlerInterfaces<THandler>(THandler handler)
        {
            return handler.GetType().GetInterfaces()
                .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || i.GetGenericTypeDefinition() == typeof(IValidationHandler<>)));
        }

        public IEnumerable<ICommandHandler<TCommand>> GetCommandRegistrations<TCommand>() where TCommand : ICommand
        {
            ICollection<ICommandHandler<dynamic>> handler;
            if (commandHandlers.TryGetValue(typeof(TCommand), out handler))
                return handler as IEnumerable<ICommandHandler<TCommand>>;
            return null;
        }

        public IEnumerable<IValidationHandler<TCommand>> GetValidationRegistrations<TCommand>() where TCommand : ICommand
        {
            ICollection<IValidationHandler<dynamic>> handler;
            if (validationHandlers.TryGetValue(typeof(TCommand), out handler))
                return handler as IEnumerable<IValidationHandler<TCommand>>;
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (this.commandHandlers != null) this.commandHandlers.Clear();
            if (disposing) if (this.validationHandlers != null) this.validationHandlers.Clear();
        }

        ~CommandRegistrations()
        {
            Dispose(false);
        }
    }
}