using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    internal interface ICommandHandler
    {
        internal void SetNext(ICommandHandler commandHandler);

        internal void Handle(AppCommandRequest request);
    }
}
