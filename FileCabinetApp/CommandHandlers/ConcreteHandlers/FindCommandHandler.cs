using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Find command handler.
    /// </summary>
    internal class FindCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "find";
        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        /// <param name="printer"> Record printer. </param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Find(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Find(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> foundRecords;
            try
            {
                string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (inputs.Length < 2)
                {
                    Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                        $"Should be name of property and value through white space.");
                    return;
                }

                const int nameIndex = 0;
                string propertyName = inputs[nameIndex];

                const int valueIndex = 1;
                string propertyValue = inputs[valueIndex].Trim('"');

                if (propertyName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = this.FileCabinetService.FindByFirstName(propertyValue);
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = this.FileCabinetService.FindByLastName(propertyValue);
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = this.FileCabinetService.FindByDateOfBirth(propertyValue);
                }
                else
                {
                    throw new InvalidOperationException($"The {propertyName} isn't valid command searching property. Only " +
                        $"'{nameof(FileCabinetRecord.FirstName)}', '{nameof(FileCabinetRecord.LastName)}' and " +
                        $"'{nameof(FileCabinetRecord.DateOfBirth)}' allowed.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            this.printer(foundRecords);
        }
    }
}
