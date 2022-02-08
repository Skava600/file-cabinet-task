using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Utils.CommandHelper;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base command handler.
    /// </summary>
    internal class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler? nextHandler;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
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

            var commands = new string[] { "create", "delete", "find", "insert", "purge", "exit", "export", "import", "help", "list", "stat", "update" };
            List<string> similarCommands = new List<string>();

            foreach (var commandLine in commands)
            {
                double distance = LevenshteinDistance.Calculate(command, commandLine);
                double similarityMeasure = 1.0 - (distance / Math.Max(command.Length, commandLine.Length));
                if (similarityMeasure >= 0.5)
                {
                    similarCommands.Add(commandLine);
                }
            }

            if (similarCommands.Count == 1)
            {
                Console.WriteLine($"The most similar commands is {similarCommands[0]}");
            }
            else
            {
                Console.WriteLine($"The most similar commands are");
                foreach (var commandLine in similarCommands)
                {
                    Console.WriteLine($"\t\t{commandLine}");
                }
            }
        }
    }
}
