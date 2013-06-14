using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.DomainCommands
{
    public class CommandRegistrations : IRegistrationService, IDisposable
    {
        private readonly ConcurrentDictionary<Type, ICollection<ICommandHandler<dynamic>>> _commandHandlers;
        private readonly Func<Type, bool> _filterByICommandHandler;
        private readonly Func<Type, bool> _filterByIValidationHandler;
        private readonly ConcurrentDictionary<Type, ICollection<IValidationHandler<dynamic>>> _validationHandlers;

        public CommandRegistrations()
        {
            _commandHandlers = new ConcurrentDictionary<Type, ICollection<ICommandHandler<dynamic>>>();
            _validationHandlers = new ConcurrentDictionary<Type, ICollection<IValidationHandler<dynamic>>>();
            _filterByICommandHandler = i => FilterByType(i, typeof (ICommandHandler<>));
            _filterByIValidationHandler = i => FilterByType(i, typeof (IValidationHandler<>));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<ICommandHandler<TCommand>> GetCommandRegistrations<TCommand>() where TCommand : ICommand
        {
            ICollection<ICommandHandler<dynamic>> handler;
            if (_commandHandlers.TryGetValue(typeof (TCommand), out handler))
                return handler as IEnumerable<ICommandHandler<TCommand>>;
            return null;
        }

        public IEnumerable<IValidationHandler<TCommand>> GetValidationRegistrations<TCommand>()
            where TCommand : ICommand
        {
            ICollection<IValidationHandler<dynamic>> handler;
            if (_validationHandlers.TryGetValue(typeof (TCommand), out handler))
                return handler as IEnumerable<IValidationHandler<TCommand>>;
            return null;
        }

        public CommandRegistrations Add<THandler>(THandler handler)
        {
            IEnumerable<Type> interfaceTypes = GetHandlerInterfaces(handler);

            IEnumerable<Type> enumerable = interfaceTypes as IList<Type> ?? interfaceTypes.ToList();
            if (enumerable.Any(_filterByICommandHandler))
                _commandHandlers.TryAdd(enumerable.FirstOrDefault(_filterByICommandHandler),
                    new List<ICommandHandler<dynamic>> {handler as ICommandHandler<dynamic>});
            else if (enumerable.Any(_filterByIValidationHandler))
                _validationHandlers.TryAdd(enumerable.FirstOrDefault(_filterByIValidationHandler),
                    new List<IValidationHandler<dynamic>> {handler as IValidationHandler<dynamic>});

            return this;
        }

        private static bool FilterByType(Type type, Type genericInterface)
        {
            return type.GetGenericTypeDefinition() == genericInterface;
        }

        private static IEnumerable<Type> GetHandlerInterfaces<THandler>(THandler handler)
        {
            return handler.GetType().GetInterfaces()
                .Where(
                    i =>
                        i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof (ICommandHandler<>) ||
                         i.GetGenericTypeDefinition() == typeof (IValidationHandler<>)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (_commandHandlers != null) _commandHandlers.Clear();
            if (disposing) if (_validationHandlers != null) _validationHandlers.Clear();
        }

        ~CommandRegistrations()
        {
            Dispose(false);
        }
    }
}