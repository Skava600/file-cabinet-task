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
    internal class ExportCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("export", StringComparison.InvariantCultureIgnoreCase))
            {
                Export(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Export(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

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
                while (!char.ToLower(answer).Equals('y') && !char.ToLower(answer).Equals('n'));

                if (char.ToLower(answer).Equals('n'))
                {
                    return;
                }
            }

            try
            {
                if (format.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        Program.FileCabinetService.MakeSnapshot().SaveToCsv(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else if (format.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        Program.FileCabinetService.MakeSnapshot().SaveToXml(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else
                {
                    Console.WriteLine($"{format} is not correct format, available only xml and csv");
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
        }
    }
}
