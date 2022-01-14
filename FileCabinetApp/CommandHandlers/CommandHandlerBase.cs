using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    internal class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler? nextHandler;

        public virtual void Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
            else
            {
                PrintMissedCommandInfo(request.Command);
            }
        }

        void ICommandHandler.SetNext(ICommandHandler commandHandler)
        {
            if (commandHandler != null)
            {
                this.nextHandler = commandHandler;
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
