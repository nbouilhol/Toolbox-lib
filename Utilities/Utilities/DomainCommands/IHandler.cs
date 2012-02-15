using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.DomainCommands
{
    public interface IHandler<in TCommand> where TCommand : ICommand { }
}
