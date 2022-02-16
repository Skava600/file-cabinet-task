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
    internal class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string CsvString = "csv";
        private const string XmlString = "xml";
        private const string Command = "import";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Import(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Import(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());
                    if (format.Equals(CsvString, StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromCsv(reader);
                    }
                    else if (format.Equals(XmlString, StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromXml(reader);
                    }
                    else
                    {
                        Console.WriteLine($"{format} is not correct format, available only {CsvString} and {XmlString}");
                    }

                    this.FileCabinetService.Restore(snapshot);
                    Console.WriteLine($"{snapshot.Records.Count} were imported from {filePath}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
