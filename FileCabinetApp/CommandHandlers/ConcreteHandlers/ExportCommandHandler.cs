using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Export command handler.
    /// </summary>
    internal class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string CsvString = "csv";
        private const string XmlString = "xml";
        private const string Command = "export";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Export(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Export(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (inputs.Length < 2)
            {
                Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                    $"Should be export format and file path through white space.");
                return;
            }

            const int formatIndex = 0;
            string format = inputs[formatIndex];

            const int pathIndex = 1;
            string filePath = inputs[pathIndex];

            if (File.Exists(filePath))
            {
                char answer;

                do
                {
                    Console.Write($"File is exist - rewrite {filePath}? [Y/n] ");
                    answer = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                }
                while (!char.ToLowerInvariant(answer).Equals('y') && !char.ToLowerInvariant(answer).Equals('n'));

                if (char.ToLowerInvariant(answer).Equals('n'))
                {
                    return;
                }
            }

            try
            {
                if (format.Equals(CsvString, StringComparison.OrdinalIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        this.FileCabinetService.MakeSnapshot().SaveToCsv(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else if (format.Equals(XmlString, StringComparison.OrdinalIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        this.FileCabinetService.MakeSnapshot().SaveToXml(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else
                {
                    Console.WriteLine($"{format} is not correct format, available only {XmlString} and {CsvString}.");
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine($"This command not available for file storage behaviour.");
            }
        }
    }
}
