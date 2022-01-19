using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Exit command handler.
    /// </summary>
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private Action<bool> exitApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="exitApp"> Action to exit application. </param>
        public ExitCommandHandler(Action<bool> exitApp)
        {
            this.exitApp = exitApp;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Exit(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            this.exitApp(false);
        }
    }
}
