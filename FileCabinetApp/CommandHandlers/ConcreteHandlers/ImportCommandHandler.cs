using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Import command handler.
    /// </summary>
    internal class ImportCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("import", StringComparison.InvariantCultureIgnoreCase))
            {
                Import(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Import(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputs.Length < 2)
            {
                Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                    $"Should be import format and file path through white space.");
                return;
            }

            const int formatIndex = 0;
            string format = inputs[formatIndex];

            const int pathIndex = 1;
            string filePath = inputs[pathIndex];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Import error: file {filePath} is not exist.");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());
                if (format.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    snapshot.LoadFromCsv(reader);
                }
                else if (format.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    snapshot.LoadFromXml(reader);
                }
                else
                {
                    Console.WriteLine($"{format} is not correct format, available only xml and csv");
                }

                Program.FileCabinetService.Restore(snapshot);
                Console.WriteLine($"{snapshot.Records.Count} were imported from {filePath}.");
            }
        }
    }
}
