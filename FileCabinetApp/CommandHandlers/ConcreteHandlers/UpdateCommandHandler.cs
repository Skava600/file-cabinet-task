using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.CommandHelper;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    internal class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "update";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Update(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Update(string parameters)
        {
            Regex parametersRegex = new Regex(@"\s*set\s+(?<updatingProperties>.+)(?:\s+where\s+(?<searchingProperties>.+))", RegexOptions.IgnoreCase);

            var match = parametersRegex.Match(parameters);

            var updatingProperties = match.Groups["updatingProperties"].Value;
            var searchingProperties = match.Groups["searchingProperties"].Value;

            try
            {
                if (!match.Success)
                {
                    throw new ArgumentException("Wrong update parameters syntax");
                }

                const string commaSeparatpr = ",";
                const string andSeparator = " and ";
                var updatingPropertiesTuple = CommandParser.ParseUpdateParameters(updatingProperties, commaSeparatpr);
                var searchingPropertiesTuple = CommandParser.ParseUpdateParameters(searchingProperties, andSeparator);

                IEnumerable<FileCabinetRecord> foundRecords = this.FileCabinetService.GetRecords();
                foreach (var property in searchingPropertiesTuple)
                {
                    foundRecords = foundRecords.Intersect(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
                }

                foreach (var record in foundRecords)
                {
                    foreach (var updatingProperty in updatingPropertiesTuple)
                    {
                        if (updatingProperty.Item1.Name.Equals(nameof(record.Id), StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ArgumentException("You can't change id of a record");
                        }

                        var converter = TypeDescriptor.GetConverter(updatingProperty.Item1.PropertyType);
                        var newValue = converter.ConvertFromInvariantString(updatingProperty.Item2);
                        updatingProperty.Item1.SetValue(record, newValue);
                    }

                    var recordData = new RecordData(record);
                    this.FileCabinetService.EditRecord(record.Id, recordData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Updating failed : {ex.Message}.");
            }
        }
    }
}
