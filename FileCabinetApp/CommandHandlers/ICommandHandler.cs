using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command Handler interface.
    /// </summary>
    internal interface ICommandHandler
    {
        /// <summary>
        /// Sets next command hadler.
        /// </summary>
        /// <param name="commandHandler"> next command handler. </param>
        internal void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="request"> Command request. </param>
        internal void Handle(AppCommandRequest request);
    }
}
