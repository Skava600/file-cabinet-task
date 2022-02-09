using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Help command handler.
    /// </summary>
    internal class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static readonly string Command = "help";

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "create", "creates a new record", "The 'create' command creates a record to the service." },
            new string[] { "update", "updates specified properties in found records by some propertie", "The 'update <updating properties> where <searching properties>' command updates speccified properties of found records." },
            new string[] { "insert", "inserts record", "The 'insert (<property names>) values (<property values>)' command inserts record." },
            new string[] { "delete", "deletess record", "The 'remove where <searching property>' command deletes existing record." },
            new string[] { "purge", "Defragmentate data file ", "The 'purge' command removes deleted records from a data file." },
            new string[] { "select", "prints selected properties of records, oprtional fount by concrete properties", "The 'select <selectingproperties> (where <searching properties)' command prints array of records." },
            new string[] { "export", "exports service data into file .csv or .xml", "The 'export <format> <file path>' command exports service data into specified format." },
            new string[] { "import", "imports servcie data from file .csv or .xml", "The 'export <format> <file path>' command imports service data from file with specified format." },
            new string[] { "stat", "prints the count of records", "The 'stat' command prints count of the records in service." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                PrintHelp(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
