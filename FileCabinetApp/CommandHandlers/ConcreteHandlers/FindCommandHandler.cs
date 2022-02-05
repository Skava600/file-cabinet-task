using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Utils.Iterators;

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
            IEnumerable<FileCabinetRecord> records;
            try
            {
                string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (inputs.Length < 2)
                {
                    Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                        $"Should be name of property and value through white space");
                    return;
                }

                const int nameIndex = 0;
                string propertyName = inputs[nameIndex];
                PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
                var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(propertyName.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"There is no such property as {propertyName}");
                }

                const int valueIndex = 1;
                string propertyValue = inputs[valueIndex].Trim('"');

                records = this.FileCabinetService.FindByProperty(propertyInfo, propertyValue);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Find command failed: {ex.Message}.");
                return;
            }

            this.printer(records);
        }
    }
}
